using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;



/// <summary>
///Common 的摘要说明
/// </summary>
public class Common
{
    static Common()
    { 
    }


    static int t = 0;
    /// <summary>
    ///  网站初始化时调用一次
    /// </summary>
    public static void InitData()
    {
        if (t > 0) return;


        //WsInHost wsInHost = new WsInHost();

        //Model.ModelList<Model.BsLocationInfo> lst = BLL.MemoryDate.GetBaseTableLstInfo<Model.BsLocationInfo>("CBsLocation");

        //foreach (Model.BsLocationInfo infoBsLocation in lst)
        //{
        //    if (infoBsLocation.LsInOut == 1 || infoBsLocation.LsInOut == 3)
        //    {
        //        wsInHost.SetFrmAdviceAuthServer(0, infoBsLocation.ID);//刷新查对医嘱数据

        //        wsInHost.SetFrmInDrugRequestServer(0, infoBsLocation.ID, infoBsLocation.SpecialityId);//刷新病区申请数据

        //        wsInHost.SetFrmInExecuteBillServer(0, infoBsLocation.ID, infoBsLocation.SpecialityId);//刷新医嘱执行数据，hospid=0 则刷新整个locationid

        //        wsInHost.SetFrmInDrugBackReqServer(0, infoBsLocation.SpecialityId);//刷新退药数据

        //    }
        //}
        t++;
    }
}
