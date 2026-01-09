<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Editipdetails.aspx.cs" Inherits="Agency_Editipdetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="row mt-4">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
            <div class="card">
                <div class="card-header">
                    <h4>Edit IP</h4>
                </div> 
                <div class="card-body">

                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
                    <asp:HiddenField ID="hfIPID" runat="server" />

                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12 mb-3">
                            <label for="txtEditIP" class="form-label">IP Address</label>
                            <asp:TextBox ID="txtEditIP" runat="server" CssClass="form-control" />
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12 mb-3">
                            <label for="txtAgencyName" class="form-label">Agency Name</label>
                            <asp:TextBox ID="txtAgencyName" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="row mt-3">
                        <h5 class="mb-3">Access Type</h5>

                        <div class="col-lg-4 col-md-4 col-sm-12 mb-2">
                            <asp:CheckBox ID="chkProcessCSV" runat="server" Text="Process CSV" CssClass="me-2" />
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12 mb-2">
                            <asp:CheckBox ID="chkFileUpload" runat="server" Text="File Upload" CssClass="me-2" />
                        </div>

                      
                    </div>

                    <div class="row mt-4">
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <asp:Button ID="btnUpdate" runat="server" Text="Update"
                                CssClass="btn btn-success w-100" OnClick="btnUpdate_Click" />
                        </div>
                    </div>
                </asp:Content>