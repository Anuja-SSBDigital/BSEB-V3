<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>
<html lang="en">


<!-- auth-login.html  21 Nov 2019 03:49:32 GMT -->
<head>
    <meta charset="UTF-8">
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, shrink-to-fit=no" name="viewport">
    <title>BSEB- Login</title>
    <!-- General CSS Files -->
    <link rel="stylesheet" href="assets/css/app.min.css">
    <link rel="stylesheet" href="assets/bundles/bootstrap-social/bootstrap-social.css">
    <!-- Template CSS -->
    <link rel="stylesheet" href="assets/css/style.css">
    <link rel="stylesheet" href="assets/css/components.css">
    <!-- Custom style CSS -->
    <link rel="stylesheet" href="assets/css/custom.css">
    <link rel='shortcut icon' type='image/x-icon' href='assets/img/favicon_v1.png' />
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

        /* Full background layer */
        .network-bg {
            position: fixed;
            inset: 0;
            z-index: -1;
            background: radial-gradient(circle at 20% 30%, rgba(0, 255, 255, 0.15), transparent 40%), radial-gradient(circle at 80% 70%, rgba(0, 150, 255, 0.15), transparent 40%), linear-gradient(120deg, #0f2027, #203a43, #2c5364);
            overflow: hidden;
        }

            /* Connecting lines */
            .network-bg::before,
            .network-bg::after {
                content: "";
                position: absolute;
                inset: 0;
                background-image: linear-gradient(rgb(255 255 255 / 15%) 1px, transparent 1px), linear-gradient(90deg, rgb(255 255 255 / 13%) 1px, #00000000 1px);
                background-size: 80px 80px;
                animation: moveGrid 40s linear infinite;
            }

            .network-bg::after {
                opacity: 0.4;
                animation-duration: 70s;
            }



        /* Login card stays clean */
        .login-wrapper {
            position: relative;
            z-index: 2;
        }
    </style>
</head>

<body onload="generate()">
    <div class="network-bg"></div>
    <form method="POST" class="needs-validation" novalidate runat="server"
        onsubmit="return validateForm();">
        <!-- <div class="square" style="--i:0;"></div>
    <div class="square" style="--i:1;"></div>
    <div class="square" style="--i:2;"></div>
    <div class="square" style="--i:3;"></div>
    <div class="square" style="--i:4;"></div>
    <div class="square" style="--i:5;"></div> -->
        <div class="login-wrapper">
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

                                        <div class="form-group">
                                            <label for="Username">UserID</label>
                                            <asp:TextBox runat="server" ID="txt_UN" CssClass="form-control"></asp:TextBox>
                                            <%-- <asp:Textbox id="txt_UN" type="text" class="form-control" name="txt_UN" tabindex="1" required autofocus>--%>
                                            <div class="invalid-feedback">
                                                Please fill in your UserName
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="d-block">
                                                <label for="password" class="control-label">Password</label>
                                            </div>
                                            <asp:TextBox ID="txt_password" CssClass="form-control form-control" placeholder="Password" runat="server" Type="password"></asp:TextBox>
                                            <div class="invalid-feedback">
                                                please fill in your password
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <input type="text" id="entered-captcha" placeholder="Enter the captcha.." class="form-control">
                                        </div>
                                        <div class="input-group form-group">
                                            <div class="input-group-prepend" style="cursor: pointer;">
                                                <div class="input-group-text" onclick="generate()">
                                                    <i class="material-icons" id="iconuser">refresh</i>
                                                </div>
                                            </div>
                                            <input type="text" id="generated-captcha" class="form-control currency" readonly oncopy="return false;">
                                        </div>
                                        <%--  <div class="float-right">
                                        <a href="forgotpassword.php" class="float-right p-b-10">
                                            Forgot Password?
                                        </a>
                                    </div>--%>

                                        <div class="form-group">
                                            <%-- <asp:Button ID="btn_LoginUser" OnClick="btn_LoginUser_Click"
                                    CssClass="btn btn-primary btn-lg btn-block" Type="submit" tabindex="4"
                                    runat="server" Text="Login" />--%>
                                            <%-- <button type="submit" id="btn_Login" name="btn_Login" class="btn btn-primary btn-lg btn-block" tabindex="4" runat="server" onclick="btn_LoginUser_Click">
                                            Login
                                        </button>--%>
                                            <asp:Button runat="server" ID="btn_submit" Text="Submit" class="btn btn-primary btn-lg btn-block" OnClick="btn_submit_Click" />
                                        </div>

                                        <div class="text-center mt-4 mb-3">
                                            <div class="text-job text-muted">
                                                <a href="registerAgs.aspx" class="float-right p-b-10">Don't Have Account Register Here                                        </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </div>

        </div>
        <script src="assets/bundles/izitoast/js/iziToast.min.js"></script>
        <!-- Page Specific JS File -->
        <%--<script src="../assets/js/page/toastr.js"></script>--%>
        <script>
            let captcha;
            let alphabets = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz0123456789";

            console.log(alphabets.length);

            generate = () => {
                // console.log(status)
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
                // console.log(captcha);
                document.getElementById("generated-captcha").value = captcha;
                document.getElementById("entered-captcha").value = "";

            };

            check = () => {
                // console.log(status)
                let userValue = document.getElementById("entered-captcha").value;
                console.log(captcha);
                console.log(userValue);
                if (userValue == captcha) {
                    // window.location.href = "dashboard.php";
                    iziToast.success({
                        title: 'Captcha,',
                        message: 'Read Successfully',
                        position: 'bottomRight'
                    });
                    document.getElementById("entered-captcha").value = "";
                    return true;
                } else {
                    generate();
                    iziToast.error({
                        title: 'Something, Wrong !',
                        message: 'Pleace Enter Valid Captcha',
                        position: 'bottomRight'
                    });
                    return false;
                }
            };

            const captchaInput = document.getElementById('generated-captcha');
            captchaInput.addEventListener('copy', (event) => {
                event.preventDefault();
            });
        </script>
        <script>
            function validateForm() {
                const userInput = document.getElementById("entered-captcha").value.trim();
                const username = document.getElementById("<%= txt_UN.ClientID %>").value.trim();
                const password = document.getElementById("<%= txt_password.ClientID %>").value.trim();

                let isValid = true;
                let errorMessage = "";

                // Username validation
                if (!username) {
                    errorMessage += "Please enter UserID\n";
                    isValid = false;
                    highlightError("<%= txt_UN.ClientID %>");
                }

                // Password validation
                if (!password) {
                    errorMessage += "Please enter Password\n";
                    isValid = false;
                    highlightError("<%= txt_password.ClientID %>");
                }
                // Optional: You can add more password rules here
                // else if (password.length < 6) {
                //     errorMessage += "Password must be at least 6 characters\n";
                //     isValid = false;
                // }

                // Captcha validation
                if (!userInput) {
                    errorMessage += "Please enter Captcha\n";
                    isValid = false;
                    highlightError("entered-captcha");
                }
                else if (userInput !== captcha) {
                    errorMessage += "Invalid Captcha!\n";
                    isValid = false;
                    highlightError("entered-captcha");
                    generate(); // refresh captcha on wrong input
                }

                if (!isValid) {
                    iziToast.error({
                        title: 'Validation Error',
                        message: errorMessage,
                        position: 'bottomRight',
                        timeout: 5000
                    });
                    return false;
                }

                // If everything is okay → show success toast (optional)
                iziToast.success({
                    title: 'Success',
                    message: 'Validations passed... Logging in',
                    position: 'bottomRight'
                });

                return true; // allow form submission
            }

            // Helper function to highlight invalid fields
            function highlightError(elementId) {
                const field = document.getElementById(elementId);
                if (field) {
                    field.classList.add("is-invalid");
                    // Remove after 4 seconds (optional)
                    setTimeout(() => {
                        field.classList.remove("is-invalid");
                    }, 4000);
                }
            }
        </script>

    </form>
</body>

<!-- auth-login.html  21 Nov 2019 03:49:32 GMT -->
</html>
