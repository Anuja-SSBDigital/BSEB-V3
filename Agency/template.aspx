<%@ Page Title="" Language="C#" MasterPageFile="~/Agency/MasterPage.master" AutoEventWireup="true" CodeFile="template.aspx.cs" Inherits="Agency_template" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-12">
            <div class="card card-primary">
                <div class="card-header">
                    <h4>Download Template </h4>
                </div>
                <div class="card-body">
                    <div class="row">

                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/AWARD_SAMPLE_RAW_DATA.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Award Sample Data File</a>

                            </div>
                        </div>
                        <%--  <div class="col-md-6">
                         <div class="form-group">
                             <a href="../Template/PRACTICAL_SAMPLE_RAW_DATA.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download>
                                 <i class="fas fa-download"></i>Practical Sample Data File
                             </a>
                         </div>
                     </div>--%>

                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/OMR_SAMPLE_RAW_DATA.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>OMR Sample Data File </a>

                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/FOIL_SAMPLE_RAW_DATA.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Foil Sample Data File</a>

                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/Absentee_sample_data.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Absentee Sample Data File</a>

                            </div>
                        </div>
                        <%--  <div class="col-md-6">
                         <div class="form-group">
                             <a href="../Template/Attendance_sample_data.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Attendance Sample Data File</a>

                         </div>
                     </div>--%>
                        <%-- <div class="col-md-6">
                         <div class="form-group">
                             <a href="../Template/Flying_Sample_ Data_File.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Flying Slip Sample Data File</a>

                         </div>
                     </div>--%>
                        <%--  <div class="col-md-6">
                         <div class="form-group">
                             <a href="../Template/Attendance_A_Sample_ Data_File.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Attendance A Sample Data File</a>

                         </div>
                     </div>--%>
                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/Attendance_B_Sample_ Data_File.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Attendance B Sample Data File</a>

                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/Litho_Sample_Template.xlsx" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Litho Sample Data File</a>

                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <a href="../Template/Final_Input_Sample_Template.rar" class="btn btn-icon icon-left btn-primary btn-block" download><i class="fas fa-download"></i>Final Input Sample Data Files</a>

                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

