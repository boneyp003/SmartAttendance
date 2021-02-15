using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gradproj_webapp.Controllers
{
    public class CheckRegistrationController : ApiController
    {
        // GET: api/CheckRegistration
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CheckRegistration/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CheckRegistration
        public IOClass.CheckRegistrationModel Post([FromBody]IOClass.CheckRegistrationModel value)
        {
            string stack = "defaultservice/MarkAttendance() --> StudentID: " + value.StudentID;
            IOClass.CheckRegistrationModel result = new IOClass.CheckRegistrationModel();
            result.StudentID = "Error";
            result.DeviceID = "Error";
            
            try
            {
                result = Utility.CheckDevice(value.StudentID, value.DeviceID, stack);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message.ToString());
                //result = "Error";
            }
            return result;
        }

        // PUT: api/CheckRegistration/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CheckRegistration/5
        public void Delete(int id)
        {
        }
    }
}
