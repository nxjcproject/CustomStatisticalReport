using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;
using CustomStatisticalReport.Model.ProductionDailyReport;

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
            GetCementSpecsData(ref m_EquipmentCommonInfoTable, myOrganizationId, myDateTime, _dataFactory);
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
        private static void GetCementSpecsData(ref DataTable myEquipmentCommonInfoTable, string myOrganizationId, string myDateTime, ISqlServerDataFactory myDataFactory)
        {
            DateTime m_DateTime = DateTime.Parse(myDateTime);

            ///////年统计///////
            DataTable m_CementSpecsChangeTimesTable_Year = GetCementSpecsChangeTimes(myOrganizationId, m_DateTime.ToString("yyyy-01-01 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            DataTable m_MachineRuntimeTable_Year = GetMachineRuntime(myOrganizationId, m_DateTime.ToString("yyyy-01-01 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            if (m_CementSpecsChangeTimesTable_Year != null)
            {
                Dictionary<string, CementEquipmentOutputInfo> m_CementSpecsStatistical_Year = GetCementSpecsStatistical(m_CementSpecsChangeTimesTable_Year);
                GetCementSpecsOutput(ref m_CementSpecsStatistical_Year, myDataFactory);
                GetCementSpecsRuntime(ref m_CementSpecsStatistical_Year, m_MachineRuntimeTable_Year);
                ////////////////////////添加到table中/////////////////////////
                SetCementSpecsValueToDataTable(ref myEquipmentCommonInfoTable, m_CementSpecsStatistical_Year, "year");
            }
            ///////月统计///////
            DataTable m_CementSpecsChangeTimesTable_Month = GetCementSpecsChangeTimes(myOrganizationId, m_DateTime.ToString("yyyy-MM-01 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            DataTable m_MachineRuntimeTable_Month = GetMachineRuntime(myOrganizationId, m_DateTime.ToString("yyyy-MM-01 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            if (m_CementSpecsChangeTimesTable_Month != null)
            {
                Dictionary<string, CementEquipmentOutputInfo> m_CementSpecsStatistical_Month = GetCementSpecsStatistical(m_CementSpecsChangeTimesTable_Month);
                GetCementSpecsOutput(ref m_CementSpecsStatistical_Month, myDataFactory);
                GetCementSpecsRuntime(ref m_CementSpecsStatistical_Month, m_MachineRuntimeTable_Month);
                ////////////////////////添加到table中/////////////////////////
                SetCementSpecsValueToDataTable(ref myEquipmentCommonInfoTable, m_CementSpecsStatistical_Month, "month");
            }
            ///////日统计///////
            DataTable m_CementSpecsChangeTimesTable_Day = GetCementSpecsChangeTimes(myOrganizationId, m_DateTime.ToString("yyyy-MM-dd 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            DataTable m_MachineRuntimeTable_Day = GetMachineRuntime(myOrganizationId, m_DateTime.ToString("yyyy-MM-dd 00:00:00"), m_DateTime.ToString("yyyy-MM-dd 23:59:59"), myDataFactory);
            if (m_CementSpecsChangeTimesTable_Day != null)
            {
                Dictionary<string, CementEquipmentOutputInfo> m_CementSpecsStatistical_Day = GetCementSpecsStatistical(m_CementSpecsChangeTimesTable_Day);
                GetCementSpecsOutput(ref m_CementSpecsStatistical_Day, myDataFactory);
                GetCementSpecsRuntime(ref m_CementSpecsStatistical_Day, m_MachineRuntimeTable_Day);
                ////////////////////////添加到table中/////////////////////////
                SetCementSpecsValueToDataTable(ref myEquipmentCommonInfoTable, m_CementSpecsStatistical_Day, "day");
            }



        }
        private static DataTable GetCementSpecsChangeTimes(string myOrganizationId, string myStartTime, string myEndTime, ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.EquipmentId
                                  ,A.EquipmentName
                                  ,A.EquipmentCommonId
                                  ,A.OrganizationId
                                  ,A.ProductionLineId   
                                  ,B.MaterialColumn     
                                  ,B.MaterialDataBaseName
                                  ,B.MaterialDataTableName
	                              ,B.VariableId
	                              ,(case when C.ChangeStartTime < '{1}' then '{1}' else C.ChangeStartTime end) as StartTime
	                              ,(case when C.ChangeEndTime is null or C.ChangeEndTime > '{2}' then '{2}' else C.ChangeEndTime end) as EndTime
                              FROM equipment_EquipmentDetail A, material_MaterialChangeContrast B, material_MaterialChangeLog C, system_Organization D, system_Organization E
                              where A.[Enabled] = 1
                              and A.EquipmentCommonId = 'CementGrind'
                              and A.OrganizationId = D.OrganizationId
                              and B.VariableType = 'Cement'
                              and B.ContrastID = C.ContrastID
                              and B.OrganizationID = C.OrganizationID
                              and B.valid = C.EventValue
                              and C.ChangeStartTime < '{2}'
                              and (C.ChangeEndTime is null or C.ChangeEndTime > '{1}')
                              and B.VariableId not like '%熟料%'
                              and A.ProductionLineId = B.OrganizationId
                              and E.OrganizationId = '{0}'
                              and D.LevelCode like E.LevelCode + '%'
                              and D.LevelType = 'Factory'
                              order by A.EquipmentId, C.VariableId, C.ChangeStartTime";
            m_Sql = string.Format(m_Sql, myOrganizationId, myStartTime, myEndTime);
            try
            {
                DataTable m_CementSpecsChangeTimesTable = myDataFactory.Query(m_Sql);
                return m_CementSpecsChangeTimesTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetMachineRuntime(string myOrganizationId, string myStartTime, string myEndTime, ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.EquipmentId
                                  ,A.EquipmentName
                                  ,A.OrganizationId
                                  ,A.ProductionLineId
	                              ,(case when B.StartTime < '{1}' then '{1}' else B.StartTime end) as StartTime
	                              ,(case when B.HaltTime is null or B.HaltTime > '{2}' then '{2}' else B.HaltTime end) as HaltTime
                              FROM equipment_EquipmentDetail A, shift_MachineHaltLog B, system_Organization C, system_Organization D
                              where A.[Enabled]  =1
                              and A.EquipmentCommonId = 'CementGrind'
                              and A.OrganizationId = C.OrganizationId
                              and A.EquipmentId = B.EquipmentID
                              and A.ProductionLineId = B.OrganizationId
                              and B.StartTime < '{2}'
                              and (B.HaltTime is null or B.HaltTime > '{1}')
                              and D.OrganizationId = '{0}'
                              and C.LevelCode like D.LevelCode + '%'
                              and C.LevelType = 'Factory'
                              order by A.EquipmentId, B.StartTime";
            m_Sql = string.Format(m_Sql, myOrganizationId, myStartTime, myEndTime);
            try
            {
                DataTable m_MachineRunTimesTable = myDataFactory.Query(m_Sql);
                return m_MachineRunTimesTable;
            }
            catch
            {
                return null;
            }
        }
        private static Dictionary<string, CementEquipmentOutputInfo> GetCementSpecsStatistical(DataTable myCementSpecsChangeYearTimesTable)
        {
            Dictionary<string, CementEquipmentOutputInfo> m_CementSpecsStatistical = new Dictionary<string, CementEquipmentOutputInfo>();
            for (int i = 0; i < myCementSpecsChangeYearTimesTable.Rows.Count; i++)
            {
                string m_EqupimentIdTemp = myCementSpecsChangeYearTimesTable.Rows[i]["EquipmentId"].ToString();
                string m_VariableIdTemp = myCementSpecsChangeYearTimesTable.Rows[i]["VariableId"].ToString();
                if (!m_CementSpecsStatistical.ContainsKey(m_EqupimentIdTemp))
                {
                    CementEquipmentOutputInfo m_CementSpecsTemp = new CementEquipmentOutputInfo();
                    m_CementSpecsTemp.MaterialColumn = myCementSpecsChangeYearTimesTable.Rows[i]["MaterialColumn"].ToString();
                    m_CementSpecsTemp.MaterialDataBaseName = myCementSpecsChangeYearTimesTable.Rows[i]["MaterialDataBaseName"].ToString();
                    m_CementSpecsTemp.MaterialDataTableName = myCementSpecsChangeYearTimesTable.Rows[i]["MaterialDataTableName"].ToString();

                    CementSpecsInfo m_CementSpecsItemTemp = new CementSpecsInfo();
                    m_CementSpecsItemTemp.SpecsName = m_VariableIdTemp;
                    m_CementSpecsItemTemp.StatisitalTimes.Add(new DateTime[]{(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["StartTime"]
                                                  ,(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["EndTime"]});

                    m_CementSpecsTemp.CementSpecsItemInfo.Add(m_VariableIdTemp, m_CementSpecsItemTemp);
                    m_CementSpecsStatistical.Add(m_EqupimentIdTemp, m_CementSpecsTemp);
                }
                else
                {
                    if (!m_CementSpecsStatistical[m_EqupimentIdTemp].CementSpecsItemInfo.ContainsKey(m_VariableIdTemp))
                    {
                        CementSpecsInfo m_CementSpecsItemTemp = new CementSpecsInfo();
                        m_CementSpecsItemTemp.SpecsName = m_VariableIdTemp;
                        m_CementSpecsItemTemp.StatisitalTimes.Add(new DateTime[]{(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["StartTime"]
                                                  ,(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["EndTime"]});

                        m_CementSpecsStatistical[m_EqupimentIdTemp].CementSpecsItemInfo.Add(m_VariableIdTemp, m_CementSpecsItemTemp);
                    }
                    else
                    {

                        m_CementSpecsStatistical[m_EqupimentIdTemp].CementSpecsItemInfo[m_VariableIdTemp].StatisitalTimes.Add(new DateTime[]{(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["StartTime"]
                                                  ,(DateTime)myCementSpecsChangeYearTimesTable.Rows[i]["EndTime"]});
                    }
                }
            }
            return m_CementSpecsStatistical;
        }
        private static void GetCementSpecsOutput(ref Dictionary<string, CementEquipmentOutputInfo> myCementSpecsStatistical, ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = "";
            foreach (string myEquipmentId in myCementSpecsStatistical.Keys)
            {
                foreach (string myCementSpecs in myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo.Keys)
                {
                    string m_SqlTemp = string.Format("Select '{0}' as EquipmentId, '{2}' as VariableId, sum(case when {1} is null then 0 else {1} end) as Value  from {3}", myEquipmentId
                                                , myCementSpecsStatistical[myEquipmentId].MaterialColumn
                                                , myCementSpecs
                                                , myCementSpecsStatistical[myEquipmentId].MaterialDataBaseName + ".dbo." + myCementSpecsStatistical[myEquipmentId].MaterialDataTableName);
                    string m_ConditionTemp = "";
                    for (int i = 0; i < myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo[myCementSpecs].StatisitalTimes.Count; i++)
                    {
                        if (i == 0)
                        {
                            m_ConditionTemp = string.Format(" where (vDate >= '{0}' and vDate <= '{1}')", myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo[myCementSpecs].StatisitalTimes[i][0]
                                                                  , myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo[myCementSpecs].StatisitalTimes[i][1]);
                        }
                        else
                        {
                            m_ConditionTemp = m_ConditionTemp + string.Format(" or (vDate >= '{0}' and vDate <= '{1}')", myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo[myCementSpecs].StatisitalTimes[i][0]
                                                                  , myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo[myCementSpecs].StatisitalTimes[i][1]);
                        }
                    }
                    if (m_ConditionTemp != "")
                    {
                        if (m_Sql == "")
                        {
                            m_Sql = m_SqlTemp + m_ConditionTemp;
                        }
                        else
                        {
                            m_Sql = m_Sql + " union all " + m_SqlTemp + m_ConditionTemp;
                        }
                    }
                }
            }
            try
            {
                DataTable m_CementSpecsOutputTable = myDataFactory.Query(m_Sql);
                if (m_CementSpecsOutputTable != null)
                {
                    for (int i = 0; i < m_CementSpecsOutputTable.Rows.Count; i++)
                    {
                        string m_EquipmentIdTemp = m_CementSpecsOutputTable.Rows[i]["EquipmentId"].ToString();
                        string m_VariableTemp = m_CementSpecsOutputTable.Rows[i]["VariableId"].ToString();
                        myCementSpecsStatistical[m_EquipmentIdTemp].CementSpecsItemInfo[m_VariableTemp].MaterialWeight = m_CementSpecsOutputTable.Rows[i]["Value"] != DBNull.Value ? (decimal)m_CementSpecsOutputTable.Rows[i]["Value"] : 0.0m; 
                    }
                }
            }
            catch
            {
            }
        }
        private static void GetCementSpecsRuntime(ref Dictionary<string, CementEquipmentOutputInfo> myCementSpecsStatistical, DataTable  myMachineRuntimeTable)
        {
            foreach (string myEquipmentId in myCementSpecsStatistical.Keys)
            {
                DataRow[] m_EquipmentRuntimeRow = myMachineRuntimeTable.Select(string.Format("EquipmentId = '{0}'", myEquipmentId));
                foreach (CementSpecsInfo myCementSpecsInfo in myCementSpecsStatistical[myEquipmentId].CementSpecsItemInfo.Values)
                {
                    for (int i = 0; i < m_EquipmentRuntimeRow.Length; i++)
                    {
                        for (int j = 0; j < myCementSpecsInfo.StatisitalTimes.Count; j++)
                        {
                            DateTime m_StartTimeTemp = (DateTime)m_EquipmentRuntimeRow[i]["StartTime"];
                            DateTime m_EndTimeTemp = (DateTime)m_EquipmentRuntimeRow[i]["HaltTime"];
                            if (m_StartTimeTemp < myCementSpecsInfo.StatisitalTimes[j][1] && m_EndTimeTemp > myCementSpecsInfo.StatisitalTimes[j][0])
                            {
                                m_StartTimeTemp = m_StartTimeTemp < myCementSpecsInfo.StatisitalTimes[j][0] ? myCementSpecsInfo.StatisitalTimes[j][0] : m_StartTimeTemp;
                                m_EndTimeTemp = m_EndTimeTemp > myCementSpecsInfo.StatisitalTimes[j][1] ? myCementSpecsInfo.StatisitalTimes[j][1] : m_EndTimeTemp;
                                TimeSpan m_TimeLongTemp = m_EndTimeTemp - m_StartTimeTemp;
                                myCementSpecsInfo.RunTime = myCementSpecsInfo.RunTime + (decimal)m_TimeLongTemp.TotalHours;
                            }
                        }
                    }
                }
            }
        }
        private static void SetCementSpecsValueToDataTable(ref DataTable myEquipmentCommonInfoTable, Dictionary<string, CementEquipmentOutputInfo> myCementSpecsStatistical, string myStatisticlCycle)
        {
            int m_EquipmentIndex = 0;
            foreach (string m_EquipmentId in myCementSpecsStatistical.Keys)
            {
                foreach (string m_VariableId in myCementSpecsStatistical[m_EquipmentId].CementSpecsItemInfo.Keys)
                {
                    CementSpecsInfo m_CementSpecs = myCementSpecsStatistical[m_EquipmentId].CementSpecsItemInfo[m_VariableId];
                    DataRow[] m_ContainsVariableId = myEquipmentCommonInfoTable.Select(string.Format("EquipmentId = '{0}' and ParentEquipmentId = '{1}'", m_VariableId + m_EquipmentIndex.ToString(), m_EquipmentId));
                    if (m_CementSpecs.MaterialWeight != 0 || m_CementSpecs.RunTime != 0)
                    {
                        if (m_ContainsVariableId.Length == 0)
                        {
                            DataRow m_NewDataRow = myEquipmentCommonInfoTable.NewRow();
                            m_NewDataRow["EquipmentId"] = m_VariableId + m_EquipmentIndex.ToString();
                            m_NewDataRow["Name"] = m_CementSpecs.SpecsName;
                            m_NewDataRow["DisplayIndex"] = "99999";
                            m_NewDataRow["ParentEquipmentId"] = m_EquipmentId;
                            if (myStatisticlCycle == "year")
                            {
                                m_NewDataRow["Output_Year"] = m_CementSpecs.MaterialWeight;
                                m_NewDataRow["RunTime_Year"] = m_CementSpecs.RunTime;
                                m_NewDataRow["TimeOutput_Year"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                            }
                            else if (myStatisticlCycle == "month")
                            {
                                m_NewDataRow["Output_Month"] = m_CementSpecs.MaterialWeight;
                                m_NewDataRow["RunTime_Month"] = m_CementSpecs.RunTime;
                                m_NewDataRow["TimeOutput_Month"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                            }
                            else if (myStatisticlCycle == "day")
                            {
                                m_NewDataRow["Output_Day"] = m_CementSpecs.MaterialWeight;
                                m_NewDataRow["RunTime_Day"] = m_CementSpecs.RunTime;
                                m_NewDataRow["TimeOutput_Day"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                            }
                            myEquipmentCommonInfoTable.Rows.Add(m_NewDataRow);
                        }
                        else
                        {
                            for (int i = 0; i < m_ContainsVariableId.Length; i++)
                            {
                                if (myStatisticlCycle == "year")
                                {
                                    m_ContainsVariableId[i]["Output_Year"] = m_CementSpecs.MaterialWeight;
                                    m_ContainsVariableId[i]["RunTime_Year"] = m_CementSpecs.RunTime;
                                    m_ContainsVariableId[i]["TimeOutput_Year"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                                }
                                else if (myStatisticlCycle == "month")
                                {
                                    m_ContainsVariableId[i]["Output_Month"] = m_CementSpecs.MaterialWeight;
                                    m_ContainsVariableId[i]["RunTime_Month"] = m_CementSpecs.RunTime;
                                    m_ContainsVariableId[i]["TimeOutput_Month"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                                }
                                else if (myStatisticlCycle == "day")
                                {
                                    m_ContainsVariableId[i]["Output_Day"] = m_CementSpecs.MaterialWeight;
                                    m_ContainsVariableId[i]["RunTime_Day"] = m_CementSpecs.RunTime;
                                    m_ContainsVariableId[i]["TimeOutput_Day"] = m_CementSpecs.RunTime != 0 ? m_CementSpecs.MaterialWeight / m_CementSpecs.RunTime : 0.0m;
                                }
                            }
                        }
                    }
                }
                m_EquipmentIndex = m_EquipmentIndex + 1;
            }
        }
    }
}
