<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="managepassword.aspx.cs" Inherits="Agency_managepassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12 grid-margin stretch-card">
            <div class="card">
                <div class="card-header">
                    <h4>Change Password</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <label for="txt_OldPassword">Current Password</label>
                            <div class="form-group">
                                <asp:HiddenField ID="hfSessionUserId" runat="server" />
                                <asp:TextBox ID="txt_CurrentPassword" CssClass="form-control"
                                    runat="server" Type="password" Onkeyup="PasswordCheck();"></asp:TextBox>
                                <span id="currentpass-error" class="text-danger"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <label for="txt_newpassword">New Password</label>
                            <div class="form-group">
                                <asp:TextBox ID="txt_NewPassword" CssClass="form-control"
                                    runat="server" Type="password" Onkeyup="PasswordCheck();"></asp:TextBox>
                                <span id="newpass-error" class="text-danger"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <label for="txt_cPassword">Confirm Password</label>
                            <div class="form-group">
                                <asp:TextBox ID="txt_ConfirmPassword" CssClass="form-control"
                                    runat="server" Type="password" Onkeyup="PasswordCheck();"></asp:TextBox>
                                <span id="confirmpass-error" class="text-danger"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Button ID="btn_ChangePassword" runat="server" Text="Submit" CssClass="btn btn-primary mt-2"
                                Type="submit"
                                OnClientClick="return Val()&& managepassword();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function PasswordCheck() {
            var CurrentPassword = document.getElementById("<%= txt_CurrentPassword.ClientID %>").value;
              var NewPassword = document.getElementById("<%= txt_NewPassword.ClientID %>").value;
              var ConfNewPassword = document.getElementById("<%= txt_ConfirmPassword.ClientID %>").value;
            var maxNumberofChars = 8;
            var regularExpression = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@_])[A-Za-z\d@_]{8,}$/;

            if (!regularExpression.test(CurrentPassword)) {
                $('#currentpass-error').text('The password should be exactly 8 characters long and contain at least 1 number, 1 uppercase letter, 1 lowercase letter, and 1 special character (supporting only "@ and _").');
                return false;
            } else {
                $('#currentpass-error').text('');
            }

            if (!regularExpression.test(NewPassword)) {

                $('#newpass-error').text('The password should be exactly 8 characters long and contain at least 1 number, 1 uppercase letter, 1 lowercase letter, and 1 special character (supporting only "@ and _").');
                return false;
            } else {
                $('#newpass-error').text('');
            }

            if (!regularExpression.test(ConfNewPassword)) {
                $('#confirmpass-error').text('The password should be exactly 8 characters long and contain at least 1 number, 1 uppercase letter, 1 lowercase letter, and 1 special character (supporting only "@ and _").');
                return false;
            } else {
                $('#confirmpass-error').text('');
            }

        }
        function Val() {
            var CurrentPassword = document.getElementById("<%= txt_CurrentPassword.ClientID %>");
            var NewPassword = document.getElementById("<%= txt_NewPassword.ClientID %>");
            var ConfNewPassword = document.getElementById("<%= txt_ConfirmPassword.ClientID %>");

            if (CurrentPassword.value == "") {
                CurrentPassword.classList.add('is-invalid');
                return false;
            } else {
                CurrentPassword.classList.remove('is-invalid');
            }

            //pw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{4,15}$/;
            //if (!pw.test(NewPassword.value)) {
            if (NewPassword.value == "") {
                NewPassword.classList.add('is-invalid');
                return false;
            } else {
                NewPassword.classList.remove('is-invalid');
            }

            if (ConfNewPassword.value != NewPassword.value) {
                ConfNewPassword.classList.add('is-invalid');
                return false;
            } else {
                ConfNewPassword.classList.remove('is-invalid');
            }

            return true;
        }
        function managepassword() {
            var currentpassword = $("#<%= txt_CurrentPassword.ClientID %>").val();
                    var newpassowrd = $("#<%= txt_NewPassword.ClientID %>").val();
            var confirmpassword = $("#<%= txt_ConfirmPassword.ClientID %>").val();
            var userid = $("#<%= hfSessionUserId.ClientID %>").val();
            $.ajax({
                type: "POST",
                url: "managepassword.aspx/ChangePassword",
                data: JSON.stringify({
                    currentpassword: currentpassword,
                    newpassowrd: newpassowrd,
                    confirmpassword: confirmpassword,
                    userid: userid
                }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    var message = JSON.parse(response.d).message;
                    alert(message);
                    if (message === "Password changed successfully.") {
                        window.location.href = "../login.aspx";
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error: " + error);
                    alert("An error occurred while changing the password.");
                }
            });

            return false;
        }

    </script>
</asp:Content>

