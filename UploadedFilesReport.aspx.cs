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

    }

    
    protected void btn_Search_Click(object sender, EventArgs e)
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