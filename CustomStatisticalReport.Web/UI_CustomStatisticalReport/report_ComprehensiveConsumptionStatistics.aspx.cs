using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomStatisticalReport.Service;
using CustomStatisticalReport.Service.CustomDayConsumptionReport;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport
{
    public partial class report_ComprehensiveConsumptionStatistics : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                DayConsumptionReportService.ExportExcelFile("xls", m_Parameter2 + "综合电耗统计.xls", m_ExportTable);
            }
        }

        [WebMethod]
        public static string GetComprehensiveConsumptionData(string mStartTime, string mEndTime)
        {
            DataTable table = CustomStatisticalReport.Service.report_ComprehensiveConsumptionStatistics.GetComprehensiveConsumptionDataInfo(mStartTime, mEndTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}