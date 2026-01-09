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
       

        string dtUploadJson = fl.Getfiledetails();
        DataTable dtDownload = fl.getactivitylogdata();

        if (!dtUploadJson.StartsWith("Error"))
        {
            DataTable dtUpload = fl.Tabulate(dtUploadJson);

            var fileDetails = dtUpload.AsEnumerable()
                .Select(row => new
                {
                    FileName = row["filename"].ToString(),
                    FileHash = row["filehash"].ToString(),
                    UploadedBy = row["agencyname"].ToString(),
                    UploadedDate = ConvertFromUnixTimestamp(Convert.ToInt64(row["createddate"])), // Assuming upload date is a Unix timestamp
                    DownloadedBy = dtDownload.AsEnumerable()
    .Where(dl => dl["filename"].ToString() == row["filename"].ToString() &&
                 dl["agencyname"].ToString() != row["agencyname"].ToString()) // Exclude uploader
    .Select(dl => new
    {
        RequestedBy = dl["agencyname"].ToString(),
        RequestDate = Convert.ToDateTime(dl["createddate"])
    }).ToList()
                }).ToList();

            rptUploadedFiles.DataSource = fileDetails;
            rptUploadedFiles.DataBind();
        }
    }

    protected void rptUploadedFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            dynamic currentFile = e.Item.DataItem;
            Repeater rptDownloadDetails = (Repeater)e.Item.FindControl("rptDownloadDetails");

            rptDownloadDetails.DataSource = currentFile.DownloadedBy;
            rptDownloadDetails.DataBind();
        }
    }
    protected string ConvertFromUnixTimestamp(object timestamp)
    {
        if (timestamp == null || string.IsNullOrWhiteSpace(timestamp.ToString()))
            return "";

        long value;
        if (long.TryParse(timestamp.ToString(), out value))
        {
            try
            {
                DateTimeOffset dateTimeOffset;

                if (value > 9999999999) // likely milliseconds
                {
                    dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(value);
                }
                else // seconds
                {
                    dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(value);
                }

                return dateTimeOffset.UtcDateTime.ToString("dd-MM-yyyy HH:mm:ss");
                // Or return dateTimeOffset.LocalDateTime.ToString("dd-MM-yyyy HH:mm:ss");
            }
            catch (ArgumentOutOfRangeException)
            {
                return "Invalid timestamp";
            }
        }

        return "Invalid input";
    }

}
