package app.com.smartattendancesystem.Network.model;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class CheckRegisterRes {

    @SerializedName("deviceId")
    @Expose
    private String deviceId;

    public String getDeviceId() {
        return deviceId;
    }
}
