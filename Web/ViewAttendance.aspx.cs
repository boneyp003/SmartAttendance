using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gradproj_webapp
{
    public partial class ViewAttendance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        public static string GetAttendance(string userid, string idtype, string courseid, string studentid, string method)
        {
            string stack = "(HomeASPX)GetStudentList() --> " + method;
            string result = "Error";
            try
            {
                result = Utility.GetAttendance(userid, idtype, courseid, studentid, stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
            }
            return result;
        }



    }
}