using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;

public partial class DocumentCategoryMaster : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
                        
    private void LogMessage(string message)
    {
         
        try
        {
            // Always map safely from app root, even if page is in /Agency/
            string logDirectory = Server.MapPath("~/Logs");

            // If Logs folder doesn’t exist, create it
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFilePath = Path.Combine(logDirectory, "DocumentCategoryMaster.txt");

            // Write message with timestamp
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message);
            }
        }
        catch (Exception ex)
        {
            // In case logging itself fails, fallback to Event Viewer
            try
            {
                System.Diagnostics.EventLog.WriteEntry("Application",
                    "DocumentCategoryMaster log failed: " + ex.Message,
                    System.Diagnostics.EventLogEntryType.Error);
            }
            catch
            {
                // ignore silently
            }
        }
    }

  
 //                                                                                                                                                                      
    protected void Page_Load(object sender, EventArgs e)
    {
        LogMessage("=== Page_Load triggered on " + DateTime.Now + " ===");
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                string userRole = Session["role"] != null ? Session["role"].ToString() : "";

              
                if (userRole == "Admin")
                {
                    div_search.Visible = true;
                    Div_admin.Visible = true;
                    LogMessage("Admin user logged in.");
                }
                else
                {
                    LogMessage("Unauthorized role access. Redirecting to login.");
                    Response.Redirect("../login.aspx");
                    
                }
            }
            else
            {

                LogMessage("Session expired or missing. Redirecting to login.");
                Response.Redirect("../login.aspx");
            }
        }
    }


    protected void btnAddCategory_Click(object sender, EventArgs e)
    {
      try
 {
    string categoryName = txtCategoryName.Text.Trim();
 
      if (string.IsNullOrEmpty(categoryName))
      {
          ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
              "alert('Please enter a category name.');", true);
          return;
      }
 
       string resultMessage = fl.AddDocumentCategory(txtCategoryName.Text);
 
       ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
           "alert('" + resultMessage + "');", true);
 
       if (resultMessage == "Category added successfully.")
       {
           txtCategoryName.Text = string.Empty;
       }
 }
 catch (Exception ex)
 {
     string safeMessage = ex.Message.Replace("'", "\\'");
     ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
         string.Format("alert('An error occurred: {0}');", safeMessage), true);
 }
    }


    protected void btnsearch_Click(object sender, EventArgs e)
    {
        BindDocumentCategoryData();
    }

    private void BindDocumentCategoryData()
    {
        string status = ddl_Status.SelectedValue;
        DataTable dt = fl.GetDocumentCategoryData(status);

        if (dt.Rows.Count > 0)
        {
            rpt_DocumentTypeData.DataSource = dt;
            rpt_DocumentTypeData.DataBind();
            lblMessage.Text = "";
        }
        else
        {
            rpt_DocumentTypeData.DataSource = null;
            rpt_DocumentTypeData.DataBind();
            lblMessage.Text = "No records found.";
        }
    }

    protected void rpt_DocumentTypeData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int docId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "ToggleStatus")
        {
            bool currentStatus = fl.GetCurrentStatus(docId);
            bool newStatus = !currentStatus;

            if (fl.UpdateDocumentCategoryStatus(docId, newStatus))
            {
                lblMessage.Text = newStatus ? "Record activated successfully." : "Record deactivated successfully.";
            }
            else
            {
                lblMessage.Text = "Operation failed!";
            }

            BindDocumentCategoryData();
        }
        else if (e.CommandName == "EditDoc")
        {
            Response.Redirect("Editdocumentcategorydetails.aspx?DocId=" + docId);
        }
    }
}