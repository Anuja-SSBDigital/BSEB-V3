using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;      
using System.IO;             
using System.Linq;        
using System.Web;   
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;       
using System.Globalization;
using log4net;
using System.Configuration;


public partial class fileupload : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    private static readonly ILog log = LogManager.GetLogger(typeof(fileupload));

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                
                BindDocCategory();
                BindDocumentType();
             
             BindExamSessionType();
                btn_submit.Visible = false;
                div_fileupload.Visible = false;
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    private void BindDocCategory()
    {
        try   
        {
            FlureeCS fl = new FlureeCS();
            DataTable dt = fl.DocumentCategoryMaster();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddl_doctype.DataSource = dt;
                ddl_doctype.DataTextField = "DocCategoryName";
                ddl_doctype.DataValueField = "DocCategoryName";
                ddl_doctype.DataBind();
            }

            ddl_doctype.Items.Insert(0, new ListItem("Select Doc Category", "0"));
        }
        catch (Exception ex)
        {
            
        }
    }
    private void BindDocumentType()
    {
        try
        {
            FlureeCS fl = new FlureeCS();
            DataTable dt = fl.Documenttypemaster();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddl_sub_doc_type.DataSource = dt;
                ddl_sub_doc_type.DataTextField = "DocTypeName";
                ddl_sub_doc_type.DataValueField = "DocTypeName";
                ddl_sub_doc_type.DataBind();
            }

            ddl_sub_doc_type.Items.Insert(0, new ListItem("Select File Type", "0"));
        }
        catch (Exception ex)
        {
           
        }
    }

    private void BindExamSessionType()
    {
        try
        {
            FlureeCS fl = new FlureeCS();
            DataTable dt = fl.ExamSessionmaster();

            ddl_Examsession.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddl_Examsession.DataSource = dt;
                ddl_Examsession.DataTextField = "SessionName";
                ddl_Examsession.DataValueField = "SessionName";
                ddl_Examsession.DataBind();
            }

          
            ListItem exam26 = ddl_Examsession.Items.FindByValue("Exam-26");
            if (exam26 != null)
            {
                ddl_Examsession.ClearSelection();
                exam26.Selected = true;
            }
        }
        catch (Exception ex)      
        {
          
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

      

    protected void btn_submit_Click(object sender, EventArgs e)
    {      
        if (fl_file.HasFile)
        {
            try       
            {
                string clientIp = GetClientIp();
                List<string> allowedIps =fl. GetAllowedIPsFromDB().Select(ip => ip.Trim()).ToList();

              
                if (clientIp.StartsWith("::ffff:"))
                {
                    clientIp = clientIp.Replace("::ffff:", "");
                }

                log.Info("Client IP detected: " + clientIp);
                log.Info("Allowed IPs: " + string.Join(", ", allowedIps));

             
                if (!allowedIps.Contains(clientIp) && clientIp != "127.0.0.1")
                {
                    log.Info("Unauthorized upload attempt from IP: " + clientIp);
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", @"swal({ title: 'Access Denied!', text: 'You are not authorized to Upload files.', icon: 'error', button: 'OK' });", true);
                    return;
                }

              
                string examSession = ddl_Examsession.SelectedValue;
                if (string.IsNullOrEmpty(examSession))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "alert", "alert('Please select Exam Session');", true);
                    return;
                }

                string baseUploadFolder = Server.MapPath("~/Uploads/");
                //   string processFolder = Path.Combine(baseUploadFolder, "Process");

                string sessionFolder = Path.Combine(baseUploadFolder, examSession);

                if (!Directory.Exists(sessionFolder)) Directory.CreateDirectory(sessionFolder);

                string doctype = ddl_doctype.SelectedValue;
                string subdoctype = ddl_sub_doc_type.SelectedValue;

                string agency = Session["agencyname"].ToString();
                string username = Session["username"].ToString();

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileExtension = Path.GetExtension(fl_file.FileName).ToLower();

                string actualfilename = fl_file.FileName;

                //string doctypeFolder = Path.Combine(processFolder, doctype);

                string doctypeFolder = Path.Combine(sessionFolder, doctype);
                if (!Directory.Exists(doctypeFolder)) Directory.CreateDirectory(doctypeFolder);

                string subdoctypeFolder = Path.Combine(doctypeFolder, subdoctype);
                if (!Directory.Exists(subdoctypeFolder)) Directory.CreateDirectory(subdoctypeFolder);
                 
                string[] words = subdoctype.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string cleanedSubdoctype = "";

                if (words.Length > 3)
                {
                    cleanedSubdoctype = words[0] + "_" + words[1] + "_" + words[2] + "_" + words[3];
                }
                else if (words.Length > 2)
                {
                    cleanedSubdoctype = words[0] + "_" + words[1] + words[2];
                }
                else if (words.Length > 1)
                {
                    cleanedSubdoctype = words[0] + "_" + words[1];
                }
                else if (words.Length == 1)
                {
                    cleanedSubdoctype = words[0];
                }
                else      
                {
                    cleanedSubdoctype = subdoctype.Replace(" ", "");      
                }

                string filename = string.Concat(agency, "_Inter_", doctype, "_", cleanedSubdoctype, "_", timestamp, fileExtension);

                //string uploadRootPath = Path.Combine(baseUploadFolder, filename);
                //fl_file.SaveAs(uploadRootPath);

                //FileStream fs = File.OpenRead(uploadRootPath);
                //string hash = fl.SHA256CheckSum(fs);

                //string res = fl.Insertfilehash(actualfilename, filename, hash, agency, cleanedSubdoctype);

                string savedFilePath = Path.Combine(subdoctypeFolder, filename);
                fl_file.SaveAs(savedFilePath);

                //string dbFilePath = "Uploads/Process/" + doctype + "/" + subdoctype + "/" + filename;


                string dbFilePath = "Uploads/" + examSession + "/" + doctype + "/" + subdoctype + "/" + filename;

          
                using (FileStream fs = File.OpenRead(savedFilePath))
                {
                    string hash = fl.SHA256CheckSum(fs);
                    string res = fl.Insertfilehash(actualfilename, filename, hash, agency, cleanedSubdoctype);
                }

                string resfile = fl.InsertProcessFileDetails(filename, dbFilePath, agency, username);

                string resdatainst2 = fl.Insert_DownloadFileDetail(actualfilename, filename, dbFilePath, subdoctype, agency, "Process");
                   
                string userId = Session["username"].ToString();
                string agencyName = Session["agencyname"].ToString();
                string deviceUsed = Request.Browser.Type;
                   
                string reslog = fl.Insertactivitylog(userId, clientIp, deviceUsed, "upload", filename, agencyName);

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Your file " + filename + " has been uploaded successfully!');", true);
            }
            catch (Exception ex)
            {      
                string script = "alert('Error: " + ex.Message.Replace("'", "\\'") + "');";
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", script, true);
            }
        }                                               
        else
        {           
            Response.Write("<script>alert('Please select a file to upload.')</script>");
        }
             
    }       
                                                                                              
    private DataTable ReadExcelFile(string filePath)
    {
        DataTable dt = new DataTable();  
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });
                dt = ds.Tables[0]; 
            }
        }
        return dt;       
    }

    private DataTable ReadCsvFile(string filePath)
    {
        DataTable dt = new DataTable();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",  
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            MissingFieldFound = null
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Read();
            csv.ReadHeader();
                 
            foreach (var header in csv.HeaderRecord)
            {
                if (!dt.Columns.Contains(header))
                    dt.Columns.Add(header, typeof(string)); 
            }

            while (csv.Read())
            {
                DataRow row = dt.NewRow();

                foreach (DataColumn column in dt.Columns)
                {
                   
                    string fieldValue = csv.HeaderRecord.Contains(column.ColumnName) ? csv.GetField(column.ColumnName) : null;

                    Console.WriteLine("Column: " + column.ColumnName + ", Value: " + fieldValue);

                    row[column.ColumnName] = string.IsNullOrWhiteSpace(fieldValue) ? (object)DBNull.Value : (object)fieldValue.Trim();

                }

                dt.Rows.Add(row);
            }
        }

        return dt;
    }  
   
                                                                                                                                                                    
    private void SaveDataTableToCSV(DataTable dt, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
           
            writer.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

          
            foreach (DataRow row in dt.Rows)
            {
                writer.WriteLine(string.Join(",", row.ItemArray.Select(field => field.ToString().Replace(",", " "))));
            }
        }
    }         
    //    
    protected void btn_submittoken_Click(object sender, EventArgs e)
    {
    
        FlureeCS fl = new FlureeCS();
        string username = Session["Username"].ToString();
        string enteredKey = txt_pvtkey.Text.Trim();

        if (fl.IsPrivateKeyValidwithusername(username,enteredKey))  
        {
            lbl_validate.Text = "Key is Valid";
            lbl_validate.ForeColor = System.Drawing.Color.Green;
            btn_submit.Visible = true;
            div_fileupload.Visible = true;
        }
        else
        {
            lbl_validate.Text = "Key is Invalid or Expired";
            lbl_validate.ForeColor = System.Drawing.Color.Red;
            btn_submit.Visible = false;    
            div_fileupload.Visible = false;
        }
    }

}