using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class registerAgs : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {

        }
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        try
        {
            // Register the user
            string AgencyName = ddl_AgencyName.SelectedValue;
            string agencyAbbreviation = GetAgencyAbbreviation(AgencyName);
            string randomNumber = GenerateRandomNumber();
            string username = agencyAbbreviation + "-" + randomNumber;
            bool isUserIdExists = fl.CheckUserExist(username);

            // Generate a unique username if needed
            while (isUserIdExists)
            {
                randomNumber = GenerateRandomNumber();
                username = agencyAbbreviation + "-" + randomNumber;
                isUserIdExists = fl.CheckUserExist(username);
            }

            // Use the generated username for registration
            string res_register = fl.Register_User(username, AgencyName, txt_Email.Text, txt_Phone.Text, "Pass@123");

            // Check the result for success or error
            if (!res_register.StartsWith("Error"))
            {
                // If registration is successful, send a confirmation email
                SendConfirmationEmail(username, txt_Email.Text);

                // Show a thank-you message and redirect to authentication page
                Response.Write("<script language='javascript'>window.alert('User Added SuccessFully. Link to check User Status is sent to Your Registered Email ID');window.location='login.aspx';</script>");
            }
            else
            {

                Response.Write("<script language='javascript'>window.alert('Oops! Something went wrong with your registration. Please try again.');window.location='authentication.aspx';</script>");


            }
        }
        catch (WebException ex)
        {
            Response.Write("<script>alert('Email Sending Error: " + ex.Message + "');</script>");
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
           
        }
    }
    private void SendConfirmationEmail(string username, string email)
    {
        string html = "<div style='width: 80%;height: auto;bottom: 0;left: 0;right: 0;margin: auto;margin-bottom: 30px;margin-top:20px'>"
                    + "<div style = 'border: 3px solid #f1f1f1;font-family: Arial;'>"
                    + "<div style = 'padding: 20px;background-color: #f1f1f1;'>"
                    + "<h2> Registration Confirmation </h2>"
                    + "<p> Dear User,</p><p> Your username is: " + username + "</p><p> Thanks for Registering! Your Profile is in Under Approval Process. " +
                     "Once the Profile is Approved Your Credentials will be sent to your Registered EmailID.</p>"
                    + "<p>Please note your User ID for future reference: " + username + "</p></div>"
                    + "<h3>Regards,<br/><b>SSBI Team</b></h3>";

        //try
        //{
        //    MailMessage message = new MailMessage();
        //    SmtpClient smtp = new SmtpClient();
        //    message.From = new MailAddress("helpme@ssbdigital.in");
        //    message.To.Add(new MailAddress(email));
        //    message.Subject = "Registration Confirmation";
        //    message.IsBodyHtml = true;
        //    message.Body = html;
        //    smtp.Port = 587;
        //    smtp.Host = "sg2plzcpnl505639.prod.sin2.secureserver.net";
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = new NetworkCredential("helpme@ssbdigital.in", "5FG_W^nt.#TU");
        //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    smtp.EnableSsl = true;
        //    smtp.Send(message);
        //}
        //catch (Exception ex)
        //{
        //    Response.Write("<script>alert('Error: " + ex.Message + "');</script>");

        //}
    }

    private string GetAgencyAbbreviation(string agencyName)
    {
        switch (agencyName)
        {
            case "Mappel":
                return "MP";
            case "Datacode":
                return "DC";
            case "Kids":
                return "KD";
            case "MCRK":
                return "MK";
            case "Keltron":
                return "KT";
            case "Charu Mindworks":
                return "CM";
            case "Hitech":
                return "HT";
            case "Shree Jagannath Udyog":
                return "JU";
            case "Datasoft":
                return "DS";
            case "SSB Digital":
                return "SD";
            case "Antier":
                return "AT";
            default:
                return "";
        }
    }

    private string GenerateRandomNumber()
    {
        Random random = new Random();
        return random.Next(1000, 9999).ToString("D4");
    }
}

