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
    table.file-table tr {
        page-break-inside: avoid;
    }
}
 
</style>
            <style type="text/css" media="print">
    #btnPrint {
        display: none !important;
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
        <thead>
            <tr>
                <th style="width:5%;">#</th>
                <th style="width:15%;">Sub Doc Type</th>
                <th style="width:20%;">Actual File Name</th>
                <th style="width:20%;">File Name</th>
                <th style="width:25%;">File Hash</th>
                <th style="width:15%;">Uploaded Date</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptFileDetails" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblRowNumber" runat="server" Text='<%# Container.ItemIndex + 1 %>' />
                        </td>
                        <td><%# Eval("subdoctype") %></td>
                        <td><%# Eval("actualfilename") %></td>
                        <td><%# Eval("filename") %></td>
                        <td style="word-break: break-word;"><%# Eval("filehash") %></td>
                       <td><%# ConvertFromUnixTimestamp(Eval("createddate")) %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>



        </div>
    </form>
</body>
</html>
