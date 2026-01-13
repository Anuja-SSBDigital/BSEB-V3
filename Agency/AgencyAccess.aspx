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
                    <!-- STEP 1 -->
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


                    <label class="text-dark">Select Category and Document you want to show.</label>
                    <div class="border p-2 rounded">
                        <div class="mb-4 text-dark">
                            <label class="fw-bold d-block mb-2 p-2" style="background-color: #f0f0f0; border-radius: 5px;">
                                <strong>Select Category</strong>
                            </label>

                            <asp:Repeater ID="rptDocumentTypes" runat="server">
                                <ItemTemplate>
                                    <div class="form-check" style="display: inline-block; width: 230px; margin: 15px;">
                                        <asp:CheckBox ID="chkDoc" runat="server" CssClass="form-check-input me-2" />
                                        <asp:Label ID="lblDocName" runat="server" AssociatedControlID="chkDoc"
                                            Text='<%# Eval("DocTypeName") %>' CssClass="form-check-label text-dark"></asp:Label>
                                        <asp:HiddenField ID="hdnDocTypeName" runat="server" Value='<%# Eval("DocTypeName") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                    <label class="text-dark">Tick the agencies that should be able to see the selected document type uploaded by the chosen Owner Agency.</label>

                    <!-- STEP 3 -->
                    <div class="mb-4">
                        <div class="border p-2 rounded">
                            <asp:CheckBoxList ID="chkViewerAgencies" runat="server"
                                RepeatDirection="Vertical" CssClass="chk-list" RepeatLayout="Flow" />
                        </div>
                    </div>

                    <!-- SAVE BUTTON -->
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

    <script type="text/javascript">
        function validateSelection() {


            // --- VIEWER AGENCY CHECK ---
            var viewerList = document.getElementById('<%=chkViewerAgencies.ClientID%>');
            if (viewerList) {
                var inputs = viewerList.querySelectorAll('input[type="checkbox"]:not([disabled])');
                var anyViewer = Array.from(inputs).some(chk => chk.checked);
                if (!anyViewer) {
                    alert("Please select at least one Viewer Agency.");
                    return false;
                }
            }

            return true;
        }
    </script>
</asp:Content>
