using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Accessfilelist : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
       
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null &&
                Session["role"] != null &&
                Session["role"].ToString() != "Agency")
            {
                BindOwnerAgency();
            }

            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    private void BindOwnerAgency()
    {
        DataTable dt = fl.GetActiveAgencies();

        ddlOwnerAgency.DataSource = dt;
        ddlOwnerAgency.DataTextField = "agencyname";
        ddlOwnerAgency.DataValueField = "agencyname";
        ddlOwnerAgency.DataBind();


        ddlOwnerAgency.Items.Insert(1, new ListItem("ALL", "ALL"));
    }


    protected void btnsearch_Click(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(ddlOwnerAgency.SelectedValue))
        //{
        //    lblMessage.Text = "Please Select Agency";
        //    return;
        //}

        BindFiles();
        //Agency_detailes.Visible = true;
    }
    private void BindFiles()
    {
        lblMessage.Text = "";

        string selectedAgency = ddlOwnerAgency.SelectedValue;
        DataTable dt;

        if (selectedAgency == "ALL")
            dt = fl.ShowFilesdetails("ALL");
        else
            dt = fl.ShowFilesdetails(selectedAgency);

        if (dt == null)
            dt = new DataTable();

        rpt_Agencywisedata.DataSource = dt;
        rpt_Agencywisedata.DataBind();

        // ✅ If no rows → add message row manually
        if (dt.Rows.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "noDataRow",
                @"
                $(document).ready(function () {
                    $('#table-1 tbody').html(
                        '<tr><td colspan=""9"" class=""text-center text-danger"">No records found.</td></tr>'
                    );
                });
                ", true);
        }
    }
    
   
   
    //private void BindFiles()
    //{

    //    lblMessage.Text = "";
    //    Agency_detailes.Visible = false;

    //    string selectedAgency = ddlOwnerAgency.SelectedValue;

    //    DataTable dt;

    //    if (selectedAgency == "ALL")
    //    {

    //        dt = fl.ShowFilesdetails("ALL");
    //    }
    //    else
    //    {
    //        dt = fl.ShowFilesdetails(selectedAgency);
    //    }
    //    if (dt != null && dt.Rows.Count > 0)
    //    {
    //        rpt_Agencywisedata.DataSource = dt;
    //        rpt_Agencywisedata.DataBind();
    //        Agency_detailes.Visible = true;
    //    }
    //    else
    //    {
    //        rpt_Agencywisedata.DataSource = null;
    //        rpt_Agencywisedata.DataBind();
    //        Agency_detailes.Visible = true;
    //        lblMessage.Text = "No records found.";
    //    }
    //}



    protected void rpt_Agencywisedata_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {

            DropDownList ddlRowAgency = (DropDownList)e.Item.FindControl("ddlRowAgency");
            if (ddlRowAgency != null)
            {

                DataTable dtAgencies = fl.GetActiveAgencies();

                ddlRowAgency.DataSource = dtAgencies;
                ddlRowAgency.DataTextField = "agencyname";
                ddlRowAgency.DataValueField = "agencyname";
                ddlRowAgency.DataBind();


                ddlRowAgency.Items.Insert(0, new ListItem("Select Agency", ""));


                object currentAgencyObj = DataBinder.Eval(e.Item.DataItem, "ViewerAgencies");
                if (currentAgencyObj != null)
                {
                    string currentAgency = currentAgencyObj.ToString();
                    if (ddlRowAgency.Items.FindByValue(currentAgency) != null)
                    {
                        ddlRowAgency.SelectedValue = currentAgency;
                    }
                }
            }
        }
    }

    

              
                                                                                                                   
    protected void rpt_Agencywisedata_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "ToggleStatus")
        {
            int fileId = Convert.ToInt32(e.CommandArgument);

            RepeaterItem item = (RepeaterItem)e.Item;
            DropDownList ddlRowAgency = (DropDownList)item.FindControl("ddlRowAgency");

            if (ddlRowAgency == null || string.IsNullOrEmpty(ddlRowAgency.SelectedValue))
            {
                string script = @"
    Swal.fire({
        icon: 'warning',
        title: 'Select Agency',
        text: 'Please select agency from row.',
        confirmButtonColor: '#3085d6'
    });";

                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "swalMessage", script, true);

                return;
            }



            string selectedAgency = ddlRowAgency.SelectedItem.Text;

            string createdBy = "";
            if (Session["username"] != null)
            {
                createdBy = Session["username"].ToString();
            }

            bool isHideAction = ((Button)e.CommandSource).Text == "Hide File";
            bool newStatus = isHideAction ? false : true;


            fl.FileAgencyAccessRights(fileId, selectedAgency, newStatus, createdBy);

            string message = "";

            if (isHideAction)
            {
                message = "File hidden for " + selectedAgency + ".";
            }
            else
            {
                message = "File visible for " + selectedAgency + ".";
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "alertMessage", "alert('" + message + "');", true);

            BindFiles();
        }
    }


    //public string GetHiddenAgencies(object fileIdObj)
    //{
    //    int fileId = Convert.ToInt32(fileIdObj);

    //    string result = "";

    //    string query = @"
    //        SELECT ViewerAgency
    //        FROM FileAgencyAccessList
    //        WHERE FileId = @FileId
    //        AND IsVisible = 0";

    //    using (SqlConnection con = new SqlConnection(conStr))
    //    using (SqlCommand cmd = new SqlCommand(query, con))
    //    {
    //        cmd.Parameters.AddWithValue("@FileId", fileId);

    //        con.Open();
    //        SqlDataReader dr = cmd.ExecuteReader();

    //        while (dr.Read())
    //        {

    //            result += "<span>"
    //                     + dr["ViewerAgency"].ToString()
    //                     + "</span>";
    //        }
    //    }


    //    return result == "" ? "<span>None</span>" : result;
    //}

    public string GetHiddenAgencies(object fileIdObj)
    {
        int fileId = Convert.ToInt32(fileIdObj);

        string result = "";

        string query = @"
        SELECT ViewerAgency
        FROM FileAgencyAccessList
        WHERE FileId = @FileId
        AND IsVisible = 0";

        using (SqlConnection con = new SqlConnection(conStr))
        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@FileId", fileId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if (result != "")
                {
                    result += ", ";
                }

                result += dr["ViewerAgency"].ToString();
            }
        }

        return result == "" ? "None" : result;
    }

}
