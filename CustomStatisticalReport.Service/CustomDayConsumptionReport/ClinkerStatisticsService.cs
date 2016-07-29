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
    public class ClinkerStatisticsService
    {
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        private const int LENGTH = 80;
        public static DataTable GetClinkerStatisticsData(string[] levelCodes,string startDate,string endDate)
        {
            //行基本数据
            string myFrameSql = @"select A.VariableId,A.Name,A.LevelCode
                                    from report_ClinkerStatistics A                                    
                                    order by A.LevelCode";
            DataTable frameTable = _dataFactory.Query(myFrameSql);
            //列基本数据
            string myOrgStr = @"select A.OrganizationID
                                from system_Organization A
                                where A.Type='熟料' AND ({0})
                                order by A.OrganizationID";
            StringBuilder myBuilder = new StringBuilder();
            foreach (string item in levelCodes)
            {
                myBuilder.Append("A.LevelCode like '");
                myBuilder.Append(item);
                myBuilder.Append("%' OR ");
               
            }
            myBuilder.Remove(myBuilder.Length - 4, 4);
            DataTable orgTable = _dataFactory.Query(string.Format(myOrgStr,myBuilder.ToString()));
            foreach(DataRow dr in orgTable.Rows)
            {
                DataColumn EQcolumn = new DataColumn(dr["OrganizationID"].ToString().Trim() + "_ElectricityQuantity",typeof(decimal));
                DataColumn ECcolumn = new DataColumn(dr["OrganizationID"].ToString().Trim() + "_ElectricityConsumption", typeof(decimal));
                frameTable.Columns.Add(EQcolumn);
                frameTable.Columns.Add(ECcolumn);
            }
            //基础数据
//            string myDataSql = @"SELECT M.OrganizationID,M.VariableId,M.ElectricityQuantity,N.Material,
//	                                    (case N.Material when 0 then 0  when  Null then Null else M.ElectricityQuantity/N.Material end) as ElectricityConsumption
//                                    FROM
//	                                    (select C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as ElectricityQuantity
//	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C
//	                                    where A.Enabled='true'
//	                                    and B.BalanceId=C.KeyId
//	                                    and A.VariableId+'_ElectricityQuantity'=C.VariableId
//	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
//	                                    group by A.VariableId,C.OrganizationID) M
//                                    LEFT JOIN
//	                                    (select C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as Material
//	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C
//	                                    where A.Enabled='true'
//	                                    and B.BalanceId=C.KeyID
//	                                    and A.MaterialID=C.VariableId
//	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
//	                                    group by A.VariableId,C.OrganizationID) N
//                                    ON
//	                                    N.VariableId=M.VariableId AND M.OrganizationID=N.OrganizationID
//	                                    ---///以上部分为求公式中已有的结点，以下为合并结点，公式中m
//                                    UNION ALL
//                                    SELECT 
//	                                    N.BindField as OrganizationID,N.VariableId,M.ElectricityQuantity AS ElectricityQuantity,L.Material,
//	                                    (case L.Material when 0 then 0  when  Null then Null else M.ElectricityQuantity/L.Material end) as ElectricityConsumption
//                                    FROM
//	                                    report_ClinkerStatisticsNodeMaintenance N
//                                    LEFT JOIN
//	                                    (select SUM(B.TotalPeakValleyFlatB) as ElectricityQuantity,C.OrganizationID,C.ParentVariableId
//	                                    from tz_Balance A,balance_Energy B,report_ClinkerStatisticsNodeMaintenance C
//	                                    where A.BalanceId=B.KeyId
//	                                    and B.VariableId=C.VariableId+'_ElectricityQuantity'
//	                                    and B.OrganizationID=C.OrganizationID
//	                                    and (A.TimeStamp>=@startDate and A.TimeStamp<=@endDate)
//	                                    group by C.OrganizationID,C.ParentVariableId) M
//                                    ON 	M.OrganizationID=N.OrganizationID
//	                                    AND M.ParentVariableId=N.VariableId
//                                    LEFT JOIN
//	                                    (select  C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as Material
//	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C,report_ClinkerStatisticsNodeMaintenance D
//	                                    where A.Enabled='true'
//	                                    and B.BalanceId=C.KeyID
//	                                    and A.MaterialID=C.VariableId
//	                                    and D.VariableId=A.VariableId
//	                                    and D.OrganizationID=B.OrganizationID
//	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
//	                                    group by A.VariableId,C.OrganizationID) L
//                                    ON 
//	                                    L.OrganizationID=N.OrganizationID
//	                                    AND L.VariableId=N.VariableId
//                                    WHERE 
//	                                    N.Displayed='true'";
            string myDataSql = @"SELECT M.OrganizationID,M.VariableId,M.ElectricityQuantity,N.Material,
	                                    (case N.Material when 0 then 0  when  Null then Null else M.ElectricityQuantity/N.Material end) as ElectricityConsumption
                                    FROM
	                                    (select C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as ElectricityQuantity
	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C
	                                    where A.Enabled='true'
	                                    and B.BalanceId=C.KeyId
	                                    and A.VariableId+'_ElectricityQuantity'=C.VariableId
	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
                                        and B.StaticsCycle='day'
	                                    group by A.VariableId,C.OrganizationID) M
                                    LEFT JOIN
	                                    (select C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as Material
	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C
	                                    where A.Enabled='true'
	                                    and B.BalanceId=C.KeyID
	                                    and A.MaterialID=C.VariableId
	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
                                        and B.StaticsCycle='day'
	                                    group by A.VariableId,C.OrganizationID) N
                                    ON
	                                    N.VariableId=M.VariableId AND M.OrganizationID=N.OrganizationID
	                                    ---///以上部分为求公式中已有的结点，以下为合并结点，公式中m
                                    UNION ALL
                                    SELECT 
	                                    N.BindField as OrganizationID,N.VariableId,M.ElectricityQuantity AS ElectricityQuantity,L.Material,
	                                    (case L.Material when 0 then 0  when  Null then Null else M.ElectricityQuantity/L.Material end) as ElectricityConsumption
                                    FROM
	                                    report_ClinkerStatisticsNodeMaintenance N
                                    LEFT JOIN
	                                    (select SUM(B.TotalPeakValleyFlatB) as ElectricityQuantity,C.OrganizationID,C.ParentVariableId
	                                    from tz_Balance A,balance_Energy B,report_ClinkerStatisticsNodeMaintenance C
	                                    where A.BalanceId=B.KeyId
	                                    and B.VariableId=C.VariableId+'_ElectricityQuantity'
	                                    and B.OrganizationID=C.OrganizationID
	                                    and (A.TimeStamp>=@startDate and A.TimeStamp<=@endDate)
                                        and A.StaticsCycle='day'
	                                    group by C.OrganizationID,C.ParentVariableId) M
                                    ON 	M.OrganizationID=N.OrganizationID
	                                    AND M.ParentVariableId=N.VariableId
                                    LEFT JOIN
	                                    (select  C.OrganizationID,A.VariableId,SUM(C.TotalPeakValleyFlatB) as Material
	                                    from report_ClinkerStatistics A,tz_Balance B,balance_Energy C,report_ClinkerStatisticsNodeMaintenance D
	                                    where B.BalanceId=C.KeyID
	                                    and A.MaterialID=C.VariableId
	                                    and D.VariableId=A.VariableId
	                                    and (D.OrganizationID=B.OrganizationID or D.OrganizationID=C.OrganizationID)
	                                    and (B.TimeStamp>=@startDate and B.TimeStamp<=@endDate)
                                        and B.StaticsCycle='day'
	                                    group by A.VariableId,C.OrganizationID) L
                                    ON 
	                                    L.OrganizationID=N.OrganizationID
	                                    AND L.VariableId=N.VariableId
                                    WHERE 
	                                    N.Displayed='true'";
            SqlParameter[] parameters = { new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate) };
            DataTable dataTable = _dataFactory.Query(myDataSql,parameters);
            Dictionary<string, decimal> myDictionary = new Dictionary<string, decimal>();
            foreach (DataRow dr in dataTable.Rows)
            {
                decimal value = 0;
                decimal.TryParse(dr["ElectricityQuantity"].ToString(),out value);
                myDictionary.Add(dr["OrganizationID"].ToString().Trim() + "_ElectricityQuantity>>" + 
                    dr["VariableId"].ToString().Trim(),value);
                decimal.TryParse(dr["ElectricityConsumption"].ToString(), out value);
                myDictionary.Add(dr["OrganizationID"].ToString().Trim() + "_ElectricityConsumption>>" +
                    dr["VariableId"].ToString().Trim(), value);
            }
            foreach (DataRow dr in frameTable.Rows)
            {
                foreach (DataColumn col in frameTable.Columns)
                {
                    string col_name = col.ColumnName;
                    string m_key = col_name + ">>" + dr["VariableId"].ToString().Trim();
                    if (myDictionary.Keys.Contains(m_key))
                    {
                        dr[col_name] = myDictionary[m_key];
                    }
                }
            }
            frameTable.Columns.Remove("VariableId");
            frameTable.Columns.Remove("LevelCode");
            return frameTable;
        }

        public static string CreatHtml(string[] levelCodes)
        {
            string mySql = @"SELECT OrganizationID, LevelCode, Name, Type, LevelType
                                    FROM      system_Organization as A
                                    WHERE   (Type = '熟料' OR LevelCode in (select substring(LevelCode,1,3) from system_Organization where Type='熟料')) 
                                       AND  ({0})
                                    ORDER BY OrganizationID, LevelCode";
            StringBuilder myBuilder = new StringBuilder();
            foreach (string item in levelCodes)
            {
                myBuilder.Append("A.LevelCode like '");
                myBuilder.Append(item);
                myBuilder.Append("%' OR ");
                myBuilder.Append(string.Format("CHARINDEX(A.LevelCode,'{0}')>0", item));
                myBuilder.Append(" OR ");
            }
            myBuilder.Remove(myBuilder.Length - 4, 4);
            DataTable table = _dataFactory.Query(string.Format(mySql,myBuilder.ToString()));
            StringBuilder htmlBuilder = new StringBuilder();
            string m_th = "<th data-options=\"field:'{0}',width:{1},align:'center'\" colspan=\"{2}\">{3}</th>";
            htmlBuilder.Append("<table id=\"gridMain_ReportTemplate\" class=\"easyui-datagrid\" data-options=\"toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true,fit:true\">");
            htmlBuilder.Append("<thead>");
            //生成第一行（分公司行）
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<th data-options=\"field:'Name',width:150\" rowspan=\"3\">名称</th>");
            foreach (DataRow dr in table.Select("Type='分公司'", "OrganizationID, LevelCode"))
            {
                string myLevelCode = dr["LevelCode"].ToString().Trim();
                int depth = myLevelCode.Length;
                DataRow[] chlidren = table.Select("LevelCode like '" + myLevelCode + "%' and Len(LevelCode)=" + (depth + 4));

                htmlBuilder.Append(string.Format(m_th, dr["OrganizationID"].ToString().Trim(),LENGTH*chlidren.Count()*2,chlidren.Count()*2,dr["Name"].ToString().Trim()));
            }
            htmlBuilder.Append("</tr>");
            //生成第二行（熟料线行）
            htmlBuilder.Append("<tr>");
            foreach (DataRow dr in table.Select("Type='熟料'", "OrganizationID, LevelCode"))
            {
                htmlBuilder.Append(string.Format(m_th, dr["OrganizationID"].ToString().Trim(), LENGTH * 2,2 ,dr["Name"].ToString().Trim()));
            }
            htmlBuilder.Append("</tr>");
            //生成第三行（电量电耗行）
            htmlBuilder.Append("<tr>");
            
            foreach (DataRow dr in table.Select("Type='熟料'", "OrganizationID, LevelCode"))
            {
                htmlBuilder.Append(string.Format(m_th, dr["OrganizationID"].ToString().Trim() + "_ElectricityQuantity", LENGTH, 1,"电量"));
                htmlBuilder.Append(string.Format(m_th, dr["OrganizationID"].ToString().Trim() + "_ElectricityConsumption", LENGTH, 1,"电耗"));
            }
            htmlBuilder.Append("</tr>");

            htmlBuilder.Append("</thead>");
            htmlBuilder.Append("</table>");
            return htmlBuilder.ToString();
        }

        public static List<MergesValueClass> GetFormatInfo()
        {
//            string myFormatSql = @"select (row_number() over (order by A.LevelCode))-1 as [index],
//	                                    B.RowSpan as rowspan,B.BindField as field
//                                    from 
//	                                    report_ClinkerStatistics A
//                                    left join 
//	                                    report_ClinkerStatisticsNodeMaintenance B
//                                    on 
//	                                    A.VariableId=B.VariableId
//	                                    and B.Displayed='true'";
            string myFormatSql = @"SELECT M.[index],N.RowSpan as rowspan,N.BindField as field
                                        FROM
	                                        (select (row_number() over (order by LevelCode))-1 as [index],VariableId
	                                        from report_ClinkerStatistics) M
                                        LEFT JOIN
	                                        (select VariableId,RowSpan,BindField
	                                        from report_ClinkerStatisticsNodeMaintenance
	                                        where DIsplayed='true') N
                                        ON 
	                                        M.VariableId=N.VariableId";
            DataTable formatTable = _dataFactory.Query(myFormatSql);
            List<string> myList = new List<string>();
            List<MergesValueClass> megesResult = new List<MergesValueClass>();
            foreach (DataRow dr in formatTable.Rows)
            {
                if (!(dr["rowspan"] is DBNull) && !(dr["field"] is DBNull))
                {
                    megesResult.Add(new MergesValueClass{
                        index=int.Parse(dr["index"].ToString().Trim()),
                        rowspan=int.Parse(dr["rowspan"].ToString().Trim()),
                        field = dr["field"].ToString().Trim() + "_ElectricityQuantity"
                    });
                }
            }
            //string template = "index:{0},rowspan:{1},field:{2}";
            //foreach (DataRow dr in formatTable.Rows)
            //{
            //    if (!(dr["rowspan"] is DBNull) && !(dr["field"] is DBNull))
            //    {
            //        myList.Add(string.Format(template, dr["index"].ToString().Trim(),
            //            dr["rowspan"].ToString().Trim(), dr["field"].ToString().Trim() + "_ElectricityQuantity"));
            //    }
            //}
            //return myList;
            return megesResult;
        }
        #region
        //        private static DataTable GetOrganizationInfo(string[] levelCodes)
//        {
//            string myOrgStr = @"select A.OrganizationID
//                                from system_Organization A
//                                where A.Type='熟料' AND ({0})
//                                order by A.OrganizationID";
//            StringBuilder myBuilder = new StringBuilder();
//            foreach (string item in levelCodes)
//            {
//                myBuilder.Append("A.LevelCode like '");
//                myBuilder.Append(item);
//                myBuilder.Append("%' OR ");
//            }
//            myBuilder.Remove(myBuilder.Length - 4, 4);
//            DataTable orgTable = _dataFactory.Query(string.Format(myOrgStr, myBuilder.ToString()));
//            return orgTable;
        //        }
        #endregion
    }

    public class MergesValueClass
    {
        public int index { get; set; }
        public int rowspan { get; set; }
        public string field { get; set; }
    }
}
