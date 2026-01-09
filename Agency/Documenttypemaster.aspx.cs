using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_DocumentTypeMaster : System.Web.UI.Page
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
 
           
           string result = fl.AddDocumentType(categoryName);
 
          
           ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
               "alert('" + result.Replace("'", "\\'") + "');", true);
 
          
           if (result.ToLower().Contains("success"))
           {
               txtCategoryName.Text = string.Empty;
           }
       }
       catch (Exception ex)
       {
           string safeMsg = ex.Message.Replace("'", "\\'");
           ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
               "alert('Error: " + safeMsg + "');", true);
       }
    }


    protected void btnsearch_Click(object sender, EventArgs e)
    {
        BindDocumentCategoryData();
    }

    private void BindDocumentCategoryData()
    {
        string status = ddl_Status.SelectedValue;
        DataTable dt = fl.GetDocumentTypeData(status);

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
        }
    }

    protected void rpt_DocumentTypeData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int docId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "ToggleStatus")
        {
            bool currentStatus = fl.GetDocumentTypeStatus(docId);
            bool newStatus = !currentStatus;

            if (fl.UpdateDocumentTypeStatus(docId, newStatus))
            {
                lblMessage.Text = newStatus ? "Record activated successfully." : "Record deactivated successfully.";
            }
            else
            {
                lblMessage.Text = "Operation failed!";
            }

            BindDocumentCategoryData();
        }
        else if (e.CommandName == "EditDoc")
        {
            Response.Redirect("Editdocumenttypedetail.aspx?DocId=" + docId);
        }
    }
}