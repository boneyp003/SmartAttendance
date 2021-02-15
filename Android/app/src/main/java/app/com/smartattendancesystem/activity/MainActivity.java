package app.com.smartattendancesystem.activity;

import android.Manifest;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import java.util.Calendar;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.location.LocationListener;
import com.google.android.gms.location.LocationRequest;
import com.google.android.gms.location.LocationServices;

import java.util.ArrayList;

import app.com.smartattendancesystem.Network.DataService;
import app.com.smartattendancesystem.Network.RetrofitClientInstance;
import app.com.smartattendancesystem.Network.model.MarkAttendanceReq;
import app.com.smartattendancesystem.Network.model.MarkAttendanceRes;
import app.com.smartattendancesystem.R;
import app.com.smartattendancesystem.Util.Common;
import app.com.smartattendancesystem.sharedpreference.Preferences;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity
        implements GoogleApiClient.ConnectionCallbacks,
                   GoogleApiClient.OnConnectionFailedListener, LocationListener{

  private Location location;

  private GoogleApiClient googleApiClient;
  private static final int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;
  private LocationRequest locationRequest;
  private static final long UPDATE_INTERVAL = 5000, FASTEST_INTERVAL = 5000; // = 5 seconds
  // lists for permissions
  private ArrayList<String> permissionsToRequest;
  private ArrayList<String> permissionsRejected = new ArrayList<>();
  private ArrayList<String> permissions = new ArrayList<>();
  // integer for permissions results request
  private static final int ALL_PERMISSIONS_RESULT = 1011;

  private Button btnOtpSubmit;
  private Button btnReRegister;
  private ProgressBar progressBar;
  private EditText etOtp;
  private TextView txtName;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.activity_main);

    progressBar = findViewById(R.id.progress_bar);
    btnOtpSubmit = findViewById(R.id.btnOtpSubmit);
    btnReRegister = findViewById(R.id.btnReRegister);
    etOtp = findViewById(R.id.etOtp);
    txtName = findViewById(R.id.txtName);

    txtName.setText(Preferences.getAppPrefString(Preferences.KEY_STUDENT_NAME));
    setupPermissionAndLocation();
    btnOtpSubmit.setOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View view) {
        if (location != null) {
          Log.e("LAT-LONG", "Latitude : " + location.getLatitude() + "\nLongitude : " + location.getLongitude());

          markAttendance();
        }
      }
    });
    btnReRegister.setOnClickListener(new View.OnClickListener() {
      @Override
      public void onClick(View view) {
        new AlertDialog.Builder(MainActivity.this)
                .setTitle("Are you sure?")
                .setMessage("Do you really want to reset current device-registration?")
                .setIcon(android.R.drawable.ic_dialog_alert)
                .setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                  public void onClick(DialogInterface dialog, int whichButton) {
                    Preferences.resetPreferences();
                    Intent intent = new Intent(MainActivity.this, LoginActivity.class);
                    startActivity(intent);
                    finish();
                  }})
                .setNegativeButton(android.R.string.no, null).show();

      }
    });
  }

  private void setupPermissionAndLocation() {
    permissions.add(Manifest.permission.ACCESS_FINE_LOCATION);
    permissions.add(Manifest.permission.ACCESS_COARSE_LOCATION);

    permissionsToRequest = permissionsToRequest(permissions);

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
      if (permissionsToRequest.size() > 0) {
        requestPermissions(permissionsToRequest.toArray(
                new String[permissionsToRequest.size()]), ALL_PERMISSIONS_RESULT);
      }
    }

    // we build google api client
    googleApiClient = new GoogleApiClient.Builder(MainActivity.this).
            addApi(LocationServices.API).
            addConnectionCallbacks(MainActivity.this).
            addOnConnectionFailedListener(MainActivity.this).build();
  }


  @Override
  public boolean onCreateOptionsMenu(Menu menu) {
    MenuInflater inflater = getMenuInflater();
    inflater.inflate(R.menu.main_menu, menu);
    return super.onCreateOptionsMenu(menu);
  }

  @Override
  public boolean onOptionsItemSelected(MenuItem item) {
    switch (item.getItemId()) {
      case R.id.menu_logout:
        logout();
        return true;
      default:
        return super.onOptionsItemSelected(item);
    }
  }

  private void logout() {

    Preferences.writeSharedPreferencesBool(Preferences.IS_LOGGED_IN, false);
    Preferences.writeSharedPreferences(Preferences.KEY_UNIQUE_ID, "");

    Intent intent = new Intent(this, CheckRegisterActivity.class);
    startActivity(intent);
    finish();
  }

  private ArrayList<String> permissionsToRequest(ArrayList<String> wantedPermissions) {
    ArrayList<String> result = new ArrayList<>();

    for (String perm : wantedPermissions) {
      if (!hasPermission(perm)) {
        result.add(perm);
      }
    }

    return result;
  }

  private boolean hasPermission(String permission) {
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
      return checkSelfPermission(permission) == PackageManager.PERMISSION_GRANTED;
    }

    return true;
  }

  @Override
  protected void onStart() {
    super.onStart();

    if (googleApiClient != null) {
      googleApiClient.connect();
    }
  }

  @Override
  protected void onResume() {
    super.onResume();

    if (!checkPlayServices()) {
      Toast.makeText(this, "You need to install Google Play Services to use the App properly", Toast.LENGTH_LONG).show();
    }
  }

  @Override
  protected void onPause() {
    super.onPause();

    // stop location updates
    if (googleApiClient != null  &&  googleApiClient.isConnected()) {
      LocationServices.FusedLocationApi.removeLocationUpdates(googleApiClient, this); 
	  googleApiClient.disconnect();
    }
  }

  private boolean checkPlayServices() {
    GoogleApiAvailability apiAvailability = GoogleApiAvailability.getInstance();
    int resultCode = apiAvailability.isGooglePlayServicesAvailable(this);

    if (resultCode != ConnectionResult.SUCCESS) {
      if (apiAvailability.isUserResolvableError(resultCode)) {
        apiAvailability.getErrorDialog(this, resultCode, PLAY_SERVICES_RESOLUTION_REQUEST);
      } else {
        finish();
      }

      return false;
    }

    return true;
  }

  @Override
  public void onConnected(@Nullable Bundle bundle) {
    if (ActivityCompat.checkSelfPermission(this, 
	                 Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED
        &&  ActivityCompat.checkSelfPermission(this, 
		             Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
      return;
    }

    // Permissions ok, we get last location
    location = LocationServices.FusedLocationApi.getLastLocation(googleApiClient);


    startLocationUpdates();
  }

  private void startLocationUpdates() {
    locationRequest = new LocationRequest();
    locationRequest.setPriority(LocationRequest.PRIORITY_HIGH_ACCURACY);
    locationRequest.setInterval(UPDATE_INTERVAL);
    locationRequest.setFastestInterval(FASTEST_INTERVAL);

    if (ActivityCompat.checkSelfPermission(this, 
	          Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED
          &&  ActivityCompat.checkSelfPermission(this, 
		      Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
      Toast.makeText(this, "You need to enable permissions to display location !", Toast.LENGTH_SHORT).show();
    }

    LocationServices.FusedLocationApi.requestLocationUpdates(googleApiClient, locationRequest, this);
  }

  @Override
  public void onConnectionSuspended(int i) {
  }

  @Override
  public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {
  }

  @Override
  public void onLocationChanged(Location location) {
    /*if (location != null) {
      locationTv.setText("Latitude : " + location.getLatitude() + "\nLongitude : " + location.getLongitude());
    }*/
  }

  @Override
  public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
    switch(requestCode) {
      case ALL_PERMISSIONS_RESULT:
        for (String perm : permissionsToRequest) {
          if (!hasPermission(perm)) {
            permissionsRejected.add(perm);
          }
        }

        if (permissionsRejected.size() > 0) {
          if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (shouldShowRequestPermissionRationale(permissionsRejected.get(0))) {
              new AlertDialog.Builder(MainActivity.this).
                  setMessage("These permissions are mandatory to get your location. You need to allow them.").
                  setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                      if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                        requestPermissions(permissionsRejected.
						    toArray(new String[permissionsRejected.size()]), ALL_PERMISSIONS_RESULT);
                      }
                    }
                  }).create().show();

                  return;
            }
          }
        } else {
          if (googleApiClient != null) {
            googleApiClient.connect();
          }
        }

        break;
    }
  }

  private void markAttendance() {

    MarkAttendanceReq markAttendanceReq = new MarkAttendanceReq();
    markAttendanceReq.setStudentId(Preferences.getAppPrefString(Preferences.KEY_STUDENT_ID));
    markAttendanceReq.setOtp(etOtp.getText().toString().trim());
    //markAttendanceReq.setTimestamp(String.valueOf(Common.getTimeStamp()));
    markAttendanceReq.setTimestamp(String.valueOf(Calendar.getInstance().getTimeInMillis()));
    markAttendanceReq.setLatitude(String.valueOf(location.getLatitude()));
    markAttendanceReq.setLongitude(String.valueOf(location.getLongitude()));
    markAttendanceReq.setDeviceId(Preferences.getAppPrefString(Preferences.KEY_UNIQUE_ID));

    DataService service = RetrofitClientInstance.getRetrofitInstance().create(DataService.class);
    Call<MarkAttendanceRes> call = service.markAttendance(markAttendanceReq);
    call.enqueue(new Callback<MarkAttendanceRes>() {
//Error:Success
      @Override
      public void onResponse(Call<MarkAttendanceRes> call, Response<MarkAttendanceRes> response) {
          MarkAttendanceRes markAttendanceRes = response.body();
          if(markAttendanceRes.getResult().equals("Success")) {
              Toast.makeText(MainActivity.this, "Attendance Completed", Toast.LENGTH_LONG).show();
          } else {
            Toast.makeText(MainActivity.this, "Error:" + markAttendanceRes.getResult(), Toast.LENGTH_LONG).show();
          }
      }

      @Override
      public void onFailure(Call<MarkAttendanceRes> call, Throwable t) {

      }
    });
  }

  private void showProgressBar() {
    progressBar.setVisibility(View.VISIBLE);
  }

  private void hideProgressBar() {
    progressBar.setVisibility(View.GONE);
  }
}