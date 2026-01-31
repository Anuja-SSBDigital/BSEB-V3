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
            lblMessage.Text = "";

        }
    }

    protected void btn_Search_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        Agency_detailes.Visible = false;
        rpt_Agencywisedata.DataSource = null;
        rpt_Agencywisedata.DataBind();

        if (string.IsNullOrEmpty(ddl_AgencyName.SelectedValue))
        {
            lblMessage.Text = "Please select an agency.";
            return;
        }

        DataTable dt = fl.ShowFilesdetails(ddl_AgencyName.SelectedValue);

        if (dt.Rows.Count > 0)
        {
            rpt_Agencywisedata.DataSource = dt;
            rpt_Agencywisedata.DataBind();
            Agency_detailes.Visible = true;
        }
        else
        {
            lblMessage.Text = "No files available for selected agency.";
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