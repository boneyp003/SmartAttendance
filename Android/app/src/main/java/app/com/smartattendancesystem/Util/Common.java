package app.com.smartattendancesystem.Util;

import android.text.TextUtils;

public class Common {

    public static boolean isValidEmail(CharSequence target) {
        return !TextUtils.isEmpty(target) && android.util.Patterns.EMAIL_ADDRESS.matcher(target).matches();
    }

    public static Long getTimeStamp() {
        return System.currentTimeMillis()/1000;
    }
}
