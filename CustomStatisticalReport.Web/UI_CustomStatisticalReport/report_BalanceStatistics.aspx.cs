using CustomStatisticalReport.Service.CustomDayConsumptionReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport
{
    public partial class report_BalanceStatistics : WebStyleBaseForEnergy.webStyleBase
    {
        private static char[] CRUD;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
                mPageOpPermission = "1000";
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                          //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "report_BalanceStatistics.aspx";                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;

                //控制网页的增删改查权限
                //低位共四位：查看、增加、修改、删除
                CRUD = mPageOpPermission.ToArray();

                ///以下是接收js脚本中post过来的参数
                string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
                string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
                string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
                if (m_FunctionName == "ExcelStream")
                {
                    //ExportFile("xls", "导出报表1.xls");
                    string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                    m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                    m_ExportTable = m_ExportTable.Replace("&nbsp", "  ");
                    DayConsumptionReportService.ExportExcelFile("xls", m_Parameter2 + "用电平衡报表.xls", m_ExportTable);
                }
            }
        }

        /// <summary>
        /// 增删改查权限控制
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static char[] AuthorityControl()
        {
            return mPageOpPermission.ToArray();
        }

        [WebMethod]
        public static Dictionary<string, string> GetReportDataSet(string organizationId, string startDate,string endDate, string statisticalType)
        {
            return BalanceStatisticsService.GetDataSet(organizationId, startDate,endDate, statisticalType);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="organizationId">分厂ID</param>
        /// <param name="statisticalType">统计类型（抄表时间/自然月）</param>
        /// <param name="dataSet">数据</param>
        /// <returns></returns>
        [WebMethod]
        public static string SaveData(string dateTime, string organizationId, string statisticalType, string dataSet)
        {
            if (CRUD[2] == '1')
            {
                return BalanceStatisticsService.Save(dateTime, organizationId, statisticalType, dataSet);
            }
            else
            {
                return "没有修改权限！";
            }
        }

        //[WebMethod]
        //public static string SaveRemarks(string dateTime, string organizationId, string statisticalType, string remarksSet)
        //{

        //}
        [WebMethod]
        public static string GetMeterReadingValue(string organizationId)
        {
            DataTable table = BalanceStatisticsService.GetCombogrid(organizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}