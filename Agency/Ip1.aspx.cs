using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Ip1 : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                string userRole = Session["role"] != null ? Session["role"].ToString() : "";

            }
            else
            {
                Response.Redirect("../login.aspx", false);
            }
        }
    }

   
    [WebMethod]
    public static object AddIP(string ipNumber, string agencyName, bool canProcessCSV, bool canFileUpload)
    {
        try
        {
            if (string.IsNullOrEmpty(ipNumber))
            {
                return new
                {
                    status = "error",
                    message = "Please enter an IP address."
                };
            }

            if (!canProcessCSV && !canFileUpload)
            {
                return new
                {
                    status = "error",
                    message = "Please select at least one access type."
                };
            }

            FlureeCS fl = new FlureeCS();

            string updatedBy = HttpContext.Current.Session["username"] != null
                ? HttpContext.Current.Session["username"].ToString()
                : "System";

            string resultMessage = fl.InsertIP(ipNumber, agencyName, updatedBy,
                                              canProcessCSV, canFileUpload);

            return new
            {
                status = resultMessage.Contains("success") ? "success" : "error",
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
    public static object GetIPData(string status)
    {
        try
        {
            FlureeCS fl = new FlureeCS();
            DataTable dt = fl.GetIPList(status);

            if (dt != null && dt.Rows.Count > 0)
            {
                var list = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        Id = Convert.ToInt32(row["Id"]),   
                        IPNumber = row["IPNumber"].ToString(), 
                        AgencyName = row["AgencyName"].ToString(),

                        CanProcessCSV = Convert.ToBoolean(row["CanProcessCSV"]), 
                        CanUpload = Convert.ToBoolean(row["CanUpload"]),        

                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        Status = Convert.ToBoolean(row["IsActive"]) ? "Active" : "Inactive"
                    });
                }

                return new { status = "success", data = list };
            }
            else
            {
                return new { status = "empty", message = "No records found." };
            }
        }
        catch (Exception ex)
        {
            return new { status = "error", message = ex.Message };
        }
    }

    
    [WebMethod]
    public static object ToggleIPStatus(int ipId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            string updatedBy = HttpContext.Current.Session["username"] != null
                ? HttpContext.Current.Session["username"].ToString()
                : "System";

            bool result = fl.ToggleIPStatus(ipId, updatedBy);

            return new
            {
                status = result ? "success" : "error",
                message = result ? "Status updated successfully." : "Operation failed!"
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
}