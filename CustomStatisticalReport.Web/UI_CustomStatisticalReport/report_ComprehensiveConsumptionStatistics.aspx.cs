using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomStatisticalReport.Service;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport
{
    public partial class report_ComprehensiveConsumptionStatistics : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
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