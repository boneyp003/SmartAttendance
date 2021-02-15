using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gradproj_webapp
{
    public partial class home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void testserv()
        {

            
        }

        [WebMethod] 
        public static string GenerateOTP(string userid, string classid, string datetime, string method)
        {
            string stack = "Home/generateOTP() --> " + method;
            string result = "Error";
            try
            {
                result = Utility.GenerateOTP(userid, classid, datetime, stack);
            }   
            catch(Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
                //result = "Error";
            }

            return result;
        }

        [WebMethod]
        public static string GetSettings(string method)
        {
            string stack = "Home/getSettings() --> " + method;
            string result = "error";
            try
            {
                result = Utility.getSettings(stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
                result = "error";
            }
            return result;

        }

        [WebMethod]
        public static string FetchTableColumns(string method)
        {
            string stack = "Home/fetchTableColumns() --> " + method;
            string result = "error";
            try
            {
                result = Utility.FetchTableColumns(stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
                result = "error";
            }
            return result;
        }

        [WebMethod]
        public static string GetCourses(string userid, string idtype, string method)
        {
            string stack = "(HomeASPX)GetCourses() --> " + method;
            string result = "Error";
            try
            {
                result = Utility.GetCourses(userid, idtype, stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
            }
            return result;
        }

        [WebMethod]
        public static string GetStudentList(string userid, string idtype, string courseid, string method)
        {
            string stack = "(HomeASPX)GetStudentList() --> " + method;
            string result = "Error";
            try
            {
                result = Utility.GetStudentList(userid, idtype, courseid, stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
            }
            return result;
        }

    }
}