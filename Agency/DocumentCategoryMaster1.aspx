<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master"
    AutoEventWireup="true" CodeFile="DocumentCategoryMaster1.aspx.cs"
    Inherits="Agency_DocumentCategoryMaster1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>

        function addCategory() {

            var categoryName = $('#<%= txtCategoryName.ClientID %>').val().trim();

            if (categoryName === "") {
                alert("Please enter a category name.");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "DocumentCategoryMaster1.aspx/AddCategory",
                data: JSON.stringify({ categoryName: categoryName }),
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


        function loadCategoryData() {

            var status = $('#<%= ddl_Status.ClientID %>').val();

            $.ajax({
                type: "POST",
                url: "DocumentCategoryMaster1.aspx/GetDocumentCategoryData",
                data: JSON.stringify({ status: status }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    if (response.status === "success") {

                        bindCategoryTable(response.data);
                        $('#<%= lblMessage.ClientID %>').text("");

                    } else {

                        $("#table-1 tbody").html("");
                        $('#<%= lblMessage.ClientID %>')
                            .text(response.message)
                            .addClass("text-danger");
                    }
                },

                error: function () {
                    alert("Error loading data");
                }
            });

            return false;
        }

        function bindCategoryTable(data) {


            if ($.fn.DataTable && $.fn.DataTable.isDataTable('#table-1')) {
                $('#table-1').DataTable().clear().destroy();
            }

            var html = "";

            if (data.length === 0) {
                html = "<tr><td colspan='4' class='text-center'>No data found</td></tr>";
            }

            $.each(data, function (i, item) {

                html += "<tr>";
                html += "<td>" + (i + 1) + "</td>";
                html += "<td>" + item.CategoryName + "</td>";
                html += "<td>" + item.Status + "</td>";

                html += "<td>";

                if (item.Status === "Active") {
                    html += "<button type='button' class='btn btn-danger btn-sm' " +
                        "onclick=\"toggleStatus(" + item.DocId + ")\">Deactivate</button>";
                } else {
                    html += "<button type='button' class='btn btn-success btn-sm' " +
                        "onclick=\"toggleStatus(" + item.DocId + ")\">Activate</button>";
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

        function toggleStatus(docId) {

            if (!confirm("Are you sure?")) return;

            $.ajax({
                type: "POST",
                url: "DocumentCategoryMaster1.aspx/ToggleStatus",
                data: JSON.stringify({ docId: docId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (res) {

                    var response = res.d;

                    alert(response.message);
                    loadCategoryData();
                },

                error: function () {
                    alert("Error updating status");
                }
            });
        }


        function editDoc(docId) {
            window.location.href = "Editdocumentcategorydetails.aspx?DocId=" + docId;
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="row" runat="server" id="div_search">
        <div class="col-12">
            <div class="card card-primary">

                <div class="card-header">
                    <h4>Category Master</h4>
                </div>

                <div class="card-body">
                    <div class="row">

                        <div class="col-md-4" runat="server" id="Div_admin">
                            <label>Document Category</label>
                            <asp:TextBox runat="server" ID="txtCategoryName"
                                CssClass="form-control"
                                placeholder="Document Category Name" />
                        </div>

                        <div class="col-md-2">
                            <asp:Button runat="server" ID="btnAddCategory"
                                CssClass="btn btn-primary mt-4"
                                Text="Submit"
                                OnClientClick="return addCategory();" />
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
                    <h4>Document Category Master Data</h4>
                </div>

                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label>Select Status</label>

                            <div class="input-group">
                                <asp:DropDownList runat="server" ID="ddl_Status"
                                    CssClass="form-control">
                                    <asp:ListItem Value="" Text="All"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="Inactive"></asp:ListItem>
                                </asp:DropDownList>

                                <div class="input-group-append">
                                    <asp:Button runat="server" ID="btnsearch"
                                        CssClass="btn btn-primary"
                                        Text="Search"
                                        OnClientClick="return loadCategoryData();" />
                                </div>
                            </div>
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblMessage" runat="server"
                                CssClass="text-info"></asp:Label>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-striped" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr.No.</th>
                                    <th>Document Category Name</th>
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
</asp:Content>
