using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

public partial class Logout : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
       
        HttpContext.Current.Session.Clear();
        HttpContext.Current.Session.Abandon();

        if (Request.Cookies["ASP.NET_SessionId"] != null)
        {
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
        }

        if (Request.Cookies[".ASPXAUTH"] != null)
        {
            Response.Cookies[".ASPXAUTH"].Expires = DateTime.Now.AddDays(-1);
        }

        FormsAuthentication.SignOut();

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Expires = -1;
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));


      
        Response.Redirect("~/Login.aspx");
    }

}