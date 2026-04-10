using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

public partial class Agency_approveprofile1 : System.Web.UI.Page
{
  
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null &&
                Session["role"] != null &&
                Session["role"].ToString() != "Agency")
            {
                BindAgencyDropdown();
            }

            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    private void BindAgencyDropdown()
    {
        string conStr = ConfigurationManager
                        .ConnectionStrings["dbcon"]
                        .ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT DISTINCT LTRIM(RTRIM(agencyname)) AS agencyname
              FROM agencyuser
              WHERE agencyname IS NOT NULL
              GROUP BY LTRIM(RTRIM(agencyname))
              ORDER BY LTRIM(RTRIM(agencyname))", con))
            {
                con.Open();

                ddlOwnerAgency.DataSource = cmd.ExecuteReader();
                ddlOwnerAgency.DataTextField = "agencyname";
                ddlOwnerAgency.DataValueField = "agencyname";
                ddlOwnerAgency.DataBind();
            }
        }
    }

   
    [WebMethod]
    public static object GetUserData(string agencyId, string userStatus)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            DataTable dt = fl.FindUser(agencyId, userStatus);

            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    id = row["id"].ToString(),
                    username = row["username"].ToString(),
                    email = row["email"].ToString(),
                    mobileno = row["mobileno"].ToString(),
                    status = row["status"].ToString(),
                    agencyname = row["agencyname"].ToString()
                });
            }

            return new
            {
                status = "success",
                data = list
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
    public static object ApproveUser(string userId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            string res = fl.Updateagencyuserstatus(userId, "Active");

            if (!res.StartsWith("Error"))
            {
                return new { status = "success", message = "User Approved Successfully" };
            }
            else
            {
                return new { status = "error", message = res };
            }
        }
        catch (Exception ex)
        {
            return new { status = "error", message = ex.Message };
        }
    }

   
    [WebMethod]
    public static object RejectUser(string userId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            string res = fl.Updateagencyuserstatus(userId, "Rejected");

            if (!res.StartsWith("Error"))
            {
                return new { status = "success", message = "User Rejected Successfully" };
            }
            else
            {
                return new { status = "error", message = res };
            }
        }
        catch (Exception ex)
        {
            return new { status = "error", message = ex.Message };
        }
    }

   
    [WebMethod]
    public static object ActiveUser(string userId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            string res = fl.Updateagencyuserstatus(userId, "Active");

            if (!res.StartsWith("Error"))
            {
                return new { status = "success", message = "User Activated Successfully" };
            }
            else
            {
                return new { status = "error", message = res };
            }
        }
        catch (Exception ex)
        {
            return new { status = "error", message = ex.Message };
        }
    }

  
    [WebMethod]
    public static object DeactiveUser(string userId)
    {
        try
        {
            FlureeCS fl = new FlureeCS();

            string res = fl.Updateagencyuserstatus(userId, "DeActive");

            if (!res.StartsWith("Error"))
            {
                return new { status = "success", message = "User Deactivated Successfully" };
            }
            else
            {
                return new { status = "error", message = res };
            }
        }
        catch (Exception ex)
        {
            return new { status = "error", message = ex.Message };
        }
    }

   
    private string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}