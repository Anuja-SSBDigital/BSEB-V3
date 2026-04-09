using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Documenttypemaster1 : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("../login.aspx");
                return;
            }

            string userRole = Session["role"] != null ? Session["role"].ToString() : "";

            if (userRole != "Admin")
            {
                Response.Redirect("../login.aspx");
                return;
            }

            div_search.Visible = true;

            BindDocCategory();
        }
    }

 
    private void BindDocCategory()
    {
        DataTable dt = fl.GetDocumentCategoryForDropdown();

        if (dt != null && dt.Rows.Count > 0)
        {
            ddlDocType.DataSource = dt;
            ddlDocType.DataTextField = "DocCategoryName";
            ddlDocType.DataValueField = "doctypeId";
            ddlDocType.DataBind();
        }

        ddlDocType.Items.Insert(0, new ListItem("-- Select Document Category --", ""));
    }

   
    [WebMethod]
    public static object AddDocumentType(string docTypeId, string subDocName)
    {
        try
        {
            if (string.IsNullOrEmpty(docTypeId))
            {
                return new
                {
                    status = "error",
                    message = "Please select document category."
                };
            }

            if (string.IsNullOrWhiteSpace(subDocName))
            {
                return new
                {
                    status = "error",
                    message = "Please enter document type name."
                };
            }

            int doctypeId = Convert.ToInt32(docTypeId);

            FlureeCS fl = new FlureeCS();

            string result = fl.AddDocumentType(doctypeId, subDocName.Trim());

            return new
            {
                status = result.ToLower().Contains("success") ? "success" : "error",
                message = result
            };
        }
        catch (Exception ex)
        {
            return new
            {
                status = "error",
                message = "Error : " + ex.Message
            };
        }
    }

  
    [WebMethod]
    public static object GetDocumentTypeData(string status)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            DataTable dt = fl.GetDocumentTypeData(status);

            if (dt != null && dt.Rows.Count > 0)
            {
                var list = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                       
                        Id = Convert.ToInt32(row["SubDocId"]),

                       
                        SubDocName = row["subdoctypename"].ToString(),

                        IsActive = Convert.ToBoolean(row["IsActive"]),

                        Status = Convert.ToBoolean(row["IsActive"])
                            ? "Active"
                            : "Inactive"
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
    public static object ToggleStatus(int subdocId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            bool currentStatus = fl.GetDocumentTypeStatus(subdocId);
            bool newStatus = !currentStatus;

            if (fl.UpdateDocumentTypeStatus(subdocId, newStatus))
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

   
    private void Alert(string msg)
    {
        ScriptManager.RegisterStartupScript(this, GetType(),
            "alert", "alert('" + msg.Replace("'", "\\'") + "');", true);
    }
}