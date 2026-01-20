using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

public partial class Agency_approveprofile : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {

        BindAgencyDropdown();
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {

             

            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
       
        var UserStatus = ddl_Userstatus.SelectedValue;
        DataTable resforuser = fl.FindUser(ddl_AgencyName.SelectedValue, UserStatus);
        if (resforuser.Rows.Count > 0)
        {
            rpt_userData.DataSource = resforuser;
            rpt_userData.DataBind();    
        }
        else
        {
            rpt_userData.DataSource = null;
            rpt_userData.DataBind();
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
                @"SELECT DISTINCT agencyname 
                  FROM agencyuser 
                  WHERE agencyname IS NOT NULL 
                  ORDER BY agencyname", con))
            {
                con.Open();

                ddl_AgencyName.DataSource = cmd.ExecuteReader();
                ddl_AgencyName.DataTextField = "agencyname";
                ddl_AgencyName.DataValueField = "agencyname";
                ddl_AgencyName.DataBind();
            }
        }
    }


    protected void rpt_userData_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Label lbl_userstatus = (Label)e.Item.FindControl("lbl_userstatus");
            LinkButton link_Active = (LinkButton)e.Item.FindControl("link_Active");
            LinkButton link_DeActive = (LinkButton)e.Item.FindControl("link_DeActive");
            LinkButton link_approve = (LinkButton)e.Item.FindControl("link_approve");
            LinkButton link_rejected = (LinkButton)e.Item.FindControl("link_rejected");
            HiddenField hf_status = (HiddenField)e.Item.FindControl("hf_status");

            if (lbl_userstatus.Text == "Active")
            {
                link_DeActive.Visible = true;
                link_Active.Visible = false;
                link_approve.Visible = false;
                link_rejected.Visible = false;
            }
            else if (lbl_userstatus.Text == "DeActive")
            {
                link_Active.Visible = true;
                link_DeActive.Visible = false;
                link_approve.Visible = false;
                link_rejected.Visible = false;
            }
            else if (lbl_userstatus.Text == "Rejected")
            {
                link_approve.Visible = true;
                link_rejected.Visible = true;
                link_Active.Visible = false;
                link_DeActive.Visible = false;
            }
            else
            {
                link_approve.Visible = true;
                link_rejected.Visible = true;
                link_Active.Visible = false;
                link_DeActive.Visible = false;
            }

            //if (hf_status.Value == "Pending For Approval")
            //{
            //    chkSelectAll.Visible = true;
            //}
            //else
            //{
            //    chkSelectAll.Visible = false;

            //}

            //CheckBox chk = (CheckBox)e.Item.FindControl("chkSelect");
            //string value = ((Label)e.Item.FindControl("lbl_userid")).Text;

            //// Check if the checkbox value exists in the session
            //List<string> selectedItems = Session["SelectedItems"] as List<string> ?? new List<string>();

            //if (selectedItems.Contains(value))
            //{
            //    chk.Checked = true; // Restore selection from session
            //}
        }

    }
    protected void rpt_userData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        HiddenField emailid = (HiddenField)e.Item.FindControl("hf_emailid");
        HiddenField agency = (HiddenField)e.Item.FindControl("hf_agency");
        HiddenField username = (HiddenField)e.Item.FindControl("hf_username");

        if (e.CommandName == "link_approve")
        {
            string userId = e.CommandArgument.ToString();
            string resUser = fl.Updateagencyuserstatus(userId, "Active");

            if (!string.IsNullOrEmpty(resUser) && emailid != null && username != null && agency != null)
            {
                SendUserIdEmail(emailid.Value, username.Value, agency.Value, userId);
            }
            else
            {
                fl.log.Error("Error: One or more hidden fields are null.");
            }
        }
        else if (e.CommandName == "link_rejected")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "Rejected");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('User Rejected Successfully');location.href = location.href;</script>");
            }
        }
        else if (e.CommandName == "link_Active")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "Active");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('User DeActive Successfully');location.href = location.href;</script>");
            }
        }
        else if (e.CommandName == "link_DeActive")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "DeActive");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('Status Changed Successfully');location.href = location.href;</script>");
            }
        }
    }

    private void SendUserIdEmail(string email, string username, string agencyname, string Userid)
    {
        // Generate a random password
        string password = GenerateRandomPassword(8);
        string encryptedPassword = fl.EncryptString(password);

        // Update user password in the database
        bool resUser = fl.UpdateUserPassword(encryptedPassword, Userid);

        if (resUser)
        {
            // Email body with login credentials
            string html = "<div style='width: 80%; height: auto; margin: auto; padding: 20px; font-family: Arial;'>"
                        + "<div style='border: 3px solid #f1f1f1; padding: 20px; background-color: #f1f1f1;'>"
                        + "<h2>Account Approved</h2>"
                        + "<p>Dear " + username + ",</p>"
                        + "<p>Your account has been approved. You can now log in using the following credentials:</p>"
                        + "<p><strong>Username:</strong> " + username + "</p>"
                        + "<p><strong>Password:</strong> " + password + " </p>"
                        + "<p><strong>Agency Name:</strong> " + agencyname + "</p>"
                        + "<p><strong>Key for Upload:</strong></p>"
                        + "<p><a href='https://ho.ssbdigital.in/bseb-v1/'>Click here to login</a></p>"
                        + "<h3>Regards,<br/><b>SSBI Team</b></h3>"
                        + "</div></div>";

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress("helpme@ssbdigital.in");
                message.To.Add(new MailAddress(email));
                message.Subject = "Your Account Has Been Approved";
                message.IsBodyHtml = true;
                message.Body = html;

                smtp.Port = 587;
                smtp.Host = "sg2plzcpnl505639.prod.sin2.secureserver.net";
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("helpme@ssbdigital.in", "5FG_W^nt.#TU");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(message);
                Response.Write("<script language='javascript'>window.alert('Approval email sent successfully!'); location.href = location.href;</script>");

            }
            catch (Exception ex)
            {
                fl.log.Error("Email Sending Error: " + ex.Message);
            }
        }
        else
        {
            Response.Write("<script language='javascript'>window.alert('Oops! Something went wrong with your registration. Please try again.'); window.location.reload();</script>");

            //Console.WriteLine("Password update failed.");
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