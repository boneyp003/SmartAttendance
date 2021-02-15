package app.com.smartattendancesystem.Network;

import app.com.smartattendancesystem.Network.model.CheckRegisterReq;
import app.com.smartattendancesystem.Network.model.CheckRegisterRes;
import app.com.smartattendancesystem.Network.model.MarkAttendanceReq;
import app.com.smartattendancesystem.Network.model.MarkAttendanceRes;
import app.com.smartattendancesystem.Network.model.RegisterReq;
import app.com.smartattendancesystem.Network.model.RegisterRes;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface DataService {

    /*@GET("todos/1")
    Call<ResponseBody> getToDo();*/

    @POST("RegisterDevice/")
    Call<RegisterRes> registerDevice(@Body RegisterReq registerReq);

    @POST("CheckRegistration/")
    Call<CheckRegisterRes> checkRegistration(@Body CheckRegisterReq checkRegisterReq);

    @POST("MarkAttendance/")
    Call<MarkAttendanceRes> markAttendance(@Body MarkAttendanceReq markAttendanceReq);



}