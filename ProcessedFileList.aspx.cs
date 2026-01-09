using CsvHelper.Configuration;
using CsvHelper;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProcessedFileList : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            getfileslist();
        }
    }

    public void getfileslist()
    {

        DataTable res = fl.GetProcessFileList();
        if (res != null && res.Rows.Count > 0)
        {
            rptProcessedFiles.DataSource = res;
            rptProcessedFiles.DataBind();
        }
        else
        {
            rptProcessedFiles.DataSource = null;
            rptProcessedFiles.DataBind();
        }
    }
    protected void rptProcessedFiles_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "StartProcessing")
        {
            string file = e.CommandArgument.ToString();
            string filePath = Server.MapPath(e.CommandArgument.ToString());
            ProcessFile(file,filePath);
        }
    }

    private void ProcessFile(string file,string filePath)
    {
        //string fullFilePath = Server.MapPath("~/" + filePath);
        bool isExactDuplicate, isDiscrepancy;
        DataTable dt = new DataTable();
        string baseDir = "Uploads/";
        string subDirpending = "Pending/";
        string subDirprocess = "Process/";

        string baseUploadFolder = Server.MapPath("~/Uploads/");
        string pendingFolder = Path.Combine(baseUploadFolder, "Pending");
        string processFolder = Path.Combine(baseUploadFolder, "Process");

        string[] fileParts = file.Split('_');
        string extractedDoctype = fileParts.Length > 0 ? fileParts[0] : "";
        string[] doc = extractedDoctype.Split('/');
        string finaldoctype = doc.Length > 1 ? doc[1] : "";
        string extractedSubdoctype = fileParts.Length > 1 ? fileParts[1] : "";
        string extractedFaculty = fileParts.Length > 2 ? fileParts[2] : "";
        string extractedSubjectCode = fileParts.Length > 3 ? fileParts[3] : "";
        string extractedAgency = fileParts.Length > 4 ? fileParts[4] : "";
        string extractetimestamp = fileParts.Length > 5 ? fileParts[5] : "";

        string[] timestamp = extractetimestamp.Split('.');
        string finaltimestamp = timestamp.Length > 0 ? timestamp[0] : "";
       

        string fileExtension = Path.GetExtension(filePath);
        string filename = finaldoctype + "_" + extractedSubdoctype + "_" + extractedFaculty + "_" + extractedSubjectCode + "_" + extractedAgency + "_" + finaltimestamp;

        List<DataRow> validRecords = new List<DataRow>();
        List<DataRow> duplicates = new List<DataRow>();

        bool hasError = false;
        string barcode = "";
        if (fileExtension == ".csv")
        {
            dt = ReadCsvFile(filePath);
        }
        else if (fileExtension == ".xlsx" || fileExtension == ".xls")
        {
            dt = ReadExcelFile(filePath);
        }
        else
        {
            Response.Write("<script>alert('Invalid file format. Please upload a CSV or Excel file.')</script>");
            return;
        }
        foreach (DataRow row in dt.Rows)
        {
            if (extractedSubdoctype == "Foil Sheet" || extractedSubdoctype == "Award Sheet" ||
                extractedSubdoctype == "OMR Sheet" || extractedSubdoctype == "Practical Sheet")
            {
                barcode = row["BARCODE"].ToString();
            }
            else
            {
                barcode = row["BARCODE_TOP"].ToString();
            }
            if(extractedSubdoctype =="OMR Sheet")
            {
                fl.CheckForDuplicateOMR(extractedSubdoctype, row, out isExactDuplicate, out isDiscrepancy);

                // ✅ Handle different cases
                if (isExactDuplicate || isDiscrepancy)  
                {
                    duplicates.Add(row);
                    continue;  
                }
                
            }
            bool isDuplicate = fl.CheckForDuplicate(extractedSubdoctype, barcode);

            if (isDuplicate)
            {
                duplicates.Add(row);
            }

            validRecords.Add(row);

        }
        string res = "";
        // Process valid records
        if (validRecords.Count > 0)
        {
            DataTable validTable = dt.Clone();
            DataTable duplicateTable = new DataTable();
            foreach (var row in validRecords) validTable.ImportRow(row);

            string dbprocessPath = Path.Combine(baseDir, subDirprocess, filename + ".csv");
            string processFilePath = Path.Combine(processFolder, filename + ".csv");
            SaveDataTableToCSV(validTable, processFilePath);

            string resdatainst = fl.Insert_DownloadFileDetail(filename + ".csv", dbprocessPath, "", extractedAgency, "Process");
            if (extractedSubdoctype == "OMR Sheet")
            {
                res = fl.insertOMRsheet("Intermediate", "", "", extractedAgency, finaldoctype, extractedSubdoctype,
               extractedFaculty, "", extractedSubjectCode, filePath, "", "Active", validTable, out duplicateTable);
            }
            else
            {
                res = fl.BulkInsert("Intermediate", "", "", extractedAgency, finaldoctype, extractedSubdoctype,
               extractedFaculty, "", extractedSubjectCode, filePath, "", "Active", validTable);
            }

            if (res != "Success" && res != "Invalid records found")
            {
                hasError = true;
            }

            if (res == "Invalid records found" && duplicateTable.Rows.Count > 0)
            {
                DataTable dtdupli = duplicateTable;
               
                string resdupli = fl.SaveToDuplicateTable("Intermediate", "", "", extractedAgency, finaldoctype, extractedSubdoctype,
                    extractedFaculty, "", extractedSubjectCode, filePath, "", "Active", duplicateTable);

                if (resdupli != "Success")
                {
                    hasError = true;
                }
                string pendingFilePath = Path.Combine(pendingFolder, filename + "_invalid.csv");
                SaveDataTableToCSV(dtdupli, pendingFilePath);

                string dbpendingPath = Path.Combine(baseDir, subDirpending, filename + "_invalid.csv");
                fl.Insert_DownloadFileDetail(filename + "_invalid.csv", dbpendingPath, "", extractedAgency, "Pending");

            }
        }

        // Process duplicate records
        if (duplicates.Count > 0)
        {
            DataTable duplicateTable = dt.Clone();
            foreach (var row in duplicates) duplicateTable.ImportRow(row);

            string pendingFilePath = Path.Combine(pendingFolder, filename + ".csv");
            SaveDataTableToCSV(duplicateTable, pendingFilePath);

            string dbpendingPath = Path.Combine(baseDir, subDirpending, filename + ".csv");

            string resdatainst = fl.Insert_DownloadFileDetail(filename + ".csv", dbpendingPath, "", extractedAgency, "Pending");

            string resdupli = fl.SaveToDuplicateTable("Intermediate", "", "", extractedAgency, finaldoctype, extractedSubdoctype,
                extractedFaculty, "", extractedSubjectCode, filePath, "", "Active", duplicateTable);

            if (resdupli != "Success")
            {
                hasError = true;
            }
        }

        // Define alert message based on conditions
        string message;
        if (hasError)
        {
            message = "Error: Some records failed to process. Please check and try again.";
        }
        else if (validRecords.Count > 0 && duplicates.Count > 0)
        {
            message = string.Concat("File uploaded successfully. ", validRecords.Count,
                         " records inserted, ", duplicates.Count, " duplicates found.");
            DataTable dtres = fl.Update_ProcessedFileDetail(filename + fileExtension);
        }
        else if (validRecords.Count > 0)
        {
            message = string.Concat("File uploaded successfully. ", validRecords.Count, " records inserted.");
            DataTable dtres = fl.Update_ProcessedFileDetail(filename + fileExtension);
        }
        else if (duplicates.Count > 0)
        {
            message = string.Concat("File uploaded but all ", duplicates.Count, " records were found to be duplicates.");
            DataTable dtres = fl.Update_ProcessedFileDetail(filename + fileExtension);
        }
        else
        {
            message = "No records were processed.";
        }

        // Show success or error message
        ClientScript.RegisterStartupScript(this.GetType(), "alert",
     string.Concat("alert('", message, "'); window.location.href='ProcessedFileList.aspx';"), true);

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

    
}