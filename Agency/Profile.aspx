<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="Agency_Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        #preview {
            width: 150px;
            height: 150px;
            border: 2px dashed #ddd;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            margin-bottom: 10px;
            border-radius: 10px;
        }

            #preview img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="content-wrapper">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header text-center">
                        <h4 class="card-title">Manage Profile</h4>
                    </div>
                    <div class="card-body">

                        <div class="row grid-margin stretch-card">
                            <div class="col-md-4">
                                <div id="preview">
                                    <img id="previewImg" src="../assets/img/users/1577909.png" alt="Profile Preview">
                                </div>
                                <div class="custom-file">
                                    <%--<input type="file" class="custom-file-input" id="customFile" runat="server">
                                    <label class="custom-file-label" for="customFile">Choose file</label>--%>
                                    <asp:Fileupload runat="server" ID="fl_image" class="form-control"  />
                                </div>

                            </div>

                            <div class="col-md-8">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="UsernameLabel">UserID</label>
                                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" ReadOnly></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="PemailLabel">Email</label>
                                            <asp:TextBox ID="txtEmail" Type="email" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="PmobileNoLabel">Mobile No</label>
                                            <asp:TextBox ID="txtMobileNo" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="AgencyNameLabel">Agency Name</label>
                                            <asp:TextBox ID="txtAgencyName" runat="server" CssClass="form-control" ReadOnly></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>


                            <div class="card-footer text-end">
                                <asp:Button runat="server" ID="btnsearch"  CssClass="btn btn-primary" Text="Update Profile" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <script type="text/javascript">
        document.getElementById('customFile').addEventListener('change', function (event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function () {
                    document.getElementById('previewImg').src = reader.result;
                }
                reader.readAsDataURL(file);
            }
        });




    </script>
</asp:Content>
