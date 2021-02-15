package app.com.smartattendancesystem.Network.model;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class RegisterRes {

    @SerializedName("DeviceID")
    @Expose
    private String deviceId;
    @SerializedName("StudentID")
    @Expose
    private String studentId;
    @SerializedName("StudentName")
    @Expose
    private String studentName;

    // set response parameter in this file

    public String getDeviceId() {
        return deviceId;
    }

    public String getStudentId() {
        return studentId;
    }

    public String getStudentName() {
        return studentName;
    }
}
