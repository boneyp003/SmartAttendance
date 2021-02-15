package app.com.smartattendancesystem.Network.model;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class MarkAttendanceRes {

    @SerializedName("studentId")
    @Expose
    private String studentId;

    @SerializedName("deviceId")
    @Expose
    private String deviceId;

    @SerializedName("Result")
    @Expose
    private String result;

    public String getStudentId() {
        return studentId;
    }

    public String getDeviceId() {
        return deviceId;
    }

    public String getResult() {
        return result;
    }
}
