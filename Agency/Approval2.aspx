<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Approval2.aspx.cs" Inherits="Approval2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12 col-md-12 col-lg-12">
            <!-- Roll No Dropdown and Search Button -->
            <div class="card">
                <div class="card-header">
                    <h4>Approval of Change</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <label for="rollnoDropdown">RollCode</label>
                            <asp:DropDownList runat="server" ID="ddl_status" CssClass="form-control">
                                <asp:ListItem Value="ALL">Select Status</asp:ListItem>
                                <asp:ListItem Value="Pending">Pending</asp:ListItem>
                                <asp:ListItem Value="Approve">Approve</asp:ListItem>
                                <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group mt-2 text-center">
                            <asp:Button runat="server" ID="btn_search" CssClass="btn btn-primary btn-lg" Text="Submit" OnClick="btn_search_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h4>Marks Details</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="userTable">
                            <thead class="thead-dark">
                                <tr class="text-center align-middle">
                                    <th>#</th>
                                    <th>Student Name</th>
                                    <th>Roll Code</th>
                                    <th>Roll Number</th>
                                    <th>Faculty Name</th>
                                    <th>Subject Paper Name</th>
                                    <th>Prev ObtainedMarks</th>
                                    <th>Updated ObtainedMarks</th>
                                    <th>Prev TotalTheoryMarks</th>
                                    <th>Updated TotalTheoryMarks</th>
                                    <th>Prev CCEMarks</th>
                                    <th>Updated CCEMarks</th>
                                    <th>Prev SubjectTotal</th>
                                    <th>Updated SubjectTotal</th>
                                    <th>Prev TotalMarks</th>
                                    <th>Updated TotalMarks</th>
                                    <th>Admin Approval</th>
                                    <th>Final Approval</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rpt_details" runat="server" OnItemDataBound="rpt_details_ItemDataBound" OnItemCommand="rpt_details_ItemCommand">
                                    <ItemTemplate>
                                        <tr class="align-middle text-center">
                                            <td><%# Container.ItemIndex + 1 %></td>

                                            <td>
                                                <asp:Label ID="lblStudentFullName" runat="server" Text='<%# Eval("StudentFullName") %>' />
                                            </td>

                                            <asp:HiddenField runat="server" ID="hf_rollcode" Value='<%# Eval("RollCode") %>' />
                                            <asp:HiddenField runat="server" ID="hf_rollno" Value='<%# Eval("RollNumber") %>' />
                                            <asp:HiddenField runat="server" ID="hf_SubjectPaperName" Value='<%# Eval("SubjectPaperName") %>' />
                                        <asp:HiddenField runat="server" ID="hf_ObtainedMarks" Value='<%# Eval("Updated_ObtainedMarks") %>' />
                                        <asp:HiddenField runat="server" ID="hf_TotalTheoryMarks" Value='<%# Eval("Updated_TotalTheoryMarks") %>' />
                                        <asp:HiddenField runat="server" ID="hf_CCEMarks" Value='<%# Eval("Updated_CCEMarks") %>' />
                                        <asp:HiddenField runat="server" ID="hf_SubjectTotal" Value='<%# Eval("Updated_SubjectTotal") %>' />
                                        <asp:HiddenField runat="server" ID="hf_TotalMarks" Value='<%# Eval("Updated_TotalMarks") %>' />
                                       
                                   


                                            <td><%# Eval("RollCode") %></td>
                                            <td><%# Eval("RollNumber") %></td>
                                            <td><%# Eval("FacultyName") %></td>
                                            <td><%# Eval("SubjectPaperName") %></td>

                                            <td><%# Eval("Prev_ObtainedMarks") %></td>
                                            <td><%# Eval("Updated_ObtainedMarks") %></td>
                                            <td><%# Eval("Prev_TotalTheoryMarks") %></td>
                                            <td><%# Eval("Updated_TotalTheoryMarks") %></td>
                                            <td><%# Eval("Prev_CCEMarks") %></td>
                                            <td><%# Eval("Updated_CCEMarks") %></td>
                                            <td><%# Eval("Prev_SubjectTotal") %></td>
                                            <td><%# Eval("Updated_SubjectTotal") %></td>
                                            <td><%# Eval("Prev_TotalMarks") %></td>
                                            <td><%# Eval("Updated_TotalMarks") %></td>
                                            <td><%# Eval("AdminApprovalStatus") %></td>



                                            <td>
                                                <asp:HiddenField runat="server" ID="hfAdminApprovalStatus" Value='<%# Eval("AdminApprovalStatus") %>' />
                                                <asp:HiddenField runat="server" ID="hfFinalApprovalStatus" Value='<%# Eval("FinalApprovalStatus") %>' />
                                                <asp:LinkButton ID="lnkApprove" runat="server" Text="Approve" CommandName="ApproveRow" CommandArgument='<%# Eval("Pk_ResultChangeRequestId") %>' CssClass="btn btn-success btn-sm" />
                                                <asp:LinkButton ID="lnkReject" runat="server" Text="Reject" CommandName="RejectRow" CommandArgument='<%# Eval("Pk_ResultChangeRequestId") %>' CssClass="btn btn-danger btn-sm" />

                                                <asp:Label ID="lblApproved" runat="server" Text="Approved" CssClass="text-success fw-bold" Visible="false" />
                                                <asp:Label ID="lblRejected" runat="server" Text="Rejected" CssClass="text-danger fw-bold" Visible="false" />
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="row mt-3" runat="server" id="div_key" visible="false">
                        <div class="col-md-12">
                            <div class="form-group">

                                <label for="file-upload">Enter Private Key</label>
                                <div class="input-group mb-3">
                                    <asp:HiddenField ID="hf_SelectedRequestId" runat="server" />

                                    <asp:TextBox runat="server" ID="txt_pvtkey" CssClass="form-control" onblur="validateInput()" />
                                    <span id="keyError" style="color: red; display: none;">Private key is required.</span>
                                    <div class="input-group-append">
                                        <asp:Button runat="server" ID="btn_submittoken" CssClass="btn btn-primary" OnClick="btn_submittoken_Click" Text="Verify" />
                                    </div>
                                </div>
                                <asp:Label runat="server" ID="lbl_validate" class="font-18"></asp:Label>
                                <div class="form-group text-center">
                                    <asp:Button runat="server" ID="btn_submit" CssClass="btn btn-primary btn-lg" Text="Submit" OnClick="btn_submit_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
