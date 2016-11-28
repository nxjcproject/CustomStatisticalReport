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
    public partial class DayConsumptionReport1 : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                          //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "DayConsumptionReport.aspx";                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.LeveDepth = 3;



            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                DayConsumptionReportService.ExportExcelFile("xls", "中材甘肃水泥能耗日报.xls", m_ExportTable);
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
        public static IEnumerable<Model_DataItem> GetReportData(string organizationId, string datetime)
        {
            DataTable m_ProcessValueTable = DayConsumptionReportService.GetDataFromBalanceTable(datetime, datetime, organizationId);
            List<Model_DataItem> DataItems = new List<Model_DataItem>();
            ///////////////////////////工序电量、电耗以及产量///////////////////////
            if (m_ProcessValueTable != null)
            {
                foreach (DataRow m_RowItem in m_ProcessValueTable.Rows)
                {
                    Model_DataItem m_DataItem = new Model_DataItem();
                    m_DataItem.ID = m_RowItem["DictionaryKey"].ToString();
                    m_DataItem.Value = ((decimal)m_RowItem["Value"]).ToString("#0.00");
                    DataItems.Add(m_DataItem);
                }
            }
            ////////////////////////初始化综合能耗计算参数/////////////////
            DataTable m_ClinkerComprehensiveDataTable = DayConsumptionReportService.GetClinkerComprehensiveData(datetime, datetime, organizationId);
            Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = DayConsumptionReportService.SetComprehensiveDataParameters(organizationId, datetime);
            Standard_GB16780_2012.Function_EnergyConsumption_V1 m_Function_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
            ///////////////////////////熟料综合电耗、煤耗、能耗///////////////////////////

           
            m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerComprehensiveDataTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

            decimal m_ClinkerPowerConsumption = m_Function_EnergyConsumption_V1.GetClinkerPowerConsumption();
            decimal m_ClinkerCoalConsumption = m_Function_EnergyConsumption_V1.GetClinkerCoalConsumption();
            decimal m_ClinkerEnergyConsumption = m_Function_EnergyConsumption_V1.GetClinkerEnergyConsumption(m_ClinkerPowerConsumption, m_ClinkerCoalConsumption);
            Model_DataItem m_ClinkerPowerConsumptionItem = new Model_DataItem();
            m_ClinkerPowerConsumptionItem.ID = "zc_nxjc_byc_byf_clinker_ElectricityConsumption_Comprehensive";
            m_ClinkerPowerConsumptionItem.Value = m_ClinkerPowerConsumption.ToString("#0.00");
            DataItems.Add(m_ClinkerPowerConsumptionItem);
            Model_DataItem m_ClinkerCoalConsumptionItem = new Model_DataItem();
            m_ClinkerCoalConsumptionItem.ID = "zc_nxjc_byc_byf_clinker_CoalConsumption_Comprehensive";
            m_ClinkerCoalConsumptionItem.Value = m_ClinkerCoalConsumption.ToString("#0.00");
            DataItems.Add(m_ClinkerCoalConsumptionItem);
            Model_DataItem m_ClinkerEnergyConsumptionItem = new Model_DataItem();
            m_ClinkerEnergyConsumptionItem.ID = "zc_nxjc_byc_byf_clinker_EnergyConsumption_Comprehensive";
            m_ClinkerEnergyConsumptionItem.Value = m_ClinkerEnergyConsumption.ToString("#0.00");
            DataItems.Add(m_ClinkerEnergyConsumptionItem);
            ///////////////////////////水泥磨综合电耗、煤耗、能耗///////////////////////////
            DataTable m_CementComprehensiveDataTable = DayConsumptionReportService.GetCementComprehensiveData(datetime, datetime, organizationId);
            //m_Function_EnergyConsumption_V1.ClearPropertiesList();

            m_Function_EnergyConsumption_V1.LoadComprehensiveData(m_CementComprehensiveDataTable, m_Parameters_ComprehensiveData, "VariableId", "Value");

            decimal m_CementPowerConsumption = m_Function_EnergyConsumption_V1.GetCementPowerConsumption(m_ClinkerPowerConsumption);
            decimal m_CementCoalConsumption = m_Function_EnergyConsumption_V1.GetCementCoalConsumption(m_ClinkerCoalConsumption);
            decimal m_CementEnergyConsumption = m_Function_EnergyConsumption_V1.GetCementEnergyConsumption(m_CementPowerConsumption, m_CementCoalConsumption);
            Model_DataItem m_CementPowerConsumptionItem = new Model_DataItem();
            m_CementPowerConsumptionItem.ID = "zc_nxjc_byc_byf_cementmill_ElectricityConsumption_Comprehensive";
            m_CementPowerConsumptionItem.Value = m_CementPowerConsumption.ToString("#0.00");
            DataItems.Add(m_CementPowerConsumptionItem);
            Model_DataItem m_CementCoalConsumptionItem = new Model_DataItem();
            m_CementCoalConsumptionItem.ID = "zc_nxjc_byc_byf_cementmill_CoalConsumption_Comprehensive";
            m_CementCoalConsumptionItem.Value = m_CementCoalConsumption.ToString("#0.00");
            DataItems.Add(m_CementCoalConsumptionItem);
            Model_DataItem m_CementEnergyConsumptionItem = new Model_DataItem();
            m_CementEnergyConsumptionItem.ID = "zc_nxjc_byc_byf_cementmill_EnergyConsumption_Comprehensive";
            m_CementEnergyConsumptionItem.Value = m_CementEnergyConsumption.ToString("#0.00");
            DataItems.Add(m_CementEnergyConsumptionItem);
            return DataItems;
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