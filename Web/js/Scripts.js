



//#region global functions

function globalPageInitiate(method) {
    var stack = "(scriptsJS)globalPageInitiate() -->" + method;
    try {
        //set navigation bar for current page.

        $('#navigatelist').children().removeClass('active');
        if (document.URL.indexOf("Login.aspx") != -1) {
            $('#NavBar').children().hide();
        }
        else {
            if (document.URL.indexOf('Home.aspx') != -1) {
                $('#menuitem_home').addClass('active')
            }
            else if (document.URL.indexOf('ViewAttendance.aspx') != -1) {
                $('#menuitem_view_attendance').addClass('active')
            }
            else if (document.URL.indexOf('UpdateAttendance.aspx') != -1) {
                $('#menuitem_update_attendance').addClass('active')
            }
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);

    }
}

function setNavBar(method) {
    var stack = "(globalJS)SetNavBar() --> " + method;
    try {

    }
    catch(ex) {
        appendLog(stack, ex.message);
    }
}

function checkProfile(method) {
    var stack = "(ScriptsJS)CheckProfile() -->" + method;
    result = false;
    try {
        var tempobj = sessionStorage.getItem('ProfileObj');
        if (!(tempobj == null || typeof tempobj == "undefined" || tempobj.length <= 1)) {
            tempobj = JSON.parse(tempobj);
            result = true;
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
        result = false;
    }
    return result;
}

function setProfile(method) {
    var stack = "(ScriptsJS)setProfile() -->" + method;
    try {
        Profile_Object = JSON.parse(sessionStorage.getItem('ProfileObj'));
        if (Profile_Object.ID_Type == "student") {
            if (document.URL.indexOf("ViewAttendance.aspx") == -1) {
                window.location.href = "ViewAttendance.aspx";
            }
            $('#menuitem_home').remove();
            $('#menuitem_view_attendance').remove();
            $('#menuitem_update_attendance').remove();
        }

    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function getCourses(method) {
    var stack = "(ScriptsJS)getCourses() --> " + method;
    try {
        var multiPara = {
            "userid": Profile_Object.UserID,
            "idtype": Profile_Object.ID_Type,
            "method": stack
        };

        var tempobj = ajaxCall("Home.aspx/GetCourses", JSON.stringify(multiPara));
        if (!(tempobj == null || typeof tempobj == "undefined" || tempobj.length <= 1 || tempobj.indexOf("Error") != -1)) {
            //alert(tempobj);
            tempobj = JSON.parse(tempobj);
            Course_Object = tempobj.Courses;            
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function updateStudentList(method) {
    var stack = "(ScriptsJS)updateStudentList() --> " + method;
    try {

        if ($('#select_class').val() == "0") {
            return false;
        }
        var multiPara = {
            "userid": Profile_Object.UserID,
            "idtype": Profile_Object.ID_Type,
            "courseid": $('#select_class').val(),
            "method": stack
        };

        var tempobj = ajaxCall("Home.aspx/GetStudentList", JSON.stringify(multiPara));
        if (!(tempobj == null || typeof tempobj == "undefined" || tempobj.length <= 1 || tempobj.indexOf("Error") != -1)) {
            //alert(tempobj);
            Student_Object = JSON.parse(tempobj);
            tempobj = JSON.parse(tempobj).Students;
            //var htmlstring = '';
            tempobj.forEach(function (element) {
                $("#select_student").append('<option value="' + element.StudentID + '">' + element.StudentName +'('+ element.StudentID + ')' +'</option>');
            });
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

//#endregion

//#region handler

function courseChanged(method) {
    var stack = "(ScriptsJS)course_changed() --> " + method;
    try {
        $('#btn_get_records').attr('disabled', true);

        var selector = 'select_student';
        resetSelectControl(selector, '0', 'All', stack);
        updateStudentList(stack);

        $('#btn_get_records').removeAttr('disabled');
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function logoutClicked() {
    var stack = "(scriptsJS)logout_clicked()";
    try {
        sessionStorage.clear();
        window.location.href = "Login.aspx";
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

//#endregion

//#region utility

function resetSelectControl(selectorid, defaultValue, defaultText, method) {
    var stack = "(ScriptsJS)resetSelectControl() --> " + method;
    try {
        $('#' + selectorid).empty();
        $('#' + selectorid).append('<option value="' + defaultValue + '">' + defaultText + '</option>');
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function appendLog(method, ex) {
    try {
        var multiPara = {
            "method": method,
            "mException": ex
        }
        var result = ajaxCall("defaultservice.asmx/appendAjaxLog", JSON.stringify(multiPara));
    }
    catch (ex) {

    }
}

function ajaxCall(link, para_string) {
    var result;

    //alert(link + '\n' + para_string);

    try {
        $.ajax({
            type: "POST",
            url: link,
            data: para_string,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg, status) {
                //alert(msg.d);
                result = msg.d;
            },
            error: function (xhr, msg, e) {
                //alert("error" + e.toString());
                result = "error";
            }
        });
    }
    catch {
        result = "error";
    }
    //alert(result);
    return result;
}

function isOnline() {

    var result = false;

    if (navigator.onLine) {
        result = true;
    }

    return result;
}

function getCurrentTimeInMS() {
    try {
        var x = Date.now();
        x = x + ((new Date()).getTimezoneOffset()) * 60 * 1000;
        return x;
    }
    catch (ex) {

    }
}

//#endregion