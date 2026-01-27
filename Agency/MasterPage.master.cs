using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    public void menurights()
    {
        if(Session["role"].ToString() == "Admin")
        {
            li_approveprofile.Visible = true;
            li_dashboard.Visible = false;
            li_template.Visible = false;
            li_fileupload.Visible = false;
            li_processfilecsvfiles.Visible = false;
            li_duplicatediscrepency.Visible = false;

            li_dcmntaccess.Visible = true;
            li_keygenaration.Visible = true;
            li_accesslist.Visible = true;
            li_doccategory.Visible = true;
            li_Ipmaster.Visible = true;
            li_Documenttypemaster.Visible = true;
        }
        else
        {
            li_template.Visible = true;
            li_dashboard.Visible = false;
            li_approveprofile.Visible = false;
            li_fileupload.Visible = true;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                menurights();
                if (Session["agencyname"].ToString() == "Datasoft" || Session["agencyname"].ToString() == "Antier" || Session["agencyname"].ToString() == "SSB Digital")
                {
                    li_fileDownload.Visible = false;
                }
                else
                {
                    li_fileDownload.Visible = false;
                }
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        //FlureeCS fl = new FlureeCS();
        //  public static string serverqryurl = "http://localhost:8090/fdb/ssb/bseb/query";
        //string response = fl.sendTransaction("", serverqryurl, txt_pvtkey.Text);
    }
}
