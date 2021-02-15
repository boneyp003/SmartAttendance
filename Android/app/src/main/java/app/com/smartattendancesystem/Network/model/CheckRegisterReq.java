package app.com.smartattendancesystem.Network.model;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class CheckRegisterReq {

    @SerializedName("studentId")
    @Expose
    private String studentId;

    public void setStudentId(String studentId) {
        this.studentId = studentId;
    }
}
