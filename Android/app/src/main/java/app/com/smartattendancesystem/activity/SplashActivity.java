package app.com.smartattendancesystem.activity;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import app.com.smartattendancesystem.R;
import app.com.smartattendancesystem.sharedpreference.Preferences;

public class SplashActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        new Handler().postAtTime(
                new Runnable() {
                    @Override
                    public void run() {
                        if(Preferences.getAppPrefBool(Preferences.IS_LOGGED_IN, false)) {
                            goToMainActivity();
                        } else {
                            goToCheckRegisterActivity();
                        }
                    }
                }
        , 500);
    }

    private void goToCheckRegisterActivity() {
        Intent intent = new Intent(this, LoginActivity.class);
        startActivity(intent);
        finish();
    }

    private void goToMainActivity() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }
}

