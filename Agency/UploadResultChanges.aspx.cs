using ExcelDataReader;
using Org.BouncyCastle.Asn1.Cmp;
using Spire.Pdf.General.Paper.Uof;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_UploadResultChanges : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
               
                btn_submit.Visible = false;
                div_fileupload.Visible = false;
                div_remarks.Visible= false;
            }
            else
            {
                Response.Redirect("../login.aspx");
            }
        }
    }

    
    protected void btn_submittoken_Click(object sender, EventArgs e)
    {
        
        FlureeCS fl = new FlureeCS();
        string enteredKey = txt_pvtkey.Text.Trim();

        if (fl.IsPrivateKeyValid(enteredKey))  
        {
            lbl_validate.Text = "Key is Valid";
            lbl_validate.ForeColor = System.Drawing.Color.Green;
            btn_submit.Visible = true;
            div_fileupload.Visible = true;
            div_remarks.Visible = true;
        }
        else
        {
            lbl_validate.Text = "Key is Invalid ";
            lbl_validate.ForeColor = System.Drawing.Color.Red;
            btn_submit.Visible = false;
            div_fileupload.Visible = false;

            div_remarks.Visible = false;
        }
    }

    public void InsertResultChangeRequestsUsingSP(DataTable dataTable, string agencyremarks, string filepath, string agencyname)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            foreach (DataRow row in dataTable.Rows)
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddCompartResultChangeRequest", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string stuId = row["Stu_UniqueId"].ToString().Trim();
                    if (stuId.ToUpper().Contains("E"))
                    {
                        double d;
                        if (double.TryParse(stuId, out d))
                        {
                            stuId = d.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    cmd.Parameters.AddWithValue("@Stu_UniqueId", stuId);
                    cmd.Parameters.AddWithValue("@CollegeName", row["CollegeName"]);
                    string roll = row["RollNumber"].ToString().Trim();
                    if (roll.ToUpper().Contains("E"))
                    {
                        double d;
                        if (double.TryParse(roll, out d))
                        {
                            roll = d.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    cmd.Parameters.AddWithValue("@RollNumber", roll);

                    cmd.Parameters.AddWithValue("@RollCode", row["RollCode"]);

                  
                    cmd.Parameters.AddWithValue("@StudentFullName", row["StudentFullName"]);

                    cmd.Parameters.AddWithValue("@FatherName", row["FatherName"]);
                    cmd.Parameters.AddWithValue("@FacultyName", row["FacultyName"]);
                    cmd.Parameters.AddWithValue("@SubjectGroupName", row["SubjectGroupName"]);

                    cmd.Parameters.AddWithValue("@SubjectPaperName", row["SubjectPaperName"]);

                    cmd.Parameters.AddWithValue("@Prev_ObtainedMarks",
     row["Previous_ObtainedMarks"] != DBNull.Value ? Convert.ToInt32(row["Previous_ObtainedMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Prev_CCEMarks",
                        row["Previous_CCEMarks"] != DBNull.Value ? Convert.ToInt32(row["Previous_CCEMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Prev_TotalTheoryMarks",
                        row["Previous_TotalTheoryMarks"] != DBNull.Value ? Convert.ToInt32(row["Previous_TotalTheoryMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Prev_SubjectTotal",
                        row["Previous_SubjectTotal"] != DBNull.Value ? row["Previous_SubjectTotal"] : DBNull.Value);

                    cmd.Parameters.AddWithValue("@Prev_TotalMarks",
                        row["Previous_TotalMarks"] != DBNull.Value ? Convert.ToInt32(row["Previous_TotalMarks"]) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Prev_Division", row["Previous_Division"]);

                    cmd.Parameters.AddWithValue("@Updated_ObtainedMarks",
      row["Updated_ObtainedMarks"] != DBNull.Value ? Convert.ToInt32(row["Updated_ObtainedMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Updated_CCEMarks",
                        row["Updated_CCEMarks"] != DBNull.Value ? Convert.ToInt32(row["Updated_CCEMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Updated_TotalTheoryMarks",
                        row["Updated_TotalTheoryMarks"] != DBNull.Value ? Convert.ToInt32(row["Updated_TotalTheoryMarks"]) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Updated_TotalMarks",
                        row["Updated_TotalMarks"] != DBNull.Value ? Convert.ToInt32(row["Updated_TotalMarks"]) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Updated_SubjectTotal", row["Updated_SubjectTotal"]);

                    cmd.Parameters.AddWithValue("@Updated_Division", row["Updated_Division"]);

                    cmd.Parameters.AddWithValue("@AgencyRemarks", agencyremarks);
                    cmd.Parameters.AddWithValue("@SupportingFilePath", filepath);
                    cmd.Parameters.AddWithValue("@UploadedByAgency", agencyname);

                    cmd.ExecuteNonQuery();
                }
            }
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

    public DataTable ReadCsvToDataTable(string filePath)
    {
        DataTable dt = new DataTable();

        using (var reader = new StreamReader(filePath))
        {
            string headerLine = reader.ReadLine();
            if (headerLine == null)
                throw new Exception("CSV file is empty or unreadable.");

            string[] headers = headerLine.Split(',');

            foreach (string header in headers)
                dt.Columns.Add(header.Trim());

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == null) continue;

                string[] values = line.Split(',');
                dt.Rows.Add(values);
            }
        }

        return dt;
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        if (fl_file.HasFile)
        {
            string uploadsFolder = Server.MapPath("~/Uploads/ChangeRequest");

          
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Path.GetFileName(fl_file.FileName);
            string fileExt = Path.GetExtension(fileName).ToLower();
            string filePath = Path.Combine(uploadsFolder, fileName);

            
            fl_file.SaveAs(filePath);

         
            string dbFilePath = "Uploads/ChangeRequest/" + fileName;

            DataTable dt = null;

            if (fileExt == ".xlsx" || fileExt == ".xls")
            {
               
                dt = ReadExcelFile(filePath);
            }
            else if (fileExt == ".csv")
            {
               
                dt = ReadCsvToDataTable(filePath);
            }

            InsertResultChangeRequestsUsingSP(dt,txt_remarks.Text, dbFilePath, Session["username"].ToString());

            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('FileUpload has been uploaded successfully!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a file.');", true);

        }
    }
}