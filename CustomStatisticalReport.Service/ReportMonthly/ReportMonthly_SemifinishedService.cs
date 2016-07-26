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
    public class ReportMonthly_SemifinishedService
    {
        public static Dictionary<string, decimal> GetReportData(string organizationId, string dateTime)
        {
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();

            //时间处理
            string[] timeArray = dateTime.Split('-');
            //月时间
            DateTime monthStart = new DateTime(Convert.ToInt16(timeArray[0]), Convert.ToInt16(timeArray[1]), 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
            //年时间
            DateTime yearStart = new DateTime(Convert.ToInt16(timeArray[0]),1,1);
            DateTime yearEnd = yearStart.AddYears(1).AddDays(-1);
            
            string mySql = @"select 
	                                B.VariableId,SUM(B.TotalPeakValleyFlatB) as Value
                                from 
	                                tz_Balance A,
	                                balance_Energy B,system_Organization C,
	                                (select LevelCode from system_Organization A where OrganizationID=@organizationId) D
                                where A.BalanceId=B.KeyId
	                                and B.OrganizationID=C.OrganizationID
	                                and B.ValueType='MaterialWeight'
	                                and C.LevelCode like D.LevelCode+'%'
	                                and A.TimeStamp>@startDate
	                                and A.TimeStamp<@endDate
                                group by 
	                                B.VariableId";
            SqlParameter[] monthParameters = { new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startDate",monthStart.ToString("yyyy-MM-dd")),
                                        new SqlParameter("endDate",monthEnd.ToString("yyyy-MM-dd"))};
            DataTable monthData = _dataFactory.Query(mySql, monthParameters);
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
            DataTable yearData = _dataFactory.Query(mySql, yeraParameters);
            foreach (DataRow dr in yearData.Rows)
            {
                if (!dictionary.Keys.Contains(dr["VariableId"].ToString().Trim() + ">month"))
                {
                    dictionary.Add(dr["VariableId"].ToString().Trim() + ">year", dr["value"] is DBNull ? 0 : Convert.ToDecimal(dr["value"]));
                }             
            }
            //DataTable result = new DataTable();
            //result = monthData.Copy();
            //result.Merge(yearData);
            return dictionary;
        }
    }
}
