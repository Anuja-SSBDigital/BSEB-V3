using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Filedetails : System.Web.UI.Page
{
    FlureeCS fl = new FlureeCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            getfiledetails();
        }
    }

    public void getfiledetails()
    {
        string res = fl.Getfiledetails();
        if (!res.StartsWith("Error"))
        {
            DataTable dtdata = fl.Tabulate(res);
            if (dtdata.Rows.Count > 0)
            {
                rptFileDetails.DataSource = dtdata;
                rptFileDetails.DataBind();
            }
        }

    }
    protected string ConvertFromUnixTimestamp(object timestamp)
    {
        if (timestamp == null || timestamp.ToString() == "")
            return "";

        double milliseconds;
        if (double.TryParse(timestamp.ToString(), out milliseconds))
        {
            // Convert milliseconds to seconds if needed
            if (milliseconds > 9999999999)
                milliseconds /= 1000;

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)milliseconds);
            return dateTimeOffset.LocalDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        return "";
    }
}