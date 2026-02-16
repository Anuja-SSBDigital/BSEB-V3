using Spire.Doc.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UploadedFilesReport : System.Web.UI.Page
{
    
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindAgencyDropdown();
            lblMessage.Text = "";

        }
    }

    protected void btn_Search_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        Agency_detailes.Visible = false;
        rpt_Agencywisedata.DataSource = null;
        rpt_Agencywisedata.DataBind();

        string selectedAgency = ddlOwnerAgency.SelectedValue;

        DataTable dt;

        if (string.IsNullOrEmpty(selectedAgency))
        {
           
            dt = fl.ShowFilesdetails(""); 
        }
        else
        {
            dt = fl.ShowFilesdetails(selectedAgency);
        }

        if (dt.Rows.Count > 0)
        {
            rpt_Agencywisedata.DataSource = dt;
            rpt_Agencywisedata.DataBind();
            Agency_detailes.Visible = true;
        }
        else
        {
            Agency_detailes.Visible = true;
        }
    }
    private void BindAgencyDropdown()
    {

        DataTable dt = fl.GetActiveAgencies();
        if (dt != null && dt.Rows.Count > 0)
        {
            ddlOwnerAgency.DataSource = dt;
            ddlOwnerAgency.DataTextField = "agencyname";
            ddlOwnerAgency.DataValueField = "agencyname";
            ddlOwnerAgency.DataBind();

        }
    }

    protected void ddl_AgencyName_SelectedIndexChanged(object sender, EventArgs e)
    {

        Agency_detailes.Visible = false;


        rpt_Agencywisedata.DataSource = null;
        rpt_Agencywisedata.DataBind();


        lblMessage.Text = "";
    }


}