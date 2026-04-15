<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Approvaldata2.aspx.cs" Inherits="Agency_Approvaldata2" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style> 
        .card {
            margin-bottom: 20px;
        }

        #table-1 {
            width: 100%;
            border-collapse: collapse;
        }

            #table-1 thead th {
                white-space: nowrap;
                text-align: center;
                background-color: #f4f6f9;
                font-weight: 600;
                font-size: 13px;
                padding: 10px;
            }

            #table-1 tbody td {
                white-space: nowrap;
                text-align: center;
                padding: 8px;
                font-size: 13px;
            }

            #table-1 td {
                max-width: 180px;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            #table-1 tbody tr:nth-child(even) {
                background-color: #fafafa;
            }

            #table-1 tbody tr:hover {
                background-color: #f1f1f1;
            }

        .table-responsive {
            overflow-x: auto;
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script>
        function confirmApprove(btn) {
            Swal.fire({
                title: 'Are you sure?',
                text: "Approve all records?",
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Yes, Approve'
            }).then((result) => {
                if (result.isConfirmed) {
                    btn.onclick = null;
                    btn.click();
                }
            });
            return false;
        }

        function confirmReject(btn) {
            Swal.fire({
                title: 'Are you sure?',
                text: "Reject all records?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Reject'
            }).then((result) => {
                if (result.isConfirmed) {
                    btn.onclick = null;
                    btn.click();
                }
            });
            return false;
        }
        function clearUI() {


            document.getElementById('<%= rollCode.ClientID %>').value = "";
            document.getElementById('<%= rollNo.ClientID %>').value = "";


            var panel = document.getElementById('<%= Student_details.ClientID %>');
            if (panel) panel.style.display = "none";


            var tableBody = document.querySelector("#table-1 tbody");
            if (tableBody) tableBody.innerHTML = "";


            var action = document.getElementById('<%= divAction2.ClientID %>');
            if (action) action.style.display = "none";
        }


        function validateSearch() {
            var rc = document.getElementById('<%= rollCode.ClientID %>').value.trim();
            var rn = document.getElementById('<%= rollNo.ClientID %>').value.trim();

            if (rc === "" || rn === "") {

                clearUI();

                Swal.fire("Error", "Enter Roll Code & Roll Number", "error");
                return false;
            }
            return true;
        }
    </script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div class="card">
        <div class="card-header">
            <h4>Data Approval 2</h4>
        </div>

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

        <div class="card-footer">
            <asp:Button ID="btn_search" runat="server"
                CssClass="btn btn-primary"
                Text="Search"
                OnClick="btn_search_Click"
                OnClientClick="return validateSearch();" />
        </div>
    </div>


    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>


    <div runat="server" id="Student_details" visible="false">

        <div class="card">
            <div class="card-header">
                <h4>Student Details</h4>
            </div>

            <div class="card-body">

                <div class="table-responsive">
                    <table id="table-1" class="table table-bordered">

                        <thead>
                            <tr>
                                <th>Sr No</th>
                                <th>Reg No</th>
                                <th>Roll Code</th>
                                <th>Roll No</th>
                                <th>Subject Name</th>
                                <th>Subject Code</th>
                                <th>Barcode</th>
                                <th>Litho</th>
                                <th>Marks Source</th>
                                <th>Subjective Marks</th>
                                <th>Total</th>
                                <th>Approval 1</th>
                                <th>Approval 2</th>
                            </tr>
                        </thead>

                        <tbody>
                            <asp:Repeater ID="rpt_userData" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Container.ItemIndex + 1 %></td>
                                        <td><%# Eval("reg_no") %></td>
                                        <td><%# Eval("roll_code") %></td>
                                        <td><%# Eval("roll_no") %></td>
                                        <td><%# Eval("Subjectname") %></td>
                                        <td><%# Eval("subjectcode") %></td>
                                        <td><%# Eval("BARCODE_BOTTOM") %></td>
                                        <td><%# Eval("Litho_Cbar_Fly") %></td>
                                        <td><%# Eval("MarksSourceName") %></td>
                                        <td><%# Eval("SubjectiveMarks") %></td>
                                        <td><%# Eval("subjecttotal") %></td>
                                        <td><%# Eval("Approval1") %></td>
                                        <td><%# Eval("Approval2") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>

                    </table>
                </div>

                <div class="text-center mt-3" runat="server" id="divAction2" visible="false">

                    <asp:Button ID="btnApproveAll" runat="server"
                        Text="Approve All"
                        CssClass="btn btn-success"
                        OnClick="btnApproveAll_Click"
                        OnClientClick="return confirmApprove(this);" />

                    <asp:Button ID="btnRejectAll" runat="server"
                        Text="Reject All"
                        CssClass="btn btn-danger"
                        OnClick="btnRejectAll_Click"
                        OnClientClick="return confirmReject(this);" />

                </div>

            </div>
        </div>

    </div>

</asp:Content>
 