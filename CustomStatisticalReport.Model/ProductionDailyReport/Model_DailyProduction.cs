using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomStatisticalReport.Model.ProductionDailyReport
{
    public class CementSpecsInfo
    {
        private string _SpecsName;
        private decimal _RunTime;                   //运行时间
        private decimal _MaterialWeight;            //产量
        private List<DateTime[]> _StatisitalTimes;
        public CementSpecsInfo()
        {
            _SpecsName = "";
            _RunTime = 0.0m;
            _MaterialWeight = 0.0m;
            _StatisitalTimes = new List<DateTime[]>();
        }
        public string SpecsName
        {
            get
            {
                return _SpecsName;
            }
            set
            {
                _SpecsName = value;
            }
        }
        public decimal RunTime
        {
            get
            {
                return _RunTime;
            }
            set
            {
                _RunTime = value;
            }
        }
        public decimal MaterialWeight
        {
            get
            {
                return _MaterialWeight;
            }
            set
            {
                _MaterialWeight = value;
            }
        }
        public List<DateTime[]> StatisitalTimes
        {
            get
            {
                return _StatisitalTimes;
            }
            set
            {
                _StatisitalTimes = value;
            }
        }
    }
    public class CementEquipmentOutputInfo
    {
        private string _MaterialColumn;             //水泥产量字段名
        private string _MaterialDataTableName;       //水泥产量表名
        private string _MaterialDataBaseName;       //水泥产量数据库名

        private Dictionary<string, CementSpecsInfo> _CementSpecsItemInfo;    //统计时间
        public CementEquipmentOutputInfo()
        {
            _MaterialColumn = "";
            _MaterialDataTableName = "";
            _MaterialDataBaseName = "";
            _CementSpecsItemInfo = new Dictionary<string, CementSpecsInfo>();
        }
        public string MaterialColumn
        {
            get
            {
                return _MaterialColumn;
            }
            set
            {
                _MaterialColumn = value;
            }
        }
        public string MaterialDataTableName
        {
            get
            {
                return _MaterialDataTableName;
            }
            set
            {
                _MaterialDataTableName = value;
            }
        }


        public string MaterialDataBaseName
        {
            get
            {
                return _MaterialDataBaseName;
            }
            set
            {
                _MaterialDataBaseName = value;
            }
        }

        public Dictionary<string, CementSpecsInfo> CementSpecsItemInfo
        {
            get
            {
                return _CementSpecsItemInfo;
            }
            set
            {
                _CementSpecsItemInfo = value;
            }
        }
    }; 
}
