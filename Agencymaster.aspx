<%@ Page Title="Add Agency User" Language="C#"MasterPageFile="~/Agency/MasterPage.master"AutoEventWireup="true"CodeFile="Agencymaster.aspx.cs"Inherits="Agency_Agencymaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">

        function validateForm() {

            var username = document.getElementById('<%=txtUsername.ClientID%>').value.trim();
        var email = document.getElementById('<%=txtEmail.ClientID%>').value.trim();
        var mobile = document.getElementById('<%=txtMobile.ClientID%>').value.trim();
        var agency = document.getElementById('<%=txtAgency.ClientID%>').value.trim();

            if (username === "") {
                alert("Please Enter Username");
                return false;
            }
            if (email === "") {
                alert("Please Enter Email");
                return false;
            }
            if (mobile === "") {
                alert("Please Enter Mobile number");
                return false;
            }
            if (agency === "") {
                alert("Please Enter Agency Name ");
                return false;
            }

            return true;
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="card card-primary">
        <div class="card-body">

            <h5>Add Agency User</h5>
            <hr />

            <div class="row">

                      <div class="col-md-6 mb-3">
          <label>Agency Name</label>
          <asp:TextBox ID="txtAgency" runat="server" CssClass="form-control" />
      </div>

                <div class="col-md-6 mb-3">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6 mb-3">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6 mb-3">
                    <label>Mobile No</label>
                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" />
                </div>

            </div>

               <div class="form-group text-center">
       <asp:Button ID="btnSave" runat="server" Text=" Save Agency"
           CssClass="btn btn-primary btn-lg"
           OnClientClick="return validateForm();"
           OnClick="btnSave_Click" />
   </div>

        </div>
    </div>

</asp:Content>
