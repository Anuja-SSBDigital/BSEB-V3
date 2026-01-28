<%@ Page Title="Document Type Master" Language="C#" MasterPageFile="~/Agency/MasterPage.master"
    AutoEventWireup="true" CodeFile="DocumentTypeMaster.aspx.cs" Inherits="Agency_DocumentTypeMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row" runat="server" id="div_search">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>Document Type Master</h4>
                </div>

                <div class="card-body">
                    <div class="row">

                        <div class="col-lg-4 col-md-6 col-sm-6 col-12 px-2" runat="server" id="Div_admin">
                            <h5 class="font-15">Document Type</h5>
                            <asp:TextBox runat="server" ID="txtCategoryName" CssClass="form-control"
                                Placeholder="Document Type Name">
                            </asp:TextBox>
                        </div>

                        <div class="col-lg-2 col-md-2 col-sm-12 px-2 form-inline">
                            <asp:Button runat="server" ID="btnAddCategory" OnClick="btnAddCategory_Click"
                                CssClass="btn btn-primary mt-4" Text="Submit" />
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblAddMessage" runat="server" CssClass="text-success d-block mb-2"></asp:Label>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </div>


    <div class="row mt-3">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>Document Type Master Data</h4>
                </div>

                <div class="card-body">


                    <div class="row mb-3">
                        <div class="col-lg-4 col-md-6 col-sm-6 col-12 px-2">
                            <h5 class="font-15">Select Status</h5>
                            <div class="form-group">
                                <div class="input-group">
                                    <asp:DropDownList runat="server" ID="ddl_Status" CssClass="form-control">
                                        <asp:ListItem Value="" Text="All" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Inactive"></asp:ListItem>
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                        <asp:Button runat="server" ID="btnsearch" OnClick="btnsearch_Click"
                                            CssClass="btn btn-primary" Text="Search" />
                                    </div>
                                </div>
                            </div>
                        </div>



                        <div class="col-12 mt-2">
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-info d-block mb-2"></asp:Label>
                        </div>
                    </div>


                    <asp:Repeater runat="server" ID="rpt_DocumentTypeData" OnItemCommand="rpt_DocumentTypeData_ItemCommand">
                        <headertemplate>
                            <div class="table-responsive">
                                <table class="table table-striped" id="table-1">
                                    <thead>
                                        <tr>
                                            <th>Sr.No.</th>
                                            <th>Document Type Name</th>
                                            <th>Status</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        </headertemplate>

                        <itemtemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td><%# Eval("DocTypeName") %></td>
                                <td><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></td>
                                <td>

                                    <%--  <asp:Button runat="server" ID="btnEdit" Text="Edit"
                                        CommandName="EditDoc"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-primary btn-sm" />--%>

                                    <asp:Button runat="server" ID="btnDeactivate" Text="Deactivate"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-danger btn-sm"
                                        OnClientClick="return confirm('Are you sure you want to deactivate this record?');"
                                        Visible='<%# Convert.ToBoolean(Eval("IsActive")) %>' />

                                    <asp:Button runat="server" ID="btnActivate" Text="Activate"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-success btn-sm"
                                        OnClientClick="return confirm('Are you sure you want to activate this record?');"
                                        Visible='<%# !Convert.ToBoolean(Eval("IsActive")) %>' />
                                </td>
                            </tr>
                        </itemtemplate>

                        <footertemplate>
                    </tbody>
                                </table>
                </div>
                </FooterTemplate>
                    </asp:Repeater>

            </div>
        </div>
    </div>

</asp:Content>
