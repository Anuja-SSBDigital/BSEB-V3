<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="approveprofile.aspx.cs" Inherits="Agency_approveprofile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>User Detailes </h4>
                </div>

                <div class="card-body">
                    <div class="row">

                        <%--  <div class="col-md-6">
                            <div class="form-group">
                                <label for="agency-name">Agency Name</label>
                                <asp:DropDownList runat="server" ID="ddl_AgencyName" CssClass="form-control" Required="true">
                                    <asp:ListItem Value="" Text="Select Agency Name" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Mappel" Text="Mappel"></asp:ListItem>
                                    <asp:ListItem Value="Datacone" Text="Datacone"></asp:ListItem>
                                    <asp:ListItem Value="Kids" Text="Kids"></asp:ListItem>
                                    <asp:ListItem Value="MCRK" Text="MCRK"></asp:ListItem>
                                    <asp:ListItem Value="Keltron" Text="Keltron"></asp:ListItem>
                                    <asp:ListItem Value="Charu Mindworks" Text="Charu Mindworks"></asp:ListItem>
                                </asp:DropDownList>
                                <div class="invalid-feedback">Please select User name</div>
                            </div>
                        </div>--%>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="agency-name">Agency Name</label>

                                <asp:DropDownList
                                    runat="server"
                                    ID="ddl_AgencyName"
                                    CssClass="form-control"
                                    AppendDataBoundItems="true">
                                    <asp:ListItem Value="" Text="Select Agency Name"></asp:ListItem>
                                </asp:DropDownList>

                                <div class="invalid-feedback">Please select Agency Name</div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="agency-name">User Status</label>
                                <asp:DropDownList runat="server" ID="ddl_Userstatus" CssClass="form-control" Required="true">
                                    <asp:ListItem Value="ALL" Text="Select User status" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="DeActive" Text="DeActive"></asp:ListItem>
                                    <asp:ListItem Value="Pending For Approval" Text="Pending For Approval"></asp:ListItem>
                                </asp:DropDownList>
                                <div class="invalid-feedback">Please select User name</div>
                            </div>
                        </div>


                    </div>
                </div>
                <div class="card-footer">
                    <asp:Button runat="server" ID="btn_submit" Text="Submit" CssClass="btn btn-primary btn-lg " OnClick="btn_submit_Click" />

                </div>
            </div>
        </div>

        <div class="col-12" runat="server" id="User_detailes" visible="false">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>User Detailes</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr No</th>
                                    <th>User Name</th>

                                    <th>Email</th>
                                    <th>Mobile No</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>

                                <asp:Repeater runat="server" ID="rpt_userData" OnItemCommand="rpt_userData_ItemCommand" OnItemDataBound="rpt_userData_ItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:HiddenField runat="server" ID="hf_emailid" Value='<%#Eval("email") %>' />
                                                <asp:HiddenField runat="server" ID="hf_agency" Value='<%#Eval("agencyname") %>' />
                                                <asp:HiddenField ID="hf_status" runat="server" Value='<%#Eval("status") %>' />
                                                <asp:HiddenField runat="server" ID="hf_username" Value='<%#Eval("username") %>' />
                                                <asp:HiddenField runat="server" ID="hf_userid" Value='<%#Eval("id") %>' />

                                                <asp:Label ID="lblRowNumber" Text='<%# Container.ItemIndex + 1 %>' runat="server" />
                                            </td>
                                            <td><%#Eval("username") %></td>

                                            <td><%#Eval("email") %></td>
                                            <td><%#Eval("mobileno") %></td>
                                            <td>
                                                <asp:Label ID="lbl_userstatus" runat="server" Text='<%#Eval("status") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="link_approve" CommandName="link_approve"
                                                    CommandArgument='<%#Eval("id") %>'
                                                    runat="server" data-bs-toggle="tooltip" data-placement="right"
                                                    title="Approve" CssClass="btn-icon btn-success btn-sm "
                                                    Visible="false" OnClientClick="return confirm('Are you sure you want to Approve This User?');">
                                                    <i class="fas fa-check"></i>
                                                </asp:LinkButton>
                                                &nbsp;
                                                            <asp:LinkButton ID="link_rejected" CommandName="link_rejected" data-bs-toggle="tooltip" data-placement="right" title="Reject"
                                                                CommandArgument='<%#Eval("id") %>' runat="server"
                                                                CssClass="btn-icon btn-danger btn-sm" Visible="false"
                                                                OnClientClick="return confirm('Are you sure you want to Reject This User?');">
                                                                <i class="fas fa-times"></i>
                                                            </asp:LinkButton>
                                                <asp:LinkButton ID="link_Active" CommandName="link_Active" CommandArgument='<%#Eval("id") %>' runat="server" CssClass="btn-icon btn-success btn-sm" Visible="false">
                                                    Active
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="link_DeActive" CommandName="link_DeActive"
                                                    CommandArgument='<%#Eval("id") %>' runat="server"
                                                    CssClass="btn-icon btn-danger btn-sm" Visible="false">
                                                    Deactive</asp:LinkButton>
                                            </td>


                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>


                </div>
            </div>
        </div>
    </div>
</asp:Content>