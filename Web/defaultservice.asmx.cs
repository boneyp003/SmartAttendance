using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace gradproj_webapp
{
    /// <summary>
    /// Summary description for defaultservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class defaultservice : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld(string name)
        {

            return "Hey there " + name + ". This is hosted web service!!!";
        }

        [WebMethod]
        public string RegisterDevice(string studentID, string email, string regTime)
        //public string RegisterDevice(string input)
        {
            string stack = "defaultservice/RegisterDevice() --> StudentID: " + studentID;
            string result = "";
            try
            {
                //result = Utility.RegisterDevice(studentID, email, regTime, stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message.ToString());
            }
            return result;
        }

        [WebMethod]
        public string MarkAttendance(string studentID, string timeStamp, string oneTP, string longitude, string latitude, string regNo)
        {
            string stack = "defaultservice/MarkAttendance() --> StudentID: " + studentID;
            string result = "";
            try
            {
                result = Utility.MarkAttendance(studentID, timeStamp, oneTP, longitude, latitude, regNo, stack);
            }
            catch(Exception ex)
            {
                Utility.AppendLog(stack, ex.Message.ToString());
                result = "Error";
            }
            return result;
        }

        [WebMethod]
        public void AppendAjaxLog(string method, string exception)
        {            
            Utility.AppendLog(method, exception);
        }
    }
}
