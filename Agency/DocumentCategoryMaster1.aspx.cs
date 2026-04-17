using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;

public partial class Agency_DocumentCategoryMaster1 : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    private void LogMessage(string message)
    {
        try
        {
            string logDirectory = Server.MapPath("~/Logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFilePath = Path.Combine(logDirectory, "DocumentCategoryMaster.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message);
            }
        }
        catch (Exception ex)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry("Application",
                    "DocumentCategoryMaster log failed: " + ex.Message,
                    System.Diagnostics.EventLogEntryType.Error);
            }
            catch { }
        }
    }

  
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
                    LogMessage("Unauthorized role access.");
                    Response.Redirect("../login.aspx");
                }
            }
            else
            {
                LogMessage("Session expired or missing.");
                Response.Redirect("../login.aspx");
            }
        }
    }

  
    [WebMethod]
    public static object AddCategory(string categoryName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return new
                {
                    status = "error",
                    message = "Please enter a category name."
                };
            }

            FlureeCS fl = new FlureeCS();

            string resultMessage = fl.AddDocumentCategory(categoryName.Trim());

            return new
            {
                status = "success",
                message = resultMessage
            };
        }
        catch (Exception ex)
        {
            return new
            {
                status = "error",
                message = ex.Message
            };
        }
    }

    
    [WebMethod]
    public static object GetDocumentCategoryData(string status)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            DataTable dt = fl.GetDocumentCategoryData(status);

            if (dt != null && dt.Rows.Count > 0)
            {
                var list = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        DocId = Convert.ToInt32(row["doctypeId"]),
                        CategoryName = row["DocCategoryName"].ToString(),
                        Status = Convert.ToBoolean(row["IsActive"]) ? "Active" : "Inactive"
                    });
                }

                return new
                {
                    status = "success",
                    data = list
                };
            }
            else
            {
                return new
                {
                    status = "empty",
                    message = "No records found."
                };
            }
        }
        catch (Exception ex)
        {
            return new
            {
                status = "error",
                message = ex.Message
            };
        }
    }

    [WebMethod]
    public static object ToggleStatus(int docId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            bool currentStatus = fl.GetCurrentStatus(docId);
            bool newStatus = !currentStatus;

            if (fl.UpdateDocumentCategoryStatus(docId, newStatus))
            {
                return new
                {
                    status = "success",
                    message = newStatus
                        ? "Record activated successfully."
                        : "Record deactivated successfully."
                };
            }
            else
            {
                return new
                {
                    status = "error",
                    message = "Operation failed!"
                };
            }
        }
        catch (Exception ex)
        {
            return new
            {
                status = "error",
                message = ex.Message
            };
        }
    }
}