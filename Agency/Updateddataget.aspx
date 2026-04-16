<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master"
    AutoEventWireup="true" CodeFile="Updateddataget.aspx.cs"
    Inherits="Agency_Updateddataget" Async="true" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        body {
            background-color: #f4f6f9;
        }

        .card {
            margin-bottom: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            border: none;
        }


        .card-header {
            background: #1f2d3d;
            text-align: center;
            padding: 12px;
            border-radius: 10px 10px 0 0;
        }

            .card-header h4,
            .card-header h5 {
                margin: 0;
                color: #ffffff;
                font-weight: 600;
            }

        .card-body {
            padding: 20px;
        }

        .card-footer {
            text-align: center;
            background: #fff;
            border-top: none;
            padding-bottom: 15px;
        }


        #Student_details .card-header {
            background: #ffffff;
            color: #000000;
            border-bottom: 1px solid #ddd;
        }

            #Student_details .card-header h4 {
                color: #000000;
            }


        #Student_details td,
        #Student_details label,
        #Student_details span,
        #Student_details div {
            color: #000000 !important;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
            margin-top: 10px;
        }

        th, td {
            border: 1px solid #dee2e6;
            padding: 6px;
        }


        #Student_details table th {
            background: #007bff !important;
            color: #ffffff !important;
            text-align: center;
            font-weight: 600;
        }

        #Student_details th * {
            color: #ffffff !important;
        }

        td {
            text-align: center;
            background: #ffffff;
        }

            td:nth-child(2) {
                text-align: left;
            }


        .version-title {
            background: #ffffff;
            color: #000000;
            border: 1px solid #000;
            text-align: center;
            font-weight: bold;
            margin-bottom: 10px;
            padding: 6px;
            border-radius: 5px;
        }

        .result-box {
            margin-top: 10px;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background: #ffffff;
            font-weight: 600;
        }

        label {
            font-weight: 600;
        }

        @media (max-width: 768px) {
            .col-md-6 {
                margin-bottom: 15px;
            }
        }
    </style>


    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script>

        function validateSearch() {
            var rc = document.getElementById('<%= rollCode.ClientID %>').value.trim();
            var rn = document.getElementById('<%= rollNo.ClientID %>').value.trim();

            if (rc === "" || rn === "") {
                clearOldData();
                Swal.fire("Error", "Enter Roll Code & Roll Number", "error");
                return false;
            }
            return true;
        }

        function clearOldData() {

            var lbls = [
                '<%= lblUID.ClientID %>',
                '<%= lblName.ClientID %>',
                '<%= lblFather.ClientID %>',
                '<%= lblCollege.ClientID %>',
                '<%= lblRollCode.ClientID %>',
                '<%= lblRollNo.ClientID %>',
                '<%= lblRegNo.ClientID %>',
                '<%= lblFaculty.ClientID %>'

            ];     

            for (var i = 0; i < lbls.length; i++) {
                var el = document.getElementById(lbls[i]);
                if (el) el.innerHTML = "";
            }

            var v1 = document.getElementById('<%= rpt_v1.ClientID %>');
            var v2 = document.getElementById('<%= rpt_v2.ClientID %>');

            if (v1) v1.innerHTML = "";
            if (v2) v2.innerHTML = "";

            var panel = document.getElementById('<%= Student_details.ClientID %>');
            if (panel) panel.style.display = "none";
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="card">

        <h5 style="text-align: left; font-weight: bold; margin-left: 10px; margin-top: 10px;">Bihar School Examination Board Result
    </h5>      
           
        <div class="card-body row">      

            <div class="col-md-6">
                <label>Roll Code</label>
                <asp:TextBox ID="rollCode" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label>Roll Number</label>
                <asp:TextBox ID="rollNo" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="card-footer" style="text-align: left; padding-left: 20px;">
            <asp:Button ID="btn_search" runat="server"
                CssClass="btn btn-primary"
                Text="Search"
                OnClick="btn_search_Click"
                OnClientClick="return validateSearch();" />
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>

    <div runat="server" id="Student_details" clientidmode="Static" visible="false">

        <div class="card">
            <div class="card-header">
                <h4>Student Details</h4>
            </div>

            <div class="card-body">
                <div class="row">

                    <div class="col-md-4">
                        <b>BSEB Unique Id:</b>
                        <asp:Label ID="lblUID" runat="server" />
                    </div>
                    <div class="col-md-4">
                        <b>Student's Name:</b>
                        <asp:Label ID="lblName" runat="server" />
                    </div>
                    <div class="col-md-4">
                        <b>Father's Name:</b>
                        <asp:Label ID="lblFather" runat="server" />
                    </div>


                    <div class="col-md-4">
                        <b>Roll Code:</b>
                        <asp:Label ID="lblRollCode" runat="server" />
                    </div>
                    <div class="col-md-4">
                        <b>Roll Number:</b>
                        <asp:Label ID="lblRollNo" runat="server" />
                    </div>

                    <div class="col-md-4">
                        <b>Registration Number:</b>
                        <asp:Label ID="lblRegNo" runat="server" />
                    </div>
                    <div class="col-md-4">
                        <b>Faculty:</b>
                        <asp:Label ID="lblFaculty" runat="server" />
                    </div>

                    <div class="col-md-4">
                        <b>School/College Name:</b>
                        <asp:Label ID="lblCollege" runat="server" />
                    </div>

                </div>

                <hr />

                <div class="row">

                    <div class="col-md-6" style="padding-right: 10px;">
                        <div class="version-title">V1</div>

                        <table border="1">
                            <tr>
                                <th rowspan="2">Subject Group</th>
                                <th rowspan="2">Subject</th>
                                <th rowspan="2">Full Marks</th>
                                <th rowspan="2">Pass Marks</th>
                                <th rowspan="2">Theory</th>
                                <th rowspan="2">Practical</th>
                                <th id="thCCE_v1" runat="server" rowspan="2">CCE</th>
                                <th colspan="2">Regulation</th>
                                <th rowspan="2">Subject Total</th>
                            </tr>
                            <tr>
                                <th>Th</th>
                                <th>Pr</th>
                            </tr>

                            <asp:Repeater ID="rpt_v1" runat="server" OnItemDataBound="rpt_v1_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td runat="server" id="tdGroup"></td>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <%# Eval("sub") %>
                                        </td>

                                        <td style="text-align: center; vertical-align: middle;">
                                            <%# Eval("maxMark") %>
                                        </td>

                                        <td><%# Eval("passMark") %></td>
                                        <td><%# Eval("theory") %></td>
                                        <td><%# Eval("oB_PR") %></td>
                                        <td id="tdCCE_v1" runat="server"><%# Eval("cceMarks") %></td>

                                        <td>
                                            <%# (Eval("grC_THO") == DBNull.Value || Convert.ToInt32(Eval("grC_THO")) == 0) ? "" : Eval("grC_THO") %>
                                        </td>

                                        <td>
                                            <%# (Eval("grC_PR") == DBNull.Value || Convert.ToInt32(Eval("grC_PR")) == 0) ? "" : Eval("grC_PR") %>
                                        </td>

                                        <td><%# Eval("totSub") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                        </table>


                        <div class="result-box">
                            <div>
                                Aggregate Marks: <span>
                                    <asp:Label ID="V1totalmarks" runat="server" /></span>
                            </div>
                            <div>
                                Result Division: <span>
                                    <asp:Label ID="V1division" runat="server" /></span>
                            </div>
                        </div>

                    </div>
                    <div class="col-md-6" style="padding-left: 10px;">

                        <div class="version-title">V2</div>

                        <table border="1">
                            <tr>

                                <th rowspan="2">Subject Group</th>
                                <th rowspan="2">Subject</th>
                                <th rowspan="2">Full Marks</th>
                                <th rowspan="2">Pass Marks</th>
                                <th rowspan="2">Theory</th>
                                <th rowspan="2">Practical</th>
                                <th id="thCCE_v2" runat="server" rowspan="2">CCE</th>
                                <th colspan="2">Regulation</th>
                                <th rowspan="2">Subject Total</th>
                            </tr>
                            <tr>
                                <th>Th</th>
                                <th>Pr</th>
                            </tr>

                            <asp:Repeater ID="rpt_v2" runat="server" OnItemDataBound="rpt_v2_ItemDataBound">
                                <ItemTemplate>
                                    <tr>

                                        <td runat="server" id="tdGroup_v2"></td>

                                        <td style="text-align: center; vertical-align: middle;">
                                            <%# Eval("sub") %>
                                        </td>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <%# Eval("maxMark") %>
                                        </td>
                                        <td><%# Eval("passMark") %></td>
                                        <td><%# Eval("theory") %></td>
                                        <td><%# Eval("oB_PR") %></td>

                                        <td id="tdCCE_v2" runat="server"><%# Eval("cceMarks") %></td>

                                        <td>
                                            <%# (Eval("grC_THO") == DBNull.Value || Convert.ToInt32(Eval("grC_THO")) == 0) ? "" : Eval("grC_THO") %>
                                        </td>

                                        <td>
                                            <%# (Eval("grC_PR") == DBNull.Value || Convert.ToInt32(Eval("grC_PR")) == 0) ? "" : Eval("grC_PR") %>
                                        </td>

                                        <td><%# Eval("totSub") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>

                        <div class="result-box">
                            <div>
                                Aggregate Marks: <span>
                                    <asp:Label ID="V2totalmarks" runat="server" /></span>
                            </div>
                            <div>
                                Result Division: <span>
                                    <asp:Label ID="V2division" runat="server" /></span>
                            </div>
                        </div>

                    </div>

                </div>
            </div>
        </div>
    </div>

</asp:Content>