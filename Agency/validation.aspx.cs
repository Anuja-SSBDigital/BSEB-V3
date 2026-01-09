using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.HtmlControls;

public partial class validation : System.Web.UI.Page
{

    FlureeCS fl = new FlureeCS();

    public void FetchStudentData(string rollCode, string rollNumber)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SecondDbConnection"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_fetch_result_backup", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rollNumber", rollNumber);
                    cmd.Parameters.AddWithValue("@rollCode", rollCode);

                    SqlParameter retValue = new SqlParameter("@retValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(retValue);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dtResult = new DataTable();
                        da.Fill(dtResult);

                        int errorCode = Convert.ToInt32(retValue.Value);

                        if (errorCode == -1) //  No result found
                        {
                            Response.Write("<script>alert('No result found for the given Roll Number and Roll Code.');</script>");
                            return;
                        }

                        if (dtResult.Rows.Count > 0)
                        {
                            //  Assign Student Details (First Row)
                            DataRow studentRow = dtResult.Rows[0];
                            string categoryname = studentRow["CategoryName"].ToString();
                            //if (categoryname == "COMPARTMENTAL")
                            //{
                            //    lbl_heading.Text = "INTERMEDIATE COMPARTMENTAL EXAMINATION-2025";
                            //}
                            //else
                            //{
                            //    lbl_heading.Text = "INTERMEDIATE SPECIAL EXAMINATION-2025";

                            //}
                            int IsResultAfterScrutiny = 0;
                            if (studentRow["IsResultAfterScrutiny"] != DBNull.Value)
                            {
                                IsResultAfterScrutiny = Convert.ToBoolean(studentRow["IsResultAfterScrutiny"]) ? 1 : 0;
                            }

                            string ResultAfterScrutinyRemarks = studentRow["ResultAfterScrutinyRemarks"] != DBNull.Value
                                ? studentRow["ResultAfterScrutinyRemarks"].ToString()
                                : string.Empty;


                            lbl_bsebuniqueID.Text = studentRow["BSEB_Unique_Id"].ToString();
                            lbl_studentname.Text = studentRow["Student_Name"].ToString();
                            lbl_fathername.Text = studentRow["Father_Name"].ToString();
                            lbl_collegename.Text = studentRow["School_College_Name"].ToString();
                            lbl_rollcode.Text = studentRow["Roll_Code"].ToString();
                            lbl_rollnumber.Text = studentRow["Roll_Number"].ToString();
                            lbl_registrationno.Text = studentRow["Registration_Number"].ToString();
                            lbl_faculty.Text = studentRow["Faculty"].ToString();
                            lbl_aggregratemarks.Text = studentRow["AggregateMarks"].ToString();
                            lbl_division.Text = studentRow["Result_Division"].ToString();

                            //  Filter Subject-Wise Marks (Remaining Rows)
                            DataView dvSubjects = new DataView(dtResult);

                            bool hasVocationalTrade = dtResult.AsEnumerable()
         .Any(row =>
             row["Subject_Group_Name"].ToString() == "Vocational Trade" ||
             row["Subject_Group_Name"].ToString() == "Additional subject group Vocational (100 marks)");

                            var groupedData = dtResult.AsEnumerable()
    .GroupBy(row =>
        row["Subject_Group_Name"].ToString() == "Vocational Trade" ||
        row["Subject_Group_Name"].ToString() == "Additional subject group Vocational (100 marks)"
            ? "Vocational Trade"
            : row["Subject_Group_Name"].ToString()
    )
    .Select(g => new
    {
        SubjectGroupName = g.Key,
        Subjects = g.CopyToDataTable()
    })
    .ToList();

                            ViewState["ShowCCEColumn"] = hasVocationalTrade;
                            th_CCE.Visible = hasVocationalTrade;

                            RepeaterSubjectGroups.DataSource = groupedData;
                            RepeaterSubjectGroups.DataBind();
                            DivBlockchain.Visible = true;
                        }
                        else
                        {
                            Response.Write("<script>alert('⚠ No records found for this student.');</script>");

                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Server Error: " + ex.Message.Replace("'", "\\'") + "');</script>");

        }
    }
    //public void BindFlureeData()
    //{
    //    string res = fl.getstudentmasterdata(txt_rollcode.Text, txt_rollnumber.Text);
    //    if (!res.StartsWith("Error"))
    //    {
    //        DataTable dt = fl.Tabulate(res);
    //        if (dt.Rows.Count > 0)
    //        {
    //            string lbl_bsebuniqueID = dt.Rows[0]["Stu_UniqueId"].ToString();
    //            string lbl_studentname = dt.Rows[0]["StudentFullName"].ToString();
    //            string lbl_fathername = dt.Rows[0]["FatherName"].ToString();
    //            string lbl_collegename = dt.Rows[0]["CollegeName"].ToString();
    //            string lbl_rollcode = dt.Rows[0]["RollCode"].ToString();
    //            string lbl_rollnumber = dt.Rows[0]["RollNumber"].ToString();
    //            string lbl_registrationno = dt.Rows[0]["RegistrationNo"].ToString();
    //            string lbl_faculty = dt.Rows[0]["Faculty"].ToString();

