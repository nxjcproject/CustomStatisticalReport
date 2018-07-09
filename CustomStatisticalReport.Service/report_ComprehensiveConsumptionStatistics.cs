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
        private static string mCementConsumption = null;
        private static string mClinkerConsumption = null;
        public static DataTable GetComprehensiveConsumptionDataInfo(string mStartTime, string mEndTime)
        {
            DataTable mOrgTable = GetFactoryOrganizationID();
            int count = mOrgTable.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                string mOrganizationID = mOrgTable.Rows[i]["OrganizationID"].ToString().Trim();
                GetConsumptionDataTable(mOrganizationID,mStartTime,mEndTime);
                mOrgTable.Rows[i]["cementConsumption"] = mCementConsumption;
                mOrgTable.Rows[i]["clinkerConsumption"] = mClinkerConsumption;                             
            }
            return mOrgTable;
        }
        private static DataTable GetFactoryOrganizationID()
        {
            string mSql = @"SELECT ID
                                  ,OrganizationID
                                  ,LevelCode
                                  ,Name
                                  ,Type
                                  ,LevelType
                                  ,ENABLED
                                  ,'' as cementConsumption
                                  ,'' as clinkerConsumption                                 
                              FROM system_Organization
                              where ENABLED=1
                              and LevelType='Factory'
                              order by LevelCode";
            try
            {
                DataTable table = _datafactory.Query(mSql);
                return table;
            }
            catch {
                return null;
            }
        }
        private static void GetConsumptionDataTable(string mOrganizationID,string mStartTime,string mEndTime)
        {
            decimal mTatalPower = 0;//总用电量
            decimal mCementPower = 0;//水泥相关电量
            decimal mCemmentOutput = 0;//水泥产量
            decimal mClikerOutput = 0;//熟料产量
            string mOrgID = mOrganizationID;
            if (mOrgID == "zc_nxjc_ychc_yfcf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_ychc_lsf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric2_ElectricityQuantity','totalElectric3_ElectricityQuantity','totalElectric4_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric2_ElectricityQuantity' or VariableId='totalElectric3_ElectricityQuantity' or VariableId='totalElectric4_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_ychc_ndf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalPowerSupplyOfCementGrindingMill_ElectricityQuantity',
		                                                'packDustCollectingFan1_ElectricityQuantity','packingMachine_ElectricityQuantity',
							                            'cement_CementOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalPowerSupplyOfCementGrindingMill_ElectricityQuantity' or VariableId='packDustCollectingFan1_ElectricityQuantity' or VariableId='packingMachine_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mCementPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                mClinkerConsumption = "--";
            }
            else if (mOrgID == "zc_nxjc_qtx_efc")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric312_ElectricityQuantity','totalElectric318_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric312_ElectricityQuantity' or VariableId='totalElectric318_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_qtx_tys")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric4_ElectricityQuantity','totalElectric5_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric4_ElectricityQuantity' or VariableId='totalElectric5_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_byc_byf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('purchasedElectricity_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='purchasedElectricity_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_tsc_tsf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric1_ElectricityQuantity','totalElectric2_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric1_ElectricityQuantity' or VariableId='totalElectric2_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_znc_znf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric1_ElectricityQuantity','totalElectric2_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric1_ElectricityQuantity' or VariableId='totalElectric2_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_znc_zlf")
            {                
                    mCementConsumption = "--";
                    mClinkerConsumption = "--";
            }
            else if (mOrgID == "zc_nxjc_lpsc_lpsf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('purchasedElectricity_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='purchasedElectricity_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_whsmc_whsmf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('purchasedElectricity_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='purchasedElectricity_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_klqc_klqf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('purchasedElectricity_ElectricityQuantity',
                                                        'wasteHeatElectricityGeneration_ElectricityQuantity',
		                                                'cementmill_ElectricityQuantity','cementPacking_ElectricityQuantity',
							                            'cement_CementOutput','clinker_ClinkerOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='purchasedElectricity_ElectricityQuantity' or VariableId='wasteHeatElectricityGeneration_ElectricityQuantity'");
                mCementPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cementmill_ElectricityQuantity' or VariableId='cementPacking_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                mClikerOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='clinker_ClinkerOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                if (mClikerOutput != 0)
                {
                    mClinkerConsumption = ((mTatalPower - mCementPower) / mClikerOutput).ToString("0.00");//熟料综合电耗
                }
                else
                {
                    mClinkerConsumption = "0.00";
                }
            }
            else if (mOrgID == "zc_nxjc_szsc_szsf")
            {
                string mSql = @"SELECT B.VariableId
                                    ,B.VariableName
                                    ,B.OrganizationID
                                    ,SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
                                  FROM tz_Balance A,
                                       balance_Energy B
                                 where A.OrganizationID = @mOrganizationID
                                   and A.StaticsCycle = 'day'
                                   and A.BalanceId = B.KeyId
                                   and A.TimeStamp >= @mStartTime
                                   and A.TimeStamp <= @mEndTime
                                   and B.VariableId in ('totalElectric1_ElectricityQuantity',
							                            'cement_CementOutput')
                                   group by B.OrganizationID,B.VariableId,B.VariableName
                                   order by B.OrganizationID";
                SqlParameter[] paras ={ new SqlParameter("@mOrganizationID",mOrgID),
                                        new SqlParameter("@mStartTime",mStartTime),
                                        new SqlParameter("@mEndTime",mEndTime)};
                DataTable table = _datafactory.Query(mSql, paras);
                mTatalPower = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='totalElectric1_ElectricityQuantity'");
                mCemmentOutput = (decimal)table.Compute("sum(TotalPeakValleyFlatB)", "VariableId='cement_CementOutput'");
                if (mCemmentOutput != 0)
                {
                    mCementConsumption = (mTatalPower / mCemmentOutput).ToString("0.00");//水泥综合电耗
                }
                else
                {
                    mCementConsumption = "0.00";
                }
                mClinkerConsumption = "--";
            }
        }
    }
}
