using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;

namespace CustomStatisticalReport.Service.ProductionDailyReport
{
    public class DailyProduction
    {
        public static DataTable GetEquipmentCommonInfo(string myOrganizationId, ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select M.* from (
                                select distinct A.EquipmentCommonId as EquipmentId, A.Name, A.DisplayIndex as DisplayIndex, '0' as ParentEquipmentId
                                from equipment_EquipmentCommonInfo A, equipment_EquipmentDetail B, system_Organization C, system_Organization D
                                where A.EquipmentCommonId = B.EquipmentCommonId
                                and B.OrganizationID = C.OrganizationID
                                and B.Enabled = 1
                                and D.OrganizationID = '{0}'
                                and C.LevelCode like D.LevelCode + '%'
                                union all
                                select A.EquipmentId as EquipmentId, A.EquipmentName as Name,99999 as DisplayIndex, A.EquipmentCommonId as ParentEquipmentId
                                from equipment_EquipmentDetail A, system_Organization C, system_Organization D
                                where A.OrganizationID = C.OrganizationID
                                and A.Enabled = 1
                                and D.OrganizationID = '{0}'
                                and C.LevelCode like D.LevelCode + '%') M
                                order by M.ParentEquipmentId, M.DisplayIndex, M.Name";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                DataTable m_EquipmentInfoTable = myDataFactory.Query(m_Sql);
                return m_EquipmentInfoTable;
            }
            catch
            {
                return null;
            }
        }
        public static DataTable GetDailyProductionData(string myOrganizationId, string myDateTime)
        {
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            DataTable m_EquipmentCommonInfoTable = GetEquipmentCommonInfo(myOrganizationId, _dataFactory);
            GetDailyProductionPlanData(ref m_EquipmentCommonInfoTable, myOrganizationId, myDateTime, _dataFactory);
            GetOutputData(ref m_EquipmentCommonInfoTable, myOrganizationId, myDateTime, _dataFactory);
            GetRunTimeData(ref m_EquipmentCommonInfoTable, myOrganizationId, myDateTime, _dataFactory);
            return m_EquipmentCommonInfoTable;
        }
        private static void GetDailyProductionPlanData(ref DataTable myEquipmentCommonInfoTable, string myOrganizationId, string myDateTime, ISqlServerDataFactory myDataFactory)
        {
            ////////初始化列////////////
            if (myEquipmentCommonInfoTable != null)
            {
                myEquipmentCommonInfoTable.Columns.Add("Output_Plan", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("TimeOutput_Plan", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunTime_Plan", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunRate_Plan", typeof(decimal));

                string[] m_MonthArray = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                DateTime m_DateTime = DateTime.Parse(myDateTime);

                string m_Sql = @"Select B.EquipmentId, C.QuotasID, C.Type, B.{2} as Value from tz_Plan A, plan_ProductionYearlyPlan B, plan_ProductionPlan_Template C, system_Organization D, system_Organization E
                                where A.Date = '{1}'
                                and A.OrganizationID = D.OrganizationID
                                and A.PlanType = 'Production'
                                and A.Statue = 1
                                and A.KeyId = B.KeyId
                                and B.QuotasID = C.QuotasID
                                and (C.QuotasID like '%产量%' or C.QuotasID like '%台时产量%' or C.QuotasID like '%运转率%' or C.QuotasID like '%运转时间%')
                                and C.Type in ('MaterialWeight','EquipmentUtilization')
                                and (C.OrganizationID = D.OrganizationID or C.OrganizationID is null)
                                and D.OrganizationID = '{0}'
                                and E.LevelCode like D.LevelCode + '%'";
                m_Sql = string.Format(m_Sql, myOrganizationId, m_DateTime.Year.ToString(), m_MonthArray[m_DateTime.Month - 1]);
                try
                {
                    DataTable m_DailyProductionPlanTable = myDataFactory.Query(m_Sql);

                    for (int i = 0; i < myEquipmentCommonInfoTable.Rows.Count; i++)
                    {
                        myEquipmentCommonInfoTable.Rows[i]["Output_Plan"] = 0.0m;
                        myEquipmentCommonInfoTable.Rows[i]["TimeOutput_Plan"] = 0.0m;
                        myEquipmentCommonInfoTable.Rows[i]["RunTime_Plan"] = 0.0m;
                        myEquipmentCommonInfoTable.Rows[i]["RunRate_Plan"] = 0.0m;
                        if (m_DailyProductionPlanTable != null)
                        {
                            for (int j = 0; j < m_DailyProductionPlanTable.Rows.Count; j++)
                            {
                                string m_QuotasID = m_DailyProductionPlanTable.Rows[j]["QuotasID"] != DBNull.Value ? m_DailyProductionPlanTable.Rows[j]["QuotasID"].ToString() : "";
                                if (m_QuotasID.Contains("产量") && myEquipmentCommonInfoTable.Rows[i]["EquipmentId"].ToString() == m_DailyProductionPlanTable.Rows[j]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[i]["Output_Plan"] = m_DailyProductionPlanTable.Rows[j]["Value"] != DBNull.Value ? m_DailyProductionPlanTable.Rows[j]["Value"] : 0;
                                }
                                else if (m_QuotasID.Contains("台时产量") && myEquipmentCommonInfoTable.Rows[i]["EquipmentId"].ToString() == m_DailyProductionPlanTable.Rows[j]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[i]["TimeOutput_Plan"] = m_DailyProductionPlanTable.Rows[j]["Value"] != DBNull.Value ? m_DailyProductionPlanTable.Rows[j]["Value"] : 0;
                                }
                                else if (m_QuotasID.Contains("运转时间") && myEquipmentCommonInfoTable.Rows[i]["EquipmentId"].ToString() == m_DailyProductionPlanTable.Rows[j]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[i]["RunTime_Plan"] = m_DailyProductionPlanTable.Rows[j]["Value"] != DBNull.Value ? m_DailyProductionPlanTable.Rows[j]["Value"] : 0;
                                }
                                else if (m_QuotasID.Contains("运转率") && myEquipmentCommonInfoTable.Rows[i]["EquipmentId"].ToString() == m_DailyProductionPlanTable.Rows[j]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[i]["RunRate_Plan"] = m_DailyProductionPlanTable.Rows[j]["Value"] != DBNull.Value ? m_DailyProductionPlanTable.Rows[j]["Value"] : 0;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private static void GetOutputData(ref DataTable myEquipmentCommonInfoTable, string myOrganizationId, string myDateTime, ISqlServerDataFactory myDataFactory)
        {
            if (myEquipmentCommonInfoTable != null)
            {
                myEquipmentCommonInfoTable.Columns.Add("Output_Day", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("Output_Month", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("Output_Year", typeof(decimal));
                DateTime m_DateTime = DateTime.Parse(myDateTime);
                string m_Sql = @"Select P.VariableId,P.VariableName, P.Value as ValueDay, Q.Value as ValueMonth, M.Value as ValueYear from 
                                (Select B.VariableId, B.VariableName, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Production B, system_Organization C, system_Organization D
                                where A.OrganizationID = C.OrganizationID
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp >= '{1}'
                                and A.TimeStamp <= '{1}'
                                and A.BalanceId = B.KeyId
                                and B.VariableType = 'EquipmentOutput'
                                and B.ValueType = 'MaterialWeight'
                                and D.OrganizationID = '{0}'
                                and C.LevelCode like D.LevelCode + '%'
                                group by B.VariableId, B.VariableName) P,
                                (Select B.VariableId, B.VariableName, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Production B, system_Organization C, system_Organization D
                                where A.OrganizationID = C.OrganizationID
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp >= '{2}'
                                and A.TimeStamp <= '{1}'
                                and A.BalanceId = B.KeyId
                                and B.VariableType = 'EquipmentOutput'
                                and B.ValueType = 'MaterialWeight'
                                and D.OrganizationID = '{0}'
                                and C.LevelCode like D.LevelCode + '%'
                                group by B.VariableId, B.VariableName) Q,
                                (Select B.VariableId, B.VariableName, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Production B, system_Organization C, system_Organization D
                                where A.OrganizationID = C.OrganizationID
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp >= '{3}'
                                and A.TimeStamp <= '{1}'
                                and A.BalanceId = B.KeyId
                                and B.VariableType = 'EquipmentOutput'
                                and B.ValueType = 'MaterialWeight'
                                and D.OrganizationID = '{0}'
                                and C.LevelCode like D.LevelCode + '%'
                                group by B.VariableId, B.VariableName) M
                                where P.VariableId = Q.VariableId
                                and Q.VariableId = M.VariableId";
                m_Sql = string.Format(m_Sql, myOrganizationId, m_DateTime.ToString("yyyy-MM-dd"), m_DateTime.ToString("yyyy-MM-01"), m_DateTime.ToString("yyyy-01-01"));
                try
                {
                    DataTable m_DailyProductionResultTable = myDataFactory.Query(m_Sql);

                    for (int i = 0; i < myEquipmentCommonInfoTable.Rows.Count; i++)
                    {
                        myEquipmentCommonInfoTable.Rows[i]["Output_Day"] = 0.0m;
                        myEquipmentCommonInfoTable.Rows[i]["Output_Month"] = 0.0m;
                        myEquipmentCommonInfoTable.Rows[i]["Output_Year"] = 0.0m;
                        if (m_DailyProductionResultTable != null)
                        {
                            for (int j = 0; j < m_DailyProductionResultTable.Rows.Count; j++)
                            {
                                string m_EquipmentId = m_DailyProductionResultTable.Rows[j]["VariableId"].ToString();
                                if (myEquipmentCommonInfoTable.Rows[i]["EquipmentId"].ToString() == m_EquipmentId)
                                {
                                    myEquipmentCommonInfoTable.Rows[i]["Output_Day"] = m_DailyProductionResultTable.Rows[j]["ValueDay"] != DBNull.Value ? m_DailyProductionResultTable.Rows[j]["ValueDay"] : 0;
                                    myEquipmentCommonInfoTable.Rows[i]["Output_Month"] = m_DailyProductionResultTable.Rows[j]["ValueMonth"] != DBNull.Value ? m_DailyProductionResultTable.Rows[j]["ValueMonth"] : 0;
                                    myEquipmentCommonInfoTable.Rows[i]["Output_Year"] = m_DailyProductionResultTable.Rows[j]["ValueYear"] != DBNull.Value ? m_DailyProductionResultTable.Rows[j]["ValueYear"] : 0;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private static void GetRunTimeData(ref DataTable myEquipmentCommonInfoTable, string myOrganizationId, string myDateTime, ISqlServerDataFactory myDataFactory)
        {
            DateTime m_DateTime = DateTime.Parse(myDateTime);
            if (myEquipmentCommonInfoTable != null)
            {
                myEquipmentCommonInfoTable.Columns.Add("RunTime_Day", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunRate_Day", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("TimeOutput_Day", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunTime_Month", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunRate_Month", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("TimeOutput_Month", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunTime_Year", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("RunRate_Year", typeof(decimal));
                myEquipmentCommonInfoTable.Columns.Add("TimeOutput_Year", typeof(decimal));

                DataRow[] m_EquipmentCommonRows = myEquipmentCommonInfoTable.Select("ParentEquipmentId = '0'");
                for (int i = 0; i < myEquipmentCommonInfoTable.Rows.Count; i++)
                {
                    myEquipmentCommonInfoTable.Rows[i]["RunTime_Day"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["RunRate_Day"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["TimeOutput_Day"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["RunTime_Month"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["RunRate_Month"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["TimeOutput_Month"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["RunTime_Year"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["RunRate_Year"] = 0.0m;
                    myEquipmentCommonInfoTable.Rows[i]["TimeOutput_Year"] = 0.0m;
                }

                for (int i = 0; i < m_EquipmentCommonRows.Length; i++)
                {
                    /////////////////日统计////////////////
                    DataTable m_EquipmentUtilizationTableDay = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(new string[] { "运转率", "运转时间", "台时产量" }, m_EquipmentCommonRows[i]["EquipmentId"].ToString(), myOrganizationId, m_DateTime.ToString("yyyy-MM-dd"), m_DateTime.ToString("yyyy-MM-dd"), myDataFactory);
                    if (m_EquipmentUtilizationTableDay != null)
                    {
                        for (int m = 0; m < myEquipmentCommonInfoTable.Rows.Count; m++)
                        {
                            for (int n = 0; n < m_EquipmentUtilizationTableDay.Rows.Count; n++)
                            {
                                if (myEquipmentCommonInfoTable.Rows[m]["EquipmentId"].ToString() == m_EquipmentUtilizationTableDay.Rows[n]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[m]["RunRate_Day"] = m_EquipmentUtilizationTableDay.Rows[n]["运转率"] != DBNull.Value ? m_EquipmentUtilizationTableDay.Rows[n]["运转率"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["RunTime_Day"] = m_EquipmentUtilizationTableDay.Rows[n]["运转时间"] != DBNull.Value ? m_EquipmentUtilizationTableDay.Rows[n]["运转时间"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["TimeOutput_Day"] = m_EquipmentUtilizationTableDay.Rows[n]["台时产量"] != DBNull.Value ? m_EquipmentUtilizationTableDay.Rows[n]["台时产量"] : 0;
                                    break;
                                }
                            }
                        }
                    }
                    /////////////////月统计////////////////
                    DataTable m_EquipmentUtilizationTableMonth = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(new string[] { "运转率", "运转时间", "台时产量" }, m_EquipmentCommonRows[i]["EquipmentId"].ToString(), myOrganizationId, m_DateTime.ToString("yyyy-MM-01"), m_DateTime.ToString("yyyy-MM-dd"), myDataFactory);
                    if (m_EquipmentUtilizationTableMonth != null)
                    {
                        for (int m = 0; m < myEquipmentCommonInfoTable.Rows.Count; m++)
                        {

                            for (int n = 0; n < m_EquipmentUtilizationTableMonth.Rows.Count; n++)
                            {
                                if (myEquipmentCommonInfoTable.Rows[m]["EquipmentId"].ToString() == m_EquipmentUtilizationTableMonth.Rows[n]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[m]["RunRate_Month"] = m_EquipmentUtilizationTableMonth.Rows[n]["运转率"] != DBNull.Value ? m_EquipmentUtilizationTableMonth.Rows[n]["运转率"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["RunTime_Month"] = m_EquipmentUtilizationTableMonth.Rows[n]["运转时间"] != DBNull.Value ? m_EquipmentUtilizationTableMonth.Rows[n]["运转时间"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["TimeOutput_Month"] = m_EquipmentUtilizationTableMonth.Rows[n]["台时产量"] != DBNull.Value ? m_EquipmentUtilizationTableMonth.Rows[n]["台时产量"] : 0;
                                    break;
                                }
                            }
                        }
                    }
                    /////////////////年统计////////////////
                    DataTable m_EquipmentUtilizationTableYear = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(new string[] { "运转率", "运转时间", "台时产量" }, m_EquipmentCommonRows[i]["EquipmentId"].ToString(), myOrganizationId, m_DateTime.ToString("yyyy-01-01"), m_DateTime.ToString("yyyy-MM-dd"), myDataFactory);
                    if (m_EquipmentUtilizationTableYear != null)
                    {
                        for (int m = 0; m < myEquipmentCommonInfoTable.Rows.Count; m++)
                        {
                            for (int n = 0; n < m_EquipmentUtilizationTableYear.Rows.Count; n++)
                            {
                                if (myEquipmentCommonInfoTable.Rows[m]["EquipmentId"].ToString() == m_EquipmentUtilizationTableYear.Rows[n]["EquipmentId"].ToString())
                                {
                                    myEquipmentCommonInfoTable.Rows[m]["RunRate_Year"] = m_EquipmentUtilizationTableYear.Rows[n]["运转率"] != DBNull.Value ? m_EquipmentUtilizationTableYear.Rows[n]["运转率"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["RunTime_Year"] = m_EquipmentUtilizationTableYear.Rows[n]["运转时间"] != DBNull.Value ? m_EquipmentUtilizationTableYear.Rows[n]["运转时间"] : 0;
                                    myEquipmentCommonInfoTable.Rows[m]["TimeOutput_Year"] = m_EquipmentUtilizationTableYear.Rows[n]["台时产量"] != DBNull.Value ? m_EquipmentUtilizationTableYear.Rows[n]["台时产量"] : 0;
                                    break;
                                }
                            }
                        }
                    }
                }            
            }
        }
    }
}
