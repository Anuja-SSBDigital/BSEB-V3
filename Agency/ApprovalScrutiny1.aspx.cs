using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Activities.Expressions;

public partial class Agency_ApprovalScrutiny1 : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "Swal.fire({icon:'error', title:'Invalid Input', text:'Roll Code and Roll Number must be numeric'});",
                true);
            return;
        }

        BindGlobalSummary();
        BindData(rollCodeValue, rollNoValue);
    }
     
    
    private void BindData(string rollCodeValue, string rollNoValue)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        SELECT Id, reg_no, roll_code, roll_no, Subjectname, subjectcode,
               BARCODE_BOTTOM, Litho_Cbar_Fly, MarksSourceName,
               SubjectiveMarks, subjecttotal,
               ISNULL(Approval1,'Pending') AS Approval1
        FROM scrutinydata
        WHERE roll_code = @rollCode AND roll_no = @rollNo";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@rollCode", rollCodeValue);
                cmd.Parameters.AddWithValue("@rollNo", rollNoValue);
                cmd.CommandTimeout = 60;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Student_details.Visible = true;

                    rpt_userData.DataSource = dt;
                    rpt_userData.DataBind();

                    bool showButtons = dt.AsEnumerable()
                        .Any(r => r["Approval1"].ToString().ToLower() == "pending");

                    divAction.Visible = showButtons;
                }
                else
                {
                    Student_details.Visible = false;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "Swal.fire({icon:'error', title:'No Data Found', text:'No record found'});",
                        true);
                }
            }
        }
    }

    private int UpdateGlobalStatus(string status)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        UPDATE scrutinydata
        SET Approval1 = @status
        WHERE Approval1 IS NULL OR Approval1 <> @status";   

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.CommandTimeout = 120; 

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }

    private void BindGlobalSummary()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        SELECT 
            COUNT(*) AS TotalRows,

            COUNT(DISTINCT CONCAT(roll_code, '-', roll_no)) AS UniqueStudents,

            SUM(CASE 
                WHEN Approval1 IS NULL OR Approval1 = 'Pending' 
                THEN 1 ELSE 0 
            END) AS PendingApproval1Count

        FROM scrutinydata WITH (NOLOCK)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandTimeout = 120;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    summaryCard.Visible = true;

                    lblTotalRows.Text = dr["TotalRows"].ToString();
                    lblUniqueCount.Text = dr["UniqueStudents"].ToString();

                    int pending = Convert.ToInt32(dr["PendingApproval1Count"]);
                    divAction.Visible = (pending > 0);
                }
            }
        }
    } 

    [WebMethod]
    public static object GetSummary()
    {
        string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        SELECT 
            COUNT(*) AS TotalRows,
            COUNT(DISTINCT CONCAT(roll_code, '-', roll_no)) AS UniqueStudents
        FROM scrutinydata WITH (NOLOCK)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandTimeout = 120;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new
                    {
                        TotalRows = dr["TotalRows"].ToString(),
                        UniqueStudents = dr["UniqueStudents"].ToString()
                    };
                }
            }
        }

        return null;
    }

    //protected void btnGlobalApprove_Click(object sender, EventArgs e)
    //{
    //    int rows = UpdateGlobalStatus("Approved");

    //    ScriptManager.RegisterStartupScript(this, GetType(), "ok",
    //        "Swal.fire('Done','" + rows + " records approved','success');", true);

    //    BindGlobalSummary();
    //}


    protected void btnGlobalApprove_Click(object sender, EventArgs e)
    {
        try
        {
            List<string> allowedIps = new List<string> { "192.168.1.106",    "127.0.0.1",
    "::1"

            };


            string clientIp = GetClientIp();

            if (!allowedIps.Contains(clientIp))
            {
                string script = @"
Swal.fire({
    title: 'Access Denied!',
    text: 'You are not authorized to Approved Status.',
    icon: 'error',
    confirmButtonText: 'OK'
});";

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
                return;
            }

            int rows = UpdateGlobalStatus("Approved");

            ScriptManager.RegisterStartupScript(this, GetType(), "ok",
                "Swal.fire('Done','" + rows + " records approved','success');", true);

            BindGlobalSummary();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err2",
                "Swal.fire('Error','" + ex.Message.Replace("'", "") + "','error');",
                true);
        }
    }

    public static string GetClientIp()
    {
        string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ip))
        {

            string[] ipArray = ip.Split(',');
            ip = ipArray[0].Trim();
        }
        else
        {
            ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        if (string.IsNullOrEmpty(ip))
        {
            ip = "127.0.0.1";
        }


        if (ip == "::1")
        {
            ip = "127.0.0.1";
        }


        if (ip.StartsWith("::ffff:"))
        {
            ip = ip.Replace("::ffff:", "");
        }

        ip = ip.Trim();

        return ip;
    }

    //protected void btnGlobalReject_Click(object sender, EventArgs e)
    //{
    //    int rows = UpdateGlobalStatus("Rejected");

    //    ScriptManager.RegisterStartupScript(this, GetType(), "b",
    //        "Swal.fire('Done','" + rows + " records rejected','success');", true);

    //    BindGlobalSummary();
    //}


    protected void btnGlobalReject_Click(object sender, EventArgs e)
    {
        try
        {
            List<string> allowedIps = new List<string> { "192.168.1.106",    "127.0.0.1",
    "::1"
   
            };

            string clientIp = GetClientIp();

            if (!allowedIps.Contains(clientIp))
            {
                string script = @"
Swal.fire({
    title: 'Access Denied!',
    text: 'You are not authorized to Approved Status.',
    icon: 'error',
    confirmButtonText: 'OK'
});";

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
                return;
            }

            int rows = UpdateGlobalStatus("Rejected");

        ScriptManager.RegisterStartupScript(this, GetType(), "b",
            "Swal.fire('Done','" + rows + " records rejected','success');", true);

        BindGlobalSummary();
    }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err2",
                "Swal.fire('Error','" + ex.Message.Replace("'", "") + "','error');",
                true);
        }
    }

}