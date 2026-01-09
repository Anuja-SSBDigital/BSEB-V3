using iText.StyledXmlParser.Jsoup.Helper;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Approval1 : System.Web.UI.Page
{
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
    public void BindRepeator(string AdminApprovalStatus)
    {

        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            string query = "SELECT * FROM Compart_ResultChangeRequest WHERE AdminApprovalStatus = @AdminApprovalStatus";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AdminApprovalStatus", AdminApprovalStatus);
           
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                rpt_details.DataSource = reader;
                rpt_details.DataBind();
                if (AdminApprovalStatus == "Approve" || AdminApprovalStatus == "Rejected")
                {
                    btn_submit.Visible = false;
                  
                    btn_submittoken.Visible = false;
                    txt_pvtkey.Text = "";
                    lbl_validate.Text = "";
                  
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
            
            HiddenField hfStatus = (HiddenField)e.Item.FindControl("hfAdminApprovalStatus");
            HiddenField hfRequestId = (HiddenField)e.Item.FindControl("hfRequestId");
            LinkButton btnApprove = (LinkButton)e.Item.FindControl("lnkApprove");
            LinkButton btnReject = (LinkButton)e.Item.FindControl("lnkReject");
            Label lblApproved = (Label)e.Item.FindControl("lblApproved");
            Label lblRejected = (Label)e.Item.FindControl("lblRejected");

            if (hfStatus != null && hfStatus.Value == "Approve")
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
                lblApproved.Visible = true;
                lblRejected.Visible = false;
            }
            else if (hfStatus != null && hfStatus.Value == "Rejected")
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
                lblApproved.Visible = false;
                lblRejected.Visible = true;
            }
            if ( hfRequestId != null && hf_SelectedRequestId != null && hfRequestId.Value == hf_SelectedRequestId.Value && ViewState["IsApproving"] != null && (bool)ViewState["IsApproving"] == true)
            {
                btnReject.Visible = false;
                
            }
            else if (hfRequestId != null && hf_SelectedRequestId != null && hfRequestId.Value == hf_SelectedRequestId.Value && ViewState["IsRejecting"] != null && (bool)ViewState["IsRejecting"] == true)
            {
                btnApprove.Visible = false;
               
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
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @" UPDATE Compart_ResultChangeRequest SET AdminApprovalStatus = @AdminApprovalStatus,AdminReviewedBy = @AdminReviewedBy,AdminReviewedDate = @AdminReviewedDate WHERE Pk_ResultChangeRequestId = @Pk_ResultChangeRequestId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
               
                    cmd.Parameters.AddWithValue("@Pk_ResultChangeRequestId", requestId);
                    cmd.Parameters.AddWithValue("@AdminApprovalStatus", approvalStatus);
                    cmd.Parameters.AddWithValue("@AdminReviewedBy", Session["username"].ToString());
                    cmd.Parameters.AddWithValue("@AdminReviewedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                    string message = approvalStatus == "Approve" ? "Row approved successfully!" : "Row rejected successfully!";
                   
                    btn_submittoken.Visible = false;
                    btn_submit.Visible = false;
                    lbl_validate.Text = "";
                    hf_SelectedRequestId.Value = ""; 
                    ViewState["Key Is Valid"] = false;
                    
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('" + message + "');", true);

                }
                else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('No rows were updated.');", true);
                    }
               
            }
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
            lbl_validate.Text = "Key is Invalid";
            lbl_validate.ForeColor = System.Drawing.Color.Red;
            btn_submit.Visible = false;
            
        }
    }

}       