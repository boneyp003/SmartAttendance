package app.com.smartattendancesystem.sharedpreference;

import android.content.Context;
import android.content.SharedPreferences;

import app.com.smartattendancesystem.Core.SmartAttendanceApp;


public class Preferences {

    public static final String KEY_LOGIN = "login";
    public static final String IS_LOGGED_IN = "is_logged_in";
    public static final String KEY_UNIQUE_ID = "unique_id";
    private static final String PREFS_NAME = "SmartAttendancePref";

    public static final String KEY_STUDENT_ID = "student_id";
    public static final String KEY_STUDENT_NAME = "student_name";
    public static final String KEY_STUDENT_EMAIL = "student_email";

    private static SharedPreferences get() {
        return SmartAttendanceApp.getAppContext().getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
    }

    public static void writeSharedPreferences(String key, String value) {
        SharedPreferences settings = get();
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(key, value);
        editor.apply();
    }

    public static void writeSharedPreferencesBool(String key, boolean value) {
        get().edit().putBoolean(key, value).apply();
    }

    public static String getAppPrefString(String key) {
        return get().getString(key, "");
    }

    public static void resetPreferences() {
        SmartAttendanceApp.getAppContext().getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE).edit().clear().commit();
    }

    public static boolean getAppPrefBool(String key, boolean default_value) {
        return get().getBoolean(key, default_value);
    }
}
