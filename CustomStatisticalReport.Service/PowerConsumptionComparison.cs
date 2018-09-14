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
        private static string _connString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connString);
        private static readonly AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1 AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1(_dataFactory);

        private const decimal mClinkerOutsourcingInputConsumption = 65M;//外购熟料电耗65
        private static decimal mTotalElectricityConsumptionOfClinker = 0;//某厂熟料总共耗电量
        private static decimal mTotalElectricityConsumptionOfCement = 0;//某厂水泥总共耗电量
        private static decimal mCemmentOutput = 0;//某厂水泥产量
        private static decimal mClikerOutput = 0;//某厂熟料产量
        private static decimal mCemmentOfClinkerInput = 0;//某厂水泥磨自产熟料消耗量
        private static decimal mCemmentOfClinkerOutsourcingInput = 0;//某厂水泥磨外购熟料消耗量
        private static string mCementConsumption = null;//水泥单耗电耗
        private static string mClinkerConsumption = null;//熟料综合电耗
        private static string mCementComprehensiveConsumption = null;//水泥综合电耗

        public static DataTable GetReportData(string organizationId, string mStartDate, string mEndDate)
        {
            string mOrganizationLevelCode = null;//为计算综合电耗，获取分厂的LevelCode

            DataTable mResultTable = GetResultTable(organizationId, out mOrganizationLevelCode);
            DataTable mBalanceEnergyTable = GetBalanceEnergyDataTable(organizationId, mStartDate, mEndDate);

            decimal mRawMaterialsPreparationOutput = 0M;//生料产量
            decimal mRawMaterialsPreparationElectricityQuantity = 0M;//生料制备电量
            decimal mClinkerPreparationElectricityQuantity = 0M;//熟料制备电量
            decimal mCementPreparationElectricityQuantity = 0M;//水泥制备电量

            if (mBalanceEnergyTable.Select("VariableId='clinker_MixtureMaterialsOutput'").Length != 0)
            {
                mRawMaterialsPreparationOutput = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_MixtureMaterialsOutput'");
            }
            if (mBalanceEnergyTable.Select("VariableId='rawMaterialsPreparation_ElectricityQuantity'").Length != 0)
            {
                mRawMaterialsPreparationElectricityQuantity = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='rawMaterialsPreparation_ElectricityQuantity'");
            }
            if (mBalanceEnergyTable.Select("VariableId='clinkerPreparation_ElectricityQuantity'").Length != 0)
            {
                mClinkerPreparationElectricityQuantity = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinkerPreparation_ElectricityQuantity'");
            }
            if (mBalanceEnergyTable.Select("VariableId='cementPreparation_ElectricityQuantity'").Length != 0)
            {
                mCementPreparationElectricityQuantity = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementPreparation_ElectricityQuantity'");
            }

            GetCementComprehensiveConsumption(organizationId, mBalanceEnergyTable);//计算水泥综合电耗

            for (int i = 0; i < mResultTable.Rows.Count; i++)
            {
                if (mResultTable.Rows[i]["OrganizationType"].ToString().Trim() == "Factory")
                {
                    mResultTable.Rows[i]["ElectricityQuantity"] = (mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity + mCementPreparationElectricityQuantity).ToString("0.0");
                    mResultTable.Rows[i]["MaterialWeight"] = mCemmentOutput.ToString("0.0");
                    mResultTable.Rows[i]["PowerConsumption"] = mCemmentOutput >= 10 ? ((mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity + mCementPreparationElectricityQuantity) / mCemmentOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["CalculationPowerConsumption"] = mCemmentOutput >= 10 ? ((mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity + mCementPreparationElectricityQuantity) / mCemmentOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["ComprehensivePowerConsumption"] = "";
                    mResultTable.Rows[i]["ComprehensiveCementConsumption"] = mCementComprehensiveConsumption;
                    mResultTable.Rows[i]["state"] = "open";
                }
                if (mResultTable.Rows[i]["OrganizationType"].ToString().Trim() == "clinker")
                {
                    mResultTable.Rows[i]["ElectricityQuantity"] = (mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity).ToString("0.0");
                    mResultTable.Rows[i]["MaterialWeight"] = mClikerOutput.ToString("0.0");
                    mResultTable.Rows[i]["PowerConsumption"] = mClikerOutput >= 10 ? ((mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity) / mClikerOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["CalculationPowerConsumption"] = mClikerOutput >= 10 ? ((mRawMaterialsPreparationElectricityQuantity + mClinkerPreparationElectricityQuantity) / mClikerOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["ComprehensivePowerConsumption"] = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", mStartDate.ToString(), mEndDate.ToString(), mOrganizationLevelCode).CaculateValue.ToString("0.0");
                    mResultTable.Rows[i]["ComprehensiveCementConsumption"] = "";
                    mResultTable.Rows[i]["state"] = "open";
                }
                if (mResultTable.Rows[i]["OrganizationType"].ToString().Trim() == "rawMaterialsPreparation")
                {
                    mResultTable.Rows[i]["ElectricityQuantity"] = mRawMaterialsPreparationElectricityQuantity.ToString("0.0");
                    mResultTable.Rows[i]["MaterialWeight"] = mRawMaterialsPreparationOutput.ToString("0.0");
                    mResultTable.Rows[i]["PowerConsumption"] = mRawMaterialsPreparationOutput >= 1 ? (mRawMaterialsPreparationElectricityQuantity / mRawMaterialsPreparationOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["CalculationPowerConsumption"] = mRawMaterialsPreparationOutput >= 1 ? (mRawMaterialsPreparationElectricityQuantity / mRawMaterialsPreparationOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["ComprehensivePowerConsumption"] = "";//AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", mStartDate.ToString(), mEndDate.ToString(), mOrganizationLevelCode).CaculateValue.ToString("0.0");
                    mResultTable.Rows[i]["ComprehensiveCementConsumption"] = "";
                    mResultTable.Rows[i]["state"] = "open";
                }
                if (mResultTable.Rows[i]["OrganizationType"].ToString().Trim() == "clinkerPreparation")
                {
                    mResultTable.Rows[i]["ElectricityQuantity"] = mClinkerPreparationElectricityQuantity.ToString("0.0");
                    mResultTable.Rows[i]["MaterialWeight"] = mClikerOutput.ToString("0.0");
                    mResultTable.Rows[i]["PowerConsumption"] = mClikerOutput >= 10 ? (mClinkerPreparationElectricityQuantity / mClikerOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["CalculationPowerConsumption"] = mClikerOutput >= 10 ? (mClinkerPreparationElectricityQuantity / mClikerOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["ComprehensivePowerConsumption"] = "";//AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", mStartDate.ToString(), mEndDate.ToString(), mOrganizationLevelCode).CaculateValue.ToString("0.0");
                    mResultTable.Rows[i]["ComprehensiveCementConsumption"] = "";
                    mResultTable.Rows[i]["state"] = "open";
                }
                if (mResultTable.Rows[i]["OrganizationType"].ToString().Trim() == "cementilment")
                {
                    mResultTable.Rows[i]["ElectricityQuantity"] = mCementPreparationElectricityQuantity.ToString("0.0");
                    mResultTable.Rows[i]["MaterialWeight"] = mCemmentOutput.ToString("0.0");
                    mResultTable.Rows[i]["PowerConsumption"] = mCemmentOutput >= 10 ? (mCementPreparationElectricityQuantity / mCemmentOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["CalculationPowerConsumption"] = mCemmentOutput >= 10 ? (mCementPreparationElectricityQuantity / mCemmentOutput).ToString("0.0") : "0.0";
                    mResultTable.Rows[i]["ComprehensivePowerConsumption"] = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", mStartDate.ToString(), mEndDate.ToString(), mOrganizationLevelCode).CaculateValue.ToString("0.0");
                    mResultTable.Rows[i]["ComprehensiveCementConsumption"] = "";
                    mResultTable.Rows[i]["state"] = "open";
                }
            }
            return mResultTable;
        }

        private static DataTable GetResultTable(string organizationId, out string mOrganizationLevelCode)
        {
            string mSql = @"SELECT A.Type
                              FROM system_Organization A,
                                   system_Organization B
                              WHERE B.OrganizationID = @OrganizationID
                                AND A.LevelCode LIKE B.LevelCode + '%'
	                            AND (A.Type = '熟料' OR A.Type = '水泥磨' OR A.Type = '分厂')
                              GROUP BY A.Type
                              ORDER BY A.Type";

            string mySql = @"SELECT LevelCode, Name
                                    FROM system_Organization
                                    WHERE OrganizationID = @OrganizationID";

            SqlParameter mpara = new SqlParameter("@OrganizationID", organizationId);
            SqlParameter mypara = new SqlParameter("@OrganizationID", organizationId);

            DataTable mProductionLineTable = _dataFactory.Query(mSql, mpara);
            DataTable mOrganizationTable = _dataFactory.Query(mySql, mypara);
            mOrganizationLevelCode = mOrganizationTable.Rows[0]["LevelCode"].ToString().Trim();

            DataTable mResultTable = GetResultTableStructrue();
            for (int i = 0; i < mProductionLineTable.Rows.Count; i++)
            {
                if (mProductionLineTable.Rows[i]["Type"].ToString().Trim() == "分厂")
                {
                    mResultTable.Rows.Add(mOrganizationTable.Rows[0]["Name"].ToString(), "Factory", "R01");
                }
                if (mProductionLineTable.Rows[i]["Type"].ToString().Trim() == "熟料")
                {
                    mResultTable.Rows.Add("熟料", "clinker", "R0101");
                    mResultTable.Rows.Add("生料制备", "rawMaterialsPreparation", "R010101");
                    mResultTable.Rows.Add("熟料制备", "clinkerPreparation", "R010102");
                }
                if (mProductionLineTable.Rows[i]["Type"].ToString().Trim() == "水泥磨")
                {
                    mResultTable.Rows.Add("水泥", "cementilment", "R0102");
                }
            }
            return mResultTable;
        }

        private static DataTable GetBalanceEnergyDataTable(string mOrganizationID, string mStartDate, string mEndDate)
        {
            string mSql = @"SELECT A.OrganizationID as FactoryOrganizationID
                                   ,B.VariableId
                                   ,B.VariableName
                                   ,B.OrganizationID
                                   ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                 FROM tz_Balance A,
                                      balance_Energy B
                                where A.OrganizationID = @OrganizationID
                                  and A.StaticsCycle = 'day'
                                  and A.BalanceId = B.KeyId
                                  and A.TimeStamp >= @mStartTime
                                  and A.TimeStamp <= @mEndTime
                                  and B.VariableId in ('totalElectricityConsumptionOfClinker_ElectricityQuantity',  --熟料总共耗电量
                                                       'totalElectricityConsumptionOfCement_ElectricityQuantity',   --水泥总共耗电量
                                                       'clinker_MixtureMaterialsOutput','rawMaterialsPreparation_ElectricityQuantity',--生料制备产量、电量
		                                               'clinker_ClinkerOutput','clinkerPreparation_ElectricityQuantity',--熟料制备产量、电量
                                                       'cement_CementOutput','cementPreparation_ElectricityQuantity',--水泥制备产量、电量
                                                       'clinker_ClinkerInput', --水泥磨自产熟料消耗量
                                                       'clinker_ClinkerOutsourcingInput')   --水泥磨外购熟料消耗量
                                  group by A.OrganizationID,B.OrganizationID,B.VariableId,B.VariableName
                                  order by B.OrganizationID";
            SqlParameter[] paras = { new SqlParameter("@OrganizationID",mOrganizationID),
                                     new SqlParameter("@mStartTime",mStartDate),
                                     new SqlParameter("@mEndTime",mEndDate)};
            try
            {
                DataTable table = _dataFactory.Query(mSql, paras);
                return table;
            }
            catch
            {
                return null;
            }

        }

        private static DataTable GetResultTableStructrue()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ProductionLineName", typeof(string));
            table.Columns.Add("OrganizationType", typeof(string));
            table.Columns.Add("LevelCode", typeof(string));
            table.Columns.Add("ElectricityQuantity", typeof(string));
            table.Columns.Add("MaterialWeight", typeof(string));
            table.Columns.Add("PowerConsumption", typeof(string));
            table.Columns.Add("CalculationPowerConsumption", typeof(string));
            table.Columns.Add("ComprehensivePowerConsumption", typeof(string));
            table.Columns.Add("ComprehensiveCementConsumption", typeof(string));
            table.Columns.Add("state", typeof(string));
            return table;
        }

        private static void GetCementComprehensiveConsumption(string mFactoryOrganizationID, DataTable table)
        {
            DataTable mAllBalanceEnergyTable = table;
            if (mAllBalanceEnergyTable.Rows.Count > 0)
            {
                //totalElectricityConsumptionOfClinker_ElectricityQuantity  熟料总共耗电量
                //totalElectricityConsumptionOfCement_ElectricityQuantity   水泥总共耗电量
                //clinker_ClinkerOutput                 熟料产量
                //cement_CementOutput                   水泥产量
                //clinker_ClinkerInput                  水泥磨自产熟料消耗量
                //clinker_ClinkerOutsourcingInput       水泥磨外购熟料消耗量

                if (mAllBalanceEnergyTable.Select("VariableId='totalElectricityConsumptionOfClinker_ElectricityQuantity'").Length != 0)
                {
                    mTotalElectricityConsumptionOfClinker = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectricityConsumptionOfClinker_ElectricityQuantity'");
                }
                else
                {
                    mTotalElectricityConsumptionOfClinker = 0M;
                }

                if (mAllBalanceEnergyTable.Select("VariableId='totalElectricityConsumptionOfCement_ElectricityQuantity'").Length != 0)
                {
                    mTotalElectricityConsumptionOfCement = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectricityConsumptionOfCement_ElectricityQuantity'");
                }
                else
                {
                    mTotalElectricityConsumptionOfCement = 0M;
                }

                if (mAllBalanceEnergyTable.Select("VariableId='clinker_ClinkerOutput'").Length != 0)
                {
                    mClikerOutput = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                }
                else
                {
                    mClikerOutput = 0M;
                }

                if (mAllBalanceEnergyTable.Select("VariableId='cement_CementOutput'").Length != 0)
                {
                    mCemmentOutput = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                }
                else
                {
                    mCemmentOutput = 0M;
                }

                if (mAllBalanceEnergyTable.Select("VariableId='clinker_ClinkerInput'").Length != 0)
                {
                    mCemmentOfClinkerInput = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerInput'");
                }
                else
                {
                    mCemmentOfClinkerInput = 0M;
                }

                if (mAllBalanceEnergyTable.Select("VariableId='clinker_ClinkerOutsourcingInput'").Length != 0)
                {
                    mCemmentOfClinkerOutsourcingInput = (decimal)mAllBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutsourcingInput'");
                }
                else
                {
                    mCemmentOfClinkerOutsourcingInput = 0M;
                }

                if (mClikerOutput >= 10)//默认熟料产量小于10，电耗即为0
                {
                    mClinkerConsumption = (mTotalElectricityConsumptionOfClinker / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }

                if (mCemmentOutput >= 10)//默认水泥磨产量小于10，电耗即为0
                {
                    mCementConsumption = (mTotalElectricityConsumptionOfCement / mCemmentOutput).ToString("0.00");//水泥单耗电耗
                    if (mFactoryOrganizationID == "zc_nxjc_szsc_szsf")//石嘴山分厂的外购熟料也当成是自产熟料计算，与宁东相同
                    {
                        mCementComprehensiveConsumption = mCementConsumption;
                    }
                    else
                    {
                        mCementComprehensiveConsumption = (((mCemmentOfClinkerOutsourcingInput * mClinkerOutsourcingInputConsumption) + decimal.Parse(mClinkerConsumption) * mCemmentOfClinkerInput + mTotalElectricityConsumptionOfCement) /
                           mCemmentOutput).ToString("0.00");//水泥综合电耗
                    }
                }
                else
                {
                    mCementConsumption = "0.00";
                    mCementComprehensiveConsumption = "0.00";
                }
            }
            else
            {
                mCementConsumption = "0.00";
                mClinkerConsumption = "0.00";
                mCementComprehensiveConsumption = "0.00";
            }

        }
    }

}
