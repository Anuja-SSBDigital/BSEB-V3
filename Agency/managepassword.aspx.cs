using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_managepassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["username"] != null)
        {

            hfSessionUserId.Value = Session["UserId"].ToString();

        }
        else
        {
            Response.Redirect("../login.aspx");
        }
    }

    [WebMethod]
    public static string ChangePassword(string currentpassword, string newpassowrd, string confirmpassword, string userid)
    {
        FlureeCS fl = new FlureeCS();

        //if (fl.EncryptString(currentpassword) == currentpasswordsession)
        //{
        if (newpassowrd == confirmpassword)
        {
            string res = fl.ChangePassword(userid, fl.EncryptString(newpassowrd));
            if (res.StartsWith("Error"))
            {
                return fl.ToJson(new { message = "Password doesn't change. Please try again later." });
            }
            else
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.RemoveAll();
                HttpContext.Current.Session["userid"] = null;
                return fl.ToJson(new { message = "Password changed successfully." });
            }
        }
        else
        {
            return fl.ToJson(new { message = "*New Password and Confirm Password does not match." });

        }
        //}
        //else
        //{
        //    return fl.ToJson(new { message = "*Current Password Is Incorrect." });
        //}
    }
}