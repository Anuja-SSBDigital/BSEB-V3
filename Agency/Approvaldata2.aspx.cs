using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Approvaldata2 : System.Web.UI.Page
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

                    if (userRole != "Admin")
                        Response.Redirect("../login.aspx", false);
                }
                else
                {
                    Response.Redirect("../login.aspx", false);
                }
            }
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('An unexpected error occurred during page load.');", true);
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        string rc = rollCode.Text.Trim();
        string rn = rollNo.Text.Trim();

        int x, y;
        if (!int.TryParse(rc, out x) || !int.TryParse(rn, out y))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "a",
                "Swal.fire('Error','Invalid Roll Code / Roll No','error');", true);
            return;
        }

        BindData(rc, rn);
    }

    private void BindData(string rollCodeValue, string rollNoValue)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT Id, reg_no, roll_code, roll_no, Subjectname, subjectcode,
                                BARCODE_BOTTOM, Litho_Cbar_Fly, MarksSourceName,
                                SubjectiveMarks, subjecttotal,
                                ISNULL(Approval1,'Pending') AS Approval1,
                                ISNULL(Approval2,'Pending') AS Approval2
                         FROM scrutinydata
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


                bool showButtons = dt.AsEnumerable().Any(r =>
                {
                    string a1 = Convert.ToString(r["Approval1"]);
                    string a2 = Convert.ToString(r["Approval2"]);

                    return a1 == "Approved" &&
                           (string.IsNullOrEmpty(a2) || a2 == "Pending");
                });

                divAction2.Visible = showButtons;
            }
            else
            {
                User_detailes.Visible = false;
                divAction2.Visible = false;

                ScriptManager.RegisterStartupScript(this, GetType(), "b",
                    "Swal.fire('No Data','No record found','error');", true);
            }
        }
    }

    protected async void btnApproveAll_Click(object sender, EventArgs e)
    {

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string q = @"UPDATE scrutinydata 
                         SET Approval2='Approved'
                         WHERE roll_code=@rc AND roll_no=@rn";

            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@rc", rollCode.Text.Trim());
            cmd.Parameters.AddWithValue("@rn", rollNo.Text.Trim());

            con.Open();
            cmd.ExecuteNonQuery();
        }


        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7171/");

                string url = string.Format(
       "api/ResultUpdate/Updatereultdata?rollCode={0}&rollNo={1}",
       rollCode.Text.Trim(),
       rollNo.Text.Trim()
   );

                HttpResponseMessage response = await client.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "c",
                        "Swal.fire('Success','Approved & Published','success');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "c",
                        "Swal.fire('Warning','Approved but API failed','warning');", true);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "c",
                "Swal.fire('Error','API Error: " + ex.Message.Replace("'", "") + "','error');", true);
        }


        BindData(rollCode.Text.Trim(), rollNo.Text.Trim());
    }


    protected void btnRejectAll_Click(object sender, EventArgs e)
    {
        string rc = rollCode.Text.Trim();
        string rn = rollNo.Text.Trim();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string q = @"UPDATE scrutinydata 
                         SET Approval2='Rejected'
                         WHERE roll_code=@rc AND roll_no=@rn";

            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@rc", rc);
            cmd.Parameters.AddWithValue("@rn", rn);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "d",
            "Swal.fire('Success','All Rejected','success');", true);

        BindData(rc, rn);
    }

    protected void rpt_userData_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView row = (DataRowView)e.Item.DataItem;

            string a1 = Convert.ToString(row["Approval1"]);
            string a2 = Convert.ToString(row["Approval2"]);

            LinkButton approve = (LinkButton)e.Item.FindControl("link_approve");
            LinkButton reject = (LinkButton)e.Item.FindControl("link_reject");


            if (a2 == "Approved" || a2 == "Rejected")
            {
                approve.Visible = false;
                reject.Visible = false;
                return;
            }

            if (a1 != "Approved")
            {
                approve.Visible = false;
                reject.Visible = false;
                return;
            }
            approve.Visible = true;
            reject.Visible = true;
        }
    }
}