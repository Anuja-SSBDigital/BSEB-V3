<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="ExamSessionMaster.aspx.cs" Inherits="Agency_ExamSessionMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="row" runat="server" id="div_search">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>Exam Session Master</h4>
                </div>
                <div class="card-body">
                    <div class="row">

                        <div class="col-lg-4">
                            <h5>Exam Session Name</h5>
                            <asp:TextBox ID="txtExamName" runat="server"
                                CssClass="form-control"
                                Placeholder="Enter session name"></asp:TextBox>
                        </div>

                        <div class="col-lg-2 mt-4">
                            <asp:Button ID="btnAddSession" runat="server"
                                Text="Submit"
                                CssClass="btn btn-primary"
                                OnClick="btnAddSession_Click" />
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblAddMessage" runat="server"
                                CssClass="text-success"></asp:Label>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="row mt-3">
        <div class="col-12">
            <div class="card">

                <div class="card-header">
                    <h4>Exam Session List</h4>
                </div>

                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-lg-4">
                            <h5>Select Status</h5>
                            <asp:DropDownList ID="ddl_Status" runat="server"
                                CssClass="form-control">
                                <asp:ListItem Value="" Text="All" Selected="True" />
                                <asp:ListItem Value="1" Text="Active" />
                                <asp:ListItem Value="0" Text="Inactive" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-2 mt-4">
                            <asp:Button ID="btnSearch" runat="server"
                                Text="Search"
                                CssClass="btn btn-primary"
                                OnClick="btnSearch_Click" />
                        </div>
                    </div>

                    <asp:Repeater ID="rpt_sessionData" runat="server"
                        OnItemCommand="rpt_sessionData_ItemCommand">

                        <HeaderTemplate>
                            <table class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Session Name</th>
                                        <th>Status</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td><%# Eval("SessionName") %></td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                </td>
                                <td>
                                    <asp:Button ID="btnEdit" runat="server"
                                        Text="Edit"
                                        CssClass="btn btn-sm btn-primary"
                                        CommandName="EditSession"
                                        CommandArgument='<%# Eval("Id") %>' />

                                    <asp:Button ID="btnDeactivate" runat="server"
                                        Text="Deactivate"
                                        CssClass="btn btn-sm btn-danger"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        Visible='<%# Convert.ToBoolean(Eval("IsActive")) %>'
                                        OnClientClick="return confirm('Deactivate this session?');" />

                                    <asp:Button ID="btnActivate" runat="server"
                                        Text="Activate"
                                        CssClass="btn btn-sm btn-success"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        Visible='<%# !Convert.ToBoolean(Eval("IsActive")) %>'
                                        OnClientClick="return confirm('Activate this session?');" />
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

</asp:Content>