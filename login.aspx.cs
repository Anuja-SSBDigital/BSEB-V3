using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

public partial class login : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)         
        {                                                   
            Session.Clear();
            Session.Abandon();            
            Session.RemoveAll();
            Session["userid"] = null;
        }
    }
    
                                                      
    protected void btn_submit_Click(object sender, EventArgs e)
    {


        // List<string> allowedIps = new List<string> { "115.243.18.60", "117.203.160.250", "103.90.159.37", "125.16.33.1", "152.58.187.74","152.58.189.90" };
        // string userIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        // if (string.IsNullOrEmpty(userIp))
        // {
        // userIp = Request.ServerVariables["REMOTE_ADDR"];
        // }

        // if (!allowedIps.Contains(userIp))
        // {
        // Response.Write("<script>alert('Access denied');</script>");
        // return;
        // }

        try
        {
               
            DataTable res = fl.CheckLogin(txt_UN.Text, txt_password.Text);
            if (res.Rows.Count > 0)
            {
                Session["userid"] = res.Rows[0]["id"].ToString();
                Session["username"] = res.Rows[0]["username"].ToString();     
                Session["role"] = res.Rows[0]["role"].ToString();
                Session["mobileno"] = res.Rows[0]["mobileno"].ToString();
                Session["agencyname"] = res.Rows[0]["agencyname"].ToString();

                if (Session["role"].ToString() == "Admin")
                {

                    Response.Redirect("~/Agency/AgencyAccess.aspx");
                }
                else
                {
                    Response.Redirect("~/Agency/fileupload.aspx");

                }

            }     
            else
            {       
                Response.Write("<script>alert('Invalid Username or Password');</script>");
            }

        }
        
        catch (WebException ex)
        {

          
            Response.Write("<script>alert('Error: " + ex + "');</script>");

        }
        catch (Exception ex)
        {

            Response.Write("<script>alert('Error: " + ex + "');</script>");

        }

    }
}