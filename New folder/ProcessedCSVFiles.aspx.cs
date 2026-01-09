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
                // getfiles();
                LoadFilteredData();
                //string loggedInAgency = Session["agencyname"].ToString() ?? "";
                //if (loggedInAgency == "Keltron" || loggedInAgency == "Datasoft")
                //{
                //    CrossFiles_All.Visible = true;


                //}
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
    private void LoadFilteredData()
    {

        string loggedInAgency = Session["agencyname"].ToString() ?? "";
        DataTable res = fl.GetAccessibleFiles(loggedInAgency);
        if (res != null && res.Rows.Count > 0)
        {


            // Dictionary to store data for each agency tab
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
                //string subdoctype = row["subdoctype"].ToString();
                //string fileAgency = row["agency"].ToString();

                string agencyName = row["agency"].ToString();

                if (tabData.ContainsKey(agencyName))
                {
                    // ✅ ImportRow keeps original values
                    tabData[agencyName].ImportRow(row);
                }

                //                if (processedFiles.Contains(row))
                //                    continue; // Skip already processed files

                //                // ✅ Cross-file logic for specific subdoctypes
                //                if ((subdoctype == "Process OMR Sheet" || subdoctype == "Process Attendance B" ||
                //                     subdoctype == "Process Absentee Sheet" || subdoctype == "Process Foil Sheet" || subdoctype == "Process Award Sheet" ||
                //                     subdoctype == "Wanting OMR Sheet" || subdoctype == "Wanting Foil Sheet" || subdoctype == "Wanting Flying Sheet" ||
                //                     subdoctype == "Wanting Absentee Sheet" || subdoctype == "Wanting Attendance B" || subdoctype == "Wanting Award Sheet" || subdoctype == "Final OMR Sheet" || subdoctype == "OMR Sheet") &&
                //                    ((loggedInAgency == "Mapple" && fileAgency == "Datacon") ||
                //                     (loggedInAgency == "Datacon" && fileAgency == "Mapple")))
                //                {
                //                    if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }
                //                    else if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Process Award Sheet" &&
                //                    ((loggedInAgency == "Mapple" && fileAgency == "Kids") ||
                //                     (loggedInAgency == "Kids" && fileAgency == "Mapple")))
                //                {
                //                    if (fileAgency == "Kids")
                //                    {
                //                        tabData["Kids"].ImportRow(row);
                //                    }
                //                    else if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Process Foil Sheet" &&
                //                    ((loggedInAgency == "MCRK" && fileAgency == "Datacon") ||
                //                     (loggedInAgency == "Datacon" && fileAgency == "MCRK")))
                //                {
                //                    if (fileAgency == "MCRK")
                //                    {
                //                        tabData["MCRK"].ImportRow(row);
                //                    }
                //                    else if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Process Online Data" || subdoctype == "Final Online Data") &&
                //                    (loggedInAgency == "Kids" && fileAgency == "Charu Mindworks"))
                //                {
                //                    tabData["Charu_Mindworks"].ImportRow(row);
                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if ((subdoctype == "Process Online Data") &&
                //                    (loggedInAgency == "Datacon" && fileAgency == "Charu Mindworks"))
                //                {
                //                    tabData["Charu_Mindworks"].ImportRow(row);
                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if ((subdoctype == "Final Online Data") &&
                //                  (loggedInAgency == "SSB Digital" || loggedInAgency == "Antier" && fileAgency == "Charu Mindworks"))
                //                {
                //                    tabData["Charu_Mindworks"].ImportRow(row);
                //                    processedFiles.Add(row);
                //                    continue;
                //                }


                //                if ((subdoctype == "3 year Carry Data" || subdoctype == "Missmatched") &&
                //                   ((loggedInAgency == "Keltron" && fileAgency == "Datasoft")))
                //                {
                //                    if (fileAgency == "Datasoft")
                //                    {
                //                        tabData["Datasoft"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Topper" || subdoctype == "Missmatched") &&
                //                  ((loggedInAgency == "Datasoft" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if (subdoctype == "Theory Attendance Sheet" &&
                //    (loggedInAgency == "Datacon" || loggedInAgency == "Mapple") &&
                //    fileAgency == "Charu Mindworks"
                //)
                //                {
                //                    if (fileAgency == "Charu Mindworks")
                //                    {
                //                        tabData["Charu_Mindworks"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }


                //                if ((subdoctype == "Award Sheet") &&
                //                  ((loggedInAgency == "Kids" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Award Sheet") &&
                //                ((loggedInAgency == "Kids" && fileAgency == "SSB Digital")))
                //                {
                //                    if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Final Data") &&
                //                 ((loggedInAgency == "Antier" && fileAgency == "SSB Digital")))
                //                {
                //                    if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Award Sheet") &&
                //                 ((loggedInAgency == "SSB Digital" || loggedInAgency == "Antier" && fileAgency == "Kids")))
                //                {
                //                    if (fileAgency == "Kids")
                //                    {
                //                        tabData["Kids"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "OMR Sheet" || subdoctype == "Practical Award Sheet") &&
                //                  ((loggedInAgency == "Mapple" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Practical Attendance Sheet" || subdoctype == "Theory Attendance Sheet" || subdoctype == "Flying Sheet") &&
                //                  ((loggedInAgency == "Datacon" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if (subdoctype == "Final Flying Sheet" || subdoctype == "Final Attendance B" || subdoctype == "Final Absentee Sheet" &&
                //     (
                //         (loggedInAgency == "SSB Digital" || loggedInAgency == "Antier") && fileAgency == "Datacon"))
                //                {
                //                    if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }


                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if ((loggedInAgency == "Keltron" || loggedInAgency == "Datasoft" || loggedInAgency == "Antier" || loggedInAgency == "SSB Digital") &&
                //    (subdoctype == "Practical Data Structure" || subdoctype == "Theory Data Structure") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Bottom BARCODE Master") &&
                //  ((loggedInAgency == "Datacon" && fileAgency == "Charu Mindworks" || loggedInAgency == "Keltron" && fileAgency == "Charu Mindworks")))
                //                {
                //                    if (fileAgency == "Charu Mindworks")
                //                    {
                //                        tabData["Charu_Mindworks"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if (subdoctype == "Wanting Award Sheet" &&
                //    ((loggedInAgency == "Kids" && fileAgency == "Antier") || (loggedInAgency == "Datacon" && fileAgency == "Antier")))
                //                {
                //                    if (fileAgency == "Antier")
                //                    {
                //                        tabData["Antier"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Wanting Award Sheet" &&
                //   ((loggedInAgency == "Kids" && fileAgency == "SSB Digital")))
                //                {
                //                    if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }




                //                if ((subdoctype == "Practical Marks" || subdoctype == "Flying Sheet") &&
                //     loggedInAgency == "Datacon" &&
                //     fileAgency == "SSB Digital")
                //                {
                //                    tabData["SSBDigital"].ImportRow(row);
                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if (subdoctype == "Wanting OMR Sheet" &&
                //    ((loggedInAgency == "Mapple" && fileAgency == "Antier") ||
                //     (loggedInAgency == "Antier" && fileAgency == "Mapple")))
                //                {
                //                    if (fileAgency == "Antier")
                //                    {
                //                        tabData["Antier"].ImportRow(row); // Assuming Antier should always see it
                //                    }
                //                    else if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }


                //                if (subdoctype == "Final Attendance A" &&
                //  ((loggedInAgency == "Datacon" && fileAgency == "Datasoft")))
                //                {
                //                    if (fileAgency == "Datasoft")
                //                    {
                //                        tabData["Datasoft"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Final Attendance B" &&
                //((loggedInAgency == "Kids" && fileAgency == "Datacon")))
                //                {
                //                    if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }


                //                if (subdoctype == "Final Result Data" &&
                // ((loggedInAgency == "Antier" && fileAgency == "SSB Digital")))
                //                {
                //                    if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if (subdoctype == "Final Result Data" &&
                //((loggedInAgency == "SSB Digital" || loggedInAgency == "Antier" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Sample Result Data" &&
                //  ((loggedInAgency == "SSB Digital" && fileAgency == "Keltron")))
                //                {
                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Sample Result Data" &&
                // ((loggedInAgency == "SSB Digital" && fileAgency == "Antier")))
                //                {
                //                    if (fileAgency == "Antier")
                //                    {
                //                        tabData["Antier"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if ((loggedInAgency == "Kids" || loggedInAgency == "Mapple") &&
                //    (subdoctype == "Final Award Sheet") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Datacon" || loggedInAgency == "Mapple") &&
                //    (subdoctype == "Final OMR Sheet") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Datacon") &&
                //   (subdoctype == "Final Attendance A" || subdoctype == "Final Attendance B" || subdoctype == "Final Absentee Sheet" || subdoctype == "Flying Sheet") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if ((loggedInAgency == "Datacon" || loggedInAgency == "MCRK") &&
                // (subdoctype == "Final Foil Sheet") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Datacon") &&
                //(subdoctype == "Final Foil Sheet") && fileAgency == "MCRK")
                //                {

                //                    if (fileAgency == "MCRK")
                //                    {
                //                        tabData["MCRK"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Kids" || loggedInAgency == "MCRK") &&
                // (subdoctype == "Final Foil Sheet") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Kids") &&
                //(subdoctype == "Scrutiny Print Master") && fileAgency == "Hitech")
                //                {

                //                    if (fileAgency == "Hitech")
                //                    {
                //                        tabData["Hitech"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Keltron" || loggedInAgency == "Datasoft") &&
                //(subdoctype == "Scrutiny Theory Award") && fileAgency == "Kids")
                //                {

                //                    if (fileAgency == "Kids")
                //                    {
                //                        tabData["Kids"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if ((loggedInAgency == "Kids") &&
                //(subdoctype == "Scrutiny Result Data") && fileAgency == "Keltron")
                //                {

                //                    if (fileAgency == "Keltron")
                //                    {
                //                        tabData["Keltron"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Mapple") &&
                //(subdoctype == "Final Attendance B") && fileAgency == "Datacon")
                //                {

                //                    if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Datacon") &&
                //(subdoctype == "Wanting Foil Sheet" || subdoctype == "Final Foil Sheet") && fileAgency == "MCRK")
                //                {

                //                    if (fileAgency == "MCRK")
                //                    {
                //                        tabData["MCRK"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((loggedInAgency == "Mapple") &&
                //(subdoctype == "Final Absentee Sheet" || subdoctype == "Final Foil Sheet") && fileAgency == "Datacon")
                //                {

                //                    if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }
                //                if ((loggedInAgency == "SSB Digital" || loggedInAgency == "Antier") &&
                //(subdoctype == "Practical Award Sheet" || subdoctype == "Final Award Sheet" || subdoctype == "Final OMR Sheet") && fileAgency == "Mapple")
                //                {

                //                    if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if (subdoctype == "Wanting OMR Sheet" || subdoctype == "OMR Sheet" &&
                //    ((loggedInAgency == "Mapple" && fileAgency == "SSB Digital") ||
                //     (loggedInAgency == "SSB Digital" && fileAgency == "Mapple")))
                //                {
                //                    if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row); // Assuming Antier should always see it
                //                    }
                //                    else if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row); // Assuming Antier should always see it

                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }



                //                if (subdoctype == "Wanting Flying Sheet" || subdoctype == "Flying Sheet" &&
                //     ((loggedInAgency == "Datacon" && fileAgency == "SSB Digital") || (loggedInAgency == "SSB Digital" && fileAgency == "Datacon")))
                //                {
                //                    if (fileAgency == "SSB Digital")
                //                    {
                //                        tabData["SSBDigital"].ImportRow(row);
                //                    }
                //                    else if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }

                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Wanting Flying Sheet" || subdoctype == "Flying Sheet") &&
                //    loggedInAgency == "Antier" &&
                //    fileAgency == "Datacon")
                //                {
                //                    tabData["Datacon"].ImportRow(row);
                //                    processedFiles.Add(row);
                //                    continue;
                //                }

                //                if ((subdoctype == "Process OMR Sheet" || subdoctype == "Final Attendance B" || subdoctype == "Practical Award Sheet" || subdoctype == "OMR Sheet" || subdoctype == "Final Absentee Sheet" || subdoctype == "Final Foil Sheet") &&
                //   ((loggedInAgency == "SSB Digital" || loggedInAgency == "Antier" && (fileAgency == "Mapple" || fileAgency == "Datacon"))))
                //                {
                //                    if (fileAgency == "Mapple")
                //                    {
                //                        tabData["Mapple"].ImportRow(row);
                //                    }
                //                    else if (fileAgency == "Datacon")
                //                    {
                //                        tabData["Datacon"].ImportRow(row);
                //                    }
                //                    processedFiles.Add(row);
                //                    continue;
                //                }


                //if (loggedInAgency == fileAgency)
                //{
                //    if (fileAgency == "Keltron")
                //    {
                //        tabData["Keltron"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Mapple")
                //    {
                //        tabData["Mapple"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Datacon")
                //    {
                //        tabData["Datacon"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Kids")
                //    {
                //        tabData["Kids"].ImportRow(row);
                //    }
                //    else if (fileAgency == "MCRK")
                //    {
                //        tabData["MCRK"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Charu Mindworks")
                //    {
                //        tabData["Charu_Mindworks"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Shree Jagannath Udyog")
                //    {
                //        tabData["Shree_Jagannath_Udyog"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Hitech")
                //    {
                //        tabData["Hitech"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Antier")
                //    {
                //        tabData["Antier"].ImportRow(row);
                //    }
                //    else if (fileAgency == "Datasoft")
                //    {
                //        tabData["Datasoft"].ImportRow(row);
                //    }
                //    else if (fileAgency == "SSB Digital")
                //    {
                //        tabData["SSBDigital"].ImportRow(row);
                //    }

                //    processedFiles.Add(row);
                //    continue;
                //}


            }

            // ✅ Bind data to repeaters dynamically
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

    // 🔹 Helper function to bind repeater
    private void BindRepeater(Repeater rpt, DataTable dt)
    {
        rpt.DataSource = dt.Rows.Count > 0 ? dt : null;
        rpt.DataBind();
    }


    //private void LoadFilteredData()
    //{
    //    DataTable res = fl.getdownfiledetails();
    //    if (res != null && res.Rows.Count > 0)
    //    {
    //        string loggedInAgency = Session["agencyname"].ToString() ?? "";

    //        // Dictionary to store data for each agency tab
    //        Dictionary<string, DataTable> tabData = new Dictionary<string, DataTable>();

    //        // Create an empty DataTable clone for different agencies
    //        tabData["Datacon"] = res.Clone();
    //        tabData["Kids"] = res.Clone();
    //        tabData["MCRK"] = res.Clone();
    //        tabData["Mapple"] = res.Clone();
    //        tabData["Charu_Mindworks"] = res.Clone();
    //        tabData["Shree Jagannath Udyog"] = res.Clone();
    //        tabData["Hitech"] = res.Clone();


    //        foreach (DataRow row in res.Rows)
    //        {
    //            string subdoctype = row["subdoctype"].ToString();
    //            string fileAgency = row["agency"].ToString();

    //            // ✅ Only process files should be considered
    //            if (!subdoctype.StartsWith("Process"))
    //            {
    //                continue;
    //            }

    //            // ✅ If Keltron or Datasoft logs in, show only process files in their respective tabs
    //            if (loggedInAgency == "Keltron" || loggedInAgency == "Datasoft")
    //            {
    //                if (fileAgency == "Datacon") tabData["Datacon"].ImportRow(row);
    //                else if (fileAgency == "Kids") tabData["Kids"].ImportRow(row);
    //                else if (fileAgency == "MCRK") tabData["MCRK"].ImportRow(row);
    //                else if (fileAgency == "Mapple") tabData["Mapple"].ImportRow(row);
    //                else if (fileAgency == "Charu Mindworks") tabData["Charu_Mindworks"].ImportRow(row);
    //                else if (fileAgency == "Shree Jagannath Udyog") tabData["Shree Jagannath Udyog"].ImportRow(row);
    //                else if (fileAgency == "Hitech") tabData["Hitech"].ImportRow(row);

    //            }
    //            else
    //            {
    //                // ✅ Cross-file logic for other agencies (only process files)
    //                if ((subdoctype == "Process OMR Sheet" &&
    //                     ((loggedInAgency == "Mapple" && fileAgency == "Datacon") ||
    //                      (loggedInAgency == "Datacon" && fileAgency == "Mapple"))))
    //                {
    //                    tabData["Datacon"].ImportRow(row);
    //                }

    //                if ((subdoctype == "Process Award Sheet" &&
    //                     ((loggedInAgency == "Mapple" && fileAgency == "Kids") ||
    //                      (loggedInAgency == "Kids" && fileAgency == "Mapple"))))
    //                {
    //                    tabData["Kids"].ImportRow(row);
    //                }

    //                if ((subdoctype == "Process Foil Sheet" &&
    //                     ((loggedInAgency == "MCRK" && fileAgency == "Datacon") ||
    //                      (loggedInAgency == "Datacon" && fileAgency == "MCRK"))))
    //                {
    //                    tabData["MCRK"].ImportRow(row);
    //                }
    //            }
    //        }

    //        // ✅ Bind data to repeater inside each tab
    //        rptDatacon.DataSource = tabData["Datacon"].Rows.Count > 0 ? tabData["Datacon"] : null;
    //        rptDatacon.DataBind();

    //        rptKids.DataSource = tabData["Kids"].Rows.Count > 0 ? tabData["Kids"] : null;
    //        rptKids.DataBind();

    //        rptMCRK.DataSource = tabData["MCRK"].Rows.Count > 0 ? tabData["MCRK"] : null;
    //        rptMCRK.DataBind();

    //        rptMapple.DataSource = tabData["Mapple"].Rows.Count > 0 ? tabData["Mapple"] : null;
    //        rptMapple.DataBind();

    //        RepeaterCharu_Mindworks.DataSource = tabData["Charu_Mindworks"].Rows.Count > 0 ? tabData["Charu_Mindworks"] : null;
    //        RepeaterCharu_Mindworks.DataBind();

    //        RepeaterShree_Jagannath_Udyog.DataSource = tabData["Shree Jagannath Udyog"].Rows.Count > 0 ? tabData["Shree Jagannath Udyog"] : null;
    //        RepeaterShree_Jagannath_Udyog.DataBind();

    //        RepeaterHitech.DataSource = tabData["Hitech"].Rows.Count > 0 ? tabData["Hitech"] : null;
    //        RepeaterHitech.DataBind();

    //        //RepeaterKeltron.DataSource = tabData["Keltron"].Rows.Count > 0 ? tabData["Keltron"] : null;
    //        //RepeaterKeltron.DataBind();
    //    }
    //}


    //public void getfiles()
    //{
    //    DataTable res = fl.getdownfiledetails();
    //    if (res != null && res.Rows.Count > 0)
    //    {
    //        string loggedInAgency = Session["agencyname"].ToString();

    //        // Dictionary to store data for each agency tab
    //        Dictionary<string, DataTable> tabData = new Dictionary<string, DataTable>
    //    {
    //        { "Datacon", res.Clone() },
    //        { "Kids", res.Clone() },
    //        { "MCRK", res.Clone() },
    //        { "Mapple", res.Clone() },
    //        { "Charu_Mindworks", res.Clone() },
    //        { "Keltron", res.Clone() },
    //        { "Datasoft", res.Clone() }
    //    };

    //        HashSet<DataRow> processedFiles = new HashSet<DataRow>();

    //        foreach (DataRow row in res.Rows)
    //        {
    //            string subdoctype = row["subdoctype"].ToString();
    //            string fileAgency = row["agency"].ToString();

    //            if (processedFiles.Contains(row))
    //                continue; // Skip already processed files

    //            // ✅ Keltron & Datasoft see all process files (first priority)
    //            if (loggedInAgency == "Keltron" || loggedInAgency == "Datasoft")

    //            {
    //                if ((subdoctype.StartsWith("Process") || subdoctype.StartsWith("Final")))
    //                {
    //                    processedFiles.Add(row);
    //                    continue;
    //                }

    //            }

    //            // ✅ Cross-file logic (Assign file to first matching tab and skip others)
    //            if ((subdoctype == "Process OMR Sheet" || subdoctype == "Process Attendance B" ||
    // subdoctype == "Process Absentee Sheet" || subdoctype == "Process Foil Sheet" ||
    // subdoctype == "Wanting OMR Sheet" || subdoctype == "Wanting Foil Sheet" || subdoctype == "Wanting Absentee Sheet" || subdoctype == "Wanting Attendance B") &&
    //((loggedInAgency == "Mapple" && fileAgency == "Datacon") ||
    // (loggedInAgency == "Datacon" && fileAgency == "Mapple")))
    //            {
    //                if (fileAgency == "Datacon")
    //                {
    //                    tabData["Datacon"].ImportRow(row);
    //                }
    //                else if (fileAgency == "Mapple")
    //                {
    //                    tabData["Mapple"].ImportRow(row);
    //                }
    //                processedFiles.Add(row);
    //                continue;
    //            }


    //            if (subdoctype == "Process Award Sheet" &&
    //                ((loggedInAgency == "Mapple" && fileAgency == "Kids") ||
    //                 (loggedInAgency == "Kids" && fileAgency == "Mapple")))
    //            {
    //                if (fileAgency == "Kids")
    //                {
    //                    tabData["Kids"].ImportRow(row);
    //                }
    //                else if (fileAgency == "Mapple")
    //                {
    //                    tabData["Mapple"].ImportRow(row);
    //                }
    //                processedFiles.Add(row);
    //                continue;
    //            }

    //            if (subdoctype == "Process Foil Sheet" &&
    //                ((loggedInAgency == "MCRK" && fileAgency == "Datacon") ||
    //                 (loggedInAgency == "Datacon" && fileAgency == "MCRK")))
    //            {
    //                if (fileAgency == "MCRK")
    //                {
    //                    tabData["MCRK"].ImportRow(row);
    //                }
    //                else if (fileAgency == "Datacon")
    //                {
    //                    tabData["Datacon"].ImportRow(row);
    //                }
    //                processedFiles.Add(row);
    //                continue;
    //            }

    //            if ((subdoctype == "Process Online Data") &&
    //                (loggedInAgency == "Kids" && fileAgency == "Charu Mindworks"))
    //            {
    //                tabData["Charu_Mindworks"].ImportRow(row);
    //                processedFiles.Add(row);
    //                continue;
    //            }
    //        }

    //        // ✅ Bind data to repeater inside each tab
    //        rptDatacon.DataSource = tabData["Datacon"].Rows.Count > 0 ? tabData["Datacon"] : null;
    //        rptDatacon.DataBind();

    //        rptKids.DataSource = tabData["Kids"].Rows.Count > 0 ? tabData["Kids"] : null;
    //        rptKids.DataBind();

    //        rptMCRK.DataSource = tabData["MCRK"].Rows.Count > 0 ? tabData["MCRK"] : null;
    //        rptMCRK.DataBind();

    //        rptMapple.DataSource = tabData["Mapple"].Rows.Count > 0 ? tabData["Mapple"] : null;
    //        rptMapple.DataBind();

    //        RepeaterCharu_Mindworks.DataSource = tabData["Charu_Mindworks"].Rows.Count > 0 ? tabData["Charu_Mindworks"] : null;
    //        RepeaterCharu_Mindworks.DataBind();

    //    }
    //}

    protected void btnDownload_Click(object sender, EventArgs e)
    {

        try
        {

            List<string> allowedIps = new List<string> { "115.243.18.60", "117.203.160.250", "125.16.33.1", "152.58.187.74","152.58.154.108","152.58.129.98","152.58.152.113" };

           string clientIp = GetClientIp();

    //        // ✅ Check if client IP is in the allowed list
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
                string reslog = fl.Insertactivitylog(userId, clientIp, deviceUsed, "download", fileName, agencyName);
                log.Info("Activity Log Inserted: " + reslog);

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


    protected void rptMapple_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            // Get the logged-in agency (Assuming it's stored in Session)
            string loggedInAgency = Session["agencyname"].ToString();

            // Get the file agency from the current row
            string fileAgency = DataBinder.Eval(e.Item.DataItem, "agency").ToString();

            // Find the download button in the row
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
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
            Button btnDownload = (Button)e.Item.FindControl("btnDownload");

            // Hide button if the logged-in agency is the same as the file agency
            if (btnDownload != null)
            {
                btnDownload.Visible = loggedInAgency != fileAgency;
            }
        }
    }
}