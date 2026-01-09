<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="validation.aspx.cs" Inherits="validation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        #sql-user-table td {
            vertical-align: middle;
        }

        #sql-user-table .fw-bold {
            font-weight: bold;
        }
    </style>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        th, td {
            border: 1px solid black;
            padding: 7px;
            text-align: left;
        }

        th {
            background-color: #f2f2f2;
        }

        .header {
            text-align: center;
            margin-bottom: 20px;
        }

        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            padding: 0;
        }

        .container {
            width: 80%;
            margin: auto;
            border: 2px solid black;
            padding: 20px;
        }

        .header {
            text-align: center;
            font-size: 20px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .logo {
            text-align: center;
            margin-bottom: 10px;
        }

            .logo img {
                width: 80px;
            }

        .marksheet {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 10px;
        }

            .marksheet, .marksheet th, .marksheet td {
                border: 1px solid black;
            }

                .marksheet th, .marksheet td {
                    padding: 10px;
                    text-align: center;
                }

        .subject-group {
            text-align: left;
            font-weight: bold;
            background-color: #e0e0e0;
        }

        .watermark {
            position: fixed;
            top: 40%;
            left: 50%;
            transform: translate(-50%, -50%) rotate(-30deg);
            font-size: 70px;
            font-weight: bold;
            color: rgba(0, 0, 0, 0.3);
            z-index: 9999;
            white-space: nowrap;
            pointer-events: none;
            text-align: center;
        }

        .watermark-subtext {
            display: block;
            font-size: 20px;
            font-weight: normal;
            margin-top: 10px;
            color: rgba(0, 0, 0, 0.4);
        }

        @page {
            margin: 10px 20px;
        }

        .print-btn-container {
            text-align: center;
            margin-top: 10px;
            margin-bottom: 20px;
        }

        .noprint {
            display: inline-block;
            width: 120px;
            padding: 10px;
            font-size: 16px;
            background-color: #28a745;
            color: white;
            border: none;
            cursor: pointer;
            border-radius: 5px;
        }

        .text_center th,
        .text_center td {
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        //with watermark
        function printDiv(divName) {
            var printContents = document.getElementById(divName).innerHTML;
            var originalContents = document.body.innerHTML;

            // Append watermark inside the print area
            var watermarkDiv = document.createElement("div");
            watermarkDiv.className = "watermark";
            watermarkDiv.innerHTML = `
            WEB COPY
           <span class="watermark-subtext">
    1. Bihar School Examination Board, Patna is not responsible for any <br>
    inadvertent error that may have crept in the results being published <br>
    on the NET. <br>
    2. The results published on the NET are for immediate information to <br>
    the examinees. <br>
    3. This is not a valid document.
</span>
        `;

            document.getElementById(divName).appendChild(watermarkDiv);
            // Set body content to the print section
            document.body.innerHTML = document.getElementById(divName).innerHTML;
            window.print();

            // Restore original content
            document.body.innerHTML = originalContents;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h4>Search Record</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <label>RollCode</label>
                            <asp:TextBox runat="server" ID="txt_rollcode" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label>RollNo</label>
                            <asp:TextBox runat="server" ID="txt_rollnumber" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group mt-2 text-center">
                        <asp:Button runat="server" ID="btn_submit" CssClass="btn btn-primary btn-lg" Text="Search" OnClick="btn_submit_Click" />
                    </div>
                </div>

            </div>
        </div>
        <%--<div class="col-12 col-md-6 col-lg-6">
            <div class="card">
                <div class="card-header">
                    <h4>MSSQL</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="sql-user-table">

                            <tbody>
                                <asp:Repeater ID="rpt_mssqldata" runat="server" OnItemDataBound="rpt_mssqldata_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="table table-bordered table-hover" id="sql-user-table">
                                            <thead class="thead-dark">
                                                <tr class="text-center align-middle">
                                                    <th>#</th>
                                                    <th>Student Name</th>
                                                    <th>Subject</th>
                                                    <th>Theory Marks</th>
                                                    <th>Practical Marks</th>
                                                    <th>CCE Marks</th>
                                                    <th>Theory Grace</th>
                                                    <th>Practical Grace</th>
                                                    <th>Tampered Status</th>
                                                    <th>Updated Date</th>
                                                    <th>Updated By</th>
                                                    <th>System Name</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>

                                    <ItemTemplate>
                                        <tr class="align-middle">
                                            <td class="text-center"><%# Container.ItemIndex + 1 %></td>

                                            
                                            <td runat="server" id="tdNameCell" class="text-center align-middle fw-bold">
                                                <asp:Label ID="lblStudentFullName" runat="server" Text='<%# Eval("StudentFullName") %>' />
                                            </td>

                                            <td><%# Eval("SubjectPaperName") %></td>
                                            <td class="text-center"><%# Eval("TotalTheoryMarks") %></td>
                                            <td class="text-center"><%# Eval("PRObtainedMarks") %></td>
                                            <td class="text-center"><%# Eval("CCEMarks") %></td>
                                            <td class="text-center"><%# Eval("TheoryGraceMarks") %></td>
                                            <td class="text-center"><%# Eval("PracticalGraceMarks") %></td>
                                            <td style='<%# Eval("TamperStatus").ToString() == "Tampered" ? "color:red;font-weight:bold;": "color:green;" %>'>
                                                <%# Eval("TamperStatus") %>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>

                                        </tr>
                                    </ItemTemplate>

                                    <FooterTemplate>
                                        </tbody>
        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>--%>

        <!-- FlureeDB (Blockchain) Data Section -->
        <div class="col-lg-12" id="DivBlockchain" runat="server" visible="false">
            <div class="card">
                <div class="card-header">
                    <h4>Blockhain Base Result </h4>
                </div>
                <div class="card-body">
                    <%-- <div class="print-btn-container">
                        <input type="button" value="PRINT" class="noprint btn btn-success" onclick="printDiv('pagewrap')" />
                    </div>--%>
                    <div class="container" id="pagewrap">
                        <br />
                        <table>
                            <tr>
                                <td><strong>BSEB Unique ID</strong></td>
                                <td>
                                    <asp:Label ID="lbl_bsebuniqueID" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Student's Name</strong></td>
                                <td>
                                    <asp:Label ID="lbl_studentname" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Father's Name</strong></td>
                                <td>
                                    <asp:Label ID="lbl_fathername" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>School/College Name</strong></td>
                                <td>
                                    <asp:Label ID="lbl_collegename" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Roll Code</strong></td>
                                <td>
                                    <asp:Label ID="lbl_rollcode" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Roll Number</strong></td>
                                <td>
                                    <asp:Label ID="lbl_rollnumber" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Registration Number</strong></td>
                                <td>
                                    <asp:Label ID="lbl_registrationno" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><strong>Faculty</strong></td>
                                <td>
                                    <asp:Label ID="lbl_faculty" runat="server"></asp:Label></td>
                            </tr>
                        </table>

                        <br>
                        <div class="table-responsive">
                            <table>
                                <tr class="text_center">
                                    <th rowspan="2">Subject</th>
                                    <th rowspan="2">Full Marks</th>
                                    <th rowspan="2">Pass Marks</th>
                                    <th rowspan="2">Theory</th>
                                    <th rowspan="2">Practical</th>
                                    <th rowspan="2" runat="server" id="th_CCE">CCE</th>
                                    <th colspan="2">Regulation</th>
                                    <th rowspan="2">Subject Total</th>
                                </tr>
                                <tr class="text_center">
                                    <th>Th.</th>
                                    <th>Pr.</th>
                                </tr>
                                <asp:Repeater ID="RepeaterSubjectGroups" runat="server">
                                    <ItemTemplate>
                                        <!--  Display Subject Group Name -->
                                        <tr>
                                            <td class="subject-group" colspan="9"><%# Eval("SubjectGroupName") %></td>
                                        </tr>

                                        <!-- Nested Repeater for Subjects inside the Group -->
                                        <asp:Repeater ID="RepeaterSubjects" runat="server" DataSource='<%# Eval("Subjects") %>'>
                                            <ItemTemplate>
                                                <tr class="text_center">

                                                    <td><%# Eval("Subject") %></td>
                                                    <td><%# Eval("FullMarks") %></td>
                                                    <td><%# Eval("PassingMarks") %></td>
                                                    <td><%# Eval("Theory") %></td>
                                                    <td><%# Eval("PracticalMarks") %></td>
                                                    <% if ((bool)ViewState["ShowCCEColumn"])
                                                        { %>
                                                    <td><%# Eval("CCEMarks") %></td>
                                                    <% } %>
                                                    <td><%# Eval("Regulation_Theory") %></td>
                                                    <td><%# Eval("Regulation_Practical") %></td>
                                                    <td><%# Eval("Subject_Total") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                        <br>
                        <table>
                            <tr>
                                <td colspan="2"><strong>Final Result:</strong></td>
                            </tr>
                            <tr>
                                <td style="width: 20%;"><strong>Aggregate Marks:</strong></td>
                                <td>
                                    <asp:Label runat="server" ID="lbl_aggregratemarks"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="width: 20%;"><strong>Result/Division:</strong></td>
                                <td>
                                    <asp:Label runat="server" ID="lbl_division"></asp:Label></td>
                            </tr>
                        </table>

                        <br>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

