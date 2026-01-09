using System;
using System.Data;
using System.Web.UI;

public partial class Agency_Editipdetails : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    { 
        if (!IsPostBack)
        {
            if (Request.QueryString["IPID"] != null)
            {
                int ipId = Convert.ToInt32(Request.QueryString["IPID"]);
                LoadIPData(ipId);
            }
            else        
            {
                Response.Redirect("Ip.aspx");
            }
        }
    }

    private void LoadIPData(int ipId)
    {
        DataTable dt = fl.GetIPById(ipId);

        if (dt.Rows.Count > 0)
        {   
            DataRow row = dt.Rows[0];

            txtEditIP.Text = row["IPNumber"].ToString();
            txtAgencyName.Text = row["AgencyName"] != DBNull.Value ? row["AgencyName"].ToString() : "";

           
            chkProcessCSV.Checked = Convert.ToBoolean(row["CanProcessCSV"]);
            chkFileUpload.Checked = Convert.ToBoolean(row["CanUpload"]);
          

            hfIPID.Value = ipId.ToString();
        }
        else
        {
            Response.Redirect("Ip.aspx");
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        int ipId = Convert.ToInt32(hfIPID.Value);
        string newIP = txtEditIP.Text.Trim();
        string agencyName = txtAgencyName.Text.Trim();
        string updatedBy = Session["username"] != null ? Session["username"].ToString() : "System";

        if (string.IsNullOrEmpty(newIP))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('IP address cannot be empty.');", true);
            return;
        }

        bool canProcessCSV = chkProcessCSV.Checked;
        bool canFileUpload = chkFileUpload.Checked;
       
       
        bool updated = fl.UpdateIP(ipId, newIP, agencyName, updatedBy,
                                     canProcessCSV, canFileUpload);

        string message = updated ? "Records updated successfully!" : "No records were updated.";
        string redirectUrl = "Ip.aspx";

        string script = string.Format(@"
            alert('{0}');
            setTimeout(function() {{
                window.location.href = '{1}';
            }}, 1000);", message.Replace("'", "\\'"), redirectUrl);

        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertRedirect", script, true);
    }
}