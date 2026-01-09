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
                // BindSubjectDropdown();
                btn_submit.Visible = false;
                div_fileupload.Visible = false;
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
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
    protected void btn_submit_Click(object sender, EventArgs e)
    {


        if (fl_file.HasFile)
        {
            try
            {
                List<string> allowedIps = new List<string> { "115.243.18.60", "117.203.160.250", "103.90.159.37", "125.16.33.1", "152.58.187.74","152.58.130.209","152.58.154.108","103.90.159.37","223.184.134.172","45.114.57.242" };

                string clientIp = GetClientIp();

                if (!allowedIps.Contains(clientIp))
                {
                    log.Info("Unauthorized upload attempt from IP: " + clientIp);
                    string script = @"
    swal({
        title: 'Access Denied!',
        text: 'You are not authorized to Upload files.',
        icon: 'error',
        button: 'OK'
    });";

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);

                    return;
                    //ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    //    "alert('You are not authorized to upload files.');", true);

                }

                string baseUploadFolder = Server.MapPath("~/Uploads/");
                string processFolder = Path.Combine(baseUploadFolder, "Process");

                if (!Directory.Exists(processFolder)) Directory.CreateDirectory(processFolder);

                string doctype = ddl_doctype.SelectedValue;
                string subdoctype = ddl_sub_doc_type.SelectedValue;

                string agency = Session["agencyname"].ToString();
                string username = Session["username"].ToString();

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileExtension = Path.GetExtension(fl_file.FileName).ToLower();

                string actualfilename = fl_file.FileName;

                string doctypeFolder = Path.Combine(processFolder, doctype);
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


                //string cleanedSubdoctype = subdoctype.Replace(" ", "");
                string filename = string.Concat(agency, "_Inter_", doctype, "_", cleanedSubdoctype, "_", timestamp, fileExtension);

                string uploadRootPath = Path.Combine(baseUploadFolder, filename);
                fl_file.SaveAs(uploadRootPath);

                FileStream fs = File.OpenRead(uploadRootPath);
                string hash = fl.SHA256CheckSum(fs);

                string res = fl.Insertfilehash(actualfilename, filename, hash, agency, cleanedSubdoctype);

                string savedFilePath = Path.Combine(subdoctypeFolder, filename);
                fl_file.SaveAs(savedFilePath);

                string dbFilePath = "Uploads/Process/" + doctype + "/" + subdoctype + "/" + filename;

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

        //if (fl_file.HasFile)
        //{
        //    try
        //    {
        //        string baseUploadFolder = Server.MapPath("~/Uploads/");
        //        string pendingFolder = Path.Combine(baseUploadFolder, "Pending");
        //        string processFolder = Path.Combine(baseUploadFolder, "Process");

        //        // string baseDir = "Uploads/"; // Root directory
        //        // string subDirpending = "Pending/"; // Subdirectory for pending files
        //        // string subDirprocess = "Process/";



        //        // Ensure directories exist
        //        if (!Directory.Exists(pendingFolder)) Directory.CreateDirectory(pendingFolder);
        //        if (!Directory.Exists(processFolder)) Directory.CreateDirectory(processFolder);

        //        string doctype = ddl_doctype.SelectedValue;
        //        string subdoctype = ddl_sub_doc_type.SelectedValue;
        //        string faculty = ddl_Faculty.SelectedItem.Text;
        //        string subjectCode = txt_subjectcode.Text;
        //        string agency = Session["agencyname"].ToString();
        //        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        //        string fileExtension = Path.GetExtension(fl_file.FileName).ToLower();

        //        string filename = string.Concat(doctype, "_", subdoctype, "_", faculty, "_", subjectCode, "_", agency, "_", timestamp, fileExtension);
        //        string savedFilePath = Path.Combine(baseUploadFolder, filename);

        //        // Save the uploaded file
        //        fl_file.SaveAs(savedFilePath);

        //        string dbFilePath = "Uploads/" + filename;

        //        DataTable dt = new DataTable();
        //        if (fileExtension == ".csv")
        //        {
        //            dt = ReadCsvFile(savedFilePath);
        //        }
        //        else if (fileExtension == ".xlsx" || fileExtension == ".xls")
        //        {
        //            dt = ReadExcelFile(savedFilePath);
        //        }
        //        else
        //        {
        //            Response.Write("<script>alert('Invalid file format. Please upload a CSV or Excel file.')</script>");
        //            return;
        //        }
        //        string resfile = fl.InsertProcessFileDetails(filename, dbFilePath, agency, Session["username"].ToString());
        //        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('File uploaded successfully!');", true);
        //        //if (dt.Rows.Count > 0)
        //        //{
        //        //    List<DataRow> duplicates = new List<DataRow>();
        //        //    List<DataRow> validRecords = new List<DataRow>();
        //        //    bool hasError = false;
        //        //    string barcode = "";

        //        //    foreach (DataRow row in dt.Rows)
        //        //    {
        //        //        if (ddl_sub_doc_type.SelectedValue == "Foil Sheet" || ddl_sub_doc_type.SelectedValue == "Award Sheet" ||
        //        //            ddl_sub_doc_type.SelectedValue == "OMR Sheet" || ddl_sub_doc_type.SelectedValue == "Practical Sheet")
        //        //        {
        //        //            barcode = row["BARCODE"].ToString();
        //        //        }
        //        //        else
        //        //        {
        //        //            barcode = row["BARCODE_TOP"].ToString();
        //        //        }

        //        //        bool isDuplicate = fl.CheckForDuplicate(ddl_sub_doc_type.SelectedValue, barcode);

        //        //        if (isDuplicate)
        //        //        {
        //        //            duplicates.Add(row);
        //        //        }
        //        //        else
        //        //        {
        //        //            validRecords.Add(row);
        //        //        }
        //        //    }

        //        //    // Process valid records
        //        //    if (validRecords.Count > 0)
        //        //    {
        //        //        DataTable validTable = dt.Clone();
        //        //        foreach (var row in validRecords) validTable.ImportRow(row);

        //        //        string dbprocessPath = Path.Combine(baseDir, subDirprocess, filename.Replace(fileExtension, ".csv"));

        //        //        string processFilePath = Path.Combine(processFolder, filename.Replace(fileExtension, ".csv"));
        //        //        SaveDataTableToCSV(validTable, processFilePath);

        //        //        string resdatainst = fl.Insert_DownloadFileDetail(filename.Replace(fileExtension, ".csv"), dbprocessPath, "", Session["agencyname"].ToString(), "Process");

        //        //        string res = fl.BulkInsert(ddl_class.SelectedValue, Session["username"].ToString(),
        //        //            Session["userid"].ToString(), Session["agencyname"].ToString(), ddl_doctype.SelectedValue, ddl_sub_doc_type.SelectedValue,
        //        //            ddl_Faculty.SelectedItem.Text, ddl_subject.SelectedValue, txt_subjectcode.Text, dbFilePath, "", "Active", validTable);

        //        //        if (res != "Success")
        //        //        {
        //        //            hasError = true;
        //        //        }


        //        //    }

        //        //    // Process duplicate records
        //        //    if (duplicates.Count > 0)
        //        //    {
        //        //        DataTable duplicateTable = dt.Clone();
        //        //        foreach (var row in duplicates) duplicateTable.ImportRow(row);

        //        //        string pendingFilePath = Path.Combine(pendingFolder, filename.Replace(fileExtension, ".csv"));
        //        //        SaveDataTableToCSV(duplicateTable, pendingFilePath);

        //        //        string dbpendingPath = Path.Combine(baseDir, subDirpending, filename.Replace(fileExtension, ".csv"));

        //        //        string resdatainst = fl.Insert_DownloadFileDetail(filename, dbpendingPath, "", Session["agencyname"].ToString(), "Pending");

        //        //        string resdupli = fl.SaveToDuplicateTable(ddl_class.SelectedValue, Session["username"].ToString(),
        //        //            Session["userid"].ToString(), Session["agencyname"].ToString(), ddl_doctype.SelectedValue, ddl_sub_doc_type.SelectedValue,
        //        //            ddl_Faculty.SelectedItem.Text, ddl_subject.SelectedValue, txt_subjectcode.Text, dbFilePath, "", "Active", duplicateTable);

        //        //        if (resdupli != "Success")
        //        //        {
        //        //            hasError = true;
        //        //        }

        //        //        // Save duplicate data as CSV in Pending folder

        //        //    }

        //        //    // Show success or error message
        //        //    if (hasError)
        //        //    {
        //        //        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error: Some records failed to process. Please check and try again.');", true);
        //        //    }
        //        //    else
        //        //    {
        //        //        ClientScript.RegisterStartupScript(this.GetType(), "alert",
        //        //         "alert('File uploaded successfully. " + validRecords.Count + " records inserted, " + duplicates.Count + " duplicates found.');" +
        //        //         "window.location.href='fileupload.aspx';", true);
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    Response.Write("<script>alert('No valid records found in the file.')</script>");
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        string script = "alert('Error: " + ex.Message.Replace("'", "\\'") + "');";
        //        ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", script, true);
        //    }
        //}
        //else
        //{
        //    Response.Write("<script>alert('Please select a file to upload.')</script>");
        //}


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
                dt = ds.Tables[0]; // Read first sheet
            }
        }
        return dt;
    }

    //private DataTable ReadCsvFile(string filePath)
    //{
    //    DataTable dt = new DataTable();
    //    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
    //    {
    //        string[] headers = sr.ReadLine().Split(',');
    //        foreach (string header in headers)
    //            dt.Columns.Add(header.Trim());

    //        while (!sr.EndOfStream)
    //        {
    //            string[] rows = sr.ReadLine().Split(',');
    //            dt.Rows.Add(rows);
    //        }
    //    }
    //    return dt;
    //}

    //private DataTable ReadCsvFile(string filePath)
    //{
    //    DataTable dt = new DataTable();

    //    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
    //    {
    //        string line;
    //        bool isFirstRow = true;
    //        int columnCount = 0;

    //        while ((line = sr.ReadLine()) != null)
    //        {
    //            // Split by comma and handle extra/missing fields
    //            string[] values = line.Split(',');

    //            if (isFirstRow)
    //            {
    //                columnCount = values.Length; // Get column count from header
    //                foreach (string column in values)
    //                {
    //                    dt.Columns.Add(column.Trim());
    //                }
    //                isFirstRow = false;
    //            }
    //            else
    //            {
    //                // Normalize row length to match column count
    //                if (values.Length > columnCount)
    //                {
    //                    values = values.Take(columnCount).ToArray(); // Trim extra values
    //                }
    //                else if (values.Length < columnCount)
    //                {
    //                    Array.Resize(ref values, columnCount); // Fill missing columns with empty values
    //                }

    //                dt.Rows.Add(values);
    //            }
    //        }
    //    }
    //    return dt;
    //}

    //private DataTable ReadCsvFile(string filePath)
    //{
    //    DataTable dt = new DataTable();

    //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    //    {
    //        HasHeaderRecord = true,
    //        Delimiter = ",",
    //        TrimOptions = TrimOptions.Trim,
    //        BadDataFound = null,
    //        MissingFieldFound = null
    //    };

    //    using (var reader = new StreamReader(filePath))
    //    using (var csv = new CsvReader(reader, config))
    //    {
    //        csv.Read();
    //        csv.ReadHeader();

    //        // Define columns explicitly
    //        foreach (var header in csv.HeaderRecord)
    //        {
    //            if (!dt.Columns.Contains(header))
    //                dt.Columns.Add(header, typeof(string)); // Adjust type if needed
    //        }

    //        while (csv.Read())
    //        {
    //            DataRow row = dt.NewRow();
    //            foreach (DataColumn column in dt.Columns)
    //            {
    //                string fieldValue = csv.GetField(column.ColumnName);

    //                // Assign separately to avoid type mismatch error
    //                if (string.IsNullOrEmpty(fieldValue))
    //                {
    //                    row[column.ColumnName] = DBNull.Value;
    //                }
    //                else
    //                {
    //                    row[column.ColumnName] = fieldValue.Trim();
    //                }
    //            }
    //            dt.Rows.Add(row);
    //        }
    //    }
    //    return dt;
    //}
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

            // Define columns explicitly based on CSV headers
            foreach (var header in csv.HeaderRecord)
            {
                if (!dt.Columns.Contains(header))
                    dt.Columns.Add(header, typeof(string)); // Using string type for flexibility
            }

            while (csv.Read())
            {
                DataRow row = dt.NewRow();

                foreach (DataColumn column in dt.Columns)
                {
                    // Ensure column exists in CSV before trying to access it
                    string fieldValue = csv.HeaderRecord.Contains(column.ColumnName) ? csv.GetField(column.ColumnName) : null;

                    // Debugging: Print values to verify they are read correctly
                    Console.WriteLine("Column: " + column.ColumnName + ", Value: " + fieldValue);


                    // Ensure we store a trimmed value or NULL if empty
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
            // Write column headers
            writer.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

            // Write data rows
            foreach (DataRow row in dt.Rows)
            {
                writer.WriteLine(string.Join(",", row.ItemArray.Select(field => field.ToString().Replace(",", " "))));
            }
        }
    }

    protected void btn_submittoken_Click(object sender, EventArgs e)
    {

        //List<string> validKeys = new List<string>
        //{
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzc2IvYnNlYiIsInN1YiI6IlRmTE1VaThzcGR4ZlR0Z0tOR216dVNNZWY3QUE4cGZLaDhKIiwiZXhwIjoxODM0MjMwNDc5MzQ2LCJpYXQiOjE3Mzk2MjI0NzkzNDYsInByaSI6IjRkN2NkOThjNWU2MTM4MDFmNDNhYzdiMGFmMmZjYmNhZTgwYzg5NTc4YTJiNjZlYjRkYzg4NDExNzc5MzVhZGRlNDIzMTFlYjY3NTUxMzhkZGYxNjMwYjBjMWViMWFlY2Q0ZDRiNzMzMGQ5NWUyMDAwZjIwNzgxNDdmZWQ4NDIwYWRhMmZkM2FmYmVjMmYzZWE0NTYzMjExZjZiYzc0ZmIifQ.Qov_ye1ktkjYtafN1K1nUWC4Gr_bd3pylfhKz4vLyI0",
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzc2IvYnNlYiIsInN1YiI6IlRmRXB1djVLZVdVVUF6VU1OM3RTcm9ENG92cmlTU3Noc0gxIiwiZXhwIjoxODM0MjMwNTI3NDY4LCJpYXQiOjE3Mzk2MjI1Mjc0NjgsInByaSI6ImEwNzVlMzQ2NWNkZWVhOGFjZWQwZTJiNmRlNDQ0NTA1ZjhlMmY5ZmQ1OWZmNTBlNjNlODIwZDZlMTM4YTBhNTI3NzA4YmZiMWFlMDdjZDkwODQ2OGI2NDNmMDA4YTRhM2ZkZDJjZDA1NzlkN2Q0MzhhZTIwYzhiMGFkNWRhOGM1MWMzNDJiYjVmMzVhNmYwODU4NWJlZGNlZTc0ZTlmNjYifQ.GK2vHgkC-RBJLj72MBXbeqR4QcFT8g6dRkAY11uV5es",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvS2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj1E=",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvS2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj1G=",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj1J=",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj1F=",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4F6",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4G8",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4H7",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4J0",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4N9",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4QD",
        //    "x6gTKrlaPLim729MdF6o75z4Z+Gh+MP7pyEwIe1tMe6yIFxvWiExMyFEqCrxRuQePFeopWFwFxo01r3kediZqY5SQBCjdUFu7Lz2JufizccdUqc8s/Uam/44Am3Oue01WoOaggQhMTMhp53dj59ZVTxkJGubdm+P2RnhR6qC9BPNB5iCFuz9YlJJt6qCLillhRT46l3e9HwaIKWq/xX0/pu4m/6xXkOzJPOw7u1LU15lntTIRd5xITM5IeHRITEzIcCh38jevkp0+uoOyR8VZZeuNtdAqfDxO2oQE7CogFx7gyyyskT0o1iz28PxcLOuHgf+iW1cuOvDdzp66YCTKhmS14JtAaR7ITE2MCGA6HdrGYa175YhMzQhD7L0d2HmzCExMyFqHsiH1yz8nvrkNm8SuZpwtscZ5rXwITM0IbMUhNan9SEwIW8mfaURwoOq1rshMCEzWlNk8OiQik31xXpkITM0Ic1KysoksaEhMTYwIRM75MbfOC5nRlPqqNAdEkTNsGTJfiX3z2Cm+mVsKg7LmStDpL7W62xMVYdCsy58t3ysFe0/w8jKvGb0lLx46bxwQSEzOSG9vqtRvmLqrUqzXItjeSEzOSEkLlVCFWQhMzQhFJw8YqxLMdH36uf/o7ReQz2yphrJJj6cITkhHiWvmE0lj3mojgchMTIhzKWOcVJnrJGYTBayITMzIU4St0qN5YXkQfhXz4GLZ2vrt5J+3fXKZOohMTYwIc8qrn+xLfCOOs0cGzhnbbj9FZRy64v6wYdeY3+XOSlJs+BIWQW2KQH6Wb6Oc6eXjlbQF5ljaasYkH1nKjDVXyr/3aVOEVAsZxGM2wH85yExMiG0YiExMSHrsxeGD8AO84t2vC+Uh5KnLgd3r8FqORp3BPAYqsqjtDRdjixMVSq3jiEwIfUcgt89epYCbd/wzvPyecju2XaBgSExMCElwG6ZzCEzMyFSMoq6PJKHM/cWJuMgvHPKD1hwN7gqo3ib1loZVTYDFUwz8dLhWIMDkRhRadf2ej97h0q4ogQdYqxLMdH36udquOqBTdzRTPYD1XJFy/mhITMzIbDzo8brK0whMzkhZKwkvOvFcknvPOoPfiqv46Pe8uKrWzwrEvecjE4bXyExMCF/qbqpf3zxX53sn2KMEUZCnRjTITEwIRyo6kU1aymirrxNAWL1SuNRStFslfe777Xd88Y1VWk1xS+/hdIGyRaGGs3neVOyYnWHO3QobAeXuZRJro8hMTAhc3QwVCE5IbGv89Ad+hshMzkhP4nU5LMghs//HjHANVGdeo3IonWNgSEwIcRcbO6DgWwqZcKoq+khMCGAWiEzNCH4xlh33hvD2swIuSExMSFWOZmCTJgOZNDNGWy146I123GowTt6KeOihwSMNZgWvWyLj4ZN"
        //};


        // Validate the key
        //if (validKeys.Contains(txt_pvtkey.Text))
        //{
        //    lbl_validate.Text = "Key is Valid";
        //    lbl_validate.ForeColor = System.Drawing.Color.Green;
        //    btn_submit.Visible = true;
        //    div_fileupload.Visible = true;
        //}
        //else
        //{
        //    lbl_validate.Text = "Key is InValid";
        //    lbl_validate.ForeColor = System.Drawing.Color.Red;
        //    btn_submit.Visible = false;
        //    div_fileupload.Visible = false;

        //}
        FlureeCS fl = new FlureeCS();
        string enteredKey = txt_pvtkey.Text.Trim();
      string username = Session["Username"].ToString();

        if (fl.IsPrivateKeyValid(username,enteredKey))   // ✅ Calling function
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

    //protected void ddl_Faculty_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    int selectedFacultyId;
    //    if (int.TryParse(ddl_Faculty.SelectedValue, out selectedFacultyId) && selectedFacultyId != 0) // Ensure a valid integer selection
    //    {
    //        System.Diagnostics.Debug.WriteLine("Selected Faculty Id: " + selectedFacultyId); // Print to console (Debug Output)
    //        Bindsubjectdropdown(selectedFacultyId); // Pass the selected Faculty Id
    //    }
    //}

    //protected void ddl_subject_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    Bindsubjectcode();
    //}
}