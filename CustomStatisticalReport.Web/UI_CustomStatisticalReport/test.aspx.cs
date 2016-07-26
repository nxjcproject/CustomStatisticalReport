using CustomStatisticalReport.Service.CustomDayConsumptionReport;
using CustomStatisticalReport.Service.ReportMonthly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string[] levelcodes = { "O03","O02"};
            ////ClinkerStatisticsService.GetClinkerStatisticsData(levelcodes);
            //ClinkerStatisticsService.CreatHtml(levelcodes);

            ////student stu=new student("张三","济南");

            ////Console.WriteLine({0},stu.introduce());

            //ClinkerStatisticsService.GetFormatInfo();
            //DateTime t1 = new DateTime(2014, 1, 1);
            //DateTime t2 = new DateTime(2015,1,1);
            //int n=t2.CompareTo(t1);


            //string[] dateArray = "2015-12".Split('-');
            //DateTime t_time = new DateTime(Int16.Parse(dateArray[0]), Int16.Parse(dateArray[1]), 1);
            //DateTime startDate = t_time.AddDays(-1);
            //DateTime endDate = t_time.AddMonths(1);
            //int i;

            ReportMonthly_SemifinishedService.GetReportData("zc_nxjc_byc", "2015-08");
        }
    }
}