using System;
using System.Collections.Generic;
using System.Data;
using System.Web;   
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_Ip : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
             
                if (Session["userid"] != null)
                {
                    string userRole = Session["role"] != null ? Session["role"].ToString() : string.Empty;

                    if (userRole == "Admin")
                    {
                        div_search.Visible = true;
                    
                    }
                    else
                    {
                        Response.Redirect("../login.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect("../login.aspx", false);
                }
            }
        }
        catch (Exception ex)
        {
          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('An unexpected error occurred during page load.');", true);
        }
    }


                                              
    protected void ChkBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkBox = sender as CheckBox;
        if (chkBox != null)
        {
          
            lblAddMessage.Text = chkBox.Text + " changed → Checked = " + chkBox.Checked.ToString();
        }
    }

    protected void btnAddIP_Click(object sender, EventArgs e)
    {
        string ipNumber = txtIPNumber.Text.Trim();
        string agencyName = txtAgencyName.Text.Trim();
        string updatedBy = Session["username"] != null ? Session["username"].ToString() : "System";

        bool canProcessCSV = hdnProcessCSV.Value == "true";
        bool canFileUpload = hdnFileUpload.Value == "true";
      

        if (string.IsNullOrEmpty(ipNumber))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('Please enter an IP address.');", true);
            return;
        }

        if (!canProcessCSV && !canFileUpload)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('Please select at least one access type.');", true);
            return;
        }
         
        string resultMessage = fl.InsertIP(ipNumber, agencyName, updatedBy,
                                             canProcessCSV, canFileUpload);

        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
            "alert('" + resultMessage.Replace("'", "\\'") + "');", true);

        if (resultMessage == "IP added successfully.")
        {
            txtIPNumber.Text = string.Empty;
            txtAgencyName.Text = string.Empty;
            hdnProcessCSV.Value = "false";
            hdnFileUpload.Value = "false";
           
        }
    }

        
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindIPData();
    }

    private void BindIPData()
    {
        string statusFilter = ddl_Status.SelectedValue;
        DataTable dt = fl.GetIPList(statusFilter);

        rpt_IPData.DataSource = dt;
        rpt_IPData.DataBind();

    }

    protected void rpt_IPData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int ipId = Convert.ToInt32(e.CommandArgument);
        string updatedBy = Session["username"] != null ? Session["username"].ToString() : "System";

        if (e.CommandName == "ToggleStatus")
        {
            bool toggled = fl.ToggleIPStatus(ipId, updatedBy);
            if (toggled)
                BindIPData();
        }
        else if (e.CommandName == "EditIP")
        {
            Response.Redirect("Editipdetails.aspx?IPID=" + ipId);
        }
    }
}