<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="duplicateordiscrepancy.aspx.cs" Inherits="Agency_duplicateordiscrepancy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   <div class="row">
    <div class="col-12">
        <div class="card">
            <div class="card-header">
                <h4>OMR Sheet Data</h4>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <h5 class="mb-3">
                        Duplicate/Discrepency OMR Sheet Files Available
                        
                    </h5>
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="d-block mb-3"></asp:Label>

                    <asp:Repeater ID="rptCSVFiles" runat="server">
                        <HeaderTemplate>
                            <table class="table table-bordered">
                                <thead class="thead-dark">
                                    <tr>
                                        <th>File Name</th>
                                        <th>Download</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("fileName") %></td>
                                <td>
                                    <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>'/>
                                    <asp:Button ID="btnDownload" runat="server" CssClass="btn btn-primary btn-sm"
                                        Text="Download" CommandArgument='<%# Eval("filePath") %>'
                                        OnClick="btnDownload_Click" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</div>

</asp:Content>

