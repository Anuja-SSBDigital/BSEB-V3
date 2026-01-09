using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class AccessKeyManagement : System.Web.UI.Page
{

    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {

            }

            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

                                    
    public string GenerateJwtToken(string agencyName, out DateTime expiryDate)
    {
        expiryDate = DateTime.UtcNow.AddMonths(9);


        var header = "{\"alg\":\"HS512\",\"typ\":\"JWT\"}";
        string base64Header = Base64UrlEncode(Encoding.UTF8.GetBytes(header));

        // 2️⃣ Generate random values (for pri and session)
        string priValue = GenerateRandomBase64(64);    // ~88 chars
        string sessionValue = GenerateRandomBase64(32); // ~44 chars

        // 3️⃣ Payload (added extra claims to make token longer)
        var payload = string.Format(
            "{{\"iss\":\"{0}\",\"sub\":\"{1}\",\"exp\":{2},\"iat\":{3},\"pri\":\"{4}\",\"role\":\"Admin\",\"agencyId\":\"{5}\",\"session\":\"{6}\"}}",
            "ssb/bseb", // issuer
            agencyName, // subject
            new DateTimeOffset(expiryDate).ToUnixTimeSeconds(), // expiry
            GetIssuedAtUnixTime(), // issued at
            priValue, // pri → random string
            Guid.NewGuid().ToString(), // agencyId
            sessionValue // session → random string
        );

        string base64Payload = Base64UrlEncode(Encoding.UTF8.GetBytes(payload));

        // 4️⃣ Signature (HS512)
        string secretKey = "yKX9wQZC+uZqQn6fQKX1Vq3t1B1ZqLhHcPjV8N2yT1U=";
        string signature = CreateSignature(base64Header + "." + base64Payload, secretKey, "SHA512");

        // 5️⃣ Final JWT
        string jwtToken = base64Header + "." + base64Payload + "." + signature;

        return jwtToken;
    }

    // Utility: Base64Url encode
    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }


    // Utility: Create signature
    private string CreateSignature(string data, string secretKey, string algorithm = "SHA256")
    {
        using (var hmac = algorithm == "SHA512"
            ? (HMAC)new HMACSHA512(Encoding.UTF8.GetBytes(secretKey))
            : new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Base64UrlEncode(hash);
        }
    }

    private string GenerateRandomBase64(int byteLength)
    {
        byte[] randomBytes = new byte[byteLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
    private long GetIssuedAtUnixTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
    private static long GetExpiryUnixTime(int monthsToAdd)
    {
        return new DateTimeOffset(DateTime.UtcNow.AddMonths(monthsToAdd)).ToUnixTimeSeconds();
    }
    protected void btnGenerateToken_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlOwnerAgency.SelectedValue))
        {
            lblToken.Text = "Please select an agency first.";
            lblToken.CssClass = "form-control text-danger";
            return;
        }

        // Store expiry date for later use
        DateTime expiryDate;
        string token = GenerateJwtToken(ddlOwnerAgency.SelectedValue, out expiryDate);

        lblToken.Text = token;
        lblToken.CssClass = "form-control text-success";

        // Save expiry date in hidden field for btnSendEmail_Click
        hdnExpiryDate.Value = expiryDate.ToString("yyyy-MM-dd HH:mm:ss");
    }

    protected void btnSendEmail_Click(object sender, EventArgs e)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        string query = @"
        UPDATE agencyuser
        SET PrivateKey = @PrivateKey,
            Key_expiry = @Key_expiry
        WHERE agencyname = @AgencyName";

        using (SqlConnection conn = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@PrivateKey", lblToken.Text);
            cmd.Parameters.AddWithValue("@Key_expiry", DateTime.Parse(hdnExpiryDate.Value));
            cmd.Parameters.AddWithValue("@AgencyName", ddlOwnerAgency.SelectedValue);

            conn.Open();

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {

                // suppose you take email from a textbox

                // Dictionary of allowed agencies
                var validAgencies = new Dictionary<string, string>
    {
        //{ "Shree Jagannath Udyog", "shreejagannath150@gmail.com" },
        //{ "Hitech", "hpslit1@hitechprivt.com" },
        //{ "Charu Mindworks", "devanshu@indiaresults.com" },
        //{ "Datacon", "ramesh.datacon@gmail.com" },
        //{ "Mapple", "techsupport@bigdataimaging.com" },
        //{ "Kids", "kiplbseb@gmail.com" },
        //{ "MCRK", "mcrkjaipur@gmail.com" },
        //{ "SSB Digital", "nikesh@ssbi.in" },
        //{ "Antier", "singh.gaurav@antiersolution.com" }
        { "Antier", "anuja.dubey@ssbi.in" }
    };
                string selectedAgency = ddlOwnerAgency.SelectedItem.Text;

                if (validAgencies.ContainsKey(selectedAgency))
                {
                    string agencyEmail = validAgencies[selectedAgency];
                    // Success
                    //  SendTokenEmail(ddlOwnerAgency.SelectedItem.Text, lblToken.Text, DateTime.Parse(hdnExpiryDate.Value), agencyEmail);
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert",
                    "alert('Token and expiry date updated successfully.');", true);
            }
            else
            {
                // No record updated
                ScriptManager.RegisterStartupScript(this, GetType(), "failureAlert",
                    "alert('No matching agency found. Update failed.');", true);
            }
        }

    }


    protected void btnsearch_Click(object sender, EventArgs e)
    {
        BindAgencyUserData();
    }

    private void BindAgencyUserData()
    {
      
        if (string.IsNullOrEmpty(ddl_agency.SelectedValue))
        {
            rpt_DocumentTypeData.DataSource = null;
            rpt_DocumentTypeData.DataBind();

            lblMessage.Text = "Please select an agency.";
            lblMessage.CssClass = "text-danger fw-bold";
            return;
        }

        string agency = ddl_agency.SelectedValue;
        DataTable dt = fl.AgencyWiseData(agency);

        if (dt.Rows.Count > 0)
        {
            rpt_DocumentTypeData.DataSource = dt;
            rpt_DocumentTypeData.DataBind();

            lblMessage.Text = "";
        }
        else
        {
            rpt_DocumentTypeData.DataSource = null;
            rpt_DocumentTypeData.DataBind();

            lblMessage.Text = "No records found.";
            lblMessage.CssClass = "text-info fw-bold";
        }
    }


    private void SendTokenEmail(string agencyName, string token, DateTime expiryDate, string recipientEmail)
    {
        string absoluteurl = HttpContext.Current.Request.Url.AbsoluteUri;
        string[] SplitedURL = absoluteurl.Split('/');
        string http = SplitedURL[0];
        string Host = HttpContext.Current.Request.Url.Host;
        string strUrl = "";
        if (Host == "localhost")
        {
            strUrl = http + "//" + HttpContext.Current.Request.Url.Authority + "/";

        }
        else
        {
            strUrl = http + "//" + Host + HttpContext.Current.Request.ApplicationPath + "/";
        }
        string url = strUrl + "login.aspx";

        string html = string.Format(
     "<div style='width: 80%; margin: 20px auto; font-family: Arial, sans-serif;'>" +
         "<div style='border: 2px solid #ccc; border-radius: 8px; overflow: hidden;'>" +
             "<div style='padding: 20px; background-color: #392C70; color: white;'>" +
                 "<h2 style='margin:0;'>Your Private Key</h2>" +
             "</div>" +
             "<div style='padding: 20px; background-color: #fff; font-size: 14px; line-height: 1.6;'>" +
                 "<p>Dear {0},</p>" +
                 "<p>Your new private key has been generated for your agency access. Please find it below:</p>" +
                 "<div style='padding: 15px; background-color: #f4f4f4; border: 1px solid #ccc; word-break: break-all;'>" +
                     "<b>Private Key:</b><br/>{1}" +
                 "</div>" +
                 "<p>This key will be valid until <b>{2}</b>. Keep it secure and do not share it with anyone.</p>" +
             "</div>" +
             "<div style='padding: 20px; background-color: #f1f1f1; text-align:center;'>" +
                 "<a href='{3}' style='display:inline-block; padding: 12px 25px; background-color:#392C70; color:white; text-decoration:none; border-radius:4px;'>Login Now</a>" +
             "</div>" +
         "</div>" +
         "<p style='margin-top:20px;'>Regards,<br/><b>SSB Digital Team</b></p>" +
     "</div>",
     agencyName,
     token,
     expiryDate.ToString("dd-MMM-yyyy"),
        url
        );
        try
        {

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress("helpme@ssbdigital.in");
            message.To.Add("anuja.dubey@ssbi.in");
            message.Bcc.Add("anuja.dubey@ssbi.in");
            // message.Bcc.Add("bcc2@example.com");
            message.Subject = "Your New Private Key for Agency Access";
            message.IsBodyHtml = true;
            message.Body = html;

            smtp.Port = 587;
            //smtp.Host = "sg2plzcpnl505639.prod.sin2.secureserver.net";
            smtp.Host = "ssbdigital.in";
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("helpme@ssbdigital.in", "5FG_W^nt.#TU");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;

            smtp.Send(message);

        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "emailFailAlert",
                "alert('Token saved but email could not be sent.');", true);
        }
    }

}