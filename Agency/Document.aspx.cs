using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Document2 : System.Web.UI.Page
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


            bool isAdded = fl.AddDocumentCategory(categoryName);


            ScriptManager.RegisterStartupScript(this, this.GetType(), "done",
                isAdded
                    ? "alert('Category added successfully!');"
                    : "alert('Failed to add category.');",
                true);

            if (isAdded)
            {
                txtCategoryName.Text = "";

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