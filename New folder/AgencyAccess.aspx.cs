using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AgencyAccess : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null) BindAgencies();
            else Response.Redirect("../login.aspx");
        }
    }

    private void BindAgencies()
    {
        chkViewerAgencies.Items.Clear();
        chkViewerAgencies.Items.Add(new ListItem("<b>Scanning Agencies</b>", "") { Enabled = false });
        chkViewerAgencies.Items.Add(new ListItem("Datacon", "Datacon"));
        chkViewerAgencies.Items.Add(new ListItem("Kids", "Kids"));
        chkViewerAgencies.Items.Add(new ListItem("Mapple", "Mapple"));
        chkViewerAgencies.Items.Add(new ListItem("MCRK", "MCRK"));

        chkViewerAgencies.Items.Add(new ListItem("<b>Result Processing</b>", "") { Enabled = false });
        chkViewerAgencies.Items.Add(new ListItem("SSB Digital (Intermediate Result processing)", "SSB Digital"));
        chkViewerAgencies.Items.Add(new ListItem("Antier (Matrix Result processing)", "Antier"));

        chkViewerAgencies.Items.Add(new ListItem("<b>Marks Entry</b>", "") { Enabled = false });
        chkViewerAgencies.Items.Add(new ListItem("Charu Mindworks", "Charu Mindworks"));

        chkViewerAgencies.Items.Add(new ListItem("<b>Printing</b>", "") { Enabled = false });
        chkViewerAgencies.Items.Add(new ListItem("Shree Jagannath Udyog", "Shree Jagannath Udyog"));
        chkViewerAgencies.Items.Add(new ListItem("Hitech", "Hitech"));
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string error;
        if (!ValidateServerSide(out error))
        {
            ScriptManager.RegisterStartupScript( this,this.GetType(), "vFail",string.Format("alert('{0}');", error.Replace("'", "\\'")),true
        );

        }

        string ownerAgency = ddlOwnerAgency.SelectedValue;
        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        int insertedCount = 0;

        string category = chkFinal.Checked ? "Final" : chkWanting.Checked ? "Wanting" : chkProcess.Checked ? "Process" : "";

        List<string> selectedDocs = new List<string>();
        if (chkAttendanceA.Checked) selectedDocs.Add("Attendance A");
        if (chkAttendanceB.Checked) selectedDocs.Add("Attendance B");
        if (chkOnlineData.Checked) selectedDocs.Add("Online Data");
        if (chkAwardSheet.Checked) selectedDocs.Add("Award Sheet");
        if (chkLitho.Checked) selectedDocs.Add("Litho Sheet");
        if (chkFoilSheet.Checked) selectedDocs.Add("Foil Sheet");
        if (chkOMRSheet.Checked) selectedDocs.Add("OMR Sheet");
        if (chkFlying.Checked) selectedDocs.Add("Flying Sheet");
        if (chkAbsentee.Checked) selectedDocs.Add("Absentee Sheet");
        if (chkResultData.Checked) selectedDocs.Add("Result Data");
        if (chkFlyingSheet.Checked) selectedDocs.Add("Final Data");
        if (chkAdmitCardMaster.Checked) selectedDocs.Add("Admit Card Master");
        if (chkBottomBarcode.Checked) selectedDocs.Add("Bottom Barcode Master");
        if (chkCarryData.Checked) selectedDocs.Add("3 Year Carry Data");
        if (chkPracticalDS.Checked) selectedDocs.Add("Practical Data Structure");
        if (chkTheoryDS.Checked) selectedDocs.Add("Theory Data Structure");
        if (chkTheoryAS.Checked) selectedDocs.Add("Theory Attendance Sheet");
        if (chkTopper.Checked) selectedDocs.Add("Topper");
        if (chkMissmatched.Checked) selectedDocs.Add("Missmatched");
        if (chkPracticalMarks.Checked) selectedDocs.Add("Practical Marks");
        if (chkSampleResultData.Checked) selectedDocs.Add("Sample Result Data");
        if (chkScrutinyResultData.Checked) selectedDocs.Add("Scrutiny Result Data");
        if (chkScrutinyPrintMaster.Checked) selectedDocs.Add("Scrutiny Print Master");
        if (chkScrutinyTheoryAward.Checked) selectedDocs.Add("Scrutiny Theory Award");
        if (chkBBOSE_THEORY_OMR.Checked) selectedDocs.Add("BBOSE_THEORY_OMR");
        if (chkBBOSE_THEORY_FLY.Checked) selectedDocs.Add("BBOSE_THEORY_FLY");
        if (chkBBOSE_THEORY_ATTENDANCE.Checked) selectedDocs.Add("BBOSE_THEORY_ATTENDANCE");
        if (chkBBOSE_PRACTICAL_ATTENDANCE.Checked) selectedDocs.Add("BBOSE_PRACTICAL_ATTENDANCE");
        if (chkBBOSE_PRACTICAL_FOIL.Checked) selectedDocs.Add("BBOSE_PRACTICAL_FOIL");
        if (chkBBOSE_THEORY_EXPULSION.Checked) selectedDocs.Add("BBOSE_THEORY_EXPULSION");

        if (chkBBOSE_THEORY_AWARD.Checked) selectedDocs.Add("BBOSE_THEORY_AWARD");
        if (chkBBOSE_THEORY_FOIL.Checked) selectedDocs.Add("BBOSE_THEORY_FOIL");
        if (chkBBOSE_THEORY_ONLINE_MARKS.Checked) selectedDocs.Add("BBOSE_THEORY_ONLINE_MARKS");
        if (chkBBOSE_PRACTICAL_AWARD.Checked) selectedDocs.Add("BBOSE_PRACTICAL_AWARD");
        if (chkBBOSE_ADMITCARD_MASTER_DATA.Checked) selectedDocs.Add("BBOSE_ADMITCARD_MASTER_DATA");
        if (chkBBOSE_OLDRESULT_DATA.Checked) selectedDocs.Add("BBOSE_OLDRESULT_DATA");
        if (chkBBOSE_COPY_PRINTING_DATA.Checked) selectedDocs.Add("BBOSE_COPY_PRINTING_DATA");
       

        List<string> otherDocs = new List<string>
        {
            "Final Data",
            "Admit Card Master",
            "Bottom Barcode Master",
            "3 Year Carry Data",
            "Practical Data Structure",
            "Theory Data Structure",
            "Topper",
            "Missmatched",
            "Practical Marks",
            "Sample Result Data",
            "Scrutiny Result Data",
            "Scrutiny Print Master",
            "Scrutiny Theory Award",
			"Theory Attendance Sheet",
			"BBOSE_THEORY_OMR",
            "BBOSE_THEORY_FLY",
            "BBOSE_THEORY_ATTENDANCE",
            "BBOSE_PRACTICAL_ATTENDANCE",
            "BBOSE_PRACTICAL_FOIL",
            "BBOSE_THEORY_EXPULSION",
            "BBOSE_THEORY_AWARD",
            "BBOSE_THEORY_FOIL",
            "BBOSE_THEORY_ONLINE_MARKS",
            "BBOSE_PRACTICAL_AWARD",
            "BBOSE_ADMITCARD_MASTER_DATA",
            "BBOSE_OLDRESULT_DATA",
            "BBOSE_COPY_PRINTING_DATA"
        };
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            foreach (ListItem viewer in chkViewerAgencies.Items)
            {
                if (!viewer.Enabled || !viewer.Selected) continue;

                foreach (var doc in selectedDocs)
                {
                    //string documentType = string.IsNullOrEmpty(category) ? doc : category + " " + doc;
                    string documentType;
                    if (otherDocs.Contains(doc))
                    {
                        documentType = doc;
                    }
                    else
                    {
                        documentType = string.IsNullOrEmpty(category) ? doc : category + " " + doc;
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO AgencyDocumentAccess (OwnerAgency, ViewerAgency, DocumentType)
                        VALUES (@OwnerAgency, @ViewerAgency, @DocumentType);", con))
                    {
                        cmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                        cmd.Parameters.AddWithValue("@ViewerAgency", viewer.Value);
                        cmd.Parameters.AddWithValue("@DocumentType", documentType);
                        insertedCount += cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        ScriptManager.RegisterStartupScript(this, this.GetType(), "done",
            insertedCount > 0 ? "alert('Records inserted successfully!');" : "alert('No records were inserted.');", true);
    }

    private bool ValidateServerSide(out string error)
    {
        error = string.Empty;

        if (ddlOwnerAgency.SelectedValue == "ALL")
        {
            error = "Please select Owner Agency.";
            return false;
        }

        bool categorySelected = chkFinal.Checked || chkWanting.Checked || chkProcess.Checked;

        // NEW: Check for documents specifically from the "Document Options" section
        bool docOptionsSelected = chkAttendanceA.Checked || chkAttendanceB.Checked || chkOnlineData.Checked || chkLitho.Checked || chkFoilSheet.Checked || chkAbsentee.Checked || chkResultData.Checked;

        // Check for documents from the "Other Documents" section
        bool otherDocsSelected = chkFlyingSheet.Checked || chkAdmitCardMaster.Checked || chkBottomBarcode.Checked || chkCarryData.Checked || chkPracticalDS.Checked || chkTheoryDS.Checked || chkTheoryAS.Checked ||
                                 chkTopper.Checked || chkMissmatched.Checked || chkPracticalMarks.Checked || chkSampleResultData.Checked || chkScrutinyResultData.Checked || chkScrutinyPrintMaster.Checked || chkScrutinyTheoryAward.Checked || chkBBOSE_THEORY_OMR.Checked || chkBBOSE_THEORY_FLY.Checked || chkBBOSE_THEORY_ATTENDANCE.Checked || chkBBOSE_PRACTICAL_ATTENDANCE.Checked || chkBBOSE_PRACTICAL_FOIL.Checked || chkBBOSE_THEORY_EXPULSION.Checked || chkBBOSE_THEORY_AWARD.Checked || chkBBOSE_THEORY_FOIL.Checked || chkBBOSE_THEORY_ONLINE_MARKS.Checked || chkBBOSE_PRACTICAL_AWARD.Checked || chkBBOSE_ADMITCARD_MASTER_DATA.Checked || chkBBOSE_OLDRESULT_DATA.Checked || chkBBOSE_COPY_PRINTING_DATA.Checked;
        {

        };

        // Check for "exempt" documents (OMR, Flying, Award Sheet)
        bool anyExemptDocsSelected = chkOMRSheet.Checked || chkFlying.Checked || chkAwardSheet.Checked;

        // Check if any document at all is selected
        bool anyDocSelected = docOptionsSelected || otherDocsSelected || anyExemptDocsSelected;

        if (!anyDocSelected)
        {
            //error = "Please select at least one document.";
            //return false;
        }

        // The core validation logic: only require a category if a "Document Option" is selected.
        if (docOptionsSelected && !categorySelected)
        {
            error = "Please select a Category (Final / Wanted / Process) for the chosen documents.";
            return false;
        }

        // At least one viewer agency must be selected
        bool anyViewer = chkViewerAgencies.Items.Cast<ListItem>().Any(i => i.Enabled && i.Selected);
        if (!anyViewer)
        {
            error = "Please select at least one Viewer Agency.";
            return false;
        }

        return true;
    }
}