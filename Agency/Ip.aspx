<%@ Page Title="IP Master" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true"
    CodeFile="Ip.aspx.cs" Inherits="Agency_Ip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function handleCheckboxChange(checkbox, type) {
            var value = checkbox.checked ? "true" : "false";

            switch (type) {
                case "ProcessCSV":
                    document.getElementById('<%= hdnProcessCSV.ClientID %>').value = value;
                    break;
                case "FileUpload":
                    document.getElementById('<%= hdnFileUpload.ClientID %>').value = value;
                    break;

            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="row" runat="server" id="div_search">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>IP Master</h4>
                </div>

                <div class="card-body">
                    <div class="row">


                        <div class="col-lg-4 col-md-6 col-sm-6 col-12 px-2">
                            <h5 class="font-15">IP Address</h5>
                            <asp:TextBox runat="server" ID="txtIPNumber" CssClass="form-control"
                                Placeholder="Enter IP Address">
                            </asp:TextBox>
                        </div>


                        <div class="col-lg-4 col-md-6 col-sm-6 col-12 px-2">
                            <h5 class="font-15">Agency Name</h5>
                            <asp:TextBox runat="server" ID="txtAgencyName" CssClass="form-control"
                                Placeholder="Agency Name (optional)">
                            </asp:TextBox>
                        </div>


                        <div class="col-lg-4 col-md-12 col-sm-12 col-12 px-2">
                            <h5 class="font-15">Access Type</h5>


                            <asp:HiddenField ID="hdnProcessCSV" runat="server" />
                            <asp:HiddenField ID="hdnFileUpload" runat="server" />


                            <div class="form-check">
                                <input type="checkbox" id="chkProcessCSV" class="form-check-input"
                                    onclick="handleCheckboxChange(this, 'ProcessCSV')" />
                                <label class="form-check-label" for="chkProcessCSV">File Download</label>
                            </div>

                            <div class="form-check">
                                <input type="checkbox" id="chkFileUpload" class="form-check-input"
                                    onclick="handleCheckboxChange(this, 'FileUpload')" />
                                <label class="form-check-label" for="chkFileUpload">File Upload</label>
                            </div>


                        </div>

                        <div class="col-lg-2 col-md-2 col-sm-12 px-2">
                            <asp:Button runat="server" ID="btnAddIP" OnClick="btnAddIP_Click"
                                CssClass="btn btn-primary mt-4" Text="Submit" />
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblAddMessage" runat="server"
                                CssClass="text-success d-block mb-2"></asp:Label>
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
                    <h4>IP Master Data</h4>
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
                                        <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click"
                                            CssClass="btn btn-primary " Text="Search" />
                                    </div>
                                </div>
                            </div>
                        </div>



                        <div class="col-12 mt-2">
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-info d-block mb-2"></asp:Label>
                        </div>
                    </div>


                    <asp:Repeater runat="server" ID="rpt_IPData" OnItemCommand="rpt_IPData_ItemCommand">
                        <HeaderTemplate>
                            <div class="table-responsive">
                                <table class="table table-striped" id="table-1">
                                    <thead>
                                        <tr>
                                            <th>Sr.No.</th>
                                            <th>IP Address</th>

                                            <th>Agency Name</th>

                                            <th>Access Type </th>

                                            <th>Status</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>

                                <td><%# Eval("IPNumber") %></td>

                                <td>
                                    <%# string.IsNullOrEmpty(Eval("AgencyName").ToString()) 
                ? " " 
                : Eval("AgencyName") %>
                                </td>

                                <td>
                                    <%#
        (Convert.ToBoolean(Eval("CanUpload")) ? "File Upload<br/>" : "") +
        (Convert.ToBoolean(Eval("CanProcessCSV")) ? "File Download" : "")
                                    %>
                                </td>

                                <td>
                                    <span class='<%# Convert.ToBoolean(Eval("IsActive")) %>'>
                                        <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                    </span>
                                </td>

                                <td>
                                    <asp:Button runat="server" ID="btnEdit" Text="Edit"
                                        CommandName="EditIP"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-primary btn-sm" />

                                    <asp:Button runat="server" ID="btnDeactivate" Text="Deactivate"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-danger btn-sm"
                                        OnClientClick="return confirm('Are you sure you want to deactivate this IP?');"
                                        Visible='<%# Convert.ToBoolean(Eval("IsActive")) %>' />

                                    <asp:Button runat="server" ID="btnActivate" Text="Activate"
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-success btn-sm"
                                        OnClientClick="return confirm('Are you sure you want to activate this IP?');"
                                        Visible='<%# !Convert.ToBoolean(Eval("IsActive")) %>' />
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