using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_ApprovalScrutiny2 : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;


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
            string query = @"SELECT Id, reg_no, roll_code, roll_no, Subjectname, subjectcode,
                         BARCODE_BOTTOM, Litho_Cbar_Fly, MarksSourceName,
                         SubjectiveMarks, subjecttotal, CreatedDate,
                         ISNULL(Approval1,'Pending') AS Approval1,
                         ISNULL(Approval2,'Pending') AS Approval2
                         FROM [BSEB-V3].[dbo].[scrutinydata]
                         WHERE roll_code = @rollCode AND roll_no = @rollNo";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@rollCode", rollCodeValue);
            cmd.Parameters.AddWithValue("@rollNo", rollNoValue);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

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
                divAction.Visible = false;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "Swal.fire({icon:'error', title:'No Data Found', text:'No record found'});",
                    true);
            }
        }
    }


    private int UpdateGlobalStatus(string status)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        UPDATE scrutinydata
        SET Approval2 = @status
        WHERE 
            ISNULL(Approval2,'Pending') <> @status
            AND Approval1 = 'Approved'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@status", status);

            con.Open();
            return cmd.ExecuteNonQuery();
        }
    }


    private void BindGlobalSummary()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        SELECT 
            COUNT(*) AS TotalRows,

            COUNT(DISTINCT CAST(roll_code AS VARCHAR(20)) + '_' + CAST(roll_no AS VARCHAR(20))) AS UniqueStudents,

            SUM(CASE WHEN Approval2 = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,

            SUM(CASE WHEN Approval2 = 'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,

            SUM(CASE WHEN Approval2 IS NULL OR Approval2 = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,

            SUM(CASE WHEN Approval2 IN ('Approved','Rejected') THEN 1 ELSE 0 END) AS ChangedCount,

            -- 🔹 Button logic condition
            SUM(CASE 
                WHEN Approval1 = 'Approved' 
                     AND (Approval2 IS NULL OR Approval2 = 'Pending') 
                THEN 1 ELSE 0 
            END) AS ShowButtonCount

        FROM [BSEB-V3].[dbo].[scrutinydata] WITH (NOLOCK)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandTimeout = 120;

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                summaryCard.Visible = true;

                lblTotalRows.Text = dr["TotalRows"].ToString();
                lblUniqueCount.Text = dr["UniqueStudents"].ToString();


                int count = Convert.ToInt32(dr["ShowButtonCount"]);


                divAction.Visible = count > 0;
            }
        }
    }


    protected void btnGlobalApprove_Click(object sender, EventArgs e)
    {
        int pending = 0;
        int rejected = 0;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string checkQuery = @"
        SELECT 
            SUM(CASE WHEN Approval1 IS NULL OR Approval1='Pending' THEN 1 ELSE 0 END),
            SUM(CASE WHEN Approval1='Rejected' THEN 1 ELSE 0 END)
        FROM scrutinydata";

            SqlCommand cmd = new SqlCommand(checkQuery, con);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                pending = Convert.ToInt32(dr[0]);
                rejected = Convert.ToInt32(dr[1]);
            }
            con.Close();
        }


        if (pending > 0 || rejected > 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",


                "Swal.fire('Error','" + pending + " Pending and " + rejected + " Rejected Records Exist at Approval Level 1, so they cannot be Approved.','error');",
                true);



            return;
        }


        int rows = UpdateGlobalStatus("Approved");

        ScriptManager.RegisterStartupScript(this, GetType(), "ok",
            "Swal.fire('Done','" + rows + " records approved','success');", true);

        BindGlobalSummary();
    }



    protected void btnGlobalReject_Click(object sender, EventArgs e)
    {
        int rows = UpdateGlobalStatus("Rejected");

        ScriptManager.RegisterStartupScript(this, GetType(), "b",
            "Swal.fire('Done','" + rows + " records rejected','success');", true);

        BindGlobalSummary();
    }


    [System.Web.Services.WebMethod]
    public static object GetSummary()
    {
        string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
            SELECT 
                COUNT(*) AS TotalRows,

                COUNT(DISTINCT CAST(roll_code AS VARCHAR(20)) + '_' + CAST(roll_no AS VARCHAR(20))) AS UniqueStudents,

                SUM(CASE WHEN Approval2 = 'Approved' THEN 1 ELSE 0 END) AS Approved,

                SUM(CASE WHEN Approval2 = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,

                SUM(CASE WHEN Approval2 IS NULL OR Approval2='Pending' THEN 1 ELSE 0 END) AS Pending,

                SUM(CASE WHEN Approval2 IN ('Approved','Rejected') THEN 1 ELSE 0 END) AS Changed

            FROM [BSEB-V3].[dbo].[scrutinydata] WITH (NOLOCK)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandTimeout = 120;

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new
                {
                    TotalRows = dr["TotalRows"].ToString(),
                    UniqueStudents = dr["UniqueStudents"].ToString(),
                    Approved = dr["Approved"].ToString(),
                    Rejected = dr["Rejected"].ToString(),
                    Pending = dr["Pending"].ToString(),
                    Changed = dr["Changed"].ToString()
                };
            }
        }

        return null;
    }
}