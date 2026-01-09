using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using iText.StyledXmlParser.Jsoup.Helper;
using Org.BouncyCastle.Asn1.Cmp;

public partial class Approval2 : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
              
                btn_submit.Visible = false;

                btn_submittoken.Visible = false;

            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }
    public void BindRepeator(string FinalApprovalStatus)
    {

        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            string query = "SELECT * FROM Compart_ResultChangeRequest WHERE FinalApprovalStatus = @FinalApprovalStatus";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FinalApprovalStatus", FinalApprovalStatus);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                rpt_details.DataSource = dt;
                rpt_details.DataBind();

               
                if (dt.Rows.Count > 0)
                {
                    string FinalStatus = dt.Rows[0]["FinalApprovalStatus"].ToString();
                    ViewState["FinalStatus"] = FinalStatus;
                }
                else
                {
                    ViewState["FinalStatus"] = null;
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
           
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        BindRepeator(ddl_status.SelectedValue);

    }

    protected void rpt_details_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {

            HiddenField hfAdminApprovalStatus = (HiddenField)e.Item.FindControl("hfAdminApprovalStatus");
            HiddenField hfFinalApprovalStatus = (HiddenField)e.Item.FindControl("hfFinalApprovalStatus");
            LinkButton btnApprove = (LinkButton)e.Item.FindControl("lnkApprove");
            LinkButton btnReject = (LinkButton)e.Item.FindControl("lnkReject");
            Label lblApproved = (Label)e.Item.FindControl("lblApproved");
            Label lblRejected = (Label)e.Item.FindControl("lblRejected");
            
            if (hfAdminApprovalStatus != null && hfFinalApprovalStatus != null)
            {
                string adminStatus = hfAdminApprovalStatus.Value;
                string finalStatus = hfFinalApprovalStatus.Value;

                if (adminStatus == "Approve" && finalStatus == "Approve")
                {
                  
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                    lblApproved.Visible = true;
                    lblRejected.Visible = false;
                }
                else if (adminStatus == "Approve" && finalStatus == "Rejected")
                {
                   
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                    lblApproved.Visible = false;
                    lblRejected.Visible = true;
                }
                else if (adminStatus == "Approve" && finalStatus == "Pending")
                {
                    
                    btnApprove.Visible = true;
                    btnReject.Visible = true;
                    lblApproved.Visible = false;
                    lblRejected.Visible = false;
                }

                else if (adminStatus == "Pending" || adminStatus == "Rejected")
                {
                  
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                    lblApproved.Visible = false;
                    lblRejected.Visible = false;
                }
            }

        }
    }


    private int? ParseNullableInt(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            int result;
            if (int.TryParse(input.Trim(), out result))
            {
                return result;
            }
        }
        return null;
    }
    protected void rpt_details_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int requestId = Convert.ToInt32(e.CommandArgument);
        hf_SelectedRequestId.Value = requestId.ToString();
        div_key.Visible = true;
        btn_submittoken.Visible = true;


        if (e.CommandName == "ApproveRow")
        {
            ViewState["IsApproving"] = true;
            ViewState["IsRejecting"] = false;

        }
        else if (e.CommandName == "RejectRow")
        {
            ViewState["IsRejecting"] = true;
            ViewState["IsApproving"] = false;

        }

        BindRepeator(ddl_status.SelectedValue);

        ScriptManager.RegisterStartupScript(this, this.GetType(), "showKeyInput", "$('#txt_pvtkey').closest('.form-group').show();", true);
    }

    private void UpdateApprovalStatus(int requestId, string approvalStatus, string message)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @"UPDATE Compart_ResultChangeRequest 
                         SET FinalApprovalStatus = @FinalApprovalStatus,
                             AdminReviewedBy = @AdminReviewedBy,
                             AdminReviewedDate = @AdminReviewedDate 
                         WHERE Pk_ResultChangeRequestId = @Pk_ResultChangeRequestId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Pk_ResultChangeRequestId", requestId);
                cmd.Parameters.AddWithValue("@FinalApprovalStatus", approvalStatus);
                cmd.Parameters.AddWithValue("@AdminReviewedBy", Session["username"].ToString());
                cmd.Parameters.AddWithValue("@AdminReviewedDate", DateTime.Now);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    btn_submittoken.Visible = false;
                    btn_submit.Visible = false;
                    lbl_validate.Text = "";
                    hf_SelectedRequestId.Value = "";
                    ViewState["Key Is Valid"] = false;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + message.Replace("'", "\\'") + "');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('❌ Failed to update approval status.');", true);
                }
            }
        }
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        string key = txt_pvtkey.Text.Trim();



        if (string.IsNullOrEmpty(key))
        {
            lbl_validate.Text = "Private key is required.";
            return;
        }


        int requestId;
        if (!int.TryParse(hf_SelectedRequestId.Value, out requestId))
        {
            lbl_validate.Text = "Invalid request.";
            return;
        }
        string approvalStatus = "";

       
        if (ViewState["IsApproving"] != null && (bool)ViewState["IsApproving"])
        {
            approvalStatus = "Approve";
        }
        else if (ViewState["IsRejecting"] != null && (bool)ViewState["IsRejecting"])
        {
            approvalStatus = "Rejected";
        }
        else
        {
            lbl_validate.Text = "Invalid operation.";
            return;
        }

        if (approvalStatus == "Approve")
        {
            int updatedCount = 0;

            foreach (RepeaterItem item in rpt_details.Items)
            {

                HiddenField hfObtainedMarks = (HiddenField)item.FindControl("hf_ObtainedMarks");
                HiddenField hfTotalTheoryMarks = (HiddenField)item.FindControl("hf_TotalTheoryMarks");
                HiddenField hfCCEMarks = (HiddenField)item.FindControl("hf_CCEMarks");
                HiddenField hfSubjectTotal = (HiddenField)item.FindControl("hf_SubjectTotal");
                HiddenField hfTotalMarks = (HiddenField)item.FindControl("hf_TotalMarks");
                HiddenField hf_rollcode = (HiddenField)item.FindControl("hf_rollcode");
                HiddenField hf_rollno = (HiddenField)item.FindControl("hf_rollno");

                string rollCode = hf_rollcode != null ? hf_rollcode.Value : null;
                string rollNumber = hf_rollno != null ? hf_rollno.Value : null;

                int? obtainedMarks = !string.IsNullOrEmpty(hfObtainedMarks.Value) ? (int?)Convert.ToInt32(hfObtainedMarks.Value) : null;
                int? totalTheoryMarks = !string.IsNullOrEmpty(hfTotalTheoryMarks.Value) ? (int?)Convert.ToInt32(hfTotalTheoryMarks.Value) : null;
                int? cceMarks = !string.IsNullOrEmpty(hfCCEMarks.Value) ? (int?)Convert.ToInt32(hfCCEMarks.Value) : null;
                int? subjectTotal = !string.IsNullOrEmpty(hfSubjectTotal.Value) ? (int?)Convert.ToInt32(hfSubjectTotal.Value) : null;
                int? totalMarks = !string.IsNullOrEmpty(hfTotalMarks.Value) ? (int?)Convert.ToInt32(hfTotalMarks.Value) : null;

                bool? absentTh = null;
                bool? absentPr = null;
                int? theoryGraceMarks = null;
                int? practicalGraceMarks = null;
                string division = null;
                bool? isPassInTotal = null;

                string res = fl.UpdateMarksSelective(
                  rollCode, rollNumber,
                    obtainedMarks,
                    cceMarks,
                    absentTh,
                    absentPr,
                    totalTheoryMarks,
                    subjectTotal,
                    theoryGraceMarks,
                    practicalGraceMarks,
                    totalMarks,
                    division,
                    isPassInTotal
                );

                if (res.StartsWith("✅"))
                {
                    updatedCount++;
                }
            }

           
            UpdateApprovalStatus(requestId, approvalStatus, updatedCount > 0
                ? "Status approved. Blockchain data updated successfully."
                : "Status approved but marks were not updated.");
        }
        else if (approvalStatus == "Rejected")
        {
            
            UpdateApprovalStatus(requestId, approvalStatus, "Request rejected successfully.");
        }

        ViewState["IsApproving"] = null;
        ViewState["IsRejecting"] = null;

        BindRepeator(ddl_status.SelectedValue);
    }

    protected void btn_submittoken_Click(object sender, EventArgs e)
    {
        FlureeCS fl = new FlureeCS();
        string enteredKey = txt_pvtkey.Text.Trim();

        if (fl.IsPrivateKeyValid(enteredKey))   
        {
            lbl_validate.Text = "Key is Valid";
            lbl_validate.ForeColor = System.Drawing.Color.Green;
            btn_submit.Visible = true;

        }
        else
        {
            lbl_validate.Text = "Key is Invalid or Expired";
            lbl_validate.ForeColor = System.Drawing.Color.Red;
            btn_submit.Visible = false;

        }
    }

}