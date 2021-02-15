

function initiateLoginPage() {
    try {
        // hide Nav tabs





    }
    catch (ex) {
        
    }
}


//#region event handler

function selectorClicked(category) {
    var stack = "(LoginJS)selectorClicked()";
    try {
        $('#selector_control').children().removeClass('active');
        if (category === 'professor') {
            $('#btn_professor').addClass('active');
        }
        else if (category === 'student') {
            $('#btn_student').addClass('active');
        }
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
}

function submitClicked() {
    var stack = "(LoginJS)submitClicked()";

    try {
        $('#btn_submit').attr('disabled', true);

        var login_id = $('#txt_login_id').val();
        var login_pass = $('#txt_pass').val();

        var valid = checkLogin(login_id, login_pass, stack);
        if (valid.indexOf("Error") == -1 && valid.indexOf("|E|") == -1) {
            sessionStorage.setItem('ProfileObj', valid);
            window.location.href = "Home.aspx";
        }

    }
    catch (ex) {
        appendLog(stack, ex.message);
    }

    $('#btn_submit').removeAttr('disabled');
}



//#endregion



//#region helper

function checkLogin(userid, pass, method) {
    var stack = "(LoginJS)checkLogin() --> " + method;
    var result = false;
    try {

        var idtype = $('#selector_control').find('button.active').text();

        if (idtype == null || idtype == "" || idtype == '') {
            alert('We donot know if you are a student or professor. Please select one.');
            return "Error";
        }

        var multiPara = {
            "userid": userid,
            "pass": pass,
            "idtype": idtype.toLowerCase(),
            "method": stack
        };

        var json_string = ajaxCall("Login.aspx/Auth_Login", JSON.stringify(multiPara));
        return json_string;
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }

}

//#endregion