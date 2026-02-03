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
            if (Session["userid"] == null)
            {
                Response.Redirect("../login.aspx");
                return;
            }

            string userRole = Session["role"] != null ? Session["role"].ToString() : "";

            if (userRole != "Admin")
            {
                Response.Redirect("../login.aspx");
                return;
            }

            div_search.Visible = true;
            Div_admin.Visible = true;

            BindDocCategory();          
         //   BindDocumentCategoryData(); 
        }
    }

   
    private void BindDocCategory()
    {
        DataTable dt = fl.DocumentCategoryMaster();

        ddlDocType.DataSource = dt;
        ddlDocType.DataTextField = "DocCategoryName"; 
        ddlDocType.DataValueField = "doctypeId";      
        ddlDocType.DataBind();

        ddlDocType.Items.Insert(0,
            new ListItem("-- Select Document Category --", ""));
    }

   
    protected void btnAddCategory_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ddlDocType.SelectedValue))
            {
                Alert("Please select document category.");
                return;
            }

            string subDocName = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(subDocName))
            {
                Alert("Please enter document type name.");
                return;
            }

            int doctypeId = Convert.ToInt32(ddlDocType.SelectedValue);

            string result = fl.AddDocumentType(doctypeId, subDocName);

            Alert(result);

            if (result.ToLower().Contains("success"))
            {
                txtCategoryName.Text = "";
                BindDocumentCategoryData();
            }
        }
        catch (Exception ex)
        {
            Alert("Error : " + ex.Message);
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

        rpt_DocumentTypeData.DataSource = dt;
        rpt_DocumentTypeData.DataBind();

        lblMessage.Text = dt.Rows.Count == 0 ? "No records found." : "";
    }

   
    protected void rpt_DocumentTypeData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int subdocId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "ToggleStatus")
        {
            bool currentStatus = fl.GetDocumentTypeStatus(subdocId);
            bool newStatus = !currentStatus;

            if (fl.UpdateDocumentTypeStatus(subdocId, newStatus))
            {
                lblMessage.Text = newStatus
                    ? "Record activated successfully."
                    : "Record deactivated successfully.";
            }
            else
            {
                lblMessage.Text = "Operation failed!";
            }

            BindDocumentCategoryData();
        }
    }

   
    private void Alert(string msg)
    {
        ScriptManager.RegisterStartupScript(this, GetType(),
            "alert", "alert('" + msg.Replace("'", "\\'") + "');", true);
    }
}
