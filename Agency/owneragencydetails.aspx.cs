using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

public partial class Agency_owneragencydetails : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                if (Session["role"].ToString() == "Admin")
                    div_search.Visible = true;
                else
                {
                    div_search.Visible = true;
                    Div_admin.Visible = false;
                }
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    protected void btnsearch_Click(object sender, EventArgs e) 
    {
        string ownerAgency = ddl_AgencyName.SelectedValue.Trim();

        if (string.IsNullOrEmpty(ownerAgency))
        {
            lblMessage.Text = "Please select Owner Agency.";
            lblMessage.CssClass = "text-danger";
            rpt_Agencywisedata.DataSource = null;
            rpt_Agencywisedata.DataBind();
            return;
        }

        DataTable dt = fl.GetProcessFileList(ownerAgency);

        if (dt != null && dt.Rows.Count > 0)
        {
            rpt_Agencywisedata.DataSource = dt;
            rpt_Agencywisedata.DataBind();
         
            lblMessage.CssClass = "text-success";
        }
        else
        {
            rpt_Agencywisedata.DataSource = null;
            rpt_Agencywisedata.DataBind();
            
        }
    }
}
