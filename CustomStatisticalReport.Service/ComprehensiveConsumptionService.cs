using AutoSetParameters;
using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomStatisticalReport.Service
{
    public class ComprehensiveConsumptionService
    {
        private const string Company = "Company";
        private const string Factory = "Factory";
        private const string ProductionLine = "ProductionLine";

        private static readonly SqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        private static readonly AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1 AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1(new SqlServerDataAdapter.SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString));

        public static Standard_GB16780_2012.Model_CaculateValue GetComprehensiveData(string myOrganizationId, string myVariableId, string mStartDate, string mEndDate)
        {
            DateTime m_Datetime = DateTime.Now;
            DataTable m_OrganizationLevelCodeTable = GetOrganizationInfo(myOrganizationId);
            if (m_OrganizationLevelCodeTable != null && m_OrganizationLevelCodeTable.Rows.Count > 0)
            {
                string m_OrganizationLevelCode = m_OrganizationLevelCodeTable.Rows[0]["LevelCode"].ToString();
                if (myVariableId == "clinker_ElectricityConsumption_Comprehensive")
                {
                    return AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day",
                                           mStartDate.ToString(), mEndDate.ToString(), m_OrganizationLevelCode);

                }
                else if (myVariableId == "cementmill_ElectricityConsumption_Comprehensive")
                {
                    return AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day",
                                           mStartDate.ToString(), mEndDate.ToString(), m_OrganizationLevelCode);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private static DataTable GetOrganizationInfo(string myOrganizationId)
        {
            string m_Sql = @"select A.LevelCode as LevelCode, A.LevelType as LevelType from system_Organization A
                     where A.OrganizationID = @OrganizationID";
            List<SqlParameter> m_Parameters = new List<SqlParameter>();
            m_Parameters.Add(new SqlParameter("@OrganizationID", myOrganizationId));
            DataTable table = _dataFactory.Query(m_Sql, m_Parameters.ToArray());
            return table;
        }
    }
}
