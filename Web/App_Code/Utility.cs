using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Device.Location;

namespace gradproj_webapp
{   
    public class Utility
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;


        public Utility()
        { }

        #region logs

        public static void AppendLog(string stacktrace, string exception)
        {
            string result = "";

            try
            {
                string path = HttpContext.Current.Server.MapPath("~" + @"\log\");

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                path += @"\error_log.txt";
                FileStream fs = null;

                if (!File.Exists(path))
                {
                    using (fs = File.Create(path)) { }
                }
                if (File.Exists(path))
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.Write("\r\n");
                        sw.WriteLine("{0} {1}", DateTime.Now.ToString("hh:mm:ss.f tt"), DateTime.Now.ToLongDateString());
                        sw.WriteLine("Method trace: {0}", stacktrace);
                        sw.WriteLine("Exception: {0}", exception);
                        sw.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        sw.WriteLine();
                    }
                }
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }

            //return result;
        }
        
        #endregion
        
        #region deviceregistration

        private static string RandomNumberGenerator(string studentid, string method)
        {
            string stack = "Utility/randomNumberGenerator() --> " + method;
            string result = "Error";
            try
            {
                Random generator = new Random();
                String r = generator.Next(0, 1000000).ToString("D6");
                /////if (r.Distinct().Count() == 1) { r = GenerateNewRandom(); } return r;
                result = r + "_" + studentid;
            }
            catch (Exception ex)
            {
                //result = "Error";
                AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        //public static IOClass.MarkAttendaceOut RegisterDevice(IOClass.MarkAttendaceIn value, string method)
        public static IOClass.RegisterDeviceOut RegisterDevice(string StudentID, string email, string regTime, string method)
        {
            string stack = "Utility/randomNumberGenerator() --> " + method;


            AutoCloseOTP(stack);

            IOClass.RegisterDeviceOut result = new IOClass.RegisterDeviceOut();
            result.StudentName = "Error";
            result.StudentID = "Error";
            result.DeviceID = "Error";
            if (CheckForActiveOTP(StudentID, stack))
            {
                //// Return desired result if there is a an active OTP going on for registered class.
            }

            try
            {
                DB_class dbconn = new DB_class();
                string sqlstr = "";

                sqlstr = "select first_name + ' ' + last_name from Student where student_id = '" + StudentID + "'";
                result.StudentName = dbconn.ExecuteScalar(sqlstr, stack);

                string regno = RandomNumberGenerator(StudentID, method);
                if(regno != "Error" && result.StudentName != "Error")
                {
                    sqlstr = "insert into Device_Register(device_id,student_id, reg_date) ";
                    sqlstr += "values('"+ regno +"', '"+ StudentID + "', GETDATE())";

                    if(dbconn.NonQuery(sqlstr, method) > 0)
                    {
                        //deactivate previous device_id
                        sqlstr = "update Device_Register set active = 0 where student_id = '"+ StudentID +"' and device_id <> '"+ regno +"'";
                        int x = dbconn.NonQuery(sqlstr, stack);

                        result.StudentID = StudentID;
                        result.DeviceID = regno;
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }

            return result;
        }

        public static IOClass.CheckRegistrationModel CheckDevice(string studentID, string regNo, string method)
        {
            string stack = "(UtilityCS)CheckDeviceRegistration() --> " + method;
            IOClass.CheckRegistrationModel result = new IOClass.CheckRegistrationModel();

            result.StudentID = "Error";
            result.DeviceID = "Error";
            try
            {
                if (CheckDeviceRegistration(studentID, regNo, stack))
                {
                    result.StudentID = studentID;
                    result.DeviceID = regNo;
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        private static bool CheckDeviceRegistration(string studentID, string regNo, string method)
        {
            string stack = "(UtilityCS)MarkAttendance() --> " + method;
            bool result = false;
            try
            {
                string sqlstr = "select count(*) from Device_Register where student_id='"+ studentID +"' and device_id='"+ regNo +"' and active = 1";
                DB_class dbconn = new DB_class();
                string tempcourseid = dbconn.ExecuteScalar(sqlstr, stack).ToString();

                if (dbconn.ExecuteScalar(sqlstr, stack).ToString() != "0")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        private static bool CheckForActiveOTP(string studentid, string method)
        {
            string stack = "(UtilityCS)CheckForActiveOTP() --> " + method;
            bool result = false;
            try
            {
                string sqlstr = "select count(*) from One_Time_Pass as onetp "+
                    "inner join Class_Register as cr on cr.course_id = onetp.course_id and student_id ='"+ studentid +"' "+
                    "where onetp.is_valid = 1 and onetp.active = 1";
                DB_class dbconn = new DB_class();
                result = (dbconn.ExecuteScalar(sqlstr, stack) != "0");
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        #endregion

        #region Attendance

        public static string MarkAttendance(string studentID, string timeStamp, string oneTP, string longitude, string latitude, string regNo, string method)
        {
            string stack = "(UtilityCS)MarkAttendance() --> " + method;
            string result = "Error";
            AutoCloseOTP(stack);

            try
            {
                if (CheckDeviceRegistration(studentID, regNo, stack))
                {
                    string sqlstr = "select cl.course_id, co.gps_latitude, co.gps_longitude from Class_Register as cl ";
                    sqlstr += "inner join Course as co on cl.course_id=co.course_uid and cl.student_id='" + studentID + "' ";
                    sqlstr += "inner join One_Time_Pass as ot on otp='" + oneTP + "' and cl.course_id=ot.course_id and ot.active=1 and ot.is_valid=1";
                    DB_class dbconn = new DB_class();
                    //string tempcourseid = dbconn.ExecuteScalar(sqlstr, stack).ToString();
                    DataTable dt = new DataTable();
                    dt = dbconn.ExecuteTable(sqlstr, stack);
                    if (dt.Rows.Count < 1)
                    {
                        result = "Error: You are not registered for this class.";
                    }
                    else
                    {
                        // check if the user has already marked attendance
                        DataRow dr = dt.Rows[0];
                        string tempcourseid = dr["course_id"].ToString();
                        sqlstr = "select COUNT(*) from Attendance where student_id='" + studentID + "' and otp='" + oneTP + "' and course_id=" + tempcourseid + " and DATEADD(hh,1,create_time)>GETDATE()";
                        if(int.Parse(dbconn.ExecuteScalar(sqlstr, stack)) > 0)
                        {
                            return "Error: You have already marked attendance";
                        }



                        //float[] actualco = { float.Parse(templatitude, CultureInfo.InvariantCulture.NumberFormat), float.Parse(templongitude, CultureInfo.InvariantCulture.NumberFormat) };

                        //float[] recvdco = { float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat), float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat) };

                        string inrange = "0";
                        try
                        {
                            double templongitude = Convert.ToDouble(dr["gps_longitude"].ToString());
                            double templatitude = Convert.ToDouble(dr["gps_latitude"].ToString());
                            double inlongitude = Convert.ToDouble(longitude);
                            double inlatitude = Convert.ToDouble(latitude);

                            //double xxx = GetDistanceFromLatLonInKm(templatitude, templongitude, inlatitude, inlongitude);
                            //double xxx = DistanceBetweenPlaces(templatitude, templongitude, inlatitude, inlongitude);
                            

                            ////if (Math.Abs(templatitude-inlatitude) <= 0.0001 && Math.Abs(templongitude-templongitude) <= 0.0001)

                            var sCoord = new GeoCoordinate(templatitude, templongitude);
                            var eCoord = new GeoCoordinate(inlatitude, inlongitude);

                            // this returns the distance in prefed coordinates and received coordinates in meters
                            double xxx = sCoord.GetDistanceTo(eCoord);  // value is in meters

                            AppendLog(stack, xxx.ToString());
                            //xxx = 
                            if (xxx < 15.00)
                            {
                                inrange = "1";
                            }

                            //if (latitude.IndexOf(templatitude.ToString("C3")) > -1 && longitude.IndexOf(templongitude.ToString("C3")) > -1)
                            //{ inrange = "1"; }
                        }
                        catch
                        {

                        }
                        sqlstr = "insert into Attendance(device_id, gps_latitude, gps_longitude, otp, student_id, time_stamp, course_id, in_range, create_time) ";
                        sqlstr += "values('" + regNo + "','" + latitude + "', '" + longitude + "', '" + oneTP + "', '" + studentID + "', '"+ timeStamp +"', " + tempcourseid + ", "+ inrange +", GETDATE())";

                        
                        if (dbconn.NonQuery(sqlstr, stack) > 0)
                        {
                            result = "Success";
                        }
                    }
                }
                else
                {
                    result = "Error: Device not registered.";
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(double lon1,double lat1,double lon2,double lat2)
        {
            double dlon = Radians(lon2 - lon1);
            double dlat = Radians(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }

        private static double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radius of the earth in km
            double dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            double dLon = Deg2Rad(lon2 - lon1);
            double a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in km
            return d;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static string GetAttendance(string userid, string idtype, string courseid, string studentid,  string method)
        {
            string stack = "(Utility)GetAttendance()" + method;
            string result = "Error";

            AutoCloseOTP(stack);

            try
            {
                DB_class dbconn = new DB_class();
                string sqlstr = "";
                if (idtype.Equals("student"))
                {
                    sqlstr = "select at.*, st.first_name, st.last_name, onetp.create_time from Attendance as at "
                        + "inner join One_Time_Pass as onetp on onetp.course_id=at.course_id and onetp.is_valid=1 and onetp.otp=at.otp "
                        + "and onetp.start_time<at.create_time and onetp.stop_time>at.create_time "
                        + "inner join Student as st on st.student_id = at.student_id "
                        + "where at.course_id = " + courseid + " and at.student_id = '"+ userid +"'";
                }
                else if (idtype.Equals("professor"))
                {
                    sqlstr = "select at.*, st.first_name, st.last_name, onetp.create_time from Attendance as at";
                    sqlstr += " inner join One_Time_Pass as onetp on onetp.course_id=at.course_id and onetp.is_valid=1 and onetp.otp=at.otp "
                        + "and onetp.start_time<at.create_time and onetp.stop_time>at.create_time";
                    sqlstr += " inner join Student as st on st.student_id = at.student_id";
                    sqlstr += " where at.course_id = " + courseid;
                    if (!studentid.Equals("0"))
                    {
                        sqlstr += " and at.student_id = '"+ studentid +"'";
                    }
                }
                else
                {
                    return result;
                }

                DataTable dt = new DataTable();
                dt = dbconn.ExecuteTable(sqlstr, stack);

                if (!dt.Equals(DBNull.Value))
                {
                    sqlstr = "select count(*) from One_Time_Pass where course_id="+ courseid +" and is_valid=1";
                    string total_attendance = dbconn.ExecuteScalar(sqlstr, stack);
                    result = Json_string_attendane(dt, total_attendance, stack);
                }

            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        private static string Json_string_attendane(DataTable dt, string total_attendance, string method)
        {
            string stack = "(Utility)Json_string_attendane() --> " + method;
            string result = "Error";
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(2000);
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter w = new JsonTextWriter(sw))
                {
                    w.WriteStartObject();
                    w.WritePropertyName("Students");
                    w.WriteStartArray();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        w.WriteStartObject();
                        w.WritePropertyName("StudentID");
                        w.WriteValue(dr["student_id"].ToString());
                        w.WritePropertyName("OTP");
                        w.WriteValue(dr["otp"].ToString());
                        w.WritePropertyName("TimeStamp");
                        w.WriteValue(dr["time_stamp"].ToString());
                        w.WritePropertyName("InRange");
                        w.WriteValue(dr["in_range"].ToString());
                        w.WritePropertyName("OtpTime");
                        w.WriteValue(dr["create_time"].ToString());
                        w.WritePropertyName("StudentName");
                        w.WriteValue(dr["last_name"].ToString() + " " + dr["first_name"].ToString());
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WritePropertyName("TotalAttendance");
                    w.WriteValue(total_attendance);
                    w.WriteEndObject();

                    //w.WriteEndObject();
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                result = "Error";
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        private static void AutoCloseOTP(string method)
        {
            string stack = "(Utility)Json_string_attendane() --> " + method;
            DB_class dbconn = new DB_class();
            try
            {
                //string stacktrace = "(service)CloseOTP()";
                string sqlstr = "update One_Time_Pass set active = 0, stop_time = dateadd(mi, 10, start_time) "
                    +"where dateadd(mi, 15, start_time) < getdate() and active = 1";
                int no_of_rows = dbconn.NonQuery(sqlstr, stack);
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }
        }
        
        #endregion

        #region initiate

        public static string getSettings(string method)
        {
            string stack = "Utility/getSettings() --> " + method;
            string result = "error";
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"\");
                path += @"config.json";
                using (TextReader iniFile = new StreamReader(path))
                {
                    result = iniFile.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
                result = "error";
            }
            return result;
        }

        public static string FetchTableColumns(string method)
        {
            string stack = "Utility/getSettings() --> " + method;
            string result = "error";
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"\");
                path += @"config.json";
                using (TextReader iniFile = new StreamReader(path))
                {
                    result = iniFile.ReadToEnd();
                }

                JObject json = JObject.Parse(result);
                var tempjson = json["Table_Columns"];
                result = JsonConvert.SerializeObject(tempjson);
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
                result = "error";
            }
            return result;
        }

        public static string GetCourses(string userid, string idtype, string method)
        {
            string stack = "(Utility)GetCourses() --> " + method;
            string result = "Error";
            try
            {
                DB_class dbconn = new DB_class();
                string sqlstr = "";
                DataTable dt = null;
                if(idtype == "student")
                {
                    sqlstr = "select co.* from Course as co ";
                    sqlstr += "inner join Class_Register as cr on cr.course_id = co.course_uid and cr.student_id = '"+ userid +"' ";
                    sqlstr += "where co.active = 1";

                    dt = dbconn.ExecuteTable(sqlstr, stack);
                }
                else if (idtype == "professor")
                {
                    sqlstr = "select * from Course where professor_id='"+ userid +"' and active=1";
                    dt = dbconn.ExecuteTable(sqlstr, stack);
                }

                if(dt != null && !dt.Equals(DBNull.Value) )
                {
                    result = Json_string_Courses(dt, stack);
                }

            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }
            return result;
        }
        
        private static string Json_string_Courses(DataTable dt, string method)
        {
            string stack = "(Utility)Json_string_Courses() --> " + method;
            string result = "Error";
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(2000);
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter w = new JsonTextWriter(sw))
                {
                    w.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

                    //w.WriteStartObject();
                    //w.WritePropertyName("User_Profile");
                    w.WriteStartObject();
                    w.WritePropertyName("Courses");
                    w.WriteStartArray();

                    for(int i=0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];

                        //w.WriteStartObject();
                        //w.WritePropertyName(dr["course_uid"].ToString());

                        w.WriteStartObject();
                        w.WritePropertyName("CourseID");
                        w.WriteValue(dr["course_uid"].ToString());
                        w.WritePropertyName("CourseName");
                        w.WriteValue(dr["course_name"].ToString());
                        w.WritePropertyName("CourseCode");
                        w.WriteValue(dr["course_no"].ToString());
                        w.WriteEndObject();

                        //w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();

                    //w.WriteEndObject();
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                //result = "error";
                AppendLog(stack, ex.Message);
            }
            return result;
        }
        
        public static string GetStudentList(string userid, string idtype, string courseid, string method)
        {
            string stack = "(Utility)GetCourses() --> " + method;
            string result = "Error";
            try
            {
                DB_class dbconn = new DB_class();
                string sqlstr = "";
                DataTable dt = null;
                if (idtype == "student")
                {
                    sqlstr = "select st.* from Student as st inner join Class_Register as cr on cr.student_id=st.student_id and cr.course_id="+ courseid
                        +" where st.student_id='"+ userid +"'";
                    
                    dt = dbconn.ExecuteTable(sqlstr, stack);
                }
                else if (idtype == "professor")
                {
                    sqlstr = "select st.* from Student as st inner join Class_Register as cr on cr.student_id=st.student_id and cr.course_id=" + courseid;
                    dt = dbconn.ExecuteTable(sqlstr, stack);
                }

                if (dt.Rows.Count > 0)
                {
                    result = Json_string_Student_List(dt, idtype, stack);
                }

            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        private static string Json_string_Student_List(DataTable dt, string idtype, string method)
        {
            string stack = "(Utility)Json_string_Student_List() --> " + method;
            string result = "Error";
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(2000);
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter w = new JsonTextWriter(sw))
                {
                    w.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

                    //w.WriteStartObject();
                    //w.WritePropertyName("User_Profile");
                    w.WriteStartObject();
                    w.WritePropertyName("Students");
                    w.WriteStartArray();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        w.WriteStartObject();
                        w.WritePropertyName("StudentID");
                        w.WriteValue(dr["student_id"].ToString());
                        w.WritePropertyName("StudentName");
                        w.WriteValue(dr["last_name"].ToString() + " " + dr["first_name"].ToString());
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();

                    //w.WriteEndObject();
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                //result = "error";
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        #endregion

        #region login

        public static string Auth_Login(string userid, string pass, string id_type, string method)
        {
            string stack = "(Utility)Auth_Login() --> " + method;
            string result = "";

            try
            {
                if (id_type == "student")
                {
                    if(pass == "stu123")
                    {
                        string sqlstr = "select * from Student where student_id = ? and active = 1";
                        object[] parameters = { userid };

                        DB_class dbconn = new DB_class();
                        DataRow dr = (dbconn.ExecuteTable(sqlstr, stack, parameters)).Rows[0];
                        if(dr != null)
                        {
                            result = Json_string_profile(dr, id_type, stack);
                        }
                    }
                    else
                    {
                        result = "|E|Invalid Login";
                    }
                }
                else if (id_type == "professor")
                {
                    if (pass == "proff123")
                    {
                        string sqlstr = "select * from Professor where professor_id = ? and active = 1";
                        object[] parameters = { userid };

                        DB_class dbconn = new DB_class();
                        DataRow dr = (dbconn.ExecuteTable(sqlstr, stack, parameters)).Rows[0];
                        if (dr != null)
                        {
                            result = Json_string_profile(dr, id_type, stack);
                        }
                    }
                    else
                    {
                        result = "|E|Invalid Login";
                    }
                }
                else
                {
                    result = "Error: Unable to authenticate user.";
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        private static string Json_string_profile(DataRow dr, string idtype, string method)
        {
            string stack = "(Utility)create_json_profile() --> " + method;
            string result = "";
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(2000);
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter w = new JsonTextWriter(sw))
                {
                    w.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

                    //w.WriteStartObject();
                    //w.WritePropertyName("User_Profile");
                    w.WriteStartObject();

                    w.WritePropertyName("UserID");
                    if(idtype == "student")
                        w.WriteValue(dr["student_id"].ToString());
                    else
                        w.WriteValue(dr["professor_id"].ToString());

                    w.WritePropertyName("UserName");
                    w.WriteValue(dr["first_name"].ToString() + " "+ dr["last_name"].ToString());

                    w.WritePropertyName("ID_Type");
                    w.WriteValue(idtype);

                    w.WriteEndObject();

                    //w.WriteEndObject();
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                result = "error";
                AppendLog(stack, ex.Message);
            }
            return result;
        }

        #endregion

        #region OTP

        public static string GenerateOTP(string userid, string classid, string datetime, string method)
        {
            string stack = "(UtilityCS)GenerateOTP() --> " + method;
            string result = "Error";
            try
            {
                DB_class dbconn = new DB_class();

                //DateTime a = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                //a = a.AddMilliseconds(long.Parse(datetime));  //.ToLocalTime();                
                //xx.AddMilliseconds(double.Parse(datetime));

                string sqlstr = "select count(*) from Course  where professor_id = '" + userid + "' and course_uid = " + classid + " and active = 1";
                if (dbconn.ExecuteScalar(sqlstr, stack) == "1")
                {
                    Random generator = new Random();
                    string r = generator.Next(0, 10000).ToString("D4");

                    sqlstr = "INSERT INTO [One_Time_Pass] (otp, course_id, professor_id, start_time, create_time, active) ";
                    sqlstr += "VALUES('"+ r +"', "+ classid +", '"+ userid +"', GETDATE(), '"+ datetime +"', 1)";
                    if (dbconn.NonQuery(sqlstr,stack) == 1)
                    {
                        result = r;

                        try
                        {
                            sqlstr = "UPDATE One_Time_Pass SET is_valid=0, active=0, stop_time=GETDATE() "
                                +"WHERE DATEADD(hh,1,start_time)>GETDATE() AND start_time<GETDATE() AND otp<>"+ r +" AND course_id="+ classid;
                            int x = dbconn.NonQuery(sqlstr, stack);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog(stack, ex.Message);
                //result = "Error";
            }

            return result;
        }
        
        #endregion

    }
}