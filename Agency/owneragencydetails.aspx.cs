using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

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

                BindAgencyDropdown();
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    private void BindAgencyDropdown()
    {
        string conStr = ConfigurationManager
                        .ConnectionStrings["dbcon"]
                        .ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT DISTINCT LTRIM(RTRIM(agencyname)) AS agencyname
              FROM agencyuser
              WHERE agencyname IS NOT NULL
              GROUP BY LTRIM(RTRIM(agencyname))
              ORDER BY LTRIM(RTRIM(agencyname))", con))
            {
                con.Open();

                ddlOwnerAgency.DataSource = cmd.ExecuteReader();
                ddlOwnerAgency.DataTextField = "agencyname";
                ddlOwnerAgency.DataValueField = "agencyname";
                ddlOwnerAgency.DataBind();
            }
        }
    }
    [WebMethod]
    public static bool DeleteAccess(int accessId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();
            fl.SoftdeleteAgencyDocumentAccess(accessId);
            return true;
        }
        catch
        {   
            return false;
        }
    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        string ownerAgency = ddlOwnerAgency.SelectedValue.Trim();

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
            Agency_detailes.Visible = true;
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