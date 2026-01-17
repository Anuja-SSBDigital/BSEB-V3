using MySql.Data.MySqlClient.Memcached;
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
using log4net;
            
public partial class ProcessedCSVFiles : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
    private static readonly ILog log = LogManager.GetLogger(typeof(ProcessedCSVFiles));

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {     
          
         
            if (Session["userid"] != null)
            {
               
                LoadFilteredData();
                
            }
            else
            {
                Response.Redirect("../login.aspx");

            }
          
        }
    }

    public string GetClientIp()
    {
        string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ip))
        {
           
            string[] ipArray = ip.Split(',');
            ip = ipArray[0].Trim();
        }
        else
        {
            ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        if (string.IsNullOrEmpty(ip))
        {
            ip = "127.0.0.1"; 
        }

    
        if (ip == "::1")
        {
            ip = "127.0.0.1";
        }

    
        if (ip.StartsWith("::ffff:"))
        {
            ip = ip.Replace("::ffff:", "");
        }

        ip = ip.Trim(); 

        return ip;
    }
    private void LoadFilteredData()
    {

        string loggedInAgency = Session["agencyname"].ToString() ?? "";
        DataTable res = fl.GetAccessibleFiles(loggedInAgency);
        if (res != null && res.Rows.Count > 0)
        {
           
            Dictionary<string, DataTable> tabData = new Dictionary<string, DataTable>
                        {
                            { "Datacon", res.Clone() },
                            { "Kids", res.Clone() },
                            { "MCRK", res.Clone() },
                            { "Mapple", res.Clone() },
                            { "Charu_Mindworks", res.Clone() },
                            { "Shree_Jagannath_Udyog", res.Clone() },
                            { "Hitech", res.Clone() },
                            { "Keltron", res.Clone() },
                            { "Datasoft", res.Clone() },
                            { "Antier", res.Clone() },
                            { "SSBDigital", res.Clone() }
                        };
                 
    tabData["SSB Digital"] = tabData["SSBDigital"];
	 tabData["Charu Mindworks"] = tabData["Charu_Mindworks"];
 tabData["Shree Jagannath Udyog"] = tabData["Shree_Jagannath_Udyog"];
            HashSet<DataRow> processedFiles = new HashSet<DataRow>();

            foreach (DataRow row in res.Rows)
            {
               
                string agencyName = row["agency"].ToString();

                if (tabData.ContainsKey(agencyName))
                {
                   
                    tabData[agencyName].ImportRow(row);
                }

            }                

           
            BindRepeater(rptDatacon, tabData["Datacon"]);
            BindRepeater(rptKids, tabData["Kids"]);
            BindRepeater(rptMCRK, tabData["MCRK"]);
            BindRepeater(rptMapple, tabData["Mapple"]);
            BindRepeater(RepeaterCharu_Mindworks, tabData["Charu_Mindworks"]);
            BindRepeater(RepeaterShree_Jagannath_Udyog, tabData["Shree_Jagannath_Udyog"]);
            BindRepeater(RepeaterHitech, tabData["Hitech"]);
            BindRepeater(RepeaterKeltronAG, tabData["Keltron"]);
            BindRepeater(RepeaterDatasoft_ag, tabData["Datasoft"]);
            BindRepeater(RepeaterAntier_ag, tabData["Antier"]);
            BindRepeater(RepeaterSSBDigital_ag, tabData["SSBDigital"]);
        }
    }

   
    private void BindRepeater(Repeater rpt, DataTable dt)
    {
        rpt.DataSource = dt.Rows.Count > 0 ? dt : null;
        rpt.DataBind();
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {

        try
        {
            //string clientIp = GetClientIp();
            //List<string> allowedIps = GetAllowedIPsFromDB().Select(ip => ip.Trim()).ToList();


            //if (clientIp.StartsWith("::ffff:"))
            //{
            //    clientIp = clientIp.Replace("::ffff:", "");
            //}

            //log.Info("Client IP detected: " + clientIp);
            //log.Info("Allowed IPs: " + string.Join(", ", allowedIps));


            //if (!allowedIps.Contains(clientIp) && clientIp != "127.0.0.1")
            //{
            //    {
            //        log.Info("Unauthorized download attempt from IP: " + clientIp);
            //        string script = @"
            //    swal({
            //        title: 'Access Denied!',
            //        text: 'You are not authorized to download files.',
            //        icon: 'error',
            //       button: 'OK'
            //    });";

            //        ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);

            //        return;
            //    }
            //}

            string clientIp = GetClientIp();


            // Normalize IPv6-mapped IPv4
            if (clientIp.StartsWith("::ffff:"))
            {
                clientIp = clientIp.Replace("::ffff:", "");
            }

            log.Info("Client IP detected: " + clientIp);

            // Check permission
            bool isAllowed = fl.IsActionAllowed(clientIp, "DOWNLOAD");

            log.Info("Is download allowed: " + isAllowed);

            // Allow localhost for testing
           if (!isAllowed && clientIp != "127.0.0.1")
            {
                log.Info("Unauthorized Download attempt from IP: " + clientIp);

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "alert",
                    @"swal({ 
            title: 'Access Denied!', 
            text: 'You are not authorized to Download files.', 
            icon: 'error', 
            button: 'OK' 
        });",
                    true
                );
                return;
            }

            //Button btn = (Button)sender;
            LinkButton btn = (LinkButton)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

          
            HiddenField hfId = (HiddenField)item.FindControl("hf_id");
            int fileId = Convert.ToInt32(hfId.Value); 

          
            string relativeFilePath = btn.CommandArgument;
            string fullFilePath = Server.MapPath("~/" + relativeFilePath); 

          
            if (File.Exists(fullFilePath))
            {
                string fileName = Path.GetFileName(fullFilePath);
                string fileExtension = Path.GetExtension(fullFilePath).ToLower();

               
                string contentType = "application/octet-stream"; 
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

              
                string reslog = fl.Insertactivitylog(userId, clientIp, deviceUsed, "download", fileName, agencyName);
                log.Info("Activity Log Inserted: " + reslog);

                
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
                Response.TransmitFile(fullFilePath);
                Response.Flush();
                Response.End();

            }
            else
            {
                log.Warn("File not found: " + fullFilePath);
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('❌ File not found: " + fullFilePath + "');", true);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error downloading file: " + ex.Message, ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                "alert('❌ Error downloading file: " + ex.Message + "');", true);
        }

    }
        
                                                                                 
    private List<string> GetAllowedIPsFromDB()
    {
        List<string> ips = new List<string>();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT IPNumber FROM IPMaster WHERE IsActive = 1";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();     
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ips.Add(reader["IPNumber"].ToString().Trim());
                }
            }
        }

        return ips;
    }

    protected void rptMapple_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
           
            string loggedInAgency = Session["agencyname"].ToString();

          
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

         
            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");

           
            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void rptDatacon_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
          
            string loggedInAgency = Session["agencyname"].ToString();

         
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void rptKids_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void rptMCRK_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
           
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void RepeaterCharu_Mindworks_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
           
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void RepeaterShree_Jagannath_Udyog_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void RepeaterHitech_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
          
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }
               
    protected void RepeaterKeltronAG_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
              
            string loggedInAgency = Session["agencyname"].ToString();

        
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }        

    protected void RepeaterDatasoft_ag_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            
            string loggedInAgency = Session["agencyname"].ToString();

          
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void RepeaterAntier_ag_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
          
            string loggedInAgency = Session["agencyname"].ToString();

           
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();


            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");


            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }

    protected void RepeaterSSBDigital_ag_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            LinkButton btnDownload = (LinkButton)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }
}