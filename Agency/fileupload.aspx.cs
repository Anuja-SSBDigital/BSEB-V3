using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using iTextSharp.text;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class fileupload : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    private static readonly ILog log = LogManager.GetLogger(typeof(fileupload));

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userid"] == null)
        {
            Response.Redirect("../login.aspx");
        }

        if (!IsPostBack)
        {
            BindDocCategory();
            BindExamSessionType();

            btn_submit.Visible = false;
            div_fileupload.Visible = false;
        }
        else
        {
           
            if (!string.IsNullOrEmpty(hfDoctypeId.Value))
            {
                ddl_doctype.SelectedValue = hfDoctypeId.Value;
                BindSubDocumentType(Convert.ToInt32(hfDoctypeId.Value));
                ddl_sub_doc_type.SelectedValue = hfSubdoctypeId.Value;
            }
        }
    }



    public class SubDocTypeVM
    {
        public int subdocId { get; set; }
        public string subdoctypename { get; set; }
    }

  
    [WebMethod]
    public static List<SubDocTypeVM> GetSubDocTypes(string doctypeId)
    {
        List<SubDocTypeVM> list = new List<SubDocTypeVM>();

        int docTypeId;
        if (!int.TryParse(doctypeId, out docTypeId))
            return list;

        FlureeCS fl = new FlureeCS();
        DataTable dt = fl.GetSubDocTypes(docTypeId, HttpContext.Current.Session["agencyname"].ToString());

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataRow row = dt.Rows[i];

            SubDocTypeVM vm = new SubDocTypeVM();
          //  vm.subdocId = Convert.ToInt32(row["subdocId"]);
            vm.subdoctypename = row["SubDocType"].ToString();

            list.Add(vm);
        }

        return list;
    }



    private void BindDocCategory()
    {
        string AgencyName = Session["agencyname"].ToString();

        DataTable dt = fl.DocumentCategoryMaster();

        // Clone structure
        DataTable filteredDt = dt.Clone();

        if (AgencyName.Equals("Hitech", StringComparison.OrdinalIgnoreCase))
        {
            // ONLY show Practical Printing & Theory Printing
            var rows = dt.AsEnumerable().Where(r => r.Field<string>("DocCategoryName") == "Practical Printing" ||  r.Field<string>("DocCategoryName") == "Theory Printing");

            if (rows.Any())
                filteredDt = rows.CopyToDataTable();
        }
        else if (AgencyName.Equals("SSB Digital", StringComparison.OrdinalIgnoreCase) || AgencyName.Equals("Antier", StringComparison.OrdinalIgnoreCase))
        {
            // ONLY show Practical Printing & Theory Printing
            var rows = dt.AsEnumerable().Where(r => r.Field<string>("DocCategoryName") == "Result Data");

            if (rows.Any())
                filteredDt = rows.CopyToDataTable();
        }
        else
        {
            // EXCLUDE Practical Printing & Theory Printing
            var rows = dt.AsEnumerable().Where(r => r.Field<string>("DocCategoryName") != "Practical Printing" && r.Field<string>("DocCategoryName") != "Theory Printing" && r.Field<string>("DocCategoryName") != "Result Data");

            if (rows.Any())
                filteredDt = rows.CopyToDataTable();
        }

        ddl_doctype.DataSource = filteredDt;
        ddl_doctype.DataTextField = "DocCategoryName";
        ddl_doctype.DataValueField = "doctypeId";
        ddl_doctype.DataBind();

        ddl_doctype.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Doc Category", "0"));

    }


    protected void ddl_doctype_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddl_sub_doc_type.Items.Clear();

        if (ddl_doctype.SelectedValue == "0")
        {
            ddl_sub_doc_type.Items.Insert(0,
                new System.Web.UI.WebControls.ListItem("Select File Type", "0"));
            return;
        }

        int doctypeId = Convert.ToInt32(ddl_doctype.SelectedValue);

        DataTable dt = fl.GetSubDocTypes(doctypeId, HttpContext.Current.Session["agencyname"].ToString());

        ddl_sub_doc_type.DataSource = dt;
        ddl_sub_doc_type.DataTextField = "SubDocType";
        ddl_sub_doc_type.DataValueField = "SubDocType";
        ddl_sub_doc_type.DataBind();

        ddl_sub_doc_type.Items.Insert(0,
            new System.Web.UI.WebControls.ListItem("Select File Type", "0"));
    }



    private void BindSubDocumentType(int doctypeId)
    {
        DataTable dt = fl.GetSubDocTypes(doctypeId, HttpContext.Current.Session["agencyname"].ToString());

        ddl_sub_doc_type.Items.Clear();

        if (dt != null && dt.Rows.Count > 0)
        {
            ddl_sub_doc_type.DataSource = dt;
            ddl_sub_doc_type.DataTextField = "SubDocType";
            ddl_sub_doc_type.DataValueField = "SubDocType";
            ddl_sub_doc_type.DataBind();
        }

        ddl_sub_doc_type.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select File Type", "0"));
    }

    private void BindExamSessionType()
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


                // Normalize IPv6-mapped IPv4
                if (clientIp.StartsWith("::ffff:"))
                {
                    clientIp = clientIp.Replace("::ffff:", "");
                }

                log.Info("Client IP detected: " + clientIp);

                // Check permission
                bool isAllowed = fl.IsActionAllowed(clientIp, "UPLOAD");

                log.Info("Is upload allowed: " + isAllowed);

                // Allow localhost for testing
                if (!isAllowed && clientIp != "127.0.0.1")

                {
                    log.Info("Unauthorized upload attempt from IP: " + clientIp);

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "alert",
                        @"swal({ 
            title: 'Access Denied!', 
            text: 'You are not authorized to upload files.', 
            icon: 'error', 
            button: 'OK' 
        });",
                        true
                    );
                    return;
                }

                string examSession = ddl_Examsession.SelectedValue;
                if (string.IsNullOrEmpty(examSession))
                {
                    ScriptManager.RegisterStartupScript(
    this,
    GetType(),
    "alert",
    "swal({ " +
    "title: 'Required!', " +
    "text: 'Please select Exam Session.', " +
    "icon: 'warning', " +
    "button: 'OK' " +
    "});",
    true
);
                    return;

                }

                string baseUploadFolder = Server.MapPath("~/Uploads/");
                //   string processFolder = Path.Combine(baseUploadFolder, "Process");

                string sessionFolder = Path.Combine(baseUploadFolder, examSession);

                if (!Directory.Exists(sessionFolder)) Directory.CreateDirectory(sessionFolder);

                string examsession = ddl_Examsession.SelectedValue;

                string doctypeId = hfDoctypeId.Value;
                string doctype = hfDoctypeText.Value.Trim();

                string subdoctypeId = hfSubdoctypeId.Value;
                string subdoctype = hfSubdoctypeText.Value.Trim();

                string remark = txtRemark.Text.Trim();


                if (string.IsNullOrWhiteSpace(remark))
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "remarkAlert",
                        "swal({ " +
                        "title: 'Required!', " +
                        "text: 'Remark is mandatory for file upload.', " +
                        "icon: 'warning', " +
                        "button: 'OK' " +
                        "});",
                        true
                    );
                    return;
                }


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
                    cleanedSubdoctype = words[0]  + words[1]  + words[2]  + words[3];
                }
                else if (words.Length > 2)
                {
                    cleanedSubdoctype = words[0]  + words[1] + words[2];
                }
                else if (words.Length > 1)
                {
                    cleanedSubdoctype = words[0]  + words[1];
                }


                else if (words.Length == 1)
                {
                    cleanedSubdoctype = words[0];
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

                string savedFilePath = Path.Combine(subdoctypeFolder, filename);
                fl_file.SaveAs(savedFilePath);

                FileStream fs = File.OpenRead(savedFilePath);
                string hash = fl.SHA256CheckSum(fs);

                string res = fl.Insertfilehash(actualfilename, filename, hash, agency, cleanedSubdoctype);
                //string dbFilePath = "Uploads/Process/" + doctype + "/" + subdoctype + "/" + filename;


                string dbFilePath = "Uploads/" + examSession + "/" + doctype + "/" + subdoctype + "/" + filename;

                string resfile = fl.InsertProcessFileDetails(filename, dbFilePath, agency, username);

                string resdatainst2 = fl.Insert_DownloadFileDetail(actualfilename, filename, dbFilePath, examsession, doctype, subdoctype, agency, "", remark);

                string userId = Session["username"].ToString();
                string agencyName = Session["agencyname"].ToString();
                string deviceUsed = Request.Browser.Type;

                string reslog = fl.Insertactivitylog(userId, clientIp, deviceUsed, "upload", filename, agencyName);

                //               ClientScript.RegisterStartupScript(
                //    this.GetType(),
                //    "alert",
                //    "swal({ " +
                //    "title: 'Success!', " +
                //    "text: 'Your file " + filename + " has been uploaded successfully!', " +
                //    "icon: 'success', " +
                //    "button: 'OK' " +
                //    "});",
                //    true
                //);
                //               return;

                string script = "swal({ " +
    "title: 'Success!', " +
    "text: 'Your file " + filename + " has been uploaded successfully!', " +
    "icon: 'success', " +
    "button: 'OK' " +
"}).then(function(value) { " +

    "$('#" + txtRemark.ClientID + "').val(''); " +

"});";

                ClientScript.RegisterStartupScript(this.GetType(), "successAlert", script, true);

            }

            catch (Exception ex)
            {
                string errorMessage = ex.Message.Replace("'", "\\'");

                string script =
                    "swal({ " +
                    "title: 'Error!', " +
                    "text: 'Error: " + errorMessage + "', " +
                    "icon: 'error', " +
                    "button: 'OK' " +
                    "});";

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "errorAlert",
                    script,
                    true
                );
                return;

            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(
    this,
    GetType(),
    "fileAlert",
    "swal({ title: 'Warning!', text: 'Please select a file to upload.', icon: 'warning', button: 'OK' });",
    true
);
            return;



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

    protected void btn_submittoken_Click(object sender, EventArgs e)
    {

        FlureeCS fl = new FlureeCS();
        string username = Session["Username"].ToString();
        string enteredKey = txt_pvtkey.Text.Trim();

        if (fl.IsPrivateKeyValidwithusername(username, enteredKey))
        {
            lbl_validate.Text = "Key is Valid";
            lbl_validate.ForeColor = System.Drawing.Color.Green;
            btn_submit.Visible = true;
            div_fileupload.Visible = true;

            div_remarks.Visible = true;
        }
        else
        {
            lbl_validate.Text = "Key is Invalid or Expired";
            lbl_validate.ForeColor = System.Drawing.Color.Red;
            btn_submit.Visible = false;
            div_fileupload.Visible = false;
            div_remarks.Visible = false;
        }
    }


}