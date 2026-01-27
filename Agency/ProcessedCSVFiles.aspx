<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="ProcessedCSVFiles.aspx.cs" Inherits="ProcessedCSVFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .nav-pills .nav-link {
            color: #cbd5e1;
            border-radius: 0.5rem;
        }

            .nav-pills .nav-link.active {
                background-color: #135c99; /* teal */
                color: white;
            }

            .nav-pills .nav-link:hover {
                background-color: rgba(14, 165, 233, 0.15);
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="row">
        <div class="col-12">
            <div class="card" runat="server" id="CrossFiles_All" visible="true">
                <div class="card-header text-center">
                    <h5>Files Available for Download</h5>
                </div>
                <div class="card-body">

                    <!-- Bootstrap Tabs -->
                    <ul class="nav nav-pills" id="crossFileTabs" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="Mapple-tab" data-toggle="tab" href="#mapple" role="tab">Mapple</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="datacon-tab" data-toggle="tab" href="#datacon" role="tab">Datacon</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="kids-tab" data-toggle="tab" href="#kids" role="tab">Kids</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="mcrk-tab" data-toggle="tab" href="#mcrk" role="tab">MCRK</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="CharuMindworks-tab" data-toggle="tab" href="#Charu_Mindworks" role="tab">Charu Mindworks</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="Shree_Jagannath_UdyogagAG-tab" data-toggle="tab" href="#Shree_Jagannath_UdyogagAG" role="tab">Shree Jagannath Udyog</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="Hitech-tab" data-toggle="tab" href="#Hitech_ag" role="tab">Hitech</a>
                        </li>
                      <%--  <li class="nav-item">
                            <a class="nav-link" id="Keltron-tab" data-toggle="tab" href="#KeltronAG" role="tab">Keltron</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="Datasoft-tab" data-toggle="tab" href="#Datasoft_ag" role="tab">Datasoft</a>
                        </li>--%>
                        <li class="nav-item">
                            <a class="nav-link" id="Antier-tab" data-toggle="tab" href="#Antier_ag" role="tab">Antier</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="SSBDigital-tab" data-toggle="tab" href="#SSBDigital_ag" role="tab">SSB Digital</a>
                        </li>
                    </ul>

                    <!-- Tab Content -->
                    <div class="tab-content mt-3" id="crossFileTabsContent">
                        <div class="tab-pane fade show active" id="mapple" role="tabpanel">
                            <asp:Repeater ID="rptMapple" runat="server" OnItemDataBound="rptMapple_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                                                                        <%--<asp:Button ID="btnDownload" runat="server" CssClass="btn btn-primary btn-sm"
                                                Text="Download" CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click" />--%>
                                            <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>

                                        </td>


                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Datacon Tab -->
                        <div class="tab-pane fade" id="datacon" role="tabpanel">
                            <asp:Repeater ID="rptDatacon" runat="server" OnItemDataBound="rptDatacon_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                             <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Kids Tab -->
                        <div class="tab-pane fade" id="kids" role="tabpanel">
                            <asp:Repeater ID="rptKids" runat="server" OnItemDataBound="rptKids_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                             <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- MCRK Tab -->
                        <div class="tab-pane fade" id="mcrk" role="tabpanel">
                            <asp:Repeater ID="rptMCRK" runat="server" OnItemDataBound="rptMCRK_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                              <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <!-- Charu Mindworks Tab -->
                        <div class="tab-pane fade" id="Charu_Mindworks" role="tabpanel">
                            <asp:Repeater ID="RepeaterCharu_Mindworks" runat="server" OnItemDataBound="RepeaterCharu_Mindworks_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                              <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <!-- Shree_Jagannath_Udyogag Tab -->
                        <div class="tab-pane fade" id="Shree_Jagannath_UdyogagAG" role="tabpanel">
                            <asp:Repeater ID="RepeaterShree_Jagannath_Udyog" runat="server" OnItemDataBound="RepeaterShree_Jagannath_Udyog_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                              <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="tab-pane fade" id="Hitech_ag" role="tabpanel">
                            <asp:Repeater ID="RepeaterHitech" runat="server" OnItemDataBound="RepeaterHitech_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                              <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    <%--    <div class="tab-pane fade" id="KeltronAG" role="tabpanel">
                            <asp:Repeater ID="RepeaterKeltronAG" runat="server" OnItemDataBound="RepeaterKeltronAG_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                             <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="tab-pane fade" id="Datasoft_ag" role="tabpanel">
                            <asp:Repeater ID="RepeaterDatasoft_ag" runat="server" OnItemDataBound="RepeaterDatasoft_ag_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                             <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>--%>
                        <div class="tab-pane fade" id="Antier_ag" role="tabpanel">
                            <asp:Repeater ID="RepeaterAntier_ag" runat="server" OnItemDataBound="RepeaterAntier_ag_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                              <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
 </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="tab-pane fade" id="SSBDigital_ag" role="tabpanel">
                            <asp:Repeater ID="RepeaterSSBDigital_ag" runat="server" OnItemDataBound="RepeaterSSBDigital_ag_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>File Name</th>
                                                <th>Download</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("fileName") %></td>
                                        <td>
                                            <asp:HiddenField runat="server" ID="hf_id" Value='<%# Eval("id") %>' />
                                             <asp:LinkButton
                                                ID="btnDownload"
                                                runat="server"
                                                CssClass="btn btn-icon icon-left btn-success"
                                                CommandArgument='<%# Eval("filePath") %>'
                                                OnClick="btnDownload_Click">
    <i class="fa fa-download"></i> 
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
</table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
