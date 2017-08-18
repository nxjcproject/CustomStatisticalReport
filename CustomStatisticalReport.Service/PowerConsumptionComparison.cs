using CustomStatisticalReport.Infrastructure.Configuration;
using Monitor_shell.Service.ProcessEnergyMonitor;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CustomStatisticalReport.Service
{
    public class PowerConsumptionComparison
    {

        public static DataTable GetReportData(string organizationId, string mStartDate, string mEndDate) 
        {
            //private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            DataTable mStructTable = GetStructTablebyOrganizationId(organizationId);
            string ConnectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionString);

            string mySql = @"select  B.[VariableId]
		                        ,B.[VariableName]
		                        ,B.[OrganizationID]
		                        ,B.[ValueType]
		                        ,sum(B.[TotalPeakValleyFlat]) as sumValue from [NXJC].[dbo].[tz_Balance] A,[NXJC].[dbo].[balance_Energy] B
                                where A.OrganizationID like @organizationId+'%'
                                and A.[BalanceId]=B.[KeyId]
                                and [StaticsCycle]='day'
                                and A.[TimeStamp]>=@mStartDate and A.[TimeStamp]<=@mEndDate
                                and (B.[ValueType]='ElectricityQuantity' or B.[ValueType]='MaterialWeight')
                                and( B.OrganizationID like '%clinker%' or B.OrganizationID like '%cementmill%')
                                and(B.VariableId='rawMaterialsPreparation_ElectricityQuantity' or B.VariableId='clinker_MixtureMaterialsOutput'
                                or B.VariableId='clinkerPreparation_ElectricityQuantity' or B.VariableId='clinker_ClinkerOutput'
                                or B.VariableId='cementPreparation_ElectricityQuantity' or B.VariableId='cement_CementOutput'
                                or B.VariableId='coalPreparation_ElectricityQuantity' or B.VariableId='clinker_PulverizedCoalOutput'
                                )
                                group by
                                       B.[VariableId]
                                      ,B.[VariableName]
                                      ,B.[OrganizationID]
                                      ,B.[ValueType]
                                order by B.[ValueType],B.OrganizationID desc";
            SqlParameter[] myPara = {   new SqlParameter("@organizationId", organizationId),
                                        new SqlParameter("@mStartDate", mStartDate),
                                        new SqlParameter("@mEndDate", mEndDate) };
            DataTable originalTable = dataFactory.Query(mySql, myPara);
          
            for (int i = 0; i < mStructTable.Rows.Count;i++ )
            {
              //  mStructTable.Rows[i]["OrganizationId"]; mStructTable.Rows[i]["ElectricityQuantityVariable"]; mStructTable.Rows[i]["MaterialWeightVariable"]
                decimal ElectricityQuantityValue = 0;
                decimal MaterialWeightValue = 0;
                for(int j=0;j<originalTable.Rows.Count;j++)
                {
                    if (mStructTable.Rows[i]["OrganizationId"].ToString().Trim() == originalTable.Rows[j]["OrganizationID"].ToString().Trim())
                    {
                      
                        if (mStructTable.Rows[i]["ElectricityQuantityVariable"].ToString().Trim() == originalTable.Rows[j]["VariableId"].ToString().Trim())
                        {
                            mStructTable.Rows[i]["ElectricityQuantity"] = originalTable.Rows[j]["sumValue"];
                            ElectricityQuantityValue = Convert.ToDecimal(originalTable.Rows[j]["sumValue"]);
                        }
                        else if (mStructTable.Rows[i]["MaterialWeightVariable"].ToString().Trim() == originalTable.Rows[j]["VariableId"].ToString().Trim())
                        {
                            mStructTable.Rows[i]["MaterialWeight"] = originalTable.Rows[j]["sumValue"];
                            MaterialWeightValue = Convert.ToDecimal(originalTable.Rows[j]["sumValue"]);
                        }                   
                    }
                }          
                mStructTable.Rows[i]["PowerConsumption"] =
                    MaterialWeightValue == 0 ? 0 : (mStructTable.Rows[i]["PowerConsumption"] = Convert.ToDouble(ElectricityQuantityValue / MaterialWeightValue).ToString("0.00"));
                if (mStructTable.Rows[i]["OrganizationId"].ToString().Trim().Contains("cementmill"))
                {
                    mStructTable.Rows[i]["CalculationPowerConsumption"] = mStructTable.Rows[i]["PowerConsumption"];
                    string mOrganizationId=mStructTable.Rows[i]["OrganizationId"].ToString().Trim();
                    string mVariable=mStructTable.Rows[i]["ComprehensivePowerConsumptionVariable"].ToString().Trim();
                    string Value = ComprehensiveConsumptionService.GetComprehensiveData(mOrganizationId, mVariable, mStartDate, mEndDate).CaculateValue.ToString("0.00").Trim();

                    mStructTable.Rows[i]["ComprehensivePowerConsumption"] = Value;
                }                         
            }
            for (int i = 2; i < mStructTable.Rows.Count; i++)
            {
              
                if ((mStructTable.Rows[i]["OrganizationID"] == mStructTable.Rows[i - 1]["OrganizationID"])&& (mStructTable.Rows[i]["OrganizationID"]== mStructTable.Rows[i-2]["OrganizationID"]) && mStructTable.Rows[i]["OrganizationID"].ToString().Trim().Contains("clinker"))
                {
                    //熟料计算电耗
                    decimal clinkerProcessElectricityQuantity = Convert.ToDecimal(mStructTable.Rows[i]["ElectricityQuantity"]) + Convert.ToDecimal(mStructTable.Rows[i-1]["ElectricityQuantity"]) +
                        Convert.ToDecimal(mStructTable.Rows[i -2]["ElectricityQuantity"]);
                    decimal ClinkerOutput = Convert.ToDecimal(mStructTable.Rows[i]["MaterialWeight"]);
                    mStructTable.Rows[i]["CalculationPowerConsumption"] = ClinkerOutput == 0 ? 0 : (mStructTable.Rows[i]["CalculationPowerConsumption"] = Convert.ToDouble(clinkerProcessElectricityQuantity / ClinkerOutput).ToString("0.00"));
                    mStructTable.Rows[i - 1]["CalculationPowerConsumption"] = mStructTable.Rows[i]["CalculationPowerConsumption"];
                    mStructTable.Rows[i - 2]["CalculationPowerConsumption"] = mStructTable.Rows[i]["CalculationPowerConsumption"];

                    //熟料综合电耗
                    string mOrganizationId = mStructTable.Rows[i]["OrganizationId"].ToString().Trim();
                    string mVariable = mStructTable.Rows[i]["ComprehensivePowerConsumptionVariable"].ToString().Trim();
                    string Value = ComprehensiveConsumptionService.GetComprehensiveData(mOrganizationId, mVariable, mStartDate, mEndDate).CaculateValue.ToString("0.00").Trim();

                    mStructTable.Rows[i]["ComprehensivePowerConsumption"] = Value;
                    mStructTable.Rows[i-1]["ComprehensivePowerConsumption"] = Value;
                    mStructTable.Rows[i-2]["ComprehensivePowerConsumption"] = Value;
                }
            }

            return mStructTable;
        }
        private static DataTable GetStructTablebyOrganizationId(string organizationId) 
        {
            string ConnectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionString);
            string mySql = @"  select OrganizationID,Name from [NXJC].[dbo].[tz_Material]  where OrganizationID like @organizationId+'%' 
	                          and Enable=1 and State=0
	                          order by OrganizationID,Name";
            DataTable originalTable = dataFactory.Query(mySql, new SqlParameter("@organizationId", organizationId));
            DataTable table = GetTableStructrue();
            for(int i=0;i<originalTable.Rows.Count;i++)
            {
                if (originalTable.Rows[i]["OrganizationID"].ToString().Trim().Contains("clinker")) {
                    table.Rows.Add(originalTable.Rows[i]["OrganizationID"], originalTable.Rows[i]["Name"], "生料制备", "rawMaterialsPreparation_ElectricityQuantity", null, "clinker_MixtureMaterialsOutput");
                    table.Rows.Add(originalTable.Rows[i]["OrganizationID"], originalTable.Rows[i]["Name"], "煤粉制备", "coalPreparation_ElectricityQuantity", null, "clinker_PulverizedCoalOutput");
                    table.Rows.Add(originalTable.Rows[i]["OrganizationID"], originalTable.Rows[i]["Name"], "熟料制备", "clinkerPreparation_ElectricityQuantity", null, "clinker_ClinkerOutput", null, null, null, "clinker_ElectricityConsumption_Comprehensive");            
                }
                else if (originalTable.Rows[i]["OrganizationID"].ToString().Trim().Contains("cementmill")) {
                    table.Rows.Add(originalTable.Rows[i]["OrganizationID"], originalTable.Rows[i]["Name"], "水泥制备", "cementPreparation_ElectricityQuantity", null, "cement_CementOutput", null, null, null, "cementmill_ElectricityConsumption_Comprehensive");    
                }                    
            }
            return table;
        }
        private static DataTable GetTableStructrue() 
        {
            DataTable structrueTable = new DataTable();
            structrueTable.Columns.Add("OrganizationId",typeof(string));
            structrueTable.Columns.Add("ProcessName", typeof(string));
            structrueTable.Columns.Add("Type", typeof(string));
            structrueTable.Columns.Add("ElectricityQuantityVariable", typeof(string));
            structrueTable.Columns.Add("ElectricityQuantity", typeof(decimal));
            structrueTable.Columns.Add("MaterialWeightVariable", typeof(string));
            structrueTable.Columns.Add("MaterialWeight", typeof(decimal));
            structrueTable.Columns.Add("PowerConsumption", typeof(decimal));
            structrueTable.Columns.Add("CalculationPowerConsumption", typeof(decimal));
            structrueTable.Columns.Add("ComprehensivePowerConsumptionVariable", typeof(string));
            structrueTable.Columns.Add("ComprehensivePowerConsumption", typeof(string));
            return structrueTable;       
        }
    }

}
