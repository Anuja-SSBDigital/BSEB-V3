<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Documenttypemaster1.aspx.cs" Inherits="Agency_Documenttypemaster1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row" runat="server" id="div_search">
        <div class="col-12">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>Document Type Master</h4>
                </div>

                <div class="card-body">
                    <div class="row mb-3">

                        <div class="col-md-4">
                            <label>Document Category</label>
                            <asp:DropDownList ID="ddlDocType" runat="server"
                                CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4">
                            <label>Document Type</label>
                            <asp:TextBox runat="server" ID="txtCategoryName"
                                CssClass="form-control"
                                placeholder="Document Type Name">
                            </asp:TextBox>
                        </div>

                        <div class="col-md-2 mt-4">
                            <asp:Button
                                runat="server"
                                ID="btnAddCategory"
                                CssClass="btn btn-primary w-100"
                                Text="Submit"
                                OnClientClick="return addDocumentType();" />
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
            <div class="card card-primary">

                <div class="card-header">
                    <h4>Document Type Master Data</h4>
                </div>

                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label>Select Status</label>

                            <div class="input-group">
                                <asp:DropDownList runat="server" ID="ddl_Status"
                                    CssClass="form-control">
                                    <asp:ListItem Value="" Text="All"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="Inactive"></asp:ListItem>
                                </asp:DropDownList>

                                <div class="input-group-append">
                                    <asp:Button
                                        runat="server"
                                        ID="btnsearch"
                                        CssClass="btn btn-primary"
                                        Text="Search"
                                        OnClientClick="return loadDocumentTypeData();" />
                                </div>
                            </div>
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblMessage" runat="server"
                                CssClass="text-info"></asp:Label>
                        </div>
                    </div>


                    <div class="table-responsive">
                        <table class="table table-bordered" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr.No.</th>
                                    <th>Document Type</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <script>
        function loadDocumentTypeData() {

            var status = $('#<%= ddl_Status.ClientID %>').val();

            $.ajax({
                type: "POST",
                url: "Documenttypemaster1.aspx/GetDocumentTypeData",
                data: JSON.stringify({ status: status }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    if (response.status === "success") {

                        bindTable(response.data);

                        $('#<%= lblMessage.ClientID %>').text("");
                    }
                    else {

                        $("#table-1 tbody").html("");

                        $('#<%= lblMessage.ClientID %>')
                            .text(response.message)
                            .removeClass()
                            .addClass("text-danger");
                    }
                },

                error: function () {
                    alert("Error loading data");
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
                html = "<tr><td colspan='4' style='text-align:center;'>No data found</td></tr>";
            }

            $.each(data, function (i, item) {

                html += "<tr>";
                html += "<td>" + (i + 1) + "</td>";
                html += "<td>" + item.SubDocName + "</td>";
                html += "<td>" + item.Status + "</td>";

                html += "<td>";

                if (item.Status === "Active") {

                    html += "<button type='button' class='btn btn-danger btn-sm' " +
                        "onclick=\"toggleStatus(" + item.Id + ")\">Deactivate</button>";

                } else {

                    html += "<button type='button' class='btn btn-success btn-sm' " +
                        "onclick=\"toggleStatus(" + item.Id + ")\">Activate</button>";
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

            if (!confirm("Are you sure?")) return;

            $.ajax({
                type: "POST",
                url: "Documenttypemaster1.aspx/ToggleStatus",
                data: JSON.stringify({ subdocId: id }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    alert(response.message);

                    loadDocumentTypeData();
                },

                error: function () {
                    alert("Error updating status");
                }
            });


            return false;
        }


        function addDocumentType() {

            var docTypeId = $('#<%= ddlDocType.ClientID %>').val();
            var subDocName = $('#<%= txtCategoryName.ClientID %>').val().trim();

            if (docTypeId === "") {
                alert("Please select document category.");
                return false;
            }

            if (subDocName === "") {
                alert("Please enter document type name.");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "Documenttypemaster1.aspx/AddDocumentType",
                data: JSON.stringify({
                    docTypeId: docTypeId,
                    subDocName: subDocName
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    alert(response.message);

                    if (response.status === "success") {

                        $('#<%= txtCategoryName.ClientID %>').val('');

                    }
                },

                error: function () {
                    alert("Something went wrong");
                }
            });

            return false;
        }

    </script>

</asp:Content>