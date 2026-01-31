<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UploadedFilesReport.aspx.cs" Inherits="UploadedFilesReport" %>

<!DOCTYPE html>
<html lang="en">


<!-- auth-reset-password.html  21 Nov 2019 04:05:02 GMT -->
<head>
    <meta charset="UTF-8">
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, shrink-to-fit=no" name="viewport">
    <title>BSEB - Admin Dashboard</title>
    <!-- General CSS Files -->
    <link rel="stylesheet" href="assets/css/app.min.css">
    <!-- Template CSS -->
    <link rel="stylesheet" href="assets/css/style.css">
    <link rel="stylesheet" href="assets/css/components.css">
    <!-- Custom style CSS -->
    <link rel="stylesheet" href="assets/css/custom.css">
    <link rel='shortcut icon' type='image/x-icon' href='../assets/img/favicon_v1.png' />
    <link rel="stylesheet" href="../assets/bundles/datatables/datatables.min.css">
    <link rel="stylesheet" href="../assets/bundles/datatables/DataTables-1.10.16/css/dataTables.bootstrap4.min.css">
    <style>
        body {
            background: radial-gradient(circle at top left, #2c4a9c 0%, #142b5e 70%);
        }
    </style>
</head>

<body>
    <div class="loader"></div>
    <form id="form1" runat="server">

        <div id="app">
            <section class="section">
                <div class="container mt-5">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="card card-primary">
                                <div class="card-header">
                                    <h4>Search File Upload Details</h4>
                                </div>
                                <div class="card-body">

                                    <div class="row" runat="server" id="div_search">


                                        <div class="col-md-6 " runat="server" id="Div_admin">
                                            <h5 class="font-15">Select Agency</h5>
                                            <h2 class="mb-3 font-18">
                                                <asp:Label runat="server" ID="Label1"></asp:Label>
                                            </h2>
                                            <asp:DropDownList runat="server" ID="ddl_AgencyName" CssClass="form-control">
                                                <asp:ListItem Value="" Text="Select Agency Name" Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="Hitech" Text="Hitech"></asp:ListItem>
                                                <asp:ListItem Value="Datacon" Text="Datacon"></asp:ListItem>
                                                <asp:ListItem Value="Charu Mindworks" Text="Charu Mindworks"></asp:ListItem>
                                                <asp:ListItem Value="MCRK" Text="MCRK"></asp:ListItem>
                                                <asp:ListItem Value="Mapple" Text="Mapple"></asp:ListItem>
                                                <asp:ListItem Value="Kids" Text="Kids"></asp:ListItem>
                                                <asp:ListItem Value="Antier" Text="Antier"></asp:ListItem>
                                                <asp:ListItem Value="SSB Digital" Text="SSB Digital"></asp:ListItem>
                                            </asp:DropDownList>
                                            <small id="passwordHelpBlock" class="form-text text-muted">Fileformats : Agency_Board_doctype_Subdoctype_timestamp 
                                        </small>
                                        </div>
                                        
                                    </div>



                                </div>
                                <div class="card-footer text-end">
                                    <asp:Button runat="server" ID="btnsearch" OnClick="btn_Search_Click" CssClass="btn btn-primary" Text="Search" />
                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-info d-block mb-2"></asp:Label>

                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="mt-3" runat="server" id="Agency_detailes" visible="false">

                                <div class="card card-primary">
                                    <div class="card-header">
                                        <h4>File Upload Details</h4>
                                    </div>
                                    <div class="card-body">
                                        <div class="table-responsive">
                                            <table class="table table-striped" id="table-1">
                                                <thead>
                                                    <tr>
                                                        <th>Sr.No.</th>
                                                        <th>Actual File Name</th>
                                                        <th>File Name</th>
                                                        <th>Remarks</th>
                                                        <th>Upload Date</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater runat="server" ID="rpt_Agencywisedata">

                                                        <ItemTemplate>
                                                            <tr>
                                                                <td><%# Container.ItemIndex + 1 %></td>
                                                                <td><%# Eval("actualfilename") %></td>
                                                                <td><%# Eval("filename") %></td>
                                                                <td><%# Eval("Remarks") %></td>
                                                                <td><%# Eval("createddate") %></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>


            </section>
        </div>
    </form>
    <!-- General JS Scripts -->
    <script src="assets/js/app.min.js"></script>
    <!-- JS Libraies -->
    <!-- Page Specific JS File -->
    <!-- Template JS File -->
    <script src="assets/js/scripts.js"></script>
    <!-- Custom JS File -->
    <script src="assets/js/custom.js"></script>
    <script src="../assets/bundles/datatables/datatables.min.js"></script>
    <script src="../assets/bundles/datatables/DataTables-1.10.16/js/dataTables.bootstrap4.min.js"></script>
    <script src="../assets/bundles/jquery-ui/jquery-ui.min.js"></script>
    <script src="../assets/js/page/datatables.js"></script>

</body>


<!-- auth-reset-password.html  21 Nov 2019 04:05:02 GMT -->
</html>
