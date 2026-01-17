<%@ Page Language="C#" AutoEventWireup="true" CodeFile="registerAgs.aspx.cs" Inherits="registerAgs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">


<!-- auth-login.html  21 Nov 2019 03:49:32 GMT -->
<head>
    <meta charset="UTF-8">
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, shrink-to-fit=no" name="viewport">
    <title>BSEB- Register</title>
    <!-- General CSS Files -->
    <link rel="stylesheet" href="assets/css/app.min.css">
    <link rel="stylesheet" href="assets/bundles/bootstrap-social/bootstrap-social.css">
    <!-- Template CSS -->
    <link rel="stylesheet" href="assets/css/style.css">
    <link rel="stylesheet" href="assets/css/components.css">
    <!-- Custom style CSS -->
    <link rel="stylesheet" href="assets/css/custom.css">
    <link rel='shortcut icon' type='image/x-icon' href='../assets/img/favicon_v1.png' />
    <link rel="stylesheet" href="assets/bundles/izitoast/css/iziToast.min.css">
    <style>
        .is-invalid {
            border-color: #dc3545 !important;
            background-image: none !important;
            box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
        }

            .is-invalid:focus {
                border-color: #dc3545;
                box-shadow: 0 0 0 0.25rem rgba(220, 53, 69, 0.25);
            }

        @media (max-width: 576px) { /* mobile view */
            .header-logo img {
                width: 100% !important; /* or max 140-160px */
                max-width: 100px;
                height: auto;
                margin: 0 auto 10px auto;
                display: block;
            }
        }
    </style>
</head>

