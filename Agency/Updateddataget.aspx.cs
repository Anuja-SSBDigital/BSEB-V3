using System;
using System.Collections.Generic;
using System.Net.Http; 
using System.Threading.Tasks;
using System.Web.UI; 
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Linq;
using System.Configuration;
using System.Data;

public partial class Agency_Updateddataget : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();

    string lastGroup_v1 = "";
    string lastGroup_v2 = "";

    bool hasCCE_v1 = false;
    bool hasCCE_v2 = false;

    Dictionary<string, int> groupCount_v1 = new Dictionary<string, int>();
    Dictionary<string, int> groupCount_v2 = new Dictionary<string, int>();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Session["userid"] != null)
                {
                    string userRole = Session["role"] != null ? Session["role"].ToString() : "";

                    if (userRole != "Admin")
                    {
                        Response.Redirect("../login.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect("../login.aspx", false);
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('An unexpected error occurred during page load.');", true);
        }
    }

    protected async void btn_search_Click(object sender, EventArgs e)
    {


        string rc = rollCode.Text.Trim();
        string rn = rollNo.Text.Trim();

        int x, y;
        if (!int.TryParse(rc, out x) || !int.TryParse(rn, out y))
        {
            ClearUI();

            ScriptManager.RegisterStartupScript(this, GetType(), "a",
            "Swal.fire('Error','Invalid Roll Code / Roll No','error');", true);
            return;
        }

        await BindData(rc, rn);
    }


    private async Task BindData(string rc, string rn)
    {
        try
        {      
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12;

            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

            using (HttpClient client = new HttpClient())
            {

                string url = baseUrl + "api/ResultPublish/decrypt?rollCode=" + rc + "&rollNo=" + rn;

                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (json.TrimStart().StartsWith("{"))
                {
                    dynamic err = JsonConvert.DeserializeObject(json);

                    ClearUI();

                    ScriptManager.RegisterStartupScript(this, GetType(), "err",
                        "Swal.fire('No Data','" + err.message + "','warning');", true);

                    return;
                }


                List<ApiResponse> dataList = JsonConvert.DeserializeObject<List<ApiResponse>>(json);

                if (dataList == null || dataList.Count == 0)
                {
                    ClearUI();

                    ScriptManager.RegisterStartupScript(this, GetType(), "err",
                        "Swal.fire('No Data','Invalid Roll Code or Roll No','warning');", true);

                    return;
                }

                ApiResponse v1 = dataList.Find(delegate (ApiResponse x) { return x.version == "ENC_v1"; });
                ApiResponse v2 = dataList.Find(delegate (ApiResponse x) { return x.version == "ENC_v2"; });

                lastGroup_v1 = "";
                lastGroup_v2 = "";

                hasCCE_v1 = false;
                hasCCE_v2 = false;



                if (v1 != null && v1.data != null && v1.data.subjectResults != null)
                {
                    hasCCE_v1 = v1.data.subjectResults.Exists(delegate (SubjectResult s)
                    {
                        string val = (s.cceMarks ?? "").Trim();
                        return val != "" && val != "0" && val != "0.0";
                    });
                }

                if (v2 != null && v2.data != null && v2.data.subjectResults != null)
                {
                    hasCCE_v2 = v2.data.subjectResults.Exists(delegate (SubjectResult s)
                    {
                        string val = (s.cceMarks ?? "").Trim();
                        return val != "" && val != "0" && val != "0.0";
                    });
                }

                thCCE_v1.Visible = hasCCE_v1;
                thCCE_v2.Visible = hasCCE_v2;


                if (v1 != null && v1.data != null && v1.data.subjectResults != null)
                {
                    v1.data.subjectResults = v1.data.subjectResults
                        .OrderBy(x => (x.subjectGroupName ?? "").Trim())
                        .ToList();

                    groupCount_v1 = v1.data.subjectResults
                        .GroupBy(x =>
                        {
                            string g = (x.subjectGroupName ?? "").Trim();
                            return g.ToLower().Contains("vocational") ? "Vocational Trade" : g;
                        })
                        .ToDictionary(g => g.Key, g => g.Count());
                }

                if (v2 != null && v2.data != null && v2.data.subjectResults != null)
                {
                    v2.data.subjectResults = v2.data.subjectResults
                        .OrderBy(x => (x.subjectGroupName ?? "").Trim())
                        .ToList();

                    groupCount_v2 = v2.data.subjectResults
                        .GroupBy(x =>
                        {
                            string g = (x.subjectGroupName ?? "").Trim();
                            return g.ToLower().Contains("vocational") ? "Vocational Trade" : g;
                        })
                        .ToDictionary(g => g.Key, g => g.Count());
                }


                rpt_v1.DataSource = (v1 != null && v1.data != null) ? v1.data.subjectResults : null;
                rpt_v1.DataBind();

                rpt_v2.DataSource = (v2 != null && v2.data != null) ? v2.data.subjectResults : null;
                rpt_v2.DataBind();


                ApiResponse latest = dataList[dataList.Count - 1];
                StudentData d = latest.data;

                lblName.Text = d.sn;
                lblFather.Text = d.fn;
                lblRegNo.Text = d.regno;
                lblRollCode.Text = d.rollCode;
                lblRollNo.Text = d.rollNo;
                lblUID.Text = d.bsebUniqueID;
                lblFaculty.Text = d.faculty;
                lblCollege.Text = d.clgname;

                if (v1 != null && v1.data != null)
                {
                    V1totalmarks.Text = v1.data.totalAggMarks;
                    V1division.Text = v1.data.division;
                }
                else
                {
                    V2totalmarks.Text = "";
                    V2division.Text = "";
                }


                if (v2 != null && v2.data != null)
                {
                    V2totalmarks.Text = v2.data.totalAggMarks;
                    V2division.Text = v2.data.division;
                }
                else
                {
                    V2totalmarks.Text = "";
                    V2division.Text = "";
                }
                          


                Student_details.Visible = true;
            }
        }

        catch (HttpRequestException)
        {

            ClearUI();

            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "Swal.fire('Error','Unable to Connect Remoter server or Api Not Connected','error');", true);
        }

        catch (Exception ex)       
        {
            ClearUI();

            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "Swal.fire('Error','Something went wrong','error');", true);

            lblMessage.Text = ex.Message;
        }
    }


    
    protected void rpt_v1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            SubjectResult data = (SubjectResult)e.Item.DataItem;

            string currentGroup = (data.subjectGroupName ?? "").Trim();

            if (currentGroup.ToLower().Contains("vocational"))
                currentGroup = "Vocational Trade";

            System.Web.UI.HtmlControls.HtmlTableCell td =
                (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdGroup");

            if (currentGroup == lastGroup_v1)
            {
                td.Visible = false;
            }
            else
            {
                td.InnerText = currentGroup;

                td.RowSpan = groupCount_v1.ContainsKey(currentGroup)
                    ? groupCount_v1[currentGroup]
                    : 1;

                lastGroup_v1 = currentGroup;
            }

            System.Web.UI.HtmlControls.HtmlTableCell tdCCE =
                (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdCCE_v1");

            if (tdCCE != null)
                tdCCE.Visible = hasCCE_v1;
        }
    }
    protected void rpt_v2_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            SubjectResult data = (SubjectResult)e.Item.DataItem;

            string currentGroup = (data.subjectGroupName ?? "").Trim();

            if (currentGroup.ToLower().Contains("vocational"))
                currentGroup = "Vocational Trade";

            System.Web.UI.HtmlControls.HtmlTableCell td =
                (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdGroup_v2");

            if (currentGroup == lastGroup_v2)
            {
                td.Visible = false;
            }
            
            else
            {
                td.InnerText = currentGroup;

                td.RowSpan = groupCount_v2.ContainsKey(currentGroup)
                    ? groupCount_v2[currentGroup]
                    : 1;

                lastGroup_v2 = currentGroup;
            }

            System.Web.UI.HtmlControls.HtmlTableCell tdCCE =
                (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdCCE_v2");

            if (tdCCE != null)
                tdCCE.Visible = hasCCE_v2;
        }
    }

    private void ClearUI()
    {
        Student_details.Visible = false;

        lblName.Text = "";
        lblFather.Text = "";
        lblRegNo.Text = "";
        lblRollCode.Text = "";
        lblRollNo.Text = "";
        lblUID.Text = "";
            
        lblFaculty.Text = "";
        lblCollege.Text = "";
      
        rpt_v1.DataSource = null;
        rpt_v1.DataBind();

        rpt_v2.DataSource = null;
        rpt_v2.DataBind();
    }
}
