<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="fileupload.aspx.cs" Inherits="fileupload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12 col-md-12 col-lg-12">
            <div class="card card-primary">
                <div class="card-header">     
                    File Upload
                </div>
                <div class="card-body">
                    <div method="POST" class="needs-validation" novalidate="" onsubmit="return check();">
                        <div class="row">
                            <div class="col-md-12">
                                <div class=" border-danger p-2 mb-2" style="background: #ff000026; border-radius: 5px;">
                                    <i class="fas fa-info-circle"></i>
                                    <strong>Note:</strong> Only files up to <strong>2GB</strong> in size can be uploaded.
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="class">Class</label>
                                    <asp:DropDownList runat="server" ID="ddl_class" CssClass="form-control">
                                        <asp:ListItem Value="Intermediate">Intermediate</asp:ListItem>
                                    </asp:DropDownList>
                                    <div class="invalid-feedback">Please select a class</div>
                                </div>
                            </div>

                           <div class="col-md-3">
                                <div class="form-group">
                                    <label for="Exam Session">Exam Session</label>
                                    <asp:DropDownList ID="ddl_Examsession" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="invalid-feedback">Please select a Session</div>
                                </div>
                            </div>


                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="category">Doc Category</label>
                                    <asp:DropDownList ID="ddl_doctype" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="invalid-feedback">Please select a category</div>
                                </div>
                            </div>

                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="category">Document Type</label>
                                    <asp:DropDownList ID="ddl_sub_doc_type" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </div>
                              
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="file-upload">Enter Key for Upload</label>
                                    <div class="input-group mb-3">
                                        <asp:TextBox runat="server" ID="txt_pvtkey" CssClass="form-control" onblur="validateInput()" />
                                        <span id="keyError" style="color: red; display: none;">Private key is required.</span>
                                        <div class="input-group-append">
                                            <asp:Button runat="server" ID="btn_submittoken" CssClass="btn btn-primary" OnClick="btn_submittoken_Click" Text="Verify" />
                                        </div>
                                    </div>
                                    <asp:Label runat="server" ID="lbl_validate" class="font-18"></asp:Label>

                                </div>

                            </div>
                        </div>
                        <div class="row" runat="server" id="div_fileupload">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="file-upload">Upload File (CSV/Excel)</label>
                                    <asp:FileUpload runat="server" ID="fl_file" class="form-control" onchange="checkFile()" />
                                    <div class="invalid-feedback">Please upload a valid file</div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group text-center">
                            <asp:Button runat="server" ID="btn_submit" CssClass="btn btn-primary btn-lg" Text="Submit" OnClick="btn_submit_Click" OnClientClick="return validateForm();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>     


        function validateForm() {   
            var ddlDoctype = document.getElementById('<%= ddl_doctype.ClientID %>').value;
            var ddlSubDocType = document.getElementById('<%= ddl_sub_doc_type.ClientID %>').value;

            var ddlExamSessionType = document.getElementById('<%= ddl_Examsession.ClientID %>').value;
               
            if (ddlDoctype === "ALL") {
                alert("Please select a valid Document Category.");
                return false;
            }

            if (ddlSubDocType === "ALL") {
                alert("Please select a valid File Type.");
                return false;
            }

            if (ddlFaculty === "") {
                alert("Please select a Faculty.");
                return false;
            }

            if (ddlSubject === "") {
                alert("Please select a Subject.");
                return false;
            }

            if (ddl_Examsession === "") {
                alert("Please select a Exam Session.");
                return false;
            }     
            return true;
        }

        function checkFile() {
            var fileInput = document.getElementById("<%= fl_file.ClientID %>");
            var filePath = fileInput.value;
            var allowedExtensions = /\.(csv|xlsx|xls|mdb|bak|zip)$/i;

            if (!allowedExtensions.exec(filePath)) {
                swal({
                    title: "Invalid File Type",
                    text: "Only CSV, XLS, XLSX, MDB, BAK, or ZIP files are allowed.",
                    icon: "error",
                    button: "OK",
                });

                fileInput.value = "";
            }
        }

    </script>

</asp:Content>