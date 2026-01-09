using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Dashboard : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    public void filldashboarddata()
    {
        string agencyname = "";
        int totalRecords;
        if (Session["role"].ToString() == "Admin")
        {
            agencyname = ddl_AgencyName.SelectedValue;
        }
        else
        {
            agencyname = Session["agencyname"].ToString();
        }
        //DateTime createDate = DateTime.ParseExact(txt_date.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //txt_date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        string resdata = fl.getActualdata_agencydatewise(agencyname, txt_date.Text, out totalRecords);
        lbl_dailyprocddata.Text = totalRecords > 0 ? totalRecords.ToString() : "0";

        DataTable dttotal = fl.GetTotalCountsDashboard(agencyname);
        if (dttotal.Rows.Count > 0) // Ensure the DataTable has data
        {
            lbl_totaldata.Text = dttotal.Rows[0]["AdjustedTotalRecords"].ToString();
        }
        else
        {
            lbl_totaldata.Text = "0"; 
        }


        int total, duplicate;
        

        DataTable dt = fl.dashboardsheetwisecount(agencyname, txt_date.Text, out total, out duplicate);
        if (dt != null && dt.Rows.Count > 0)
        {
            rpt_FileuploadCsvData.DataSource = dt;
            rpt_FileuploadCsvData.DataBind();
        }
        else
        {
            rpt_FileuploadCsvData.DataSource = null;
            rpt_FileuploadCsvData.DataBind();
        }


    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {

                txt_date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                if (Session["role"].ToString() == "Admin")
                {
                    div_search.Visible = true;
                }
                else
                {
                    div_search.Visible = true;
                    Div_admin.Visible = false;
                    Div_ags.Visible = true;
                    filldashboarddata();

                }
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }
    
    protected void rpt_FileuploadCsvData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "lnk_edit")
        {
            Response.Redirect("editdevice.aspx?macid=" + e.CommandArgument.ToString() + "");

        }
    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
     
        filldashboarddata();
    }
}