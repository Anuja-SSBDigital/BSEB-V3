using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Agencymaster : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e) 
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
              //  BindAgencyDropdown();
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }
                                                                                                     
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

            if (string.IsNullOrEmpty(ddlagency_type.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "typeError",
                    "alert('Please select Agency Type.');",
                    true);
                return;
            }

            string plainPassword = GeneratePassword();

          
            string hashedPassword = EncryptString(plainPassword);

            using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO agencyuser
            (username, email, mobileno, agencyname, password, role,
             status, created_at, updated_at, PrivateKey, Key_Expiry, PlainTextPassword,AgencyType)
            VALUES
            (@username, @email, @mobileno, @agencyname, @password, @role,
             'Active', GETDATE(), GETDATE(), '', NULL, @PlainTextPassword, @AgencyType)", con))
            {
                cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@mobileno", txtMobile.Text.Trim());
                cmd.Parameters.AddWithValue("@agencyname", txtAgency.Text.Trim());
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@PlainTextPassword", plainPassword);
                cmd.Parameters.AddWithValue("@role", "Agency");

                cmd.Parameters.AddWithValue("@AgencyType", ddlagency_type.SelectedValue);


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

    protected void btn_search_Click(object sender, EventArgs e)
    {
        //DataTable resforuser = fl.FindAgency(ddlOwnerAgency.SelectedValue);

        DataTable resforuser = fl.FindAgencystatus(ddl_Agencytatus.SelectedValue);


        if (resforuser.Rows.Count > 0)
        {
            User_detailes.Visible = true;
            rpt_userData.DataSource = resforuser;
            rpt_userData.DataBind();
        }
        else
        {
            rpt_userData.DataSource = null;
            rpt_userData.DataBind();
        }
    }


    //private void BindAgencyDropdown()
    //{
    //    string conStr = ConfigurationManager
    //                    .ConnectionStrings["dbcon"]
    //                    .ConnectionString;

    //    using (SqlConnection con = new SqlConnection(conStr))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(
    //            @"SELECT DISTINCT LTRIM(RTRIM(agencyname)) AS agencyname
    //          FROM agencyuser
    //          WHERE agencyname IS NOT NULL
    //          GROUP BY LTRIM(RTRIM(agencyname))
    //          ORDER BY LTRIM(RTRIM(agencyname))", con))
    //        {
    //            con.Open();

    //            ddlOwnerAgency.DataSource = cmd.ExecuteReader();
    //            ddlOwnerAgency.DataTextField = "agencyname";
    //            ddlOwnerAgency.DataValueField = "agencyname";
    //            ddlOwnerAgency.DataBind();
    //        }
    //    }
    //}

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

        }
        else if (e.CommandName == "link_rejected")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "Rejected");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('Agency Rejected Successfully');location.href = location.href;</script>");
            }
        }
        else if (e.CommandName == "link_Active")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "Active");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('Agency Status Changed Successfully');location.href = location.href;</script>");
            }
        }
        else if (e.CommandName == "link_DeActive")
        {
            string res = fl.Updateagencyuserstatus(e.CommandArgument.ToString(), "DeActive");
            if (!res.StartsWith("Error"))
            {
                fl.log.Info("Status Changed Successfully");
                Response.Write("<script>alert('Agency Status Changed Successfully');location.href = location.href;</script>");
            }
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