    //        }

    //    }


    //    string resd = fl.getstudentdetailsdata(txt_rollcode.Text, txt_rollnumber.Text);
    //    if (!resd.StartsWith("Error"))
    //    {
    //        DataTable dtd = fl.Tabulate(resd);
    //        if (dtd.Rows.Count > 0)
    //        {
    //            // Check if Vocational Trade exists
    //            bool hasVocationalTrade = dtd.AsEnumerable()
    //                .Any(row =>
    //                    row["Subject_Group_Name"].ToString() == "Vocational Trade" ||
    //                    row["Subject_Group_Name"].ToString() == "Additional subject group Vocational (100 marks)"
    //                );

    //            // Grouping the data by Subject Group Name
    //            var groupedData = dtd.AsEnumerable()
    //                .GroupBy(row =>
    //                    row["Subject_Group_Name"].ToString() == "Vocational Trade" ||
    //                    row["Subject_Group_Name"].ToString() == "Additional subject group Vocational (100 marks)"
    //                        ? "Vocational Trade"
    //                        : row["Subject_Group_Name"].ToString()
    //                )
    //                .Select(g => new
    //                {
    //                    SubjectGroupName = g.Key,
    //                    Subjects = g.CopyToDataTable()
    //                })
    //                .ToList();

    //            ViewState["ShowCCEColumn"] = hasVocationalTrade;
    //            th_CCE.Visible = hasVocationalTrade;

    //            RepeaterSubjectGroups.DataSource = groupedData;
    //            RepeaterSubjectGroups.DataBind();
    //        }
    //    }
    //}

    // Function to compute hash of data

    //public void BindDetails()
    //{
    //    // Fetch Fluree data
    //    Fluree fl = new Fluree();
    //    string res = fl.getstudentdetails(); // Get student details from Fluree

    //    if (!res.StartsWith("Error"))
    //    {
    //        // Convert Fluree response to DataTable
    //        DataTable flureeDataTable = fl.Tabulate(res);
    //        var sortedRows = flureeDataTable.AsEnumerable()
    //.OrderBy(row => int.Parse(row["id"].ToString())) // Ensure 'id' is parsed as an integer
    //.CopyToDataTable();

    //        // Fetch MSSQL data
    //        DataTable mssqlDataTable = FetchMSSQLData();

    //        // Add columns for hash and tampered status
    //        flureeDataTable.Columns.Add("FlureeHash", typeof(string));
    //        flureeDataTable.Columns.Add("MSSQLHash", typeof(string));
    //        flureeDataTable.Columns.Add("TamperedStatus", typeof(string));

    //        mssqlDataTable.Columns.Add("FlureeHash", typeof(string));
    //        mssqlDataTable.Columns.Add("MSSQLHash", typeof(string));
    //        mssqlDataTable.Columns.Add("TamperedStatus", typeof(string));

    //        // Compare Fluree and MSSQL data
    //        foreach (DataRow flureeRow in flureeDataTable.Rows)
    //        {
    //            // Compute Fluree hash
    //            double createdat = Convert.ToDouble(flureeRow["createdat"]);
    //            DateTime convertedDateTime = fl.ConvertTimestampToDateTime(createdat);
    //            string convertedDateTimeString = convertedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    //            string flureeDataString = flureeRow["name"].ToString() + flureeRow["semester"].ToString() +
    //                                      flureeRow["subject1"].ToString() + flureeRow["subject2"].ToString() +
    //                                      flureeRow["subject3"].ToString();


