
function initiateHome(method) {
    var stack = "(HomeJS)initateHome() --> " + method;

    try {
        setProfile();
        $('#lbl_user').text('UserID: ' + Profile_Object.UserName);
        getCourses(stack);
        Course_Object.forEach(function (element) {
            $("#select_class").append('<option value="' + element.CourseID + '">(' + element.CourseCode + ')' + element.CourseName + '</option>');
        });
    }
    catch (ex) {
        appendlog(stack, ex.message);
    }
}

function generateOTP() {
    var stack = "(Home/JS)generateOTP()"; 
    try {

        if ($('#select_class').val() == "0") {
            alert("Please select a class");
            return false;
        }
        //$('#generateOTP').val('generateOTP just called!!');
        $('#btn_generateotp').attr('disabled', true);

        var dataPara =
        {
            "userid": Profile_Object.UserID,
            "classid": $('#select_class').val(),
            "datetime": getCurrentTimeInMS().toString(),
            "method": stack
        };
        $.ajax({
            type: 'POST',
            url: "Home.aspx/GenerateOTP",
            data: JSON.stringify(dataPara),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg,status) {
                if (msg.d != "E") {
                    $('#lbl_otp').text(msg.d);
                }
                else {
                    alert('Unable to fetch OTP!!!!\nPlease try again.');
                }
            },
            error: function(xhr,status,e) {
                //appendLog(stack, e.message);
            }
        });

        getLocation();
    }
    catch (ex) {
        appendLog(stack, ex.message);
    }
    $('#btn_generateotp').removeAttr('disabled');
}



function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    } else {
        x.innerHTML = "Geolocation is not supported by this browser.";
    }
}

function showPosition(position) {
    alert("Latitude: " + position.coords.latitude +
        "<br>Longitude: " + position.coords.longitude);
}

