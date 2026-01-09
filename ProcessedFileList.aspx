<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProcessedFileList.aspx.cs" Inherits="ProcessedFileList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Unprocessed Files</h2>
        <asp:Repeater ID="rptProcessedFiles" runat="server" OnItemCommand="rptProcessedFiles_ItemCommand">
            <HeaderTemplate>
                <table border="1" cellpadding="5">
                    <tr>
                        <th>Filename</th>
                        <th>Agency</th>
                        <th>Uploaded By</th>
                        <th>Action</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Filename") %></td>
                    <td><%# Eval("Agency") %></td>
                    <td><%# Eval("UploadedBy") %></td>
                    <td>
                        <asp:Button ID="btnStart" runat="server" CommandName="StartProcessing"
                            CommandArgument='<%# Eval("FilePath") %>' Text="Start" />
                    </td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                </table>          
            </FooterTemplate>
        </asp:Repeater>
    </form>
</body>
</html>