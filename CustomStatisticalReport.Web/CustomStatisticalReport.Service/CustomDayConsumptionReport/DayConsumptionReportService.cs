using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SqlServerDataAdapter;
using CustomStatisticalReport.Infrastructure.Configuration;

namespace CustomStatisticalReport.Service.CustomDayConsumptionReport
{
    public class DayConsumptionReportService
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        private static readonly AutoSetParameters.AutoGetEnergyConsumption_V1 AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumption_V1(new SqlServerDataAdapter.SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString));
        public static DataTable GetReportData(string myOrganizationId, string myDatetime)
        {
            DataTable m_ReportStructTable = GetReportDataStruct(myOrganizationId);
            Dictionary<string, string[]> m_MaterialDictionary = GetMartieralDictionary(myOrganizationId);
            AddReportStructColumns(m_ReportStructTable);
            ModifyMaterial(m_MaterialDictionary, m_ReportStructTable);   //修改分母变成相应的variableId
            GetDayBalanceData(myOrganizationId, myDatetime, m_ReportStructTable);            //获取日电量、电耗、产量数据
            GetMonthBalanceData(myOrganizationId, myDatetime, m_ReportStructTable);            //获取月电量、电耗、产量数据
            GetTotalElectrictyConsumption(m_ReportStructTable);                   

            GetTotalBalanceData(myOrganizationId, myDatetime, m_ReportStructTable); 

            return m_ReportStructTable;
        }
        private static DataTable GetReportDataStruct(string myOrganizationId)
        {
            List<string> RemoveClinkerProcessNodeId = new List<string>();
            RemoveClinkerProcessNodeId.Add("clinkerElectricityGeneration");
            RemoveClinkerProcessNodeId.Add("electricityOwnDemand");

            string m_Sql = @"select Z.* from(
                                select 
		                                E.LevelCode as OrganizationLevelCode, 
										(case when E.Type = '熟料' then 'R01' when E.Type = '水泥磨' then 'R02' when E.Type = '余热发电' then 'R03' when E.Type = '分厂' then 'R04' else '' end) 
		                                 + (case when E.Type = '分厂' then SUBSTRING(B.LevelCode,2, LEN(B.LevelCode) - 1)  ELSE SUBSTRING(E.LevelCode, LEN(E.LevelCode) - 1, 2) + SUBSTRING(B.LevelCode,4, LEN(B.LevelCode) - 3) end) as LevelCode, 
									    B.VariableId as VariableId, 
		                                B.LevelType as LevelType,
		                                E.Type as OrganizationType,
		                                E.Name + B.Name as Name,
		                                B.Denominator as Denominator 
		                                from tz_Formula A, formula_FormulaDetail B,system_Organization D, system_Organization E
			                                where D.OrganizationID = '{0}'
			                                and E.LevelCode like D.LevelCode + '%' 
			                                and A.OrganizationID in (E.OrganizationID)
			                                and A.ENABLE = 1
			                                and A.State = 0
			                                and A.KeyID = B.KeyID
											and B.LevelType <> 'MainMachine'
	                                union all 
	                                    select '' as OrganizationLevelCode, 'R01'  as LevelCode, 'clinker' as VariableId, 'Total' as LevelType, '熟料' as OrganizationType, '熟料' as Name, 'clinker_ClinkerOutput' as Denominator
								    union all
									    select '' as OrganizationLevelCode, 'R02'  as LevelCode, 'cement' as VariableId, 'Total' as LevelType, '水泥磨' as OrganizationType, '水泥磨' as Name, 'cement_CementOutput' as Denominator
									union all 
									    select '' as OrganizationLevelCode, 'R03'  as LevelCode, 'wasteHeatElectricityGeneration' as VariableId, 'Total' as LevelType, '余热发电' as OrganizationType, '余热发电' as Name, 'clinker_ClinkerOutput' as Denominator
                                    union all 
									    select '' as OrganizationLevelCode, 'R04'  as LevelCode, 'Common' as VariableId, 'Total' as LevelType, '公共工序' as OrganizationType, '公共工序' as Name, '' as Denominator
		                                ) Z 
                                order by Z.LevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            DataTable ReportStructTable = _dataFactory.Query(m_Sql);
            if (ReportStructTable != null)
            {
                int m_RemoveIndex = 0;
                bool m_FindKey = false;
                while (m_RemoveIndex < ReportStructTable.Rows.Count)
                {
                    m_FindKey = false;
                    if (ReportStructTable.Rows[m_RemoveIndex]["LevelCode"].ToString().Contains("R01") == true)
                    {
                        for (int i = 0; i < RemoveClinkerProcessNodeId.Count; i++)
                        {
                            if (ReportStructTable.Rows[m_RemoveIndex]["VariableId"].ToString() == RemoveClinkerProcessNodeId[i])   //需要移除表中的该记录
                            {
                                ReportStructTable.Rows.RemoveAt(m_RemoveIndex);
                                m_FindKey = true;
                                break;
                            }
                        }
                    }
                    if (m_FindKey == false)
                    {
                        m_RemoveIndex++;
                    }
                }
            }
            return ReportStructTable;
        }

        
        private static void ModifyLevelCode(DataTable myDataTable)
        {
            if (myDataTable != null)
            {
                int m_Index = 0;
                while (m_Index < myDataTable.Rows.Count)
                {
                    string m_VariableId = myDataTable.Rows[m_Index]["VariableId"].ToString();
                    string m_LevelCodeHead = myDataTable.Rows[m_Index]["LevelCode"].ToString().Substring(0, 3);
                    if (m_LevelCodeHead != "R03" && myDataTable.Rows[m_Index]["LevelType"].ToString() == "Process" && (m_VariableId == "electricityOwnDemand" || m_VariableId == "clinkerElectricityGeneration"))  //当是均摊在工序中的公共变量,删除均摊的余热发电部分,但包括余热本身的
                    {
                        myDataTable.Rows.RemoveAt(m_Index);
                    }
                    else
                    {
                        string m_LevelCode = myDataTable.Rows[m_Index]["LevelCode"].ToString();
                        string m_LevelCodePart = "";
                        string m_LevelCodeResult = "R";
                        for (int i = 0; i < m_LevelCode.Length / 2; i++)
                        {
                            m_LevelCodePart = m_LevelCode.Substring(i * 2 + 1, 2);
                            if (m_LevelCodePart == "99" && m_Index > 0)
                            {
                                m_LevelCodePart = (Int32.Parse(myDataTable.Rows[m_Index - 1]["LevelCode"].ToString().Substring(i * 2 + 1, 2)) + 1).ToString("D2");
                            }
                            m_LevelCodeResult = m_LevelCodeResult + m_LevelCodePart;
                        }
                        myDataTable.Rows[m_Index]["LevelCode"] = m_LevelCodeResult;
                        m_Index = m_Index + 1;
                    }
                }
            }
        }
        private static void AddReportStructColumns(DataTable myDataTable)
        {
            myDataTable.Columns.Add("DayElectricityQuantity",typeof(decimal));
            myDataTable.Columns.Add("TotalElectricityQuantity",typeof(decimal));
            myDataTable.Columns.Add("DayOutput", typeof(decimal));
            myDataTable.Columns.Add("TotalOutput", typeof(decimal));
            myDataTable.Columns.Add("DayElectricityConsumption", typeof(decimal));
            myDataTable.Columns.Add("TotalElectricityConsumption", typeof(decimal));
            myDataTable.Columns.Add("MaterialName", typeof(string));
            myDataTable.Columns.Add("ValueTypeName", typeof(string));
        }

        private static Dictionary<string, string[]> GetMartieralDictionary(string myOrganizationId)
        {
            string m_Sql = @"select B.VariableId as VariableId, D.LevelCode + B.Formula as Formula, B.Name as Name from tz_Material A, material_MaterialDetail B, system_Organization C, system_Organization D
                                where C.OrganizationID = '{0}'
                                and D.LevelCode like C.LevelCode + '%'
                                and A.OrganizationID in (D.OrganizationID)
                                and A.Enable = 1
                                and A.State = 0
                                and A.KeyID = B.KeyID
                            union all
                            select B.VariableId as VariableId, substring(D.LevelCode, 1, len(D.LevelCode) - 2) + B.Formula as Formula, B.Name as Name from tz_Material A, material_MaterialDetail B, system_Organization C, system_Organization D
                                where C.OrganizationID = '{0}'
                                and D.LevelCode like C.LevelCode + '%'
                                and A.OrganizationID in (D.OrganizationID)
                                and A.Enable = 1
                                and A.State = 0
                                and A.KeyID = B.KeyID";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            DataTable MaterialIdTable = _dataFactory.Query(m_Sql);
            Dictionary<string, string[]> m_MaterialIds = new Dictionary<string, string[]>();
            if (MaterialIdTable != null)
            {
                for (int i = 0; i < MaterialIdTable.Rows.Count; i++)
                {
                    string m_KeyWord = MaterialIdTable.Rows[i]["Formula"].ToString();
                    if (!m_MaterialIds.ContainsKey(m_KeyWord))
                    {
                        m_MaterialIds.Add(m_KeyWord, new string[] {MaterialIdTable.Rows[i]["VariableId"].ToString(), MaterialIdTable.Rows[i]["Name"].ToString()});
                    }
                }
            }
            return m_MaterialIds;
        }
        private static void ModifyMaterial(Dictionary<string, string[]> myMaterialDictionary, DataTable myDataTable)
        {
            if (myDataTable != null)
            {
                for (int i = 0; i < myDataTable.Rows.Count; i++)
                {
                    string m_Denominator = myDataTable.Rows[i]["Denominator"].ToString();   //
                    string m_OrganizationLevelCode = myDataTable.Rows[i]["OrganizationLevelCode"].ToString();
                    //if (myDataTable.Rows[i]["OrganizationType"].ToString() == "余热发电" && m_OrganizationLevelCode != "")   //取熟料
                    //{
                    //    m_Denominator = m_Denominator.Length >= 4 ? m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2) + m_Denominator.Substring(0, 4) : m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2);
                    //}
                    //else
                    //{
                        
                    //}
                    m_Denominator = m_Denominator.Length >= 4 ? myDataTable.Rows[i]["OrganizationLevelCode"].ToString() + m_Denominator.Substring(0, 4) : myDataTable.Rows[i]["OrganizationLevelCode"].ToString();
                    if(myMaterialDictionary.ContainsKey(m_Denominator))
                    {
                        myDataTable.Rows[i]["Denominator"] = myMaterialDictionary[m_Denominator][0];
                        myDataTable.Rows[i]["MaterialName"] = myMaterialDictionary[m_Denominator][1];
                    }
                }
            }
        }
        private static void GetDayBalanceData(string myOrganizationId, string myDatetime, DataTable myDataTable)
        {
            Dictionary<string, decimal> m_BalanceData = new Dictionary<string, decimal>();
            DataTable m_BalanceDataTable = GetDataFromBalanceTable(myDatetime, myDatetime, myOrganizationId);
            DataTable m_TotalBalanceDataTable = GetTotalDataFromBalanceTable(myDatetime, myDatetime, myOrganizationId);   //公共工序的汇总数据
            if (m_BalanceDataTable != null)
            {
                for (int i = 0; i < m_BalanceDataTable.Rows.Count; i++)
                {
                    string m_KeyWord = m_BalanceDataTable.Rows[i]["DictionaryKey"].ToString();
                    if (!m_BalanceData.ContainsKey(m_KeyWord))
                    {
                        m_BalanceData.Add(m_KeyWord, (decimal)m_BalanceDataTable.Rows[i]["Value"]);
                    }
                }
            }
            if (m_TotalBalanceDataTable != null)
            {
                for (int i = 0; i < m_TotalBalanceDataTable.Rows.Count; i++)
                {
                    string m_KeyWord = m_TotalBalanceDataTable.Rows[i]["DictionaryKey"].ToString();
                    if (!m_BalanceData.ContainsKey(m_KeyWord))
                    {
                        m_BalanceData.Add(m_KeyWord, (decimal)m_TotalBalanceDataTable.Rows[i]["Value"]);
                    }
                }
            }
            if (myDataTable != null)
            {
                string m_ElectricityKeyWord = "";               // + "_ElectricityQuntity"
                string m_MartieralKeyWord = "";
                foreach(DataRow m_RowItem in myDataTable.Rows)
                {
                    m_ElectricityKeyWord = m_RowItem["OrganizationLevelCode"].ToString() + m_RowItem["VariableId"].ToString();
                    m_MartieralKeyWord = m_RowItem["OrganizationLevelCode"].ToString() + m_RowItem["Denominator"].ToString();
                    if (m_BalanceData.ContainsKey(m_ElectricityKeyWord + "_ElectricityQuantity"))
                    {
                        m_RowItem["DayElectricityQuantity"] = m_BalanceData[m_ElectricityKeyWord + "_ElectricityQuantity"];
                    }
                    //if (m_BalanceData.ContainsKey(m_ElectricityKeyWord + "_ElectricityConsumption"))
                    //{
                    //    m_RowItem["DayElectricityConsumption"] = m_BalanceData[m_ElectricityKeyWord + "_ElectricityConsumption"];
                    //}
                    if (m_BalanceData.ContainsKey(m_MartieralKeyWord))
                    {
                        m_RowItem["DayOutput"] = m_BalanceData[m_MartieralKeyWord];
                    }

                    //string m_OrganizationLevelCode = m_RowItem["OrganizationLevelCode"].ToString();    
                    //if (m_RowItem["OrganizationType"].ToString() == "余热发电" && m_OrganizationLevelCode != "")   //余热发电吨熟料发电量取熟料
                    //{
                    //    m_ElectricityKeyWord = m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2) + m_RowItem["VariableId"].ToString();
                    //    if (m_BalanceData.ContainsKey(m_ElectricityKeyWord))
                    //    {
                    //        m_RowItem["DayOutput"] = m_BalanceData[m_ElectricityKeyWord];
                    //    }
                    //}
                }
            }
        }
        private static void GetMonthBalanceData(string myOrganizationId, string myDatetime, DataTable myDataTable)
        {
            Dictionary<string, decimal> m_BalanceData = new Dictionary<string, decimal>();
            DateTime m_StartTime = DateTime.Parse(myDatetime);
            DataTable m_BalanceDataTable = GetDataFromBalanceTable(m_StartTime.ToString("yyyy-MM-01"), myDatetime, myOrganizationId);
            DataTable m_TotalBalanceDataTable = GetTotalDataFromBalanceTable(m_StartTime.ToString("yyyy-MM-01"), myDatetime, myOrganizationId);   //公共工序的汇总数据
            if (m_BalanceDataTable != null)
            {
                for (int i = 0; i < m_BalanceDataTable.Rows.Count; i++)
                {
                    string m_KeyWord = m_BalanceDataTable.Rows[i]["DictionaryKey"].ToString();
                    if (!m_BalanceData.ContainsKey(m_KeyWord))
                    {
                        m_BalanceData.Add(m_KeyWord, (decimal)m_BalanceDataTable.Rows[i]["Value"]);
                    }
                }
            }
            if (m_TotalBalanceDataTable != null)
            {
                for (int i = 0; i < m_TotalBalanceDataTable.Rows.Count; i++)
                {
                    string m_KeyWord = m_TotalBalanceDataTable.Rows[i]["DictionaryKey"].ToString();
                    if (!m_BalanceData.ContainsKey(m_KeyWord))
                    {
                        m_BalanceData.Add(m_KeyWord, (decimal)m_TotalBalanceDataTable.Rows[i]["Value"]);
                    }
                }
            }
            if (myDataTable != null)
            {
                string m_ElectricityKeyWord = "";               // + "_ElectricityQuntity"
                string m_MartieralKeyWord = "";
                foreach (DataRow m_RowItem in myDataTable.Rows)
                {
                    m_ElectricityKeyWord = m_RowItem["OrganizationLevelCode"].ToString() + m_RowItem["VariableId"].ToString();
                    m_MartieralKeyWord = m_RowItem["OrganizationLevelCode"].ToString() + m_RowItem["Denominator"].ToString();
                    if (m_BalanceData.ContainsKey(m_ElectricityKeyWord + "_ElectricityQuantity"))
                    {
                        m_RowItem["TotalElectricityQuantity"] = m_BalanceData[m_ElectricityKeyWord + "_ElectricityQuantity"];
                    }
                    //if (m_BalanceData.ContainsKey(m_ElectricityKeyWord + "_ElectricityConsumption"))
                    //{
                    //    m_RowItem["TotalElectricityConsumption"] = m_BalanceData[m_ElectricityKeyWord + "_ElectricityConsumption"];
                    //}
                    if (m_BalanceData.ContainsKey(m_MartieralKeyWord))
                    {
                        m_RowItem["TotalOutput"] = m_BalanceData[m_MartieralKeyWord];
                    }
                }
            }
        }

        private static void GetTotalBalanceData(string myOrganizationId, string myDatetime, DataTable myDataTable)
        {
            if (myDataTable != null)
            {
                DataRow[] m_Root = myDataTable.Select("LEN(LevelCode) = 3");
                if (m_Root != null)             //熟料根节点
                {
                    for (int i = 0; i < m_Root.Length; i++)
                    {
                        GetTotalData(m_Root[i], myOrganizationId, myDatetime, myDataTable);
                    }
                }
            }
        }

        private static void GetTotalData(DataRow myRoot, string myOrganizationId, string myDatetime, DataTable myDataTable)
        {
            DateTime m_Datetime = DateTime.Parse(myDatetime);
            if (myRoot != null)    //每条产线的熟料节点
            {
                string m_OrganizationLevelCode = "";
                string m_LevelCode = myRoot["LevelCode"].ToString();
                DataRow[] m_SubRoot = myDataTable.Select(string.Format("LevelCode like '{0}%' and Len(LevelCode) = 5", m_LevelCode));

                for (int j = 0; j < m_SubRoot.Length; j++)
                {
                    m_OrganizationLevelCode = m_SubRoot[j]["OrganizationLevelCode"].ToString();
                    //////////////根节点的数据
                    if (myRoot["DayElectricityQuantity"] == DBNull.Value)        //日电量汇总数据
                    {
                        myRoot["DayElectricityQuantity"] = 0;
                    }
                    if (m_SubRoot[j]["DayElectricityQuantity"] != DBNull.Value)
                    {
                        myRoot["DayElectricityQuantity"] = (decimal)myRoot["DayElectricityQuantity"] + (decimal)m_SubRoot[j]["DayElectricityQuantity"];
                    }

                    if (myRoot["TotalElectricityQuantity"] == DBNull.Value)       //月电量汇总数据
                    {
                        myRoot["TotalElectricityQuantity"] = 0;
                    }
                    if (m_SubRoot[j]["TotalElectricityQuantity"] != DBNull.Value)
                    {
                        myRoot["TotalElectricityQuantity"] = (decimal)myRoot["TotalElectricityQuantity"] + (decimal)m_SubRoot[j]["TotalElectricityQuantity"];
                    }
                    if (m_LevelCode == "R01" || m_LevelCode == "R02")
                    {
                        if (myRoot["DayOutput"] == DBNull.Value)       //日产量汇总数据
                        {
                            myRoot["DayOutput"] = 0;
                        }
                        if (m_SubRoot[j]["DayOutput"] != DBNull.Value)
                        {
                            myRoot["DayOutput"] = (decimal)myRoot["DayOutput"] + (decimal)m_SubRoot[j]["DayOutput"];
                        }
                        if (myRoot["TotalOutput"] == DBNull.Value)       //月产量汇总数据
                        {
                            myRoot["TotalOutput"] = 0;
                        }
                        if (m_SubRoot[j]["TotalOutput"] != DBNull.Value)
                        {
                            myRoot["TotalOutput"] = (decimal)myRoot["TotalOutput"] + (decimal)m_SubRoot[j]["TotalOutput"];
                        }
                    }
                    //////////////////////////////////////产线计算综合电耗////////////////////////////////
                    if (m_LevelCode == "R01")
                    {
                        myRoot["MaterialName"] = "熟料产量";
                        decimal m_ClinkerPowerConsumptionDay = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day",
                                       m_Datetime.ToString("yyyy-MM-dd"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;

                        decimal m_ClinkerPowerConsumptionMonth = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day",
                                                         m_Datetime.ToString("yyyy-MM-01"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                        m_SubRoot[j]["DayElectricityConsumption"] = m_ClinkerPowerConsumptionDay;
                        m_SubRoot[j]["TotalElectricityConsumption"] = m_ClinkerPowerConsumptionMonth;

                    }
                    else if (m_LevelCode == "R02")
                    {
                        myRoot["MaterialName"] = "水泥产量";
                        decimal m_CementPowerConsumptionDay = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                                        m_Datetime.ToString("yyyy-MM-dd"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                        decimal m_CementPowerConsumptionMonth = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                                          m_Datetime.ToString("yyyy-MM-01"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                        m_SubRoot[j]["DayElectricityConsumption"] = m_CementPowerConsumptionDay;
                        m_SubRoot[j]["TotalElectricityConsumption"] = m_CementPowerConsumptionMonth;
                    }
                }
                //////////////////////////////////////计算Total综合电耗////////////////////////////////
                if (m_LevelCode == "R01")
                {
                    myRoot["MaterialName"] = "熟料产量";
                    decimal m_ClinkerPowerConsumptionDay = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day",
                                   m_Datetime.ToString("yyyy-MM-dd"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2)).CaculateValue;

                    decimal m_ClinkerPowerConsumptionMonth = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day",
                                                     m_Datetime.ToString("yyyy-MM-01"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2)).CaculateValue;
                    myRoot["DayElectricityConsumption"] = m_ClinkerPowerConsumptionDay;
                    myRoot["TotalElectricityConsumption"] = m_ClinkerPowerConsumptionMonth;
  
                }
                else if (m_LevelCode == "R02")
                {
                    myRoot["MaterialName"] = "水泥产量";
                    decimal m_CementPowerConsumptionDay = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                                    m_Datetime.ToString("yyyy-MM-dd"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2)).CaculateValue;
                    decimal m_CementPowerConsumptionMonth = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                                      m_Datetime.ToString("yyyy-MM-01"), m_Datetime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode.Substring(0, m_OrganizationLevelCode.Length - 2)).CaculateValue;
                    myRoot["DayElectricityConsumption"] = m_CementPowerConsumptionDay;
                    myRoot["TotalElectricityConsumption"] = m_CementPowerConsumptionMonth;
                }
            }
        }
        private static void GetTotalElectrictyConsumption(DataTable myReportStructTable)
        {
            if (myReportStructTable != null)
            {
                foreach (DataRow m_RowItem in myReportStructTable.Rows)    //日电耗计算
                {
                    if (m_RowItem["DayElectricityQuantity"] != DBNull.Value && m_RowItem["DayOutput"] != DBNull.Value)
                    {
                        decimal m_Denominator = (decimal)m_RowItem["DayOutput"];
                        if (m_Denominator != 0)
                        {
                            m_RowItem["DayElectricityConsumption"] = (decimal)m_RowItem["DayElectricityQuantity"] / m_Denominator;
                        }
                        else
                        {
                            m_RowItem["DayElectricityConsumption"] = 0.0m;
                        }
                    }
                }
                foreach (DataRow m_RowItem in myReportStructTable.Rows)   //月电耗计算
                {
                    if (m_RowItem["TotalElectricityQuantity"] != DBNull.Value && m_RowItem["TotalOutput"] != DBNull.Value)
                    {
                        decimal m_Denominator = (decimal)m_RowItem["TotalOutput"];
                        if (m_Denominator != 0)
                        {
                            m_RowItem["TotalElectricityConsumption"] = (decimal)m_RowItem["TotalElectricityQuantity"] / m_Denominator;
                        }
                        else
                        {
                            m_RowItem["TotalElectricityConsumption"] = 0.0m;
                        }
                    }
                }
            }
        }

      /// <summary>
      /// /////////////////////////////////////
      /// </summary>
      /// <param name="myStartDate"></param>
      /// <param name="myEndDate"></param>
      /// <param name="myOrganizationId"></param>
      /// <returns></returns>

        private static DataTable GetDataFromBalanceTable(string myStartDate, string myEndDate, string myOrganizationId)
        {
            string m_Sql = @"select E.LevelCode +  B.VariableId as DictionaryKey, sum(B.TotalPeakValleyFlatB) as Value
                                from tz_Balance A, balance_energy B
                                left join system_Organization E on B.OrganizationID = E.OrganizationID 
                                , system_Organization C, system_Organization D
                                where A.OrganizationID in (D.OrganizationID)
                                and A.StaticsCycle = 'day'
                                and A.TimeStamp >=@StartDate
                                and A.TimeStamp<=@EndDate
                                and A.BalanceId = B.KeyID
								and C.OrganizationID = @OrganizationId
								and D.LevelCode like C.LevelCode + '%'
								and D.LevelType = 'Factory'
                                group by E.LevelCode +  B.VariableId";
            SqlParameter[] parameter = new SqlParameter[]{
                                        new SqlParameter("OrganizationId",myOrganizationId),
                                        new SqlParameter("StartDate",myStartDate),
                                        new SqlParameter("EndDate",myEndDate)};
            DataTable BalanceDataTable = _dataFactory.Query(m_Sql, parameter);
            return BalanceDataTable;
        }
        private static DataTable GetTotalDataFromBalanceTable(string myStartDate, string myEndDate, string myOrganizationId)
        {
            string m_SqlProcess = @"select D.LevelCode + B.VariableId as DictionaryKey, sum(B.TotalPeakValleyFlatB) as Value
                                        from tz_Balance A, balance_energy B, system_Organization C, system_Organization D
                                        where A.OrganizationID in (D.OrganizationID)
                                        and A.StaticsCycle = 'day'
                                        and A.TimeStamp >=@StartDate
                                        and A.TimeStamp<=@EndDate
                                        and A.BalanceId = B.KeyID
								        and C.OrganizationID = @OrganizationId
								        and D.LevelCode like C.LevelCode + '%'
								        and D.LevelType = 'Factory'
								        and B.OrganizationID <> D.OrganizationID
                                        group by D.LevelCode + B.VariableId";
            string m_SqlCommon = @"select D.LevelCode + B.VariableId as DictionaryKey, sum(B.TotalPeakValleyFlatB) as Value
                                        from tz_Balance A, balance_energy B, system_Organization C, system_Organization D
                                        where A.OrganizationID in (D.OrganizationID)
                                        and A.StaticsCycle = 'day'
                                        and A.TimeStamp >=@StartDate
                                        and A.TimeStamp<=@EndDate
                                        and A.BalanceId = B.KeyID
								        and C.OrganizationID = @OrganizationId
								        and D.LevelCode like C.LevelCode + '%'
								        and D.LevelType = 'Factory'
								        and B.OrganizationID = D.OrganizationID
                                        group by D.LevelCode + B.VariableId";
            SqlParameter[] Processparameter = new SqlParameter[]{
                                        new SqlParameter("OrganizationId",myOrganizationId),
                                        new SqlParameter("StartDate",myStartDate),
                                        new SqlParameter("EndDate",myEndDate)};
            SqlParameter[] Commonparameter = new SqlParameter[]{
                                        new SqlParameter("OrganizationId",myOrganizationId),
                                        new SqlParameter("StartDate",myStartDate),
                                        new SqlParameter("EndDate",myEndDate)};
            DataTable ProcessBalanceDataTable = _dataFactory.Query(m_SqlProcess, Processparameter);
            DataTable CommonBalanceDataTable = _dataFactory.Query(m_SqlCommon, Commonparameter);
            if (CommonBalanceDataTable != null && ProcessBalanceDataTable != null)
            {
                for (int i = 0; i < CommonBalanceDataTable.Rows.Count; i++)
                {
                    DataRow[] m_FindRows = ProcessBalanceDataTable.Select(string.Format("DictionaryKey = '{0}'", CommonBalanceDataTable.Rows[i]["DictionaryKey"].ToString()));
                    if (m_FindRows.Length == 0)   //找到相同的变量.则不需要再加上公共工序中的电量
                    {
                        ProcessBalanceDataTable.ImportRow(CommonBalanceDataTable.Rows[i]);
                    }
                }
            }
            return ProcessBalanceDataTable;
        }
        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {
            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }
    }
}