<body onload="generate()" style="background-image: url('assets/img/bg1.jpg'); background-position: center; background-size: cover; background-repeat: no-repeat; background-attachment: fixed;">


    <div id="app">
        <section class="section">
            <div class="container mt-5">
                <div class="row">
                    <div class="col-12 col-lg-6 col-md-6 col-sm-8 offset-lg-3 offset-md-3 offset-sm-2">
                        <div class="card card-primary">
                            <div class="header-logo text-center mt-2">
                                <div class="row">
                                    <div class="align-right col-md-3 mt-2">
                                        <img alt="image" src="assets/img/bsebimage.jpg" class="w-75">
                                    </div>
                                    <div class="align-content-around align-self-lg-end col-md-9 text-body">

                                        <h5>बिहार विद्यालय परीक्षा समिति
                                        </h5>
                                        <h6>BIHAR SCHOOL EXAMINATION BOARD
                                        </h6>
                                    </div>
                                </div>
                                <%--<img alt="image" src="assets/img/bsebimage.jpg" class="header-logo" />--%>
                            </div>

                            <div class="card-body">
                                <form runat="server" class="needs-validation" novalidate>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label for="agency-name">Agency Name</label>
                                                <asp:DropDownList runat="server" ID="ddl_AgencyName" CssClass="form-control" Required="true">
                                                    <asp:ListItem Value="" Text="Select Agency Name" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="Mapple" Text="Mapple"></asp:ListItem>
                                                    <asp:ListItem Value="Datacon" Text="Datacon"></asp:ListItem>
                                                    <asp:ListItem Value="Kids" Text="Kids"></asp:ListItem>
                                                    <asp:ListItem Value="MCRK" Text="MCRK"></asp:ListItem>
                                                    <asp:ListItem Value="Keltron" Text="Keltron"></asp:ListItem>
                                                    <asp:ListItem Value="Charu Mindworks" Text="Charu Mindworks"></asp:ListItem>
                                                    <asp:ListItem Value="Hitech" Text="Hitech"></asp:ListItem>
                                                    <asp:ListItem Value="Shree Jagannath Udyog" Text="Shree Jagannath Udyog"></asp:ListItem>
                                                    <asp:ListItem Value="Datasoft" Text="Datasoft"></asp:ListItem>
                                                    <asp:ListItem Value="Antier" Text="Antier"></asp:ListItem>
                                                    <asp:ListItem Value="SSB Digital" Text="SSB Digital"></asp:ListItem>
                                                </asp:DropDownList>
                                                <div class="invalid-feedback">Please select agency name</div>
                                            </div>
                                        </div>
                                    </div>


                                    <%-- <div class="form-group">
                                        <label for="Username">UserName</label>
                                        <asp:TextBox runat="server" ID="txt_UN" CssClass="form-control" Required="true"></asp:TextBox>
                                        <div class="invalid-feedback">Please fill in your UserName</div>
                                    </div>--%>

                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label for="email">Email</label>
                                                <asp:TextBox runat="server" ID="txt_Email" CssClass="form-control" TextMode="Email" Required="true"></asp:TextBox>
                                                <div class="invalid-feedback">Please enter a valid email</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label for="phone">Phone</label>
                                                <asp:TextBox runat="server" ID="txt_Phone" CssClass="form-control" Required="true"></asp:TextBox>
                                                <div class="invalid-feedback">Please enter a valid phone number</div>
                                            </div>
                                        </div>
                                    </div>

                                    <%--<div class="form-group">
                                        <label for="password">Password</label>
                                        <asp:TextBox runat="server" ID="txt_Password" CssClass="form-control" TextMode="Password" Required="true"></asp:TextBox>
                                        <div class="invalid-feedback">Please fill in your password</div>
                                    </div>

                                    <div class="form-group">
                                        <label for="confirm-password">Confirm Password</label>
                                        <asp:TextBox runat="server" ID="txt_ConfirmPassword" CssClass="form-control" TextMode="Password" Required="true"></asp:TextBox>
                                        <div class="invalid-feedback">Please confirm your password</div>
                                    </div>--%>

                                    <div class="form-group">
                                        <asp:TextBox runat="server" ID="entered_captcha" CssClass="form-control" Placeholder="Enter the captcha.."></asp:TextBox>
                                    </div>

                                    <div class="input-group form-group">
                                        <div class="input-group-prepend">
                                            <div class="input-group-text" onclick="generate()">
                                                <i class="material-icons" id="iconuser">refresh</i>
                                            </div>
                                        </div>
                                        <asp:TextBox runat="server" ID="generated_captcha" CssClass="form-control currency" ReadOnly="true" OnCopy="return false;"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <asp:Button runat="server" ID="btn_submit" Text="Register" CssClass="btn btn-primary btn-lg btn-block" OnClick="btn_submit_Click" />
                                    </div>
                                </form>
                                <div class="text-center mt-4 mb-3">
                                    <div class="text-job text-muted">
                                        <a href="login.aspx" class="float-right p-b-10">Already have an account? Login here</a>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </section>
    </div>

    <script src="assets/bundles/izitoast/js/iziToast.min.js"></script>
    <script>
        let captcha;
        let alphabets = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz0123456789";

        // CAPTCHA GENERATION
        generate = () => {
            let first = alphabets[Math.floor(Math.random() * alphabets.length)];
            let second = Math.floor(Math.random() * 10);
            let third = Math.floor(Math.random() * 10);
            let fourth = alphabets[Math.floor(Math.random() * alphabets.length)];
            let fifth = alphabets[Math.floor(Math.random() * alphabets.length)];
            let sixth = Math.floor(Math.random() * 10);
            captcha =
                first.toString() +
                second.toString() +
                third.toString() +
                fourth.toString() +
                fifth.toString() +
                sixth.toString();
            document.getElementById('<%= generated_captcha.ClientID %>').value = captcha;
            document.getElementById('<%= entered_captcha.ClientID %>').value = "";
        };

        // FORM VALIDATION FUNCTION (SHOW ONLY 1 ERROR AT A TIME)
        const validateForm = (event) => {
            // Get form elements
            let agency = document.getElementById('<%= ddl_AgencyName.ClientID %>').value;
            let email = document.getElementById('<%= txt_Email.ClientID %>').value;
            let phone = document.getElementById('<%= txt_Phone.ClientID %>').value;
            let enteredCaptcha = document.getElementById('<%= entered_captcha.ClientID %>').value;

            // AGENCY VALIDATION (Check first)
            if (agency === "") {
                iziToast.error({
                    title: 'Validation Error',
                    message: "⚠️ Please select an agency.",
                    position: 'bottomRight'
                });
                event.preventDefault(); // Stop form submission
                return false;
            }

            // EMAIL VALIDATION
            let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            if (!emailRegex.test(email)) {
                iziToast.error({
                    title: 'Validation Error',
                    message: "⚠️ Please enter a valid email address.",
                    position: 'bottomRight'
                });
                event.preventDefault();
                return false;
            }

            // PHONE NUMBER VALIDATION (10-digit numeric)
            let phoneRegex = /^[0-9]{10}$/;
            if (!phoneRegex.test(phone)) {
                iziToast.error({
                    title: 'Validation Error',
                    message: "⚠️ Phone number must be exactly 10 digits.",
                    position: 'bottomRight'
                });
                event.preventDefault();
                return false;
            }

            // CAPTCHA VALIDATION
            if (enteredCaptcha !== captcha) {
                iziToast.error({
                    title: 'Validation Error',
                    message: "⚠️ Incorrect CAPTCHA entered.",
                    position: 'bottomRight'
                });
                generate(); // Refresh CAPTCHA
                event.preventDefault();
                return false;
            }

            return true; // Proceed with form submission if no errors
        };

        // PREVENT FORM SUBMISSION IF VALIDATION FAILS (ONLY ONE EVENT LISTENER)
        document.addEventListener("DOMContentLoaded", function () {
            let form = document.querySelector("form");

            // Remove existing event listeners to prevent duplicates
            form.removeEventListener("submit", validateForm);

            // Attach event listener once
            form.addEventListener("submit", validateForm);
        });
    </script>
</body>
</html>
