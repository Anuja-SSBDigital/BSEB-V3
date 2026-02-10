using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
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
                BindDocCategory();
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

    private void BindDocCategory()
    {
        FlureeCS fl = new FlureeCS();
        DataTable dt = fl.DocumentCategoryMaster();

        ddl_doctype.Items.Clear();

        if (dt != null && dt.Rows.Count > 0)
        {
            ddl_doctype.DataSource = dt;
            ddl_doctype.DataTextField = "DocCategoryName";
            ddl_doctype.DataValueField = "doctypeId";
            ddl_doctype.DataBind();
        }

        ddl_doctype.Items.Insert(0, new ListItem("Select Doc Category", "0"));
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
        DataTable dt = fl.GetSubdoctypeforAccess(docTypeId);

        foreach (DataRow row in dt.Rows)
        {
            SubDocTypeVM vm = new SubDocTypeVM();
            vm.subdocId = Convert.ToInt32(row["subdocId"]);
            vm.subdoctypename = row["subdoctypename"].ToString();
            list.Add(vm);
        }
        return list;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string error;
        if (!ValidateServerSide(out error))
        {
            ScriptManager.RegisterStartupScript(
                this, this.GetType(), "vFail",
                "alert('" + error.Replace("'", "\\'") + "');", true);
            return;
        }

        string ownerAgency = ddlOwnerAgency.SelectedValue;
        string docTypeCategory = ddl_doctype.SelectedItem.Text; // Category → DocType



        string doctypeId = ddl_doctype.SelectedValue;


        // Selected Sub Document Types
        List<string> selectedDocs = hfSubdoctypeNames.Value
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();

        if (selectedDocs.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "noDocs",
                "alert('Please select at least one Sub Document Type.');", true);
            return;
        }

        // Selected Viewer Agencies
        List<string> selectedViewers = new List<string>();
        foreach (ListItem item in chkViewerAgencies.Items)
        {
            if (item.Selected && item.Enabled)
            {
                selectedViewers.Add(item.Value);
            }
        }

        if (selectedViewers.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "noViewers",
                "alert('Please select at least one Viewer Agency.');", true);
            return;
        }

        int insertedCount = 0;
        int skippedCount = 0;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            foreach (string viewerAgency in selectedViewers)
            {
                foreach (string subDoc in selectedDocs)
                {
                    string docType = docTypeCategory; // Category
                    string subDocType = subDoc;       // Sub document

                    // 🔎 Skip check (active only)
                    int activeCount = 0;
                    using (SqlCommand checkCmd = new SqlCommand(
                        @"SELECT COUNT(*) 
                      FROM AgencyDocumentAccess
                      WHERE OwnerAgency=@OwnerAgency
                        AND ViewerAgency=@ViewerAgency
                        AND DocType=@DocType
                        AND SubDocType=@SubDocType
                        AND IsActive=1", con))
                    {
                        checkCmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                        checkCmd.Parameters.AddWithValue("@ViewerAgency", viewerAgency);
                        checkCmd.Parameters.AddWithValue("@DocType", docType);
                        checkCmd.Parameters.AddWithValue("@SubDocType", subDocType);

                        activeCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                    }

                    if (activeCount > 0)
                    {
                        skippedCount++;
                        continue;
                    }


                    using (SqlCommand insertCmd = new SqlCommand(
                        @"INSERT INTO AgencyDocumentAccess
                      (OwnerAgency, ViewerAgency, SubDocType, DocType, createddate,doctypeId, IsActive)
                      VALUES
                      (@OwnerAgency, @ViewerAgency, @SubDocType, @DocType, GETDATE(),@doctypeId, 1)", con))
                    {
                        insertCmd.Parameters.AddWithValue("@OwnerAgency", ownerAgency);
                        insertCmd.Parameters.AddWithValue("@ViewerAgency", viewerAgency);
                        insertCmd.Parameters.AddWithValue("@SubDocType", subDocType);
                        insertCmd.Parameters.AddWithValue("@DocType", docType);

                        insertCmd.Parameters.AddWithValue("@doctypeId", doctypeId);

                        insertCmd.ExecuteNonQuery();
                        insertedCount++;
                    }

                }
            }
        }

        // ✅ Final message + reset form (no refresh)
        string finalMessage = "Inserted: " + insertedCount + ", Skipped: " + skippedCount;

        string script =
            "alert('" + finalMessage.Replace("'", "\\'") + "');" +
            "document.getElementById('" + ddlOwnerAgency.ClientID + "').value='ALL';" +
            "document.getElementById('" + ddl_doctype.ClientID + "').value='0';" +
            "document.getElementById('" + hfSubdoctypeIds.ClientID + "').value='';" +
            "document.getElementById('" + hfSubdoctypeNames.ClientID + "').value='';" +
            "document.getElementById('subDocContainer').innerHTML='<span class=\"text-muted\">Please select category</span>';" +
            "var v=document.getElementById('" + chkViewerAgencies.ClientID + "').getElementsByTagName('input');" +
            "for(var i=0;i<v.length;i++){v[i].checked=false;}";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "done", script, true);
    }


    private bool ValidateServerSide(out string error)
    {
        error = "";

        if (ddlOwnerAgency.SelectedValue == "ALL" || ddlOwnerAgency.SelectedValue == "")
        {
            error = "Please select Owner Agency.";
            return false;
        }

        if (string.IsNullOrEmpty(hfSubdoctypeIds.Value))
        {
            error = "Please select at least one Sub Document Type.";
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
