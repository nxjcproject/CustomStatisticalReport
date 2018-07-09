using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomStatisticalReport.Service.CustomDayConsumptionReport
{
    public class BalanceStatisticsService
    {
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        
        private static readonly AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1 AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1(_dataFactory);//计算综合电耗用到
        private static Guid keyid;
        public static Dictionary<string, string> GetDataSet(string organizationID, string startDate,string endDate, string statisticalType)
        {
            //string[] dateArray = datetime.Split('-');
            //DateTime t_time = new DateTime(Int16.Parse(dateArray[0]), Int16.Parse(dateArray[1]), 1);
            //DateTime startDate = t_time.AddDays(-1);
            //DateTime endDate = t_time.AddMonths(1);
            //当月数据
            string mySql = @"select 
	                                A.OrganizationID as factoryOrganizationId,B.OrganizationID,B.VariableId,(B.OrganizationID+'>'+B.VariableId+'>month') as ID,
	                                SUM(B.TotalPeakValleyFlatB) as Value
                                from 
	                                tz_Balance A,balance_Energy B
                                where 
	                                A.BalanceId=B.KeyId
	                                and A.OrganizationID=@organizationID
	                                and(B.VariableId='limestoneMine_ElectricityQuantity' --
	                                or B.VariableId='clinkerBurning_ElectricityQuantity' --
	                                or B.VariableId='coalPreparation_ElectricityQuantity'--
	                                or B.VariableId='cementmill_ElectricityQuantity'     --
	                                or B.VariableId='cementPacking_ElectricityQuantity'  --
	                                or B.VariableId='auxiliaryProduction_ElectricityQuantity'
                                    --or B.VariableId='clinkerElectricityGeneration_ElectricityQuantity'
                                    --or B.VariableId='electricityOwnDemand_ElectricityQuantity'
	                                --以上为电量
	                                or B.VariableId='clinker_PulverizedCoalOutput'--煤粉产量
	                                or B.VariableId='cement_CementOutput'		  --水泥产量
	                                or B.VariableId='clinker_ClinkerOutput'       --熟料产量
                                    or B.VariableId='clinker_MixtureMaterialsOutput'
	                                )
                                    and A.TimeStamp>@startDate and A.TimeStamp<@endDate
                                    and A.StaticsCycle='day'
                                group by A.OrganizationID,B.OrganizationID,B.VariableId,(B.OrganizationID+'>'+B.VariableId)";
            SqlParameter[] parameter = {new SqlParameter("organizationID", organizationID),
                                       new SqlParameter("startDate",DateTime.Parse(startDate).ToString("yyyy-MM-dd")),
                                       new SqlParameter("endDate",DateTime.Parse(endDate).ToString("yyyy-MM-dd"))};
            DataTable dataTable = _dataFactory.Query(mySql, parameter);
            //Dictionary<string, decimal> dictionary_month = new Dictionary<string, decimal>();//当月值
            Dictionary<string, string> resultDataSet = new Dictionary<string, string>();//最终结果
            foreach (DataRow dr in dataTable.Rows)
            {
                resultDataSet.Add(dr["ID"].ToString().Trim(), dr["Value"].ToString());
            }
            //当月数据（合并的数据）
            string sumSql = @"select A.OrganizationID,(A.OrganizationID+'>'+B.VariableId+'>month') as ID,Sum(B.TotalPeakValleyFlatB) as Value
                                from
	                                tz_Balance A,balance_Energy B,system_Organization C
                                where 
	                                A.BalanceId=B.KeyId
	                                and B.OrganizationID=C.OrganizationID
                                    and A.OrganizationID=@organizationId
	                                and C.Type<>'熟料'
	                                and (B.VariableId='electricityOwnDemand_ElectricityQuantity'
	                                or B.VariableId='clinkerElectricityGeneration_ElectricityQuantity')
                                    and A.TimeStamp>@startDate and A.TimeStamp<@endDate
                                    and A.StaticsCycle='day'
                                group by A.OrganizationID,B.VariableId,(A.OrganizationID+'>'+B.VariableId+'>month')";
            SqlParameter[] sumParameter = {new SqlParameter("organizationID", organizationID),
                                       new SqlParameter("startDate",DateTime.Parse(startDate).ToString("yyyy-MM-dd")),
                                       new SqlParameter("endDate",DateTime.Parse(endDate).ToString("yyyy-MM-dd"))};
            DataTable sumTable = _dataFactory.Query(sumSql, sumParameter);
            foreach (DataRow dr in sumTable.Rows)
            {
                
                resultDataSet.Add(dr["ID"].ToString().Trim(), dr["Value"].ToString());
            }
            //累计数据(累计到前一个月)
            string myTotalSql = @"select (B.OrganizationID+'>'+B.VariableId+'>accumulation') as ID,Sum(B.Value) as Value
                                    from 
	                                    tz_BalanceStatistics A,Balance_EnergyStatistics B
                                    where 
	                                    A.BalanceId=B.KeyId
	                                    and A.TimeStamp>=@startMonth
	                                    and A.TimeStamp<=@endMonth
	                                    and A.OrganizationID=@organizationId
	                                    and A.StatisticalType=@statisticalType
	                                    and (B.ValueType='ElectricityQuantity' or B.ValueType='MaterialWeight')--只求产量电量
                                    group by (B.OrganizationID+'>'+B.VariableId)";
            SqlParameter[] parametersTotal = {new SqlParameter("organizationId",organizationID),
                                             new SqlParameter("startMonth",DateTime.Parse(startDate).ToString("yyyy")+"-"+"01"),
                                             new SqlParameter("endMonth",DateTime.Parse(endDate).AddMonths(-1).ToString("yyyy-MM")),
                                             new SqlParameter("statisticalType",statisticalType)};
            DataTable accumulativeTable = _dataFactory.Query(myTotalSql,parametersTotal);
            //Dictionary<string, decimal> resultDataSet = new Dictionary<string, decimal>();//最终结果
            //Dictionary<string, decimal> dictionary_accum = new Dictionary<string, decimal>();//累计到前一月的值
            foreach (DataRow dr in accumulativeTable.Rows)
            {
                string t_id = dr["ID"].ToString().Trim();
                resultDataSet.Add(t_id, dr["Value"].ToString());
            }
            //foreach (string id in dictionary_month.Keys)
            //{
            //    resultDataSet.Add(id + ">month", dictionary_month[id]);//将当月值加到最终结果中
            //    resultDataSet.Add(id + ">accumulation", dictionary_accum.Keys.Contains(id) ? dictionary_accum[id] + dictionary_month[id] : dictionary_month[id]);//将累计值加到结果中
            //}
            GetCementData(organizationID, startDate, endDate, resultDataSet);

            
            //查询备注信息
            //查询备注信息应该放在查询页面
            /*
            string remarksSql = @"select (B.OrganizationID+'>'+B.VariableId+'>placeholder') as ID,B.Remarks as Value
                                    from 
	                                    tz_BalanceStatistics A,Balance_EnergyStatistics B
                                    where 
	                                    A.BalanceId=B.KeyId
	                                    and A.TimeStamp=@datetime
	                                    and A.OrganizationID=@organizationId
	                                    and A.StatisticalType=@statisticalType
	                                    and B.ValueType='Remarks'";
            SqlParameter[] remarksParameters = { new SqlParameter("datetime",DateTime.Parse(endDate).ToString("yyyy-MM")),
                                               new SqlParameter("organizationId", organizationID),
                                               new SqlParameter("statisticalType", statisticalType)};
            DataTable remarksTable = _dataFactory.Query(remarksSql, remarksParameters);
            foreach (DataRow dr in remarksTable.Rows)
            {
                string t_id = dr["ID"].ToString().Trim();
                resultDataSet.Add(t_id, dr["Value"].ToString());
            }
            */


            //计算综合电耗
            string orgainizationLevelCode = GetOrganizationLevelCode(organizationID);
            decimal calculateValue= AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                           DateTime.Parse(startDate).ToString("yyyy-MM-dd"), DateTime.Parse(endDate).ToString("yyyy-MM-dd"), orgainizationLevelCode).CaculateValue;
            resultDataSet.Add(organizationID + ">" + "cementmill_ElectricityConsumption_Comprehensive", calculateValue.ToString());
            return resultDataSet;
        }

        /// <summary>
        /// 根据组织机构ID查找对应的层次码
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private static string GetOrganizationLevelCode(string organizationId)
        {
            string mySql = @"select A.LevelCode
                                from system_Organization A
                                where A.OrganizationID = @organizationId";
            SqlParameter parameter = new SqlParameter("organizationId",organizationId);
            return _dataFactory.Query(mySql, parameter).Rows[0]["LevelCode"].ToString().Trim();
            
        }
        //public static Dictionary<string, string> GetRemarksSet(string organizationID, string datetime, string statisticalType)
        //{
        //    string[] dateArray = datetime.Split('-');
        //    DateTime t_time = new DateTime(Int16.Parse(dateArray[0]), Int16.Parse(dateArray[1]), 1);
        //    DateTime startDate = t_time.AddDays(-1);
        //    DateTime endDate = t_time.AddMonths(1);
        //    string remarksSql = @"";
        //}
        private static void GetCementData(string organizationId,string startDate,string endDate,Dictionary<string,string> dic)
        {
            string myCementSql = @"select (A.OrganizationID+'>'+B.VariableId+'>'+B.ValueType) AS ID,SUM(B.TotalPeakValleyFlatB) as Value
                                    from tz_Balance A,balance_Energy B,system_CementTypesAndConvertCoefficient C
                                    where A.BalanceId=B.KeyId
                                    and (B.VariableId = C.CementTypes)
                                    and A.OrganizationID=@organizationId
                                    and A.TimeStamp>=@startDate
                                    and A.TimeStamp<=@endDate
                                    and A.StaticsCycle='day'
                                    group by (A.OrganizationID+'>'+B.VariableId+'>'+B.ValueType)";
            SqlParameter[] parameters ={new SqlParameter("organizationId",organizationId),
                                          new SqlParameter("startDate",DateTime.Parse(startDate).ToString("yyyy-MM-dd")),
                                          new SqlParameter("endDate",DateTime.Parse(endDate).ToString("yyyy-MM-dd"))};
            DataTable table = _dataFactory.Query(myCementSql, parameters);
            //Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            foreach (DataRow dr in table.Rows)
            {
                string id=dr["ID"].ToString().Trim();
                if (!dic.Keys.Contains(id))
                {
                    dic.Add(id, dr["Value"].ToString());
                }
            }
        }
        //保存
        public static string Save(string date, string organizationId, string statisticalType, string dataset)
        {
            string aimDate = date;//DateTime.Parse(date).ToString("yyyy-MM");
            if (CheckData(organizationId, aimDate, statisticalType))
            {
                keyid = Guid.NewGuid();
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(Guid));
                table.Columns.Add("VariableId", typeof(string));
                table.Columns.Add("KeyId", typeof(Guid));
                table.Columns.Add("OrganizationID", typeof(string));
                table.Columns.Add("ValueType", typeof(string));
                table.Columns.Add("Value", typeof(decimal));
                table.Columns["Value"].DefaultValue = 0;
                table.Columns.Add("Remarks",typeof(string));

                //table.Columns["Remarks"].DefaultValue = "";
                string[] datasetArray = dataset.Split(',');
                foreach (string item in datasetArray)
                {
                    string[] array = item.Split('>', ':');
                    DataRow newRow = table.NewRow();
                    newRow["ID"] = Guid.NewGuid();
                    newRow["VariableId"] = array[1];
                    newRow["KeyId"] = keyid;
                    newRow["OrganizationID"] = array[0];
                    newRow["ValueType"] = array[3];

                    //如果是备注则存放在Remarks字段中，否则存放在Value字段中
                    if ("Remarks" == array[3])
                    {
                        newRow["Remarks"] = array[4].ToString();
                    }
                    else
                    {
                        newRow["Value"] = Convert.ToDecimal(array[4]);
                    }
                    table.Rows.Add(newRow);
                }
                using (SqlConnection conn = new SqlConnection(ConnectionStringFactory.NXJCConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())//建立事务
                    {
                        try
                        {

                            //保存tz_BalanceStatistics
                            string tzSql = @"insert into tz_BalanceStatistics
                                            (BalanceId, OrganizationID, TimeStamp, StatisticalType)
                                            values
                                            (@BalanceId, @OrganizationID, @TimeStamp, @StatisticalType)";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Transaction = transaction;
                            cmd.Connection = conn;
                            cmd.Parameters.Add(new SqlParameter("BalanceId", keyid));
                            cmd.Parameters.Add(new SqlParameter("OrganizationID", organizationId));
                            cmd.Parameters.Add(new SqlParameter("TimeStamp", aimDate));
                            cmd.Parameters.Add(new SqlParameter("StatisticalType", statisticalType));
                            cmd.CommandText = tzSql;
                            cmd.ExecuteNonQuery();
                            //保存Balance_EnergyStatistics

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, transaction))
                            {
                                bulkCopy.BatchSize = 10;
                                bulkCopy.BulkCopyTimeout = 60;
                                bulkCopy.DestinationTableName = "Balance_EnergyStatistics";
                                bulkCopy.WriteToServer(table);
                            }
                            //string 
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {

                            transaction.Rollback();
                            conn.Close();
                            return "保存数据失败！";
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            else
            {
                return "数据已经存在,不能重复保存！";
            }
            
            return "保存成功！";
        }

        //public static string saveRemarks(string date, string organizationId, string statisticalType, string dataset)
        //{

        //}

        //检查数据是否存在
        private static bool CheckData(string organizationId,string date,string type)
        {
            string aimDate = DateTime.Parse(date).ToString("yyyy-MM");
            string checkSql = @"select *
                                from tz_BalanceStatistics A
                                where A.OrganizationID=@organizationId
                                and A.TimeStamp=@date
                                and A.StatisticalType=@type";
            SqlParameter[] parameters ={new SqlParameter("organizationId",organizationId),
                                          new SqlParameter("date",aimDate),
                                          new SqlParameter("type",type)};
            DataTable table= _dataFactory.Query(checkSql, parameters);
            if (table.Rows.Count == 0)
                return true;
            else
                return false;
        }

        public static DataTable GetCombogrid(string organizationId)
        {
            DateTime time = DateTime.Now;
            DateTime startDate = time.AddYears(-1);
            
            string myGridSql = @"select A.DataItemId,A.TimeStamp,A.DataValue
                                    from system_EnergyDataManualInput A
                                    where A.VariableId='PurchasingPower_ElectricityQuantity'
                                    and A.UpdateCycle='day'
                                    and A.OrganizationID=@organizationId
                                    and A.TimeStamp>@startDate
                                    and A.TimeStamp<@endDate";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId),
                                        new SqlParameter("startDate", startDate),
                                        new SqlParameter("endDate", time)};
            DataTable table = _dataFactory.Query(myGridSql,parameters);
            return table;
        }
    }
}
