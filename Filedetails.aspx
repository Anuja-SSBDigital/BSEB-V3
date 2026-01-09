<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Filedetails.aspx.cs" Inherits="Filedetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
          <style>
   .file-container {
    margin: 20px;
    font-family: Arial, sans-serif;
}

.file-title {
    color: black;          /* black text */
    font-weight: bold;     /* bold text */
    text-align: center;    /* center horizontally */
    font-size: 18px;       /* keep size */
    padding: 10px;         /* spacing */
    /* Remove background-color if you want no background color */
}

table.file-table {
    width: 100%;
    border-collapse: collapse;
    margin: 0;
    table-layout: auto; /* let columns adjust width automatically */
    word-wrap: break-word;
    page-break-inside: auto; /* allow page breaks inside table */
}

table.file-table th, table.file-table td {
    border: 1px solid #000;
    padding: 8px;
    text-align: left;
    vertical-align: top; /* align content to top for better spacing */
    white-space: normal; /* allow wrapping */
    word-break: break-word; /* break long words */
}

table.file-table th {
    background-color: #f0f0f0;
}

table.file-table tr:nth-child(even) {
    background-color: #fafafa;
}

table.file-table tr:hover {
    background-color: #eaeaea;
}

/* Optional: prevent breaking rows across pages */
@media print {
    html, body {
        margin: 0 !important;
        padding: 0 !important;
    }

    /* Keep this from earlier */
    .file-container, .file-table, .file-table tr, .file-table td, .file-table th {
        page-break-inside: avoid !important;
        break-inside: avoid !important;
    }

    .file-container:not(:last-child) {
        page-break-after: always;
    }

    .print-btn-container {
        display: none !important;
    }

    table {
        width: 100%;
        table-layout: fixed;
    }

    td, th {
        word-wrap: break-word;
    }
}
 
</style>
            <style type="text/css" media="print">
    #btnPrint {
        display: none !important;
    }
    

     html, body {
        margin: 0 !important;
        padding: 0 !important;
        height: 100% !important;
    }

    @page {
        margin: 0; /* Remove default browser print margin */
        size: auto;
    }

    #btnPrint, .print-btn-container {
        display: none !important;
    }

    .file-container {
        page-break-after: auto;
    }

    .file-container:not(:last-child) {
        page-break-after: always;
    }

    table, tr, td, th {
        page-break-inside: avoid !important;
        break-inside: avoid !important;
    }
    
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
     <div class="print-btn-container">
     <asp:Button ID="btnPrint" runat="server" Text="PDF" OnClientClick="window.print(); return false;" />
 </div>

<div class="file-container" id="pagewrap">
    <div class="file-title">Blockchain File Record Details</div>
    <table class="file-table">
     <%--   <thead>
            <tr>
                <th style="width:5%;">#</th>
                <th style="width:15%;">Sub Doc Type</th>
                <th style="width:20%;">Actual File Name</th>
                <th style="width:20%;">File Name</th>
                <th style="width:25%;">File Hash</th>
                <th style="width:15%;">Uploaded Date</th>
            </tr>
        </thead>--%>
        <tbody>
         <asp:Repeater ID="rptUploadedFiles" runat="server" OnItemDataBound="rptUploadedFiles_ItemDataBound">
    <ItemTemplate>
        <div style="text-align: center; font-weight: bold; font-size: 16px; color: black; margin-top: 20px;">
            Upload Details
        </div>

        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;" border="1">
            <tr style="font-weight: bold;">

                <th style="padding: 5px;">#</th>
                <th style="padding: 5px;">File Name</th>
                <th style="padding: 5px;">Hash of the File</th>
                <th style="padding: 5px;">Uploaded By</th>
                <th style="padding: 5px;">File Uploaded On</th>
            </tr>
            <tr>
                <td>
    <asp:Label ID="lblRowNumber" Text='<%# Container.ItemIndex + 1 %>' runat="server" />
</td>
                <td style="padding: 5px;"><%# Eval("FileName") %></td>
                <td style="padding: 5px;"><%# Eval("FileHash") %></td>
                <td style="padding: 5px;"><%# Eval("UploadedBy") %></td>
                <td style="padding: 5px;"><%# Eval("UploadedDate") %></td>
            </tr>
        </table>

        <div style="text-align: center; font-weight: bold; font-size: 16px; color: black; margin-top: 20px;">
            Download Report Details
        </div>

        <asp:Repeater ID="rptDownloadDetails" runat="server">
            <ItemTemplate>
                <table style="width: 100%; border-collapse: collapse; margin-top: 10px;" border="1">
                    <tr>
                        <td style="padding: 5px; font-weight: bold;">Downloaded By:</td>
                        <td style="padding: 5px;"><%# Eval("RequestedBy") %></td>
                    </tr>
                    <tr>
                        <td style="padding: 5px; font-weight: bold;">Downloaded On:</td>
                        <td style="padding: 5px;"><%# Eval("RequestDate") %></td>
                    </tr>
                </table>
                <br />
            </ItemTemplate>
        </asp:Repeater>

        <hr style="border-top: 2px dashed #000; margin-top: 30px;" />
    </ItemTemplate>
</asp:Repeater>

        </tbody>
    </table>
</div>



        </div>
    </form>
</body>
</html>
