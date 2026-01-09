using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_DocCategory : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
       if (!IsPostBack)
 {
     if (Session["userid"] != null)
     {
         string userRole = Session["role"] != null ? Session["role"].ToString() : "";


         if (userRole == "Admin")
         {
             div_search.Visible = true;
             Div_admin.Visible = true;
           
         }
         else
         {
            
             Response.Redirect("../login.aspx");

         }
     }
     else
     {

        
         Response.Redirect("../login.aspx");
     }
 }
    }
	
	
	  protected void btnAddCategory_Click(object sender, EventArgs e)
  {
      try
 {
    string categoryName = txtCategoryName.Text.Trim();
 
      if (string.IsNullOrEmpty(categoryName))
      {
          ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
              "alert('Please enter a category name.');", true);
          return;
      }
 
       string resultMessage = fl.AddDocumentCategory(txtCategoryName.Text);
 
       ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
           "alert('" + resultMessage + "');", true);
 
       if (resultMessage == "Category added successfully.")
       {
           txtCategoryName.Text = string.Empty;
       }
 }
 catch (Exception ex)
 {
     string safeMessage = ex.Message.Replace("'", "\\'");
     ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
         string.Format("alert('An error occurred: {0}');", safeMessage), true);
 }
  }
}