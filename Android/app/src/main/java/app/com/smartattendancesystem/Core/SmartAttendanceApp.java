package app.com.smartattendancesystem.Core;

import android.app.Application;
import android.content.Context;

public class SmartAttendanceApp extends Application {

    public static SmartAttendanceApp smartAttendanceApp = null;

    public SmartAttendanceApp() {
        smartAttendanceApp = this;
    }

    @Override
    public void onCreate() {
        super.onCreate();
    }

    public static Context getAppContext() {
        if (smartAttendanceApp == null) {
            smartAttendanceApp = new SmartAttendanceApp();
        }
        return smartAttendanceApp;
    }
}
