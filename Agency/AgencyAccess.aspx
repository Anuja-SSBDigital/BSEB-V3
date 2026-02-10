<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="AgencyAccess.aspx.cs" Inherits="AgencyAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .chk-list input[disabled] {
            display: none;
        }

        .chk-list label {      
            padding-left: 8px;
        }

        .chk-list input[disabled] + label {
            font-weight: bold;
            color: #000;
            background-color: #f0f0f0;
            display: block;
            padding: 5px 8px;
            margin-top: 8px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>Smart Contract–Enabled File Access</h4>
                </div>
                <div class="card-body">

                    <!-- STEP 1: Owner Agency -->
                    <div class="mb-4">
                        <label class="text-dark">This is the agency that originally uploaded the document.</label>
                        <asp:DropDownList ID="ddlOwnerAgency" runat="server" CssClass="form-control">
                            <asp:ListItem Value="ALL">Select Agency</asp:ListItem>
                            <asp:ListItem Value="Antier">Antier</asp:ListItem>
                            <asp:ListItem Value="Charu Mindworks">Charu Mindworks</asp:ListItem>
                            <asp:ListItem Value="Datacon">Datacon</asp:ListItem>
                            <asp:ListItem Value="Hitech">Hitech</asp:ListItem>
                            <asp:ListItem Value="Kids">Kids</asp:ListItem>
                            <asp:ListItem Value="Mapple">Mapple</asp:ListItem>
                            <asp:ListItem Value="MCRK">MCRK</asp:ListItem>
                            <asp:ListItem Value="Shree Jagannath Udyog">Shree Jagannath Udyog</asp:ListItem>
                            <asp:ListItem Value="SSB Digital">SSB Digital</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <!-- STEP 2: Document Category -->
                    <div class="mb-3">
                        <label class="text-dark">Doc Category</label>
                        <asp:DropDownList ID="ddl_doctype" runat="server" CssClass="form-control"></asp:DropDownList>

                        <asp:HiddenField ID="hfSubdoctypeIds" runat="server" />
                        <asp:HiddenField ID="hfSubdoctypeNames" runat="server" />
                    </div>

                    <label class="text-dark">Select Category and Document you want to show.</label>
                    <div class="border p-2 rounded">

                        <div class="row px-2" id="subDocContainer">
                            <span class="text-muted">Please select category</span>

                        </div>

                    </div>

                    <label class="text-dark">Tick the agencies that should be able to see the selected document type uploaded by the chosen Owner Agency.</label>
                    <div class="mb-4">
                        <div class="border p-2 rounded">
                            <asp:CheckBoxList ID="chkViewerAgencies" runat="server"
                                RepeatDirection="Vertical" CssClass="chk-list" RepeatLayout="Flow" />
                        </div>
                    </div>

                    <div class="form-group text-center">
                        <asp:Button ID="btnSave" runat="server" Text="Save Access"
                            CssClass="btn btn-primary btn-lg"
                            OnClientClick="return validateSelection();"
                            OnClick="btnSave_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>


    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            var container = $('#subDocContainer');
                    
         
            $('#<%= ddl_doctype.ClientID %>').on('change', function () {
                var doctypeId = $(this).val();
                container.html('');

                if (doctypeId == "0") {
                    container.html('<span class="text-muted">Please select category</span>');
                    return;
                }
                 
                $.ajax({
                    type: "POST",
                    url: "AgencyAccess.aspx/GetSubDocTypes",
                    data: JSON.stringify({ doctypeId: doctypeId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var data = response.d;
                        if (data.length === 0) {
                            container.html('<span class="text-muted">No Sub Document Types found</span>');
                            return;
                        }
                        var html =
                            '<div class="w-100 text-dark fw-bold p-2" ' +

                            '<label class="fw-bold d-block p-2" ' +

                            'style="background-color:#f0f0f0; border-radius:5px;">' +
                            '<strong>Select Category</strong>' +
                            '</label>' +
                            '</div>' +
                            '<div class="row px-2">';

                        $.each(data, function (i, item) {

                            html += '<div class="form-check" style="display:inline-block;width:230px;margin:15px;">';
                            html += '<input type="checkbox" class="form-check-input subdoc-checkbox me-2" ' +
                                'id="chk_' + item.subdocId + '" value="' + item.subdocId + '" data-name="' + item.subdoctypename + '">';
                            html += '<label class="form-check-label text-dark" for="chk_' + item.subdocId + '">' + item.subdoctypename + '</label>';
                            html += '</div>';
                        });


                        html += '</div>';

                        container.html(html);


                        $('.subdoc-checkbox').on('change', function () {
                            var ids = [];
                            var names = [];
                            $('.subdoc-checkbox:checked').each(function () {
                                ids.push($(this).val());
                                names.push($(this).data('name'));
                            });
                            $('#<%= hfSubdoctypeIds.ClientID %>').val(ids.join(','));
                            $('#<%= hfSubdoctypeNames.ClientID %>').val(names.join(','));
                        });
                    },
                    error: function () {
                        alert("Error loading sub-document types.");
                    }
                });
            });

        });


        function validateSelection() {

            var viewerList = document.getElementById('<%=chkViewerAgencies.ClientID%>');
            if (viewerList) {
                var inputs = viewerList.querySelectorAll('input[type="checkbox"]:not([disabled])');
                var anyViewer = Array.from(inputs).some(chk => chk.checked);
                if (!anyViewer) {
                    alert("Please select at least one Viewer Agency.");
                    return false;
                }
            }


            var anySubDoc = $('.subdoc-checkbox:checked').length > 0;
            if (!anySubDoc) {
                alert("Please select at least one Sub Document Type.");
                return false;
            }

            return true;
        }
    </script>

</asp:Content>
