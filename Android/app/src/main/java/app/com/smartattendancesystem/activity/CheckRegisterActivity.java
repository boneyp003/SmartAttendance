package app.com.smartattendancesystem.activity;

import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;
import android.annotation.TargetApi;
import android.app.LoaderManager.LoaderCallbacks;
import android.content.CursorLoader;
import android.content.Intent;
import android.content.Loader;
import android.content.pm.PackageManager;
import android.database.Cursor;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.provider.ContactsContract;
import android.support.annotation.NonNull;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.inputmethod.EditorInfo;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import app.com.smartattendancesystem.Network.DataService;
import app.com.smartattendancesystem.Network.RetrofitClientInstance;
import app.com.smartattendancesystem.Network.model.CheckRegisterReq;
import app.com.smartattendancesystem.Network.model.CheckRegisterRes;
import app.com.smartattendancesystem.Network.model.RegisterRes;
import app.com.smartattendancesystem.R;
import okhttp3.Response;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;

import static android.Manifest.permission.READ_CONTACTS;

/**
 * A login screen that offers login via email/password.
 */
public class CheckRegisterActivity extends AppCompatActivity {

    private EditText etStudentId;
    private Button btnCheckRegistered, btnNewRegister;
    private String studentId;
    private ProgressBar progressBar;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_check_registered);


        progressBar = findViewById(R.id.login_progress);
        etStudentId = findViewById(R.id.etStudentId);
        btnCheckRegistered = findViewById(R.id.btnCheckRegistered);
        btnNewRegister = findViewById(R.id.btnNewRegister);

        studentId = etStudentId.getText().toString().trim();

        btnCheckRegistered.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                checkRegister();
            }
        });

        btnNewRegister.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                goToLoginActivity();
            }
        });

        /*mEmailView = (AutoCompleteTextView) findViewById(R.id.email);

        Button mEmailSignInButton = (Button) findViewById(R.id.email_sign_in_button);
        mEmailSignInButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                attemptLogin();
            }
        });

        mLoginFormView = findViewById(R.id.login_form);
        mProgressView = findViewById(R.id.login_progress);*/
    }

    private void goToLoginActivity() {

        Intent intent = new Intent(this, LoginActivity.class);
        startActivity(intent);
    }

    private void checkRegister() {

        CheckRegisterReq checkRegisterReq = new CheckRegisterReq();
        checkRegisterReq.setStudentId(etStudentId.getText().toString().trim());

        DataService service = RetrofitClientInstance.getRetrofitInstance().create(DataService.class);
        Call<CheckRegisterRes> call = service.checkRegistration(checkRegisterReq);
        call.enqueue(new Callback<CheckRegisterRes>() {

            @Override
            public void onResponse(Call<CheckRegisterRes> call, retrofit2.Response<CheckRegisterRes> response) {

            }

            @Override
            public void onFailure(Call<CheckRegisterRes> call, Throwable t) {

            }
        });
    }

    /*private void checkRegister() {
        DataService service = RetrofitClientInstance.getRetrofitInstance().create(DataService.class);
        Call<ResponseBody> call = service.getToDo();
        call.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, retrofit2.Response<ResponseBody> response) {
                Log.e("RESPONSE BODY", response.toString());
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {

            }
        });
    }*/

    private void showProgressBar() {
        progressBar.setVisibility(View.VISIBLE);
    }

    private void hideProgressBar() {
        progressBar.setVisibility(View.GONE);
    }
}

