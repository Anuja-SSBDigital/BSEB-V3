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
     <div class=" border-danger p-2 mb-2" style="background: #ff000026;border-radius: 5px;">
         <i class="fas fa-info-circle"></i>
         <strong>Note:</strong> Only files up to <strong>2GB</strong> in size can be uploaded.
     </div>
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
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="category">Doc Category</label>
                                    <asp:DropDownList ID="ddl_doctype" runat="server" CssClass="form-control" name="category">
                                       <asp:ListItem Value="ALL">Select Doc Category</asp:ListItem>
									   <asp:ListItem Value="BBOSE">BBOSE</asp:ListItem>
									   <asp:ListItem Value="BBOSE Master Data">BBOSE Master Data</asp:ListItem>
									   <asp:ListItem Value="BBOSE Collison">BBOSE Collison</asp:ListItem>
									   <asp:ListItem Value="DMS_DATA">DMS_DATA</asp:ListItem>
        <%--   <asp:ListItem Value="Practical">Practical</asp:ListItem>
         <asp:ListItem Value="Theory">Theory</asp:ListItem>
         <asp:ListItem Value="Internal">Internal</asp:ListItem>
         <asp:ListItem Value="Master Data">Master Data</asp:ListItem>
         <asp:ListItem Value="Final Data">Final Data</asp:ListItem>
         <asp:ListItem Value="Result">Result</asp:ListItem>
         <asp:ListItem Value="Input Matching">Input Matching</asp:ListItem>--%>
                                    </asp:DropDownList>
                                    <div class="invalid-feedback">Please select a category</div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="category">Document Type</label>
                                    <asp:DropDownList ID="ddl_sub_doc_type" runat="server" CssClass="form-control" name="category">
                                        <asp:ListItem Value="ALL">Select File Type</asp:ListItem>
										 <asp:ListItem Value="BBOSE_FINAL_RESULT">BBOSE_FINAL_RESULT</asp:ListItem>
										 <asp:ListItem Value="BBOSE_MISMATCH_RESULT">BBOSE_MISMATCH_RESULT</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_EXPULSION">BBOSE_THEORY_EXPULSION</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_FLY">BBOSE_THEORY_FLY</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_AWARD">BBOSE_THEORY_AWARD</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_FOIL">BBOSE_THEORY_FOIL</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_OMR">BBOSE_THEORY_OMR</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_ONLINE_MARKS">BBOSE_THEORY_ONLINE_MARKS</asp:ListItem>
										 <asp:ListItem Value="BBOSE_THEORY_ATTENDANCE">BBOSE_THEORY_ATTENDANCE</asp:ListItem>
										 <asp:ListItem Value="BBOSE_PRACTICAL_AWARD">BBOSE_PRACTICAL_AWARD</asp:ListItem>
										 <asp:ListItem Value="BBOSE_PRACTICAL_FOIL">BBOSE_PRACTICAL_FOIL</asp:ListItem>
										 <asp:ListItem Value="BBOSE_PRACTICAL_ATTENDANCE">BBOSE_PRACTICAL_ATTENDANCE</asp:ListItem>
										 <asp:ListItem Value="BBOSE_ADMITCARD_MASTER_DATA">BBOSE_ADMITCARD_MASTER_DATA</asp:ListItem>
										 <asp:ListItem Value="BBOSE_OLDRESULT_DATA">BBOSE_OLDRESULT_DATA</asp:ListItem>
										 <asp:ListItem Value="BBOSE_COPY_PRINTING_DATA">BBOSE_COPY_PRINTING_DATA</asp:ListItem>
										 <asp:ListItem Value="BBOSE_FLY_WANTING">BBOSE_FLY_WANTING</asp:ListItem>
										 <asp:ListItem Value="BBOSE_SUBJECTIVE_WANTING">BBOSE_SUBJECTIVE_WANTING</asp:ListItem>
										 <asp:ListItem Value="BBOSE_OBJECTIVE_WANTING">BBOSE_OBJECTIVE_WANTING</asp:ListItem>
										 <asp:ListItem Value="BBOSE_PRACTICAL_WANTING">BBOSE_PRACTICAL_WANTING</asp:ListItem>
										  <asp:ListItem Value="DMS_COMPART_2025">DMS_COMPART_2025</asp:ListItem>
										  <asp:ListItem Value="DMS_SPECIAL_2025">DMS_SPECIAL_2025</asp:ListItem>
										 
                                       <%--   <asp:ListItem Value="Final DMS Data">Final DMS Data</asp:ListItem>
										
                                          <asp:ListItem Value="Final Result Data">Final Result Data</asp:ListItem>
                                        <asp:ListItem Value="Final OMR Sheet">Final OMR Sheet</asp:ListItem>
                                        <asp:ListItem Value="Final Foil Sheet">Final Foil Sheet</asp:ListItem>
                                        <asp:ListItem Value="Final Attendance B">Final Attendance B</asp:ListItem>
                                        <asp:ListItem Value="Final Absentee Sheet">Final Absentee Sheet</asp:ListItem>
                                        <asp:ListItem Value="Final Award Sheet">Final Award Sheet</asp:ListItem>
                                         <asp:ListItem Value="Final Flying Sheet">Final Flying Sheet</asp:ListItem>
                                          <asp:ListItem Value="Final Attendance A">Final Attendance A</asp:ListItem>
                                        <asp:ListItem Value="Final Online Data">Final Online Data</asp:ListItem>

                                        <asp:ListItem Value="Wanting OMR Sheet">Wanting OMR Sheet</asp:ListItem>
                                        <asp:ListItem Value="Wanting Foil Sheet">Wanting Foil Sheet</asp:ListItem>
                                        <asp:ListItem Value="Wanting Attendance B">Wanting Attendance B</asp:ListItem>
                                        <asp:ListItem Value="Wanting Absentee Sheet">Wanting Absentee Sheet</asp:ListItem>
                                        <asp:ListItem Value="Wanting Award Sheet">Wanting Award Sheet</asp:ListItem>
                                         <asp:ListItem Value="Wanting Flying Sheet">Wanting Flying Sheet</asp:ListItem>
                                        <asp:ListItem Value="Wanting Litho Sheet">Wanting Litho Sheet</asp:ListItem>
                                        <asp:ListItem Value="Wanting Online Data">Wanting Online Data</asp:ListItem>

                                        <asp:ListItem Value="Process OMR Sheet">Process OMR Sheet</asp:ListItem>
                                        <asp:ListItem Value="Process Foil Sheet">Process Foil Sheet</asp:ListItem>
                                        <asp:ListItem Value="Process Attendance B">Process Attendance B</asp:ListItem>
                                        <asp:ListItem Value="Process Absentee Sheet">Process Absentee Sheet</asp:ListItem>
                                        <asp:ListItem Value="Process Award Sheet">Process Award Sheet</asp:ListItem>
                                        <asp:ListItem Value="Process Online Data">Process Online Data</asp:ListItem>
                                        <asp:ListItem Value="Admit Card Master">Admit Card Master</asp:ListItem>
                                        <asp:ListItem Value="Bottom BARCODE Master">Bottom BARCODE Master</asp:ListItem>
                                        <asp:ListItem Value="3 year Carry Data">3 year Carry Data</asp:ListItem>
                                         <asp:ListItem Value="Practical Data Structure">Practical Data Structure</asp:ListItem>
                                        <asp:ListItem Value="Theory Data Structure">Theory Data Structure</asp:ListItem>
                                        <asp:ListItem Value="Final Data">Final Data</asp:ListItem>
                                        <asp:ListItem Value="Topper">Topper</asp:ListItem>
                                        <asp:ListItem Value="Missmatched">Missmatched</asp:ListItem>
                                        <asp:ListItem Value="Flying Sheet">Flying Sheet</asp:ListItem>
                                        <asp:ListItem Value="OMR Sheet">OMR Sheet</asp:ListItem>
                                        <asp:ListItem Value="Award Sheet">Award Sheet</asp:ListItem>
                                          <asp:ListItem Value="Practical Award Sheet">Practical Award Sheet</asp:ListItem>
                                        <asp:ListItem Value="Absentee Sheet">Absentee Sheet</asp:ListItem>
                                        <asp:ListItem Value="Theory Attendance Sheet">Theory Attendance Sheet</asp:ListItem>
                                           <asp:ListItem Value="Practical Attendance Sheet">Practical Attendance Sheet</asp:ListItem>
                                        <asp:ListItem Value="PracticalMarks">PracticalMarks</asp:ListItem>
                                        <asp:ListItem Value="Sample Result Data">Sample Result Data</asp:ListItem>
                                          <asp:ListItem Value="Scrutiny Result Data">Scrutiny Result Data</asp:ListItem>
                                         <asp:ListItem Value="Scrutiny Print Master">Scrutiny Print Master</asp:ListItem>
                                         <asp:ListItem Value="Scrutiny Theory Award">Scrutiny Theory Award</asp:ListItem>
 <asp:ListItem Value="Admit Card">Admit Card</asp:ListItem>--%>
                                        <%--<asp:ListItem Value="Practical Sheet">Practical Sheet</asp:ListItem>

                                      
                                        <asp:ListItem Value="Award Sheet">Award Sheet</asp:ListItem>--%>
                                        <%--   <asp:ListItem Value="Practical-Standard Marks Foil">Practical-Standard Marks Foil</asp:ListItem>--%>
                                        <%--<asp:ListItem Value="Absentee">Absentee</asp:ListItem>
                                        <asp:ListItem Value="Attendance">Attendance</asp:ListItem>
                                        <asp:ListItem Value="Flying Slip">Flying Slip</asp:ListItem>
                                        <asp:ListItem Value="Attendance A">Attendance A</asp:ListItem>

                                        <asp:ListItem Value="Attendance B">Attendance B</asp:ListItem>--%>
                                    </asp:DropDownList>
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

        function validateForm() {
            var ddlDoctype = document.getElementById('<%= ddl_doctype.ClientID %>').value;
            var ddlSubDocType = document.getElementById('<%= ddl_sub_doc_type.ClientID %>').value;


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


    <!-- Include Bootstrap for Modal functionality -->


</asp:Content>

