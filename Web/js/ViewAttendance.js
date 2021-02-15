
//#region page initiate

function initiateViewAttendance(method) {

    var stack = "(ViewAttendanceJS)initiateViewAttendance() -->" + method;

    try {
        setProfile();
        rangeSelectorClicked('semester');
        getCourses(stack);
        Course_Object.forEach(function (element) {
            $("#select_class").append('<option value="' + element.CourseID + '">(' + element.CourseCode + ')' + element.CourseName + '</option>');
        });
        fetchTableHeaders(stack);
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function initiateTable(method) {
    var stack = "(ViewAttendanceJS)initiateTable() -->" + method;

    try {
        var table_obj = $('#AttendanceTable');
        initateTableHeader(stack);
        //table_obj.bootstrapTable()
        $('#AttendanceTable td').attr('min-width', 10);

    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function initateTableHeader(method) {
    var stack = "(ViewAttendanceJS)initateTableHeader() -->" + method;
    try {
        //var header_row = $('#AttendanceTable > thead tr');
        var json_string = localStorage.getItem('TableColumns');
        var json_obj = null;
        if ($('#select_student').val() === "0" && Profile_Object.ID_Type === "professor")
            json_obj = JSON.parse(json_string).Group_Attendance_Columns;
        else
            json_obj = JSON.parse(json_string).Individual_Attendance_Columns;            

        $('#AttendanceTable').bootstrapTable('destroy');
        $('#AttendanceTable > thead tr th').remove();
        $('#AttendanceTable').bootstrapTable('removeAll');
        $.each(json_obj, function (key, Element) {
            $('#AttendanceTable > thead tr').append('<th data-field="' + Element.data_field + '">' + Element.name + '</th>');
        });
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function fetchTableHeaders(method) {
    var stack = "(ViewAttendanceJS)initateTableHeader() --> " + method;
    try {
        var link = "Home.aspx/FetchTableColumns";
        var multiPara = {
            "method": stack
        };
        var result = ajaxCall(link, JSON.stringify(multiPara));
        //alert(result);

        if (result !== "error") {
            localStorage.setItem('TableColumns', result);
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

//#endregion

//#region event handlers

function rangeSelectorClicked(rangeCategory) {
    var stack = "(ViewAttendanceJS)rangeSelectorClicked()";
    try {
        $('#daterange_control').hide();
        $('#month_control').hide();
        $('#range_control').children().removeClass('active');
        if (rangeCategory === 'date') {
            $('#daterange_control').show();
            $('#btn_date').addClass('active');
        }
        else if (rangeCategory === 'month') {
            $('#month_control').show();
            $('#btn_month').addClass('active');
        }
        else if (rangeCategory === 'semester') {
            $('#btn_semester').addClass('active');
        }

        $('#AttendanceTable').bootstrapTable('destroy');
        $('#AttendanceTable > thead tr th').remove();
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function goButtonClicked() {
    var stack = "(ViewAttendanceJS)goButtonClicked()";

    try {
        initateTableHeader(stack);
        var multiPara = {
            "userid": Profile_Object.UserID,
            "idtype": Profile_Object.ID_Type,
            "courseid": $('#select_class').val(),
            "studentid": $('#select_student').val(),
            "method": stack
        };

        var attendancestring = ajaxCall("ViewAttendance.aspx/GetAttendance", JSON.stringify(multiPara));
        var optstring = ajaxCall("ViewAttendance.aspx/GetAttendance", JSON.stringify(multiPara));
        if (attendancestring.indexOf("Error") == -1 && optstring.indexOf("Error") == -1) {
            Attendance_Object = JSON.parse(attendancestring);
            //var table_obj = $('#AttendanceTable');
            //var rows = [];
            fillTable(stack);
        }
        else {
            alert(resultstring);
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function fillTable(method) {
    var stack = "(ViewAttendanceJS)fillTable() --> " + method;
    try {
        var rows = [];
        //Attendance_Object.Students.forEach(function (Element) {

        if (Profile_Object.ID_Type === "professor") {
            if ($('#select_student').val() === "0") {
                Student_Object.Students.forEach(function (Element) {
                    var at_count = 0;
                    var at_norange_count = 0;
                    Attendance_Object.Students.forEach(function (atobj) {

                        if ($('#btn_date').hasClass('active')) {
                            if ( (new Date($('#input_startdate').val())) > (new Date(parseFloat(atobj.TimeStamp))) || (new Date($('#input_enddate').val())) < (new Date(parseFloat(atobj.TimeStamp) - 86400000)) ) {
                                return;
                            }
                        }

                        if (atobj.StudentID == Element.StudentID) {
                            at_count++;
                            if (atobj.InRange == "False")
                                at_norange_count++;
                        }
                    });

                    rows.push({
                        "student_id": Element.StudentID,
                        "student_name": Element.StudentName,
                        "attendance": at_count,
                        "out_of_range": at_norange_count
                    });
                });
            }
            else {
                Attendance_Object.Students.forEach(function (atobj) {

                    if ($('#btn_date').hasClass('active')) {
                        if ((new Date($('#input_startdate').val())) > (new Date(parseFloat(atobj.TimeStamp))) || (new Date($('#input_enddate').val())) < (new Date(parseFloat(atobj.TimeStamp) - 86400000))) {
                            return;
                        }
                    }

                    if (atobj.StudentID === $('#select_student').val()) {
                        rows.push({
                            "student_id": atobj.StudentID,
                            "student_name": atobj.StudentName,
                            "out_of_range": atobj.InRange === "True" ? "No" : "Yes",
                            "time_stamp": (new Date(parseFloat(atobj.TimeStamp))).toLocaleDateString()
                            ///"time_stamp": atobj.TimeStamp
                        });
                    }
                });
            }
        }
        else if (Profile_Object.ID_Type === "student") {
            Attendance_Object.Students.forEach(function (atobj) {

                if ($('#btn_date').hasClass('active')) {
                    if ((new Date($('#input_startdate').val())) > (new Date(parseFloat(atobj.TimeStamp))) || (new Date($('#input_enddate').val())) < (new Date(parseFloat(atobj.TimeStamp) - 86400000))) {
                        return;
                    }
                }
                rows.push({
                    "student_id": atobj.StudentID,
                    "student_name": atobj.StudentName,
                    "out_of_range": atobj.InRange === "True" ? "No" : "Yes",
                    "time_stamp": (new Date(parseFloat(atobj.TimeStamp))).toLocaleDateString()
                    ///"time_stamp": atobj.TimeStamp
                });
            });
        }
        $('#AttendanceTable').bootstrapTable('removeAll');
        $('#AttendanceTable').bootstrapTable({ data: rows });
        

    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

//#endregion

//#region Attendance Table Functions

function buildAttendanceTable(method) {
    var stack = "(ViewAttendanceJS)buildAttendanceTable() --> " + mmethod;
    try {
        var table_object = $('#AttendanceTable');
        var data = getTableData(stack);
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function getTableData(method) {
    var stack = "(ViewAttendanceJS)getTableData() --> " + method;
    var result = "Error";

    try {
        
        


    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
    return result;
}

//#endregion

