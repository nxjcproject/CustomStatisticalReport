using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CustomStatisticalReport.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System.Data.SqlClient;

namespace CustomStatisticalReport.Service
{
    public class report_ComprehensiveConsumptionStatistics
    {
        private static string _connectionString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _datafactory = new SqlServerDataFactory(_connectionString);
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

        public static DataTable GetComprehensiveConsumptionDataInfo(string mStartTime, string mEndTime)
        {
            DataTable mOrgTable = GetFactoryOrganizationID();
            DataTable mBalanceEnergyTable = GetBalanceEnergyDataTable(mStartTime, mEndTime);

            int count = mOrgTable.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                string mOrganizationID = mOrgTable.Rows[i]["OrganizationID"].ToString().Trim();
                GetConsumptionData(mOrganizationID, mBalanceEnergyTable);
                mOrgTable.Rows[i]["TotalElectricityConsumptionOfClinker"] = mTotalElectricityConsumptionOfClinker.ToString("0.0");
                mOrgTable.Rows[i]["TotalElectricityConsumptionOfCement"] = mTotalElectricityConsumptionOfCement.ToString("0.0");
                mOrgTable.Rows[i]["ClikerOutput"] = mClikerOutput.ToString("0.0");
                mOrgTable.Rows[i]["CemmentOutput"] = mCemmentOutput.ToString("0.0");
                mOrgTable.Rows[i]["CemmentOfClinkerInput"] = mCemmentOfClinkerInput.ToString("0.0");
                mOrgTable.Rows[i]["CemmentOfClinkerOutsourcingInput"] = mCemmentOfClinkerOutsourcingInput.ToString("0.0");
                
                mOrgTable.Rows[i]["clinkerComprehensiveConsumption"] = mClinkerConsumption;
                mOrgTable.Rows[i]["cementComprehensiveConsumption"] = mCementComprehensiveConsumption;
                mOrgTable.Rows[i]["cementConsumption"] = mCementConsumption;
            }
            return mOrgTable;
        }
        private static DataTable GetFactoryOrganizationID()
        {
            string mSql = @"SELECT A.Name as CompanyName
                                  ,B.OrganizationID
                                  ,B.Name as FactoryName
                                  ,'' as TotalElectricityConsumptionOfClinker
                                  ,'' as TotalElectricityConsumptionOfCement
                                  ,'' as ClikerOutput
                                  ,'' as CemmentOutput
                                  ,'' as CemmentOfClinkerInput
                                  ,'' as CemmentOfClinkerOutsourcingInput
                                  ,'' as clinkerComprehensiveConsumption
                                  ,'' as cementComprehensiveConsumption
                                  ,'' as cementConsumption                                                                                                  
                              FROM system_Organization A,
                                   system_Organization B
                              where A.ENABLED = 1
                              and A.LevelType = 'Company'
                              and B.ENABLED = 1
                              and B.LevelCode like A.LevelCode + '%'
                              and B.LevelType = 'Factory'
                              and B.OrganizationID <> 'zc_nxjc_znc_zlf'
                              order by A.LevelCode,B.LevelCode";
            try
            {
                DataTable table = _datafactory.Query(mSql);
                return table;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有厂的数据
        /// </summary>
        /// <param name="mStartTime"></param>
        /// <param name="mEndTime"></param>
        /// <returns></returns>
        private static DataTable GetBalanceEnergyDataTable(string mStartTime, string mEndTime)
        {
            string mSql = @"SELECT A.OrganizationID as FactoryOrganizationID
                                   ,B.VariableId
                                   ,B.VariableName
                                   ,B.OrganizationID
                                   ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                 FROM tz_Balance A,
                                      balance_Energy B
                                where A.StaticsCycle = 'day'
                                  and A.BalanceId = B.KeyId
                                  and A.TimeStamp >= @mStartTime
                                  and A.TimeStamp <= @mEndTime
                                  and B.VariableId in ('totalElectricityConsumptionOfClinker_ElectricityQuantity',
                                                       'totalElectricityConsumptionOfCement_ElectricityQuantity',
		                                               'clinker_ClinkerOutput',
                                                       'cement_CementOutput',
                                                       'clinker_ClinkerInput', 
                                                       'clinker_ClinkerOutsourcingInput')
                                  group by A.OrganizationID,B.OrganizationID,B.VariableId,B.VariableName
                                  order by B.OrganizationID";
            SqlParameter[] paras = { new SqlParameter("@mStartTime",mStartTime),
                                     new SqlParameter("@mEndTime",mEndTime)};
            try
            {
                DataTable table = _datafactory.Query(mSql, paras);
                return table;
            }
            catch
            {
                return null;
            }

        }
        private static void GetConsumptionData(string mOrganizationID, DataTable table)
        {
            string mFactoryOrganizationID = mOrganizationID;
            DataTable mAllBalanceEnergyTable = table;
            DataRow[] drs = mAllBalanceEnergyTable.Select("FactoryOrganizationID='" + mFactoryOrganizationID + "'");
            DataTable mBalanceEnergyTable = null;
            if (drs.Length > 0)
            {
                mBalanceEnergyTable = drs.CopyToDataTable();

                //totalElectricityConsumptionOfClinker_ElectricityQuantity  熟料总共耗电量
                //totalElectricityConsumptionOfCement_ElectricityQuantity   水泥总共耗电量
                //clinker_ClinkerOutput                 熟料产量
                //cement_CementOutput                   水泥产量
                //clinker_ClinkerInput                  水泥磨自产熟料消耗量
                //clinker_ClinkerOutsourcingInput       水泥磨外购熟料消耗量

                if (mBalanceEnergyTable.Select("VariableId='totalElectricityConsumptionOfClinker_ElectricityQuantity'").Length != 0)
                {
                    mTotalElectricityConsumptionOfClinker = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectricityConsumptionOfClinker_ElectricityQuantity'");
                }
                else
                {
                    mTotalElectricityConsumptionOfClinker = 0M;
                }

                if (mBalanceEnergyTable.Select("VariableId='totalElectricityConsumptionOfCement_ElectricityQuantity'").Length != 0)
                {
                    mTotalElectricityConsumptionOfCement = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectricityConsumptionOfCement_ElectricityQuantity'");
                }
                else
                {
                    mTotalElectricityConsumptionOfCement = 0M;
                }

                if (mBalanceEnergyTable.Select("VariableId='clinker_ClinkerOutput'").Length != 0)
                {
                    mClikerOutput = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                }
                else
                {
                    mClikerOutput = 0M;
                }

                if (mBalanceEnergyTable.Select("VariableId='cement_CementOutput'").Length != 0)
                {
                    mCemmentOutput = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                }
                else
                {
                    mCemmentOutput = 0M;
                }

                if (mBalanceEnergyTable.Select("VariableId='clinker_ClinkerInput'").Length != 0)
                {
                    mCemmentOfClinkerInput = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerInput'");
                }
                else
                {
                    mCemmentOfClinkerInput = 0M;
                }

                if (mBalanceEnergyTable.Select("VariableId='clinker_ClinkerOutsourcingInput'").Length != 0)
                {
                    mCemmentOfClinkerOutsourcingInput = (decimal)mBalanceEnergyTable.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutsourcingInput'");
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
