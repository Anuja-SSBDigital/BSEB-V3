using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Approvaldata_1 : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
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


        BindData(rollCodeValue, rollNoValue);
    }

    private void BindData(string rollCodeValue, string rollNoValue)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT Id, reg_no, roll_code, roll_no, Subjectname, subjectcode,
                                    BARCODE_BOTTOM, Litho_Cbar_Fly, MarksSourceName,
                                    SubjectiveMarks, subjecttotal, CreatedDate,
                                    IsActive,
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
                User_detailes.Visible = true;
                rpt_userData.DataSource = dt;
                rpt_userData.DataBind();


                divAction.Visible = true;


                bool done = dt.AsEnumerable().All(r =>
                    r["Approval1"].ToString() == "Approved" ||
                    r["Approval1"].ToString() == "Rejected");

                if (done)
                {
                    divAction.Visible = false;

                }
            }
            else
            {
                User_detailes.Visible = false;
                divAction.Visible = false;


                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
    "Swal.fire({icon:'error', title:'No Data Found', text:'No record found for given Roll Code and Roll Number'});",
    true);
            }
        }
    }
    protected void btnApproveAll_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string q = @"UPDATE scrutinydata SET Approval1='Approved'
                         WHERE roll_code=@rc AND roll_no=@rn";

            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@rc", rollCode.Text.Trim());
            cmd.Parameters.AddWithValue("@rn", rollNo.Text.Trim());

            con.Open();
            cmd.ExecuteNonQuery();
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "c",
        "Swal.fire('Success','All Approved','success');", true);

        BindData(rollCode.Text.Trim(), rollNo.Text.Trim());
    }

    protected void btnRejectAll_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string q = @"UPDATE scrutinydata SET Approval1='Rejected'
                         WHERE roll_code=@rc AND roll_no=@rn";

            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@rc", rollCode.Text.Trim());
            cmd.Parameters.AddWithValue("@rn", rollNo.Text.Trim());

            con.Open();
            cmd.ExecuteNonQuery();
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "d",
        "Swal.fire('Success','All Rejected','success');", true);

        BindData(rollCode.Text.Trim(), rollNo.Text.Trim());
    }

   
}