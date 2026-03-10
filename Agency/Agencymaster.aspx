<%@ Page Title="Add Agency User" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Agencymaster.aspx.cs" Inherits="Agency_Agencymaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        function validateForm() {

            var username = document.getElementById('<%=txtUsername.ClientID%>').value.trim();
            var email = document.getElementById('<%=txtEmail.ClientID%>').value.trim();
            var mobile = document.getElementById('<%=txtMobile.ClientID%>').value.trim();
            var agency = document.getElementById('<%=txtAgency.ClientID%>').value.trim();

            if (username === "") {
                alert("Please Enter Username");
                return false;
            }
            if (email === "") {
                alert("Please Enter Email");
                return false;
            }
            if (mobile === "") {
                alert("Please Enter Mobile number");
                return false;
            }
            if (agency === "") {
                alert("Please Enter Agency Name ");
                return false;
            }

            return true;
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="card card-primary">

        <div class="card-header">
            <h4>Add Agency User</h4>
        </div>

        <div class="card-body">
            <div class="row">

                <div class="col-md-6 mb-3">
                    <label>Agency Name</label>
                    <asp:TextBox ID="txtAgency" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6 mb-3">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <div class="form-group">
                        <label for="agency-types">Agency Type</label>

                        <asp:DropDownList
                            runat="server"
                            ID="ddlagency_type"
                            CssClass="form-control">

                            <asp:ListItem Value="">Select Agency Types</asp:ListItem>
                            <asp:ListItem Value="BSEB DATACENTER">BSEB DATACENTER</asp:ListItem>
                            <asp:ListItem Value="Scanning Agencies">Scanning Agencies</asp:ListItem>
                            <asp:ListItem Value="Result Processing">Result Processing</asp:ListItem>
                            <asp:ListItem Value="Marks Entry">Marks Entry</asp:ListItem>
                            <asp:ListItem Value="Printing">Printing</asp:ListItem>

                        </asp:DropDownList>

                        <div class="invalid-feedback">
                            Please select Agency Types
                        </div>
                    </div>
                </div>

                <div class="col-md-6 mb-3">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6 mb-3">
                    <label>Mobile No</label>
                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" />
                </div>

            </div>

            <div class="form-group text-center">
                <asp:Button ID="btnSave" runat="server" Text=" Save Agency"
                    CssClass="btn btn-primary btn-lg"
                    OnClientClick="return validateForm();"
                    OnClick="btnSave_Click" />
            </div>

        </div>
    </div>

    <div class="row">
        <div class="col-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>Agency Details </h4>
                </div>

                <div class="card-body">
                    <div class="row">


                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="agency-name">Agency Status</label>
                                <asp:DropDownList runat="server" ID="ddl_Agencytatus" CssClass="form-control" Required="true">
                                    <asp:ListItem Value="ALL" Text="Select Agency status" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="DeActive" Text="DeActive"></asp:ListItem>

                                </asp:DropDownList>
                                <div class="invalid-feedback">Please select Status</div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="card-footer">
                    <asp:Button runat="server" ID="btn_search" Text="Search" CssClass="btn btn-primary btn-lg " OnClick="btn_search_Click" />

                </div>

            </div>
        </div>

        <div class="col-12" runat="server" id="User_detailes" visible="false">
            <div class="card card-primary">
                <div class="card-header">

                    <h4>Agency Details</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr No</th>
                                    <th>Agency Name</th>
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

                                            <td><%#Eval("agencyname") %></td>
                                            <td><%#Eval("username") %></td>

                                            <td><%#Eval("email") %></td>
                                            <td><%#Eval("mobileno") %></td>
                                            <td>
                                                <asp:Label ID="lbl_userstatus" runat="server" Text='<%#Eval("status") %>'></asp:Label>
                                            </td>
                                            <td>

                                                <asp:LinkButton
                                                    ID="link_edit"
                                                    CommandName="EditUser"
                                                    CommandArgument='<%#Eval("id") %>'
                                                    runat="server"
                                                    CssClass="btn-icon btn-primary btn-sm"
                                                    ToolTip="Edit User">
                                              <i class="fas fa-edit"></i>
                                            </asp:LinkButton>

                                                &nbsp;

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
