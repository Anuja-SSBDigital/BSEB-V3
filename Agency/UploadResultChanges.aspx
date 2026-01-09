<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="UploadResultChanges.aspx.cs" Inherits="Agency_UploadResultChanges" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12 col-md-12 col-lg-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>Upload Result Changes</h4>
                </div>
                <div class="card-body">
                    <div method="POST" class="needs-validation" novalidate="" onsubmit="return check();">
                        <div class="row">
                            <div class="col-md-12">
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="class">Class</label>
                                    <asp:DropDownList runat="server" ID="ddl_class" CssClass="form-control">
                                        <asp:ListItem Value="Intermediate">Intermediate</asp:ListItem>
                                    </asp:DropDownList>
                                    <div class="invalid-feedback">Please select a class</div>
                                </div>
                            </div>

                        </div>

                        <%--   <div class="row">
                           <div class="col-md-4">
                               <div class="form-group">
                                   <label for="faculty">Faculty</label>
                                   <asp:DropDownList runat="server" ID="ddl_Faculty" OnSelectedIndexChanged="ddl_Faculty_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control" ClientIDMode="Static">--%>
                        <%-- <asp:ListItem Value="">Select Faculty</asp:ListItem>
                                       <asp:ListItem Value="Arts">Arts</asp:ListItem>
                                       <asp:ListItem Value="Commerce">Commerce</asp:ListItem>
                                       <asp:ListItem Value="Science">Science</asp:ListItem>--%>
                        <%--  </asp:DropDownList>

                                   <div class="invalid-feedback">Please select a faculty</div>
                               </div>
                           </div>--%>

                        <%--<div class="col-md-4">
                               <div class="form-group">
                                   <label for="subject">Subject</label>
                                   <asp:DropDownList runat="server" ID="ddl_subject" OnSelectedIndexChanged="ddl_subject_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control" ClientIDMode="Static">
                                       <asp:ListItem Value="">Select Subject</asp:ListItem>--%>
                        <%--<asp:ListItem Value="Physics">Physics</asp:ListItem>
                                    <asp:ListItem Value="Chemistry">Chemistry</asp:ListItem>
                                    <asp:ListItem Value="Biology">Biology</asp:ListItem>--%>
                        <%-- </asp:DropDownList>
                                   <div class="invalid-feedback">Please select a subject</div>
                               </div>
                           </div>--%>
                        <%--<div class="col-md-4">
                               <div class="form-group">
                                   <label for="subject">Subject Code</label>
                                   <asp:TextBox runat="server" ID="txt_subjectcode" CssClass="form-control" Enabled="false"></asp:TextBox>
                                   <div class="invalid-feedback">Please select a subject</div>
                               </div>
                           </div>

                       </div>--%>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="file-upload">Enter Key for Upload</label>
                                    <div class="input-group mb-3">
                                        <asp:TextBox runat="server" ID="txt_pvtkey" CssClass="form-control" onblur="validateInput()" />
                                        <span id="keyError" style="color: red; display: none;">Private key is required.</span>
                                        <div class="input-group-append">
                                            <asp:Button runat="server" ID="btn_submittoken" CssClass="btn btn-primary" Text="Verify" OnClick="btn_submittoken_Click" />
                                        </div>
                                    </div>
                                    <asp:Label runat="server" ID="lbl_validate" class="font-18"></asp:Label>

                                </div>

                            </div>
                        </div>
                        <div class="row" runat="server" id="div_fileupload">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="file-upload">Upload Changes File (CSV/Excel)</label>
                                    <asp:FileUpload runat="server" ID="fl_file" class="form-control" onchange="checkFile()" />
                                    <div class="invalid-feedback">Please upload a valid file</div>
                                </div>
                            </div>
                        </div>
                        <div class="row" runat="server" id="div_remarks">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label for="file-upload">Enter Remarks</label>
                                   <asp:TextBox runat="server" id="txt_remarks" class="form-control" TextMode="Multiline"></asp:TextBox>
                                    <div class="invalid-feedback">Please upload a valid file</div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group text-center">
                            <asp:Button runat="server" ID="btn_submit" OnClick="btn_submit_Click" CssClass="btn btn-primary btn-lg" Text="Submit" OnClientClick="return validateForm();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        //function bindSubjects() {
        //    var facultyDropdown = document.getElementById("ddl_Faculty"); // Use exact ID since it's static
        //    var subjectDropdown = document.getElementById("ddl_subject");

        //    subjectDropdown.innerHTML = "<option value=''>Select Subject</option>";

        //    var subjects = {
        //        "Arts": ["History", "Political Science", "Geography", "Psychology", "Sociology", "Philosophy", "Economics", "Home Science", "Fine Arts"],
        //        "Commerce": ["Accountancy", "Business Studies", "Economics", "Mathematics", "English"],
        //        "Science": ["Physics", "Chemistry", "Biology", "Mathematics", "Computer Science", "Environmental Science"]
        //    };

        //    var faculty = facultyDropdown.value;

        //    console.log("Selected Faculty:", faculty); // Debugging

        //    if (subjects[faculty]) {
        //        subjects[faculty].forEach(subject => {
        //            var option = document.createElement("option");
        //            option.value = subject;
        //            option.text = subject;
        //            subjectDropdown.appendChild(option);
        //        });
        //        console.log("Subjects Added:", subjects[faculty]);
        //    } else {
        //        console.log("No subjects found for selected faculty");
        //    }
        //}



        function checkFile() {
            var fileInput = document.getElementById("<%= fl_file.ClientID %>");
            var filePath = fileInput.value;
            var allowedExtensions = /\.(csv|xlsx|xls|mdb|bak|zip)$/i;

            if (!allowedExtensions.exec(filePath)) {
                swal({
                    title: "Invalid File Type",
                    text: "Only CSV, XLS, XLSX, MDB, BAK, or ZIP files are allowed.",
                    icon: "error", // Error icon
                    button: "OK", // Custom button text
                });

                fileInput.value = ""; // Clear the file input
            }
        }

        //function submitKey() {
        //    var keyInput = document.getElementById("file-key").value;
        //    if (keyInput === "") {
        //        alert("Please enter a key to proceed.");
        //    } else {
        //        $('#keyModal').modal('hide');
        //    }

    </script>
</asp:Content>

