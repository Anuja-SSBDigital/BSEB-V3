using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_filedownload : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    private static readonly ILog log = LogManager.GetLogger(typeof(Agency_filedownload));
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
            //LoadCSVFiles();
        }
    }

    public string GetClientIp()
    {
        //string ip = "192.168.1.100";
        string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ip))
        {
            string[] ipArray = ip.Split(',');
            ip = ipArray[0].Trim(); // Get the first forwarded IP
        }
        else
        {
            ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        // ✅ Handle Localhost IPv6 (::1) and convert it to 127.0.0.1
        if (ip == "::1" || string.IsNullOrEmpty(ip))
        {
            ip = "127.0.0.1";
        }

        return ip;
    }

    public void getfiles()
    {
        //string loggedInAgency = Session["agencyname"].ToString();
        //string targetAgency = string.Empty;
        //if (loggedInAgency == "Mapple")
        //{
        //    targetAgency = "Datacon";
        //}
        //else if (loggedInAgency == "Datacon")
        //{
        //    targetAgency = "Mapple";
        //}

        //DataTable res = fl.getdownfiledetails();
        //if (res != null && res.Rows.Count > 0)
        //{

        //    rptCSVFiles.DataSource = res;
        //    rptCSVFiles.DataBind();
        //}
        //else
        //{
        //    rptCSVFiles.DataSource = null;
        //    rptCSVFiles.DataBind();
        //}


        DataTable res = fl.getdownfiledetails();
        if (res != null && res.Rows.Count > 0)
        {
            string loggedInAgency = Session["agencyname"].ToString(); // Get the logged-in agency name

            // Create a new DataTable to store filtered results
            DataTable filteredRes = res.Clone();

            foreach (DataRow row in res.Rows)
            {
                string subdoctype = row["subdoctype"].ToString();
                string fileAgency = row["agency"].ToString();

                // Logic to show cross files based on subdoctypes
                if ((loggedInAgency == "Antier") && (subdoctype == "Admit Card Master" || subdoctype== "3 year Carry Data"))
                {
                    filteredRes.ImportRow(row);
                }

            }

            if (filteredRes.Rows.Count > 0)
            {
                rptCSVFiles.DataSource = filteredRes;
                rptCSVFiles.DataBind();

            }
            else
            {
                rptCSVFiles.DataSource = null;
                rptCSVFiles.DataBind();
            }
        }
        else
        {
            rptCSVFiles.DataSource = null;
            rptCSVFiles.DataBind();
        }

    }


    protected void btnDownload_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    // Get the clicked button
        //    Button btn = (Button)sender;
        //    RepeaterItem item = (RepeaterItem)btn.NamingContainer; // Get parent RepeaterItem

        //    // Retrieve ID from HiddenField
        //    HiddenField hfId = (HiddenField)item.FindControl("hf_id");
        //    int fileId = Convert.ToInt32(hfId.Value); // Extract File ID

        //    // Retrieve File Path from CommandArgument
        //    string relativeFilePath = btn.CommandArgument;
        //    string fullFilePath = Server.MapPath("~/" + relativeFilePath);// Get physical path

        //    // ✅ Step 1: Check if file exists before downloading
        //    if (File.Exists(fullFilePath))
        //    {
        //        // ✅ Download file
        //        Response.Clear();
        //        Response.ContentType = "application/octet-stream";
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fullFilePath));
        //        Response.TransmitFile(fullFilePath);
        //        Response.Flush();

        //        // ✅ Update Download Status
        //        string resultMessage = fl.updatedownloadetailstable(fileId);

        //        // ✅ Show Success Message
        //        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('" + resultMessage + "');", true);
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('❌ File not found: " + fullFilePath + "');", true);
        //    }
        //}
        //catch (Exception ex)
        //{

        //    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('" + ex.Message + "');", true);
        //}

        try
        {

            List<string> allowedIps = new List<string> { "115.243.18.60", "117.203.160.250","192.168.27.30" };

            string clientIp = GetClientIp();

            // ✅ Check if client IP is in the allowed list
            if (!allowedIps.Contains(clientIp))
            {

                log.Info("Unauthorized download attempt from IP: " + clientIp);
                string script = @"
    swal({
        title: 'Access Denied!',
        text: 'You are not authorized to download files.',
        icon: 'error',
        button: 'OK'
    });";

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);

                return;
                //ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                //    "alert('You are not authorized to download files.');", true);
                //return;
            }

            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer; // Get parent RepeaterItem

            // ✅ Retrieve File ID from HiddenField
            HiddenField hfId = (HiddenField)item.FindControl("hf_id");
            int fileId = Convert.ToInt32(hfId.Value); // Extract File ID

            // ✅ Retrieve File Path from CommandArgument
            string relativeFilePath = btn.CommandArgument;
            string fullFilePath = Server.MapPath("~/" + relativeFilePath); // Convert to absolute path

            // ✅ Step 1: Check if file exists before downloading
            if (File.Exists(fullFilePath))
            {
                string fileName = Path.GetFileName(fullFilePath);
                string fileExtension = Path.GetExtension(fullFilePath).ToLower();

                // ✅ Determine MIME Type based on file extension
                string contentType = "application/octet-stream"; // Default
                switch (fileExtension)
                {
                    case ".csv":
                        contentType = "text/csv";
                        break;
                    case ".xls":
                        contentType = "application/vnd.ms-excel";
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                }

                string userId = Session["username"].ToString();
                string agencyName = Session["agencyname"].ToString();
                string deviceUsed = Request.Browser.Type;

                // ✅ Log Activity


                // ✅ Download file
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
                Response.TransmitFile(fullFilePath);
                Response.Flush();
                Response.End();

            }
            else
            {

                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('❌ File not found: " + fullFilePath + "');", true);

            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                "alert('❌ Error downloading file: " + ex.Message + "');", true);
        }

    }


}