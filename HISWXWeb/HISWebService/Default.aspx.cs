using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        //WsOuCard ws = new WsOuCard();
        //string Resquest = "<Request><CardNo>20110808002</CardNo><PatientName>测试</PatientName>"+
        //    "<Nation>汉族</Nation><Sex>男</Sex><Birthday>1990-10-10</Birthday><Age>21</Age><IDCardNo>434545199010108545</IDCardNo>"+
        //    "<Amt>0</Amt><Address>广东省茂名市油城五路</Address><Tel>13789765467</Tel>"+
        //    "<UserId>9</UserId><ActDate>2011-8-8 12:00:00</ActDate></Request>";
        //Label2.Text = ws.CreateCardPatInfo(Resquest);

        //string Resquest = "<Request><CardNo>812801</CardNo><PatientName>陈大伟</PatientName><Nation></Nation><Sex>男</Sex><Birthday>1988-12-25</Birthday><Age>24</Age><IDCardNo>350321133812252639</IDCardNo><SecrityNo></SecrityNo><CardSerNo></CardSerNo><Amt>100</Amt><Address>湖南省株洲市荷塘区文化六村湖南工业大学集体宿舍</Address><Tel></Tel><UserId>810001</UserId><ActDateTime>2011-8-11 17:08:05</ActDateTime></Request>";
        //string Resquest = "<Request><DOCKSList><CardNo>810001</CardNo></DOCKSList></Request>";
        //Label2.Text = ws.DOCKSList(Resquest) + ws.GetWeek().ToString() + DateTime.Now.DayOfWeek;

        //string Resquest = "<Request><CardNo>201107140022</CardNo></Request>";
        //Label2.Text = ws.GetBillInfo(Resquest);

        //string Resquest = "<Request><FlowNo>123456789</FlowNo><CardNo>810001</CardNo><Amt>30</Amt><UserId>9</UserId></Request>";
        //Label2.Text = ws.AddDeposit(Resquest);

        //string Resquest = "<Request><DOCHBList><SecrityNo></SecrityNo><CardSerNo></CardSerNo><CardNo>00157063</CardNo><Day>2011-8-15</Day><DeptId>288</DeptId><UserId>810001</UserId></DOCHBList></Request>";
        //Label2.Text = ws.DOCHBList(Resquest);

        //string Resquest = "<Request><CardNo>00157063</CardNo></Request>";
        //Label2.Text = ws.CardNoCheck(Resquest);

        //string Resquest = "<Request><OPRegist><CardNo>810001</CardNo><SecrityNo></SecrityNo><CardSerNo><FlowNo></FlowNo></CardSerNo><SumFee>3</SumFee><RowId>5630</RowId><day>2011-08-16</day><HBTime>下午</HBTime><UserId>9</UserId></OPRegist></Request>";
        //Label2.Text = ws.OPRegist(Resquest);

        //string Resquest = "<Request><CardNo>810001</CardNo><DeptId>328</DeptId><RowId>5623</RowId></Request>";
        //Label2.Text = ws.AutoOPBillCharge(Resquest);
       
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        string IDCardNo = "<Request><IDCardNo>55</IDCardNo></Request>";

        XmlDocument xx = new XmlDocument();
        xx.LoadXml(IDCardNo);//加载xml
        XmlNodeList xxList = xx.GetElementsByTagName("Request"); //取得节点名为row的XmlNode集合
        foreach (XmlNode xxNode in xxList)
        {
            XmlNodeList childList = xxNode.ChildNodes; //取得row下的子节点集合
            foreach (XmlNode xmNode in childList)
            {
                switch (xmNode.Name)
                {
                    case "IDCardNo":
                        Label1.Text = xmNode.InnerText;
                        break;
                    default:
                        break;
                }
                //xxNodes.InnerText; //返回的是col的文字内容
                //xxNode.Attributes["name"].Value; //col节点name属性值
            }
        }


        BLL.CBsUser _objBsUser = new BLL.CBsUser();
        Model.ModelList<Model.BsUserInfo> lstBsUser = new Model.ModelList<Model.BsUserInfo>();
        lstBsUser = _objBsUser.BsUser_SelectByCode(this.TextBox1.Text);
        Label1.Text += lstBsUser[0].Name;

    }
}
