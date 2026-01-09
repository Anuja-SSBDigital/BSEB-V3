using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Linq;
using Org.BouncyCastle.Ocsp;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.IdentityModel.Protocols.WSTrust;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IdentityModel.Metadata;
using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using System.Xml.Linq;
using System.Web.Security;
using System.Security.Policy;
using Org.BouncyCastle.Crypto.Tls;
using System.Activities.Expressions;
using ZXing.PDF417.Internal;
using System.Data.SqlClient;
using ExcelDataReader;
using OfficeOpenXml;
using System.Collections;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

public class FlureeCS
{
    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
    HttpWebRequest request;
    public readonly ILog log = LogManager.GetLogger
          (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static string servertranurl = "http://192.168.27.12:8090/fdb/ssb/bseb/transact";
    public static string serverqryurl = "http://192.168.27.12:8090/fdb/ssb/bseb/query";
    public static string servermulqryurl = "http://192.168.27.12:8090/fdb/ssb/bseb/multi-query";

    //public static string servertranurl = "http://122.170.117.118:8092/fdb/ssb/nehhdc/transact";
    //public static string serverqryurl = "http://122.170.117.118:8092/fdb/ssb/nehhdc/query";
    //public static string servermulqryurl = "http://122.170.117.118:8092/fdb/ssb/nehhdc/multi-query";
    public static DataTable TempTable()
    {
        DataTable dt = new DataTable();

        DataRow dr = dt.NewRow();

        DataColumn col = new DataColumn();
        col.ColumnName = "Name";
        col.DataType = typeof(string);
        dt.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "Value";
        col.DataType = typeof(string);
        dt.Columns.Add(col);

        col = new DataColumn();
        col.ColumnName = "Key";
        col.DataType = typeof(int);
        dt.Columns.Add(col);

        return dt;
    }

    public static DataTable GetStatusTable()
    {
        DataTable dt = TempTable();

        DataRow dr = dt.NewRow();
        dr["Name"] = "Active";
        dr["Key"] = 0;
        dr["Value"] = "Active";
        dt.Rows.Add(dr);
        dt.AcceptChanges();

        dr = dt.NewRow();
        dr["Name"] = "Assigned";
        dr["Key"] = 1;
        dr["Value"] = "Assigned";
        dt.Rows.Add(dr);
        dt.AcceptChanges();

        dr = dt.NewRow();
        dr["Name"] = "In Process";
        dr["Key"] = 2;
        dr["Value"] = "In Process";
        dt.Rows.Add(dr);
        dt.AcceptChanges();

        dr = dt.NewRow();
        dr["Name"] = "Send To IO";
        dr["Key"] = 3;
        dr["Value"] = "Send To IO";
        dt.Rows.Add(dr);
        dt.AcceptChanges();

        dr = dt.NewRow();
        dr["Name"] = "Received By IO";
        dr["Key"] = 4;
        dr["Value"] = "Received By IO";
        dt.Rows.Add(dr);
        dt.AcceptChanges();

        return dt;
    }
    public static DataTable dtStatus = GetStatusTable();
    #region Other
    private Random _random = new Random();

    public string EncryptString(string str)
    {
        MD5 md5Hash = MD5.Create();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
        // Create a new Stringbuilder to collect the bytes  
        // and create a string.  
        StringBuilder sBuilder = new StringBuilder();
        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string.  
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public string GenerateRandomNo()
    {
        return _random.Next(0, 999999).ToString("D6");
    }

    public string GetIP()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();

    }

    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

    public long ConvertToTimestamp(DateTime value)
    {
        TimeSpan elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalMilliseconds;
    }

    public double ConvertDateTimeToTimestamp(DateTime value)
    {
        TimeSpan epoch = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
        //return the total seconds (which is a UNIX timestamp)
        return (double)epoch.TotalMilliseconds;
    }

    public string sendTransaction(string DatatoPost, string url)
    {
        log.Info("Web request called");
        log.Info("query :" + DatatoPost + "");

        // Check if the token is valid (non-empty and not null)
        //if (string.IsNullOrEmpty(token))
        //{
        //    log.Error("Authorization token is missing or invalid.");
        //    return "Error: Authorization token is missing or invalid.";
        //}

        request = (HttpWebRequest)WebRequest.Create(url);

        // Set timeouts and keep-alive if needed
        request.Timeout = 360000;
        request.ReadWriteTimeout = 360000;
        request.KeepAlive = true;

        request.Method = "POST";
        //request.Headers.Add("Authorization", token);

        try
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(DatatoPost);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            log.Info("GetRequestStream function called");

            // Write the byte array to the request stream
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = request.GetResponse();
            log.Info("Request executed");

            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
        }
        catch (Exception ex)
        {
            log.Error("************** Error : " + ex.Message, ex);
            return "Error : " + ex.Message;
        }

    }

