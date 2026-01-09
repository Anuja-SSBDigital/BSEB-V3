<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="owneragencydetails.aspx.cs" Inherits="Agency_owneragencydetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

   
    <div class="row" runat="server" id="div_search">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
            <div class="card">
                <div class="card-body">
                    <div class="row">

                      
                        <div class="col-lg-6 col-md-6 col-sm-6 col-12 px-2" runat="server" id="Div_admin">
                            <h5 class="font-15">Select Agency</h5>
                            <h2 class="mb-3 font-18">
                                <asp:Label runat="server" ID="Label1"></asp:Label>
                            </h2>
                            <asp:DropDownList runat="server" ID="ddl_AgencyName" CssClass="form-control" Required="true">
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
                        </div>

                    </div>

                    <div class="card-footer text-end">
                        <asp:Button runat="server" ID="btnsearch" OnClick="btnsearch_Click" CssClass="btn btn-primary" Text="Search" />
                        <asp:Label ID="lblMessage" runat="server" CssClass="text-info d-block mb-2"></asp:Label>

                    </div>

                </div>
            </div>
        </div>
    </div>

   
    <div class="row mt-3">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <div class="card-header">
                    <h4>Agency Wise Access Data</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-1">
                            <thead>
                                <tr>
                                    <th>Sr.No.</th>
                                    <th>Owner Agency</th>
                                    <th>Viewer Agency</th>
                                    <th>Document Type</th>
                                </tr>
                            </thead>
                            <tbody>
                               <asp:Repeater runat="server" ID="rpt_Agencywisedata">

                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%# Eval("OwnerAgency") %></td>
                                            <td><%# Eval("ViewerAgency") %></td>
                                            <td><%# Eval("DocumentType") %></td>
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

</asp:Content>
