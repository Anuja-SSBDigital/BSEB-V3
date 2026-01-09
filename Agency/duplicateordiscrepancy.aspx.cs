using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_duplicateordiscrepancy : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                getfiles();
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    public void getfiles()
    {
        string loggedInAgency = Session["agencyname"].ToString();



        DataTable res = fl.getdownfiledetailspending(loggedInAgency);
        if (res != null && res.Rows.Count > 0)
        {

            DataTable filteredTable = res.Clone();
            foreach (DataRow row in res.Rows)
            {
                string filePath = row["FilePath"].ToString();
                if (filePath.StartsWith("Uploads/Pending/", StringComparison.OrdinalIgnoreCase))
                {
                    filteredTable.ImportRow(row);
                }
            }

            rptCSVFiles.DataSource = filteredTable;
            rptCSVFiles.DataBind();
        }
        else
        {
            rptCSVFiles.DataSource = null;
            rptCSVFiles.DataBind();
        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        try
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            HiddenField hfId = (HiddenField)item.FindControl("hf_id");
            int fileId = Convert.ToInt32(hfId.Value);

            string relativeFilePath = btn.CommandArgument;


            if (!relativeFilePath.StartsWith("Uploads/Pending/", StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('❌ Access denied: Invalid file location.');", true);
                return;
            }

            string fullFilePath = Server.MapPath("~/" + relativeFilePath);

            if (File.Exists(fullFilePath))
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fullFilePath));
                Response.TransmitFile(fullFilePath);
                Response.Flush();
                // fl.updatedownloadetailstable(fileId);
                Response.End();
            }
            else
            {
                //ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('❌ File not found: " + fullFilePath + "');", true);
                string fileName = System.IO.Path.GetFileName(fullFilePath);

                string script = @"
    swal({
        title: 'File not found',
        text: '" + fileName.Replace("\\", "\\\\") + @"',
        icon: 'error',
        button: 'OK'
    });";

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('" + ex.Message + "');", true);
        }
    }
}
