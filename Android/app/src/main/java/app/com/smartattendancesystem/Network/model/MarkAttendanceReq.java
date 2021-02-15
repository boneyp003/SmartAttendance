package app.com.smartattendancesystem.Network.model;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class MarkAttendanceReq {

    @SerializedName("RegNo")
    @Expose
    private String deviceId;

    @SerializedName("StudentID")
    @Expose
    private String studentId;

    @SerializedName("OneTP")
    @Expose
    private String otp;

    @SerializedName("Latitude")
    @Expose
    private String latitude;

    @SerializedName("Longitude")
    @Expose
    private String longitude;

    @SerializedName("TimeStamp")
    @Expose
    private String timestamp;

    public void setDeviceId(String deviceId) {
        this.deviceId = deviceId;
    }

    public void setStudentId(String studentId) {
        this.studentId = studentId;
    }

    public void setOtp(String otp) {
        this.otp = otp;
    }

    public void setLatitude(String latitude) {
        this.latitude = latitude;
    }

    public void setLongitude(String longitude) {
        this.longitude = longitude;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }
}
