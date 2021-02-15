using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gradproj_webapp.Controllers
{
    public class RegisterDeviceController : ApiController
    {
        // GET: api/RegisterDevice
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RegisterDevice/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/RegisterDevice
        public IOClass.RegisterDeviceOut Post([FromBody]IOClass.RegisterDeviceIn value)
        {
            string stack = "defaultservice/RegisterDevice() --> StudentID: ";
            string result = "";
            IOClass.RegisterDeviceOut testout = new IOClass.RegisterDeviceOut();
            try
            {
                //JObject tempjson = JObject.Parse(value);
                string StudentID = value.StudentID;     //tempjson["StudentID"].ToString();    
                string email = value.email;        //tempjson["email"].ToString();   
                string regTime = value.regTime;       //tempjson["regTime"].ToString();    
                testout = Utility.RegisterDevice(StudentID, email, regTime, stack + StudentID);
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message.ToString());
            }
            return testout;
        }

        // PUT: api/RegisterDevice/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RegisterDevice/5
        public void Delete(int id)
        {
        }
    }
}