    public DataTable Tabulate(string json)
    {
        var trgArray = new JArray();
        try
        {
            var jsonLinq = JArray.Parse(json);
            //var jsonLinq = JArray.Parse(json);

            // Find the first array using Linq
            foreach (JObject row in jsonLinq.Children<JObject>())
            {
                var cleanRow = new JObject();

                //var srcArray = jsonLinq.Descendants().Where(d => d is JProperty);
                foreach (JToken column in row.Children<JToken>())
                {
                    if (column.First is JArray)
                    {
                        try
                        {
                            JProperty jt = (JProperty)column;

                            JArray res = JArray.Parse("[" + jarray((JArray)column.First) + "]");
                            foreach (JObject jares in res.First.Children<JObject>())
                                foreach (JToken colpck in jares.Children<JToken>())
                                {
                                    if (colpck.First is JValue)
                                    {
                                        try
                                        {
                                            foreach (JValue colv in colpck)
                                            {
                                                // Only include JValue types
                                                if (colv.Parent is JProperty)
                                                {
                                                    JProperty jtv = (JProperty)colv.Parent;
                                                    //if (col.Value is JValue)
                                                    //{
                                                    cleanRow.Add(jtv.Name, jtv.Value);
                                                    //}
                                                }
                                            }
                                        }
                                        catch (Exception ex) { }
                                    }
                                    //trgArray.Merge(jares);
                                }
                        }
                        catch (Exception ex) { }

                    }
                    else if (column.First is JValue)
                    {
                        try
                        {
                            foreach (JValue col in column)
                            {
                                // Only include JValue types
                                if (col.Parent is JProperty)
                                {
                                    JProperty jt = (JProperty)col.Parent;
                                    //if (col.Value is JValue)
                                    //{
                                    cleanRow.Add(jt.Name, jt.Value);
                                    //}
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {
                        try
                        {
                            foreach (JToken t in column.Children<JToken>())
                            {
                                foreach (JProperty v in t)
                                {
                                    string prop = v.Name;
                                    //JProperty p = (JProperty)v.Parent;
                                    if (cleanRow.ContainsKey(v.Name))
                                    {
                                        JProperty colp = (JProperty)column;
                                        prop = colp.Name + "_" + prop;
                                    }
                                    cleanRow.Add(prop, v.Value);
                                }
                            }
                        }
                        catch (Exception ex) { }

                    }

                }
                trgArray.Add(cleanRow);
            }

        }
        catch (Exception ex) { }
        return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
    }

    public JArray jarray(JArray ja)
    {
        JObject cleanRows = new JObject();
        JArray jarow = new JArray();
        foreach (JToken column in ja.Children<JToken>())
        {
            JObject cleanRow = new JObject();
            if (column.Children().Count() > 0)
            {
                if (column.First is JArray)
                {
                    cleanRow.Add(jarray((JArray)column.First));
                }
                else if (column.First is JValue)
                {
                    foreach (JValue col in column)
                    {
                        // Only include JValue types
                        if (col.Parent is JProperty)
                        {
                            JProperty jt = (JProperty)col.Parent;
                            //if (col.Value is JValue)
                            //{
                            cleanRow.Add(jt.Name, jt.Value);
                            //}
                        }
                    }
                }
                else
                {
                    try
                    {
                        foreach (JToken t in column.Children<JToken>())
                        {

                            foreach (JToken col in t.Children<JToken>())
                            {
                                // Only include JValue types
                                try
                                {
                                    if (col is JValue)
                                    {
                                        //foreach (JValue col in column)
                                        //{
                                        //    // Only include JValue types
                                        if (col.Parent is JProperty)
                                        {
                                            JProperty jt = (JProperty)col.Parent;
                                            //if (col.Value is JValue)
                                            //{
                                            cleanRow.Add(jt.Name, jt.Value);
                                            //}
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        foreach (JProperty jt in col.Children<JProperty>())
                                        {
                                            if (jt.First is JValue)
                                                cleanRow.Add(jt.Name, jt.Value);
                                            else
                                            {
                                                foreach (JObject jb in jt.Children<JObject>())
                                                {
                                                    foreach (JProperty jt1 in jb.Children<JProperty>())
                                                        cleanRow.Add(jt1.Name, jt1.Value);
                                                }
                                            }

                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            else
            {
                JProperty jt = (JProperty)column.Parent;
                //if (col.Value is JValue)
                //{
                cleanRow.Add(jt.Name, jt.Value);
            }
            jarow.Add(cleanRow);
        }
        //cleanRows.Add(jarow);
        return jarow;
    }

    public string SHA256CheckSum(Stream fs)
    {
        using (SHA256 SHA256 = SHA256Managed.Create())
        {
            //using (FileStream fileStream = File.OpenRead(filePath))
            byte[] bt = SHA256.ComputeHash(fs);
            string str = "";
            foreach (byte b in bt)
                str += b.ToString("x2");
            return str;
        }
    }

    #endregion

    #region Anuja
    public string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    public DataTable CheckLogin(string un, string pass)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT username, mobileno, agencyname, role,id
            FROM agencyuser 
            WHERE (username = @un OR mobileno = @mobileno) 
            AND password = @password 
            AND status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@un", SqlDbType.VarChar).Value = un;
                    cmd.Parameters.Add("@mobileno", SqlDbType.VarChar).Value = un;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = EncryptString(pass);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }

            if (resultTable.Rows.Count > 0)
            {
                log.Info("Login successful for user: " + un);
            }
            else
            {
                log.Warn("Login failed for user: " + un + " (Invalid credentials)");
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while checking login.", sqlEx);
            return resultTable;
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return resultTable;
        }

        return resultTable;

    }

    public string Insertfilehash(string actualfilename, string filename, string filehash, string agencyname, string subdoctype)
    {
        try
        {
            string res = "[{\"_id\":\"filedetails\","
              + "\"fileid\":\"" + Guid.NewGuid() + "\","
              + "\"filename\":\"" + filename + "\","
               + "\"filehash\":\"" + filehash + "\","
               + "\"agencyname\":\"" + agencyname + "\","
                 + "\"subdoctype\":\"" + subdoctype + "\","
                   + "\"actualfilename\":\"" + actualfilename + "\","
                + "\"status\":\"Active\","
                + "\"createddate\":" + ConvertToTimestamp(DateTime.Now) + ""
               + "}]";
            string resp = sendTransaction(res, servertranurl);
            return resp;
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return "Error: " + ex.Message;
        }
    }
    public string Getfiledetails()
    {
        try
        {
            string res = "{"
    + "\"select\": {\"?stk\": ["
        + "\"_id\","
        + "\"subdoctype\","
        + "\"agencyname\","
        + "\"filename\","
        + "\"filehash\","
        + "\"actualfilename\","
        + "\"createddate\""
    + "]},"
    + "\"where\": ["
        + "[\"?stk\", \"filedetails/createddate\", \"?date\"]"
    + "],"
    + "\"order-by\": [[\"filedetails/createddate\", \"asc\"]],"
    + "\"limit\": 100"
 + "}";




            string resp = sendTransaction(res, serverqryurl);
            return resp;
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return "Error: " + ex.Message;
        }
    }

    public DataTable getactivitylogdata()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT *
                       FROM activitylog 
                       ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {


                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(resultTable);
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("❌ Database error in getdownfiledetails method: " + sqlEx.Message, sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("❌ Unexpected error in getdownfiledetails method: " + ex.Message, ex);
        }

        return resultTable;
    }
    //public string AddCSVData(string agencyname, string classtype, string category, string file_type, string faculty, string subject, string subjectcode, string barcodetop, string imagepath, string csvpath, string scannerid, string OMRString, string packetno)
    //{
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
    //    try
    //    {
    //        using (MySqlConnection connection = new MySqlConnection(connectionString))
    //        {
    //            connection.Open();
    //            string Insertquery = "Insert into CSVData (id,class,agencyname,category,file_type,faculty,subject,subject_code,barcode_top,imagepath,csvpath,scannerid,OMRString,packetno) " +
    //                "VALUES (@id, @class, @agencyname,@category,@file_type,@faculty,@subject,@subject_code,@barcode_top,@imagepath,@csvpath,@scannerid,@OMRString,@packetno);";
    //            using (MySqlCommand command = new MySqlCommand(Insertquery, connection))
    //            {
    //                Guid id = Guid.NewGuid();
    //                command.Parameters.AddWithValue("@id", id);
    //                command.Parameters.AddWithValue("@class", classtype);
    //                command.Parameters.AddWithValue("@agencyname", agencyname);
    //                command.Parameters.AddWithValue("@category", category);
    //                command.Parameters.AddWithValue("@file_type", file_type);
    //                command.Parameters.AddWithValue("@faculty", faculty);
    //                command.Parameters.AddWithValue("@subject", subject);
    //                command.Parameters.AddWithValue("@subject_code", subjectcode);
    //                command.Parameters.AddWithValue("@barcode_top", barcodetop);
    //                command.Parameters.AddWithValue("@imagepath", imagepath);
    //                command.Parameters.AddWithValue("@csvpath", csvpath);
    //                command.Parameters.AddWithValue("@scannerid", scannerid);
    //                command.Parameters.AddWithValue("@OMRString", OMRString);
    //                command.Parameters.AddWithValue("@packetno", packetno);
    //                MySqlParameter returnval = new MySqlParameter("@_ret", SqlDbType.TinyInt);
    //                returnval.Direction = ParameterDirection.Output;
    //                command.Parameters.Add(returnval);
    //                int rowsAffected = command.ExecuteNonQuery();
    //                connection.Close();
    //                return (rowsAffected > 0) ? "Success" : "No rows updated";
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        log.Error("An unexpected error occurred.", ex);
    //        throw new ApplicationException("An unexpected error occurred", ex);
    //    }
    //}
    // Method to check if the barcode exists in the main table
    public bool CheckForDuplicate(string filetype, string barcode)
    {
        if (filetype == "OMR Sheet")
        {
            string query = "SELECT COUNT(*) FROM [omr_sheet] WHERE BARCODE = @barcode"; // Replace with your actual table name
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // If count > 0, duplicate exists
            }
        }
        else if (filetype == "Practical Sheet")
        {
            string query = "SELECT COUNT(*) FROM [practical_sheet] WHERE BARCODE = @barcode";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // If count > 0, duplicate exists
            }
        }
        else if (filetype == "Award Sheet")
        {
            string query = "SELECT COUNT(*) FROM [award_sheet] WHERE BARCODE = @barcode";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }
        else if (filetype == "Foil Sheet")
        {
            string query = "SELECT COUNT(*) FROM [foil_sheet] WHERE BARCODE = @barcode";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }
        else if (filetype == "Flying Slip")
        {
            string query = "SELECT COUNT(*) FROM [flyingslip_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }


        else if (filetype == "Attendance A")
        {
            string query = "SELECT COUNT(*) FROM [attendanceA_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }

        else if (filetype == "Attendance B")
        {
            string query = "SELECT COUNT(*) FROM [attendanceB_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }

        else if (filetype == "Attendance B")
        {
            string query = "SELECT COUNT(*) FROM [attendanceB_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }

        else if (filetype == "Attendance")
        {
            string query = "SELECT COUNT(*) FROM [attendance_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }


        else if (filetype == "Absentee")
        {
            string query = "SELECT COUNT(*) FROM [absentee_sheet] WHERE BARCODE_TOP = @barcodetop";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcodetop", barcode);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // If count > 0, duplicate exists
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid file type: " + filetype); // Log the invalid file type
            return false; // Return a boolean value since the method expects bool
        }

    }

    public string SaveToDuplicateTable(string classname, string username, string agencyid, string agencyname, string doc_type, string sub_doc_type, string faculty, string subject, string subjectcode
, string csvpath, string csvname, string isactive, DataTable dt)

    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            if (!dt.Columns.Contains("classname") || !dt.Columns.Contains("username") || !dt.Columns.Contains("agencyid") ||
!dt.Columns.Contains("agencyname") || !dt.Columns.Contains("doc_type") || !dt.Columns.Contains("sub_doc_type") ||
!dt.Columns.Contains("faculty") || !dt.Columns.Contains("subject") || !dt.Columns.Contains("subjectcode") ||
!dt.Columns.Contains("isactive") || !dt.Columns.Contains("csvpath") || !dt.Columns.Contains("csvname"))
            {

                dt.Columns.Add("classname", typeof(string));
                dt.Columns.Add("username", typeof(string));
                dt.Columns.Add("agencyid", typeof(string));
                dt.Columns.Add("agencyname", typeof(string));
                dt.Columns.Add("doc_type", typeof(string));
                dt.Columns.Add("sub_doc_type", typeof(string));
                dt.Columns.Add("faculty", typeof(string));
                dt.Columns.Add("subject", typeof(string));
                dt.Columns.Add("subjectcode", typeof(string));
                dt.Columns.Add("isactive", typeof(string));
                dt.Columns.Add("csvpath", typeof(string));
                dt.Columns.Add("csvname", typeof(string));

            }
            if (!dt.Columns.Contains("createddate")) dt.Columns.Add("createddate", typeof(DateTime));
            if (!dt.Columns.Contains("updatedate")) dt.Columns.Add("updatedate", typeof(DateTime));

            if (!dt.Columns.Contains("duplicateORdiscrepancy")) dt.Columns.Add("duplicateORdiscrepancy", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                row["createddate"] = DateTime.Now;
                row["updatedate"] = DateTime.Now;


                row["duplicateORdiscrepancy"] = "Duplicate";
            }
            foreach (DataRow row in dt.Rows)
            {
                //row["id"] = id;
                row["classname"] = classname;
                row["username"] = username;
                row["agencyid"] = agencyid;
                row["agencyname"] = agencyname;
                row["doc_type"] = doc_type;
                row["sub_doc_type"] = sub_doc_type;
                row["faculty"] = faculty;
                row["subject"] = subject;
                row["subjectcode"] = subjectcode;
                row["isactive"] = "Active";
                row["csvpath"] = csvpath;
                row["csvname"] = csvname;

            }

            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    if (sub_doc_type == "OMR Sheet")
                    {
                        bool hasRequiredColumns = dt.Columns.Contains("BARCODE") &&
                              dt.Columns.Contains("SETCODE") &&
                              dt.Columns.Contains("OMR_STRING");

                        // ✅ Check if the DataTable contains only those 3 columns + system columns like classname, username, etc.
                        bool hasOnlyBasicColumns = hasRequiredColumns &&
                                                   dt.Columns.Count <= 18; // Adjust this based on known system fields

                        bool hasKidsOmrColumns = dt.Columns.Contains("SNO") &&
                                                  dt.Columns.Contains("BARCODE") &&
                                                  dt.Columns.Contains("BARCODE2") &&
                                                  dt.Columns.Contains("SETCODE") &&
                                                  dt.Columns.Contains("ANS");

                        bulkCopy.DestinationTableName = hasKidsOmrColumns ? "kids_omr_sheet_duplicate" : "omr_sheet_duplicate";

                        // ✅ Common column mappings (always added)
                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");

                        // ✅ If sheet contains only BARCODE, OMR_STRING, SETCODE (and system columns)
                        if (hasOnlyBasicColumns)
                        {
                            bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                            bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                            bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");

                            // Other columns will be NULL
                        }
                        // ✅ If sheet has kids OMR specific columns
                        else if (hasKidsOmrColumns)
                        {
                            bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                            bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                            bulkCopy.ColumnMappings.Add("SNO", "SNO");
                            bulkCopy.ColumnMappings.Add("BARCODE2", "BARCODE2");
                            bulkCopy.ColumnMappings.Add("ANS", "ANS");
                        }
                        // ✅ Default OMR sheet mappings (Full sheet)
                        else
                        {
                            bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                            bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                            bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                            bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                            bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                            bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                            bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                            bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                            bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                            bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                            bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                            bulkCopy.ColumnMappings.Add("FLG", "FLG");
                            bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                            bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                            bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                            bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                            bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                            bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                            bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                            bulkCopy.ColumnMappings.Add("SNO", "SNO");
                            bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                            bulkCopy.ColumnMappings.Add("SITTING", "SITTING");
                            bulkCopy.ColumnMappings.Add("SCHCD", "SCHCD");
                            bulkCopy.ColumnMappings.Add("NEWROLL", "NEWROLL");
                            bulkCopy.ColumnMappings.Add("SUBJECT_CODE", "SUBJECT_CODE");
                            bulkCopy.ColumnMappings.Add("REGNO", "REGNO");
                            bulkCopy.ColumnMappings.Add("ANS", "ANS");
                            bulkCopy.ColumnMappings.Add("MASFLG", "MASFLG");
                            bulkCopy.ColumnMappings.Add("REMARKS", "REMARKS");
                            bulkCopy.ColumnMappings.Add("CENT", "CENT");
                            bulkCopy.ColumnMappings.Add("KFLG", "KFLG");
                            bulkCopy.ColumnMappings.Add("S_ERROR", "S_ERROR");
                            bulkCopy.ColumnMappings.Add("DTCD", "DTCD");
                            bulkCopy.ColumnMappings.Add("MFLG", "MFLG");
                            bulkCopy.ColumnMappings.Add("FLYFLG", "FLYFLG");
                            bulkCopy.ColumnMappings.Add("DCFLG", "DCFLG");
                            bulkCopy.ColumnMappings.Add("DC_ANS", "DC_ANS");
                            bulkCopy.ColumnMappings.Add("MIS_CNT", "MIS_CNT");
                            bulkCopy.ColumnMappings.Add("MIS_QNO", "MIS_QNO");
                            bulkCopy.ColumnMappings.Add("K_ANS", "K_ANS");
                            bulkCopy.ColumnMappings.Add("FINFLG", "FINFLG");
                            bulkCopy.ColumnMappings.Add("FIN_ANS", "FIN_ANS");
                            bulkCopy.ColumnMappings.Add("CHKFLG", "CHKFLG");
                        }
                    }

                    //if (sub_doc_type == "OMR Sheet")
                    //{

                    //    bulkCopy.DestinationTableName = "omr_sheet_duplicate";


                    //    bulkCopy.ColumnMappings.Add("classname", "classname");
                    //    bulkCopy.ColumnMappings.Add("username", "username");
                    //    bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                    //    bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                    //    bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                    //    bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                    //    bulkCopy.ColumnMappings.Add("faculty", "faculty");
                    //    bulkCopy.ColumnMappings.Add("subject", "subject");
                    //    bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                    //    bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                    //    bulkCopy.ColumnMappings.Add("csvname", "csvname");
                    //    bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                    //    bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                    //    bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                    //    bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                    //    bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                    //    bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                    //    bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                    //    bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                    //    bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                    //    bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                    //    bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                    //    bulkCopy.ColumnMappings.Add("FLG", "FLG");
                    //    bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                    //    bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                    //    bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                    //    bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                    //    bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                    //    bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                    //    bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                    //    bulkCopy.ColumnMappings.Add("SNO", "SNO");
                    //    bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                    //    bulkCopy.ColumnMappings.Add("SITTING", "SITTING");
                    //    bulkCopy.ColumnMappings.Add("SCHCD", "SCHCD");
                    //    bulkCopy.ColumnMappings.Add("NEWROLL", "NEWROLL");
                    //    bulkCopy.ColumnMappings.Add("SUBJECT_CODE", "SUBJECT_CODE");
                    //    bulkCopy.ColumnMappings.Add("REGNO", "REGNO");
                    //    bulkCopy.ColumnMappings.Add("ANS", "ANS");
                    //    bulkCopy.ColumnMappings.Add("MASFLG", "MASFLG");
                    //    bulkCopy.ColumnMappings.Add("REMARKS", "REMARKS");
                    //    bulkCopy.ColumnMappings.Add("CENT", "CENT");
                    //    bulkCopy.ColumnMappings.Add("KFLG", "KFLG");
                    //    bulkCopy.ColumnMappings.Add("S_ERROR", "S_ERROR");
                    //    bulkCopy.ColumnMappings.Add("DTCD", "DTCD");
                    //    bulkCopy.ColumnMappings.Add("MFLG", "MFLG");
                    //    bulkCopy.ColumnMappings.Add("FLYFLG", "FLYFLG");
                    //    bulkCopy.ColumnMappings.Add("DCFLG", "DCFLG");
                    //    bulkCopy.ColumnMappings.Add("DC_ANS", "DC_ANS");
                    //    bulkCopy.ColumnMappings.Add("MIS_CNT", "MIS_CNT");
                    //    bulkCopy.ColumnMappings.Add("MIS_QNO", "MIS_QNO");
                    //    bulkCopy.ColumnMappings.Add("K_ANS", "K_ANS");
                    //    bulkCopy.ColumnMappings.Add("FINFLG", "FINFLG");
                    //    bulkCopy.ColumnMappings.Add("FIN_ANS", "FIN_ANS");
                    //    bulkCopy.ColumnMappings.Add("CHKFLG", "CHKFLG");
                    //    bulkCopy.ColumnMappings.Add("createddate", "createddate");
                    //    bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
                    //    bulkCopy.ColumnMappings.Add("isactive", "isactive");


                    //}

                    else if (sub_doc_type == "Practical Sheet")
                    {


                        bulkCopy.DestinationTableName = "practical_sheet_duplicate"; // Ensure this matches the table name you want to insert into



                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OCR1", "OCR1");
                        bulkCopy.ColumnMappings.Add("OCR2", "OCR2");
                        bulkCopy.ColumnMappings.Add("OCR3", "OCR3");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }

                    else if (sub_doc_type == "Award Sheet")
                    {


                        bulkCopy.DestinationTableName = "award_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("STATUS", "STATUS");
                        bulkCopy.ColumnMappings.Add("ADMIT_MASTER", "ADMIT_MASTER");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }

                    else if (sub_doc_type == "Foil Sheet")
                    {

                        bulkCopy.DestinationTableName = "foil_sheet_duplicate";  // Ensure this is the correct destination table name


                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("STATUS", "STATUS");
                        bulkCopy.ColumnMappings.Add("ADMIT_MASTER", "ADMIT_MASTER");
                        //bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }
                    else if (sub_doc_type == "Flying Slip")
                    {

                        bulkCopy.DestinationTableName = "flyingslip_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }

                    else if (sub_doc_type == "Attendance A")
                    {

                        bulkCopy.DestinationTableName = "attendanceA_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }
                    else if (sub_doc_type == "Attendance B")
                    {

                        bulkCopy.DestinationTableName = "attendanceB_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }
                    else if (sub_doc_type == "Absentee")
                    {

                        bulkCopy.DestinationTableName = "absentee_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }

                    else if (sub_doc_type == "Attendance")
                    {

                        bulkCopy.DestinationTableName = "attendance_sheet_duplicate";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }
                    else
                    {
                        return "Error: Invalid file type.";
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
                // Handle exceptions here, e.g., log the error
                // Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }


    //public string SaveToDuplicateTable(string filetype, string csv_maintable_id, DataRow row)
    //{
    //    try
    //    {
    //        string query = "";

    //        if (filetype == "OMR Sheet")
    //        {
    //            query = @"
    //    INSERT INTO [bseb].[dbo].[csv_omr_duplicate_sheet]
    //    ([PACKET_NO], [BARCODE], [BARCODE_BACK], [OMR_STRING], [OMR_STRING_BACK], 
    //    [OMR_STRING_R2], [OMR_STRING_R2_BACK], [IMAGEPATH], [IMAGEPATH_BACK], [ERROR_MESSAGE], 
    //    [ERROR_MESSAGE_BACK], [FLG], [FLGR], [BARCODEALL_FRONT], [BARCODEALL_BACK], [TEMPLATE], 
    //    [ERRFLG], [IMAGEPATH1], [IMAGEPATH_BACK1], [SNO], [SETCODE], [SITTING], [SCHCD], [NEWROLL], 
    //    [SUBJECT_CODE], [REGNO], [ANS], [MASFLG], [REMARKS], [CENT], [KFLG], [S_ERROR], [DTCD], 
    //    [MFLG], [FLYFLG], [DCFLG], [DC_ANS], [MIS_CNT], [MIS_QNO], [K_ANS], [FINFLG], [FIN_ANS], 
    //    [CHKFLG], [csv_maintable_id])
    //    VALUES 
    //    (@PACKET_NO, @BARCODE, @BARCODE_BACK, @OMR_STRING, @OMR_STRING_BACK, 
    //    @OMR_STRING_R2, @OMR_STRING_R2_BACK, @IMAGEPATH, @IMAGEPATH_BACK, @ERROR_MESSAGE, 
    //    @ERROR_MESSAGE_BACK, @FLG, @FLGR, @BARCODEALL_FRONT, @BARCODEALL_BACK, @TEMPLATE, 
    //    @ERRFLG, @IMAGEPATH1, @IMAGEPATH_BACK1, @SNO, @SETCODE, @SITTING, @SCHCD, @NEWROLL, 
    //    @SUBJECT_CODE, @REGNO, @ANS, @MASFLG, @REMARKS, @CENT, @KFLG, @S_ERROR, @DTCD, 
    //    @MFLG, @FLYFLG, @DCFLG, @DC_ANS, @MIS_CNT, @MIS_QNO, @K_ANS, @FINFLG, @FIN_ANS, 
    //    @CHKFLG, @csv_maintable_id)";
    //        }
    //        else if (filetype == "Practical Sheet")
    //        {
    //            query = @"
    //    INSERT INTO [bseb].[dbo].[csv_practical_duplicate]
    //    ([csv_maintable_id], [PACKET_NO], [BARCODE], [BARCODE_BACK], [OCR1], [OCR2], [OCR3], 
    //    [OMR_STRING], [OMR_STRING_BACK], [OMR_STRING_R2], [OMR_STRING_R2_BACK], 
    //    [IMAGEPATH], [IMAGEPATH_BACK], [ERROR_MESSAGE], [ERROR_MESSAGE_BACK], 
    //    [FLG], [FLGR], [BARCODEALL_FRONT], [BARCODEALL_BACK], [TEMPLATE], [ERRFLG], 
    //    [CreatedDate], [UpdatedDate])
    //    VALUES 
    //    (@csv_maintable_id, @PACKET_NO, @BARCODE, @BARCODE_BACK, @OCR1, @OCR2, @OCR3, 
    //    @OMR_STRING, @OMR_STRING_BACK, @OMR_STRING_R2, @OMR_STRING_R2_BACK, 
    //    @IMAGEPATH, @IMAGEPATH_BACK, @ERROR_MESSAGE, @ERROR_MESSAGE_BACK, 
    //    @FLG, @FLGR, @BARCODEALL_FRONT, @BARCODEALL_BACK, @TEMPLATE, @ERRFLG, 
    //    @CreatedDate, @UpdatedDate)";
    //        }
    //        else
    //        {
    //            query = @"
    //    INSERT INTO [bseb].[dbo].[csv_foil_duplicate_sheet]
    //    ([csv_maintable_id], [PACKET_NO], [BARCODE], [BARCODE_BACK], 
    //    [OMR_STRING], [OMR_STRING_BACK], [OMR_STRING_R2], [OMR_STRING_R2_BACK], 
    //    [IMAGEPATH], [IMAGEPATH_BACK], [ERROR_MESSAGE], [ERROR_MESSAGE_BACK], 
    //    [FLG], [FLGR], [BARCODEALL_FRONT], [BARCODEALL_BACK], [TEMPLATE], [ERRFLG], 
    //    [SNO], [IMAGEPATH1], [IMAGEPATH_BACK1], [STATUS], [ADMIT_MASTER], 
    //    [CreatedDate], [UpdatedDate])
    //    VALUES 
    //    (@csv_maintable_id, @PACKET_NO, @BARCODE, @BARCODE_BACK, 
    //    @OMR_STRING, @OMR_STRING_BACK, @OMR_STRING_R2, @OMR_STRING_R2_BACK, 
    //    @IMAGEPATH, @IMAGEPATH_BACK, @ERROR_MESSAGE, @ERROR_MESSAGE_BACK, 
    //    @FLG, @FLGR, @BARCODEALL_FRONT, @BARCODEALL_BACK, @TEMPLATE, @ERRFLG, 
    //    @SNO, @IMAGEPATH1, @IMAGEPATH_BACK1, @STATUS, @ADMIT_MASTER, 
    //    @CreatedDate, @UpdatedDate)";
    //        }

    //        using (SqlConnection conn = new SqlConnection(connectionString))
    //        {
    //            using (SqlCommand cmd = new SqlCommand(query, conn))
    //            {
    //                // Add parameters dynamically
    //                cmd.Parameters.AddWithValue("@csv_maintable_id", csv_maintable_id);
    //                foreach (var column in row.Table.Columns)
    //                {
    //                    var columnName = column.ToString();
    //                    var value = row[columnName] ?? DBNull.Value; // Handle NULL values
    //                    cmd.Parameters.AddWithValue("@" + columnName, value);
    //                }

    //                // Add CreatedDate and UpdatedDate for tables that require it
    //                if (query.Contains("@CreatedDate"))
    //                {
    //                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
    //                }

    //                conn.Open();
    //                cmd.ExecuteNonQuery();
    //            }
    //        }

    //        return "Success";
    //    }
    //    catch (SqlException sqlEx)
    //    {
    //        return "SQL Error: " + sqlEx.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        return "Error: " + ex.Message;
    //    }

    //}
    //    public string insertOMRsheet(string classname, string username, string agencyid, string agencyname, string doc_type, string sub_doc_type, string faculty, string subject, string subjectcode
    // , string csvpath, string csvname, string isactive, DataTable dt)
    //    {
    //        if (!dt.Columns.Contains("BARCODE") || !dt.Columns.Contains("OMR_STRING") || !dt.Columns.Contains("SETCODE"))
    //        {
    //            return "Missing required columns: BARCODE, OMR_STRING, or SETCODE.";
    //        }

    //        // ✅ Validate data before inserting
    //        foreach (DataRow row in dt.Rows)
    //        {
    //            string barcode = row["BARCODE"] != DBNull.Value ? row["BARCODE"].ToString().Trim() : "";
    //            string omrString = row["OMR_STRING"] != DBNull.Value ? row["OMR_STRING"].ToString().Trim() : "";
    //            string setcode = row["SETCODE"] != DBNull.Value ? row["SETCODE"].ToString().Trim() : "";

    //            if (barcode.Length < 6 || barcode.Length > 8)
    //            {
    //                return "BARCODE length must be between 6 and 8 digits.";
    //            }

    //            if (omrString.Length != 100)
    //            {
    //                return "OMR_STRING length must be exactly 100 characters.";
    //            }

    //            if (setcode.Length != 1)
    //            {
    //                return "SETCODE length must be exactly 1 characters.";
    //            }
    //        }
    //        if (!dt.Columns.Contains("classname") || !dt.Columns.Contains("username") || !dt.Columns.Contains("agencyid") ||
    //!dt.Columns.Contains("agencyname") || !dt.Columns.Contains("doc_type") || !dt.Columns.Contains("sub_doc_type") ||
    //!dt.Columns.Contains("faculty") || !dt.Columns.Contains("subject") || !dt.Columns.Contains("subjectcode") ||
    //!dt.Columns.Contains("isactive") || !dt.Columns.Contains("csvpath") || !dt.Columns.Contains("csvname"))
    //        {
    //            //dt.Columns.Add("id", typeof(Guid));
    //            dt.Columns.Add("classname", typeof(string));
    //            dt.Columns.Add("username", typeof(string));
    //            dt.Columns.Add("agencyid", typeof(string));
    //            dt.Columns.Add("agencyname", typeof(string));
    //            dt.Columns.Add("doc_type", typeof(string));
    //            dt.Columns.Add("sub_doc_type", typeof(string));
    //            dt.Columns.Add("faculty", typeof(string));
    //            dt.Columns.Add("subject", typeof(string));
    //            dt.Columns.Add("subjectcode", typeof(string));
    //            dt.Columns.Add("isactive", typeof(string));
    //            dt.Columns.Add("csvpath", typeof(string));
    //            dt.Columns.Add("csvname", typeof(string));

    //        }
    //        foreach (DataRow row in dt.Rows)
    //        {
    //            //row["id"] = id;
    //            row["classname"] = classname;
    //            row["username"] = username;
    //            row["agencyid"] = agencyid;
    //            row["agencyname"] = agencyname;
    //            row["doc_type"] = doc_type;
    //            row["sub_doc_type"] = sub_doc_type;
    //            row["faculty"] = faculty;
    //            row["subject"] = subject;
    //            row["subjectcode"] = subjectcode;
    //            row["isactive"] = "Active";
    //            row["csvpath"] = csvpath;
    //            row["csvname"] = csvname;


    //        }

    //        // ✅ Set created & updated date columns
    //        if (!dt.Columns.Contains("createddate")) dt.Columns.Add("createddate", typeof(DateTime));
    //        if (!dt.Columns.Contains("updatedate")) dt.Columns.Add("updatedate", typeof(DateTime));

    //        foreach (DataRow row in dt.Rows)
    //        {
    //            row["createddate"] = DateTime.Now;
    //            row["updatedate"] = DateTime.Now;
    //        }

    //        // ✅ Database Connection
    //        using (SqlConnection conn = new SqlConnection(connectionString))
    //        {
    //            conn.Open();
    //            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
    //            {
    //                // ✅ Determine the destination table
    //                if (sub_doc_type == "OMR Sheet")
    //                {
    //                    bool hasKidsOmrColumns = dt.Columns.Contains("SNO") &&
    //                                             dt.Columns.Contains("BARCODE") &&
    //                                             dt.Columns.Contains("BARCODE2") &&
    //                                             dt.Columns.Contains("SETCODE") &&
    //                                             dt.Columns.Contains("ANS");

    //                    bulkCopy.DestinationTableName = hasKidsOmrColumns ? "kids_omr_sheet" : "omr_sheet";

    //                    // ✅ Common column mappings
    //                    bulkCopy.ColumnMappings.Add("classname", "classname");
    //                    bulkCopy.ColumnMappings.Add("username", "username");
    //                    bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
    //                    bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
    //                    bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
    //                    bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
    //                    bulkCopy.ColumnMappings.Add("faculty", "faculty");
    //                    bulkCopy.ColumnMappings.Add("subject", "subject");
    //                    bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
    //                    bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
    //                    bulkCopy.ColumnMappings.Add("csvname", "csvname");
    //                    bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
    //                    bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
    //                    bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
    //                    bulkCopy.ColumnMappings.Add("createddate", "createddate");
    //                    bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
    //                    bulkCopy.ColumnMappings.Add("isactive", "isactive");

    //                    // ✅ Add specific mappings for `kids_omr_sheet`
    //                    if (bulkCopy.DestinationTableName == "kids_omr_sheet")
    //                    {
    //                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
    //                        bulkCopy.ColumnMappings.Add("BARCODE2", "BARCODE2");
    //                        bulkCopy.ColumnMappings.Add("ANS", "ANS");
    //                    }
    //                    else  // ✅ Default OMR sheet mappings
    //                    {
    //                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
    //                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
    //                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
    //                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
    //                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
    //                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
    //                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
    //                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
    //                        bulkCopy.ColumnMappings.Add("SITTING", "SITTING");
    //                        bulkCopy.ColumnMappings.Add("REGNO", "REGNO");
    //                        bulkCopy.ColumnMappings.Add("ANS", "ANS");
    //                    }
    //                }

    //                // ✅ Insert data into database
    //                bulkCopy.WriteToServer(dt);
    //            }
    //        }

    //        return "Success";

    //    }
    public string insertOMRsheet(string classname, string username, string agencyid, string agencyname, string doc_type, string sub_doc_type, string faculty, string subject, string subjectcode,
    string csvpath, string csvname, string isactive, DataTable dt, out DataTable duplicateTable)
    {
        duplicateTable = dt.Clone(); // Table for invalid records

        if (!dt.Columns.Contains("BARCODE") || !dt.Columns.Contains("OMR_STRING") || !dt.Columns.Contains("SETCODE"))
        {
            return "Missing required columns: BARCODE, OMR_STRING, or SETCODE.";
        }

        List<DataRow> validRecords = new List<DataRow>();
        string[] requiredColumns = {
    "classname", "username", "agencyid", "agencyname", "doc_type", "sub_doc_type",
    "faculty", "subject", "subjectcode", "isactive", "csvpath", "csvname",
    "createddate", "updatedate"
};

        // Add missing columns
        foreach (string column in requiredColumns)
        {
            if (!dt.Columns.Contains(column))
            {
                Type columnType = (column == "createddate" || column == "updatedate") ? typeof(DateTime) : typeof(string);
                dt.Columns.Add(column, columnType);
            }
        }

        // ✅ Process each row
        foreach (DataRow row in dt.Rows)
        {
            string barcode = row["BARCODE"] != DBNull.Value ? row["BARCODE"].ToString().Trim() : "";
            string omrString = row["OMR_STRING"] != DBNull.Value ? row["OMR_STRING"].ToString().Trim() : "";
            string setcode = row["SETCODE"] != DBNull.Value ? row["SETCODE"].ToString().Trim() : "";

            bool isValid = true;


            isValid = !(barcode.Length < 6 || barcode.Length > 8 || omrString.Length != 100 || setcode.Length != 1);


            // ✅ Assign values to new columns
            row["classname"] = classname;
            row["username"] = username;
            row["agencyid"] = agencyid;
            row["agencyname"] = agencyname;
            row["doc_type"] = doc_type;
            row["sub_doc_type"] = sub_doc_type;
            row["faculty"] = faculty;
            row["subject"] = subject;
            row["subjectcode"] = subjectcode;
            row["isactive"] = "Active";
            row["csvpath"] = csvpath;
            row["csvname"] = csvname;
            row["createddate"] = DateTime.Now;
            row["updatedate"] = DateTime.Now;

            // ✅ Store valid and invalid records separately
            if (!isValid)
            {
                duplicateTable.ImportRow(row);  // Add invalid record to duplicate table
            }
            else
            {
                validRecords.Add(row);
            }
        }


        // ✅ Process only valid records with your existing bulk copy logic
        if (validRecords.Count > 0)
        {
            DataTable validTable = dt.Clone();
            foreach (var row in validRecords) validTable.ImportRow(row);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    // 🔹 Your existing bulk insert logic remains unchanged
                    bool hasRequiredColumns = dt.Columns.Contains("BARCODE") &&
                           dt.Columns.Contains("SETCODE") &&
                           dt.Columns.Contains("OMR_STRING");

                    // ✅ Check if the DataTable contains only those 3 columns + system columns like classname, username, etc.
                    bool hasOnlyBasicColumns = hasRequiredColumns &&
                                               dt.Columns.Count <= 18; // Adjust this based on known system fields

                    bool hasKidsOmrColumns = dt.Columns.Contains("SNO") &&
                                              dt.Columns.Contains("BARCODE") &&
                                              dt.Columns.Contains("BARCODE2") &&
                                              dt.Columns.Contains("SETCODE") &&
                                              dt.Columns.Contains("ANS");

                    bulkCopy.DestinationTableName = hasKidsOmrColumns ? "kids_omr_sheet" : "omr_sheet";

                    // ✅ Common column mappings (always added)
                    bulkCopy.ColumnMappings.Add("classname", "classname");
                    bulkCopy.ColumnMappings.Add("username", "username");
                    bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                    bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                    bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                    bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                    bulkCopy.ColumnMappings.Add("faculty", "faculty");
                    bulkCopy.ColumnMappings.Add("subject", "subject");
                    bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                    bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                    bulkCopy.ColumnMappings.Add("csvname", "csvname");
                    bulkCopy.ColumnMappings.Add("createddate", "createddate");
                    bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
                    bulkCopy.ColumnMappings.Add("isactive", "isactive");

                    // ✅ If sheet contains only BARCODE, OMR_STRING, SETCODE (and system columns)
                    if (hasOnlyBasicColumns)
                    {
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");

                        // Other columns will be NULL
                    }
                    // ✅ If sheet has kids OMR specific columns
                    else if (hasKidsOmrColumns)
                    {
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE2", "BARCODE2");
                        bulkCopy.ColumnMappings.Add("ANS", "ANS");
                    }
                    // ✅ Default OMR sheet mappings (Full sheet)
                    else
                    {
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                        bulkCopy.ColumnMappings.Add("SITTING", "SITTING");
                        bulkCopy.ColumnMappings.Add("SCHCD", "SCHCD");
                        bulkCopy.ColumnMappings.Add("NEWROLL", "NEWROLL");
                        bulkCopy.ColumnMappings.Add("SUBJECT_CODE", "SUBJECT_CODE");
                        bulkCopy.ColumnMappings.Add("REGNO", "REGNO");
                        bulkCopy.ColumnMappings.Add("ANS", "ANS");
                        bulkCopy.ColumnMappings.Add("MASFLG", "MASFLG");
                        bulkCopy.ColumnMappings.Add("REMARKS", "REMARKS");
                        bulkCopy.ColumnMappings.Add("CENT", "CENT");
                        bulkCopy.ColumnMappings.Add("KFLG", "KFLG");
                        bulkCopy.ColumnMappings.Add("S_ERROR", "S_ERROR");
                        bulkCopy.ColumnMappings.Add("DTCD", "DTCD");
                        bulkCopy.ColumnMappings.Add("MFLG", "MFLG");
                        bulkCopy.ColumnMappings.Add("FLYFLG", "FLYFLG");
                        bulkCopy.ColumnMappings.Add("DCFLG", "DCFLG");
                        bulkCopy.ColumnMappings.Add("DC_ANS", "DC_ANS");
                        bulkCopy.ColumnMappings.Add("MIS_CNT", "MIS_CNT");
                        bulkCopy.ColumnMappings.Add("MIS_QNO", "MIS_QNO");
                        bulkCopy.ColumnMappings.Add("K_ANS", "K_ANS");
                        bulkCopy.ColumnMappings.Add("FINFLG", "FINFLG");
                        bulkCopy.ColumnMappings.Add("FIN_ANS", "FIN_ANS");
                        bulkCopy.ColumnMappings.Add("CHKFLG", "CHKFLG");
                    }

                    // ✅ Insert valid data into the database
                    bulkCopy.WriteToServer(validTable);
                }
            }
        }

        // ✅ If invalid records exist, return response to store them separately
        if (duplicateTable.Rows.Count > 0)
        {
            return "Invalid records found";
        }

        return "Success";
    }

    public void CheckForDuplicateOMR(string subdoctype, DataRow row, out bool isExactDuplicate, out bool isDiscrepancy)
    {
        isExactDuplicate = false;
        isDiscrepancy = false;

        string tableName = "omr_sheet";

        string query =
      "SELECT " +
      "   COUNT(*) AS ExactMatch, " +
      "   COUNT(CASE " +
      "           WHEN OMR_STRING <> @OMR_STRING " +
      "             OR SETCODE <> @SETCODE " +
      "           THEN 1 " +
      "           ELSE NULL " +
      "        END) AS Discrepancy " +
      "FROM " + tableName + " " +
      "WHERE BARCODE = @barcode";

        using (SqlConnection conn = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            // ✅ Add Parameters
            cmd.Parameters.AddWithValue("@barcode", row["BARCODE"].ToString());
            cmd.Parameters.AddWithValue("@OMR_STRING", row["OMR_STRING"].ToString());
            cmd.Parameters.AddWithValue("@SETCODE", row["SETCODE"].ToString());

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    isExactDuplicate = reader.GetInt32(0) > 0;
                    isDiscrepancy = reader.GetInt32(1) > 0;
                }
            }
        }
    }


    public string BulkInsert(string classname, string username, string agencyid, string agencyname, string doc_type, string sub_doc_type, string faculty, string subject, string subjectcode
 , string csvpath, string csvname, string isactive, DataTable dt)
    //public string BulkInsert(string filetype, string csv_maintable_id, DataTable dt)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();


            if (!dt.Columns.Contains("classname") || !dt.Columns.Contains("username") || !dt.Columns.Contains("agencyid") ||
!dt.Columns.Contains("agencyname") || !dt.Columns.Contains("doc_type") || !dt.Columns.Contains("sub_doc_type") ||
!dt.Columns.Contains("faculty") || !dt.Columns.Contains("subject") || !dt.Columns.Contains("subjectcode") ||
!dt.Columns.Contains("isactive") || !dt.Columns.Contains("csvpath") || !dt.Columns.Contains("csvname"))
            {
                //dt.Columns.Add("id", typeof(Guid));
                dt.Columns.Add("classname", typeof(string));
                dt.Columns.Add("username", typeof(string));
                dt.Columns.Add("agencyid", typeof(string));
                dt.Columns.Add("agencyname", typeof(string));
                dt.Columns.Add("doc_type", typeof(string));
                dt.Columns.Add("sub_doc_type", typeof(string));
                dt.Columns.Add("faculty", typeof(string));
                dt.Columns.Add("subject", typeof(string));
                dt.Columns.Add("subjectcode", typeof(string));
                dt.Columns.Add("isactive", typeof(string));
                dt.Columns.Add("csvpath", typeof(string));
                dt.Columns.Add("csvname", typeof(string));

            }
            if (!dt.Columns.Contains("createddate")) dt.Columns.Add("createddate", typeof(DateTime));
            if (!dt.Columns.Contains("updatedate")) dt.Columns.Add("updatedate", typeof(DateTime));

            foreach (DataRow row in dt.Rows)
            {
                row["createddate"] = DateTime.Now;
                row["updatedate"] = DateTime.Now;
            }
            foreach (DataRow row in dt.Rows)
            {
                //row["id"] = id;
                row["classname"] = classname;
                row["username"] = username;
                row["agencyid"] = agencyid;
                row["agencyname"] = agencyname;
                row["doc_type"] = doc_type;
                row["sub_doc_type"] = sub_doc_type;
                row["faculty"] = faculty;
                row["subject"] = subject;
                row["subjectcode"] = subjectcode;
                row["isactive"] = "Active";
                row["csvpath"] = csvpath;
                row["csvname"] = csvname;

            }

            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    if (sub_doc_type == "OMR Sheet")
                    {

                        bulkCopy.DestinationTableName = "omr_sheet";


                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("SETCODE", "SETCODE");
                        bulkCopy.ColumnMappings.Add("SITTING", "SITTING");
                        bulkCopy.ColumnMappings.Add("SCHCD", "SCHCD");
                        bulkCopy.ColumnMappings.Add("NEWROLL", "NEWROLL");
                        bulkCopy.ColumnMappings.Add("SUBJECT_CODE", "SUBJECT_CODE");
                        bulkCopy.ColumnMappings.Add("REGNO", "REGNO");
                        bulkCopy.ColumnMappings.Add("ANS", "ANS");
                        bulkCopy.ColumnMappings.Add("MASFLG", "MASFLG");
                        bulkCopy.ColumnMappings.Add("REMARKS", "REMARKS");
                        bulkCopy.ColumnMappings.Add("CENT", "CENT");
                        bulkCopy.ColumnMappings.Add("KFLG", "KFLG");
                        bulkCopy.ColumnMappings.Add("S_ERROR", "S_ERROR");
                        bulkCopy.ColumnMappings.Add("DTCD", "DTCD");
                        bulkCopy.ColumnMappings.Add("MFLG", "MFLG");
                        bulkCopy.ColumnMappings.Add("FLYFLG", "FLYFLG");
                        bulkCopy.ColumnMappings.Add("DCFLG", "DCFLG");
                        bulkCopy.ColumnMappings.Add("DC_ANS", "DC_ANS");
                        bulkCopy.ColumnMappings.Add("MIS_CNT", "MIS_CNT");
                        bulkCopy.ColumnMappings.Add("MIS_QNO", "MIS_QNO");
                        bulkCopy.ColumnMappings.Add("K_ANS", "K_ANS");
                        bulkCopy.ColumnMappings.Add("FINFLG", "FINFLG");
                        bulkCopy.ColumnMappings.Add("FIN_ANS", "FIN_ANS");
                        bulkCopy.ColumnMappings.Add("CHKFLG", "CHKFLG");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");

                    }

                    else if (sub_doc_type == "Practical Sheet")
                    {


                        bulkCopy.DestinationTableName = "practical_sheet"; // Ensure this matches the table name you 

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OCR1", "OCR1");
                        bulkCopy.ColumnMappings.Add("OCR2", "OCR2");
                        bulkCopy.ColumnMappings.Add("OCR3", "OCR3");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }

                    else if (sub_doc_type == "Award Sheet")
                    {


                        bulkCopy.DestinationTableName = "award_sheet";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("STATUS", "STATUS");
                        bulkCopy.ColumnMappings.Add("ADMIT_MASTER", "ADMIT_MASTER");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");

                    }
                    else if (sub_doc_type == "Foil Sheet")
                    {

                        bulkCopy.DestinationTableName = "foil_sheet";  // Ensure this is the correct destination table name

                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("STATUS", "STATUS");
                        bulkCopy.ColumnMappings.Add("ADMIT_MASTER", "ADMIT_MASTER");
                        //bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }
                    else if (sub_doc_type == "Award Sheet")
                    {

                        bulkCopy.DestinationTableName = "award_sheet";  // Ensure this is the correct destination table name

                        bulkCopy.ColumnMappings.Add("PACKET_NO", "PACKET_NO");
                        bulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                        bulkCopy.ColumnMappings.Add("BARCODE_BACK", "BARCODE_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_BACK", "OMR_STRING_BACK");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2", "OMR_STRING_R2");
                        bulkCopy.ColumnMappings.Add("OMR_STRING_R2_BACK", "OMR_STRING_R2_BACK");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK", "IMAGEPATH_BACK");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE", "ERROR_MESSAGE");
                        bulkCopy.ColumnMappings.Add("ERROR_MESSAGE_BACK", "ERROR_MESSAGE_BACK");
                        bulkCopy.ColumnMappings.Add("FLG", "FLG");
                        bulkCopy.ColumnMappings.Add("FLGR", "FLGR");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_FRONT", "BARCODEALL_FRONT");
                        bulkCopy.ColumnMappings.Add("BARCODEALL_BACK", "BARCODEALL_BACK");
                        bulkCopy.ColumnMappings.Add("TEMPLATE", "TEMPLATE");
                        bulkCopy.ColumnMappings.Add("ERRFLG", "ERRFLG");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH1", "IMAGEPATH1");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH_BACK1", "IMAGEPATH_BACK1");
                        bulkCopy.ColumnMappings.Add("STATUS", "STATUS");
                        bulkCopy.ColumnMappings.Add("ADMIT_MASTER", "ADMIT_MASTER");
                        //bulkCopy.ColumnMappings.Add("id", "id");
                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }
                    else if (sub_doc_type == "Flying Slip")
                    {

                        bulkCopy.DestinationTableName = "flyingslip_sheet";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");


                    }
                    else if (sub_doc_type == "Attendance A")
                    {

                        bulkCopy.DestinationTableName = "attendanceA_sheet";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");



                    }

                    else if (sub_doc_type == "Attendance B")
                    {

                        bulkCopy.DestinationTableName = "attendanceB_sheet";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }

                    else if (sub_doc_type == "Absentee")
                    {

                        bulkCopy.DestinationTableName = "absentee_sheet";

                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");

                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");




                    }

                    else if (sub_doc_type == "Attendance")
                    {

                        bulkCopy.DestinationTableName = "attendance_sheet";


                        bulkCopy.ColumnMappings.Add("classname", "classname");
                        bulkCopy.ColumnMappings.Add("username", "username");
                        bulkCopy.ColumnMappings.Add("agencyid", "agencyid");
                        bulkCopy.ColumnMappings.Add("agencyname", "agencyname");
                        bulkCopy.ColumnMappings.Add("doc_type", "doc_type");
                        bulkCopy.ColumnMappings.Add("sub_doc_type", "sub_doc_type");
                        bulkCopy.ColumnMappings.Add("faculty", "faculty");
                        bulkCopy.ColumnMappings.Add("subject", "subject");
                        bulkCopy.ColumnMappings.Add("subjectcode", "subjectcode");
                        bulkCopy.ColumnMappings.Add("csvpath", "csvpath");
                        bulkCopy.ColumnMappings.Add("isactive", "isactive");
                        bulkCopy.ColumnMappings.Add("csvname", "csvname");
                        bulkCopy.ColumnMappings.Add("SNO", "SNO");
                        bulkCopy.ColumnMappings.Add("BARCODE_TOP", "BARCODE_TOP");
                        bulkCopy.ColumnMappings.Add("IMAGEPATH", "IMAGEPATH");
                        bulkCopy.ColumnMappings.Add("SCANNER_ID", "SCANNER_ID");
                        bulkCopy.ColumnMappings.Add("OMR_STRING", "OMR_STRING");
                        bulkCopy.ColumnMappings.Add("createddate", "createddate");
                        bulkCopy.ColumnMappings.Add("updatedate", "updatedate");

                    }

                    else
                    {
                        return "Error: Invalid file type.";
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
                // Handle exceptions here, e.g., log the error
                // Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
    //public string AddCSVMainData(string id, string classname, string agencyname, string file_type, string faculty, string subject, string subjectcode, string csvpath, string csvname, string username)
    //{
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
    //    try
    //    {
    //        using (SqlConnection connection = new SqlConnection(connectionString))
    //        {
    //            connection.Open();

    //            string query = @"INSERT INTO csv_maintable 
    //  (id, class, agencyname, file_type, faculty, subject, subject_code, csvpath, status, createddate, updatedate, csvname,username)
    //  VALUES 
    //  (@id, @class, @agencyname, @file_type, @faculty, @subject, @subject_code, @csvpath, @status, @createddate, @updatedate, @csvname,@username)";

    //            using (SqlCommand cmd = new SqlCommand(query, connection))
    //            {
    //                cmd.Parameters.AddWithValue("@id", id);
    //                cmd.Parameters.AddWithValue("@class", classname);
    //                cmd.Parameters.AddWithValue("@agencyname", agencyname);
    //                cmd.Parameters.AddWithValue("@file_type", file_type);
    //                cmd.Parameters.AddWithValue("@faculty", faculty);
    //                cmd.Parameters.AddWithValue("@subject", subject);
    //                cmd.Parameters.AddWithValue("@subject_code", subjectcode);
    //                cmd.Parameters.AddWithValue("@csvpath", csvpath);
    //                cmd.Parameters.AddWithValue("@status", "Active");
    //                cmd.Parameters.AddWithValue("@createddate", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updatedate", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@csvname", csvname);
    //                cmd.Parameters.AddWithValue("@username", username);

    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        log.Error("An unexpected error occurred.", ex);
    //        throw new ApplicationException("An unexpected error occurred", ex);
    //    }
    //    string responseMessage = "Success";
    //    return responseMessage;
    //}

    public string InsertProcessFileDetails(string filename, string dbFilePath, string agency, string uploadedby)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO ProcessedFiles (Filename, FilePath, UploadedBy, Agency, IsProcessed, UploadedOn) " +
                               "VALUES (@Filename, @FilePath, @UploadedBy, @Agency, 0, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Filename", filename);
                    cmd.Parameters.AddWithValue("@FilePath", dbFilePath);
                    cmd.Parameters.AddWithValue("@UploadedBy", uploadedby);
                    cmd.Parameters.AddWithValue("@Agency", agency);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        log.Info("File details inserted successfully. Filename: " + filename + ", UploadedBy: " + uploadedby);
                        return "File details inserted successfully.";
                    }
                    else
                    {
                        log.Warn("Failed to insert file details. Filename: " + filename + ", UploadedBy: " + uploadedby);
                        return "Failed to insert file details.";
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error while inserting file details. Filename: " + filename + ", Error: " + sqlEx.Message, sqlEx);
            return "Database error: " + sqlEx.Message;
        }
        catch (Exception ex)
        {
            log.Error("Unexpected error while inserting file details. Filename: " + filename + ", Error: " + ex.Message, ex);
            return "Error: " + ex.Message;
        }
    }

    public string Insertactivitylog(string userid, string ipaddress, string deviceused, string type, string filename, string agencyname)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO activitylog (userid, ipaddress, deviceused, type, filename,agencyname) " +
                           "VALUES (@userid, @ipaddress, @deviceused, @type,@filename, @agencyname )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@deviceused", deviceused);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@agencyname", agencyname);
                    cmd.Parameters.AddWithValue("@createddate", DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        log.Info("Activity log inserted successfully. UserID: " + userid + ", IP: " + ipaddress + ", Type: " + type);
                        return "Log details inserted successfully.";
                    }
                    else
                    {
                        log.Warn("Failed to insert activity log. UserID: " + userid + ", IP: " + ipaddress + ", Type: " + type);
                        return "Failed to insert Log details.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    public DataTable GetProcessFileList()
    {
        DataTable dt = new DataTable();

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM ProcessedFiles WHERE IsProcessed = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error message
            Console.WriteLine("Error: " + ex.Message);
        }

        return dt;
    }

    public DataTable getdownfiledetailspending(string agencyname)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT id, filename, filepath, download, downloadedby, agency, foldertype 
                          FROM downloadfiledetail 
                          WHERE agency = @agency AND download = @download AND foldertype = @foldertype";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@agency", agencyname);
                    cmd.Parameters.AddWithValue("@download", 1);
                    cmd.Parameters.AddWithValue("@foldertype", "Pending");

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(resultTable);
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("❌ Database error in getdownfiledetails method: " + sqlEx.Message, sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("❌ Unexpected error in getdownfiledetails method: " + ex.Message, ex);
        }

        return resultTable;
    }

    public DataTable GetTotalCountsDashboard(string agencyName)
    {
        string query = @"
            SELECT 
                (SELECT COUNT(*) FROM [dbo].[omr_sheet] WHERE agencyname = @AgencyName) AS OMR_Sheet_Count, 
                (SELECT COUNT(*) FROM [dbo].[omr_sheet_duplicate] WHERE agencyname = @AgencyName) AS OMR_Sheet_Duplicate_Count,
                (SELECT COUNT(*) FROM [dbo].[absentee_sheet] WHERE agencyname = @AgencyName) AS Absentee_Sheet_Count,
                (SELECT COUNT(*) FROM [dbo].[attendance_sheet] WHERE agencyname = @AgencyName) AS Attendance_Sheet_Count,
                (SELECT COUNT(*) FROM [dbo].[foil_sheet] WHERE agencyname = @AgencyName) AS Foil_Sheet_Count,
                (SELECT COUNT(*) FROM [dbo].[practical_sheet] WHERE agencyname = @AgencyName) AS Practical_Sheet_Count,
                ((SELECT COUNT(*) FROM [dbo].[omr_sheet] WHERE agencyname = @AgencyName) +
                 (SELECT COUNT(*) FROM [dbo].[omr_sheet_duplicate] WHERE agencyname = @AgencyName) + 
                 (SELECT COUNT(*) FROM [dbo].[absentee_sheet] WHERE agencyname = @AgencyName) +
                 (SELECT COUNT(*) FROM [dbo].[attendance_sheet] WHERE agencyname = @AgencyName) +
                 (SELECT COUNT(*) FROM [dbo].[foil_sheet] WHERE agencyname = @AgencyName) +
                 (SELECT COUNT(*) FROM [dbo].[practical_sheet] WHERE agencyname = @AgencyName) 
                 " + (agencyName == "Datacon" ? "- 15000" : "") + @"
                ) AS AdjustedTotalRecords;";

        DataTable dt = new DataTable();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@AgencyName", agencyName);

                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
        }

        return dt;
    }

    #endregion

    #region Meet
    public string Register_User(string txt_UN, string txt_AgencyName, string txt_Email, string txt_Phone, string txt_Password)
    {
        // Get the connection string from the configuration file (appsettings.json or web.config)
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        // Encrypt the password before storing it
        string encryptedPassword = EncryptString(txt_Password);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // SQL Insert Query for MSSQL
            string InsertQuery = "INSERT INTO agencyuser (username, email, mobileno, agencyname, password, role, status) " +
                                 "VALUES (@username, @email, @mobileno, @agencyname, @password, @role, @status);";

            using (SqlCommand command = new SqlCommand(InsertQuery, connection))
            {
                // Add parameters to match the query placeholders
                command.Parameters.AddWithValue("@username", txt_UN);
                command.Parameters.AddWithValue("@email", txt_Email);
                command.Parameters.AddWithValue("@mobileno", txt_Phone);
                command.Parameters.AddWithValue("@agencyname", txt_AgencyName);
                command.Parameters.AddWithValue("@password", encryptedPassword); // Use the encrypted password
                command.Parameters.AddWithValue("@role", "Agency"); // You can modify this as needed
                command.Parameters.AddWithValue("@status", "Pending For Approval"); // You can modify this as needed

                // Execute the insert query
                int result = command.ExecuteNonQuery();

                // Return a success or error message based on whether rows were affected
                return (result > 0) ? "Success" : "Error";
            }
        }
    }
    //public DataTable getcsvdata(string agencyname)
    //{
    //    DataTable resultTable = new DataTable();
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    //    using (SqlConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();
    //        string query = @"
    //SELECT 
    //    m.file_type,
    //    COUNT(d.csv_maintable_id) AS total_duplicates,
    //    COUNT(s.id) AS total_omr_count
    //FROM csv_maintable m
    //LEFT JOIN csv_omr_duplicate_sheet d 
    //    ON m.id = d.csv_maintable_id 
    //    AND d.createddate >= @StartDate  -- Apply dynamic date filter on duplicate sheet
    //LEFT JOIN csv_omr_sheet s 
    //    ON m.id = s.csv_maintable_id 
    //    AND s.createddate >= @StartDate  -- Apply dynamic date filter on OMR sheet
    //WHERE m.agencyname = @Username
    //AND m.file_type = 'OMR Sheet'  -- Ensures only OMR-related records are included
    //GROUP BY m.file_type;";

    //        using (SqlCommand command = new SqlCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("@Username", agencyname);
    //            command.Parameters.AddWithValue("@StartDate", DateTime.Now.Date); // Pass current date dynamically

    //            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
    //            {
    //                adapter.Fill(resultTable);
    //            }
    //        }

    //    }
    //    return resultTable;
    //}
    //public DataTable getpracticaldata(string agencyname)
    //{
    //    DataTable resultTable = new DataTable();
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    //    using (SqlConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();
    //        string query = @"
    //SELECT 
    //    m.file_type,
    //    COUNT(d.csv_maintable_id) AS total_duplicates,
    //    COUNT(s.id) AS total_practical_count
    //FROM csv_maintable m
    //LEFT JOIN csv_practical_duplicate d 
    //    ON m.id = d.csv_maintable_id 
    //    AND d.createddate >= @StartDate  -- Apply dynamic date filter on duplicate sheet
    //LEFT JOIN csv_practical s 
    //    ON m.id = s.csv_maintable_id 
    //    AND s.createddate >= @StartDate  -- Apply dynamic date filter on Practical sheet
    //WHERE m.agencyname = @agencyname
    //AND m.file_type = 'Practical Sheet'  -- Ensures only Practical-related records are included
    //GROUP BY m.file_type;";

    //        using (SqlCommand command = new SqlCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("@agencyname", agencyname);
    //            command.Parameters.AddWithValue("@StartDate", DateTime.Now.Date); // Pass current date dynamically

    //            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
    //            {
    //                adapter.Fill(resultTable);
    //            }
    //        }

    //    }
    //    return resultTable;
    //}

    //public DataTable getawarfoildata(string agencyname)
    //{
    //    DataTable resultTable = new DataTable();
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    //    using (SqlConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();
    //        string query = @"
    //SELECT 
    //    m.file_type,
    //    COUNT(CASE WHEN m.file_type = 'Award Sheet' THEN d.csv_maintable_id END) AS award_sheet_duplicates,
    //    COUNT(CASE WHEN m.file_type = 'Foil Sheet' THEN d.csv_maintable_id END) AS foil_sheet_duplicates,
    //    COUNT(d.csv_maintable_id) AS total_duplicates, 
    //    SUM(CASE WHEN m.file_type = 'Award Sheet' THEN 1 ELSE 0 END) AS award_sheet_count,
    //    SUM(CASE WHEN m.file_type = 'Foil Sheet' THEN 1 ELSE 0 END) AS foil_sheet_count
    //FROM csv_maintable m
    //LEFT JOIN csv_foil_duplicate_sheet d 
    //    ON m.id = d.csv_maintable_id 
    //    AND d.createddate >= @StartDate  -- Apply date filter on duplicate sheet
    //LEFT JOIN csv_foil_sheet s 
    //    ON m.id = s.csv_maintable_id 
    //    AND s.createddate >= @StartDate  -- Apply date filter on Foil sheet
    //WHERE m.agencyname = @agencyname
    //AND m.file_type IN ('Award Sheet', 'Foil Sheet')  -- Explicitly filter for required file types
    //GROUP BY m.file_type;";

    //        using (SqlCommand command = new SqlCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("@agencyname", agencyname);
    //            command.Parameters.AddWithValue("@StartDate", DateTime.Now.Date); // Current date

    //            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
    //            {
    //                adapter.Fill(resultTable);
    //            }
    //        }

    //    }
    //    return resultTable;
    //}


    //public DataTable GetallUplaod(string agencyname)
    //{
    //    DataTable resultTable = new DataTable();
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    //    using (SqlConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();
    //        string query = @"SELECT COUNT(*) AS Totalupload FROM csv_maintable WHERE agencyname = @agencyname;";

    //        using (SqlCommand command = new SqlCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("@agencyname", agencyname); // Use the correct parameter name

    //            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
    //            {
    //                adapter.Fill(resultTable);
    //            }
    //        }
    //    }

    //    return resultTable;
    //}
    public string ChangePassword(string userid, string password)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                UPDATE agencyuser
                SET password = @newPassword
                WHERE id = @userid
                AND status = 'Active';";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.Parameters.Add("@newPassword", SqlDbType.VarChar).Value = EncryptString(password); // Encrypt the password before storing

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        log.Info("Password changed successfully for UserID: " + userid);
                        return "Password updated successfully.";
                    }
                    else
                    {
                        log.Warn("Failed to change password. User not found or inactive. UserID: " + userid);
                        return "User not found or inactive.";
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while changing password.", sqlEx);
            return "Database error. Please try again later.";
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return "An error occurred. Please try again.";
        }
    }
    #endregion

    #region Apeksha


    #endregion

    #region shubhanshu
    public string getActualdata_agencydatewise(string agencyName, string createdDate, out int totalRecords)
    {
        totalRecords = 0;
        string result = "0"; // Default response if no data found

        // Base query
        string query = @"
    SELECT 
        (
            (SELECT COUNT(*) FROM [dbo].[absentee_sheet] WHERE {CONDITION}) +    
            (SELECT COUNT(*) FROM [dbo].[attendance_sheet] WHERE {CONDITION}) +    
            (SELECT COUNT(*) FROM [dbo].[attendanceA_sheet] WHERE {CONDITION}) +  
            (SELECT COUNT(*) FROM [dbo].[attendanceB_sheet] WHERE {CONDITION}) +  
            (SELECT COUNT(*) FROM [dbo].[award_sheet] WHERE {CONDITION}) +   
            (SELECT COUNT(*) FROM [dbo].[flyingslip_sheet] WHERE {CONDITION}) + 
            (SELECT COUNT(*) FROM [dbo].[foil_sheet] WHERE {CONDITION}) + 
            (SELECT COUNT(*) FROM [dbo].[omr_sheet] WHERE {CONDITION}) + 
            (SELECT COUNT(*) FROM [dbo].[practical_sheet] WHERE {CONDITION})
        ) AS TotalRecords";

        // Dynamic condition based on agencyName
        string condition;
        if (!string.IsNullOrEmpty(agencyName))
        {
            condition = "agencyname = @AgencyName AND createddate >= @FromDate";
        }
        else
        {
            condition = "createddate >= @FromDate"; // Only filter by date
        }

        // Replace condition in query
        query = query.Replace("{CONDITION}", condition);

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Add parameters
                cmd.Parameters.AddWithValue("@FromDate", DateTime.ParseExact(createdDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));

                if (!string.IsNullOrEmpty(agencyName))
                {
                    cmd.Parameters.AddWithValue("@AgencyName", agencyName);
                }

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        totalRecords = reader.GetInt32(0);
                        result = totalRecords.ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result = "Error: " + ex.Message; // Return error message if exception occurs
        }

        return result;
    }

    public DataTable dashboardsheetwisecount(string agencyname, string fromdate, out int totalRecords, out int duplicateRecords)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("OmrCount", typeof(int));
        dt.Columns.Add("OmrDuplicateCount", typeof(int));
        dt.Columns.Add("AbsenteeCount", typeof(int));
        dt.Columns.Add("AbsenteeDuplicateCount", typeof(int));
        dt.Columns.Add("AttendanceCount", typeof(int));
        dt.Columns.Add("AttendanceDuplicateCount", typeof(int));
        dt.Columns.Add("PracticalCount", typeof(int));
        dt.Columns.Add("PracticalDuplicateCount", typeof(int));
        dt.Columns.Add("AwardCount", typeof(int));
        dt.Columns.Add("AwardDuplicateCount", typeof(int));
        dt.Columns.Add("AttendanceACount", typeof(int));
        dt.Columns.Add("AttendanceADuplicateCount", typeof(int));
        dt.Columns.Add("AttendanceBCount", typeof(int));
        dt.Columns.Add("AttendanceBDuplicateCount", typeof(int));
        dt.Columns.Add("FoilCount", typeof(int));
        dt.Columns.Add("FoilDuplicateCount", typeof(int));
        dt.Columns.Add("FlyingCount", typeof(int));
        dt.Columns.Add("FlyingDuplicateCount", typeof(int));

        totalRecords = 0;
        duplicateRecords = 0;

        string query = @"
    SELECT 
        (SELECT COUNT(*) FROM [dbo].[omr_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS OmrCount,
        (SELECT COUNT(*) FROM [dbo].[omr_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS OmrDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[absentee_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AbsenteeCount,
        (SELECT COUNT(*) FROM [dbo].[absentee_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AbsenteeDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[attendance_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceCount,
        (SELECT COUNT(*) FROM [dbo].[attendance_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[practical_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS PracticalCount,
        (SELECT COUNT(*) FROM [dbo].[practical_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS PracticalDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[award_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AwardCount,
        (SELECT COUNT(*) FROM [dbo].[award_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AwardDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[attendanceA_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceACount,
        (SELECT COUNT(*) FROM [dbo].[attendanceA_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceADuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[attendanceB_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceBCount,
        (SELECT COUNT(*) FROM [dbo].[attendanceB_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS AttendanceBDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[foil_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS FoilCount,
        (SELECT COUNT(*) FROM [dbo].[foil_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS FoilDuplicateCount,
        (SELECT COUNT(*) FROM [dbo].[flyingslip_sheet] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS FlyingCount,
        (SELECT COUNT(*) FROM [dbo].[flyingslip_sheet_duplicate] WHERE agencyname = @AgencyName AND CAST(createddate AS DATE) = @FromDate) AS FlyingDuplicateCount";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DateTime fromDateValue;
                if (!DateTime.TryParseExact(fromdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                {
                    throw new ArgumentException("Invalid date format. Expected yyyy-MM-dd.");
                }

                cmd.Parameters.AddWithValue("@AgencyName", agencyname);
                cmd.Parameters.AddWithValue("@FromDate", fromDateValue);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DataRow row = dt.NewRow();

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            row[dt.Columns[i].ColumnName] = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        }

                        dt.Rows.Add(row);

                        // ✅ Sum Up Total and Duplicate Records
                        totalRecords = row.ItemArray.Where((_, index) => index % 2 == 0).Sum(val => Convert.ToInt32(val));  // Sum even indexes (original records)
                        duplicateRecords = row.ItemArray.Where((_, index) => index % 2 == 1).Sum(val => Convert.ToInt32(val)); // Sum odd indexes (duplicate records)
                    }
                }
            }
        }
        return dt;
    }
    #endregion

    #region Apeksha

    public DataTable FacultyDropDown()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
           SELECT Id, facultyname, createddate,updateddate
           FROM Faculty 
           WHERE status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);  // Populate DataTable with query result
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while checking login.", sqlEx);
            return resultTable;  // Return empty DataTable in case of error
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return resultTable;  // Return empty DataTable in case of error
        }

        return resultTable;

    }

    public DataTable SubjectDDByFacultyId(int facultyId)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
           SELECT Id, FacultyId, subjectcode, subjectname, createddate, updateddate
           FROM Subjects
           WHERE FacultyId = @facultyId AND status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@FacultyId", SqlDbType.VarChar).Value = facultyId;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);  // Populate DataTable with query result
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while checking login.", sqlEx);
            return resultTable;  // Return empty DataTable in case of error
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return resultTable;  // Return empty DataTable in case of error
        }

        return resultTable;

    }
    public DataTable Subjectcodebysubjectname(string subjectname)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
           SELECT Id, FacultyId, subjectcode, subjectname, createddate, updateddate
           FROM Subjects
           WHERE subjectname = @subjectname AND status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@subjectname", subjectname);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);  // Populate DataTable with query result
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while checking login.", sqlEx);
            return resultTable;  // Return empty DataTable in case of error
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return resultTable;  // Return empty DataTable in case of error
        }

        return resultTable;

    }
    public bool CheckUserExist(string username)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = @"SELECT COUNT(*) FROM agencyuser WHERE username = @username;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                int count = (int)command.ExecuteScalar();
                log.Info("Query Executed");

                return count > 0;
            }
        }
    }
    public DataTable FindUser(string agencyname = null, string UserStatus = null)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM agencyuser WHERE 1=1"; // Ensures WHERE clause always exists

                if (!string.IsNullOrEmpty(agencyname))
                {
                    query += " AND agencyname LIKE @agencyname";
                }
                if (!string.IsNullOrEmpty(UserStatus) && UserStatus != "ALL")
                {
                    query += " AND status = @UserStatus";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(agencyname))
                    {
                        command.Parameters.AddWithValue("@agencyname", "%" + agencyname + "%"); // Partial search
                    }
                    if (!string.IsNullOrEmpty(UserStatus) && UserStatus != "ALL") // Ignore 'ALL'
                    {
                        command.Parameters.AddWithValue("@UserStatus", UserStatus);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(resultTable);
                    }
                }
                log.Info("FindUser executed successfully. Filters - AgencyName: " + agencyname + ", UserStatus: " + UserStatus + ", Rows Returned: " + resultTable.Rows.Count);
            }
        }
        catch (SqlException ex)
        {
            log.Error("Database error in FindUser. AgencyName: " + agencyname + ", UserStatus: " + UserStatus + ", Error: " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            log.Error("Unexpected error in FindUser. AgencyName: " + agencyname + ", UserStatus: " + UserStatus + ", Error: " + ex.Message, ex);
        }

        return resultTable;
    }


    public DataTable findUserForApprove(string username = null, string UserStatus = null)
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM agencyuser WHERE 1=1"; // Ensures WHERE clause always exists

            if (!string.IsNullOrEmpty(username))
            {
                query += " AND username LIKE @username";
            }
            if (!string.IsNullOrEmpty(UserStatus) && UserStatus != "ALL") // Ignore 'ALL'
            {
                query += " AND status = @UserStatus";
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    command.Parameters.AddWithValue("@username", "%" + username + "%"); // Partial search
                }
                if (!string.IsNullOrEmpty(UserStatus) && UserStatus != "ALL") // Ignore 'ALL'
                {
                    command.Parameters.AddWithValue("@UserStatus", UserStatus);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(resultTable);
                }
            }
        }

        return resultTable;
    }

    public string Updateagencyuserstatus(string userid, string status)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string UpdateQuery = @"
                UPDATE agencyuser  
                SET status = @status
                WHERE id = @id;";

                using (SqlCommand command = new SqlCommand(UpdateQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", userid);
                    command.Parameters.AddWithValue("@status", status);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        log.Info("User status updated successfully. UserID: " + userid + ", New Status: " + status);
                        return "Success";
                    }
                    else
                    {
                        log.Warn("No rows updated. UserID: " + userid + ", Status: " + status);
                        return "No rows updated";
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            log.Error("Database error while updating user status. UserID: " + userid + ", Error: " + ex.Message, ex);
            return "Database error: " + ex.Message;
        }
        catch (Exception ex)
        {
            log.Error("Unexpected error while updating user status. UserID: " + userid + ", Error: " + ex.Message, ex);
            return "Error: " + ex.Message;
        }
    }

    public bool UpdateUserPassword(string encryptedPassword, string Userid)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                UPDATE agencyuser
                SET password = @newPassword
                WHERE id = @userid
                AND status = 'Active';";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = Userid;
                    cmd.Parameters.Add("@newPassword", SqlDbType.VarChar).Value = encryptedPassword;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if update successful
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while changing password.", sqlEx);
            return false; // Return false on failure
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred.", ex);
            return false; // Return false on failure
        }
    }
    public string Insert_DownloadFileDetail(string actualfilename, string filename, string filepath, string subdoctype, string agencyname, string foldertype)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        try
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string InsertQuery = "INSERT INTO downloadfiledetail (actualfilename, filename, filepath, subdoctype, agency, foldertype) " +
                                     "VALUES (@actualfilename, @filename, @filepath, @subdoctype, @agencyname, @foldertype);";

                using (SqlCommand command = new SqlCommand(InsertQuery, connection))
                {
                    command.Parameters.AddWithValue("@actualfilename", actualfilename);
                    command.Parameters.AddWithValue("@filename", filename);
                    command.Parameters.AddWithValue("@filepath", filepath);
                    command.Parameters.AddWithValue("@subdoctype", subdoctype);
                    command.Parameters.AddWithValue("@agencyname", agencyname);
                    command.Parameters.AddWithValue("@foldertype", foldertype);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        log.Info("Download file details inserted successfully. Filename: " + filename + ", Agency: " + agencyname);
                        return "Success";
                    }
                    else
                    {
                        log.Warn("Failed to insert download file details. Filename: " + filename + ", Agency: " + agencyname);
                        return "Error";
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error while inserting download file details. Filename: " + filename + ", Error: " + sqlEx.Message, sqlEx);
            return "Database error: " + sqlEx.Message;
        }
        catch (Exception ex)
        {
            log.Error("Unexpected error while inserting download file details. Filename: " + filename + ", Error: " + ex.Message, ex);
            return "Error: " + ex.Message;
        }
    }
    //public string Insert_DownloadFileDetail(string filename, string filepath,string subdoctype, string agencyname, string foldertype)
    //{
    //    // Get the connection string from the configuration file (appsettings.json or web.config)
    //    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    //    using (SqlConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();

    //        // SQL Insert Query for MSSQL
    //        string InsertQuery = "INSERT INTO downloadfiledetail (filename, filepath, subdoctype,agency,foldertype) " +
    //                             "VALUES (@filename, @filepath, @downloadedby,@agencyname,@foldertype);";

    //        using (SqlCommand command = new SqlCommand(InsertQuery, connection))
    //        {
    //            // Add parameters to match the query placeholders
    //            command.Parameters.AddWithValue("@filename", filename);
    //            command.Parameters.AddWithValue("@filepath", filepath);
    //            command.Parameters.AddWithValue("@subdoctype", subdoctype);
    //            command.Parameters.AddWithValue("@agencyname", agencyname);
    //            command.Parameters.AddWithValue("@foldertype", foldertype);

    //            // Execute the insert query
    //            int result = command.ExecuteNonQuery();

    //            // Return a success or error message based on whether rows were affected
    //            return (result > 0) ? "Success" : "Error";
    //        }
    //    }
    //}
    public DataTable getdownfiledetails()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT  id, filename, filepath, download, subdoctype, agency, foldertype 
                         FROM downloadfiledetail 
                         WHERE  foldertype = @foldertype and download=@download ORDER BY createddate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    // Set parameters
                    //cmd.Parameters.AddWithValue("@agency", agencyname);
                    cmd.Parameters.AddWithValue("@download", 1);
                    cmd.Parameters.AddWithValue("@foldertype", "Process");

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(resultTable); // Assign resultTable
                    }
                }
            }

        }
        catch (SqlException sqlEx)
        {
            log.Error("❌ Database error in getdownfiledetails method: " + sqlEx.Message, sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("❌ Unexpected error in getdownfiledetails method: " + ex.Message, ex);
        }

        return resultTable;
    }

    public string updatedownloadetailstable(int id)
    {
        try
        {
            string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string updateQuery = "UPDATE downloadfiledetail SET download = 0 WHERE id = @fileId";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@fileId", id);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return "✅ Download status updated successfully.";
                    }
                    else
                    {
                        return "⚠️ No records were updated.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("❌ Error updating download status: " + ex.Message, ex);
            return "❌ Error updating download status: " + ex.Message;
        }
    }


    public DataTable Update_ProcessedFileDetail(string filename)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable processedFilesTable = new DataTable();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // ✅ Update query
            string updateQuery = "UPDATE ProcessedFiles SET IsProcessed = 1 WHERE Filename = @filename;";

            using (SqlCommand processedCommand = new SqlCommand(updateQuery, connection))
            {
                processedCommand.Parameters.AddWithValue("@filename", filename);
                processedCommand.ExecuteNonQuery();
            }

            // ✅ Select the updated record
            string selectQuery = "SELECT * FROM ProcessedFiles WHERE Filename = @filename;";

            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@filename", filename);

                using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
                {
                    adapter.Fill(processedFilesTable);
                }
            }
        }

        return processedFilesTable; // ✅ Returns updated data
    }



    public string getstudentmasterdata(string rollcode, string rollno)
    {
        try
        {
            string res = "{"
              + "\"select\":{\"?user\":["
               + "\"ResultmasterId\","
               + "\"StudentFullName\","
               + "\"Stu_UniqueId\","
                + "\"FatherName\","
                + "\"MotherName\","
                + "\"DOB\","
                + "\"Gender\","
                + "\"Faculty\","
                + "\"CollegeName\","
                + "\"RollNumber\","
                + "\"RollCode\","
                + "\"RegistrationNo\","
                + "\"EnrollNo\","
                + "\"CategoryName\","
                + "\"CastCategoryName\","
                + "\"TotalMarks\","
                + "\"ExamType\","
                + "\"Division\","
                + "\"TotalMarksInWords\""


              + "]},"
              + "\"where\":[[\"?user\",\"Compartfinal_result_master_2025/ResultmasterId\",\"?name\"]";
            if (rollcode != "")
            {
                res += ",[\"?user\",\"Compartfinal_result_master_2025/RollCode\",\"" + rollcode
                     + "\"]";
            }
            if (rollno != "")
                res += ",[\"?user\",\"Compartfinal_result_master_2025/RollNumber\",\"" + rollno
                        + "\"]]}";

            string resp = sendTransaction(res, serverqryurl);
            return resp;
        }
        catch (Exception ex)
        {
            log.Error("An error occurred.", ex);
            return "Error: " + ex.Message;
        }
    }

    public DataTable GetAccessibleFiles(string viewerAgency)
    {
        DataTable dt = new DataTable();

        string query = @"
      SELECT DISTINCT
    d.[id],
    d.[actualfilename],
    d.[filename],
    d.[filepath],
    d.[download],
    d.[subdoctype],
    d.[downloadedby],
    d.[agency],
    d.[foldertype],
    d.[createddate]
FROM [downloadfiledetail] d
LEFT JOIN [AgencyDocumentAccess] a
    ON d.agency = a.OwnerAgency
    AND d.subdoctype = a.DocumentType

WHERE (@ViewerAgency = d.agency OR a.ViewerAgency = @ViewerAgency)  order by 1 desc";

        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@ViewerAgency", viewerAgency);

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }

    public string getstudentdetailsdata(string rollcode, string rollno)
    {
        try
        {
            string res = "{"
              + "\"select\":{\"?user\":["
               + "\"ResultdetailsId\","
               + "\"SubjectPaperName\","
                + "\"ObtainedMarks\","
                + "\"FMark\","
                + "\"TotalTheoryMarks\","
                + "\"PRObtainedMarks\","
                + "\"SubjectTotal\","
                + "\"PMarks\","
                + "\"PassWithGrace\","
                + "\"IsDivisionGrace\","
                + "\"DivisionGraceMarks\","
                + "\"SubjectGroupName\","
                + "\"AbsentTh\","
                + "\"AbsentPr\","
                + "\"IsSwapped\","
                + "\"TheoryGraceMarks\","
                + "\"PracticalGraceMarks\","
                + "\"SubjectPaperCode\","
                + "\"PassedUnderRegulation\","
                + "\"IsCompartment\","
                + "\"IsImproved\","
                + "\"CCEMarks\","
                + "\"RollCode\","
                + "\"RollNumber\","
                + "\"FacultyName\""


              + "]},"
              + "\"where\":[[\"?user\",\"CompartFinal_result_details_2025/ResultdetailsId\",\"?name\"]";
            if (rollcode != "")
            {
                res += ",[\"?user\",\"CompartFinal_result_details_2025/RollCode\",\"" + rollcode
                     + "\"]";
            }
            if (rollno != "")
                res += ",[\"?user\",\"CompartFinal_result_details_2025/RollNumber\",\"" + rollno
                        + "\"]]}";

            string resp = sendTransaction(res, serverqryurl);
            return resp;
        }
        catch (Exception ex)
        {
            log.Error("An error occurred.", ex);
            return "Error: " + ex.Message;
        }
    }

    public string UpdateMarksSelective(
    string rollCode, string rollNumber,
     int? obtainedMarks, int? cceMarks,
     bool? absentTh, bool? absentPr,
     int? totalTheoryMarks, int? subjectTotal,
     int? theoryGraceMarks, int? practicalGraceMarks,
     int? totalMarks, string division, bool? isPassInTotal)
    {
        var updates = new List<string>();
        var cmd = new SqlCommand();

        // Dynamically add non-null fields to update list and parameters
        if (obtainedMarks.HasValue)
        {
            updates.Add("ObtainedMarks = @ObtainedMarks");
            cmd.Parameters.AddWithValue("@ObtainedMarks", obtainedMarks.Value);
        }

        if (cceMarks.HasValue)
        {
            updates.Add("CCEMarks = @CCEMarks");
            cmd.Parameters.AddWithValue("@CCEMarks", cceMarks.Value);
        }

        if (absentTh.HasValue)
        {
            updates.Add("AbsentTh = @AbsentTh");
            cmd.Parameters.AddWithValue("@AbsentTh", absentTh.Value);
        }

        if (absentPr.HasValue)
        {
            updates.Add("AbsentPr = @AbsentPr");
            cmd.Parameters.AddWithValue("@AbsentPr", absentPr.Value);
        }

        if (totalTheoryMarks.HasValue)
        {
            updates.Add("TotalTheoryMarks = @TotalTheoryMarks");
            cmd.Parameters.AddWithValue("@TotalTheoryMarks", totalTheoryMarks.Value);
        }

        if (subjectTotal.HasValue)
        {
            updates.Add("SubjectTotal = @SubjectTotal");
            cmd.Parameters.AddWithValue("@SubjectTotal", subjectTotal.Value);
        }

        if (theoryGraceMarks.HasValue)
        {
            updates.Add("TheoryGraceMarks = @TheoryGraceMarks");
            cmd.Parameters.AddWithValue("@TheoryGraceMarks", theoryGraceMarks.Value);
        }

        if (practicalGraceMarks.HasValue)
        {
            updates.Add("PracticalGraceMarks = @PracticalGraceMarks");
            cmd.Parameters.AddWithValue("@PracticalGraceMarks", practicalGraceMarks.Value);
        }

        if (totalMarks.HasValue)
        {
            updates.Add("TotalMarks = @TotalMarks");
            cmd.Parameters.AddWithValue("@TotalMarks", totalMarks.Value);
        }

        if (!string.IsNullOrWhiteSpace(division))
        {
            updates.Add("Division = @Division");
            cmd.Parameters.AddWithValue("@Division", division);
        }

        if (isPassInTotal.HasValue)
        {
            updates.Add("IsPassInTotal = @IsPassInTotal");
            cmd.Parameters.AddWithValue("@IsPassInTotal", isPassInTotal.Value);
        }

        // If no values provided to update
        if (updates.Count == 0)
            return "❌ No values provided to update.";

        // Build final SQL command
        string query = "UPDATE EXAM_FinalPublishedResult_backup SET " +
                    string.Join(", ", updates) +
                    " WHERE RollCode = @RollCode AND RollNumber = @RollNumber";

        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@RollCode", rollCode);
        cmd.Parameters.AddWithValue("@RollNumber", rollNumber);

        string connectionString = ConfigurationManager.ConnectionStrings["SecondDbConnection"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return string.Format("✅ Update successful. {0} row(s) affected.", rowsAffected);
                else
                    return "⚠ Update query ran but no row was found for the given ID.";
            }
        }
        catch (Exception ex)
        {
            return "❌ Error occurred: " + ex.Message;
        }
    }

  

