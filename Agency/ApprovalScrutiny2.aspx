<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="ApprovalScrutiny2.aspx.cs" Inherits="Agency_ApprovalScrutiny2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        
.summary-card {
    border: none;
    border-radius: 16px;
    background: #ffffff;
    box-shadow: 0 8px 25px rgba(0,0,0,0.06);
    overflow: hidden;
    transition: 0.3s ease;   
}

.summary-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 12px 30px rgba(0,0,0,0.08);
}


.card.summary-card .card-header {
    background: #1E3A8A !important;
    padding: 12px 14px;
    border-bottom: 0.5px solid #1E3A8A !important;
}

.card.summary-card .card-header h4 {
    margin: 0;
    font-size: 20px;
    font-weight: 700;
    color: #ffffff !important;
    letter-spacing: 0.5px;
}

.summary-card .card-body {
    padding: 25px 30px;
}

.summary-card h5 {
    font-size: 13px;
    font-weight: 600;
    color: #64748b;
    margin-bottom: 6px;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.summary-card span,
.summary-card label {
    font-size: 26px;
    font-weight: 800;
    color: #0f172a;
}

.summary-card .agency-text {
    display: inline-block;
    font-size: 14px;
    font-weight: 600;
    color: #2563eb;
    background: #eff6ff;
    padding: 6px 14px;
    border-radius: 20px;
    margin-top: 6px;
}

.summary-divider {
    height: 1px;
    background: #e5e7eb;
    margin: 20px 0;
}

#divAction {
    margin-top: 25px;
    display: flex;
    justify-content: center;
    gap: 15px;
}

#divAction {
    margin-top: 20px;
    text-align: center;
}

#divAction .btn {
    padding: 10px 20px;
    font-weight: 700;
    border-radius: 8px;
}

.btn-success {
    background: #0f5132;
    border: none;
}

.btn-danger {
    background: #842029;
    border: none;
}

.btn-success:hover {
    background: #146c43;
}

.btn-danger:hover {
    background: #a52834;
}

.form-label-bold,
label {
    font-weight: 800;
    color: #111827;
    font-size: 14px;
}

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
    vertical-align: middle;
    background-color: #f4f6f9;
    font-weight: 600;
    font-size: 13px;
    padding: 10px;
}

#table-1 tbody td {
    white-space: nowrap;
    vertical-align: middle;
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

.badge {
    padding: 10px 15px;
}

