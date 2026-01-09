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
</head>

<body onload="generate()" style="background-image: url('assets/img/bg1.jpg'); background-position: center; background-size: cover; background-repeat: no-repeat; background-attachment: fixed;">
    <form method="POST" class="needs-validation" novalidate="" runat="server" onsubmit="return check();">
        <!-- <div class="square" style="--i:0;"></div>
    <div class="square" style="--i:1;"></div>
    <div class="square" style="--i:2;"></div>
    <div class="square" style="--i:3;"></div>
    <div class="square" style="--i:4;"></div>
    <div class="square" style="--i:5;"></div> -->
        <div id="app">
            <section class="section">
                <div class="container mt-5">
                    <div class="row">
                        <div class="col-12 col-lg-6 col-md-6 col-sm-8 offset-lg-3 offset-md-3 offset-sm-2">
                            <div class="card card-primary">
                                <div class="header-logo text-center mt-2">
                                    <%--<img alt="image" src="assets/img/bsebimage.jpg" class="header-logo" />--%>
                                    <img alt="image" src="assets/img/bsebimage.jpg" class="width-per-17" >
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
                                        <div class="input-group-prepend">
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
    </form>
</body>

<!-- auth-login.html  21 Nov 2019 03:49:32 GMT -->
</html>