    public bool IsPrivateKeyValidwithusername(string username, string privateKey)
    {
        bool isValid = false;


        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT COUNT(*) 
            FROM agencyuser 
            WHERE Username = @Username
              AND PrivateKey = @PrivateKey 
              AND Key_expiry >= GETDATE()";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PrivateKey", privateKey);

                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                isValid = count > 0;
            }
        }

        return isValid;
    }






    public DataTable GetProcessFileList(string owneragency)
    {
        DataTable dt = new DataTable();

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString))
            {
                string query = @"SELECT [AccessId],[OwnerAgency],[ViewerAgency],[DocumentType]
                          FROM [AgencyDocumentAccess]
                          WHERE OwnerAgency = @owneragency";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@owneragency", SqlDbType.VarChar).Value = owneragency;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }

        return dt;
    }

    public string AddDocumentCategory(string categoryName)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string checkQuery = "SELECT COUNT(*) FROM DocumentCategoryMaster WHERE DocCategoryName = @Name";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Name", categoryName);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        return "Category already exists.";
                    }
                }


                string insertQuery = @"INSERT INTO DocumentCategoryMaster (DocCategoryName)
                                 VALUES (@Name)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", categoryName);
                    int rows = cmd.ExecuteNonQuery();

                    return rows > 0 ? "Category added successfully." : "Failed to add category.";
                }
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    public DataTable GetDocumentCategoryData(string status)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT Id, DocCategoryName, IsActive FROM DocumentCategoryMaster WHERE 1=1";

            if (!string.IsNullOrEmpty(status))
                query += " AND IsActive = @Status";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrEmpty(status))
                    cmd.Parameters.AddWithValue("@Status", Convert.ToInt32(status));

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }

    public bool UpdateDocumentCategoryStatus(int id, bool isActive)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE DocumentCategoryMaster SET IsActive=@IsActive, UpdatedDate=GETDATE() WHERE Id=@Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool GetCurrentStatus(int id)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT IsActive FROM DocumentCategoryMaster WHERE Id=@Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                object result = cmd.ExecuteScalar();
                return result != null && Convert.ToBoolean(result);
            }
        }
    }




    public string AddDocumentType(string categoryName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string checkQuery = "SELECT COUNT(*) FROM DocumentTypeMaster WHERE DocTypeName = @Name";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Name", categoryName);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        return "Document type already exists.";
                    }
                }
                string query = @"INSERT INTO DocumentTypeMaster 
                           (DocTypeName, IsActive)
                           VALUES (@Name, 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", categoryName);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        return "Document type added successfully.";
                    else
                        return "Failed to add document type.";
                }
            }
        }
        catch (SqlException sqlEx)
        {
            return "Database error: " + sqlEx.Message;
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }


    public DataTable GetDocumentTypeData(string status)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT Id, DocTypeName, IsActive FROM Documenttypemaster WHERE 1=1";

            if (!string.IsNullOrEmpty(status))
                query += " AND IsActive=@Status";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrEmpty(status))
                    cmd.Parameters.AddWithValue("@Status", Convert.ToInt32(status));

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }


    public bool UpdateDocumentTypeStatus(int id, bool isActive)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE Documenttypemaster SET IsActive=@IsActive, UpdatedDate=GETDATE() WHERE Id=@Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }


    public bool GetDocumentTypeStatus(int id)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT IsActive FROM Documenttypemaster WHERE Id=@Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                object result = cmd.ExecuteScalar();
                return result != null && Convert.ToBoolean(result);
            }
        }
    }


    public DataRow GetDocumentCategoryById(int id)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT DocCategoryName, IsActive FROM DocumentCategoryMaster WHERE Id=@Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }


    public bool CheckDuplicateDocumentCategory(string name, int excludeId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM DocumentCategoryMaster WHERE DocCategoryName=@Name AND Id<>@Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Id", excludeId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }


    public bool UpdateDocumentCategory(int id, string name, int isActive)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = @"UPDATE DocumentCategoryMaster
                        SET DocCategoryName=@Name,
                            IsActive=@IsActive,
                            UpdatedDate=GETDATE()
                        WHERE Id=@Id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }


    public DataTable GetDocumentTypeById(int docId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, DocTypeName, IsActive FROM DocumentTypeMaster WHERE Id=@Id";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", docId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }


    public bool CheckDuplicateDocumentType(string docTypeName, int excludeId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int count = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM DocumentTypeMaster WHERE DocTypeName=@DocTypeName AND Id<>@Id";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DocTypeName", docTypeName);
                cmd.Parameters.AddWithValue("@Id", excludeId);
                con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        return count > 0;
    }


    public bool UpdateDocumentType(int id, string docTypeName, int isActive)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int result = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"UPDATE DocumentTypeMaster 
                        SET DocTypeName=@DocTypeName, 
                            IsActive=@IsActive, 
                            UpdatedDate=GETDATE() 
                        WHERE Id=@Id";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DocTypeName", docTypeName);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                result = cmd.ExecuteNonQuery();
            }
        }
        return result > 0;
    }

    public bool CheckDuplicateIP(string ipNumber)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int count = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM IPMaster WHERE IPNumber=@IPNumber";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        return count > 0;
    }




    public DataTable GetIPById(int ipId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT 
               
                IPNumber,
                AgencyName,
              
                CanProcessCSV,
                CanUpload
               
            FROM IPMaster
            WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", ipId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }

        return dt;
    }


    public bool CheckDuplicateIPForEdit(string ipNumber, int ipId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int count = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM IPMaster WHERE IPNumber=@IPNumber AND IP<>@IP";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                cmd.Parameters.AddWithValue("@IP", ipId);
                con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        return count > 0;
    }



    public bool UpdateIP(int ipId, string ipNumber, string agencyName, string updatedBy,
                      bool canProcessCSV, bool canUpload)
    {
        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();

                string query = @"
                UPDATE IPMaster SET
                    IPNumber = @IPNumber,
                    AgencyName = @AgencyName,
                    CanProcessCSV = @CanProcessCSV,
                    CanUpload = @CanUpload,
                 
                    UpdatedDate = GETDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", ipId);
                    cmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                    cmd.Parameters.AddWithValue("@AgencyName",
                        string.IsNullOrEmpty(agencyName) ? (object)DBNull.Value : agencyName);

                    cmd.Parameters.AddWithValue("@CanProcessCSV", canProcessCSV);
                    cmd.Parameters.AddWithValue("@CanUpload", canUpload);

                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch
        {
            return false;
        }
    }

    public DataTable Documenttypemaster()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
           SELECT 
               Id, 
               DocTypeName, 
               IsActive, 
               CreatedDate, 
               UpdatedDate
           FROM DocumentTypeMaster
           WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while fetching document types.", sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred while fetching document types.", ex);
        }

        return resultTable;
    }



    public DataTable DocumentCategoryMaster()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
           SELECT 
               Id, 
               DocCategoryName, 
               IsActive, 
               CreatedDate, 
               UpdatedDate
           FROM DocumentCategoryMaster
           WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while fetching document types.", sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred while fetching document types.", ex);
        }

        return resultTable;
    }

    public bool IsActionAllowed(string ipAddress, string action)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"SELECT CanUpload, CanProcessCSV
                         FROM IPMaster
                         WHERE IPNumber = @IP AND IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@IP", ipAddress);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return false; // IP not allowed

                    if (action == "UPLOAD")
                        return Convert.ToInt32(reader["CanUpload"]) == 1;

                    if (action == "DOWNLOAD")
                        return Convert.ToInt32(reader["CanProcessCSV"]) == 1;
                }
            }
        }
        return false;
    }


    public List<string> GetAllowedIPsFromDB()
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
    public bool UpdateIPV(int ipId, string ipNumber, string agencyName, string updatedBy,
                    bool canProcessCSV, bool canUpload)
    {
        string conn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();

                string query = @"
                UPDATE IPMaster SET
                    IPNumber = @IPNumber,
                    AgencyName = @AgencyName,
                    CanProcessCSV = @CanProcessCSV,
                    CanUpload = @CanUpload,
                    UpdatedDate = GETDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", ipId);
                    cmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                    cmd.Parameters.AddWithValue("@AgencyName",
                        string.IsNullOrEmpty(agencyName) ? (object)DBNull.Value : agencyName);

                    cmd.Parameters.AddWithValue("@CanProcessCSV", canProcessCSV);
                    cmd.Parameters.AddWithValue("@CanUpload", canUpload);
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch
        {
            return false;
        }
    }

    public DataTable AgencyWiseData(string agency)
    {
        DataTable dt = new DataTable();
        string cs = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection conn = new SqlConnection(cs))
        {
            string query = @"
            SELECT 
                id,
                username,
                email,
                mobileno,
                agencyname,
                PrivateKey,
                Key_Expiry
            FROM agencyuser
            WHERE 1=1";

            if (!string.IsNullOrEmpty(agency))
                query += " AND agencyname = @Agency";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrEmpty(agency))
                    cmd.Parameters.AddWithValue("@Agency", agency);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }

        return dt;
    }

    public string UpdateExamSession(int id, string sessionName, int isActive)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"UPDATE Exam_Session_Master
                         SET SessionName = @SessionName,
                             IsActive = @IsActive
                         WHERE Id = @Id
                         SELECT 'Session updated successfully.'";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@SessionName", sessionName);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }
    }

    public DataTable ExamSessionmaster()
    {
        DataTable resultTable = new DataTable();
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
           SELECT 
               Id, 
               SessionName, 
               IsActive
              
           FROM Exam_Session_Master
           WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            log.Error("Database error occurred while fetching document types.", sqlEx);
        }
        catch (Exception ex)
        {
            log.Error("An unexpected error occurred while fetching document types.", ex);
        }

        return resultTable;
    }


    public string InsertExamSession(string sessionName)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"IF EXISTS (SELECT 1 FROM Exam_Session_Master WHERE SessionName = @SessionName)
                             SELECT 'Session already exists.'
                             ELSE
                             BEGIN
                                INSERT INTO Exam_Session_Master (SessionName)
                                VALUES (@SessionName)
                                SELECT 'Session added successfully.'
                             END";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@SessionName", sessionName);

                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }
    }



    public DataTable GetExamSessionList(string status)
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"SELECT Id, SessionName, IsActive
                             FROM Exam_Session_Master
                             WHERE (@Status = '' OR IsActive = @IsActive)
                             ORDER BY Id DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Status", status ?? "");
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit)
                               .Value = string.IsNullOrEmpty(status)
                                        ? (object)DBNull.Value
                                        : Convert.ToInt32(status);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }

   
    public bool ToggleSessionStatus(int id)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"UPDATE Exam_Session_Master
                             SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
                             WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

 
    public DataRow GetExamSessionById(int id)
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"SELECT Id, SessionName, IsActive
                             FROM Exam_Session_Master
                             WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }

        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public string UpdateExamSession(int id, string sessionName)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"UPDATE Exam_Session_Master
                             SET SessionName = @SessionName
                             WHERE Id = @Id
                             SELECT 'Session updated successfully.'";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@SessionName", sessionName);

                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }
    }


    public string InsertIP(string ipNumber, string agencyName, string updatedBy,
                           bool canProcessCSV, bool canUpload)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();


                string checkQuery = "SELECT COUNT(*) FROM IPMaster WHERE IPNumber = @IPNumber";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        return "This IP already exists.";
                    }
                }

                string query = @"
                INSERT INTO IPMaster
                (IPNumber, AgencyName, IsActive, CanProcessCSV, CanUpload, UpdatedDate, UpdatedBy)
                VALUES 
                (@IPNumber, @AgencyName, 1, @CanProcessCSV, @CanUpload, GETDATE(), @UpdatedBy)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IPNumber", ipNumber);
                    cmd.Parameters.AddWithValue("@AgencyName",
                        string.IsNullOrEmpty(agencyName) ? (object)DBNull.Value : agencyName);
                    cmd.Parameters.AddWithValue("@CanProcessCSV", canProcessCSV);
                    cmd.Parameters.AddWithValue("@CanUpload", canUpload);
                 
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0 ? "IP added successfully." : "Failed to add IP.";
                }
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }



    public DataTable GetIPList(string status)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM IPMaster WHERE 1=1";

            if (!string.IsNullOrEmpty(status))
                query += " AND IsActive=@IsActive";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(status))
                    cmd.Parameters.AddWithValue("@IsActive", status);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }

    public bool IsPrivateKeyValid(string privateKey)
    {
        bool isValid = false;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT COUNT(*) 
            FROM agencyuser 
            WHERE PrivateKey = @PrivateKey 
              AND Key_expiry >= GETDATE()";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@PrivateKey", privateKey);

                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                if (count > 0)
                    isValid = true;
            }
        }

        return isValid;
    }



    public bool ToggleIPStatus(int ipId, string updatedBy)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int result = 0;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"UPDATE IPMaster 
                             SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                                 UpdatedDate = GETDATE(),
                                 UpdatedBy = @UpdatedBy
                             WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", ipId);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                result = cmd.ExecuteNonQuery();
            }
        }
        return result > 0;
    }




}


#endregion


