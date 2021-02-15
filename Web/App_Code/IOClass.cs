using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gradproj_webapp
{
    public class IOClass
    {
        public class RegisterDeviceIn
        {
            public string StudentID { set; get; }
            public string email { set; get; }
            public string regTime { set; get; }
        }

        public class RegisterDeviceOut
        {
            public string StudentID { set; get; }
            public string StudentName { set; get; }
            public string DeviceID { set; get; }
        }

        public class MarkAttendanceIn
        {
            public string StudentID { set; get; }
            public string TimeStamp { set; get; }
            public string OneTP { set; get; }
            public string Longitude { set; get; }
            public string Latitude { set; get; }
            public string RegNo { set; get; }
        }

        public class MarkAttendanceOut
        {
            public string Result { set; get; }
        }

        public class CheckRegistrationModel
        {
            public string StudentID { set; get; }
            public string DeviceID { set; get; }
        }
    }
}