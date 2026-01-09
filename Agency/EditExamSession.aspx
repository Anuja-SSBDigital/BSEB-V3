<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="EditExamSession.aspx.cs" Inherits="Agency_EditExamSession" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row mt-4">
        <div class="col-12">
            <div class="card">

                <div class="card-header">
                    <h4>Edit Exam Session</h4>
                </div>

                <div class="card-body">

                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-2 d-block"></asp:Label>

                    <asp:HiddenField ID="hfSessionId" runat="server" />

                    <div class="row">
                        <div class="col-lg-6 mb-3">
                            <label class="form-label">Session Name</label>
                            <asp:TextBox ID="txtSessionName" runat="server"
                                CssClass="form-control" />
                        </div>

                        <div class="col-lg-6 mb-3">
                            <label class="form-label">Status</label>
                            <asp:DropDownList ID="ddlStatus" runat="server"
                                CssClass="form-control">
                                <asp:ListItem Value="1">Active</asp:ListItem>
                                <asp:ListItem Value="0">Inactive</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mt-3">
                        <div class="col-lg-2">
                            <asp:Button ID="btnUpdate" runat="server"
                                Text="Update"
                                CssClass="btn btn-success w-100"
                                OnClick="btnUpdate_Click" />
                        </div>
                    
                    </div>

                </div>
            </div>
        </div>
    </div>

</asp:Content>