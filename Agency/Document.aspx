<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="Document2.aspx.cs" Inherits="Document2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row" runat="server" id="div_search">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
            <div class="card">        

                <div class="card-header">
                    <h4>Category Master</h4>
                </div>

                <div class="card-body">
                    <div class="row">

                        <div class="col-lg-4 col-md-6 col-sm-6 col-12 px-2" runat="server" id="Div_admin">
                            <h5 class="font-15"> Document Category</h5>
                            <asp:TextBox runat="server" ID="txtCategoryName" CssClass="form-control"
                                Placeholder="Document Category Name">
                            </asp:TextBox>          
                        </div>       

                        <div class="col-lg-2 col-md-2 col-sm-12 px-2">
                            <asp:Button runat="server" ID="btnAddCategory" OnClick="btnAddCategory_Click"
                                CssClass="btn btn-primary mt-4 " Text="Submit" />
                        </div>

                        <div class="col-12 mt-2">
                            <asp:Label ID="lblAddMessage" runat="server"
                                CssClass="text-success d-block mb-2"></asp:Label>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>