using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Agencymaster : System.Web.UI.Page
{

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string conStr = ConfigurationManager
                        .ConnectionStrings["dbcon"]
                        .ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

          
            using (SqlCommand checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM agencyuser WHERE username = @username", con))
            {
                checkCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "userExists",
                        "alert('Username already exists! Please choose another username.');",
                        true);
                    return; 
                }
            }

       
            string plainPassword = GeneratePassword();

          
            string hashedPassword = EncryptString(plainPassword);

            using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO agencyuser
            (username, email, mobileno, agencyname, password, role,
             status, created_at, updated_at, PrivateKey, Key_Expiry, PlainTextPassword)
            VALUES
            (@username, @email, @mobileno, @agencyname, @password, @role,
             'Active', GETDATE(), GETDATE(), '', NULL, @PlainTextPassword)", con))
            {
                cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@mobileno", txtMobile.Text.Trim());
                cmd.Parameters.AddWithValue("@agencyname", txtAgency.Text.Trim());
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@PlainTextPassword", plainPassword);
                cmd.Parameters.AddWithValue("@role", "Agency");

                cmd.ExecuteNonQuery();
            }

            string script = "alert('Agency user added successfully! Username: "
                            + txtUsername.Text.Trim() +
                            " | Password: " + plainPassword + "');";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "SuccessMessage", script, true);

            ClearForm();
        }
    }        


    public string EncryptString(string str)
    {
        MD5 md5Hash = MD5.Create();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
       
        StringBuilder sBuilder = new StringBuilder();
    
        for (int i = 0; i < data.Length; i++) 
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

   
    private string GeneratePassword()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new StringBuilder();
        Random rnd = new Random();

        for (int i = 0; i < 8; i++)
        {
            sb.Append(chars[rnd.Next(chars.Length)]);
        }

        return sb.ToString();
    }
    private void ClearForm()
    {
        txtUsername.Text = "";
        txtEmail.Text = "";
        txtMobile.Text = "";
        txtAgency.Text = "";
    }
}
