using CustomStatisticalReport.Service.ReportMonthly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CustomStatisticalReport.Web.UI_CustomStatisticalReport.ProductionReport
{
    /// <summary>
    /// ProductionReportService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class ProductionReportService : System.Web.Services.WebService
    {

        /// <summary>
        /// 获取物料数据集合
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [WebMethod]
        public Dictionary<string, decimal> GetMaterialDatasetService(string organizationId, string datetime)
        {
            return ProductionReportDataService.GetMaterialDataset(organizationId, datetime, "MaterialWeight");
        }
        /// <summary>
        /// 获取电量数据
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [WebMethod]
        public Dictionary<string, decimal> GetElectricityQuantityDatasetService(string organizationId, string datetime)
        {
            return ProductionReportDataService.GetMaterialDataset(organizationId, datetime, "ElectricityQuantity");
        }
        /// <summary>
        /// 获取电量和产量数据
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [WebMethod]
        public Dictionary<string, decimal> GetDatasetWithMonthService(string organizationId, string datetime)
        {
            return ProductionReportDataService.GetDatasetWithMonth(organizationId, datetime);
        }
    }
}