.card {
    margin-bottom: 20px;
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

        function confirmGlobalApprove() {
            return confirm("Are you sure you want to APPROVE ALL records?");
        }

        function confirmGlobalReject() {
            return confirm("Are you sure you want to REJECT ALL records?");
        }

        function validateSearch() {
            var rollCode = document.getElementById('<%= rollCode.ClientID %>').value.trim();
            var rollNo = document.getElementById('<%= rollNo.ClientID %>').value.trim();

            var studentDiv = document.getElementById('<%= Student_details.ClientID %>');


            if (rollCode === "" || rollNo === "") {
                Swal.fire('Validation Error', 'Enter Roll Code & Roll Number', 'warning');


                if (studentDiv) studentDiv.style.display = "none";

                return false;
            }


            if (!/^\d+$/.test(rollCode) || !/^\d+$/.test(rollNo)) {
                Swal.fire('Invalid Input', 'Only numeric values allowed', 'error');


                if (studentDiv) studentDiv.style.display = "none";

                return false;
            }

            return true;
        }

        function approveAllAjax() {
            if (!confirm("Are you sure you want to APPROVE ALL records?")) return;

            Swal.fire({ title: 'Processing...', didOpen: () => Swal.showLoading() });

            PageMethods.GlobalApprove(function (res) {
                Swal.fire('Done', res + ' records approved', 'success');
                loadSummaryAjax();
            });
        }


        function approveAllAjax() {
            if (!confirm("Are you sure you want to APPROVE ALL records?")) return;

            Swal.fire({ title: 'Processing...', didOpen: () => Swal.showLoading() });

            PageMethods.GlobalApprove(function (res) {

                var parts = res.split('|');


                if (parts[0] === "ERROR") {
                    Swal.fire(
                        'Error',
                        parts[1] + ' Records Approval1 Pending ' + ' cannot be Approval2 Approved',
                        'error'
                    );
                }

                else {
                    Swal.fire('Done', parts[1] + ' records approved', 'success');
                }

                loadSummaryAjax();
            });
        }

        function rejectAllAjax() {
            if (!confirm("Are you sure you want to REJECT ALL records?")) return;

            Swal.fire({ title: 'Processing...', didOpen: () => Swal.showLoading() });

            PageMethods.GlobalReject(function (res) {
                Swal.fire('Done', res + ' records rejected', 'success');
                loadSummaryAjax();
            });
        }

        function loadSummaryAjax() {
            PageMethods.GetSummary(function (data) {

                document.getElementById('<%= lblTotalRows.ClientID %>').innerText = data.TotalRows;
                document.getElementById('<%= lblUniqueCount.ClientID %>').innerText = data.UniqueStudents;

                document.getElementById('<%= summaryCard.ClientID %>').style.display = "block";
            });
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

    <div class="row">
        <div class="col-12"  >
               
            <div class="card summary-card" runat="server" id="summaryCard">
                <div class="card-header">
                    <h4>Bseb Scrutiny Records</h4>
                </div>

                <div class="card-body">

                    <div class="row text-center">
                        <div class="col-md-6">
                            <h5>Scrutiny Data Updated By</h5>
                            <span class="agency-text">Kids</span>
                        </div>
                        <div class="col-md-6">
                            <h5>Result Publish By</h5>
                            <span class="agency-text">SSB Digital</span>
                        </div>
                    </div>


                    <div class="row text-center">
                        <div class="col-md-6">
                            <h5>Total No of Scrutiny Records</h5>
                            <asp:Label ID="lblUniqueCount" runat="server" />
                        </div>


                        <div class="col-md-6">
                            <h5>Total No of Result Change Records</h5>
                            <asp:Label ID="lblTotalRows" runat="server" />
                        </div>

                    </div>


                    <div class="action-global" runat="server" id="divAction">
                        <asp:Button ID="btnGlobalApprove" runat="server"
                            Text="Approve"
                            CssClass="btn btn-success"
                            OnClick="btnGlobalApprove_Click"
                            OnClientClick="approveAllAjax(); return false;" />

                        <asp:Button ID="btnGlobalReject" runat="server"
                            Text="Reject"
                            CssClass="btn btn-danger"
                            OnClick="btnGlobalReject_Click"
                            OnClientClick="rejectAllAjax(); return false;" />
                    </div>

                </div>

            </div>
        </div>
    </div>


    <div class="card">
        <div class="card-header">
            <h4>Roll Code And Roll No. Wise Scrutiny Records Status Check</h4>
        </div>

        <div class="card-body">
            <div class="row">
                <div class="col-md-6">

                    <label class="form-label-bold">Roll Code</label>
                    <asp:TextBox ID="rollCode" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">

                    <label class="form-label-bold">Roll Number</label>
                    <asp:TextBox ID="rollNo" runat="server" CssClass="form-control" />
                </div>
            </div>
        </div>

        <div class="card-footer">
            <asp:Button ID="btn_search" runat="server"
                CssClass="btn btn-primary"
                Text="Search"
                OnClick="btn_search_Click"
                OnClientClick="return validateSearch();" />
        </div>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true" />

        <div runat="server" id="Student_details" visible="false">

            <div class="card-header">
            </div>

            <div class="card-body">
                <div class="table-responsive">

                    <table class="table table-bordered" id="table-1">
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
                                <th>Status</th>
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
                                        <td><%# Eval("Approval2") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                </div>
            </div>
        </div>

    </div>

</asp:Content>
