<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master"
    AutoEventWireup="true" CodeFile="Ip1.aspx.cs" Inherits="Agency_Ip1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>

        function handleCheckboxChange(checkbox, type) {

            var value = checkbox.checked ? "true" : "false";

            if (type === "ProcessCSV") {
                $('#<%= hdnProcessCSV.ClientID %>').val(value);
            }
            else if (type === "FileUpload") {
                $('#<%= hdnFileUpload.ClientID %>').val(value);
            }
        }

        function addIP() {

            var ipNumber = $('#<%= txtIPNumber.ClientID %>').val().trim();
            var agencyName = $('#<%= txtAgencyName.ClientID %>').val().trim();
            var processCSV = $('#<%= hdnProcessCSV.ClientID %>').val();
            var fileUpload = $('#<%= hdnFileUpload.ClientID %>').val();

            if (ipNumber === "") {
                alert("Please enter an IP address.");
                return false;
            }

            if (processCSV !== "true" && fileUpload !== "true") {
                alert("Please select at least one access type.");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "Ip1.aspx/AddIP",
                data: JSON.stringify({
                    ipNumber: ipNumber,
                    agencyName: agencyName,
                    canProcessCSV: processCSV === "true",
                    canFileUpload: fileUpload === "true"
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    alert(response.message);

                    if (response.status === "success") {
                        $('#<%= txtIPNumber.ClientID %>').val('');
                        $('#<%= txtAgencyName.ClientID %>').val('');
                        $('#<%= hdnProcessCSV.ClientID %>').val('false');
                        $('#<%= hdnFileUpload.ClientID %>').val('false');


                    }
                },

                error: function () {
                    alert("Error occurred");
                }
            });

            return false;
        }

        function loadIPData() {

            var status = $('#<%= ddl_Status.ClientID %>').val();

            $.ajax({
                type: "POST",
                url: "Ip1.aspx/GetIPData",
                data: JSON.stringify({ status: status }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    if (response.status === "success") {
                        bindTable(response.data);
                    } else {
                        $("#table-1 tbody").html("");
                    }
                }
            });

            return false;
        }


        function bindTable(data) {


            if ($.fn.DataTable && $.fn.DataTable.isDataTable('#table-1')) {
                $('#table-1').DataTable().clear().destroy();
            }

            var html = "";

            if (data.length === 0) {
                html = "<tr><td colspan='6' style='text-align:center;'>No records found</td></tr>";
            }

            $.each(data, function (i, item) {

                html += "<tr>";
                html += "<td>" + (i + 1) + "</td>";
                html += "<td>" + item.IPNumber + "</td>";
                html += "<td>" + (item.AgencyName || "") + "</td>";

                html += "<td>";
                if (item.CanUpload) html += "File Upload<br/>";
                if (item.CanProcessCSV) html += "File Download";
                html += "</td>";

                html += "<td>" + item.Status + "</td>";

                html += "<td>";

                html += "<button type='button' class='btn btn-primary btn-sm mr-1' onclick='editIP(" + item.Id + ")'>Edit</button>";

                if (item.IsActive) {
                    html += "<button type='button' class='btn btn-danger btn-sm' onclick='toggleStatus(" + item.Id + ")'>Deactivate</button>";
                } else {
                    html += "<button type='button' class='btn btn-success btn-sm' onclick='toggleStatus(" + item.Id + ")'>Activate</button>";
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

        function toggleStatus(id) {

            event.preventDefault();

            if (!confirm("Are you sure?")) return false;

            $.ajax({
                type: "POST",
                url: "Ip1.aspx/ToggleIPStatus",
                data: JSON.stringify({ ipId: id }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    if (response.status === "success") {
                        alert(response.message);
                        loadIPData();
                    } else {
                        alert(response.message);
                    }
                },

                error: function () {
                    alert("Error updating status");
                }
            });

            return false;
        }

        function editIP(id) {
            window.location.href = "EditIp1.aspx?IPID=" + id;
        }

    </script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="row" runat="server" id="div_search">
        <div class="col-12">
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

                            <asp:HiddenField ID="hdnProcessCSV" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnFileUpload" runat="server" Value="false" />

                            <div>
                                <input type="checkbox" onclick="handleCheckboxChange(this,'ProcessCSV')" />
                                File Download
                            </div>
                            <div>
                                <input type="checkbox" onclick="handleCheckboxChange(this,'FileUpload')" />
                                File Upload
                            </div>
                        </div>

                        <div class="col-md-2 mt-3">
                            <asp:Button ID="btnAddIP" runat="server"
                                CssClass="btn btn-primary"
                                Text="Submit"
                                OnClientClick="return addIP();" />
                        </div>


                        <div class="col-12 mt-2">
                            <asp:Label ID="lblAddMessage" runat="server" CssClass="text-success"></asp:Label>
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

                                    <asp:DropDownList ID="ddl_Status" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="" Text="All" />
                                        <asp:ListItem Value="1" Text="Active" />
                                        <asp:ListItem Value="0" Text="Inactive" />
                                    </asp:DropDownList>

                                    <br />

                                    <asp:Button ID="btnSearch" runat="server"
                                        CssClass="btn btn-primary"
                                        Text="Search"
                                        OnClientClick="return loadIPData();" />

                                    <div class="col-12 mt-2">
                                        <asp:Label ID="lblMessage" runat="server" CssClass="text-info d-block mb-2"></asp:Label>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="table-responsive mt-3">
                            <table class="table table-striped" id="table-1">
                                <thead>
                                    <tr>
                                        <th>Sr.No.</th>
                                        <th>IP</th>
                                        <th>Agency</th>
                                        <th>Access</th>
                                        <th>Status</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
</asp:Content>
