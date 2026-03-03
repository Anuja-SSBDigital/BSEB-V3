using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Accessfilelist : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (Session["userid"] != null)
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

        ddlOwnerAgency.Items.Insert(0, new ListItem("Select Agency", ""));
        ddlOwnerAgency.Items.Insert(1, new ListItem("ALL", "ALL"));
    }


    protected void btnsearch_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlOwnerAgency.SelectedValue))
        {
            lblMessage.Text = "Please Select Agency";
            return;
        }

        BindFiles();
        Agency_detailes.Visible = true;
    }

    private void BindFiles()
    {
        lblMessage.Text = "";
        Agency_detailes.Visible = false;

        string selectedAgency = ddlOwnerAgency.SelectedValue;

        DataTable dt = fl.ShowFilesdetails(selectedAgency);

        if (dt != null && dt.Rows.Count > 0)
        {
            rpt_Agencywisedata.DataSource = dt;
            rpt_Agencywisedata.DataBind();
            Agency_detailes.Visible = true;
        }
        else
        {
            rpt_Agencywisedata.DataSource = null;
            rpt_Agencywisedata.DataBind();
            Agency_detailes.Visible = true;
            lblMessage.Text = "No records found.";
        }
    }



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
                lblMessage.Text = "Please select agency from row.";
                return;
            }

            string selectedAgency = ddlRowAgency.SelectedItem.Text;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

              
                SqlCommand getFile = new SqlCommand(
                    "SELECT filename FROM downloadfiledetail WHERE id=@id", con);
                getFile.Parameters.AddWithValue("@id", fileId);

                string fileName = Convert.ToString(getFile.ExecuteScalar());

                if (string.IsNullOrEmpty(fileName))
                    return;

                bool isHideAction = ((Button)e.CommandSource).Text == "Hide File";
                bool newStatus = isHideAction ? false : true;

              
                SqlCommand checkCmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM FileAgencyAccessList 
                WHERE FileId = @FileId 
                  AND ViewerAgency = @ViewerAgency", con);

                checkCmd.Parameters.AddWithValue("@FileId", fileId);
                checkCmd.Parameters.AddWithValue("@ViewerAgency", selectedAgency);

                int recordCount = (int)checkCmd.ExecuteScalar();

                if (recordCount > 0)
                {
                   
                    SqlCommand updateAccess = new SqlCommand(@"
                    UPDATE FileAgencyAccessList
                    SET IsVisible=@IsVisible,
                        UpdatedDate=GETDATE()
                    WHERE FileId=@FileId
                      AND ViewerAgency=@ViewerAgency", con);

                    updateAccess.Parameters.AddWithValue("@IsVisible", newStatus);
                    updateAccess.Parameters.AddWithValue("@FileId", fileId);
                    updateAccess.Parameters.AddWithValue("@ViewerAgency", selectedAgency);

                    updateAccess.ExecuteNonQuery();
                }
                else
                {
                    
                    SqlCommand insert = new SqlCommand(@"
                    INSERT INTO FileAgencyAccessList
                    (FileId, FileName, ViewerAgency, IsVisible, CreatedDate, UpdatedDate)
                    VALUES
                    (@FileId, @FileName, @ViewerAgency, @IsVisible, GETDATE(), GETDATE())", con);

                    insert.Parameters.AddWithValue("@FileId", fileId);
                    insert.Parameters.AddWithValue("@FileName", fileName);
                    insert.Parameters.AddWithValue("@ViewerAgency", selectedAgency);
                    insert.Parameters.AddWithValue("@IsVisible", newStatus);

                    insert.ExecuteNonQuery();
                }

             
                string safeFileName = fileName.Replace("'", "\\'");
                string safeAgency = selectedAgency.Replace("'", "\\'");

                string message = "";

                if (isHideAction)
                {
                    message = safeFileName + " file hidden for " + safeAgency + ".";
                }
                else
                {
                    message = safeFileName + " file visible for " + safeAgency + ".";
                }

                string script = "alert('" + message + "');";

                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "alertMessage", script, true);
            }

            BindFiles();
        }
    }
}