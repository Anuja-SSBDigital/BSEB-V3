using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Agency_ExamSessionMaster : System.Web.UI.Page
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

        }  
    }
    protected void btnAddSession_Click(object sender, EventArgs e)
    {
        string sessionName = txtExamName.Text.Trim();

        if (string.IsNullOrEmpty(sessionName))
        {
            ScriptManager.RegisterStartupScript(this, GetType(),
                "alert", "alert('Please enter session name');", true);
            return;
        }

        string result = fl.InsertExamSession(sessionName);

        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                   "alert('" + result.Replace("'", "\\'") + "');", true);

       
        if (result.Contains("success"))
        {
            txtExamName.Text = "";
            BindSessionData();
        }
    }

    

    //                               
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindSessionData();
    }

    private void BindSessionData()
    {
        string status = ddl_Status.SelectedValue;
        DataTable dt = fl.GetExamSessionList(status);

        rpt_sessionData.DataSource = dt;
        rpt_sessionData.DataBind();
    }

    protected void rpt_sessionData_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int sessionId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "ToggleStatus")
        {
            fl.ToggleSessionStatus(sessionId);
            BindSessionData();
        }
        else if (e.CommandName == "EditSession")
        {
            Response.Redirect("EditExamSession.aspx?Id=" + sessionId);
        }
    }
}