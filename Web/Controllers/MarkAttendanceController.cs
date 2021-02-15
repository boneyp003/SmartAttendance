using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gradproj_webapp.Controllers
{
    public class MarkAttendanceController : ApiController
    {
        // GET: api/MarkAttendance
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MarkAttendance/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MarkAttendance
        public IOClass.MarkAttendanceOut Post([FromBody]IOClass.MarkAttendanceIn value)
        {
            string stack = "defaultservice/MarkAttendance() --> StudentID: " + value.StudentID;
            IOClass.MarkAttendanceOut result = new IOClass.MarkAttendanceOut
            {
                Result = "Error"
            };
            try
            {
                result.Result = Utility.MarkAttendance(value.StudentID, value.TimeStamp, value.OneTP, value.Longitude, value.Latitude, value.RegNo, stack); 
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message.ToString());
                //result = "Error";
            }
            return result;
        }

        // PUT: api/MarkAttendance/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/MarkAttendance/5
        public void Delete(int id)
        {
        }
    }
}
