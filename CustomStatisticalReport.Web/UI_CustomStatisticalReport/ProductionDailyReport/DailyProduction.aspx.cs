using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionDailyReport
{
    public partial class DailyProduction : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                          //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "DailyProduction.aspx";                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 3;
            }
        }
        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string GetDailyProductionData(string myOrganizationId, string myDateTime)
        {
            DataTable m_EquipmentCommonInfoTable = CustomStatisticalReport.Service.ProductionDailyReport.DailyProduction.GetDailyProductionData(myOrganizationId, myDateTime);
            string m_ReportDataJson = EasyUIJsonParser.TreeGridJsonParser.DataTableToJson(m_EquipmentCommonInfoTable, "EquipmentId", "Name", "ParentEquipmentId", 0, new string[] { 
                "Output_Plan","TimeOutput_Plan","RunTime_Plan","RunRate_Plan",
                "Output_Day","TimeOutput_Day","RunTime_Day","RunRate_Day",
                "Output_Month","TimeOutput_Month","RunTime_Month","RunRate_Month",
                "Output_Year","TimeOutput_Year","RunTime_Year","RunRate_Year"});
            return m_ReportDataJson;
        }
    }

}