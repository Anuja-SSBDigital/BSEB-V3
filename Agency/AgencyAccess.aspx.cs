using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AgencyAccess : System.Web.UI.Page
{      
    string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                BindAgencies();
                BindDocumentTypes();
            }
            else            
            {
                Response.Redirect("../login.aspx");
            }
        }
    }
    private void BindAgencies()
    {
        chkViewerAgencies.Items.Clear();

        chkViewerAgencies.Items.Add(new ListItem("<b>BSEB DATACENTER</b>", "") { Enabled = false });
        chkViewerAgencies.Items.Add(new ListItem("DatacenterBSEB", "DatacenterBSEB"));

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

    private void BindDocumentTypes()
    {
        DataTable dt = GetActiveDocumentTypes();
        rptDocumentTypes.DataSource = dt;
        rptDocumentTypes.DataBind();


        if (ddlOwnerAgency.SelectedValue != "ALL" && ddlOwnerAgency.SelectedValue != "")
        {
            PreselectDocumentTypes(ddlOwnerAgency.SelectedValue);
        }
    }

    public DataTable GetActiveDocumentTypes()
    {
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT DocTypeName FROM Documenttypemaster where isactive=1   ORDER BY CreatedDate DESC";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }
        return dt; 
    }

    private void PreselectDocumentTypes(string ownerAgency)
    {

        DataTable dtSelected = new DataTable();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT DISTINCT DocumentType FROM AgencyDocumentAccess WHERE OwnerAgency = @OwnerAgency";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtSelected);
                }
            }
        }

        foreach (RepeaterItem item in rptDocumentTypes.Items)
        {
            CheckBox chk = (CheckBox)item.FindControl("chkDoc");
            if (chk != null)
            {
                foreach (DataRow dr in dtSelected.Rows)
                {
                    if (chk.Text == dr["DocumentType"].ToString())
                    {
                        chk.Checked = true;
                        break;
                    }
                }
            }
        }
    }
   
    protected void ddlOwnerAgency_SelectedIndexChanged(object sender, EventArgs e)
    {
            
        BindDocumentTypes();      
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string error;
        if (!ValidateServerSide(out error))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "vFail",
                string.Format("alert('{0}');", error.Replace("'", "\\'")), true);
            return;
        }
             
        string ownerAgency = ddlOwnerAgency.SelectedValue;
        List<string> selectedDocs = new List<string>();

        foreach (RepeaterItem item in rptDocumentTypes.Items)
        {
            CheckBox chk = item.FindControl("chkDoc") as CheckBox;
            Label lbl = item.FindControl("lblDocName") as Label;

            if (chk != null && chk.Checked)
            {
                string docName = lbl != null ? lbl.Text : chk.Text;
                selectedDocs.Add(docName);
            }
        }
       
        List<string> selectedViewers = new List<string>();
        foreach (ListItem viewer in chkViewerAgencies.Items)
        {
            if (viewer.Enabled && viewer.Selected)
            {
                selectedViewers.Add(viewer.Value);
            }
        }     

        string connectionString = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            int insertedCount = 0;
            int skippedCount = 0;

            foreach (string viewerAgency in selectedViewers)
            {
                foreach (string doc in selectedDocs)
                {

                    using (SqlCommand checkCmd = new SqlCommand(@"
                        SELECT COUNT(*) FROM AgencyDocumentAccess 
                        WHERE OwnerAgency = @OwnerAgency 
                          AND ViewerAgency = @ViewerAgency 
                          AND DocumentType = @DocumentType", con))
                    {
                        checkCmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                        checkCmd.Parameters.AddWithValue("@ViewerAgency", viewerAgency);
                        checkCmd.Parameters.AddWithValue("@DocumentType", doc);

                        int exists = (int)checkCmd.ExecuteScalar();

                        if (exists > 0)
                        {

                            skippedCount++;
                            continue;
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO AgencyDocumentAccess 
                        (OwnerAgency, ViewerAgency, DocumentType) 
                        VALUES (@OwnerAgency, @ViewerAgency, @DocumentType)", con))
                    {
                        cmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                        cmd.Parameters.AddWithValue("@ViewerAgency", viewerAgency);
                        cmd.Parameters.AddWithValue("@DocumentType", doc);

                        insertedCount += cmd.ExecuteNonQuery();
                    }
                }
            }


            string finalMessage = "";

            if (insertedCount > 0)
            {
                finalMessage = "Records saved successfully! Inserted: " + insertedCount +
                               ", Skipped (already existed): " + skippedCount;
            }
            else if (skippedCount > 0)
            {
                finalMessage = "All selected records already exist! Skipped: " + skippedCount;
            }
            else
            {
                finalMessage = "No records were selected.";
            }

            ScriptManager.RegisterStartupScript(
    this,
    this.GetType(),
    "done",
    string.Format("alert('{0}');", finalMessage.Replace("'", "\\'")),
    true
);

        }
    }

    private bool ValidateServerSide(out string error)
    {
        error = "";

        if (ddlOwnerAgency.SelectedValue == "ALL" || ddlOwnerAgency.SelectedValue == "")
        {
            error = "Please select Owner Agency.";
            return false;     
        }

        bool anyDocSelected = false;
        foreach (RepeaterItem item in rptDocumentTypes.Items)    
        {
            CheckBox chk = item.FindControl("chkDoc") as CheckBox;
            if (chk != null && chk.Checked)
            {
                anyDocSelected = true;
                break;
            }
        }

        if (!anyDocSelected)
        {
            error = "Please select at least one document.";
            return false;
        }

        bool anyViewer = false;
        foreach (ListItem li in chkViewerAgencies.Items)
        {
            if (li.Enabled && li.Selected)
            {
                anyViewer = true;
                break;
            }
        }

        if (!anyViewer)
        {
            error = "Please select at least one Viewer Agency.";
            return false;
        }

        return true;
    }
}