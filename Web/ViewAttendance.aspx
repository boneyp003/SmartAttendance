<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/global.Master" CodeBehind="ViewAttendance.aspx.cs" Inherits="gradproj_webapp.ViewAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .otptextarea {
            font-size : xx-large;
        }
        .pre-scrollable {
            overflow-y: scroll;
            border : thin;
        }

    </style>


    <script src="js/Home.js" type="text/javascript"></script>
    <script src="js/Scripts.js" type="text/javascript"></script>
    <script src="js/ViewAttendance.js" type="text/javascript"></script>

    <script type="text/javascript">
        var Profile_Object;
        var Course_Object;
        var Student_Object;
        var Attendance_Object;

        $(document).ready(function () {

            var stack = "(ViewAttendanceASPX)documentready() --> ";
            try {
                initiateViewAttendance(stack);
            }
            catch (ex) {
                appendLog(stack, ex);
            }


        });

    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--<div class="container">
    <div class="container-fluid">
      <h1>Hello World!</h1>
      <p>Resize the browser window to see the effect.</p>
      <p>The columns will automatically stack on top of each other when the screen is less than 768px wide.</p>
      <div class="row">
        <div class="col-sm-4" style="background-color:lavender;">.col-sm-4</div>
        <div class="col-sm-4" style="background-color:lavenderblush;">.col-sm-4</div>
        <div class="col-sm-4" style="background-color:lavender;">.col-sm-4</div>
      </div>
    </div>-->
    <div id="ControlsContainer" style="height:200px;">
        <div id="range_control" class="btn-group" role="group" aria-label="Basic example">
            <input id="btn_date" type="button" class="btn btn-secondary border-dark" value="Date" onclick="rangeSelectorClicked('date');" />
            <input id="btn_month" type="button" class="btn btn-secondary border-dark" value="Month" onclick="rangeSelectorClicked('month');" hidden />
            <input id="btn_semester" type="button" class="btn btn-secondary border-dark" value="Semester" onclick="rangeSelectorClicked('semester');" />
        </div>
        <br />
        <div class="d-inline-flex w-75 pb-2 pt-3">
            <div id="class_control" class="input-group pr-3">
                <div class="input-group-prepend">
                    <label id="lbl_class" class="input-group-text" for="select_month">Class</label>
                </div>
                <select class="custom-select" id="select_class" onchange="courseChanged('SelectClassChanged()');">
                    <option value="0">Select one.....</option>
                </select>
            </div>
            <div id="student_control" class="input-group pr-3">
                <div class="input-group-prepend">
                    <label id="lbl_student" class="input-group-text" for="select_month">Student</label>
                </div>
                <select class="custom-select" id="select_student">
                    <option value="0">All</option>
                </select>
            </div>
        </div>
        <div id="daterange_control" class="input-group date w-50 pb-1">
            <div class="input-group-prepend">
                <label id="lbl_startdate" class="input-group-text" for="input_startdate">From:</label>
            </div>
            <input id="input_startdate" type="date" class="form-control" value="12-02-2012">
            <div class="input-group-prepend">
                <label id="lbl_enddate" class="input-group-text" for="input_enddate">To:</label>
            </div>
            <input id="input_enddate" type="date" class="form-control" value="12-02-2012">
        </div>
        <%--<div id="month_control" class="input-group" hidden>
            <div class="input-group-prepend">
                <label id="lbl_month" class="input-group-text" for="select_month">Options</label>
            </div>
            <select class="custom-select" id="select_month">
                <option value="1">January</option>
                <option value="2">February</option>
                <option value="3">March</option>
                <option value="4">April</option>
                <option value="5">May</option>
                <option value="6">June</option>
                <option value="7">July</option>
                <option value="8">August</option>
                <option value="9">September</option>
                <option value="10">October</option>
                <option value="11">November</option>
                <option value="12">December</option>
            </select>
        </div>--%>
        <input id="btn_get_records" type="button" class="btn" value="GO" onclick="goButtonClicked();" />
    </div>
    <div id="TableContainer">
        <table id="AttendanceTable" class="pre-scrollable">
            <thead>
                <tr></tr>
            </thead>
        </table>
    </div>

    <!--</div>-->

</asp:Content>
