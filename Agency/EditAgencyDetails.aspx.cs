using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

public partial class Agency_EditAgencyDetails : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
   string conStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Session["userid"] != null &&
                Session["role"] != null &&
                Session["role"].ToString() != "Agency")
            {

                if (Request.QueryString["id"] != null)
                {
                    LoadUser();
                }

            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    private void LoadUser()
    {
        string id = Request.QueryString["id"];

        using (SqlConnection con = new SqlConnection(conStr))
        {

            SqlCommand cmd = new SqlCommand("SELECT * FROM agencyuser WHERE id=@id", con);

            cmd.Parameters.AddWithValue("@id", id);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {

                txtAgencyName.Text = dr["agencyname"].ToString();
                txtUsername.Text = dr["username"].ToString();
                txtEmail.Text = dr["email"].ToString();
                txtMobile.Text = dr["mobileno"].ToString();
            
                txtPlainPassword.Text = dr["PlainTextPassword"].ToString();
                ddlAgencyType.SelectedValue = dr["AgencyType"].ToString();
                ddlStatus.SelectedValue = dr["status"].ToString();
                txtRole.Text = dr["role"].ToString();
                txtPrivateKey.Text = dr["PrivateKey"].ToString();

                if (dr["Key_Expiry"] != DBNull.Value)
                {
                    txtKeyExpiry.Text = Convert.ToDateTime(dr["Key_Expiry"]).ToString("yyyy-MM-ddTHH:mm");
                }

            }

            con.Close();

        }

    }

 
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string id = Request.QueryString["id"];

        string res = fl.UpdateAgencyUser(
            id,
            txtUsername.Text.Trim(),
            txtEmail.Text.Trim(),
            txtMobile.Text.Trim(),
            txtAgencyName.Text.Trim(),
            txtPlainPassword.Text.Trim(),
            ddlAgencyType.SelectedValue,
            ddlStatus.SelectedValue
        );

        if (!res.StartsWith("Error"))
        {
            Response.Write("<script>alert('Agency Updated Successfully');window.location='Agencymaster.aspx';</script>");
        }
        else
        {
            Response.Write("<script>alert('" + res.Replace("'", "") + "');</script>");
        }
    }

 
}