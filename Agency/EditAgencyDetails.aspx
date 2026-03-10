<%@ Page Title="Edit Agency User" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="EditAgencyDetails.aspx.cs" Inherits="Agency_EditAgencyDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="row mt-4">
        <div class="col-12">

            <div class="card card-primary">

                <div class="card-header">
                    <h4>Edit Agency User Details</h4>
                </div>

                <div class="card-body">

                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>

                    <div class="row">

                        <div class="col-md-6 mb-3">
                            <label>Agency Name</label>
                            <asp:TextBox ID="txtAgencyName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Mobile Number</label>
                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Plain Password</label>
                            <asp:TextBox ID="txtPlainPassword" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Agency Type</label>

                            <asp:DropDownList ID="ddlAgencyType" runat="server" CssClass="form-control">

                                <asp:ListItem Value="">Select Agency Type</asp:ListItem>
                                <asp:ListItem Value="BSEB DATACENTER">BSEB DATACENTER</asp:ListItem>
                                <asp:ListItem Value="Scanning Agencies">Scanning Agencies</asp:ListItem>
                                <asp:ListItem Value="Result Processing">Result Processing</asp:ListItem>
                                <asp:ListItem Value="Marks Entry">Marks Entry</asp:ListItem>
                                <asp:ListItem Value="Printing">Printing</asp:ListItem>

                            </asp:DropDownList>

                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Status</label>

                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Active">Active</asp:ListItem>
                                <asp:ListItem Value="DeActive">DeActive</asp:ListItem>
                                <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                            </asp:DropDownList>

                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Role</label>
                            <asp:TextBox ID="txtRole" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Private Key</label>
                            <asp:TextBox ID="txtPrivateKey" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label>Key Expiry</label>
                            <asp:TextBox ID="txtKeyExpiry" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                        </div>

                    </div>

                    <div class="text-center mt-4">

                        <asp:Button ID="btnUpdate"
                            runat="server"
                            Text="Update Agency User"
                            CssClass="btn btn-primary"
                            OnClick="btnUpdate_Click" />

                        &nbsp;

                        <asp:Button ID="btnCancel"
                            runat="server"
                            Text="Cancel"
                            CssClass="btn btn-danger"
                            PostBackUrl="Agencymaster.aspx" />

                    </div>

                </div>

            </div>

        </div>
    </div>

</asp:Content>
