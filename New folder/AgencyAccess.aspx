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
                    <!-- STEP 2 -->
                    <div class="border p-2 rounded">
                        <!-- Category -->
                        <div class="mb-4 text-dark">
                            <label class="fw-bold d-block mb-2 p-2" style="background-color: #f0f0f0; border-radius: 5px;">
                                <strong>Select Category</strong>
                            </label>
                            <div class="d-flex flex-wrap gap-3">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkFinal" runat="server" />
                                    <label class="form-check-label" for="chkFinal">Final</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="chkWanting" runat="server" />
                                    <label class="form-check-label" for="chkWanting">Wanting</label>
                                </div>
                                <div class="form-check">
                                    <asp:CheckBox ID="chkProcess" runat="server" />
                                    <label class="form-check-label" for="chkProcess">Process</label>
                                </div>
                            </div>
                        </div>

                        <!-- Document Options -->
                        <div class="mb-4 text-dark">
                            <label class="fw-bold d-block mb-2 p-2" style="background-color: #f0f0f0; border-radius: 5px;">
                                <strong>Document Options </strong>
                            </label>
                            <div class="row g-3">
                                <!-- Process Documents -->
                                <div class="col-md-3 col-sm-6">
                                    <div class="form-check"><asp:CheckBox ID="chkAttendanceA" runat="server" /><label for="chkAttendanceA">Attendance A</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkAttendanceB" runat="server" /><label for="chkAttendanceB">Attendance B</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkOnlineData" runat="server" /><label for="chkOnlineData">Online Data</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkAwardSheet" runat="server" /><label for="chkAwardSheet">Award Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkLitho" runat="server" /><label for="chkLitho">Litho Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkFoilSheet" runat="server" /><label for="chkFoilSheet">Foil Sheet</label></div>
                                </div>
                                <!-- Wanted Documents -->
                                <div class="col-md-3 col-sm-6">
                                    <div class="form-check"><asp:CheckBox ID="chkOMRSheet" runat="server" /><label for="chkOMRSheet">OMR Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkFlying" runat="server" /><label for="chkFlying">Flying Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkAbsentee" runat="server" /><label for="chkAbsentee">Absentee Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkResultData" runat="server" /><label for="chkResultData">Result Data</label></div>
                                </div>
                            </div>
                        </div>

                        <!-- Other Documents -->
                        <div class="mb-4 text-dark">
                            <label class="fw-bold d-block mb-2 p-2" style="background-color: #f0f0f0; border-radius: 5px;">
                                <strong>Other Documents </strong>
                            </label>
                            <div class="row g-3">
                                <div class="col-md-4 col-sm-6">
                                    <div class="form-check"><asp:CheckBox ID="chkFlyingSheet" runat="server" /><label for="chkFlyingSheet">Final Data</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkAdmitCardMaster" runat="server" /><label for="chkAdmitCardMaster">Admit Card Master</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkBottomBarcode" runat="server" /><label for="chkBottomBarcode">Bottom Barcode Master</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkCarryData" runat="server" /><label for="chkCarryData">3 Year Carry Data</label></div>
                                </div>
                                <div class="col-md-4 col-sm-6">
                                    <div class="form-check"><asp:CheckBox ID="chkPracticalDS" runat="server" /><label for="chkPracticalDS">Practical Data Structure</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkTheoryDS" runat="server" /><label for="chkTheoryDS">Theory Data Structure</label></div>
									   <div class="form-check"><asp:CheckBox ID="chkTheoryAS" runat="server" /><label for="chkTheoryAS">Theory Attendance Sheet</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkTopper" runat="server" /><label for="chkTopper">Topper</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkMissmatched" runat="server" /><label for="chkMissmatched">Missmatched</label></div>
                                </div>
                                <div class="col-md-4 col-sm-6">
                                    <div class="form-check"><asp:CheckBox ID="chkPracticalMarks" runat="server" /><label for="chkPracticalMarks">Practical Marks</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkSampleResultData" runat="server" /><label for="chkSampleResultData">Sample Result Data</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkScrutinyResultData" runat="server" /><label for="chkScrutinyResultData">Scrutiny Result Data</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkScrutinyPrintMaster" runat="server" /><label for="chkScrutinyPrintMaster">Scrutiny Print Master</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkScrutinyTheoryAward" runat="server" /><label for="chkScrutinyTheoryAward">Scrutiny Theory Award</label></div>
                                     <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_OMR" runat="server" /><label for="chkBBOSE_THEORY_OMR">BBOSE_THEORY_OMR</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_FLY" runat="server" /><label for="chkBBOSE_THEORY_FLY">BBOSE_THEORY_FLY</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_ATTENDANCE" runat="server" /><label for="chkBBOSE_THEORY_ATTENDANCE">BBOSE_THEORY_ATTENDANCE</label></div>
                                     <div class="form-check"><asp:CheckBox ID="chkBBOSE_PRACTICAL_ATTENDANCE" runat="server" /><label for="chkBBOSE_PRACTICAL_ATTENDANCE">BBOSE_PRACTICAL_ATTENDANCE</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_PRACTICAL_FOIL" runat="server" /><label for="chkBBOSE_PRACTICAL_FOIL">BBOSE_PRACTICAL_FOIL</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_EXPULSION" runat="server" /><label for="chkBBOSE_THEORY_EXPULSION">BBOSE_THEORY_EXPULSION</label></div>
                                     <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_AWARD" runat="server" /><label for="chkBBOSE_THEORY_AWARD">BBOSE_THEORY_AWARD</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_FOIL" runat="server" /><label for="chkBBOSE_THEORY_FOIL">BBOSE_THEORY_FOIL</label></div>
                                    <div class="form-check"><asp:CheckBox ID="chkBBOSE_THEORY_ONLINE_MARKS" runat="server" /><label for="chkBBOSE_THEORY_ONLINE_MARKS">BBOSE_THEORY_ONLINE_MARKS</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_PRACTICAL_AWARD" runat="server" /><label for="chkBBOSE_PRACTICAL_AWARD">BBOSE_PRACTICAL_AWARD</label></div>
                                     <div class="form-check"><asp:CheckBox ID="chkBBOSE_ADMITCARD_MASTER_DATA" runat="server" /><label for="chkBBOSE_ADMITCARD_MASTER_DATA">BBOSE_ADMITCARD_MASTER_DATA</label></div>
                                     <div class="form-check"><asp:CheckBox ID="chkBBOSE_OLDRESULT_DATA" runat="server" /><label for="chkBBOSE_OLDRESULT_DATA">BBOSE_OLDRESULT_DATA</label></div>
                                      <div class="form-check"><asp:CheckBox ID="chkBBOSE_COPY_PRINTING_DATA" runat="server" /><label for="chkBBOSE_COPY_PRINTING_DATA">BBOSE_COPY_PRINTING_DATA</label></div>
                                </div>
                            </div>
                        </div>
                    </div>
