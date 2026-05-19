using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
    
public partial class Agency_Approval2 : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
    FlureeCS fl=new FlureeCS(); 
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Session["userid"] != null)
                {
                    string userRole = Session["role"] != null ? Session["role"].ToString() : "";

                    if (userRole == "Admin")
                    {
                        BindGlobalSummary();
                    }
                    else
                    {
                        Response.Redirect("../login.aspx", false);
                    }
                }
                else
                {

                    Response.Redirect("../login.aspx", false);
                }
            }
            else
            {

                if (Session["userid"] != null && Session["role"] != null && Session["role"].ToString() == "Admin")
                {
                    summaryCard.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('Error: " + ex.Message.Replace("'", "") + "');", true);
        }
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        string rollCodeValue = rollCode.Text.Trim();
        string rollNoValue = rollNo.Text.Trim();

        int rc, rn;

        if (!int.TryParse(rollCodeValue, out rc) || !int.TryParse(rollNoValue, out rn))
        {
            ShowAlert("Invalid Input", "Roll Code and Roll Number must be numeric", "error");
            return;
        }

        BindGlobalSummary();
        BindData(rollCodeValue, rollNoValue);
    }

    private void BindData(string rollCodeValue, string rollNoValue)
    {
        DataTable dt = fl.GetApproval2StudentData(rollCodeValue, rollNoValue);

        if (dt.Rows.Count > 0)
        {
            Student_details.Visible = true;

            rpt_userData.DataSource = dt;
            rpt_userData.DataBind();

            bool showButtons = false;

            foreach (DataRow row in dt.Rows)
            {
                string a1 = row["Approval1"].ToString().ToLower();
                string a2 = row["Approval2"].ToString().ToLower();

                if (a1 == "approved" && a2 == "pending")
                {
                    showButtons = true;
                    break;
                }
            }

            divAction.Visible = showButtons;
        }
        else
        {
            Student_details.Visible = false;
            ShowAlert("No Data Found", "No record found", "error");
        }
    }
       

             
    private void BindGlobalSummary()
    {
        DataTable dt = fl.GetApproval2Summary();

        if (dt.Rows.Count > 0)
        {
            summaryCard.Visible = true;

            lblTotalRows.Text = dt.Rows[0]["TotalRows"].ToString();
            lblUniqueCount.Text = dt.Rows[0]["UniqueStudents"].ToString();

            int count = Convert.ToInt32(dt.Rows[0]["ShowButtonCount"]);
            divAction.Visible = count > 0;
        }
    }
       
    protected void btnGlobalApprove_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable check = fl.CheckApproval1Status();

            int pending = Convert.ToInt32(check.Rows[0]["Pending"]);
            int rejected = Convert.ToInt32(check.Rows[0]["Rejected"]);

            if (pending > 0 || rejected > 0)
            {
                ShowAlert("Error", pending + " Pending and " + rejected + " Rejected Records Exist at Approval Level 1, so they cannot be Approved.", "error");
                return;
            }

            string clientIp = GetClientIp();
            List<string> allowedIps = fl.GetAllowedIPsFromApproval();

            bool isAllowed = allowedIps.Any(ip => ip.Equals(clientIp, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                ShowAlert("Access Denied!", "You are not authorized to Approve Status.", "error");
                return;
            }
           

            int rows = fl.UpdateApproval2Status("Approved");

            ShowAlert("Done", rows + " records approved", "success");

            BindGlobalSummary();
        }
        catch (Exception ex)
        {
            ShowAlert("Error", ex.Message, "error");
        }
    }
    protected void btnGlobalReject_Click(object sender, EventArgs e)
    {
        try 
        {
            string clientIp = GetClientIp();
            List<string> allowedIps = fl.GetAllowedIPsFromApproval();

            bool isAllowed = allowedIps.Any(ip => ip.Equals(clientIp, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                ShowAlert("Access Denied!", "You are not authorized to Reject Status.", "error");
                return;
            }

            int rows = fl.UpdateApproval2Status("Rejected");

            ShowAlert("Done", rows + " records rejected", "success");

            BindGlobalSummary();
        }
        catch (Exception ex)
        {
            ShowAlert("Error", ex.Message, "error");
        }
    }

      
    public static string GetClientIp()
    {
        string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ip))
            ip = ip.Split(',')[0];
        else
            ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        if (string.IsNullOrEmpty(ip))
            ip = "127.0.0.1";

        if (ip == "::1")
            ip = "127.0.0.1";

        if (ip.StartsWith("::ffff:"))
            ip = ip.Replace("::ffff:", "");

        return ip.Trim();
    }

    private void ShowAlert(string title, string message, string icon)
    {
        title = title.Replace("'", "\\'");
        message = message.Replace("'", "\\'");
        icon = icon.Replace("'", "\\'");

        string script = "Swal.fire('" + title + "','" + message + "','" + icon + "');";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
    }
}