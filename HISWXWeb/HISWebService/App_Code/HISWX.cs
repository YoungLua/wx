using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.IO;
using System.Net;
using System.Drawing;
using System.Web.Script.Serialization;
using BLL;
using Model;

/// <summary>
///HISWX 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class HISWX : System.Web.Services.WebService {


    BLL.CBsPatient _objBsPatient = new BLL.CBsPatient();
    BLL.CBsLocation _objBsLocation = new BLL.CBsLocation();
    BLL.CBsDoctor _objBsDoctor = new BLL.CBsDoctor();
    BLL.CBsDocRegType _objBsDocRegType = new BLL.CBsDocRegType();
    BLL.CBsRegType _objBsRegType = new BLL.CBsRegType();  //挂号类别
    BLL.CBsRegTimeSpan _objBsRegTimeSpan = new BLL.CBsRegTimeSpan();//排班类型
    BLL.COuHosInfo _objOuHosInfo = new BLL.COuHosInfo();
    BLL.CPatCardFee _objPatCardFee = new BLL.CPatCardFee(); //储值   
    BLL.CBsDocLevel _objBsDocLevel = new BLL.CBsDocLevel(); //医生职称
    BLL.CBsRegPatAmount _objBsRegPatAmount = new BLL.CBsRegPatAmount(); //挂号费用(诊金)
    BLL.CBsUser _objUserInfo = new BLL.CBsUser();
    BLL.CBKStoreLog _objBKStoreLog = new BLL.CBKStoreLog();
    BLL.CBsRegSpanSub _objBsRegSpanSub = new BLL.CBsRegSpanSub();// 
    BLL.COuDiagCall _objOuDiagCall = new COuDiagCall();
    public HISWX () {

        Model.Configuration.UserProfiles.LocationID = 1396;
        BLL.Common.DateTimeHandler.IsServerGetTime = true;
        BLL.CGblSetting objGblSetting = new BLL.CGblSetting();
        Model.Configuration.Global.LstGblSetting = objGblSetting.GetAll();

      //  BLL.MemoryDate.StartLoadData();
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {

        
       // new Utilities.Document().SaveLog(string.Format("\r\n\r\n   {0}HelloWorld：{1}", BLL.Common.DateTimeHandler.GetServerDateTime().ToString()), string.Format("C:\\{0}.log" ,BLL.Common.DateTimeHandler.GetServerDateTime().ToString("yyyy-MM-dd"))); 
        return "Hello!版本日期" + BLL.Common.DateTimeHandler.GetServerDateTime().ToString();
    }



    /// <summary>
    /// 网络通讯测试
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string NetTest(string Request)
    {
        String columns = "req,UserId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }
        string userId = GetXmlValue(Request, "req", "UserId", "");
        try
        {
            if (GetUserID(userId) == 0)
                return "<Response><ResultCode>1</ResultCode><ErrorMsg>该机器尚未维护，请联系相关人员！</ErrorMsg></Response>";
            else
                return string.Format("<Response><ResultCode>0</ResultCode><HospitalId>{0}</HospitalId><ErrorMsg>通讯正常</ErrorMsg></Response>", new BLL.CBsUser().GetByID(GetUserID(userId)).HospitalId);
        }
        catch (Exception ex)
        {
            return string.Format("<Response><ResultCode>1</ResultCode><ErrorMsg>与HIS数据通讯失败！</ErrorMsg></Response>", ex.Message);
        }
    }

    /// <summary>
    /// 建卡病人身份证校验
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string IDCardCheck(string Request)
    {
        String columns = "req,IDCardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string idCardNo = GetXmlValue(Request, "req", "IDCardNo", ""); //得到传入的身份证号
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByIdCardNo(idCardNo);
        if (lstBsPatient.Count > 0)
        {
            Model.BsPatientInfo infoBsPatient = lstBsPatient[0];
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg >已办卡</ErrorMsg ><PatientId>{0}</PatientId>", infoBsPatient.ID);
        }
        else
        {
            Response = "<ResultCode>0</ResultCode><Status>可办卡</Status>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 查询门诊药房发药队列
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string OuDrugList(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "OuDrugList");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        String columns = "req,IDCardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string idCardNo = GetXmlValue(Request, "req", "IDCardNo", ""); //得到传入的身份证号
        Model.ModelList<Model.OuDiagCallInfo> _lstOuDiagCall = _objOuDiagCall.OuDiagCall_SelectByOperTime(BLL.Common.DateTimeHandler.GetServerDateTime().Date);
        if (_lstOuDiagCall.Count > 0)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg >已办卡</ErrorMsg >");
            for (int i = 0; i < _lstOuDiagCall.Count;i++ )
            {
                //Model.OuDiagCallInfo infoOuDiagCall = _lstOuDiagCall[i];
                Response += string.Format("<PatName>{0}</PatName>", _lstOuDiagCall[i].PatName);
            }
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 获取服务器时间获取服务器时间
    /// </summary>
    /// <returns>DateTime</returns>
    [WebMethod]
    public DateTime GetServiceTime()
    {
        return BLL.Common.DateTimeHandler.GetServerDateTime();
    }

    /// <summary>
    /// 卡校验
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string CardNoCheck(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "CardNoCheck");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        String columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }


        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "CardNo", ""); //得到传入的卡号
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByCardNo(cardNo);
        if (lstBsPatient.Count > 0)
        {
            Model.BsPatientInfo infoBsPatient = lstBsPatient[0];
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg >卡号已存在</ErrorMsg ><PatientId>{0}</PatientId>", infoBsPatient.ID);
        }
        else
        {
            Response = "<ResultCode>0</ResultCode><Status>该卡号可以使用</Status>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 查询病人卡信息
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetPatInfo(string Request)
    {
        String columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "CardNo", "");
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByCardNo(cardNo); //卡号查询
            if (lstBsPatient.Count > 0)
            {
                Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient[0].ID);
                double Amount = 0;
                if (lstPatCardFee.Count > 0)
                    Amount = lstPatCardFee[0].Amount;
                Model.BsPatientInfo infoBsPatient = lstBsPatient[0];
                DateTime PatCardFeeOperTime = infoBsPatient.OperTime;
                if (lstPatCardFee.Count > 0)
                    PatCardFeeOperTime = lstPatCardFee[0].OperTime;
                Response = string.Format("<CardNo>{0}</CardNo><CardStatus>N</CardStatus>" +
                    "<AccdNo>{1}</AccdNo><AccdStatus>N</AccdStatus><AccBalance>{2}</AccBalance>" +
                    "<PatName>{3}</PatName><IDCard>{4}</IDCard><Patid>{5}</Patid><BuildTime>{6}</BuildTime><ChargeType>自费</ChargeType><Nation>{7}</Nation><Sex>{8}</Sex><Birthday>{9}</Birthday><Tel>{10}</Tel><ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg>",
                    infoBsPatient.CardNo, "", Amount, infoBsPatient.Name, infoBsPatient.IdCardNo, infoBsPatient.ID, PatCardFeeOperTime, new BLL.CBsNation().GetByID(infoBsPatient.NationId).Name, infoBsPatient.Sex == "M" ? "男" : "女", infoBsPatient.BirthDate,   infoBsPatient.Mobile);
            }
            else
            {
                Response = "<ResultCode>1</ResultCode><ErrorMsg>卡信息查询失败，请联系系统管理员</ErrorMsg>";
            }
        }
        catch (Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>卡信息查询失败，请联系系统管理员：{0}</ErrorMsg>", ex.Message);
            //throw ex;
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 查询病人IDCard
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetPatInfoByIDCard(string Request)
    {
        String columns = "req,IDCardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "IDCardNo", "");
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByIdCardNo(cardNo); //卡号查询
            if (lstBsPatient.Count > 0)
            {
                for (int i = 0; i < lstBsPatient.Count; i++)
                {
                    Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient[i].ID);
                    double Amount = 0;
                    if (lstPatCardFee.Count > 0)
                        Amount = lstPatCardFee[0].Amount;
                    Model.BsPatientInfo infoBsPatient = lstBsPatient[i];
                    DateTime PatCardFeeOperTime = infoBsPatient.OperTime;
                    if (lstPatCardFee.Count > 0)
                        PatCardFeeOperTime = lstPatCardFee[0].OperTime;

                    Response += string.Format("<PatInfo><CardNo>{0}</CardNo><CardStatus>N</CardStatus>" +
                       "<AccdNo>{1}</AccdNo><AccdStatus>N</AccdStatus><AccBalance>{2}</AccBalance>" +
                       "<PatName>{3}</PatName><IDCard>{4}</IDCard><Patid>{5}</Patid><BuildTime>{6}</BuildTime><ChargeType>自费</ChargeType><Nation>{7}</Nation><Sex>{8}</Sex><Birthday>{9}</Birthday><Tel>{10}</Tel><ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg></PatInfo>",
                       infoBsPatient.CardNo, "", Amount, infoBsPatient.Name, infoBsPatient.IdCardNo, infoBsPatient.ID, PatCardFeeOperTime, new BLL.CBsNation().GetByID(infoBsPatient.NationId).Name, infoBsPatient.Sex == "M" ? "男" : "女", infoBsPatient.BirthDate, infoBsPatient.Mobile);

                }
            }
            else
            {
                Response = "<ResultCode>1</ResultCode><ErrorMsg>卡信息查询失败，请联系系统管理员</ErrorMsg>";
            }
        }
        catch (Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>卡信息查询失败，请联系系统管理员：{0}</ErrorMsg>", ex.Message);
            //throw ex;
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 激活健康卡
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string CreateJKCard(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "CreateJKCard");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        //string oldCardNo = GetXmlValue(Request, "req", "OldCardNo", ""); //旧卡号
        string CardNo = GetXmlValue(Request, "req", "CardNo", ""); //新卡号
        string IdCardNo = GetXmlValue(Request, "req", "IDCardNo", ""); //身份证号
        int operid = GetUserID(GetXmlValue(Request, "req", "UserId", "")); //发卡机 userID
        string Response = string.Empty;
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByIdCardNo(IdCardNo); //卡号查询
            lstBsPatient.Sort("ID");
            lstBsPatient.Reverse();
            if (lstBsPatient.Count > 0)
            {
                DAL.SqlUtil db =new DAL.SqlUtil();
                db.Transaction = null;

                db.AddParameter("@CardNo", CardNo);
                db.AddParameter("@PatId", lstBsPatient[0].ID);
                db.ExecuteNonQuery("[dbo].[uspUpdatePatCardNo]");
                BLL.Tracer<Model.absModel>.AddCustTrace("Update", string.Format("[{0}]操作员[{3}]在自助机上将卡号[{1}]修改成[{2}]", operid, lstBsPatient[0].CardNo, CardNo, BLL.Common.DateTimeHandler.GetServerDateTime()), "病人卡号修改");
                Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>激活健康卡成功</ErrorMsg>");
            }
        }
        catch (global::System.Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>激活健康卡失败,错误信息{0}</ErrorMsg>", ex.Message);

        }
        return Response = string.Format("<Response>{0}</Response>", Response);

    }
    /// <summary>
    /// 为患者进行建卡/换卡
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string CreateCardPatInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "CreateCardPatInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string Response = string.Empty;
        Model.BsPatientInfo infoBsPatient = new Model.BsPatientInfo();
        DAL.SqlUtil db = new DAL.SqlUtil();
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {
            infoBsPatient.CardNo = GetXmlValue(Request, "req", "CardNo", ""); //卡号
            if (infoBsPatient.CardNo == string.Empty)
            {
                infoBsPatient.CardNo = BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.CarNo).ToString();
            }

            infoBsPatient.Name = GetXmlValue(Request, "req", "PatientName", ""); //姓名
            infoBsPatient.Sex = GetXmlValue(Request, "req", "Sex", "") == "男" ? "M" : GetXmlValue(Request, "req", "Sex", "") == "女" ? "F" : "O";
            infoBsPatient.BirthDate = Convert.ToDateTime(GetXmlValue(Request, "req", "Birthday", "")); //出生日期
            infoBsPatient.CertificateId = 1;
            infoBsPatient.IdCardNo = GetXmlValue(Request, "req", "IDCardNo", ""); //身份证号
            infoBsPatient.AccountNo = GetXmlValue(Request, "req", "Amt", ""); //办卡压金
            infoBsPatient.AddressHome = GetXmlValue(Request, "req", "Address", ""); //家庭住址
            infoBsPatient.Mobile = infoBsPatient.Phone = GetXmlValue(Request, "req", "Tel", "");//电话
            //infoBsPatient.Mobile = GetXmlValue(Request, "req", "Mobile", ""); //手机号码
            infoBsPatient.PatTypeId = 116;//自费病人
            Model.BsNationInfo infoBsNation = GetNation(GetXmlValue(Request, "req", "Nation", ""));//民族信息
            if (infoBsNation != null) infoBsPatient.NationId = infoBsNation.ID;//民族id
            infoBsPatient.ProvinceId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("DefaltProvinceId")); //默认省
            infoBsPatient.RegionId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("DefaltRegionId"));  //默认城市
            //infoBsPatient.AreaId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("DefaltAreaId")); //默认区/县
            infoBsPatient.Residence = GetXmlValue(Request, "req", "Address", ""); //联系住址
            infoBsPatient.OperID = GetUserID(GetXmlValue(Request, "req", "UserId", "")); //发卡机 userID
            infoBsPatient.IsActive = true;
            infoBsPatient.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();// Convert.ToDateTime(GetXmlValue(Request, "req", "ActDateTime", "")); //录入时间  精确到 时分秒
            //infoBsPatient.Native = GetXmlValue(Request, "req", "Address", "").Substring(0,6);  //籍贯
            //infoBsPatient.Password = Utilities.Security.MD5_Encrypt(GetXmlValue(Request, "req", "Pwd", ""));
            //BLL.CBsPatient objBsPatient = new BLL.CBsPatient();

            int PatId = _objBsPatient.Create(infoBsPatient, trn);
            int bkId = 0;
            int patCardId = 0;
            if (Convert.ToDouble(GetXmlValue(Request, "req", "Amt", "")) > 0)
            {
               // bkId = BLL.InsertAccount.UpPatCardFee(PatId, 1, "建卡充值", infoBsPatient.OperID, "1234", Convert.ToDouble(GetXmlValue(Request, "req", "Amt", "")), trn);
            }
            else
            {
                Model.PatCardFeeInfo infoPatCardFee = new Model.PatCardFeeInfo();
                infoPatCardFee.PatId = PatId;
                infoPatCardFee.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();
                infoPatCardFee.OperId = GetUserID(GetXmlValue(Request, "req", "UserId", "")); //发卡机 userID
                infoPatCardFee.PayWayId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("PatCardFeePayWayId"));  //支付方式 （现金）infoPatCardFee.PayWayId = 9;
                infoPatCardFee.Amount = Convert.ToDouble(GetXmlValue(Request, "req", "Amt", "")); //办卡压金
                // infoPatCardFee.Memo = "预交金";
                // infoPatCardFee.F4 = Utilities.Security.MD5_Encrypt(GetXmlValue(Request, "req", "Pwd", ""));
                patCardId = _objPatCardFee.Create(infoPatCardFee, trn);
            }
            trn.Commit();


            //trn = db.GetSqlTransaction();
            //if (infoPatCardFee.Amount > 0)
            //    BLL.InsertAccount.UpPatCardFee(PatId, 0, "建卡扣押金", infoPatCardFee.OperId, GetXmlValue(Request, "req", "FlowNo", ""), -1, trn);

            //trn.Commit();//提交事物
            Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>建卡成功</ErrorMsg><PatId>{0}</PatId><BkID>{1}</BkID><CreateTime>{2}</CreateTime><CardNo>{3}</CardNo>", PatId, bkId, infoBsPatient.OperTime, infoBsPatient.CardNo);

        }
        catch (global::System.Exception ex)
        {
            trn.Rollback();
            trn.Dispose();
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>建卡失败,错误信息{0}</ErrorMsg>", ex);
            //throw ex;
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 卡密码修改
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string ModifyPwd(string Request)
    {
        String columns = "req,CardNo,NewPassword";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "CardNo", ""); //得到传入的卡号
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByCardNo(cardNo);
        // Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient[0].ID);
     //   string oldPwd = GetXmlValue(Request, "req", "OldPassword", "");
        string newPwd = GetXmlValue(Request, "req", "NewPassword", "");
        string strPwd = Utilities.Security.MD5_Encrypt(newPwd);
      //  lstBsPatient[0].Password = strPwd;
        if (lstBsPatient.Count > 0)
        {
            // _objPatCardFee.Modify(lstPatCardFee[0], null);
            DAL.SqlUtil db = new DAL.SqlUtil();
            db.Transaction = null;

            db.AddParameter("@PatId", lstBsPatient[0].ID);
           // db.AddParameter("@Password", lstBsPatient[0].Password);
            int retValue = db.ExecuteNonQuery("[dbo].[uspUpdatePatPassWord]");

            Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg><NewPassword>{0}</NewPassword>", strPwd);
        }
        else
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg></ErrorMsg>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 卡账户密码校验
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string CheckAccPwd(string Request)
    {

        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "CheckAccPwd");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        String columns = "req,CardNo,Password";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "CardNo", ""); //得到传入的卡号
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByCardNo(cardNo);
        string strPwd = "";// lstBsPatient[0].Password;
        if (strPwd == Utilities.Security.MD5_Encrypt(GetXmlValue(Request, "req", "Password", "")))
        {
            Response = string.Format("<ResultCode >0</ResultCode>");
        }
        else
        {
            Response = "<ResultCode >1</ResultCode>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }
     
    /// <summary>
    /// 查询科室信息
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string getDeptInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getDeptInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string where = "(IsActive = 1 and LsInOut in (1,2,6) or exists (select 1 from bsdoctor where bsdoctor.locationid = bslocation.id and bslocation.IsActive = 1 and LsStatus = 1 and F2 = '1'))";

        string deptId = GetXmlValue(Request, "req", "deptId", "");
        string HospitalId = GetXmlValue(Request, "req", "HospitalId", "");
        int bookable = 0;
        int IsDoctorLocation = 0;

        BLL.CBsDoctor objBsDoctor = new BLL.CBsDoctor();
        BLL.CBsSpeciality objBsSpeciality = new BLL.CBsSpeciality();
        Model.BsSpecialityInfo infoBsSpeciality = new Model.BsSpecialityInfo();
        Model.ModelList<Model.BsDoctorInfo> lstBsDoctor = new Model.ModelList<Model.BsDoctorInfo>();
        Model.ModelList<Model.BsLocationInfo> lstBsLocation = new Model.ModelList<Model.BsLocationInfo>();

        if (HospitalId.Trim() != string.Empty && Utilities.Information.IsNumeric(HospitalId))
            where += string.Format(" and HospitalId = {0}", Convert.ToInt32(HospitalId));
        if (deptId.Trim() != string.Empty && Utilities.Information.IsNumeric(deptId))
            where += string.Format(" and ID = {0}", Convert.ToInt32(deptId));
        lstBsLocation = _objBsLocation.GetDynamic(where, "orderby");

        foreach (Model.BsLocationInfo infoBsLocation in lstBsLocation)
        {
            if (infoBsLocation.SpecialityId > 0)
            {
                infoBsSpeciality = objBsSpeciality.GetByID(infoBsLocation.SpecialityId);
            }
            lstBsDoctor = objBsDoctor.BsDoctor_SelectByLocationId(infoBsLocation.ID);
            if (lstBsDoctor.Count > 0)
                IsDoctorLocation = 1;
            if(infoBsLocation.F1 == "1" && infoBsLocation.LsInOut == 2)
                bookable=1;
            Response += string.Format("<deptInfo><deptId>{0}</deptId><deptName>{1}</deptName><parentId>{4}</parentId><desc>{2}</desc><section>{3}</section><bookable>{5}</bookable><featured/><DoctorLocation>{6}</DoctorLocation></deptInfo>",
                infoBsLocation.ID, infoBsLocation.Name, "", infoBsLocation.Name, infoBsLocation.SpecialityId > 0 ? infoBsLocation.SpecialityId : -1, bookable, IsDoctorLocation);
            bookable = 0;
        }

        return Response = string.Format("<res service=\"getDeptInfo\">{0}</res>", Response);
    }

    /// <summary>
    /// 门诊缴费清单查询
    /// </summary>
    [WebMethod]
    public string getPrescription(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getPrescription");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "cardNo", "");
        string clinicNo = GetXmlValue(Request, "req", "clinicNo", "");
        string status = GetXmlValue(Request, "req", "status", "");
        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(clinicNo);
        if (lstOuHosInfo.Count > 0)
        {
            if (status == "2")
            {
                BLL.Finder<Model.uspOuInvoiceDtlQry> objbllFinder = new BLL.Finder<Model.uspOuInvoiceDtlQry>();
                Model.ModelList<Model.OuInvoiceInfo> lstOuInvoice = new Model.ModelList<Model.OuInvoiceInfo>();
                BLL.COuInvoice _objOuInvoice = new BLL.COuInvoice();
                Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = new Model.ModelList<Model.uspOuInvoiceDtlQry>();
                string strIsHave = string.Format(" MzRegId ={0}  and IsCancel = 0 ", lstOuHosInfo[0].ID);
                lstOuInvoice = _objOuInvoice.GetDynamic(strIsHave, null);
                foreach (Model.OuInvoiceInfo infoOuInvoice in lstOuInvoice)
                {
                    objbllFinder.AddParameter("InvoId", infoOuInvoice.ID);
                    lstUspOuChargeDetail.AddRange(objbllFinder.Find("uspOuInvoiceDtl"));
                }
                if (lstUspOuChargeDetail.Count > 0)
                {
                    Response += string.Format("<ResultCode>0</ResultCode><ResultDesc>获取缴费单明细成功</ResultDesc><sumFee>{0}</sumFee><prescItemList>", lstUspOuChargeDetail.GetSum("Amount"));
                    foreach (Model.uspOuInvoiceDtlQry info in lstUspOuChargeDetail)
                    {
                        Response += string.Format("<itemName>{0}</itemName><account>{1}</account><itemPrice>{2}</itemPrice><unit>{3}</unit><money>{4}</money><type>{5}</type>",
                            info.Name, info.Totality, info.Price, info.UnitDiagName, info.Amount, info.InvMzItemName);
                    }
                    Response += "</prescItemList>";
                }
                else
                {
                    Response += "<ResultCode>1</ResultCode><ResultDesc>获取缴费单明细失败，没有查询到相应记录！</ResultDesc>";
                }
            }
            else
            {
                Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = BLL.SockIOPoolComm.GetPoolDataList<Model.uspOuInvoiceDtlQry>(string.Format("FrmOuCharge_RecipeDtl_{0}", lstOuHosInfo[0].ID));
                if (lstUspOuChargeDetail.Count > 0)
                {
                    Response += string.Format("<ResulgetRegInfotCode>0</ResultCode><ResultDesc>获取缴费单明细成功</ResultDesc><sumFee>{0}</sumFee><prescItemList>", lstUspOuChargeDetail.GetSum("Amount"));
                    foreach (Model.uspOuInvoiceDtlQry info in lstUspOuChargeDetail)
                    {
                        Response += string.Format("<itemName>{0}</itemName><account>{1}</account><itemPrice>{2}</itemPrice><unit>{3}</unit><money>{4}</money><type>{5}</type>",
                            info.Name, info.Totality, info.Price, info.UnitDiagName, info.Amount, info.InvMzItemName);
                    }
                    Response += "</prescItemList>";
                }
                else
                {
                    Response += "<ResultCode>1</ResultCode><ResultDesc>获取缴费单明细失败，没有查询到相应记录！</ResultDesc>";
                }
            }
        }
        return Response = string.Format("<res service=\"getPrescription\">{0}</res>", Response);
    }

    /// <summary>
    /// 缴费确认接口
    /// </summary>
    [WebMethod]
    public string payOrder(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "payOrder");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string Response = string.Empty;
        string returnstring=string.Empty;
        string orderId = GetXmlValue(Request, "req", "orderId", "");
        string orderIdPAY = GetXmlValue(Request, "req", "orderIdPAY", "");
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "");
        string payAmout = GetXmlValue(Request, "req", "payAmout", "");
        string payMode = GetXmlValue(Request, "req", "payMode", "");
        string payTime = GetXmlValue(Request, "req", "payTime", "");
        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
        if (lstOuHosInfo.Count > 0)
        {
            try
            {
                //XYHIS.FrmOuChargeWebServer frmouchar = new XYHIS.FrmOuChargeWebServer();
                //frmouchar.IsAutoCharge = true;
                //returnstring = frmouchar.AutoRegOuHosInfo(lstOuHosInfo[0].ID, 22809);
                //BLL.Common.Utils.UpdateColumn("OuInvoice", string.Format(" F5 ='{0}' ", "微信缴费"), frmouchar._infoOuInvoice.ID, null);
                //BLL.Common.Utils.UpdateColumn("OuInvoice", string.Format(" F6 ='{0}' ", orderIdPAY), frmouchar._infoOuInvoice.ID, null);

                //BLL.COuInvoicePay objOuInvoicePay = new BLL.COuInvoicePay();
                //Model.ModelList<Model.OuInvoicePayInfo> lstOuInvoicePay = objOuInvoicePay.OuInvoicePay_SelectByInvoId(frmouchar._infoOuInvoice.ID);
                //if (payMode == "1")
                //{
                //    lstOuInvoicePay.Fill("PaywayId", 331);
                //    objOuInvoicePay.Save(lstOuInvoicePay, null);
                //}
                //else if (payMode == "2")
                //{
                //    lstOuInvoicePay.Fill("PaywayId", 332);
                //    objOuInvoicePay.Save(lstOuInvoicePay, null);
                //}
                //XYHIS.FrmOuChargeWebServer frm = new XYHIS.FrmOuChargeWebServer();
                //frm.GetAutoRegOuHosInfo(lstOuHosInfo[0].ID);
            }
            catch (Exception ex)
            {
                Response += string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc>",ex.ToString());
            }
            if (returnstring.Contains("成功"))
            {
                Response += "<resultCode>0</resultCode><resultDesc>缴费成功！</resultDesc>";
            }
        }
        else
        {
            Response += "<resultCode>1</resultCode><resultDesc>没有对应的病人！</resultDesc>";
        }
        return Response = string.Format("<res service=\"payOrder\">{0}</res>", Response);
    }

    /// <summary>
    /// 查询该病人是否可以挂儿科
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string IfEKGH(string Request)
    {
        string Response = string.Empty;

        string Patid = GetXmlValue(Request, "req", "Patid", "");
        string HospitalId = GetXmlValue(Request, "req", "HospitalId", "");
        string LocationName = GetXmlValue(Request, "req", "LocationName", "");
        Model.BsPatientInfo lstBsPatient = _objBsPatient.GetByID(Convert.ToInt32(Patid)); //卡号查询
        BLL.Common.CalBirthday _calBirthday = new BLL.Common.CalBirthday();
        _calBirthday.Birthday = lstBsPatient.BirthDate;
        if (!LocationName.Contains("儿") || (_calBirthday.IsYounger && LocationName.Contains("儿")))
            return "<Response><ResultCode>0</ResultCode><ErrorMsg>可以挂号！</ErrorMsg></Response>";
        else
            return "<Response><ResultCode>1</ResultCode><ErrorMsg>年龄大于15，不能挂儿科！</ErrorMsg></Response>";
    }

    /// <summary>
    /// 查询该病人是否在预约挂号违约期内
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string IfYYWY(string Request)
    {
        string Response = string.Empty;

        string Patid = GetXmlValue(Request, "req", "Patid", "");
        string HospitalId = GetXmlValue(Request, "req", "HospitalId", "");


        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" Patid={0} and IsCancel=0  and F8<>'1' and IsPreReg=1   and  opertime >(getdate())-30 and getdate()>ouhosinfo.regtime  ", Patid), "RegTime DESC");


        if (ouhosLst.Count < 3) return "<Response><ResultCode>0</ResultCode><ErrorMsg>可以预约挂号！</ErrorMsg></Response>";
        else return "<Response><ResultCode>1</ResultCode><ErrorMsg>:在违约期内！</ErrorMsg></Response>";
    }

    /// <summary>
    /// 查询该病人是否已预约了同一天，同一个科室的号
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string IfYYDept(string Request)
    {
        string Response = string.Empty;

        string Patid = GetXmlValue(Request, "req", "Patid", "");
        string HospitalId = GetXmlValue(Request, "req", "HospitalId", "");
        string DeptId = GetXmlValue(Request, "req", "DeptId", "");
        DateTime Day = Convert.ToDateTime(GetXmlValue(Request, "req", "Day", ""));


        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" Patid={0} and IsCancel=0 and IsPreReg=1  and  RegTime >='{1}' and RegDept={2}", Patid, Day.ToString("yyyy-MM-dd"), DeptId), "RegTime DESC");


        if (ouhosLst.Count > 0)
            return "<Response><ResultCode>1</ResultCode><ErrorMsg>不能预约！</ErrorMsg></Response>";
        else 
            return "<Response><ResultCode>0</ResultCode><ErrorMsg>可以预约！</ErrorMsg></Response>";
    }


    /// <summary>
    /// 查询医生信息
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string getDoctorInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getDoctorInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        int hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "hospitalId", ""));

        int deptId = GetXmlValue(Request, "req", "deptId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "deptId", ""));

        int doctorId = GetXmlValue(Request, "req", "doctorId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "doctorId", ""));

        int isReadImage = GetXmlValue(Request, "req", "isReadImage", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "isReadImage", ""));

        string Response = string.Empty;
        string where = "LsStatus = 1 and F2 = '1'";
        Model.ModelList<Model.BsDoctorInfo> lstBsDoctor = new Model.ModelList<Model.BsDoctorInfo>();
        Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = new Model.ModelList<Model.BsDocRegTypeInfo>();
        Model.BsRegTimeSpanInfo infoBsRegTimeSpan=new Model.BsRegTimeSpanInfo();
        BLL.CBsRegTimeSpan objBsRegTimeSpan=new BLL.CBsRegTimeSpan();
        Model.BsDocLevelInfo infoBsDocLevel=new Model.BsDocLevelInfo();
        Model.BsUserInfo infoBsUser = new Model.BsUserInfo();
        BLL.CBsDocLevel objBsDocLevel=new BLL.CBsDocLevel();
        BLL.CBsDoctor objBsDoctor = new BLL.CBsDoctor();
        BLL.CBsDocRegType objBsDocRegType = new BLL.CBsDocRegType();
        BLL.BsUserImage objBsUserImage = new BLL.BsUserImage();
        if (deptId > 0)
            where += string.Format(" and LocationId = {0}", deptId);
        if (doctorId > 0)
            where += string.Format(" and ID = {0}", doctorId);

        lstBsDoctor = objBsDoctor.GetDynamic(where, "OrderBy");

        int BsDocLevel=0;
        string Sex = string.Empty;
        foreach (Model.BsDoctorInfo info in lstBsDoctor)
        {
            if (info.DocLevId > 0)
            {
                infoBsDocLevel = objBsDocLevel.GetByID(info.DocLevId);
                if (infoBsDocLevel.Name == "主治医师")
                    BsDocLevel = 1;
                else if (infoBsDocLevel.Name == "主任医师")
                    BsDocLevel = 2;
                else if (infoBsDocLevel.Name == "副主任医师")
                    BsDocLevel = 3;
                else if (infoBsDocLevel.Name == "医师")
                    BsDocLevel = 10;
                else
                    BsDocLevel = 9;
            }
            if (info.Sex.Trim() != "1" && info.Sex.Trim() != "2")
                Sex = "0";
            else
                Sex = info.Sex.Trim();
            System.Drawing.Image image = objBsUserImage.SelectImageByUserId(info.UserId);
            string ImageString;
            if (image != null && isReadImage == 1)
            {
                byte[] byteImage = GetByteImage(image);
                //System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();
                //String StringMessage = ASCII.GetString(byteImage);
                //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                //ImageString = jsonSerializer.Serialize(byteImage);
                ImageString = Convert.ToBase64String(byteImage);
            }
            else
            {
                ImageString = string.Empty;
            }
            Response += string.Format("<doctorInfo><doctorId>{0}</doctorId><doctorName>{1}</doctorName><deptId>{2}</deptId><Title>{3}</Title><Gender>{4}</Gender><Desc>{5}</Desc><Image>{6}</Image></doctorInfo>",
                info.ID, info.Name, info.LocationId, infoBsDocLevel.Name, Sex, info.Introduce, ImageString);/*BsDocLevel*/
        }

        return Response = string.Format("<res service=\"getDoctorInfo\">{0}</res>", Response);
    }

    //image格式转byte[]格式
    public byte[] GetByteImage(Image img)
    {
        byte[] bt = null;
        if (!img.Equals(null))
        {
            using (MemoryStream mostream = new MemoryStream())
            {
                Bitmap bmp = new Bitmap(img);
                bmp.Save(mostream, System.Drawing.Imaging.ImageFormat.Bmp);//将图像以指定的格式存入缓存内存流
                bt = new byte[mostream.Length];
                mostream.Position = 0;//设置流的初始位置
                mostream.Read(bt, 0, Convert.ToInt32(bt.Length));
            }
        }
        return bt;
    }

    /// <summary>
    /// 医生排班信息
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string getSchInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getSchInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        int hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "hospitalId", ""));

        DateTime startDate = GetXmlValue(Request, "req", "startDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "startDate", ""));

        DateTime endDate = GetXmlValue(Request, "req", "endDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "endDate", ""));

        int deptId = GetXmlValue(Request, "req", "deptId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "deptId", ""));

        int doctorId = GetXmlValue(Request, "req", "doctorId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "doctorId", ""));

        BLL.Finder<Model.uspgetSchInfo> objuspgetSch = new BLL.Finder<Model.uspgetSchInfo>();
        Model.ModelList<Model.uspgetSchInfo> lstuspgetSch = new Model.ModelList<Model.uspgetSchInfo>();
        objuspgetSch.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
        objuspgetSch.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));
        objuspgetSch.AddParameter("deptId", deptId);
        objuspgetSch.AddParameter("doctorId", doctorId);
        lstuspgetSch = objuspgetSch.Find("UspgetSchInfo");
        foreach (Model.uspgetSchInfo info in lstuspgetSch)
        {
            Response += string.Format("<regInfo><doctorId>{0}</doctorId><shiftId>{1}</shiftId><shiftType>{2}</shiftType><schDate>{3}</schDate><schRegTypeId>{4}</schRegTypeId><schFee>{5}</schFee><schRegfee>{6}</schRegfee><schTreatfee>{7}</schTreatfee><schBegintime>{8}</schBegintime><schEndtime>{9}</schEndtime><schRegMax>{10}</schRegMax><schRegCount>{11}</schRegCount><schLimited>{12}</schLimited><schSegmented>{13}</schSegmented><LocationId>{14}</LocationId><available>{15}</available><DiagRoom>{16}</DiagRoom><SpecialtyRoom>{17}</SpecialtyRoom></regInfo>",
                info.doctorId, info.shiftId, info.shiftType, info.schDate.ToString("yyyy-MM-dd"), info.schRegTypeId, info.schFee, info.schRegfee, info.schTreatfee, info.schBegintime, info.schEndtime, info.schRegMax, info.schRegCount, info.schLimited, info.schSegmented, info.LocationId, info.Iscancel, info.DiagRoom, info.SpecialtyRoom);
        }

        return Response = string.Format("<res service=\"getSchInfo\">{0}</res>", Response);
    }

    /// <summary>
    /// 号源信息查询
    /// </summary>
    [WebMethod]
    public string getAllRegInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getAllRegInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        int hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "hospitalId", ""));

        DateTime startDate = GetXmlValue(Request, "req", "startDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "startDate", ""));

        DateTime endDate = GetXmlValue(Request, "req", "endDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "endDate", ""));

        string doctorId = string.Empty;
        string doctorName = string.Empty;
        DateTime regDate = DateTime.MinValue;
        string regWeekDay = string.Empty;
        string startTime = string.Empty;
        string endTime = string.Empty;
        BLL.Finder<Model.uspgetAllRegInfo> objuspgetAllReg = new BLL.Finder<Model.uspgetAllRegInfo>();
        Model.ModelList<Model.uspgetAllRegInfo> lstuspgetAllReg = new Model.ModelList<Model.uspgetAllRegInfo>();
        objuspgetAllReg.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
        objuspgetAllReg.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));
        lstuspgetAllReg = objuspgetAllReg.Find("UspgetAllRegInfo");
        for (int i = 0; i < lstuspgetAllReg.Count; i++)
        {
            if (doctorId != lstuspgetAllReg[i].doctorId && doctorName != lstuspgetAllReg[i].doctorName)
            {
                if (regDate != DateTime.MinValue && regWeekDay != string.Empty)
                    Response += "</TimeRegInfoList>";
                if (doctorId != string.Empty && doctorName != string.Empty)
                    Response += "</regInfo>";
                Response += string.Format("<regInfo><doctorId>{0}</doctorId>",lstuspgetAllReg[i].doctorId);
            }
            if ((regDate != lstuspgetAllReg[i].regDate && regWeekDay != lstuspgetAllReg[i].regWeekDay) || doctorId != lstuspgetAllReg[i].doctorId)
            {
                Response += string.Format("<regDate>{0}</regDate><regStatus>{1}</regStatus><TimeRegInfoList>",
                    lstuspgetAllReg[i].regDate.ToString("yyyy-MM-dd"), lstuspgetAllReg[i].regStatus);
            }
            if ((startTime != lstuspgetAllReg[i].startTime && endTime != lstuspgetAllReg[i].endTime) || regDate != lstuspgetAllReg[i].regDate || doctorId != lstuspgetAllReg[i].doctorId)
            {
                Response += string.Format("<timeRegInfo><startTime>{0}</startTime><endTime>{1}</endTime><regTotalCount>{2}</regTotalCount><regleaveCount>{3}</regleaveCount><seqCode>{4}</seqCode><shiftType>{5}</shiftType></timeRegInfo>",
                    lstuspgetAllReg[i].startTime, lstuspgetAllReg[i].endTime, lstuspgetAllReg[i].regTotalCount1, lstuspgetAllReg[i].regleaveCount1, lstuspgetAllReg[i].onlyId, lstuspgetAllReg[i].shiftType);
            }
            doctorId = lstuspgetAllReg[i].doctorId;
            doctorName = lstuspgetAllReg[i].doctorName;
            regDate = lstuspgetAllReg[i].regDate;
            regWeekDay = lstuspgetAllReg[i].regWeekDay;
            startTime = lstuspgetAllReg[i].startTime;
            endTime = lstuspgetAllReg[i].endTime;
        }
        if (Response != string.Empty)
            Response += "</TimeRegInfoList></regInfo>";
        return string.Format("<res service=\"getAllRegInfo\">{0}</res>", Response);
    }

    //医生号源信息查询
    [WebMethod]
    public string getRegInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "getRegInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        int hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "hospitalId", ""));

        int deptId = GetXmlValue(Request, "req", "deptId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "deptId", ""));
        
        int doctorId = GetXmlValue(Request, "req", "doctorId", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "doctorId", ""));

        DateTime startDate = GetXmlValue(Request, "req", "startDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "startDate", ""));

        DateTime endDate = GetXmlValue(Request, "req", "endDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "endDate", ""));

        string doctorId1 = string.Empty;
        string doctorName = string.Empty;
        DateTime regDate = DateTime.MinValue;
        string regWeekDay = string.Empty;
        string startTime = string.Empty;
        string endTime = string.Empty;
        BLL.Finder<Model.uspgetAllRegInfo> objuspgetAllReg = new BLL.Finder<Model.uspgetAllRegInfo>();
        Model.ModelList<Model.uspgetAllRegInfo> lstuspgetAllReg = new Model.ModelList<Model.uspgetAllRegInfo>();
        objuspgetAllReg.AddParameter("deptId", deptId);
        objuspgetAllReg.AddParameter("doctorId", doctorId);
        objuspgetAllReg.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
        objuspgetAllReg.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));
        lstuspgetAllReg = objuspgetAllReg.Find("UspgetRegInfo");
        for (int i = 0; i < lstuspgetAllReg.Count; i++)
        {
            if (regDate != DateTime.MinValue && regDate != lstuspgetAllReg[i].regDate && regWeekDay != string.Empty)
                    Response += "</TimeRegInfoList>";
            if (doctorId1 != lstuspgetAllReg[i].doctorId && doctorName != lstuspgetAllReg[i].doctorName)
            {
                if (doctorId1 != string.Empty && doctorName != string.Empty)
                    Response += "</regInfo>";
                Response += string.Format("<regInfo><doctorId>{0}</doctorId>", lstuspgetAllReg[i].doctorId);
            }
            if ((regDate != lstuspgetAllReg[i].regDate && regWeekDay != lstuspgetAllReg[i].regWeekDay) || doctorId1 != lstuspgetAllReg[i].doctorId)
            {
                Response += string.Format("<regDate>{0}</regDate><regStatus>{1}</regStatus><TimeRegInfoList>",
                    lstuspgetAllReg[i].regDate.ToString("yyyy-MM-dd"), lstuspgetAllReg[i].regStatus);
            }
            if ((startTime != lstuspgetAllReg[i].startTime && endTime != lstuspgetAllReg[i].endTime) || regDate != lstuspgetAllReg[i].regDate || doctorId1 != lstuspgetAllReg[i].doctorId)
            {
                Response += string.Format("<timeRegInfo><startTime>{0}</startTime><endTime>{1}</endTime><regTotalCount>{2}</regTotalCount><regleaveCount>{3}</regleaveCount><seqCode>{4}</seqCode><shiftType>{5}</shiftType></timeRegInfo>",
                    lstuspgetAllReg[i].startTime, lstuspgetAllReg[i].endTime, lstuspgetAllReg[i].regTotalCount1, lstuspgetAllReg[i].regleaveCount1, lstuspgetAllReg[i].onlyId, lstuspgetAllReg[i].shiftType);
            }
            doctorId1 = lstuspgetAllReg[i].doctorId;
            doctorName = lstuspgetAllReg[i].doctorName;
            regDate = lstuspgetAllReg[i].regDate;
            regWeekDay = lstuspgetAllReg[i].regWeekDay;
            startTime = lstuspgetAllReg[i].startTime;
            endTime = lstuspgetAllReg[i].endTime;
        }
        if (Response != string.Empty)
            Response += "</TimeRegInfoList></regInfo>";
        else
            Response = "没有查询到数据";
        tip = string.Format("\r\n结束时间[{0}]\r\n", BLL.Common.DateTimeHandler.GetServerDateTime());
        log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        return string.Format("<res service=\"getRegInfo\">{0}</res>", Response);
    }

    //锁号
    [WebMethod]
    public string lockRegist(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "lockRegist");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        //string orderId = GetXmlValue(Request, "req", "orderId", "").Trim();
        //string hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim();
        string deptId = GetXmlValue(Request, "req", "deptId", "").Trim();
        string doctorId = GetXmlValue(Request, "req", "doctorId", "").Trim();
        DateTime regDate = GetXmlValue(Request, "req", "regDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "regDate", ""));
        int shiftType = GetXmlValue(Request, "req", "shiftType", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "shiftType", ""));
        string startTime = GetXmlValue(Request, "req", "startTime", "").Trim();
        string endTime = GetXmlValue(Request, "req", "endTime", "").Trim();
        //string regType = GetXmlValue(Request, "req", "regType", "").Trim();
        //string userIdCard = GetXmlValue(Request, "req", "userIdCard", "").Trim();
        //string userName = GetXmlValue(Request, "req", "userName", "").Trim();
        string userGender = GetXmlValue(Request, "req", "userGender", "").Trim();
        //string userMobile = GetXmlValue(Request, "req", "userMobile", "").Trim();
        string parName = GetXmlValue(Request, "req", "parName", "").Trim();
        string patCId = GetXmlValue(Request, "req", "patCId", "").Trim();
        string address = GetXmlValue(Request, "req", "address", "").Trim();
        string patMobile = GetXmlValue(Request, "req", "patMobile", "").Trim();
        DateTime userBirthday = GetXmlValue(Request, "req", "userBirthday", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "userBirthday", ""));
        string operIdCard = GetXmlValue(Request, "req", "operIdCard", "").Trim();
        string operName = GetXmlValue(Request, "req", "operName", "").Trim();
        string operMobile = GetXmlValue(Request, "req", "operMobile", "").Trim();
        string orderType = GetXmlValue(Request, "req", "orderType", "").Trim();
        DateTime orderTime = GetXmlValue(Request, "req", "orderTime", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "orderTime", ""));
        //double totalfee = Utilities.Information.IsNumeric(GetXmlValue(Request, "req", "totalfee", "").Trim()) ? Convert.ToDouble(GetXmlValue(Request, "req", "totalfee", "").Trim()) : 0;
        //double regfee = Utilities.Information.IsNumeric(GetXmlValue(Request, "req", "regfee", "").Trim()) ? Convert.ToDouble(GetXmlValue(Request, "req", "regfee", "").Trim()) : 0;
        //double treatfee = Utilities.Information.IsNumeric(GetXmlValue(Request, "req", "treatfee", "").Trim()) ? Convert.ToDouble(GetXmlValue(Request, "req", "treatfee", "").Trim()) : 0;
        string medicalcard = GetXmlValue(Request, "req", "medicalcard", "").Trim();
        double regfee = GetXmlValue(Request, "req", "regfee", "").Trim() == string.Empty ? 0 : Convert.ToDouble(GetXmlValue(Request, "req", "regfee", "").Trim());
        double treatfee = GetXmlValue(Request, "req", "treatfee", "").Trim() == string.Empty ? 0 : Convert.ToDouble(GetXmlValue(Request, "req", "treatfee", "").Trim());
        string seqCode = GetXmlValue(Request, "req", "seqCode", "").Trim();

        BLL.Common.CalBirthday objCalBirthday = new BLL.Common.CalBirthday();
        BLL.CBsPatient objBsPatient = new BLL.CBsPatient();
        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        BLL.COuDocRegType objOuDocRegType = new BLL.COuDocRegType();
        BLL.CBsDocRegType objBsDocRegType = new BLL.CBsDocRegType();
        BLL.CBsRegSpanSub objBsRegSpanSub = new BLL.CBsRegSpanSub();
        BLL.COuDocSpanSub objOUDOCSPANSUB = new BLL.COuDocSpanSub();
        BLL.CBsLocation objBsLocation = new BLL.CBsLocation();
        Model.BsLocationInfo infoBsLocation = new Model.BsLocationInfo();
        Model.BsDocRegTypeInfo infoBsDocRegType = new Model.BsDocRegTypeInfo();
        Model.OuDocRegTypeInfo infoOuDocRegType = new Model.OuDocRegTypeInfo();
        Model.ModelList<Model.BsRegSpanSubInfo> lstBsRegSpanSub = new Model.ModelList<Model.BsRegSpanSubInfo>();
        Model.BsPatientInfo infoBsPatient = new Model.BsPatientInfo();
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = new Model.ModelList<Model.BsPatientInfo>();
        Model.OuHosInfoInfo _infoOuHos = new Model.OuHosInfoInfo();
        Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = new Model.ModelList<Model.BsDocRegTypeInfo>();
        Model.OuDocSpanSubInfo infoOUDOCSPANSUB = new Model.OuDocSpanSubInfo();
        DAL.SqlUtil db = new DAL.SqlUtil();
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {
            if (seqCode.Contains("R"))
            {
                infoOUDOCSPANSUB = objOUDOCSPANSUB.GetByID(Convert.ToInt32(seqCode.Substring(1, seqCode.Length - 1)));
                infoOuDocRegType = objOuDocRegType.GetByID(Convert.ToInt32(infoOUDOCSPANSUB.F4));
            }
            else
            {
                infoOUDOCSPANSUB = objOUDOCSPANSUB.GetByID(Convert.ToInt32(seqCode.Substring(1, seqCode.Length - 1)));
                infoBsDocRegType = objBsDocRegType.GetByID(infoOUDOCSPANSUB.WeekPlanID);
            }

            lstBsPatient = objBsPatient.BsPatient_SelectByCardNo(medicalcard);
            if (lstBsPatient.Count > 0 && medicalcard.Trim() != string.Empty)
            {
                infoBsPatient = lstBsPatient[0];

            }
            else
            {
                infoBsPatient.ID = 0;
                infoBsPatient.CardNo = Convert.ToString(BLL.absBusiness<Model.absModel>.ExecuteScalar("UspGetWxCardNo", null, null));//BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.CarNo).ToString();
                infoBsPatient.Name = operName;
                infoBsPatient.BirthDate = userBirthday;
                if (userGender == "1")
                    infoBsPatient.Sex = "M";
                else if (userGender == "2")
                    infoBsPatient.Sex = "F";
                infoBsPatient.PatTypeId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("SelfPatTypeId"));
                infoBsPatient.Residence = address;
                infoBsPatient.Native = "广东省湛江市";
                infoBsPatient.OperID = 7640;
                infoBsPatient.OperTime = orderTime;
                //if (Utilities.Information.IsNumeric(regType))
                //{
                //if (Convert.ToInt32(regType) != 2)
                //{
                //    string resultDesc = string.Empty;
                //    if (userIdCard.Trim() == string.Empty)
                //        resultDesc += "患者身份证件号码不能为空  ";
                //    if (userMobile.Trim() == string.Empty)
                //        resultDesc += "患者手机号码不能为空  ";
                //    if (userBirthday == DateTime.MinValue)
                //        resultDesc += "患者出生日期不能为空  ";

                //    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard>{1}</medicalcard>",
                //             resultDesc, medicalcard);
                //    return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
                //}
                if (operIdCard.Trim() != string.Empty)
                {
                    infoBsPatient.IdCardNo = operIdCard;
                    infoBsPatient.CertificateId = 1;
                }
                infoBsPatient.Mobile = patMobile;
                //}
                infoBsPatient.ID = objBsPatient.Create(infoBsPatient, trn);
            }

            _infoOuHos.RegTime = regDate;
            _infoOuHos.IsCancel = false;
            _infoOuHos.OperId = 7640;
            _infoOuHos.OperTime = orderTime;
            _infoOuHos.RegDept = Utilities.Information.IsNumeric(deptId) ? Convert.ToInt32(deptId) : 0;
            _infoOuHos.DoctorId = Utilities.Information.IsNumeric(doctorId) ? Convert.ToInt32(doctorId): 0;
            _infoOuHos.Name = operName;
            _infoOuHos.CardNo = infoBsPatient.CardNo;
            if (userGender == "1")
                _infoOuHos.Sex = "M";
            else if (userGender == "2")
                _infoOuHos.Sex = "F";
            _infoOuHos.PatTypeId = infoBsPatient.PatTypeId;
            _infoOuHos.PatId = infoBsPatient.ID;
            _infoOuHos.RegFee = regfee;
            _infoOuHos.DiagnoFee = treatfee;
            if (infoOuDocRegType.ID > 0)
            {
                _infoOuHos.DiagRoomId = infoOuDocRegType.DiagRoomId;
                _infoOuHos.RegTypeId = infoOuDocRegType.RegTypeId;
                _infoOuHos.TimeSpanSubId = infoOUDOCSPANSUB.TimeSpanSubID;
            }
            else if (infoBsDocRegType.ID > 0)
            {
                _infoOuHos.DiagRoomId = infoBsDocRegType.DiagRoomId;
                _infoOuHos.RegTypeId = infoBsDocRegType.RegTypeId;
                _infoOuHos.TimeSpanSubId = infoOUDOCSPANSUB.TimeSpanSubID;
            }
            else
            {
                lstBsDocRegType = objBsDocRegType.GetDynamic(" IsActive = 1 and DoctorId = " + _infoOuHos.DoctorId, null);
                if (lstBsDocRegType.Count > 0)
                    _infoOuHos.RegTypeId = lstBsDocRegType[0].RegTypeId;
            }
            if (shiftType == 1)
                _infoOuHos.TimeSpanId = 1;
            else if (shiftType == 2)
                _infoOuHos.TimeSpanId = 3;
            else if (shiftType == 3)
                _infoOuHos.TimeSpanId = 4;
            _infoOuHos.IsPreReg = true;
            //_infoOuHos.IsRegist = true;
            _infoOuHos.F1 = "1";
          

            if (infoOUDOCSPANSUB.TimeSpanSubID > 0)
            {
                Model.ModelList<Model.OuHosInfoInfo> tempList = objOuHosInfo.GetDynamic(string.Format("  IsCancel=0  and F8<>'1' and IsPreReg=1 and (IsRegist = 0 or IsRegist is null) and CardNo='{0}'  and  opertime >(getdate())-30 and (getdate())>ouhosinfo.regtime  ", _infoOuHos.CardNo), null);
                if (tempList.Count >= 3)
                {
                    tempList.Sort("OperTime");
                    string lastRegTime = tempList[tempList.Count - 1].OperTime.ToLongDateString();

                    string mess = string.Format("该病人在近一月内已经违约{0}次！\r\n   最近违约信息：\r\n {1} 预约【{2}】【{3}】医生", tempList.Count, lastRegTime, tempList[tempList.Count - 1].RegTime.ToShortDateString(), new BLL.CBsDoctor().GetByID(tempList[tempList.Count - 1].DoctorId).Name);
                    trn.Rollback();
                    trn.Dispose();
                    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                        mess);
                    return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
                }

                tempList = new Model.ModelList<Model.OuHosInfoInfo>();
                tempList = objOuHosInfo.GetDynamic(string.Format(" IsCancel=0 and CardNo='{0}' and IsPreReg=1 and RegTime>='{1}' and RegTime< convert(char(10),dateadd(d,1,'{2}'),120)  and DoctorId={3}", _infoOuHos.CardNo, _infoOuHos.RegTime, _infoOuHos.RegTime, _infoOuHos.DoctorId), null);
                if (tempList.Count > 0)
                {
                    string mess = "该病人在同一天已经预约了该医生!";
                    trn.Rollback();
                    trn.Dispose();
                    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                        mess);
                    return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
                }


                int TolCount = infoOUDOCSPANSUB.SubLimitNum;
                int HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsCancel=0 and IsPreReg=1  and TimeSpanId={0}   and RegTime>=convert(char(10),'{1}',120) and RegTime<convert(char(10),dateadd(d,1,'{2}'),120) and DoctorId={3} and TimeSpanSubId ={4} ) ", _infoOuHos.TimeSpanId, _infoOuHos.RegTime, _infoOuHos.RegTime, _infoOuHos.DoctorId, _infoOuHos.TimeSpanSubId), "int"));
                if (HasCount >= TolCount)
                {
                    string mess = "预约该时间段号量已满，请预约其他时间段!";
                    trn.Rollback();
                    trn.Dispose();
                    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                        mess);
                    return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
                }
            }
            infoBsLocation = objBsLocation.GetByID(_infoOuHos.RegDept);
            if (_infoOuHos.Sex.Trim() == "M" && Convert.ToBoolean(infoBsLocation.IsForwomen))
            {
                string mess = "该科室是女性科室，男性不能挂此科室！";
                trn.Rollback();
                trn.Dispose();
                Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                    mess);
                return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
            }

            objCalBirthday.Birthday = infoBsPatient.BirthDate;
            objCalBirthday.NowDate = _infoOuHos.RegTime;
            _infoOuHos.Age = objCalBirthday.Age + (objCalBirthday.Months > 10 ? objCalBirthday.Months / 100 : objCalBirthday.Months / 10);
            _infoOuHos.AgeString = BLL.Common.Utils.CalAge(objCalBirthday);
            if (_infoOuHos.Age > 14 && infoBsLocation.Name.Contains("儿"))
            {
                string mess = "该科室是儿童科室，大于14岁的患者不能挂此科室！";
                trn.Rollback();
                trn.Dispose();
                Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                    mess);
                return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
            }
            _infoOuHos.ContactPhone = GetNotEmptyString(infoBsPatient.Mobile, infoBsPatient.Phone, infoBsPatient.PhoneHome, infoBsPatient.PhoneWork);
            if (BLL.Common.Utils.GetSystemSetting("OuRegFeeMode") == "2" || BLL.Common.Utils.GetSystemSetting("OuDiagFeeMode") == "2")
                _infoOuHos.IsInPatient = true;    //留收款处收诊金、挂号费
            do
            {
                _infoOuHos.MzRegNo = BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.MzReg).ToString();// BLL.Common.SequenceNumHandler.GetSequenceNumGH(Model.EnumSequenceNumType.MzReg, _infoOuHos.RegTime).ToString();
            }
            while (objOuHosInfo.OuHosInfo_SelectByMzRegNo(_infoOuHos.MzRegNo).Count > 0);
            _infoOuHos.F4 = "微信";
            _infoOuHos.ID = objOuHosInfo.Create(_infoOuHos, trn);
            //BLL.Common.Utils.UpdateColumn("OuHosInfo", string.Format("IsRegist={0}", 1), _infoOuHos.ID, null);
            trn.Commit();
            BLL.Common.Utils.UpdateColumn("OuHosInfo", string.Format("IsRegist={0}", 1), _infoOuHos.ID, null);
            Response = string.Format("<resultCode>0</resultCode><resultDesc>{3}</resultDesc><orderIdHIS>{0}</orderIdHIS><userFlag></userFlag><userHISId>{1}</userHISId><medicalcard>{2}</medicalcard><infoReq>{4}</infoReq><free>{5}</free>",
                _infoOuHos.MzRegNo, _infoOuHos.ID, _infoOuHos.CardNo, "成功", _infoOuHos.LineOrder, _infoOuHos.PatTypeId == 472 ? 1 : 0);
        }
        catch (Exception e)
        {
            trn.Rollback();
            trn.Dispose();
            Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><orderIdHIS></orderIdHIS><userFlag></userFlag><userHISId></userHISId><medicalcard></medicalcard><infoReq></infoReq>",
                e.ToString());
        }
         return string.Format("<res service=\"lockRegist\">{0}</res>", Response);
    }

    //取消锁号
    [WebMethod]
    public string cancelLockRegist(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "cancelLockRegist");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        //string orderId = GetXmlValue(Request, "req", "orderId", "").Trim();
        //string hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim();
        //string deptId = GetXmlValue(Request, "req", "deptId", "").Trim();
        //string doctorId = GetXmlValue(Request, "req", "doctorId", "").Trim();
        //DateTime regDate = GetXmlValue(Request, "req", "regDate", "").Trim() == string.Empty ? DateTime.MinValue : Convert.ToDateTime(GetXmlValue(Request, "req", "regDate", ""));
        //int shiftType = GetXmlValue(Request, "req", "shiftType", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "shiftType", ""));
        //string startTime = GetXmlValue(Request, "req", "startTime", "").Trim();
        //string endTime = GetXmlValue(Request, "req", "endTime", "").Trim();
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();

        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();


        lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
        if (lstOuHosInfo.Count == 1 && !lstOuHosInfo[0].IsCancel)
        {
            if (lstOuHosInfo[0].IsPreReg && Convert.ToBoolean(BLL.Common.Utils.GetColumnValue(" IsRegist ", "OuHosInfo", string.Format(" 0 or ID={0}", lstOuHosInfo[0].ID), "bool")))
            {
                DAL.SqlUtil db = new DAL.SqlUtil();
                System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
                try
                {
                    lstOuHosInfo[0].IsCancel = true;
                    lstOuHosInfo[0].CancelOperId = 7640;
                    lstOuHosInfo[0].CancelTime = BLL.Common.DateTimeHandler.GetServerDateTime();
                    objOuHosInfo.Modify(lstOuHosInfo[0], trn);
                    trn.Commit();
                    Response = "<resultCode>0</resultCode><resultDesc>成功</resultDesc>";
                }
                catch (Exception e)
                {
                    trn.Rollback();
                    trn.Dispose();
                    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc>", e.ToString());
                }
            }
            else
            {
                Response = "<resultCode>1</resultCode><resultDesc>没有可以取消的数据</resultDesc>";
            }
        }
        else
        {
            Response = "<resultCode>1</resultCode><resultDesc>没有可以取消的数据</resultDesc>";
        }
        return string.Format("<res service=\"cancelLockRegist\">{0}</res>", Response);
    }

    //[WebMethod]
    //public string getInfoOrder(string Request)
    //{
    //    string Response = string.Empty;
    //    string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();

    //    BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
    //    Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
    //    lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);

    //    if (lstOuHosInfo.Count > 0)
    //    {
    //        Response = "<resultCode>0</resultCode><resultDesc>成功</resultDesc>";
    //        if (lstOuHosInfo[0].DiagnDept == 0)
    //        {
    //            Response += string.Format("<orderStatus>0</orderStatus><infoSeq>{0}</infoSeq>", lstOuHosInfo[0].LineOrder);
    //        }
    //        else
    //        {
    //            BLL.COuInvoice objOuInvoice = new BLL.COuInvoice();
    //            Model.ModelList<Model.OuInvoiceInfo> lstOuInvoice = objOuInvoice.OuInvoice_SelectByMzRegId(lstOuHosInfo[0].ID);
    //            if (lstOuInvoice[0].IsCancel)
    //            {
    //                Response += string.Format("<orderStatus>2</orderStatus><infoSeq>{0}</infoSeq>", lstOuHosInfo[0].LineOrder);
    //            }
    //            else if (lstOuHosInfo[0].IsCancel)
    //            {
    //                Response += string.Format("<orderStatus>3</orderStatus><infoSeq>{0}</infoSeq>", lstOuHosInfo[0].LineOrder);
    //            }
    //            else if (lstOuHosInfo[0].DiagnDept > 0)
    //            {
    //                Response += string.Format("<orderStatus>1</orderStatus><infoSeq>{0}</infoSeq>", lstOuHosInfo[0].LineOrder);
    //            }
    //        }
    //    }
    //    return string.Format("<res service=\"getInfoOrder\">{0}</res>", Response);
    //}

    [WebMethod]
    public string returnPay(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "returnPay");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim();
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();
        string returnFee = GetXmlValue(Request, "req", "returnFee", "").Trim();
        DateTime returnTime = GetXmlValue(Request, "req", "returnTime", "").Trim() != string.Empty ? Convert.ToDateTime(GetXmlValue(Request, "req", "returnTime", "").Trim()) : DateTime.MinValue;
        string returnReason = GetXmlValue(Request, "req", "returnReason", "").Trim();

        DAL.SqlUtil db = new DAL.SqlUtil();
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        BLL.COulInvoiceReg objOulInvoiceReg = new BLL.COulInvoiceReg();
        Model.OulInvoiceRegInfo infoOulInvoiceReg = new Model.OulInvoiceRegInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
        Model.ModelList<Model.OulInvoiceRegInfo> lstOulInvoiceReg = new Model.ModelList<Model.OulInvoiceRegInfo>();
        lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
        try
        {
            if (lstOuHosInfo.Count > 0)
            {
                if (lstOuHosInfo[0].DiagnDept == 0 && returnTime.Date == lstOuHosInfo[0].OperTime.Date)
                {
                    lstOulInvoiceReg = objOulInvoiceReg.OulInvoiceReg_SelectByInvoNo(lstOuHosInfo[0].MzRegNo);
                    if (lstOulInvoiceReg.Count > 0)
                    {
                        infoOulInvoiceReg = lstOulInvoiceReg[0];
                        infoOulInvoiceReg.IsCancel = true;
                        infoOulInvoiceReg.CancelOperId = 7640;
                        infoOulInvoiceReg.CancelTime = returnTime;
                    }

                    Model.OuHosInfoInfo cancelOuHosinfo = lstOuHosInfo[0];
                    cancelOuHosinfo.IsCancel = true;
                    cancelOuHosinfo.CancelOperId = 7640;
                    cancelOuHosinfo.CancelTime = returnTime;
                    cancelOuHosinfo.CancelMemo = returnReason;

                    objOuHosInfo.Modify(cancelOuHosinfo, trn);
                    objOulInvoiceReg.Modify(infoOulInvoiceReg, trn);
                    trn.Commit();

                    Response += "<resultCode>0</resultCode><resultDesc>成功</resultDesc>";
                }
            }
        }
        catch (Exception e)
        {
            trn.Rollback();
            trn.Dispose();
            Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc>",
                        e.ToString());
        }
        return string.Format("<res service=\"returnPay\">{0}</res>", Response);
    }

    [WebMethod]
    public string refundPayCheck(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "refundPayCheck");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string hospitalId = GetXmlValue(Request, "req", "hospitalId", "").Trim();
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();
        string returnFee = GetXmlValue(Request, "req", "returnFee", "").Trim();
        DateTime returnTime = GetXmlValue(Request, "req", "returnTime", "").Trim() != string.Empty ? Convert.ToDateTime(GetXmlValue(Request, "req", "returnTime", "").Trim()) : DateTime.MinValue;
        string returnReason = GetXmlValue(Request, "req", "returnReason", "").Trim();
        string seqCode = GetXmlValue(Request, "req", "seqCode", "").Trim();

        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        BLL.COuDocRegType objOuDocRegType = new BLL.COuDocRegType();
        BLL.CBsRegTimeSpan objBsRegTimeSpan = new BLL.CBsRegTimeSpan();
        BLL.COuDocSpanSub objOUDOCSPANSUB = new BLL.COuDocSpanSub();
        BLL.CBsRegSpanSub objBsRegSpanSub = new BLL.CBsRegSpanSub();
        Model.BsRegSpanSubInfo infoBsRegSpanSub = new Model.BsRegSpanSubInfo();
        Model.OuDocSpanSubInfo infoOUDOCSPANSUB = new Model.OuDocSpanSubInfo();
        Model.BsRegTimeSpanInfo infoBsRegTimeSpan = new Model.BsRegTimeSpanInfo();
        Model.OuDocRegTypeInfo infoOuDocRegType = new Model.OuDocRegTypeInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
        lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
        DateTime dtEnd = DateTime.Now.Date;
        if (lstOuHosInfo.Count > 0)
        {
            //if (seqCode.StartsWith("R"))
            //{
            //    infoOUDOCSPANSUB = objOUDOCSPANSUB.GetByID(Convert.ToInt32(seqCode.Substring(1, seqCode.Length - 1)));
            //    infoOuDocRegType = objOuDocRegType.GetByID(Convert.ToInt32(infoOUDOCSPANSUB.F4));
            //    infoBsRegTimeSpan = objBsRegTimeSpan.GetByID(infoOuDocRegType.TimeSpanId);
            //    dtEnd.AddHours(Convert.ToDouble(infoBsRegTimeSpan.TimeEnd.Substring(0, 2))).AddMinutes(Convert.ToDouble(infoBsRegTimeSpan.TimeEnd.Substring(3, 2)));
            //}
            //else
            //{
                infoOUDOCSPANSUB = objOUDOCSPANSUB.GetByID(Convert.ToInt32(seqCode.Substring(1, seqCode.Length - 1)));
                infoBsRegSpanSub = objBsRegSpanSub.GetByID(infoOUDOCSPANSUB.TimeSpanSubID);
                dtEnd.AddHours(Convert.ToDouble(infoBsRegSpanSub.TimeEnd.Substring(0, 2))).AddMinutes(Convert.ToDouble(infoBsRegSpanSub.TimeEnd.Substring(3, 2)));
            //}
            if (lstOuHosInfo[0].DiagnDept == 0 && (returnTime.Date == lstOuHosInfo[0].OperTime.Date || returnTime <= Convert.ToDateTime(lstOuHosInfo[0].RegDate).AddHours(dtEnd.Hour).AddMinutes(dtEnd.Minute)))
            {
                Response += "<resultCode>0</resultCode><resultDesc>可以退费</resultDesc>";
            }
            else if (lstOuHosInfo[0].DiagnDept > 0)
                Response += "<resultCode>1</resultCode><resultDesc>病人已经被接诊</resultDesc>";
            else if (returnTime > Convert.ToDateTime(lstOuHosInfo[0].RegDate).AddHours(dtEnd.Hour).AddMinutes(dtEnd.Minute))
                Response += "<resultCode>1</resultCode><resultDesc>超过就诊时间，不能退费</resultDesc>";
        }
        return string.Format("<res service=\"refundPayCheck\">{0}</res>", Response);
    }

    /// <summary>
    /// 增加诊疗卡
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string addMedicalcard(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "addMedicalcard");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string Response = string.Empty;
        string patName = GetXmlValue(Request, "req", "patName", "").Trim();
        string phone = GetXmlValue(Request, "req", "phone", "").Trim();
        string gender = GetXmlValue(Request, "req", "gender", "").Trim();
        string address = GetXmlValue(Request, "req", "address", "").Trim();
        string Idcard = GetXmlValue(Request, "req", "Idcard", "").Trim();
        DAL.SqlUtil db = new DAL.SqlUtil();
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {
            BLL.CBsPatient objBsPatient = new BLL.CBsPatient();
            Model.BsPatientInfo infoBsPatient = new Model.BsPatientInfo();
            infoBsPatient.ID = 0;
            infoBsPatient.CardNo = Convert.ToString(BLL.absBusiness<Model.absModel>.ExecuteScalar("UspGetWxCardNo", null, null));//BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.CarNo).ToString();
            infoBsPatient.Name = patName;
            infoBsPatient.BirthDate = Utilities.Information.GetBirthdayByIdCard(Idcard);
            if (gender == "1")
                infoBsPatient.Sex = "M";
            else if (gender == "2")
                infoBsPatient.Sex = "F";
            infoBsPatient.PatTypeId = Convert.ToInt32(BLL.Common.Utils.GetSystemSetting("SelfPatTypeId"));
            infoBsPatient.Residence = address;
            infoBsPatient.Native = "广东省肇庆市";
            infoBsPatient.OperID = 7640;
            infoBsPatient.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();
            infoBsPatient.IdCardNo = Idcard;
            infoBsPatient.CertificateId = 1;
            infoBsPatient.Mobile = phone;
            infoBsPatient.ID = objBsPatient.Create(infoBsPatient, trn);
            trn.Commit();

            Response += string.Format("<resultCode>0</resultCode><resultDesc>成功</resultDesc><medicalcard>{0}</medicalcard>", infoBsPatient.CardNo);
        }
        catch (Exception e)
        {
            trn.Rollback();
            trn.Dispose();
            Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc><medicalcard></medicalcard>", e.ToString());
        }
        return string.Format("<res service=\"addMedicalcard\">{0}</res>", Response);
    }

    [WebMethod]
    public string medicalcardQueue(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "medicalcardQueue");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string patientName = GetXmlValue(Request, "req", "patientName", "").Trim();
        string medicalcardNum = GetXmlValue(Request, "req", "medicalcardNum", "").Trim();

        BLL.CBsPatient objBsPatient = new BLL.CBsPatient();
        Model.ModelList<Model.BsPatientInfo> lstBsPatient = objBsPatient.GetDynamic(string.Format(" Name = '{0}' and CardNo = '{1}'", patientName, medicalcardNum), null);

        if (lstBsPatient.Count > 0)
        {
            Response += string.Format("<resultCode>0</resultCode><patientIdCard>{0}</patientIdCard><mobile>{1}</mobile><birthday>{2}</birthday><sex>{3}</sex><address>{4}</address>",
                lstBsPatient[0].IdCardNo, lstBsPatient[0].Mobile, lstBsPatient[0].BirthDate.ToString("yyyy-MM-dd"), lstBsPatient[0].Sex == "M" ? "1" : "2", lstBsPatient[0].Residence);
        }
        else
            Response += "<resultCode>1</resultCode><patientIdCard></patientIdCard><mobile></mobile><birthday></birthday><sex></sex><address></address>";
        return string.Format("<res service=\"medicalcardQueue\">{0}</res>", Response);
    }

    [WebMethod]
    public string printRegInfo(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "printRegInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();

        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
        Model.ModelList<Model.OuHosInfoInfo> lstnowSeq = new Model.ModelList<Model.OuHosInfoInfo>();
        lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
        if (lstOuHosInfo.Count > 0 && !lstOuHosInfo[0].IsCancel)
        {
            lstnowSeq = objOuHosInfo.GetDynamic(string.Format(" DoctorId = {0} and DiagnDept is null and (ouhosinfo.RegTime>=(getdate()) and ouhosinfo.RegTime < (getdate())+1) and tallytime is null and IsCancel = 0 and RegDept = {1} and ((ouhosinfo.IsPreReg =0 and exists (select mzregid from oulinvoicereg where f4<>'1' and mzregid = ouhosinfo.id  )) or (ouhosinfo.IsPreReg=1 and ouhosinfo.f8='1'  and  (ouhosinfo.Timespansubid<=dbo.fn_getpansubId(getdate()) or ouhosinfo.iselder=1)))",
                                                     lstOuHosInfo[0].DoctorId, lstOuHosInfo[0].RegDept), "LineOrder");

            Response += string.Format("<infoSeq>{0}</infoSeq><nowSeq>{1}</nowSeq><resultCode>0</resultCode><resultDesc>成功</resultDesc>",
                lstOuHosInfo[0].LineOrder, lstnowSeq.Count > 0 ? lstnowSeq[0].LineOrder : -1);
        }
        else
            Response = "<infoSeq>0</infoSeq><nowSeq>0</nowSeq><resultCode>1</resultCode><resultDesc>没有记录</resultDesc>";
        return string.Format("<res service=\"printRegInfo\">{0}</res>", Response);
    }

    [WebMethod]
    public string addRegist(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "addRegist");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string Response = string.Empty;
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();
        string orderWXID = GetXmlValue(Request, "req", "orderWXID", "").Trim();
        int paymeth = GetXmlValue(Request, "req", "paymeth", "").Trim() == string.Empty ? 0 : Convert.ToInt32(GetXmlValue(Request, "req", "paymeth", "").Trim());

        DAL.SqlUtil db = new DAL.SqlUtil() ;
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        BLL.COulInvoiceReg objOulInvoiceReg = new BLL.COulInvoiceReg();
        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
        try
        {
            lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);
            
            if (lstOuHosInfo.Count == 1 && !lstOuHosInfo[0].IsCancel)                
            {
                if (lstOuHosInfo[0].IsPreReg && Convert.ToBoolean(BLL.Common.Utils.GetColumnValue(" IsRegist ", "OuHosInfo", string.Format(" 0 or ID={0}", lstOuHosInfo[0].ID), "bool")))
                {
                    BLL.Common.Utils.UpdateColumn("OuHosInfo", string.Format("IsRegist={0}", 0), lstOuHosInfo[0].ID, null);
                    if (paymeth == 2)
                    {
                        BLL.Common.Utils.UpdateColumn("OuHosInfo", "F8='1'", lstOuHosInfo[0].ID, null);
                        //BLL.Common.Utils.UpdateColumn("OuHosInfo", string.Format("IsPreReg={0}", 0), lstOuHosInfo[0].ID, null);
                        Model.OulInvoiceRegInfo infoOuInvoiceRe = lstOuHosInfo[0].ConvertTo<Model.OulInvoiceRegInfo>();
                        infoOuInvoiceRe.MzRegId = lstOuHosInfo[0].ID;
                        infoOuInvoiceRe.InvoTime = BLL.Common.DateTimeHandler.GetServerDateTime();
                        infoOuInvoiceRe.InvoNo = lstOuHosInfo[0].MzRegNo;
                        infoOuInvoiceRe.F4 = string.Empty;
                        infoOuInvoiceRe.InvoLastId = 0;
                        infoOuInvoiceRe.HospitalId = 31;
                        infoOuInvoiceRe.LocationId = lstOuHosInfo[0].RegDept;
                        infoOuInvoiceRe.RegFee = lstOuHosInfo[0].RegFee;
                        infoOuInvoiceRe.DiagnoFee = lstOuHosInfo[0].DiagnoFee;
                        BLL.COulInvoiceReg _objOuInvoiceReg = new BLL.COulInvoiceReg();
                        infoOuInvoiceRe.ID = objOulInvoiceReg.Create(infoOuInvoiceRe, trn);
                        trn.Commit();
                        BLL.Common.Utils.UpdateColumn("OulInvoiceReg", string.Format("OrderWXID={0}", orderWXID), infoOuInvoiceRe.ID, null);
                  }

                    Response = string.Format("<resultCode>0</resultCode><resultDesc>{0}</resultDesc>",
                        "成功");

                    string DiagRoom = string.Empty;
                    if (lstOuHosInfo[0].DiagRoomId > 0)
                    {
                        BLL.CBsDiagRoom objBsDiagRoom = new BLL.CBsDiagRoom();
                        Model.BsDiagRoomInfo infoBsDiagRoom = objBsDiagRoom.GetByID(lstOuHosInfo[0].DiagRoomId);
                        DiagRoom = infoBsDiagRoom.Name;
                    }
                    else
                    {
                        BLL.CBsLocation objBsLocation = new BLL.CBsLocation();
                        Model.BsLocationInfo infoBsLocation = objBsLocation.GetByID(lstOuHosInfo[0].RegDept);
                        DiagRoom = infoBsLocation.Name;
                    }
                    string url = string.Format("http://gdqmjk.net:7780/HisService/api/his/pushNotice?request=<req><orderIdHIS>{0}</orderIdHIS><type>2</type><title>导诊通知</title><content>请到{1}就诊</content></req>",
                        lstOuHosInfo[0].MzRegNo, DiagRoom);
                    HttpWebRequest request = null;
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.Timeout = 60000;
                    HttpWebResponse res;
                    string xml = string.Empty;
                    try
                    {
                        res = (HttpWebResponse)request.GetResponse();
                        xml = new StreamReader(res.GetResponseStream()).ReadToEnd();
                    }
                    catch (WebException ex)
                    {
                        res = (HttpWebResponse)ex.Response;
                    }
                    if (GetXmlValue(xml, "res", "resultCode", "").Trim() != "0")
                    {
                        string tip2 = string.Format("\r\n时间[{0}]：{1}", BLL.Common.DateTimeHandler.GetServerDateTime(), xml);
                        Utilities.Document log2 = new Utilities.Document();
                        log2.SaveLog(tip2, "C:\\WXJK.log");
                    }
                }
                else
                    Response = "<resultCode>1</resultCode><resultDesc>没有符合条件的数据</resultDesc>";
            }
            else
                Response = "<resultCode>1</resultCode><resultDesc>没有符合条件的数据</resultDesc>";
        }
        catch (Exception e)
        {
            trn.Rollback();
            trn.Dispose();
            Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc>",
                e.ToString());
        }
        return string.Format("<res service=\"addRegist\">{0}</res>", Response);
    }

    [WebMethod]
    public string cancelRegist(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "cancelRegist");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string orderIdHIS = GetXmlValue(Request, "req", "orderIdHIS", "").Trim();
        DateTime cancelTime = GetXmlValue(Request, "req", "cancelTime", "").Trim() == string.Empty ? BLL.Common.DateTimeHandler.GetServerDateTime() : Convert.ToDateTime(GetXmlValue(Request, "req", "cancelTime", ""));
        string cancelReason = GetXmlValue(Request, "req", "cancelReason", "").Trim();

        BLL.COuHosInfo objOuHosInfo = new BLL.COuHosInfo();
        Model.ModelList<Model.OuHosInfoInfo> lstOuHosInfo = new Model.ModelList<Model.OuHosInfoInfo>();
        BLL.COulInvoiceReg objOulInvoiceReg = new BLL.COulInvoiceReg();
        Model.OulInvoiceRegInfo infoOulInvoiceReg = new Model.OulInvoiceRegInfo();
        Model.ModelList<Model.OulInvoiceRegInfo> lstOulInvoiceReg = new Model.ModelList<Model.OulInvoiceRegInfo>();
        lstOuHosInfo = objOuHosInfo.OuHosInfo_SelectByMzRegNo(orderIdHIS);

        if (lstOuHosInfo.Count == 1 && !lstOuHosInfo[0].IsCancel)
        {
            if (!Convert.ToBoolean(BLL.Common.Utils.GetColumnValue(" IsRegist ", "OuHosInfo", string.Format(" 0 or ID={0}", lstOuHosInfo[0].ID), "bool")))
            {
                DAL.SqlUtil db = new DAL.SqlUtil();
                System.Data.Common.DbTransaction trn = db.GetSqlTransaction();

                try
                {
                    lstOulInvoiceReg = objOulInvoiceReg.OulInvoiceReg_SelectByInvoNo(lstOuHosInfo[0].MzRegNo);
                    if (lstOulInvoiceReg.Count > 0)
                    {
                        infoOulInvoiceReg = lstOulInvoiceReg[0];
                        infoOulInvoiceReg.IsCancel = true;
                        infoOulInvoiceReg.CancelOperId = 7640;
                        infoOulInvoiceReg.CancelTime = cancelTime;
                    }

                    lstOuHosInfo[0].IsCancel = true;
                    lstOuHosInfo[0].CancelOperId = 7640;
                    lstOuHosInfo[0].CancelMemo = cancelReason;
                    lstOuHosInfo[0].CancelTime = cancelTime;
                    objOuHosInfo.Modify(lstOuHosInfo[0], trn);
                    objOulInvoiceReg.Modify(infoOulInvoiceReg, trn);
                    trn.Commit();
                    Response = string.Format("<resultCode>0</resultCode><resultDesc>{0}</resultDesc>", "成功");
                }
                catch (Exception e)
                {
                    trn.Rollback();
                    trn.Dispose();
                    Response = string.Format("<resultCode>1</resultCode><resultDesc>{0}</resultDesc>", e.ToString());
                }
            }
            else
                Response = "<resultCode>1</resultCode><resultDesc>没有可以取消的数据</resultDesc>";
        }
        else
            Response = "<resultCode>1</resultCode><resultDesc>没有可以取消的数据</resultDesc>";
        return string.Format("<res service=\"cancelRegist\">{0}</res>", Response);
    }

    /// <summary>
    /// 查询7天内能挂的医生号情况
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string DocList7Day(string Request)
    {
        String columns = "req,DeptId,RowId";

        string Response = string.Empty;
        int locationId = Convert.ToInt32(GetXmlValue(Request, "req", "DeptId", "")); //医生id      
        int DocId = Convert.ToInt32(GetXmlValue(Request, "req", "RowId", "")); //医生id  

        Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(locationId); //取得科室信息

        bool isVIP = false;


        Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = new Model.ModelList<Model.BsDocRegTypeInfo>();

       
        DateTime newDate=BLL.Common.DateTimeHandler.GetServerDateTime();
        Double day = 1;
        do
        {
            int week = GetWeek(newDate.AddDays(day));

            Model.ModelList<Model.BsDocRegTypeInfo> tempList = _objBsDocRegType.GetDynamic(string.Format("LocationId = {0} and WeekDay = {1} and DoctorId={2}", locationId, week, DocId), "");

            Model.BsDocRegTypeInfo info = new Model.BsDocRegTypeInfo();
            info.WeekDay = week;
            info.F1 = newDate.AddDays(day).ToString("yyyy-MM-dd");
            if (tempList.Count > 0)
                info.F2 = "出诊";
            else
                info.F2 = "停诊";
            lstBsDocRegType.Add(info);
            day++;
        }
        while (day != 8);


        if (lstBsDocRegType.Count > 0)
        {
            BLL.CBsRegSpanSub objRegSpanSub = new BLL.CBsRegSpanSub();
            //BLL.CBsSpecialtyRoom objBsSpecialtyRoom = new BLL.CBsSpecialtyRoom();

            Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg><DOCHBList>";
            foreach (Model.BsDocRegTypeInfo infoBsDocRegType in lstBsDocRegType)
            {


                Response += string.Format("<Item><Date>{0}</Date><LsType>{1}</LsType></Item>",
                    infoBsDocRegType.F1, infoBsDocRegType.F2);
            }
            Response += "</DOCHBList>";
        }
        else
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg>请与系统管理员联系排班周没安排医生</ErrorMsg>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 查询医生当天号时间段列表
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string DOCTimeList(string Request)
    {
        String columns = "req,DocId,Day,TimeSpanId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        int DocId = Convert.ToInt32(GetXmlValue(Request, "req", "DocId", ""));
        int TimeSpanId = Convert.ToInt32(GetXmlValue(Request, "req", "TimeSpanId", ""));
        DateTime Day = Convert.ToDateTime(GetXmlValue(Request, "req", "Day", ""));
         Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType =new Model.ModelList<Model.BsDocRegTypeInfo>();
         if (TimeSpanId > 0)
             lstBsDocRegType = _objBsDocRegType.GetDynamic(string.Format(" DoctorId={0} and  WeekDay = {1} and TimeSpanId={2}", DocId, GetWeek(Day), TimeSpanId), "WeekDay");
         else
             lstBsDocRegType = _objBsDocRegType.GetDynamic(string.Format(" DoctorId={0} and  WeekDay = {1} and not exists (select id from OuDocRegType where weekplanid=BsDocRegType.id and OuDocRegType.iscancel=1  and (OuDocRegType.Regdate)=convert(char(10),'{2}',120)) ", DocId, GetWeek(Day), Day), "WeekDay");
        //Model.BsDocRegTypeInfo infoBsDocRegType = new Model.BsDocRegTypeInfo();

        //挂号类别
        //   Model.BsRegTimeSpanInfo infoBsRegTimeSpan = _objBsRegTimeSpan.GetByID(Convert.ToInt32(ouhosinfo.F8)); //排班类型


        ///  Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = _objBsDocRegType.GetDynamic(string.Format("LocationId = {0} and WeekDay = {1} and TimeSpanId >={2}", locationId, GetWeek(), GetTimeSpan()), "");
        if (lstBsDocRegType.Count > 0)
        {
            BLL.CBsRegSpanSub objRegSpanSub = new BLL.CBsRegSpanSub();
            Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg><List>";
            foreach (Model.BsDocRegTypeInfo infoBsDocRegType in lstBsDocRegType)
            {
                //Model.BsRegTypeInfo infoBsRegType = _objBsRegType.GetByID(infoBsDocRegType.RegTypeId);  //挂号类别
                Model.BsRegTimeSpanInfo infoBsRegTimeSpan = _objBsRegTimeSpan.GetByID(infoBsDocRegType.TimeSpanId); //排班类型
                //Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(infoBsDocRegType.DoctorId); //医生
                //Model.BsDocLevelInfo infoBsDocLevel = _objBsDocLevel.GetByID(infoBsDoctor.DocLevId); //医生职称
                //Model.BsRegPatAmountInfo infoBsRegPatAmount = _objBsRegPatAmount.GetDynamic(string.Format("regtypeid = {0} and pattypeid = 116", infoBsRegType.ID), "")[0];//挂号费
                //double RegFeeAmout = infoBsRegPatAmount.RegFee + infoBsRegPatAmount.DiagnoFee;

                //Model.ModelList<Model.BsRegSpanSubInfo> lstRegSpanSub = new Model.ModelList<Model.BsRegSpanSubInfo>();
                //lstRegSpanSub = objRegSpanSub.BsRegSpanSub_SelectByTimeSpanId(infoBsDocRegType.TimeSpanId);

                BLL.Finder<Model.OuDocSpanSubInfo> find = new BLL.Finder<Model.OuDocSpanSubInfo>();
                Model.ListView<Model.OuDocSpanSubInfo> _lstvOUDOCSPANSUB;
                find.AddParameter("RegTime", Day);
                find.AddParameter("WeekDay", Convert.ToInt32(infoBsDocRegType.WeekDay));
                find.AddParameter("TimeSpanId", infoBsDocRegType.TimeSpanId);
                find.AddParameter("DoctorId", infoBsDocRegType.DoctorId);
                _lstvOUDOCSPANSUB = find.Find("uspOUDOCSPANSUBPre").DefaultView;


                foreach (Model.OuDocSpanSubInfo info in _lstvOUDOCSPANSUB)
                {
                    Model.ModelList<Model.uspGetOuDoctorNumPlanQry> _lstDoctorNumPlan = XYHIS.Common.Helper.GetOuDoctorNumPlan(infoBsDocRegType.DoctorId, infoBsDocRegType.TimeSpanId, info.TimeSpanSubID, Day);

                    int cutter = _lstDoctorNumPlan.Find("TimeSpanSubId", info.TimeSpanSubID.ToString()).Count;
                    info.SpanLimitNum = Convert.ToInt32(info.SubLimitNum) - cutter;


                    if (info.SpanLimitNum > 0)
                    {
                        Model.BsRegSpanSubInfo infoRegSpanSub = objRegSpanSub.GetByID(info.TimeSpanSubID);

                        Response += string.Format("<printDate><HBTime>{0}</HBTime><DocTime>{1}</DocTime><TimeSpanId>{4}</TimeSpanId><TimeSpanSubId>{2}</TimeSpanSubId><HasCount>{3}</HasCount></printDate>",
                  infoBsRegTimeSpan.Name, infoRegSpanSub.TimeBegin + "-" + infoRegSpanSub.TimeEnd, infoRegSpanSub.ID, info.SpanLimitNum, infoRegSpanSub.TimeSpanId);

                    }
                }

                //foreach (Model.BsRegSpanSubInfo infoRegSpanSub in lstRegSpanSub)
                //{
                //    int HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsPreReg=1  and TimeSpanId={0} and TimeSpanSubId={1}  and RegTime>=trunc(to_date('{2}','yyyy-mm-dd hh24:mi:ss')) and RegTime<trunc(to_date('{3}','yyyy-mm-dd hh24:mi:ss'))+1 and DoctorId={4} )", infoBsDocRegType.TimeSpanId, infoRegSpanSub.ID, Day.Date, Day.Date, infoBsDocRegType.DoctorId), "int"));
                //    //  Response += string.Format("<printDate><DepId>{0}</DepId><DepDesc>{1}</DepDesc><RegType>{2}</RegType><HBTime>{3}</HBTime><RowId>{4}</RowId><SumFee>{5}</SumFee><SessionType>{6}</SessionType><DocName>{7}</DocName><DocRegID>{8}</DocRegID><DocTime>{9}</DocTime><DocRoom>{10}</DocRoom><DocLimit>{11}</DocLimit><DocCurrent>{12}</DocCurrent><GHF>{13}</GHF><ZJ>{14}</ZJ><Sex>{15}</Sex><timeName>{16}</timeName><TimeSpanSubId>{17}</TimeSpanSubId></printDate>",
                //    //infoBsDocRegType.LocationId, "", infoBsRegType.Name, infoBsRegTimeSpan.Name, infoBsDocRegType.DoctorId, RegFeeAmout, infoBsDocLevel.Name, infoBsDoctor.Name, infoBsDocRegType.ID, infoRegSpanSub.TimeBegin + "-" + infoRegSpanSub.TimeEnd, infoBsDocRegType.F1, infoBsDocRegType.LimitNum, infoBsDocRegType.LastLimitNum, infoBsRegPatAmount.RegFee, infoBsRegPatAmount.DiagnoFee, infoBsDoctor.Sex == "M" ? "男" : "女", infoBsRegTimeSpan.Name, infoRegSpanSub.ID);
                //    Response += string.Format("<printDate><HBTime>{0}</HBTime><DocTime>{1}</DocTime><TimeSpanId>{4}</TimeSpanId><TimeSpanSubId>{2}</TimeSpanSubId><HasCount>{3}</HasCount></printDate>",
                //   infoBsRegTimeSpan.Name, infoRegSpanSub.TimeBegin + "-" + infoRegSpanSub.TimeEnd, infoRegSpanSub.ID, Convert.ToInt32(infoRegSpanSub.F1) - HasCount, infoRegSpanSub.TimeSpanId);
                //}
            }
            Response += "</List>";
        }
        else
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg>请与系统管理员联系日排班没安排医生</ErrorMsg>";
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 挂号
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string OPRegist(string Request)
    {
        String columns = "req,OPRegist,UserId,SumFee,DocId,Day,DocRegID,PatId,DeptId,TimeSpanSubId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }


     //   new Utilities.Document().SaveLog(string.Format("\r\n\r\n   {0}保存OPRegist挂号(预约)下单：{1}", BLL.Common.DateTimeHandler.GetServerDateTime().ToString(), req), string.Format("C:\\"  + BLL.Common.DateTimeHandler.GetServerDateTime().ToString("yyyy-MM-dd")));

        string BCard = GetXmlValue(Request, "req", "OPRegist", "BCard");
        string BAmount = GetXmlValue(Request, "req", "OPRegist", "BAmount");
        string BDate = GetXmlValue(Request, "req", "OPRegist", "BDate");
        string BTime = GetXmlValue(Request, "req", "OPRegist", "BTime");
        string BNo = GetXmlValue(Request, "req", "OPRegist", "BNo");
        string BMNo = GetXmlValue(Request, "req", "OPRegist", "BMNo");
        string BCode = GetXmlValue(Request, "req", "OPRegist", "BCode");
        string BType = GetXmlValue(Request, "req", "OPRegist", "BType");
        string BHosName = GetXmlValue(Request, "req", "OPRegist", "BHosName");
        

        string Response = string.Empty;
        int hospId = 0;
        int bkId = 0;

        DateTime Day = Convert.ToDateTime(GetXmlValue(Request, "req", "OPRegist", "Day"));
        int DeptId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DeptId"));
        int userId = GetUserID(GetXmlValue(Request, "req", "OPRegist", "UserId"));
        int DocRegID = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocRegID"));
        double amount = Convert.ToDouble(GetXmlValue(Request, "req", "OPRegist", "SumFee")); //挂号费
        int TimeSpanId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "TimeSpanId"));
        string bkRegNo = "";// GetXmlValue(Request, "req", "OPRegist", "FlowNo");
        Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocId"))); //查询医生表
        Model.BsDocRegTypeInfo infoDocRegType = _objBsDocRegType.GetByID(DocRegID);
        DateTime newDate = BLL.Common.DateTimeHandler.GetServerDateTime();
        DAL.SqlUtil db = new DAL.SqlUtil();

        Model.BsRegSpanSubInfo infoRegSpanSub = new Model.BsRegSpanSubInfo();

        if (infoDocRegType.ID == 0)
        {
            string strWhere = string.Format(" DoctorId = {0} and WeekDay = {1} and TimeSpanId>={2} and not exists (select id from OuDocRegType where weekplanid=BsDocRegType.id and OuDocRegType.iscancel=1  and (OuDocRegType.Regdate)=convert(char(10),'{3}',120)) ", infoBsDoctor.ID, GetWeek(Day), TimeSpanId, Day);
            Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = _objBsDocRegType.GetDynamic(strWhere, "Id");
            if (lstBsDocRegType.Count > 0)
                infoDocRegType = lstBsDocRegType[0];
        }
        Model.BsRegTimeSpanInfo infoBsRegTimeSpan = _objBsRegTimeSpan.GetByID(infoDocRegType.TimeSpanId);
        if (!infoDocRegType.IsActive)
        {
            Response = String.Format("<ResultCode>3</ResultCode><ErrorMsg>挂号失败，{0}</ErrorMsg>", "停诊");
            Response = string.Format("<Response>{0}</Response>", Response);
            return Response;
        }
        Model.ModelList<Model.OuHosInfoInfo> tempList = new Model.ModelList<Model.OuHosInfoInfo>();
        tempList = _objOuHosInfo.GetDynamic(string.Format(" IsCancel=0 and PatId={0} and RegTime>=convert(char(10),'{1}',120) and RegTime<convert(char(10),dateadd(d,1,'{2}'),120) and DoctorId={3}", Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "PatId")), Day, Day, infoBsDoctor.ID), null);
        if (tempList.Count > 0)
        {

            Response = String.Format("<ResultCode>3</ResultCode><ErrorMsg>挂号失败,该卡号在同一天已经预约了该医生</ErrorMsg>");
            Response = string.Format("<Response>{0}</Response>", Response);
            return Response;
        }
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = new Model.ModelList<Model.BsPatientInfo>();
            Model.BsPatientInfo infoBsPatient = _objBsPatient.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "PatId"))); // 查询病人类别表            

          


            Model.OuHosInfoInfo infoOuHosInfo = new Model.OuHosInfoInfo();

            if (amount == 0)
                infoOuHosInfo.IsFreeDiag = infoOuHosInfo.IsPriority = true;

            infoOuHosInfo.MzRegNo = BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.MzReg).ToString(); //GetXmlValue(Request, "req", "OPRegist", "CardNo"); //门诊流水号
          
            infoOuHosInfo.PatId = infoBsPatient.ID;  //病人id
            infoOuHosInfo.CardNo = infoBsPatient.CardNo; //卡号
            infoOuHosInfo.Name = infoBsPatient.Name;
            infoOuHosInfo.Sex = infoBsPatient.Sex;
            infoOuHosInfo.PatTypeId = 116;  //病人类别 自费
            if (infoBsPatient.PatTypeId == 472)
                infoOuHosInfo.PatTypeId = 472;  //病人类别
            infoOuHosInfo.RegTypeId = infoBsDoctor.RegTypeId; //挂号类别
            infoOuHosInfo.DoctorId = infoBsDoctor.ID;// Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocId"));//bsdoctorid 医生id        
            infoOuHosInfo.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();// Convert.ToDateTime(GetXmlValue(Request, "req", "OPRegist", "Day"));  //挂号时发生的时间
            infoOuHosInfo.OperId = userId; //操作员id  自助挂号 的 userid
            infoOuHosInfo.RegDept = infoDocRegType.LocationId; //挂号科室id  
            // infoOuHosInfo.DiagnDept = lstBsDocRegType[0].LocationId;//接诊科室
            if (amount > 1)
            {
                infoOuHosInfo.RegFee = 1;
                infoOuHosInfo.DiagnoFee = amount - 1;
            }
            if (infoOuHosInfo.PatTypeId == 472)
            {
                infoOuHosInfo.RegFee = 0;
                infoOuHosInfo.DiagnoFee = 0;
            }
            infoOuHosInfo.RegTime = BLL.Common.DateTimeHandler.GetServerDateTime(); //挂号时间
            infoOuHosInfo.TimeSpanId = infoBsRegTimeSpan.ID;// "1";
            if (Day.Date != BLL.Common.DateTimeHandler.GetServerDateTime().Date)
            {
                infoOuHosInfo.RegTime = Day;
                infoRegSpanSub = _objBsRegSpanSub.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "TimeSpanSubId")));
                infoOuHosInfo.IsPreReg = true;
                infoOuHosInfo.IsPriority = true;
                infoOuHosInfo.F4 = "自助";
             
                infoOuHosInfo.TimeSpanSubId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "TimeSpanSubId"));
            }
            infoOuHosInfo.F1 = "1";
            infoOuHosInfo.IsCancel = false;
            infoOuHosInfo.PaySel = 1;


             BLL.Common.CalBirthday objCalBirthday = new BLL.Common.CalBirthday();
             objCalBirthday.Birthday = infoBsPatient.BirthDate;
             objCalBirthday.NowDate = infoOuHosInfo.RegTime;
             infoOuHosInfo.Age = objCalBirthday.Age + (objCalBirthday.Months > 10 ? objCalBirthday.Months / 100 : objCalBirthday.Months / 10);
             infoOuHosInfo.AgeString = BLL.Common.Utils.CalAge(objCalBirthday);
             infoOuHosInfo.ContactPhone = GetNotEmptyString(infoBsPatient.Mobile, infoBsPatient.Phone, infoBsPatient.PhoneHome, infoBsPatient.PhoneWork);
            

            int HasCount = 0;
            int TolCount = 1;
            bool isCanRegist = true;
            if (Day.Date == BLL.Common.DateTimeHandler.GetServerDateTime().Date)
            {
                TolCount = infoDocRegType.LimitNum;
                HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsCancel=0 and TimeSpanId={0}   and RegTime>=convert(char(10),'{1}',120) and RegTime<convert(char(10),dateadd(d,1,'{2}'),120) and DoctorId={3})", infoBsRegTimeSpan.ID, Day.Date, Day.Date, infoBsDoctor.ID), "int"));
               
            }
            else
            {

                TolCount = Convert.ToInt32(infoRegSpanSub.F1);
                HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsCancel=0 and IsPreReg=1  and TimeSpanId={0}   and RegTime>=convert(char(10),'{1}',120) and RegTime<convert(char(10),dateadd(d,1,'{2}'),120) and DoctorId={3} and TimeSpanSubId ={4} ) ", infoBsRegTimeSpan.ID, Day.Date, Day.Date, infoBsDoctor.ID, infoRegSpanSub.ID), "int"));
            }
            infoOuHosInfo.LineOrder = 0;
            if (HasCount >= TolCount) 
                isCanRegist = false;
            if (!isCanRegist)
            {
                trn.Rollback();
                trn.Dispose();
                Response = String.Format("<ResultCode>2</ResultCode><ErrorMsg>挂号失败，{0}</ErrorMsg>", "号满");
                Response = string.Format("<Response>{0}</Response>", Response);
                return Response;
            }

            hospId = _objOuHosInfo.Create(infoOuHosInfo, trn);
            int nvoiceReID=0;
            if (!infoOuHosInfo.IsPreReg)
            {
                Model.OulInvoiceRegInfo infoOuInvoiceRe = infoOuHosInfo.ConvertTo<Model.OulInvoiceRegInfo>();
                infoOuInvoiceRe.MzRegId = hospId;
                infoOuInvoiceRe.InvoTime = BLL.Common.DateTimeHandler.GetServerDateTime();
                infoOuInvoiceRe.InvoNo = infoOuHosInfo.MzRegNo;
                infoOuInvoiceRe.F4 = string.Empty;
                infoOuInvoiceRe.InvoLastId = 0;
                infoOuInvoiceRe.HospitalId = 31;
                infoOuInvoiceRe.LocationId = infoOuHosInfo.RegDept;
                BLL.COulInvoiceReg _objOuInvoiceReg = new BLL.COulInvoiceReg();
               nvoiceReID= infoOuInvoiceRe.ID = _objOuInvoiceReg.Create(infoOuInvoiceRe, trn);

          
               bkId = UpBankPatCardLog(infoBsPatient.ID, 2, "挂号扣减", userId, BNo, amount, trn, BCard, BMNo, BCode, nvoiceReID.ToString());
               // bkId = BLL.InsertAccount.UpPatCardFee(infoBsPatient.ID, 2, "挂号扣减", userId, hospId.ToString(), amount, trn);
            }


            trn.Commit();

            BLL.Common.Utils.UpdateColumn("ouhosinfo",string.Format(" BHosName ='{0}' " ,BHosName), hospId, null);
            if (nvoiceReID > 0)
                BLL.Common.Utils.UpdateColumn("oulinvoicereg", string.Format(" BHosName ='{0}' ", BHosName), nvoiceReID, null);
        }
        catch (global::System.Exception ex)
        {
            trn.Rollback();
            trn.Dispose();
            Response = String.Format("<ResultCode>1</ResultCode><ErrorMsg>挂号失败，{0}</ErrorMsg>", ex.Message.ToString());
            return Response;
            //throw ex;
        }
        if (hospId > 0)
        {
            Model.OuHosInfoInfo infoOuhosinfo = _objOuHosInfo.GetByID(hospId);
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(infoOuhosinfo.RegDept);
            Model.BsDocLevelInfo infoBsDocLevel = _objBsDocLevel.GetByID(infoBsDoctor.DocLevId); //医生职称

            if (Day.Date != BLL.Common.DateTimeHandler.GetServerDateTime().Date)
            {
               
                infoBsRegTimeSpan.TimeBegin = infoRegSpanSub.TimeBegin;
                infoBsRegTimeSpan.TimeEnd = infoRegSpanSub.TimeEnd;
            }

            string XNo = string.Format("候诊排号：{0}", "分诊台报到排号");


            Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>挂号成功</ErrorMsg><SerID></SerID>" +
                "<PrintData> <CardNo>{0}</CardNo><PatName>{1}</PatName>" +
                "<DepDesc>{2}</DepDesc><RMB>{3}</RMB><UserId>{4}</UserId><SessionType>{5}</SessionType><LocInfo>{6}</LocInfo><DocDesc>{7}</DocDesc><DocTime>{8}</DocTime><XNo>{9}</XNo><MzRegNo>{10}</MzRegNo> <Outid>{11}</Outid> <intBkStoreLogID>{12}</intBkStoreLogID><newDate>{13}</newDate><DeptAdd>{14}</DeptAdd></PrintData>",
                infoOuhosinfo.CardNo, infoOuhosinfo.Name, infoBsLocation.Name, amount, userId, infoBsDocLevel.Name, infoDocRegType.F4, infoBsDoctor.Name, infoBsRegTimeSpan.TimeBegin + "-" + infoBsRegTimeSpan.TimeEnd, XNo, infoOuhosinfo.MzRegNo, hospId, bkId, newDate.ToString(), infoBsLocation.F3);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 挂号
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string OPRegistJZ(string Request)
    {
        String columns = "req,OPRegist,UserId,SumFee,Day,PatId,DeptId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }


      //  new Utilities.Document().SaveLog(string.Format("\r\n\r\n   {0}保存OPRegistJZ急诊下单：{1}", BLL.Common.DateTimeHandler.GetServerDateTime().ToString(), req), string.Format("C:\\"  + BLL.Common.DateTimeHandler.GetServerDateTime().ToString("yyyy-MM-dd")));

        string BCard = GetXmlValue(Request, "req", "OPRegist", "BCard");
        string BAmount = GetXmlValue(Request, "req", "OPRegist", "BAmount");
        string BDate = GetXmlValue(Request, "req", "OPRegist", "BDate");
        string BTime = GetXmlValue(Request, "req", "OPRegist", "BTime");
        string BNo = GetXmlValue(Request, "req", "OPRegist", "BNo");
        string BMNo = GetXmlValue(Request, "req", "OPRegist", "BMNo");
        string BCode = GetXmlValue(Request, "req", "OPRegist", "BCode");
        string BType = GetXmlValue(Request, "req", "OPRegist", "BType");
        string BHosName = GetXmlValue(Request, "req", "OPRegist", "BHosName");

        int RegTypeId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "RegTypeId"));

        string Response = string.Empty;
        int hospId = 0;
        int bkId = 0;

        DateTime Day = Convert.ToDateTime(GetXmlValue(Request, "req", "OPRegist", "Day"));
        int DeptId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DeptId"));
        int userId = GetUserID(GetXmlValue(Request, "req", "OPRegist", "UserId"));
  //      int DocRegID = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocRegID"));
        double amount = Convert.ToDouble(GetXmlValue(Request, "req", "OPRegist", "SumFee")); //挂号费
     //   int TimeSpanId = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "TimeSpanId"));
        string bkRegNo = "";// GetXmlValue(Request, "req", "OPRegist", "FlowNo");
   //     Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocId"))); //查询医生表
    //    Model.BsDocRegTypeInfo infoDocRegType = _objBsDocRegType.GetByID(DocRegID);
        DateTime newDate = BLL.Common.DateTimeHandler.GetServerDateTime();
        DAL.SqlUtil db = new DAL.SqlUtil();
 

        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = new Model.ModelList<Model.BsPatientInfo>();
            Model.BsPatientInfo infoBsPatient = _objBsPatient.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "PatId"))); // 查询病人类别表            




            Model.OuHosInfoInfo infoOuHosInfo = new Model.OuHosInfoInfo();

            if (amount == 0)
                infoOuHosInfo.IsFreeDiag = infoOuHosInfo.IsPriority = true;

            infoOuHosInfo.MzRegNo = BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.MzReg).ToString(); //GetXmlValue(Request, "req", "OPRegist", "CardNo"); //门诊流水号

            infoOuHosInfo.PatId = infoBsPatient.ID;  //病人id
            infoOuHosInfo.CardNo = infoBsPatient.CardNo; //卡号
            infoOuHosInfo.Name = infoBsPatient.Name;
            infoOuHosInfo.Sex = infoBsPatient.Sex;
            infoOuHosInfo.PatTypeId = 116;  //病人类别 自费
            if (infoBsPatient.PatTypeId == 472)
                infoOuHosInfo.PatTypeId = 472;  //病人类别
            infoOuHosInfo.RegTypeId = RegTypeId; //挂号类别
        //    infoOuHosInfo.DoctorId = infoBsDoctor.ID;// Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocId"));//bsdoctorid 医生id        
            infoOuHosInfo.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();// Convert.ToDateTime(GetXmlValue(Request, "req", "OPRegist", "Day"));  //挂号时发生的时间
            infoOuHosInfo.OperId = userId; //操作员id  自助挂号 的 userid
            infoOuHosInfo.RegDept =DeptId ; //挂号科室id  
            // infoOuHosInfo.DiagnDept = lstBsDocRegType[0].LocationId;//接诊科室
            if (amount > 1)
            {
                infoOuHosInfo.RegFee = 1;
                infoOuHosInfo.DiagnoFee = amount - 1;
            }

            if (infoOuHosInfo.PatTypeId == 472)
            {
                infoOuHosInfo.RegFee = 0;
                infoOuHosInfo.DiagnoFee = 0;
            }
            infoOuHosInfo.RegTime = BLL.Common.DateTimeHandler.GetServerDateTime(); //挂号时间
            
            infoOuHosInfo.F1 = "1";
            infoOuHosInfo.IsCancel = false;
            infoOuHosInfo.PaySel = 1;


            BLL.Common.CalBirthday objCalBirthday = new BLL.Common.CalBirthday();
            objCalBirthday.Birthday = infoBsPatient.BirthDate;
            objCalBirthday.NowDate = infoOuHosInfo.RegTime;
            infoOuHosInfo.Age = objCalBirthday.Age + (objCalBirthday.Months > 10 ? objCalBirthday.Months / 100 : objCalBirthday.Months / 10);
            infoOuHosInfo.AgeString = BLL.Common.Utils.CalAge(objCalBirthday);
            infoOuHosInfo.ContactPhone = GetNotEmptyString(infoBsPatient.Mobile, infoBsPatient.Phone, infoBsPatient.PhoneHome, infoBsPatient.PhoneWork);


            //int HasCount = 0;
            //int TolCount = 1;
            //bool isCanRegist = true;
            //if (Day.Date == BLL.Common.DateTimeHandler.GetServerDateTime().Date)
            //{
            //    TolCount = infoDocRegType.LimitNum;
            //    HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsCancel=0 and TimeSpanId={0}   and RegTime>=trunc(to_date('{1}','yyyy-mm-dd hh24:mi:ss')) and RegTime<trunc(to_date('{2}','yyyy-mm-dd hh24:mi:ss'))+1 and DoctorId={3})", infoBsRegTimeSpan.ID, Day.Date, Day.Date, infoBsDoctor.ID), "int"));

            //}
            //else
            //{

            //    TolCount = Convert.ToInt32(infoRegSpanSub.F1);
            //    HasCount = Convert.ToInt32(BLL.Common.Utils.GetColumnValue(" count(id) ", "OuHosInfo", string.Format(" 0 or ( IsCancel=0 and IsPreReg=1  and TimeSpanId={0}   and RegTime>=trunc(to_date('{1}','yyyy-mm-dd hh24:mi:ss')) and RegTime<trunc(to_date('{2}','yyyy-mm-dd hh24:mi:ss'))+1 and DoctorId={3} and TimeSpanSubId ={4} ) ", infoBsRegTimeSpan.ID, Day.Date, Day.Date, infoBsDoctor.ID, infoRegSpanSub.ID), "int"));
            //}
            //infoOuHosInfo.LineOrder = 0;
            //if (HasCount >= TolCount)
            //    isCanRegist = false;
            //if (!isCanRegist)
            //{
            //    trn.Rollback();
            //    trn.Dispose();
            //    Response = String.Format("<ResultCode>2</ResultCode><ErrorMsg>挂号失败，{0}</ErrorMsg>", "号满");
            //    Response = string.Format("<Response>{0}</Response>", Response);
            //    return Response;
            //}

            hospId = _objOuHosInfo.Create(infoOuHosInfo, trn);
            int nvoiceReID = 0;
            if (!infoOuHosInfo.IsPreReg)
            {
                Model.OulInvoiceRegInfo infoOuInvoiceRe = infoOuHosInfo.ConvertTo<Model.OulInvoiceRegInfo>();
                infoOuInvoiceRe.MzRegId = hospId;
                infoOuInvoiceRe.InvoTime = BLL.Common.DateTimeHandler.GetServerDateTime();
                infoOuInvoiceRe.InvoNo = infoOuHosInfo.MzRegNo;
                infoOuInvoiceRe.F4 = string.Empty;
                infoOuInvoiceRe.InvoLastId = 0;
                infoOuInvoiceRe.HospitalId = 31;
                infoOuInvoiceRe.LocationId = infoOuHosInfo.RegDept;
                BLL.COulInvoiceReg _objOuInvoiceReg = new BLL.COulInvoiceReg();
                nvoiceReID = infoOuInvoiceRe.ID = _objOuInvoiceReg.Create(infoOuInvoiceRe, trn);


                bkId = UpBankPatCardLog(infoBsPatient.ID, 2, "挂号扣减", userId, BNo, amount, trn, BCard, BMNo, BCode, nvoiceReID.ToString());
                // bkId = BLL.InsertAccount.UpPatCardFee(infoBsPatient.ID, 2, "挂号扣减", userId, hospId.ToString(), amount, trn);
            }


            trn.Commit();

            BLL.Common.Utils.UpdateColumn("ouhosinfo", string.Format(" BHosName ='{0}' ", BHosName), hospId, null);
            if (nvoiceReID > 0)
                BLL.Common.Utils.UpdateColumn("oulinvoicereg", string.Format(" BHosName ='{0}' ", BHosName), nvoiceReID, null);
        }
        catch (global::System.Exception ex)
        {
            trn.Rollback();
            trn.Dispose();
            Response = String.Format("<ResultCode>1</ResultCode><ErrorMsg>挂号失败，{0}</ErrorMsg>", ex.Message.ToString());
            return Response;
            //throw ex;
        }
        if (hospId > 0)
        {
            Model.OuHosInfoInfo infoOuhosinfo = _objOuHosInfo.GetByID(hospId);
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(infoOuhosinfo.RegDept);
            //Model.BsDocLevelInfo infoBsDocLevel = _objBsDocLevel.GetByID(infoBsDoctor.DocLevId); //医生职称

            //if (Day.Date != BLL.Common.DateTimeHandler.GetServerDateTime().Date)
            //{

            //    infoBsRegTimeSpan.TimeBegin = infoRegSpanSub.TimeBegin;
            //    infoBsRegTimeSpan.TimeEnd = infoRegSpanSub.TimeEnd;
            //}

            string XNo = string.Format("候诊排号：{0}", "分诊台报到排号");


            Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>挂号成功</ErrorMsg><SerID></SerID>" +
                "<PrintData> <CardNo>{0}</CardNo><PatName>{1}</PatName>" +
                "<DepDesc>{2}</DepDesc><RMB>{3}</RMB><UserId>{4}</UserId><SessionType>{5}</SessionType><LocInfo>{6}</LocInfo><DocDesc>{7}</DocDesc><DocTime>{8}</DocTime><XNo>{9}</XNo><MzRegNo>{10}</MzRegNo> <Outid>{11}</Outid> <intBkStoreLogID>{12}</intBkStoreLogID><newDate>{13}</newDate><DeptAdd>{14}</DeptAdd></PrintData>",
                infoOuhosinfo.CardNo, infoOuhosinfo.Name, infoBsLocation.Name, amount, userId, "", "", "", "", XNo, infoOuhosinfo.MzRegNo, hospId, bkId, newDate.ToString(), infoBsLocation.F3);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    public static int UpBankPatCardLog(int patid, int lsType, string strMemo, int userId, string BKRegNo, double Amount, System.Data.Common.DbTransaction trn, string strF1, string strF2, string strF3, string strF4)
    {
        if (Amount == 0) return 0;
        Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = new Model.ModelList<Model.PatCardFeeInfo>();
        Model.PatCardFeeInfo infoPatCardFee;

        if (lsType == 2) Amount = -Math.Abs(Amount);

        BLL.CPatCardFee objPatCardFee = new BLL.CPatCardFee();
        //BLL.CBKStoreLog objBKStoreLog = new BLL.CBKStoreLog();
        lstPatCardFee = objPatCardFee.PatCardFee_SelectByPatId(patid);
        if (lstPatCardFee.Count > 0)
            infoPatCardFee = lstPatCardFee[0];
        else
            infoPatCardFee = new Model.PatCardFeeInfo();

        infoPatCardFee.Amount = checked(infoPatCardFee.Amount + Amount);

        if (infoPatCardFee.ID > 0)
            objPatCardFee.Modify(infoPatCardFee, trn);
        else
        {
            infoPatCardFee.PatId = patid;
            //infoPatCardFee.Amount = checked(infoPatCardFee.Amount + Amount);
            infoPatCardFee.OperTime = BLL.Common.DateTimeHandler.GetServerDateTime();
            infoPatCardFee.OperId = userId;
            infoPatCardFee.ID = objPatCardFee.Create(infoPatCardFee, trn);
        }


        //Model.BKStoreLogInfo infoBKStoreLog = new Model.BKStoreLogInfo();
        //infoBKStoreLog.HappenTime = BLL.Common.DateTimeHandler.GetServerDateTime();
        //infoBKStoreLog.UserId = userId;
        //infoBKStoreLog.PatCardid = infoPatCardFee.ID;
        //infoBKStoreLog.BKRegNo = BKRegNo; //流水号
        //infoBKStoreLog.LsActType = lsType;  //充值(0扣费  1充值)
        //infoBKStoreLog.BeforeMoney = 0;
        //infoBKStoreLog.HappenMoney = Amount;  //流动金额；
        //infoBKStoreLog.AfterMoney = 0;
        //infoBKStoreLog.Memo = strMemo;
        //infoBKStoreLog.F1 = strF1;
        //infoBKStoreLog.F2 = strF2;
        //infoBKStoreLog.F3 = strF3;
        //infoBKStoreLog.F4 = strF4;

        int id = 0;// objBKStoreLog.Create(infoBKStoreLog, trn);
        return id;
    }

    private string GetNotEmptyString(string str1, string str2, string str3, string str4)
    {
        if (str1 != string.Empty)
            return str1;
        if (str2 != string.Empty)
            return str2;
        if (str3 != string.Empty)
            return str3;
        if (str4 != string.Empty)
            return str4;
        return string.Empty;
    }

    /// <summary>
    /// 查询病人预约号列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetYYRegistList(string Request)
    {
        String columns = "req,CardNo,Day";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        DateTime dt = Convert.ToDateTime(GetXmlValue(Request, "req", "Day", ""));

        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();

        string sql = string.Format(" CardNo='{0}' and IsCancel=0 and IsPreReg=1  and  RegTime >='{1}'  and not exists(select id from OulInvoiceReg where mzregid=ouhosinfo.id  and iscancel=0 ) and (TimeSpanSubId>{2} or RegTime >'{1}')",
            GetXmlValue(Request, "req", "CardNo", ""), dt.ToString("yyyy-MM-dd"), GetTimeSpanSub());


        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" CardNo='{0}' and IsCancel=0 and IsPreReg=1  and  RegTime >='{1}'  and not exists(select id from OulInvoiceReg where mzregid=ouhosinfo.id  and iscancel=0 ) and (TimeSpanSubId>{2} or RegTime >'{1}')", GetXmlValue(Request, "req", "CardNo", ""), dt.ToString("yyyy-MM-dd"), GetTimeSpanSub()), "RegTime DESC");

        string CardNo = GetXmlValue(Request, "req", "CardNo", "");
        bool isVIP = false;

        Model.ModelList<Model.BsPatientInfo> lst = _objBsPatient.BsPatient_SelectByCardNo(CardNo);
        if (lst.Count == 1 && lst[0].PatTypeId == 472)
        {
            isVIP = true;
        }


        if (ouhosLst.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有挂号记录！</ErrorMsg></Response>";

        Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        foreach (Model.OuHosInfoInfo ouhosinfo in ouhosLst)
        {
            Model.ModelList<Model.BsDocRegTypeInfo> lstBsDocRegType = _objBsDocRegType.GetDynamic(string.Format(" DoctorId={0} and  WeekDay = {1} and TimeSpanId ={2}", ouhosinfo.DoctorId, GetWeek(ouhosinfo.RegTime), ouhosinfo.TimeSpanId), "WeekDay");
            Model.BsDocRegTypeInfo infoBsDocRegType = new Model.BsDocRegTypeInfo();
            if (lstBsDocRegType.Count > 0)
            {
                infoBsDocRegType = lstBsDocRegType[0];
            }
            //挂号类别
            Model.BsRegTimeSpanInfo infoBsRegTimeSpan = _objBsRegTimeSpan.GetByID(Convert.ToInt32(ouhosinfo.TimeSpanId)); //排班类型
            //Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(infoBsDocRegType.DoctorId); //医生

            Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(ouhosinfo.DoctorId); //医生
            Model.BsDocLevelInfo infoBsDocLevel = _objBsDocLevel.GetByID(infoBsDoctor.DocLevId); //医生职称
            Model.BsRegTypeInfo infoBsRegType = _objBsRegType.GetByID(infoBsDoctor.RegTypeId);
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(ouhosinfo.RegDept);
            Model.BsRegSpanSubInfo infoBsRegSpanSub = _objBsRegSpanSub.GetByID(Convert.ToInt32(ouhosinfo.TimeSpanSubId));

            Response += string.Format("<RbInfo><RegisterDate>{0}</RegisterDate><MzRegNo>{1}</MzRegNo><VisitID>{2}</VisitID><IsDiagnoses>{3}</IsDiagnoses><PatName>{4}</PatName><DepName>{5}</DepName><DoctName>{6}</DoctName><opertime>{7}</opertime><timespar>{8}</timespar><yyhb>{9}</yyhb><RMB>{10}</RMB><DocRegID>{11}</DocRegID><timesName>{12}</timesName><room>{13}</room><yxtime>{14}</yxtime></RbInfo>"
                , ouhosinfo.RegTime.ToString(), ouhosinfo.MzRegNo, ouhosinfo.ID, ouhosinfo.DiagnDept == 0 ? "N" : "Y", ouhosinfo.Name, infoBsLocation.Name, infoBsDoctor.Name, ouhosinfo.OperTime.ToString(), infoBsRegSpanSub.TimeBegin + "-" + infoBsRegSpanSub.TimeEnd, infoBsRegType.Name, isVIP == true ? 0 : ouhosinfo.DiagnoFee + ouhosinfo.RegFee, infoBsDocRegType.ID, infoBsRegTimeSpan.Name, infoBsLocation.Name, infoBsRegSpanSub.TimeBegin);

        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 预约取票
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string OPYYRegist(string Request)
    {
        //String columns = "req,CardNo,VisitID,UserId";
        String columns = "req,OPRegist,UserId,DocRegID,VisitID,CardNo";
        //if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        //{
        //    return GetExist(Request, columns);
        //}

        string BCard = GetXmlValue(Request, "req", "OPRegist", "BCard");
        string BAmount = GetXmlValue(Request, "req", "OPRegist", "BAmount");
        string BDate = GetXmlValue(Request, "req", "OPRegist", "BDate");
        string BTime = GetXmlValue(Request, "req", "OPRegist", "BTime");
        string BNo = GetXmlValue(Request, "req", "OPRegist", "BNo");
        string BMNo = GetXmlValue(Request, "req", "OPRegist", "BMNo");
        string BCode = GetXmlValue(Request, "req", "OPRegist", "BCode");
        string BType = GetXmlValue(Request, "req", "OPRegist", "BType");
        string BHosName = GetXmlValue(Request, "req", "OPRegist", "BHosName");
        string CardNo = GetXmlValue(Request, "req", "CardNo", "");
        bool isVIP = false;

        Model.ModelList<Model.BsPatientInfo> lst = _objBsPatient.BsPatient_SelectByCardNo(CardNo);
        if (lst.Count == 1 && lst[0].PatTypeId == 472)
        {
            isVIP = true;
        }
        
        string Response = string.Empty;
        int userId = GetUserID(GetXmlValue(Request, "req", "OPRegist", "UserId"));
        int DocRegID = Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "DocRegID"));
        Model.OuHosInfoInfo infoOuHosInfo;
        if (GetXmlValue(Request, "req", "OPRegist", "VisitID").Trim() != string.Empty)
            infoOuHosInfo = _objOuHosInfo.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "OPRegist", "VisitID")));
        else
            infoOuHosInfo = GetOuHosInfo(GetXmlValue(Request, "req", "OPRegist", "CardNo"));

        Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(infoOuHosInfo.DoctorId); //查询医生表


        DateTime newDate = BLL.Common.DateTimeHandler.GetServerDateTime();
        int bkId = 0;
        DAL.SqlUtil db = new DAL.SqlUtil();
        System.Data.Common.DbTransaction trn = db.GetSqlTransaction();
        try
        {

            if (infoOuHosInfo.IsPreReg)
            {
                if (isVIP == true)
                {
                    infoOuHosInfo.RegFee = 0;
                    infoOuHosInfo.DiagnoFee = 0;
                }
                infoOuHosInfo.F8 = "1";
                infoOuHosInfo.RegTime = newDate;
                infoOuHosInfo.MzRegNo = BLL.Common.SequenceNumHandler.GetSequenceNum(Model.EnumSequenceNumType.MzReg).ToString();

                Model.OulInvoiceRegInfo infoOuInvoiceRe = infoOuHosInfo.ConvertTo<Model.OulInvoiceRegInfo>();
                infoOuInvoiceRe.MzRegId = infoOuHosInfo.ID;
                infoOuInvoiceRe.InvoTime = newDate;
                infoOuInvoiceRe.InvoNo = infoOuHosInfo.MzRegNo;
                infoOuInvoiceRe.HospitalId = 31;
                infoOuInvoiceRe.F4 = string.Empty;
                infoOuInvoiceRe.InvoLastId = 0;
                infoOuInvoiceRe.OperId = userId;
                infoOuInvoiceRe.LocationId = infoOuHosInfo.RegDept;
                BLL.COulInvoiceReg _objOuInvoiceReg = new BLL.COulInvoiceReg();
                infoOuInvoiceRe.ID = _objOuInvoiceReg.Create(infoOuInvoiceRe, trn);

                _objOuHosInfo.Modify(infoOuHosInfo, trn);
                bkId = UpBankPatCardLog(infoOuHosInfo.PatId, 2, "挂号扣减", userId, BNo, infoOuHosInfo.RegFee + infoOuHosInfo.DiagnoFee, trn, BCard, BMNo, BCode, infoOuInvoiceRe.ID.ToString());
                //bkId = BLL.InsertAccount.UpPatCardFee(infoOuHosInfo.PatId, 2, "挂号扣减", userId, infoOuHosInfo.ID.ToString(), infoOuHosInfo.RegFee + infoOuHosInfo.DiagnoFee, trn);
                trn.Commit();

           //     new Utilities.Document().SaveLog(string.Format("\r\n\r\n   {0}保存OPYYRegist(预约)取票下单：{1}", BLL.Common.DateTimeHandler.GetServerDateTime().ToString(), req), string.Format("C:\\"  + BLL.Common.DateTimeHandler.GetServerDateTime().ToString("yyyy-MM-dd")));

                BLL.Common.Utils.UpdateColumn("oulinvoicereg", string.Format(" BHosName ='{0}' ", BHosName), infoOuInvoiceRe.ID, null);
            }
            else Response = String.Format("<ResultCode>1</ResultCode><ErrorMsg>不是有效的预约号</ErrorMsg>");
        }
        catch (global::System.Exception ex)
        {
            trn.Rollback();
            trn.Dispose();
            Response = String.Format("<ResultCode>1</ResultCode><ErrorMsg>取票失败{0}</ErrorMsg>", ex.Message.ToString());
            return Response;
            //throw ex;
        }
        if (bkId > 0)
        {

            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(infoOuHosInfo.RegDept);
            Model.BsDocLevelInfo infoBsDocLevel = _objBsDocLevel.GetByID(infoBsDoctor.DocLevId); //医生职称
            Model.BsDocRegTypeInfo infoDocRegType = _objBsDocRegType.GetByID(DocRegID);
            Model.BsRegTimeSpanInfo infoBsRegTimeSpan = _objBsRegTimeSpan.GetByID(Convert.ToInt32(infoOuHosInfo.TimeSpanId));
            Model.BsRegSpanSubInfo infoBsRegSpanSub = _objBsRegSpanSub.GetByID(Convert.ToInt32(infoOuHosInfo.TimeSpanSubId));

            string XNo = string.Format("候诊排号：{0}", infoOuHosInfo.LineOrder);

            Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>挂号成功</ErrorMsg><SerID></SerID>" +
                "<PrintData> <CardNo>{0}</CardNo><PatName>{1}</PatName>" +
                "<DepDesc>{2}</DepDesc><RMB>{3}</RMB><UserId>{4}</UserId><SessionType>{5}</SessionType><LocInfo>{6}</LocInfo><DocDesc>{7}</DocDesc><DocTime>{8}</DocTime><XNo>{9}</XNo><MzRegNo>{10}</MzRegNo> <Outid>{11}</Outid><intBkStoreLogID>{12}</intBkStoreLogID><newDate>{13}</newDate><timeName>{14}</timeName></PrintData>",
                infoOuHosInfo.CardNo, infoOuHosInfo.Name, infoBsLocation.Name, infoOuHosInfo.RegFee + infoOuHosInfo.DiagnoFee, userId, infoBsDocLevel.Name, infoDocRegType.F4, infoBsDoctor.Name, infoBsRegSpanSub.TimeBegin + "-" + infoBsRegSpanSub.TimeEnd, XNo, infoOuHosInfo.MzRegNo, infoOuHosInfo.ID, bkId, newDate.ToString(), infoBsRegTimeSpan.Name);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 查询缴费项目
    /// </summary>
    /// <returns></returns>
    //[WebMethod]
    //public string GetBillInfo(string Request)
    //{
    //    String columns = "req,CardNo";
    //    if (!String.IsNullOrEmpty(GetExist(Request, columns)))
    //    {
    //        return GetExist(Request, columns);
    //    }

    //    string Response = string.Empty;
    //    BLL.Finder<Model.uspOuRecipeDtlForOuInvoiceDtlQry> objbllFinder = new BLL.Finder<Model.uspOuRecipeDtlForOuInvoiceDtlQry>();
    //    Model.OuHosInfoInfo infoOuHos=GetOuHosInfo(GetXmlValue(Request, "req", "CardNo", ""));
    //    objbllFinder.AddParameter("MzRegId", infoOuHos.ID);
    //    objbllFinder.AddParameter("LocationId", infoOuHos.DiagnDept);
    //    objbllFinder.AddParameter("PatTypeId", 116);
    //    Model.ModelList<Model.uspOuRecipeDtlForOuInvoiceDtlQry> lstUspOuChargeDetail = objbllFinder.Find("uspOuRecipeDtlForOuInvoiceDtl");


    //    Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg><Items>";
    //    foreach (Model.uspOuRecipeDtlForOuInvoiceDtlQry infoUspOuCargeDetail in lstUspOuChargeDetail)
    //    {
    //        // <Item><ItemId>101</ItemId><ItemName>体检费</ItemName><CateId>1</CateId><CateName>西药</CateName><Price>1</Price><Num>2</Num><CtLoc>神经一科</CtLoc></Item></Items></Response>

    //        Response += string.Format("<Item><ItemId>{0}</ItemId><ItemName>{1}</ItemName><CateId>{2}</CateId><CateName>{3}</CateName><Price>{4}</Price><Num>{5}</Num><Amount>{6}</Amount><CtLoc>{7}</CtLoc></Item>",
    //            infoUspOuCargeDetail.Code, infoUspOuCargeDetail.Name, infoUspOuCargeDetail.InvItemId, infoUspOuCargeDetail.InvMzItemName, infoUspOuCargeDetail.Price, infoUspOuCargeDetail.Totality, infoUspOuCargeDetail.Amount, infoUspOuCargeDetail.Memo);
    //    }
    //    Response += "</Items>";

    //    return Response = string.Format("<Response>{0}</Response>", Response);
    //}

    /// <summary>
    /// 查询当天挂号列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetRegistList(string Request)
    {

        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "GetRegistList");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");


        String columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;

        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" CardNo='{0}'  and RegTime >= convert(nvarchar,getdate(),23) and exists(select id from OulInvoiceReg where mzregid=ouhosinfo.id  and iscancel=0 )  ORDER BY RegTime DESC  ", GetXmlValue(Request, "req", "CardNo", "")), null);


        if (ouhosLst.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有挂号记录！</ErrorMsg></Response>";

        Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        foreach (Model.OuHosInfoInfo ouhosinfo in ouhosLst)
        {
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(ouhosinfo.RegDept);
            Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(ouhosinfo.DoctorId); //医生
            Response += string.Format("<RbInfo><RegisterDate>{0}</RegisterDate><VisitNO>{1}</VisitNO><VisitID>{2}</VisitID><Status>{3}</Status><PatName>{4}</PatName><DepName>{5}</DepName><DoctName>{6}</DoctName><Room>{7}</Room></RbInfo>"
                , ouhosinfo.RegTime.ToString(), ouhosinfo.MzRegNo, ouhosinfo.ID, ouhosinfo.DiagnDept == 0 ? "N" : "Y", ouhosinfo.Name, infoBsLocation.Name, infoBsDoctor.Name, infoBsLocation.Name);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 查询医生当天挂号列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetRegistListDoc(string Request)
    {

        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "GetRegistListDoc");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");


        String columns = "req,DocId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;

        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" DOCTORID='{0}'  and RegTime >= convert(nvarchar,getdate(),23) and exists(select id from OulInvoiceReg where mzregid=ouhosinfo.id  and iscancel=0 ) and not exists(select id from ourecipe where mzregid = ouhosinfo.id) ORDER BY RegTime DESC  ", GetXmlValue(Request, "req", "DocId", "")), null);


        if (ouhosLst.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有挂号记录！</ErrorMsg></Response>";

        Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        foreach (Model.OuHosInfoInfo ouhosinfo in ouhosLst)
        {
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(ouhosinfo.RegDept);
            Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(ouhosinfo.DoctorId); //医生
            Response += string.Format("<RbInfo><RegisterDate>{0}</RegisterDate><VisitNO>{1}</VisitNO><VisitID>{2}</VisitID><Status>{3}</Status><PatName>{4}</PatName><DepName>{5}</DepName><DoctName>{6}</DoctName><Room>{7}</Room></RbInfo>"
                , ouhosinfo.RegTime.ToString(), ouhosinfo.MzRegNo, ouhosinfo.ID, ouhosinfo.DiagnDept == 0 ? "N" : "Y", ouhosinfo.Name, infoBsLocation.Name, infoBsDoctor.Name, infoBsLocation.Name);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }



    /// <summary>
    /// 查询病人挂号列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetRegistListTop(string Request)
    {
        String columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;

        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" id in ( select id from (select id from OuHosInfo where  cardno='{0}' and exists(select id from OulInvoiceReg where mzregid=ouhosinfo.id  and iscancel=0 ) order by RegTime DESC ) where rownum <=30 )  ", GetXmlValue(Request, "req", "CardNo", "")), "RegTime DESC");


        if (ouhosLst.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有挂号记录！</ErrorMsg></Response>";

        Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        foreach (Model.OuHosInfoInfo ouhosinfo in ouhosLst)
        {
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(ouhosinfo.RegDept);
            Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(ouhosinfo.DoctorId); //医生
            Response += string.Format("<RbInfo><RegisterDate>{0}</RegisterDate><VisitNO>{1}</VisitNO><VisitID>{2}</VisitID><IsDiagnoses>{3}</IsDiagnoses><PatName>{4}</PatName><DepName>{5}</DepName><DoctName>{6}</DoctName></RbInfo>"
                , ouhosinfo.RegTime.ToString(), ouhosinfo.MzRegNo, ouhosinfo.ID, ouhosinfo.DiagnDept == 0 ? "N" : "Y", ouhosinfo.Name, infoBsLocation.Name, infoBsDoctor.Name);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }

     

    /// <summary>
    /// 查询缴费项目
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetBillInfo(string Request)
    {
    //    string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "GetBillInfo");
    //    Utilities.Document log = new Utilities.Document();
    //    log.SaveLog(tip, "C:\\WXJKRC.log");

    //    return "Hello!版本日期" + BLL.Common.DateTimeHandler.GetServerDateTime().ToString();

       string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "GetBillInfo");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        String columns = "req,CardNo,VisitID";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;

        Model.OuHosInfoInfo infoOuHos;
        if (GetXmlValue(Request, "req", "VisitID", "").Trim() != string.Empty)
            infoOuHos = _objOuHosInfo.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "VisitID", "")));
        else
            infoOuHos = GetOuHosInfo(GetXmlValue(Request, "req", "CardNo", ""));

        if (infoOuHos == null) return "<Response><ResultCode>1</ResultCode><ErrorMsg>当天没有挂号记录！</ErrorMsg></Response>";

        BLL.COuInvoiceDtlTemp OuInvoiceDtlTemp = new BLL.COuInvoiceDtlTemp();
        Model.ModelList<Model.OuInvoiceDtlTempInfo> lstOuInvoiceDtlTempInfo = OuInvoiceDtlTemp.OuInvoiceDtlTemp_SelectByMzregid(infoOuHos.ID);
        Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = lstOuInvoiceDtlTempInfo.ConvertTo<Model.uspOuInvoiceDtlQry>();//  BLL.SockIOPoolComm.GetPoolDataList<Model.uspOuInvoiceDtlQry>(string.Format("FrmOuCharge_RecipeDtl_{0}", infoOuHos.ID));

        if (lstUspOuChargeDetail == null || lstUspOuChargeDetail.Count == 0) return "<Response><ResultCode>2</ResultCode><ErrorMsg>没有缴费记录！</ErrorMsg></Response>";
        //XYHIS.FrmOuCharge.IsAutoCharge = true;
        //XYHIS.FrmOuCharge oucharge = new XYHIS.FrmOuCharge();
        //Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = oucharge.GetBillInfo(infoOuHos.ID);


        Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg><VisitID>{0}</VisitID><Items>", infoOuHos.ID);
        foreach (Model.uspOuInvoiceDtlQry infoUspOuCargeDetail in lstUspOuChargeDetail)
        {
            // <Item><ItemId>101</ItemId><ItemName>体检费</ItemName><CateId>1</CateId><CateName>西药</CateName><Price>1</Price><Num>2</Num><CtLoc>神经一科</CtLoc></Item></Items></Response>

            Response += string.Format("<Item><ItemId>{0}</ItemId><ItemName>{1}</ItemName><CateId>{2}</CateId><CateName>{3}</CateName><Price>{4}</Price><Num>{5}</Num><Amount>{6}</Amount><CtLoc>{7}</CtLoc><GG>{8}</GG></Item>",
                infoUspOuCargeDetail.Code, infoUspOuCargeDetail.Name, infoUspOuCargeDetail.FeeId, infoUspOuCargeDetail.FeeName, infoUspOuCargeDetail.Price, infoUspOuCargeDetail.Totality, infoUspOuCargeDetail.Amount, infoUspOuCargeDetail.Memo, infoUspOuCargeDetail.Spec);
        }
        Response += "</Items>";

        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 执行缴费
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string AutoOPBillCharge(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "AutoOPBillCharge");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string Response = string.Empty;
        string mzRegNo = GetXmlValue(Request, "req", "VisitNO", "");  //流水号 
        //int locationId = Convert.ToInt32(GetXmlValue(Request, "req", "DeptId", "")); //科室id
        //int doctorId = Convert.ToInt32(GetXmlValue(Request, "req", "RowId", ""));//bsdoctorid 医生id    
        int userId = GetUserID(GetXmlValue(Request, "req", "UserId", ""));
        // int VisitID = GetUserID(GetXmlValue(Request, "req", "OPRegist", "VisitID"));


        string BCard = GetXmlValue(Request, "req", "BCard", "");
        string BAmount = GetXmlValue(Request, "req", "BAmount", "");
        string BDate = GetXmlValue(Request, "req", "BDate", "");
        string BTime = GetXmlValue(Request, "req", "BTime", "");
        string BNo = GetXmlValue(Request, "req", "BNo", "");
        string BMNo = GetXmlValue(Request, "req", "BMNo", "");
        string BCode = GetXmlValue(Request, "req", "BCode", "");
        string BType = GetXmlValue(Request, "req", "BType", "");
        string BHosName = GetXmlValue(Request, "req", "BHosName", "");
        string amount = GetXmlValue(Request, "req", "Amount", "");
        

        Model.OuHosInfoInfo infoOuHos;
        if (GetXmlValue(Request, "req", "VisitID", "").Trim() != string.Empty)
            infoOuHos = _objOuHosInfo.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "VisitID", "")));
        else
            infoOuHos = GetOuHosInfo(GetXmlValue(Request, "req", "CardNo", ""));


        if (infoOuHos == null || infoOuHos.ID == 0)
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>该流水号不是有效挂号信息</ErrorMsg>", Response);
    
        //XYHIS.FrmOuChargeWebServer frmouchar=null;
        //int bkId = 0;
        //try
        //{
        //    frmouchar = new XYHIS.FrmOuChargeWebServer();
        //    frmouchar.IsAutoCharge = true;
        //    Response = frmouchar.AutoRegOuHosInfo(infoOuHos.ID, userId);
        //    if (!Response.Contains("成功"))
        //        return string.Format("<ResultCode>1</ResultCode><ErrorMsg>{0}</ErrorMsg>", Response);
        //    bkId = UpBankPatCardLog(infoOuHos.PatId, 2, "收费扣减", userId, BNo, Convert.ToDouble(amount), null, BCard, BMNo, BCode, frmouchar._infoOuInvoice.ID.ToString());

        //    BLL.Common.Utils.UpdateColumn("OuInvoice", string.Format(" BHosName ='{0}' ", BHosName), frmouchar._infoOuInvoice.ID, null);

        //    XYHIS.FrmOuChargeWebServer frm = new XYHIS.FrmOuChargeWebServer();
        //    frm.GetAutoRegOuHosInfo(infoOuHos.ID);
        //}
        //catch (Exception ex)
        //{
        //    new Utilities.Document().SaveLog("\r\n " + ex.InnerException.Message + ex.Message + ex.Source + ex.StackTrace, "C:\\Log.log");
        //}

        //if (Response.Contains("成功"))
        //{
        //    BLL.Finder<Model.uspOuInvoiceDtlQry> objbllFinder = new BLL.Finder<Model.uspOuInvoiceDtlQry>();

        //    Model.OuInvoiceInfo lstOuInvoice = frmouchar._infoOuInvoice;


        //    //objbllFinder.AddParameter("InvoId", lstOuInvoice.ID);
        //    //Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = objbllFinder.Find("uspOuInvoiceDtl");

        //    Model.ModelList<Model.uspOuInvoiceInvItemGoupSumQry> _lstUspOuInvoiceInvItemGoupSumQry = new Model.ModelList<Model.uspOuInvoiceInvItemGoupSumQry>();
        //    BLL.Finder<Model.uspOuInvoiceInvItemGoupSumQry> _objUspOuInvoiceInvItemGroupSum = new BLL.Finder<Model.uspOuInvoiceInvItemGoupSumQry>();
        //    _objUspOuInvoiceInvItemGroupSum.AddParameter("InvoId", lstOuInvoice.ID);
        //    _lstUspOuInvoiceInvItemGoupSumQry = _objUspOuInvoiceInvItemGroupSum.Find("uspOuInvoiceInvItemGoupSum");

        //    Model.ModelList<Model.uspOuInvoiceFeetyGoupSumQry> _lstUspOuInvoiceFeetyGoupSumQry = new Model.ModelList<Model.uspOuInvoiceFeetyGoupSumQry>();
        //    BLL.Finder<Model.uspOuInvoiceFeetyGoupSumQry> _objUspOuInvoiceFeetyGoupSum = new BLL.Finder<Model.uspOuInvoiceFeetyGoupSumQry>();
        //    _objUspOuInvoiceFeetyGoupSum.AddParameter("InvoId", lstOuInvoice.ID);
        //    _lstUspOuInvoiceFeetyGoupSumQry = _objUspOuInvoiceFeetyGoupSum.Find("uspOuInvoiceFeetyGoupSum");

        //    Response = string.Format("<ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg><AutoOPBillCharge><SumValue>{0}</SumValue><MzRegNo>{1}</MzRegNo>"//
        //              , (lstOuInvoice.Beprice + lstOuInvoice.AddFee).ToString(), infoOuHos.MzRegNo);//, 
        //    Response += string.Format("<FPH>{2}</FPH><ZJE>{0}</ZJE><YHJE>0.00</YHJE><FYCK>{1}</FYCK><BKID>{3}</BKID><FPXM>"
        //        , (lstOuInvoice.Beprice + lstOuInvoice.AddFee).ToString(), frmouchar.strMZInjectRoomText, lstOuInvoice.InvoNo, bkId);


        //    foreach (Model.uspOuInvoiceFeetyGoupSumQry info in _lstUspOuInvoiceFeetyGoupSumQry)
        //    {
        //        if (info.Name.Contains("中") && info.Name.Contains("药"))
        //            Response += string.Format("<XM><XMMC>{0}</XMMC><JE>{1}</JE><Memo>{2}</Memo></XM>", info.Name, info.Amount, frmouchar.strChianRoomText);
        //        else if (info.Name.Contains("药"))
        //            Response += string.Format("<XM><XMMC>{0}</XMMC><JE>{1}</JE><Memo>{2}</Memo></XM>", info.Name, info.Amount, frmouchar.strMZInjectRoomText);
        //        else Response += string.Format("<XM><XMMC>{0}</XMMC><JE>{1}</JE><Memo>{2}</Memo></XM>", info.Name, info.Amount, "");
        //    }

        //    //Response += "</FPXM><SFXMMX>";
        //    //foreach (Model.uspOuInvoiceDtlQry infoUspOuCargeDetail in lstUspOuChargeDetail)
        //    //{
        //    //    Response += string.Format("<SFXM><XMMC>{0}</XMMC><DW>{1}</DW><DJ>{2}</DJ><SL>{3}</SL><JE>{4}</JE></SFXM>",
        //    //        infoUspOuCargeDetail.Name, infoUspOuCargeDetail.UnitDiagName, infoUspOuCargeDetail.Price, infoUspOuCargeDetail.Totality,
        //    //        infoUspOuCargeDetail.Amount);
        //    //}
        //    //Response += "</SFXMMX></FP></FPList></AutoOPBillCharge>";
        //    Response += "</FPXM></AutoOPBillCharge>";
        //}
        //else
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>{0}</ErrorMsg>", Response);
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 消费记录前30
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string ConsumptionRecordTop(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "ConsumptionRecordTop");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string columns = "req,Patid";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "Patid", "");
        try
        {
            Model.BsPatientInfo lstBsPatient = _objBsPatient.GetByID(Convert.ToInt32( cardNo)); //卡号查询
            Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient.ID);
            Model.ModelList<Model.BKStoreLogInfo> lstBKStoreLog = new Model.ModelList<Model.BKStoreLogInfo>();
            if (lstPatCardFee.Count > 0)
                lstBKStoreLog = _objBKStoreLog.GetDynamic(string.Format(" id in ( select id from  (select id from BKStoreLog  where patcardid={0}  order by HappenTime desc   ) where rownum <=30)  ", lstPatCardFee[0].ID), " HappenTime desc ");
            if (lstBsPatient != null && lstBsPatient.ID>0 && lstBKStoreLog.Count > 0)
            {
                Model.BsPatientInfo infoBsPatient = lstBsPatient;
                Response = "<ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg>";
                foreach (Model.BKStoreLogInfo info in lstBKStoreLog)
                {
                     if( (info.Memo.Contains("充值")))
                         Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><RecordCode>1</RecordCode></Record>",
                        "1", info.HappenMoney, info.HappenTime, "充值", info.Memo);
                    else if (info.Memo.Contains("退款"))
                         Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><RecordCode>2</RecordCode></Record> ",
                               "C", -info.HappenMoney, info.HappenTime, "退款", info.Memo);
                     else
                         Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><RecordCode>3</RecordCode></Record> ",
                            "A", -info.HappenMoney, info.HappenTime, "消费", info.Memo);                   
                }
            }
            else
            {
                Response = "<ResultCode>1</ResultCode><ErrorMsg>没有消费记录</ErrorMsg>";
            }
        }
        catch (Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>消费记录查询失败，请联系系统管理员：{0}</ErrorMsg>", ex.Message);
            //throw ex;
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 消费记录
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string ConsumptionRecord(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "ConsumptionRecord");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        string columns = "req,Patid";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "Patid", "");
        try
        {
            Model.BsPatientInfo lstBsPatient = _objBsPatient.GetByID(Convert.ToInt32(cardNo)); //卡号查询
            Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient.ID);
            Model.ModelList<Model.BKStoreLogInfo> lstBKStoreLog = new Model.ModelList<Model.BKStoreLogInfo>();
            if (lstPatCardFee.Count > 0)
                lstBKStoreLog = _objBKStoreLog.GetDynamic(string.Format("  patcardid={0}   ", lstPatCardFee[0].ID), " HappenTime desc ");
            if (lstBsPatient != null && lstBsPatient.ID > 0 && lstBKStoreLog.Count > 0)
            {
                Model.BsPatientInfo infoBsPatient = lstBsPatient;
                Response = "<ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg>";
                foreach (Model.BKStoreLogInfo info in lstBKStoreLog)
                {
                    if ((info.Memo.Contains("充值")))
                        Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><BkId>{5}</BkId><BkNo>{6}</BkNo><UserId>{7}</UserId><RecordCode>1</RecordCode></Record>",
                       "1", info.HappenMoney, info.HappenTime, "充值", info.Memo, info.ID, info.BKRegNo, _objUserInfo.GetByID(info.UserId).Code);
                    else if (info.Memo.Contains("退款"))
                        Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><BkId>{5}</BkId><BkNo>{6}</BkNo><UserId>{7}</UserId><RecordCode>2</RecordCode></Record> ",
                              "C", -info.HappenMoney, info.HappenTime, "退款", info.Memo, info.ID, info.BKRegNo, _objUserInfo.GetByID(info.UserId).Code);
                    else
                        Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><BkId>{5}</BkId><BkNo>{6}</BkNo><UserId>{7}</UserId><RecordCode>3</RecordCode></Record> ",
                           "A", -info.HappenMoney, info.HappenTime, "消费", info.Memo, info.ID, info.BKRegNo, _objUserInfo.GetByID(info.UserId).Code);
                }
            }
            else
            {
                Response = "<ResultCode>1</ResultCode><ErrorMsg>没有消费记录</ErrorMsg>";
            }
        }
        catch (Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>消费记录查询失败，请联系系统管理员：{0}</ErrorMsg>", ex.Message);
            //throw ex;
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 充值记录
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string AddBalanceRecord(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "AddBalanceRecord");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");
        string columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        string cardNo = GetXmlValue(Request, "req", "CardNo", "");
        try
        {
            Model.ModelList<Model.BsPatientInfo> lstBsPatient = _objBsPatient.BsPatient_SelectByCardNo(cardNo); //卡号查询
            Model.ModelList<Model.PatCardFeeInfo> lstPatCardFee = _objPatCardFee.PatCardFee_SelectByPatId(lstBsPatient[0].ID);
            Model.ModelList<Model.BKStoreLogInfo> lstBKStoreLog = new Model.ModelList<Model.BKStoreLogInfo>();
            if (lstPatCardFee.Count > 0)
                lstBKStoreLog = _objBKStoreLog.GetDynamic(string.Format(" LsActType=1 and  PatCardid={0}", lstPatCardFee[0].ID), "HappenTime");
            if (lstBsPatient.Count > 0 && lstBKStoreLog.Count > 0)
            {
                Model.BsPatientInfo infoBsPatient = lstBsPatient[0];
                Response = "<ResultCode>0</ResultCode><ErrorMsg>成功</ErrorMsg>";
                foreach (Model.BKStoreLogInfo info in lstBKStoreLog)
                {
                    Response += string.Format("<Record><RecordType>{0}</RecordType><Money>{1}</Money><RecordDate>{2}</RecordDate><RecordDesc>{3}</RecordDesc><Remark>{4}</Remark><BankNo>{5}</BankNo></Record>",
                        "1", info.HappenMoney, info.HappenTime, "充值", info.Memo, info.Memo.Contains("现金充值") ? "" : info.Memo);
                }
            }
            else
            {
                Response = "<ResultCode>1</ResultCode><ErrorMsg>没有消费记录</ErrorMsg>";
            }
        }
        catch (Exception ex)
        {
            Response = string.Format("<ResultCode>1</ResultCode><ErrorMsg>消费记录查询失败，请联系系统管理员：{0}</ErrorMsg>", ex.Message);
            //throw ex;
        }
        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 获取检验列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetLisItems(string Request)
    {
        String columns = "req,StartDate,EndDate,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }


        string Response = string.Empty;
        Model.ModelList<Model.uspCkLabSearchQry> _lstCkLabSearchQry;
        BLL.Finder<Model.uspCkLabSearchQry> finder = new BLL.Finder<Model.uspCkLabSearchQry>();
        finder.AddParameter("Time_Begin", GetXmlValue(Request, "req", "StartDate", ""));
        finder.AddParameter("Time_End", GetXmlValue(Request, "req", "EndDate", ""));
        finder.AddParameter("CardNo", GetXmlValue(Request, "req", "CardNo", ""));
        _lstCkLabSearchQry = finder.Find("uspAutoCkLabSearch");
        if (_lstCkLabSearchQry.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有检验记录！</ErrorMsg></Response>";

        Response = "<LisItemList>";
        foreach (Model.uspCkLabSearchQry infoCkLabSearch in _lstCkLabSearchQry)
        {

            Response += string.Format("<Items><ApplyNo>{0}</ApplyNo><SendTime>{1}</SendTime><RepName>{2}</RepName><RepCate>{3}</RepCate><RepCateName>{4}</RepCateName><Status>{5}</Status><LabId>{6}</LabId><MzRegNo>{7}</MzRegNo></Items>"
                , infoCkLabSearch.ID.ToString(), infoCkLabSearch.OperTime.ToString(), infoCkLabSearch.TestName, infoCkLabSearch.TestTypeId, infoCkLabSearch.DoctorName, infoCkLabSearch.LsStatus.ToString(), infoCkLabSearch.ID, infoCkLabSearch.MzRegNo.ToString());
        }
        Response += "</LisItemList> <ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 查询已缴费病人挂号列表
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string GetRegistListZLD(string Request)
    {
        string tip = string.Format("\r\n时间[{0}],方法{2}：{1}\r\n", BLL.Common.DateTimeHandler.GetServerDateTime(), Request, "GetRegistListZLD");
        Utilities.Document log = new Utilities.Document();
        log.SaveLog(tip, "C:\\WXJKRC.log");

        String columns = "req,CardNo";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;

        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format("  id in ( select id from (select top 30 id from OuHosInfo where  cardno='{0}' and exists(select id from ouinvoice where mzregid=ouhosinfo.id  and iscancel=0 ) order by RegTime DESC ) a )   ", GetXmlValue(Request, "req", "CardNo", "")), "RegTime DESC");


        if (ouhosLst.Count == 0) return "<Response><ResultCode>1</ResultCode><ErrorMsg>没有挂号记录！</ErrorMsg></Response>";

        Response = "<ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        foreach (Model.OuHosInfoInfo ouhosinfo in ouhosLst)
        {
            Model.BsLocationInfo infoBsLocation = _objBsLocation.GetByID(ouhosinfo.RegDept);
            Model.BsDoctorInfo infoBsDoctor = _objBsDoctor.GetByID(ouhosinfo.DoctorId); //医生
            Response += string.Format("<RbInfo><RegisterDate>{0}</RegisterDate><VisitNO>{1}</VisitNO><VisitID>{2}</VisitID><IsDiagnoses>{3}</IsDiagnoses><PatName>{4}</PatName><DepName>{5}</DepName><DoctName>{6}</DoctName></RbInfo>"
                , ouhosinfo.RegTime.ToString(), ouhosinfo.MzRegNo, ouhosinfo.ID, ouhosinfo.DiagnDept == 0 ? "N" : "Y", ouhosinfo.Name, infoBsLocation.Name, infoBsDoctor.Name);
        }

        return Response = string.Format("<Response>{0}</Response>", Response);
    }



     

    /// <summary>
    /// 费用清单
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string PatChargeList(string Request)
    {
        String columns = "req,CardNo,VisitID";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        try
        {
            BLL.Finder<Model.uspOuInvoiceDtlQry> objbllFinder = new BLL.Finder<Model.uspOuInvoiceDtlQry>();
            Model.OuHosInfoInfo infoOuHos ;
            if (GetXmlValue(Request, "req", "VisitID", "").Trim() != string.Empty)
                infoOuHos = _objOuHosInfo.GetByID(Convert.ToInt32(GetXmlValue(Request, "req", "VisitID", "").Trim()));
            else
                infoOuHos = GetOuHosInfo(GetXmlValue(Request, "req", "CardNo", ""));

            Model.ModelList<Model.OuInvoiceInfo> lstOuInvoice = new Model.ModelList<Model.OuInvoiceInfo>();
            BLL.COuInvoice _objOuInvoice = new BLL.COuInvoice();
            string strIsHave = string.Format(" MzRegId ={0}  and IsCancel = 0 ", infoOuHos.ID);
            lstOuInvoice = _objOuInvoice.GetDynamic(strIsHave, null);

            objbllFinder.AddParameter("InvoId", lstOuInvoice[0].ID);
            Model.ModelList<Model.uspOuInvoiceDtlQry> lstUspOuChargeDetail = objbllFinder.Find("uspOuInvoiceDtl");


            Response = string.Format("<PatChargeList><PatName>{0}</PatName><ChargeType>{1}</ChargeType><InvoTime>{2}</InvoTime><Items>"
                       , infoOuHos.Name, "缴费", lstUspOuChargeDetail[0].InvoTime);

            foreach (Model.uspOuInvoiceDtlQry infoUspOuCargeDetail in lstUspOuChargeDetail)
            {
                Response += string.Format("<Item><ItemId>{0}</ItemId><ItemName>{1}</ItemName><Price>{2}</Price><Num>{3}</Num><Amount>{4}</Amount>" +
                    "<ItemClass>{5}</ItemClass><Units>{6}</Units><Spec>{7}</Spec></Item>",
                    infoUspOuCargeDetail.Code, infoUspOuCargeDetail.Name, infoUspOuCargeDetail.Price, infoUspOuCargeDetail.Totality,
                    infoUspOuCargeDetail.Amount, infoUspOuCargeDetail.InvMzItemName, infoUspOuCargeDetail.UnitDiagName, infoUspOuCargeDetail.Spec);
            }
            Response += "</Items></PatChargeList><ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        }
        catch (Exception)
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg>查询失败请联系管理员</ErrorMsg>";
            //throw;
        }


        return Response = string.Format("<Response>{0}</Response>", Response);
    }


    /// <summary>
    /// 住院号列表
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string GetInpatInfo(string Request)
    {
        String columns = "req,HospitalId,PatId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        try
        {
           
            int PatId = Convert.ToInt32(GetXmlValue(Request, "req", "PatId", ""));

            Model.ModelList<Model.InHosInfoInfo> lstInHos = new Model.ModelList<Model.InHosInfoInfo>();
            BLL.CInHosInfo _objInHosInfo = new BLL.CInHosInfo();
            lstInHos = _objInHosInfo.InHosInfo_SelectByPatID(PatId);


            Response = string.Format("<Items>");

            foreach (Model.InHosInfoInfo info in lstInHos)
            {
                Response += string.Format("<Item><Id>{0}</Id><InpatNo>{1}</InpatNo><InTime>{2}</InTime><Locin>{3}</Locin><LocOut>{4}</LocOut>" +
                    "<InCount>{5}</InCount><OutTime>{6}</OutTime></Item>",
                    info.ID, info.InPatNo, info.InTime, _objBsLocation.GetByID(info.LocIn).Name,
                    _objBsLocation.GetByID(info.LocOut).Name, info.NTime, info.OutTime);
            }
            Response += "</Items><ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        }
        catch (Exception)
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg>查询失败请联系管理员</ErrorMsg>";
            //throw;
        }


        return Response = string.Format("<Response>{0}</Response>", Response);
    }

    /// <summary>
    /// 住院费用查询
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string GetInpatDetail(string Request)
    {
        String columns = "req,HospId";
        if (!String.IsNullOrEmpty(GetExist(Request, columns)))
        {
            return GetExist(Request, columns);
        }

        string Response = string.Empty;
        try
        {

            int HospId = Convert.ToInt32(GetXmlValue(Request, "req", "HospId", ""));

            Model.ModelList<Model.uspInInvoiceDtlQry> lstInInvoiceDtl = new Model.ModelList<Model.uspInInvoiceDtlQry>();
            BLL.Finder<Model.uspInInvoiceDtlQry> objInInvoiceDtl = new BLL.Finder<Model.uspInInvoiceDtlQry>();
            objInInvoiceDtl.AddParameter("HospId", HospId);
            objInInvoiceDtl.AddParameter("LocationId", 0);
            objInInvoiceDtl.AddParameter("InvItemId", 0);
            objInInvoiceDtl.AddParameter("ItemId", 0);
            objInInvoiceDtl.AddParameter("DoctorId", 0);
            objInInvoiceDtl.AddParameter("ExecLocId", 0);
            objInInvoiceDtl.AddParameter("LsMarkType", 0);
            objInInvoiceDtl.AddParameter("IsManual", false);
            objInInvoiceDtl.AddParameter("F6", " ");
            lstInInvoiceDtl = objInInvoiceDtl.Find("uspGetInpatDetailAuto");


            Response = string.Format("<Items>");

            foreach (Model.uspInInvoiceDtlQry info in lstInInvoiceDtl)
            {
                Response += string.Format("<Item><Type>{0}</Type><Sfmc>{1}</Sfmc><Spec>{2}</Spec><Unit>{3}</Unit><Totality>{4}</Totality>" +
                    "<Price>{5}</Price><Amount>{6}</Amount></Item>",
                    info.F1, info.Name, info.Spec, info.F2,
                    info.Totality, info.PriceIn, info.Amount);
            }
            Response += "</Items><ResultCode>0</ResultCode><ErrorMsg></ErrorMsg>";
        }
        catch (Exception)
        {
            Response = "<ResultCode>1</ResultCode><ErrorMsg>查询失败请联系管理员</ErrorMsg>";
            //throw;
        }


        return Response = string.Format("<Response>{0}</Response>", Response);
    }
    /// <summary>
    /// 测试打印
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [WebMethod]
    public string TestPrint(string Request)
    {
        //XYHIS.Form1 frm = new XYHIS.Form1();
        //frm.TestPrint();
        return "OK";
    }

    /// <summary>
    /// 取民族信息
    /// </summary>
    /// <param name="NationName"></param>
    /// <returns></returns>
    private Model.BsNationInfo GetNation(string nationName)
    {
        if (nationName == null || nationName == "") return null;
        BLL.CBsNation objBsNation = new BLL.CBsNation();
        Model.ModelList<Model.BsNationInfo> lstBsNation = objBsNation.BsNation_SelectByName(nationName);
        Model.BsNationInfo infoBsNation = null;
        if (lstBsNation.Count > 0) infoBsNation = lstBsNation[0];
        return infoBsNation;
    }

    /// <summary>
    /// 取星期
    /// </summary>
    /// <returns></returns>
    public int GetWeek()
    {
        DateTime dt = BLL.Common.DateTimeHandler.GetServerDateTime();
        switch (dt.DayOfWeek)
        {
            case DayOfWeek.Friday:
                return 5;
            case DayOfWeek.Monday:
                return 1;
            case DayOfWeek.Tuesday:
                return 2;
            case DayOfWeek.Sunday:
                return 0;
            case DayOfWeek.Thursday:
                return 4;
            case DayOfWeek.Saturday:
                return 6;
            case DayOfWeek.Wednesday:
                return 3;
        }
        return 0;
    }
    /// <summary>
    /// 取星期
    /// </summary>
    /// <returns></returns>
    public int GetWeek(DateTime dt)
    {
        //DateTime dt = BLL.Common.DateTimeHandler.GetServerDateTime();
        switch (dt.DayOfWeek)
        {
            case DayOfWeek.Friday:
                return 5;
            case DayOfWeek.Monday:
                return 1;
            case DayOfWeek.Tuesday:
                return 2;
            case DayOfWeek.Sunday:
                return 0;
            case DayOfWeek.Thursday:
                return 4;
            case DayOfWeek.Saturday:
                return 6;
            case DayOfWeek.Wednesday:
                return 3;
        }
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetTimeSpan()
    {

        BLL.CBsRegTimeSpan objBsRegTimeSpan = new BLL.CBsRegTimeSpan();
        Model.ModelList<Model.BsRegTimeSpanInfo> lstBsRegTimeSpan = objBsRegTimeSpan.GetAllActive();

        foreach (Model.BsRegTimeSpanInfo infoTimeSpan in lstBsRegTimeSpan)
        {
            DateTime RegTime = System.Convert.ToDateTime(BLL.Common.DateTimeHandler.GetServerDateTime().TimeOfDay.ToString());
            if ((RegTime > Convert.ToDateTime(infoTimeSpan.TimeBegin) && RegTime < Convert.ToDateTime(infoTimeSpan.TimeEnd))
                || (infoTimeSpan.F1 == "1" && (RegTime < Convert.ToDateTime(infoTimeSpan.TimeEnd) || RegTime > Convert.ToDateTime(infoTimeSpan.TimeBegin))))
                return infoTimeSpan.ID;
        }
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetTimeSpanSub()
    {

        BLL.CBsRegSpanSub objBsRegTimeSpan = new BLL.CBsRegSpanSub();
        Model.ModelList<Model.BsRegSpanSubInfo> lstBsRegTimeSpan = objBsRegTimeSpan.GetAllActive();

        foreach (Model.BsRegSpanSubInfo infoTimeSpan in lstBsRegTimeSpan)
        {
            DateTime RegTime = System.Convert.ToDateTime(BLL.Common.DateTimeHandler.GetServerDateTime().TimeOfDay.ToString());
            if ((RegTime > Convert.ToDateTime(infoTimeSpan.TimeBegin) && RegTime < Convert.ToDateTime(infoTimeSpan.TimeEnd))
                )
                return infoTimeSpan.ID;
        }
        return 0;
    }



    /// <summary>
    /// 根据卡号获取今天挂号信息
    /// </summary>
    /// <param name="cardNo"></param>
    /// <returns></returns>
    private Model.OuHosInfoInfo GetOuHosInfo(String cardNo)
    {
        Model.ModelList<Model.OuHosInfoInfo> ouhosLst = new Model.ModelList<Model.OuHosInfoInfo>();
        ouhosLst = _objOuHosInfo.GetDynamic(string.Format(" CardNo={0} and RegTime > sysdate-1 ORDER BY RegTime DESC", cardNo), null);
        if (ouhosLst.Count > 0)
            return ouhosLst[0];
        else return null;
    }

    /// <summary>
    /// 根据机器号获取UserID
    /// </summary>
    /// <param name="cardNo"></param>
    /// <returns></returns>
    private int GetUserID(string userid)
    {
        Model.ModelList<Model.BsUserInfo> ouhosLst = new Model.ModelList<Model.BsUserInfo>();
        ouhosLst = _objUserInfo.BsUser_SelectByCode(userid);
        if (ouhosLst.Count > 0)
            return ouhosLst[0].ID;
        else return 0;
    }

    /// <summary>
    /// 是否重复调用
    /// </summary>
    /// <param name="cardNo"></param>
    /// <returns></returns>
    private bool IsEd(string FlowNo)
    {
        Model.ModelList<Model.BsUserInfo> ouhosLst = new Model.ModelList<Model.BsUserInfo>();
        ouhosLst = _objUserInfo.GetDynamic(string.Format(" Code={0}  ", FlowNo), null);
        if (ouhosLst.Count > 0)
            return true;
        else return false;
    }


    /// <summary>
    /// 取xml的值
    /// </summary>
    /// <param name="xmlText">xml文本</param>
    /// <param name="node1">第一个节点</param>
    /// <param name="node2">第二个节点</param>
    /// <param name="node3">第三个节点</param>
    /// <returns></returns>
    private static string GetXmlValue(string xmlText, string node1, string node2, string node3)
    {
        string xmlvalue = string.Empty;
        XmlDocument xx = new XmlDocument();
        xx.LoadXml(xmlText);//加载xml
        XmlNodeList xxList = xx.GetElementsByTagName(node1); //取得节点名为row的XmlNode集合
        foreach (XmlNode xxNode in xxList)
        {
            XmlNodeList childList = xxNode.ChildNodes; //取得row下的子节点集合
            foreach (XmlNode xmNode in childList)
            {
                if (xmNode.Name.ToLower().Equals(node2.ToLower()))
                {
                    xmlvalue = xmNode.InnerText;
                }
                //xxNode.Attributes["name"].Value; //col节点name属性值                
                XmlNodeList scHldList = xmNode.ChildNodes;
                foreach (XmlNode xlNode in scHldList)
                {
                    if (xlNode.Name.ToLower().Equals(node3.ToLower()))
                    {
                        xmlvalue = xlNode.InnerText;
                    }
                }
            }
        }

        return xmlvalue;
    }

    /// <summary>
    /// 判断是否存在column节点(测试用的,使用就不用此方法。)
    /// </summary>
    /// <param name="req"></param>
    /// <param name="column">写法如:"str1,str2"</param>
    /// <returns></returns>
    private String GetExist(String Request, String column)
    {
        foreach (String item in column.Split(','))
        {
            if (!Request.Contains(item))
                return String.Format("<ResultCode>1</ResultCode><ErrorMsg>请联系接口程序员,参数{0}是否对应.</ErrorMsg>", item);
        }
        return string.Empty;
    }

    
}