</br>
                    <!-- STEP 3 -->
                    <div class="mb-4">
                        <label class="text-dark">Tick the agencies that should be able to see the selected document type uploaded by the chosen Owner Agency.</label>
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
        var finalChecked = document.getElementById("<%=chkFinal.ClientID%>").checked;
          var wantingChecked = document.getElementById("<%=chkWanting.ClientID%>").checked;
          var processChecked = document.getElementById("<%=chkProcess.ClientID%>").checked;
          var categorySelected = finalChecked || wantingChecked || processChecked;

          // IDs for all documents
          var allDocIds = [
              "<%=chkAttendanceA.ClientID%>", "<%=chkAttendanceB.ClientID%>", "<%=chkOnlineData.ClientID%>", "<%=chkAwardSheet.ClientID%>",
              "<%=chkLitho.ClientID%>", "<%=chkFoilSheet.ClientID%>", "<%=chkOMRSheet.ClientID%>", "<%=chkFlying.ClientID%>",
            "<%=chkAbsentee.ClientID%>", "<%=chkResultData.ClientID%>", "<%=chkFlyingSheet.ClientID%>", "<%=chkAdmitCardMaster.ClientID%>",
            "<%=chkBottomBarcode.ClientID%>", "<%=chkCarryData.ClientID%>", "<%=chkPracticalDS.ClientID%>", "<%=chkTheoryDS.ClientID%>",
            "<%=chkTopper.ClientID%>", "<%=chkMissmatched.ClientID%>", "<%=chkPracticalMarks.ClientID%>", "<%=chkSampleResultData.ClientID%>",
            "<%=chkScrutinyResultData.ClientID%>", "<%=chkScrutinyPrintMaster.ClientID%>", "<%=chkScrutinyTheoryAward.ClientID%>"
        ];

          // IDs for "Other Documents"
          var otherDocsIds = [
            "<%=chkFlyingSheet.ClientID%>", "<%=chkAdmitCardMaster.ClientID%>", "<%=chkBottomBarcode.ClientID%>", "<%=chkCarryData.ClientID%>",
            "<%=chkPracticalDS.ClientID%>", "<%=chkTheoryDS.ClientID%>", "<%=chkTopper.ClientID%>", "<%=chkMissmatched.ClientID%>",
            "<%=chkPracticalMarks.ClientID%>", "<%=chkSampleResultData.ClientID%>", "<%=chkScrutinyResultData.ClientID%>",
            "<%=chkScrutinyPrintMaster.ClientID%>", "<%=chkScrutinyTheoryAward.ClientID%>"
          ];

          var anyDocSelected = allDocIds.some(id => document.getElementById(id).checked);
          if (!anyDocSelected) {
              //alert("Please select at least one document.");
              //return false;
          }

          // Check for documents from "Document Options" section
          var docOptionsIds = [
              "<%=chkAttendanceA.ClientID%>", "<%=chkAttendanceB.ClientID%>", "<%=chkOnlineData.ClientID%>", "<%=chkAwardSheet.ClientID%>",
              "<%=chkLitho.ClientID%>", "<%=chkFoilSheet.ClientID%>", "<%=chkOMRSheet.ClientID%>", "<%=chkFlying.ClientID%>",
              "<%=chkAbsentee.ClientID%>", "<%=chkResultData.ClientID%>"
          ];
          var exemptDocsSelected = document.getElementById("<%=chkOMRSheet.ClientID%>").checked ||
              document.getElementById("<%=chkFlying.ClientID%>").checked ||
              document.getElementById("<%=chkAwardSheet.ClientID%>").checked;
          var docOptionsSelected = docOptionsIds.some(id => document.getElementById(id).checked);

        // This is the key change: Check if any "Document Options" are selected AND no category is chosen
          if (docOptionsSelected && !categorySelected && !exemptDocsSelected) {
              alert("Please select Final / Wanted / Process for these documents.");
              return false;
          }

          var owner = document.getElementById("<%=ddlOwnerAgency.ClientID%>").value;
        if (!owner || owner === "ALL") {
            alert("Please select Owner Agency.");
            return false;
        }

        var viewerList = document.getElementById("<%=chkViewerAgencies.ClientID%>");
        if (viewerList) {
            var inputs = viewerList.getElementsByTagName("input");
            var anyViewer = false;
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type === "checkbox" && !inputs[i].disabled && inputs[i].checked) {
                    anyViewer = true;
                    break;
                }
            }
            if (!anyViewer) {
                alert("Please select at least one Viewer Agency.");
                return false;
            }
        }
        return true;
    }
  </script>
</asp:Content>
