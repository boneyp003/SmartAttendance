package app.com.smartattendancesystem.activity;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Toast;

import app.com.smartattendancesystem.Network.DataService;
import app.com.smartattendancesystem.Network.RetrofitClientInstance;
import app.com.smartattendancesystem.Network.model.RegisterReq;
import app.com.smartattendancesystem.Network.model.RegisterRes;
import app.com.smartattendancesystem.R;
import app.com.smartattendancesystem.Util.Common;
import app.com.smartattendancesystem.sharedpreference.Preferences;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * A login screen that offers login via email/password.
 */
public class LoginActivity extends AppCompatActivity {

    EditText etEmail, etStudentId;
    Button btnRegister;
    private String studentId, email;
    ProgressBar progressBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        progressBar = findViewById(R.id.login_progress);

        etEmail = findViewById(R.id.etEmail);
        etStudentId = findViewById(R.id.etStudentId);
        btnRegister = findViewById(R.id.btnRegister);

        btnRegister.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {


                email = etEmail.getText().toString().trim();
                studentId = etStudentId.getText().toString().trim();


                if(email.isEmpty()) {
                    etEmail.setError("Please enter name.");
                    return;
                } else {
                    etEmail.setError(null);
                }

                if(!email.isEmpty() && !Common.isValidEmail(email)) {
                    etEmail.setError("Please enter correct email id.");
                    return;
                } else {
                    etEmail.setError(null);
                }

                if(studentId.isEmpty()) {
                    etStudentId.setError("Please enter name.");
                    return;
                } else {
                    etStudentId.setError(null);
                }

                registerDevice();
            }
        });
    }

    private void registerDevice() {
        showProgressBar();

        RegisterReq registerReq = new RegisterReq();
        registerReq.setEmail(email);

        registerReq.setStudentId(studentId);

        DataService service = RetrofitClientInstance.getRetrofitInstance().create(DataService.class);
        Call<RegisterRes> call = service.registerDevice(registerReq);
        call.enqueue(new Callback<RegisterRes>() {

            @Override
            public void onResponse(Call<RegisterRes> call, Response<RegisterRes> response) {
                RegisterRes registerRes =response.body();
                if(registerRes.getDeviceId().contains("Error")){
                    Toast.makeText(LoginActivity.this,"Unable to register Device!!!",Toast.LENGTH_SHORT).show();
                    hideProgressBar();
                }
                else {
                    Preferences.writeSharedPreferencesBool(Preferences.IS_LOGGED_IN, true);
                    Preferences.writeSharedPreferences(Preferences.KEY_UNIQUE_ID, registerRes.getDeviceId());
                    Preferences.writeSharedPreferences(Preferences.KEY_STUDENT_NAME, registerRes.getStudentName());
                    Preferences.writeSharedPreferences(Preferences.KEY_STUDENT_ID, registerRes.getStudentId());
                    Preferences.writeSharedPreferences(Preferences.KEY_STUDENT_EMAIL, "");
                    hideProgressBar();

                    goToMainActivity();
                }

            }

            @Override
            public void onFailure(Call<RegisterRes> call, Throwable t) {

                hideProgressBar();
            }
        });
    }

    private void goToMainActivity() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }

    private void showProgressBar() {
        progressBar.setVisibility(View.VISIBLE);
    }

    private void hideProgressBar() {
        progressBar.setVisibility(View.GONE);
    }
}

