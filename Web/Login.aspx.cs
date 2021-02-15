using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace gradproj_webapp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DB_class x = new DB_class();
        }

        [WebMethod]
        public static string Auth_Login(string userid, string pass, string idtype, string method)
        {
            string stack = "Home/Auth_Login() --> " + method; ;
            string result = "";

            try
            {
                if (userid.Length > 1)
                {
                    result = Utility.Auth_Login(userid, pass, idtype, method);
                }
                else
                {
                    result = "Error: invalid login";
                }
            }
            catch (Exception ex)
            {
                Utility.AppendLog(stack, ex.Message);
            }

            return result;
        }

    }
}