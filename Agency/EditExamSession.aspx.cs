using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_EditExamSession : System.Web.UI.Page
{
  FlureeCS fl = new FlureeCS();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] == null || Session["role"].ToString() != "Admin")
            {
                Response.Redirect("../login.aspx");
                return;
            }

            if (Request.QueryString["Id"] == null)
            {
                Response.Redirect("ExamSessionMaster.aspx");
                return;
            }

            int sessionId = Convert.ToInt32(Request.QueryString["Id"]);
            LoadSessionData(sessionId);
        }
    }

    private void LoadSessionData(int id)
    {
        DataRow dr = fl.GetExamSessionById(id);

        if (dr == null)
        {
            lblMessage.Text = "Session not found.";
            return;
        }

        hfSessionId.Value = dr["Id"].ToString();
        txtSessionName.Text = dr["SessionName"].ToString();
        ddlStatus.SelectedValue = Convert.ToBoolean(dr["IsActive"]) ? "1" : "0";
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(hfSessionId.Value);
        string sessionName = txtSessionName.Text.Trim();
        int status = Convert.ToInt32(ddlStatus.SelectedValue);

        if (string.IsNullOrEmpty(sessionName))
        {
            lblMessage.Text = "Session name is required.";
            return;
        }

        string result = fl.UpdateExamSession(id, sessionName, status);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "alert", "alert('{result}');", true);

        if (result.Contains("success"))
        {
            Response.Redirect("ExamSessionMaster.aspx");
        }
    }
}