    //            string flureeHash = ComputeHash(flureeDataString);
    //            flureeRow["FlureeHash"] = flureeHash;

    //            // Find matching MSSQL row
    //            DataRow[] matchedMssqlRows = mssqlDataTable.Select("id = '" + flureeRow["id"].ToString() + "'");
    //            if (matchedMssqlRows.Length > 0)
    //            {
    //                DataRow mssqlRow = matchedMssqlRows[0];

    //                // Compute MSSQL hash
    //                string mssqlDataString = mssqlRow["name"].ToString() + mssqlRow["semster"].ToString() +
    //                                         mssqlRow["subject1"].ToString() + mssqlRow["subject2"].ToString() +
    //                                         mssqlRow["subject3"].ToString();

    //                string mssqlHash = ComputeHash(mssqlDataString);
    //                flureeRow["MSSQLHash"] = mssqlHash;
    //                mssqlRow["FlureeHash"] = flureeHash;
    //                mssqlRow["MSSQLHash"] = mssqlHash;

    //                // Determine tampered status
    //                string tamperedStatus = flureeHash == mssqlHash ? "Not Tampered" : "Tampered";
    //                flureeRow["TamperedStatus"] = tamperedStatus;
    //                mssqlRow["TamperedStatus"] = tamperedStatus;
    //            }
    //            else
    //            {
    //                // No matching MSSQL row, mark Fluree row as Tampered
    //                flureeRow["TamperedStatus"] = "Tampered";
    //            }
    //        }

    //        // Mark remaining MSSQL rows that have no matching Fluree rows
    //        foreach (DataRow mssqlRow in mssqlDataTable.Rows)
    //        {
    //            if (mssqlRow["TamperedStatus"] == DBNull.Value )
    //            {
    //                mssqlRow["TamperedStatus"] = "Tampered";
    //            }
    //        }

    //        // Bind Fluree data to Repeater 1
    //        rpt_studentdetails.DataSource = sortedRows;
    //        rpt_studentdetails.DataBind();

    //        // Bind MSSQL data to Repeater 2
    //        rpt_userdetails.DataSource = mssqlDataTable;
    //        rpt_userdetails.DataBind();
    //    }
    //}

    // Function to fetch MSSQL data
    //private DataTable FetchMSSQLData()
    //{
    //    string query = @"
    //    SELECT 
    //        RTRIM(id) AS id,
    //        RTRIM(name) AS name,
    //        RTRIM(semster) AS semster,
    //        RTRIM(subject1) AS subject1,
    //        RTRIM(subject2) AS subject2,
    //        RTRIM(subject3) AS subject3,
    //        RTRIM(createdat) AS createdat,
    //        RTRIM(createdby) AS createdby,
    //        RTRIM(updatedat) AS updatedat,
    //        RTRIM(updatedby) AS updatedby,
    //        RTRIM(system_name) AS system_name
    //    FROM userdetails";
    //    DataTable mssqlDataTable = new DataTable();
    //    //using (SqlConnection conn = new SqlConnection(connectionString))
    //    //{
    //    //    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
    //    //    {
    //    //        adapter.Fill(mssqlDataTable);
    //    //    }
    //    //}
    //    return mssqlDataTable;
    //}

    //// Function to compute hash
    //private string ComputeHash(string inputData)
    //{
    //    using (SHA256 sha256 = SHA256.Create())
    //    {
    //        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputData));
    //        StringBuilder sb = new StringBuilder();
    //        foreach (byte b in hashBytes)
    //        {
    //            sb.Append(b.ToString("x2"));
    //        }
    //        return sb.ToString();
    //    }
    //}



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {

        string rollCode = txt_rollcode.Text.Trim();
        string rollNumber = txt_rollnumber.Text.Trim();
        FetchStudentData(rollCode, rollNumber);
        //BindFlureeData();

        // BindFlureeData();
        //}
        //else
        //{
        //    Response.Write("<script>alert('❌ Invalid request. Roll Number and Roll Code are required.');</script>");

        //}
    }

