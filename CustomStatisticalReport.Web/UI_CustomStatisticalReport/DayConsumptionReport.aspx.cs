using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using CustomStatisticalReport.Service.CustomDayConsumptionReport;
namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport
{
    public partial class DayConsumptionReport : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx","zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                          //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "DayConsumptionReport.aspx";                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.LeveDepth = 5;



            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                DayConsumptionReportService.ExportExcelFile("xls", m_Parameter2 + "能耗日报.xls", m_ExportTable);
            }
            if (!IsPostBack)
            {

            }
        }
        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string GetReportData(string organizationId, string datetime)
        {
            DataTable m_ReportData = CustomStatisticalReport.Service.CustomDayConsumptionReport.DayConsumptionReportService.GetReportData(organizationId, datetime);
            string m_ReportDataJson = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(m_ReportData,"LevelCode");
            return m_ReportDataJson;
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string PrintFile()
        {
            //string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            //string m_HtmlData = StatisticalReportHelper.CreatePrintHtmlTable(mFileRootPath +
            //    REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
            //return m_HtmlData;
            return "";
        }
    }
}