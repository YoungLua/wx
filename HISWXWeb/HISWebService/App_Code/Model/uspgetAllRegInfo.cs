
using System;
using System.Data;

namespace Model
{
    /// <summary>
    ///uspgetSchInfo 的摘要说明
    /// </summary>
    public class uspgetAllRegInfo : absModel
    {
        private int _iD;
        protected string _doctorId = String.Empty;
        protected string _doctorName = String.Empty;
        protected string _doctorTitle = String.Empty;
        protected DateTime _regDate;
        protected string _regWeekDay = String.Empty;
        protected int _regTotalCount;
        protected int _regleaveCount;
        protected string _totalFee = String.Empty;
        protected string _regFee = String.Empty;
        protected string _treatFee = String.Empty;
        protected string _isTimeReg = String.Empty;
        protected string _shiftType = String.Empty;
        protected string _regStatus = String.Empty;
        protected string _startTime = String.Empty;
        protected string _endTime = String.Empty;
        protected int _regTotalCount1;
        protected int _regleaveCount1;
        protected string _locationId = String.Empty;
        protected string _iscancel = String.Empty;
        protected string _onlyId;

        public uspgetAllRegInfo()
        {
        }
        public uspgetAllRegInfo(IDataReader datareader)
        {
            Fill(datareader);
        }
        public override void Fill(IDataReader datareader)
        {
            if (datareader == null) return;

            if (!datareader.IsDBNull(datareader.GetOrdinal("doctorId")))
                _doctorId = datareader["doctorId"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("doctorName")))
                _doctorName = datareader["doctorName"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("doctorTitle")))
                _doctorTitle = datareader["doctorTitle"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("regDate")))
            {
                _regDate = Convert.ToDateTime(datareader["regDate"].ToString());
            }

            if (!datareader.IsDBNull(datareader.GetOrdinal("regWeekDay")))
                _regWeekDay = datareader["regWeekDay"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("regTotalCount")))
                _regTotalCount = int.Parse(datareader["regTotalCount"].ToString());

            if (!datareader.IsDBNull(datareader.GetOrdinal("regleaveCount")))
                _regleaveCount = int.Parse(datareader["regleaveCount"].ToString());

            if (!datareader.IsDBNull(datareader.GetOrdinal("totalFee")))
                _totalFee = datareader["totalFee"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("regFee")))
                _regFee = datareader["regFee"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("treatFee")))
                _treatFee = datareader["treatFee"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("isTimeReg")))
                _isTimeReg = datareader["isTimeReg"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("shiftType")))
                _shiftType = datareader["shiftType"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("regStatus")))
                _regStatus = datareader["regStatus"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("startTime")))
                _startTime = datareader["startTime"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("Timeend")))
                _endTime = datareader["Timeend"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("regTotalCount1")))
                _regTotalCount1 = int.Parse(datareader["regTotalCount1"].ToString());

            if (!datareader.IsDBNull(datareader.GetOrdinal("regleaveCount1")))
                _regleaveCount1 = int.Parse(datareader["regleaveCount1"].ToString());

            if (!datareader.IsDBNull(datareader.GetOrdinal("LocationId")))
                _locationId = datareader["LocationId"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("Iscancel")))
                _iscancel = datareader["Iscancel"].ToString();

            if (!datareader.IsDBNull(datareader.GetOrdinal("onlyId")))
                _onlyId = datareader["onlyId"].ToString();
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
        /// 医生名称
        /// </summary>
        public string doctorName
        {
            get { return _doctorName; }
            set { _doctorName = value; }
        }
        /// <summary>
        /// 医生职称
        /// </summary>
        public string doctorTitle
        {
            get { return _doctorTitle; }
            set { _doctorTitle = value; }
        }
        /// <summary>
        /// 出诊日期，格式：YYYY-MM-DD
        /// </summary>
        public DateTime regDate
        {
            get { return _regDate; }
            set { _regDate = value; }
        }
        /// <summary>
        /// 出诊日期对应的星期
        /// </summary>
        public string regWeekDay
        {
            get { return _regWeekDay; }
            set { _regWeekDay = value; }
        }
        /// <summary>
        /// 该时段可预约的总号源数
        /// </summary>
        public int regTotalCount
        {
            get { return _regTotalCount; }
            set { _regTotalCount = value; }
        }
        /// <summary>
        /// 该时段剩余号源数
        /// </summary>
        public int regleaveCount
        {
            get { return _regleaveCount; }
            set { _regleaveCount = value; }
        }
        /// <summary>
        /// 总金额（单位“分”）
        /// </summary>
        public string totalFee
        {
            get { return _totalFee; }
            set { _totalFee = value; }
        }
        /// <summary>
        /// 挂号费(单位“分”)
        /// </summary>
        public string regFee
        {
            get { return _regFee; }
            set { _regFee = value; }
        }
        /// <summary>
        /// 诊疗费(单位“分”)
        /// </summary>
        public string treatFee
        {
            get { return _treatFee; }
            set { _treatFee = value; }
        }
        /// <summary>
        /// 是否有分时
        /// </summary>
        public string isTimeReg
        {
            get { return _isTimeReg; }
            set { _isTimeReg = value; }
        }
        /// <summary>
        /// 时段 1:上午 2:下午 3:晚上
        /// </summary>
        public string shiftType
        {
            get { return _shiftType; }
            set { _shiftType = value; }
        }
        /// <summary>
        /// 出诊状态0-停诊1-出诊2-暂未开放
        /// </summary>
        public string regStatus
        {
            get { return _regStatus; }
            set { _regStatus = value; }
        }
        /// <summary>
        /// 分时开始时间，格式：HH:MI
        /// </summary>
        public string startTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        /// <summary>
        /// 分时结束时间，格式：HH:MI
        /// </summary>
        public string endTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
        /// <summary>
        /// 该时段可预约的总号源数
        /// </summary>
        public int regTotalCount1
        {
            get { return _regTotalCount1; }
            set { _regTotalCount1 = value; }
        }
        /// <summary>
        /// 该时段剩余号源数
        /// </summary>
        public int regleaveCount1
        {
            get { return _regleaveCount1; }
            set { _regleaveCount1 = value; }
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

        public string onlyId
        {
            get { return _onlyId; }
            set { _onlyId = value; }
        }
    }
}
