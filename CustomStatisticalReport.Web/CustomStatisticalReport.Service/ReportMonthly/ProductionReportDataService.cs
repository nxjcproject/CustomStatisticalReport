using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomStatisticalReport.Service.ReportMonthly
{
    public class ProductionReportDataService
    {
        /// <summary>
        /// 获取物料数据
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> GetMaterialDataset(string organizationId, string dateTime,string type="")
        {
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();

            //时间处理
            string[] timeArray = dateTime.Split('-');
            //月时间
            DateTime monthStart = new DateTime(Convert.ToInt16(timeArray[0]), Convert.ToInt16(timeArray[1]), 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
            //年时间
            DateTime yearStart = new DateTime(Convert.ToInt16(timeArray[0]), 1, 1);
            DateTime yearEnd = yearStart.AddYears(1).AddDays(-1);

            string mySql = @"select 
	                                B.VariableId,SUM(B.TotalPeakValleyFlatB) as Value
                                from 
	                                tz_Balance A,
	                                balance_Energy B,system_Organization C,
	                                (select LevelCode from system_Organization A where OrganizationID=@organizationId) D
                                where A.BalanceId=B.KeyId
	                                and B.OrganizationID=C.OrganizationID
	                                {0}
	                                and C.LevelCode like D.LevelCode+'%'
	                                and A.TimeStamp>@startDate
	                                and A.TimeStamp<@endDate
                                    and A.StaticsCycle='day'
                                group by 
	                                B.VariableId";

            string typeCriteria = "";
            switch (type)
            {
                case "MaterialWeight'":
                    typeCriteria = "and B.ValueType='MaterialWeight'";
                    break;
                case "ElectricityQuantity":
                    typeCriteria = "and B.ValueType='ElectricityQuantity'";
                    break;
                default:
                    typeCriteria = "";
                    break;
            }

            SqlParameter[] monthParameters = { new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startDate",monthStart.ToString("yyyy-MM-dd")),
                                        new SqlParameter("endDate",monthEnd.ToString("yyyy-MM-dd"))};
            DataTable monthData = _dataFactory.Query(string.Format(mySql,typeCriteria), monthParameters);
            foreach (DataRow dr in monthData.Rows)
            {
                if (!dictionary.Keys.Contains(dr["VariableId"].ToString().Trim() + ">month"))
                {
                    dictionary.Add(dr["VariableId"].ToString().Trim() + ">month", dr["value"] is DBNull ? 0 : Convert.ToDecimal(dr["value"]));
                }
            }
            SqlParameter[] yeraParameters ={ new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startDate",yearStart.ToString("yyyy-MM-dd")),
                                        new SqlParameter("endDate",yearEnd.ToString("yyyy-MM-dd"))};
            DataTable yearData = _dataFactory.Query(string.Format(mySql,typeCriteria), yeraParameters);
            foreach (DataRow dr in yearData.Rows)
            {
                if (!dictionary.Keys.Contains(dr["VariableId"].ToString().Trim() + ">year"))
                {
                    dictionary.Add(dr["VariableId"].ToString().Trim() + ">year", dr["value"] is DBNull ? 0 : Convert.ToDecimal(dr["value"]));
                }
            }

            return dictionary;
        }

        public static Dictionary<string, decimal> GetDatasetWithMonth(string organizationId, string dateTime, string type = "")
        {
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();

            //时间处理
            string[] timeArray = dateTime.Split('-');
            //月时间
            DateTime monthStart = new DateTime(Convert.ToInt16(timeArray[0]), Convert.ToInt16(timeArray[1]), 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
            //年时间
            DateTime yearStart = new DateTime(Convert.ToInt16(timeArray[0]), 1, 1);
            DateTime yearEnd = yearStart.AddYears(1).AddDays(-1);
            string mySql = @"select 
	                                B.VariableId+'>'+SUBSTRING(A.TimeStamp,6,2) as VariableId,SUM(B.TotalPeakValleyFlatB) as Value
                                from 
	                                tz_Balance A,
	                                balance_Energy B,system_Organization C,
	                                (select LevelCode from system_Organization A where OrganizationID=@organizationId) D
                                where A.BalanceId=B.KeyId
	                                and B.OrganizationID=C.OrganizationID
                                    {0}
	                                and C.LevelCode like D.LevelCode+'%'
	                                and A.TimeStamp>@startDate
	                                and A.TimeStamp<@endDate
                                    and A.StaticsCycle='day'
                                group by 
	                               B.VariableId+'>'+SUBSTRING(A.TimeStamp,6,2)";

            string typeCriteria = "";
            switch (type)
            {
                case "MaterialWeight'":
                    typeCriteria = "and B.ValueType='MaterialWeight'";
                    break;
                case "ElectricityQuantity":
                    typeCriteria = "and B.ValueType='ElectricityQuantity'";
                    break;
                default:
                    typeCriteria = "";
                    break;
            }

            //SqlParameter[] monthParameters = { new SqlParameter("organizationId", organizationId), 
            //                                new SqlParameter("startDate",monthStart.ToString("yyyy-MM-dd")),
            //                            new SqlParameter("endDate",monthEnd.ToString("yyyy-MM-dd"))};
            //DataTable monthData = _dataFactory.Query(string.Format(mySql, typeCriteria), monthParameters);
            //foreach (DataRow dr in monthData.Rows)
            //{
            //    if (!dictionary.Keys.Contains(dr["VariableId"].ToString().Trim() + ">month"))
            //    {
            //        dictionary.Add(dr["VariableId"].ToString().Trim() + ">month", dr["value"] is DBNull ? 0 : Convert.ToDecimal(dr["value"]));
            //    }
            //}
            SqlParameter[] yeraParameters ={ new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startDate",yearStart.ToString("yyyy-MM-dd")),
                                        new SqlParameter("endDate",yearEnd.ToString("yyyy-MM-dd"))};
            DataTable yearData = _dataFactory.Query(string.Format(mySql, typeCriteria), yeraParameters);
            foreach (DataRow dr in yearData.Rows)
            {
                if (!dictionary.Keys.Contains(dr["VariableId"].ToString().Trim() + ">year"))
                {
                    dictionary.Add(dr["VariableId"].ToString().Trim(), dr["value"] is DBNull ? 0 : Convert.ToDecimal(dr["value"]));
                }
            }

            return dictionary;
        }
    }
}
