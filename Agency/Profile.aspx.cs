using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Agency_Profile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadUserProfile();
        }
    }

    private void LoadUserProfile()
    {
        if (Session["username"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        string username = Session["username"].ToString();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT TOP 1 * FROM agencyuser WHERE username = @username";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUsername.Text = reader["username"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtMobileNo.Text = reader["mobileno"].ToString();
                    txtAgencyName.Text = reader["agencyname"].ToString();
                }
                connection.Close();
            }
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string username = Session["username"].ToString();
        string userId = Session["userid"].ToString();



        string email = txtEmail.Text.Trim();
        string mobileNo = txtMobileNo.Text.Trim();
        string agencyName = txtAgencyName.Text.Trim();


        if (fl_image.HasFile)
        {
            string folderPath = Server.MapPath("~/AgencyProfile/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileExtension = Path.GetExtension(fl_image.FileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            if (Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                // Save file with UserID as the filename
                string fileName = userId + fileExtension;
                profileImagePath = "~/AgencyProfile/" + fileName;
                customFile.SaveAs(Server.MapPath(profileImagePath));
            }
            else
            {

                return;
            }
        }

        // Database connection string
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string updateQuery = @"UPDATE agencyuser 
                                   SET email = @Email, mobileno = @MobileNo, agencyname = @AgencyName 
                                   {0} 
                                   WHERE id = @UserID";

            // If a new profile image is uploaded, include it in the query
            if (!string.IsNullOrEmpty(profileImagePath))
            {
                updateQuery = string.Format(updateQuery, ", profile_image = @ProfileImage");
            }
            else
            {
                updateQuery = string.Format(updateQuery, "");
            }

            using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@MobileNo", mobileNo);
                cmd.Parameters.AddWithValue("@AgencyName", agencyName);
                cmd.Parameters.AddWithValue("@UserID", userId);

                if (!string.IsNullOrEmpty(profileImagePath))
                {
                    cmd.Parameters.AddWithValue("@ProfileImage", profileImagePath);
                }

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {


                    // Update profile image preview if a new image was uploaded

                }
                else
                {

                }
            }
        }
    }
}
