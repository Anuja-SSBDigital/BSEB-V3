<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Accessfilelist.aspx.cs"Inherits="Accessfilelist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

     <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
  
    #table-1 td .btn {
        margin-right: 5px;
        margin-bottom: 3px;
    }
</style>
    <script>
        function validateSearch() {

            var OwnerAgency = document.getElementById('<%= ddlOwnerAgency.ClientID %>').value;

            if (OwnerAgency === "") {

                Swal.fire({
                    icon: 'warning',
                    title: 'Validation Error',
                    text: 'Please select Agency Name'
                });

                return false; // stop postback
            }

            return true; // allow postback
        }
    </script>
 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div class="row" runat="server" id="div_search">
        <div class="col-md-12">
            <div class="card card-primary">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-6" runat="server" id="Div_admin">

                            <h5>Select Agency</h5>

                            <div class="form-group">
                                <label>Agency Name</label>

                                <asp:DropDownList ID="ddlOwnerAgency"  runat="server"  CssClass="form-control" AppendDataBoundItems="true">
                                    <asp:ListItem Value="">Select Agency Name</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                        </div>
                    </div>

                </div>

                <div class="card-footer text-end">
                     <asp:Button  ID="btn_Search_Click"  runat="server" Text="Search"  CssClass="btn btn-primary" OnClick="btnsearch_Click" OnClientClick="return validateSearch();" />

                    <asp:Label
                        ID="lblMessage" runat="server" CssClass="text-danger d-block mt-2"></asp:Label>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="mt-3">
            <%--<div class="mt-3" runat="server" id="Agency_detailes" visible="false">--%>

                <div class="card card-primary">
                    <div class="card-header">
                        <h4>File Upload Details</h4>
                    </div>

                    <div class="card-body">
                        <div class="table-responsive">

                            <asp:Repeater ID="rpt_Agencywisedata" runat="server" OnItemCommand="rpt_Agencywisedata_ItemCommand" OnItemDataBound="rpt_Agencywisedata_ItemDataBound">

                                <HeaderTemplate>

                                    <table class="table table-striped" id="table-1">
                                        <thead>
                                            <tr>
                                                <th>Sr.No.</th>
                                                <th>Actual File Name</th>
                                                <th>Doc Type</th>
                                                <th>File Name</th>
                                                <th>Remarks</th>
                                                <th>Upload Date</th>
                                                <th>Access Agency</th>
                                               

                                                <th>Agency Drop</th>

                                                  <th>Disable Agency</th>

                                                <th>Action</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <td><%# Container.ItemIndex + 1 %></td>

                                        <td><%# Eval("actualfilename") %></td>

                                        <td><%# Eval("subdoctype") %></td>

                                        <td><%# Eval("filename") %></td>

                                        <td><%# Eval("Remarks") %></td>

                                        <td>
                                            <%# Eval("createddate", "{0:dd-MMM-yyyy hh:mm tt}") %>
                                        </td>

                                        <td><%# Eval("ViewerAgencies") %></td>

                                        <td>
  <asp:DropDownList
                                                ID="ddlRowAgency"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                AutoPostBack="false">
                                            </asp:DropDownList>     
                                        </td>


                                          <td>
                                        <%# GetHiddenAgencies(Eval("id")) %>
                                    </td>


                                        <td>
<asp:Button
                                                ID="btnToggle"
                                                runat="server"
                                                Text="Hide File"
                                                CssClass="btn btn-warning btn-sm"
                                                CommandName="ToggleStatus"
                                                CommandArgument='<%# Eval("id") %>'
                                                OnClientClick="return confirm('Are you sure you want to Hiide file this agency?');" />

                                            <asp:Button
                                                ID="btnToggle1"
                                                runat="server"
                                                Text="Show File"
                                                CssClass="btn btn-warning btn-sm"
                                                CommandName="ToggleStatus"
                                                CommandArgument='<%# Eval("id") %>'
                                                OnClientClick="return confirm('Are you sure you want to Show file this agency?');" />                                            </td>


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
    </div>

</asp:Content>