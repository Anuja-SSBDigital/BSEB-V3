<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row" runat="server" id="div_search">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
            <div class="card">
                <!-- Card Body -->
                <div class="card-body">
                    <div class="row">
                        <!-- Select Agency -->
                        <div class="col-lg-6 col-md-6 col-sm-6 col-12 px-2" runat="server" id="Div_admin">
                            <h5 class="font-15">Select Agency</h5>
                            <h2 class="mb-3 font-18">
                                <asp:Label runat="server" ID="Label1"></asp:Label>
                            </h2>
                            <asp:DropDownList runat="server" ID="ddl_AgencyName" CssClass="form-control" Required="true">
                                <asp:ListItem Value="" Text="Select Agency Name" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="Mapple" Text="Mapple"></asp:ListItem>
                                <asp:ListItem Value="Datacon" Text="Datacon"></asp:ListItem>
                                <asp:ListItem Value="Kids" Text="Kids"></asp:ListItem>
                                <asp:ListItem Value="MCRK" Text="MCRK"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <!-- Select Date -->
                        <div class="col-lg-6 col-md-6 col-sm-6 col-12 px-2" runat="server" id="Div_ags">
                            <h5 class="font-15">Select Date</h5>
                            <h2 class="mb-3 font-18">
                                <asp:Label runat="server" ID="Label3"></asp:Label>
                            </h2>
                            <asp:TextBox runat="server" ID="txt_date" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Card Footer (Search Button) -->
                <div class="card-footer text-end">
                    <asp:Button runat="server" ID="btnsearch" OnClick="btnsearch_Click" CssClass="btn btn-primary" Text="Search" />
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
                <div class="card-statistic-4">
                    <div class="align-items-center justify-content-between">

                        <div class="row ">
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                                <div class="card-content">
                                    <h5 class="font-15">Daily Processed Data</h5>
                                    <%--<p class="mb-0"><span class="col-green">Today</span></p>--%>
                                    <h2 class="mb-3 font-18" id="dailyUploads">
                                        <asp:Label runat="server" ID="lbl_dailyprocddata"></asp:Label></h2>

                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                                <div class="banner-img">
                                    <img src="../assets/img/banner/1.png" alt="">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
                <div class="card-statistic-4">
                    <div class="align-items-center justify-content-between">
                        <div class="row ">
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                                <div class="card-content">
                                    <h5 class="font-15">Total Data Processed</h5>
                                    <h2 class="mb-3 font-18" id="totalFilesProcessed">
                                        <asp:Label runat="server" ID="lbl_totaldata"></asp:Label></h2>
                                    <%--  <p class="mb-0"><span class="col-blue">Cumulative</span></p>--%>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                                <div class="banner-img">
                                    <img src="../assets/img/banner/4.png" alt="">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function updateDashboard(daily, successRate, warning, totalProcessed) {
            document.getElementById("dailyUploads").innerText = daily;
            document.getElementById("uploadSuccessRate").innerText = successRate + "%";
            document.getElementById("warningData").innerText = warning;
            document.getElementById("totalFilesProcessed").innerText = totalProcessed;
        }
    </script>

    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>OMR Sheet Data</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr No</th>
                                    <th>File Type</th>
                                    <th>Total Correct Rows</th>
                                    <th>Duplicate/Dicrepency Entries</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rpt_FileuploadCsvData" OnItemCommand="rpt_FileuploadCsvData_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblRowNumber" runat="server" Text='<%# Container.ItemIndex + 1 %>' />
                                            </td>
                                            <td>OMR Sheet</td>
                                            <td><%# Eval("OmrCount") %></td>
                                            <td><%# Eval("OmrDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAbsenteeSheet" runat="server" Text='2' />
                                            </td>
                                            <td>Absentee Sheet</td>
                                            <td><%# Eval("AbsenteeCount") %></td>
                                            <td><%# Eval("AbsenteeDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblFoilSheet" runat="server" Text='3' />
                                            </td>
                                            <td>Foil Sheet</td>
                                            <td><%# Eval("FoilCount") %></td>
                                            <td><%# Eval("FoilDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAwardSheet" runat="server" Text='4' />
                                            </td>
                                            <td>Award Sheet</td>
                                            <td><%# Eval("AwardCount") %></td>
                                            <td><%# Eval("AwardDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPracticalSheet" runat="server" Text='5' />
                                            </td>
                                            <td>Practical Sheet</td>
                                            <td><%# Eval("PracticalCount") %></td>
                                            <td><%# Eval("PracticalDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAttendanceSheet" runat="server" Text='6' />
                                            </td>
                                            <td>Attendance Sheet</td>
                                            <td><%# Eval("AttendanceCount") %></td>
                                            <td><%# Eval("AttendanceDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAttendanceASheet" runat="server" Text='7' />
                                            </td>
                                            <td>Attendance A Sheet</td>
                                            <td><%# Eval("AttendanceACount") %></td>
                                            <td><%# Eval("AttendanceADuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAttendanceBSheet" runat="server" Text='8' />
                                            </td>
                                            <td>Attendance B Sheet</td>
                                            <td><%# Eval("AttendanceBCount") %></td>
                                            <td><%# Eval("AttendanceBDuplicateCount") %></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblFlyingSlipSheet" runat="server" Text='9' />
                                            </td>
                                            <td>Flying Slip Sheet</td>
                                            <td><%# Eval("FlyingCount") %></td>
                                            <td><%# Eval("FlyingDuplicateCount") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <%--<div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>Practical Sheet Data</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table_Practical">
                            <thead>
                                <tr>
                                    <th>Sr No</th>
                                    <th>File Type</th>
                                    <th>Correct  Rows</th>

                                    <th>Duplicate Entries</th>

                                </tr>
                            </thead>
                            <tbody>--%>

        <%-- <asp:Repeater runat="server" ID="Repeater_practical" OnItemCommand="Repeater_practical_ItemCommand">
                                    <ItemTemplate>--%>
        <%--  <tr>
                                    <td>
                                        <asp:Label ID="lblRowNumber1" Text='1' runat="server" />
                                    </td>
                                    <td>Practical Sheet</td>

                                    <td>0</td>
                                    <td>0</td>

                                </tr>--%>
        <%-- </ItemTemplate>
                                </asp:Repeater>--%>
        <%--</tbody>
                        </table>
                    </div>


                </div>
            </div>
        </div>--%>
        <%--  <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>Award Sheet and Foil Sheet Data</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table_Award_Foil">
                            <thead>
                                <tr>
                                    <th>Sr No</th>
                                    <th>File Type</th>

                                    <th>Total Awardsheet Correct Rows</th>
                                    <th>Total Foil Sheet  Correct Rows</th>
                                    <th>Awardsheet Duplicate Entries</th>
                                    <th>Foil Duplicate Entries</th>
                                </tr>
                            </thead>
                            <tbody>--%>

        <%--   <asp:Repeater runat="server" ID="Repeater_FoilAwardSheet" OnItemCommand="Repeater_FoilAwardSheet_ItemCommand">
                                    <ItemTemplate>--%>
        <%--<tr>

                                    <td>
                                        <asp:Label ID="Label2" Text='1' runat="server" />
                                    </td>
                                    <td>Foil Sheet</td>

                                    <td>0</td>
                                    <td>0</td>
                                    <td>0</td>
                                    <td>0</td>

                                </tr>--%>
        <%--    </ItemTemplate>
                                </asp:Repeater>--%>
        <%--</tbody>
                        </table>
                    </div>


                </div>
            </div>
        </div>--%>
    </div>
</asp:Content>

