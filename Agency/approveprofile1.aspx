<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="approveprofile1.aspx.cs" Inherits="Agency_approveprofile1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script>

        function loadUserData() {

            var agencyId = $('#<%= ddlOwnerAgency.ClientID %>').val();
            var userStatus = $('#<%= ddl_Userstatus.ClientID %>').val();

            if (agencyId == "") {
                swal("Required", "Please select Agency", "warning");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "approveprofile1.aspx/GetUserData",
                data: JSON.stringify({
                    agencyId: agencyId,
                    userStatus: userStatus
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    if (response.status === "success") {

                        bindRepeater(response.data);

                        $('#<%= User_detailes.ClientID %>').show();

                } else {
                    swal("Error", response.message, "error");
                }
            },

            error: function (err) {
                console.log(err.responseText);
                swal("Error", "Something went wrong", "error");
            }
        });

            return false;
        }

        function bindRepeater(data) {


            if ($.fn.DataTable && $.fn.DataTable.isDataTable('#table-1')) {
                $('#table-1').DataTable().clear().destroy();
            }

            var html = "";

            if (data.length === 0) {
                html = "<tr><td colspan='6' style='text-align:center;'>No data found</td></tr>";
            }

            $.each(data, function (i, item) {

                html += "<tr>";
                html += "<td>" + (i + 1) + "</td>";
                html += "<td>" + item.username + "</td>";
                html += "<td>" + item.email + "</td>";
                html += "<td>" + item.mobileno + "</td>";
                html += "<td>" + item.status + "</td>";

                html += "<td>";

                if (item.status === "Pending For Approval") {
                    html += "<button type='button' class='btn btn-success btn-sm' onclick=\"approveUser('" + item.id + "')\">Approve</button> ";
                    html += "<button type='button' class='btn btn-danger btn-sm' onclick=\"rejectUser('" + item.id + "')\">Reject</button>";
                }
                else if (item.status === "Active") {
                    html += "<button type='button' class='btn btn-danger btn-sm' onclick=\"deactiveUser('" + item.id + "')\">Deactivate</button>";
                }
                else {
                    html += "<button type='button' class='btn btn-success btn-sm' onclick=\"activeUser('" + item.id + "')\">Activate</button>";
                }

                html += "</td>";
                html += "</tr>";
            });

            $("#table-1 tbody").html(html);


            setTimeout(function () {
                $('#table-1').DataTable({
                    paging: true,
                    searching: true,
                    ordering: true,
                    autoWidth: false,
                    destroy: true
                });
            }, 100);
        }


        function approveUser(id) {

            if (!confirm("Are you sure to approve?")) return;

            $.ajax({
                type: "POST",
                url: "approveprofile1.aspx/ApproveUser",
                data: JSON.stringify({ userId: id }),
                contentType: "application/json",
                success: function () {
                    loadUserData();
                }
            });
            return false;

        }

        function rejectUser(id) {

            if (!confirm("Are you sure to reject?")) return;

            $.ajax({
                type: "POST",
                url: "approveprofile1.aspx/RejectUser",
                data: JSON.stringify({ userId: id }),
                contentType: "application/json",
                success: function () {
                    loadUserData();
                }
            });
            return false;

        }

        function activeUser(id) {

            $.ajax({
                type: "POST",
                url: "approveprofile1.aspx/ActiveUser",
                data: JSON.stringify({ userId: id }),
                contentType: "application/json",
                success: function () {
                    loadUserData();
                }
            });
            return false;

        }

        function deactiveUser(id) {

            $.ajax({
                type: "POST",
                url: "approveprofile1.aspx/DeactiveUser",
                data: JSON.stringify({ userId: id }),
                contentType: "application/json",
                success: function () {
                    loadUserData();
                }
            });
            return false;

        }

</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="row">

        <div class="col-12">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>User Details</h4>
                </div>

                <div class="card-body">

                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="agency-name">Agency Name</label>

                                <asp:DropDownList
                                    runat="server"
                                    ID="ddlOwnerAgency"
                                    CssClass="form-control"
                                    AppendDataBoundItems="true">
                                    <asp:ListItem Value="" Text="Select Agency Name"></asp:ListItem>
                                </asp:DropDownList>

                                <div class="invalid-feedback">Please select Agency Name</div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <label>User Status</label>
                            <asp:DropDownList runat="server" ID="ddl_Userstatus" CssClass="form-control">
                                <asp:ListItem Value="ALL">Select User status</asp:ListItem>
                                <asp:ListItem Value="Active">Active</asp:ListItem>
                                <asp:ListItem Value="DeActive">DeActive</asp:ListItem>
                                <asp:ListItem Value="Pending For Approval">Pending For Approval</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                    </div>

                </div>

                <div class="card-footer">
                    <asp:Button runat="server" ID="btn_submit"
                        Text="Submit"
                        CssClass="btn btn-primary btn-lg"
                        OnClientClick="return loadUserData();" />
                </div>

            </div>
        </div>
        <div class="col-12" runat="server" id="User_detailes" style="display: none;">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>User Details</h4>
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
                            </tbody>

                        </table>

                    </div>
                </div>

            </div>
        </div>

    </div>

</asp:Content>