    //protected void btn_submit_Click(object sender, EventArgs e)
    //{
    //    string rollCode = txt_rollcode.Text.Trim();
    //    string rollNumber = txt_rollnumber.Text.Trim();

    //    if (string.IsNullOrEmpty(rollCode) || string.IsNullOrEmpty(rollNumber))
    //    {
    //        //lblMessage.Text = "Please enter both Roll Code and Roll Number.";
    //        //return;
    //    }

    //    DataTable dtResult1 = new DataTable(); // From EXAM_FinalPublishedResult
    //    DataTable dtResult2 = new DataTable(); // From Final_Result_Details_2025

    //    using (SqlConnection conn = new SqlConnection(connStr2))
    //    {
    //        conn.Open();

    //        // Fetch from EXAM_FinalPublishedResult
    //        string query1 = @"
    //        SELECT *
    //        FROM EXAM_FinalPublishedResult
    //        WHERE RollCode = @rollcode AND RollNumber = @rollnumber
    //        ORDER BY SubjectPaperCode";

    //        using (SqlCommand cmd1 = new SqlCommand(query1, conn))
    //        {
    //            cmd1.Parameters.AddWithValue("@rollcode", rollCode);
    //            cmd1.Parameters.AddWithValue("@rollnumber", rollNumber);

    //            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
    //            da1.Fill(dtResult1);
    //        }

    //        // Fetch from Final_Result_Details_2025
    //        string query2 = @"
    //        SELECT *
    //        FROM Final_Result_Details_2025
    //        WHERE RollCode = @rollcode AND RollNumber = @rollnumber
    //        ORDER BY SubjectPaperCode";

    //        using (SqlCommand cmd2 = new SqlCommand(query2, conn))
    //        {
    //            cmd2.Parameters.AddWithValue("@rollcode", rollCode);
    //            cmd2.Parameters.AddWithValue("@rollnumber", rollNumber);

    //            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
    //            da2.Fill(dtResult2);
    //        }
    //    }

    //    // Compare logic – only for dtResult1
    //    dtResult1.Columns.Add("TamperStatus", typeof(string));

    //    string[] colsToCompare = new string[]
    //    {
    //    "TotalTheoryMarks",
    //    "PRObtainedMarks",
    //    "CCEMarks",
    //    "TheoryGraceMarks",
    //    "PracticalGraceMarks"
    //    };

    //    for (int i = 0; i < dtResult1.Rows.Count; i++)
    //    {
    //        string status = "Not Tampered";

    //        if (i >= dtResult2.Rows.Count)
    //        {
    //            status = "Tampered"; // row missing in dtResult2
    //        }
    //        else
    //        {
    //            foreach (string col in colsToCompare)
    //            {
    //                string val1 = "";
    //                if (dtResult1.Rows[i][col] != DBNull.Value && dtResult1.Rows[i][col] != null)
    //                    val1 = dtResult1.Rows[i][col].ToString().Trim();

    //                string val2 = "";
    //                if (dtResult2.Rows[i][col] != DBNull.Value && dtResult2.Rows[i][col] != null)
    //                    val2 = dtResult2.Rows[i][col].ToString().Trim();

    //                if (!string.Equals(val1, val2, StringComparison.OrdinalIgnoreCase))
    //                {
    //                    status = "Tampered";
    //                    break;
    //                }
    //            }
    //        }

    //        dtResult1.Rows[i]["TamperStatus"] = status;
    //    }

    //    // Bind to Repeater controls
    //    //rpt_mssqldata.DataSource = dtResult1;
    //    //rpt_mssqldata.DataBind();

    //    //rpt_flureedata.DataSource = dtResult2;
    //    //rpt_flureedata.DataBind();

    //}

    protected void rpt_mssqldata_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HtmlTableCell tdName = (HtmlTableCell)e.Item.FindControl("tdNameCell");

            if (tdName != null)
            {
                if (e.Item.ItemIndex == 0)
                {
                    // First row: show name and rowspan
                    //tdName.Attributes["rowspan"] = rpt_mssqldata.Items.Count.ToString();
                    tdName.Attributes["style"] = "vertical-align: middle; text-align: center; font-weight: bold;";
                }
                else
                {
                    // Hide for all other rows
                    tdName.Visible = false;
                }
            }
        }
    }


}