
using System;
using System.Data;

namespace Model
{
    /// <summary>
    ///uspgetSchInfo 的摘要说明
    /// </summary>
    public class uspgetSchInfo : absModel
    {
        private int _iD;
        protected string _doctorId = String.Empty;
        protected int _shiftId;
        protected int _shiftType;
        protected DateTime _schDate;
        protected int _schRegTypeId;
        protected double _schFee;
        protected double _schRegfee;
        protected double _schTreatfee;
        protected string _schBegintime=String.Empty;
        protected string _schEndtime=String.Empty;
        protected int _schRegMax;
        protected int _schRegCount;
        protected int _schLimited;
        protected int _schSegmented;
        protected string _locationId = String.Empty;
        protected string _iscancel = String.Empty;
        protected string _diagRoom = String.Empty;
        protected string _specialtyRoom = String.Empty;

        public uspgetSchInfo()
        {
        }
        public uspgetSchInfo(IDataReader datareader)
        {
            Fill(datareader);
        }
        public override void Fill(IDataReader datareader)
        {
            if (datareader == null) return;

            if (!datareader.IsDBNull(datareader.GetOrdinal("doctorId")))
                _doctorId = datareader["doctorId"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("shiftId")))
            {
                _shiftId = int.Parse(datareader["shiftId"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("shiftType")))
            {
                _shiftType = int.Parse(datareader["shiftType"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schDate")))
                _schDate = DateTime.Parse(datareader["schDate"].ToString());

            if (!datareader.IsDBNull(datareader.GetOrdinal("schRegTypeId")))
            {
                _schRegTypeId = int.Parse(datareader["schRegTypeId"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schFee")))
            {
                _schFee = double.Parse(datareader["schFee"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schRegfee")))
            {
                _schRegfee = double.Parse(datareader["schRegfee"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schTreatfee")))
            {
                _schTreatfee = double.Parse(datareader["schTreatfee"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schBegintime")))
                _schBegintime = datareader["schBegintime"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("schEndtime")))
                _schEndtime = datareader["schEndtime"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("schRegMax")))
            {
                _schRegMax = int.Parse(datareader["schRegMax"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schRegCount")))
            {
                _schRegCount = int.Parse(datareader["schRegCount"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schLimited")))
            {
                _schLimited = int.Parse(datareader["schLimited"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("schSegmented")))
            {
                _schSegmented = int.Parse(datareader["schSegmented"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("LocationId")))
                _locationId = datareader["LocationId"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("Iscancel")))
                _iscancel = datareader["Iscancel"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("DiagRoom")))
                _diagRoom = datareader["DiagRoom"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("SpecialtyRoom")))
                _specialtyRoom = datareader["SpecialtyRoom"].ToString();
        }

        public override int ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        /// <summary>
        /// 医生ID
        /// </summary>
        public string doctorId
        {
            get { return _doctorId; }
            set { _doctorId = value; }
        }
        /// <summary>
        /// 班别ID
        /// </summary>
        public int shiftId
        {
            get { return _shiftId; }
            set { _shiftId = value; }
        }
        /// <summary>
        /// 班别类型 (1:上午 2：下午 3:夜班)
        /// </summary>
        public int shiftType
        {
            get { return _shiftType; }
            set { _shiftType = value; }
        }
        /// <summary>
        /// 排班日期
        /// </summary>
        public DateTime schDate
        {
            get { return _schDate; }
            set { _schDate = value; }
        }
        /// <summary>
        /// 挂号类型ID
        /// </summary>
        public int schRegTypeId
        {
            get { return _schRegTypeId; }
            set { _schRegTypeId = value; }
        }
        /// <summary>
        /// 总费用
        /// </summary>
        public double schFee
        {
            get { return _schFee; }
            set { _schFee = value; }
        }
        /// <summary>
        /// 挂号费
        /// </summary>
        public double schRegfee
        {
            get { return _schRegfee; }
            set { _schRegfee = value; }
        }
        /// <summary>
        /// 诊疗费
        /// </summary>
        public double schTreatfee
        {
            get { return _schTreatfee; }
            set { _schTreatfee = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string schBegintime
        {
            get { return _schBegintime; }
            set { _schBegintime = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string schEndtime
        {
            get { return _schEndtime; }
            set { _schEndtime = value; }
        }
        /// <summary>
        /// 最大挂号量
        /// </summary>
        public int schRegMax
        {
            get { return _schRegMax; }
            set { _schRegMax = value; }
        }
        /// <summary>
        /// 已挂号量
        /// </summary>
        public int schRegCount
        {
            get { return _schRegCount; }
            set { _schRegCount = value; }
        }
        /// <summary>
        /// 是否限号
        /// </summary>
        public int schLimited
        {
            get { return _schLimited; }
            set { _schLimited = value; }
        }
        /// <summary>
        /// 是否分时段（1.分时段，0不分时段）
        /// </summary>
        public int schSegmented
        {
            get { return _schSegmented; }
            set { _schSegmented = value; }
        }

        public string LocationId
        {
            get { return _locationId; }
            set { _locationId = value; }
        }

        public string Iscancel
        {
            get { return _iscancel; }
            set { _iscancel = value; }
        }

        public string DiagRoom
        {
            get { return _diagRoom; }
            set { _diagRoom = value; }
        }

        public string SpecialtyRoom
        {
            get { return _specialtyRoom; }
            set { _specialtyRoom = value; }
        }
    }
}
