<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="AccessKeyManagement.aspx.cs" Inherits="AccessKeyManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12">
            <div class="card shadow-sm border-0 rounded-3">
                <div class="card-header bg-white border-bottom">
                    <h5 class="mb-0 text-dark">Private Key Management</h5>
                </div>
                <div class="card-body">


                    <div class="mb-3">
                        <label for="ddlOwnerAgency" class="form-label fw-bold text-dark">
                            Step 1: Select Agency
                        </label>
                        <asp:DropDownList ID="ddlOwnerAgency" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">-- Select Agency --</asp:ListItem>
                            <asp:ListItem Value="Antier">Antier</asp:ListItem>
                            <asp:ListItem Value="Charu Mindworks">Charu Mindworks</asp:ListItem>
                            <asp:ListItem Value="Datacon">Datacon</asp:ListItem>
                            <asp:ListItem Value="Hitech">Hitech</asp:ListItem>
                            <asp:ListItem Value="Kids">Kids</asp:ListItem>
                            <asp:ListItem Value="Mapple">Mapple</asp:ListItem>
                            <asp:ListItem Value="MCRK">MCRK</asp:ListItem>
                            <asp:ListItem Value="Shree Jagannath Udyog">Shree Jagannath Udyog</asp:ListItem>
                            <asp:ListItem Value="SSB Digital">SSB Digital</asp:ListItem>
                            <asp:ListItem Value="DatacenterBSEB">DatacenterBSEB</asp:ListItem>
                        </asp:DropDownList>
                    </div>


                    <div class="mb-3">
                        <label class="form-label fw-bold text-dark">
                            Step 2: Generate Token
                        </label>
                        <asp:Button ID="btnGenerateToken" runat="server"
                            Text="Generate Token" CssClass="btn btn-primary w-100"
                            OnClick="btnGenerateToken_Click" />
                    </div>


                    <div class="mb-3">
                        <label class="form-label fw-bold text-dark">
                            Generated Token
                        </label>
                        <asp:HiddenField runat="server" ID="hdnExpiryDate" />
                        <asp:TextBox runat="server" ID="lblToken" CssClass="form-control" Style="color: black !important;" ReadOnly="true" placeholder="Token will appear here"></asp:TextBox>
                    </div>


                    <div class="mb-3">
                        <label class="form-label fw-bold text-dark">
                            Step 3: Send Token
                        </label>
                        <asp:Button ID="btnSendEmail" runat="server"
                            Text="Send Token via Email" CssClass="btn btn-success w-100"
                            OnClick="btnSendEmail_Click" />
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4">
        <div class="col-12">
            <div class="card">

                <div class="card-header">
                    <h4>Agency User Key Data</h4>
                </div>

                <div class="card-body">

                    
                    <div class="row mb-3">
                        <div class="col-lg-6 col-md-6">
                            <label class="fw-bold">Select Agency</label>
                            <div class="form-group">
                                <div class="input-group">
                                    <asp:DropDownList ID="ddl_agency" runat="server" CssClass="custom-select">
                                        <asp:ListItem Value="">-- Select Agency --</asp:ListItem>
                                        <asp:ListItem Value="Antier">Antier</asp:ListItem>
                                        <asp:ListItem Value="Charu Mindworks">Charu Mindworks</asp:ListItem>
                                        <asp:ListItem Value="Datacon">Datacon</asp:ListItem>
                                        <asp:ListItem Value="Hitech">Hitech</asp:ListItem>
                                        <asp:ListItem Value="Kids">Kids</asp:ListItem>
                                        <asp:ListItem Value="Mapple">Mapple</asp:ListItem>
                                        <asp:ListItem Value="MCRK">MCRK</asp:ListItem>
                                        <asp:ListItem Value="Shree Jagannath Udyog">Shree Jagannath Udyog</asp:ListItem>
                                        <asp:ListItem Value="SSB Digital">SSB Digital</asp:ListItem>
                                        <asp:ListItem Value="DatacenterBSEB">DatacenterBSEB</asp:ListItem>
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                        <asp:Button ID="btnsearch" runat="server"
                                            Text="Search"
                                            CssClass="btn btn-primary"
                                            OnClick="btnsearch_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2 text-end">
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblMessage" runat="server"
                                CssClass="text-info fw-bold"></asp:Label>
                        </div>
                    </div>


                    <asp:Repeater ID="rpt_DocumentTypeData" runat="server">

                        <HeaderTemplate>
                            <div class="table-responsive">
                                <table class="table table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>Sr.No.</th>
                                            <th>User Name</th>
                                            <%-- <th>Email</th>
                                            <th>Mobile</th>--%>
                                            <th>Agency</th>
                                            <th>Token</th>
                                            <th>Expiry Date</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td><%# Eval("username") %></td>
                                <%--                                <td><%# Eval("email") %></td>
                                <td><%# Eval("mobileno") %></td>--%>
                                <td><%# Eval("agencyname") %></td>


                                <td>
                                    <asp:TextBox runat="server"
                                        Text='<%# Eval("PrivateKey") %>'
                                        CssClass="form-control form-control-sm"
                                        ReadOnly="true" />
                                </td>


                                <td>
                                    <%# Eval("Key_Expiry") == DBNull.Value 
                                    ? "-" 
                                    : Convert.ToDateTime(Eval("Key_Expiry")).ToString("dd-MMM-yyyy HH:mm") %>
                                </td>
                            </tr>
                        </ItemTemplate>

                        <FooterTemplate>
                            </tbody>
                            </table>
                        </div>
                        </FooterTemplate>

                    </asp:Repeater>

                </div>
            </div>
        </div>
    </div>

</asp:Content>
