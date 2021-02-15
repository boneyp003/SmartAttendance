using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Data.Sql;
using System.Linq;
using System.Web;
namespace gradproj_webapp
{
    public class DB_class
    {
        //public string ConnectionString { get; set; }
        //private string constr;
        //private string dbstack;

        private OdbcConnection conn;

        public DB_class()
        {
            GetConnStr();
        }


        #region private methods

        private void GetConnStr()
        {
            string stack = "(DB_class)getConnStr()";
            try
            {
                string temp_json_string = "";
                string path = HttpContext.Current.Server.MapPath(@"\");
                path += @"config.json";
                using (TextReader iniFile = new StreamReader(path))
                {
                    temp_json_string = iniFile.ReadToEnd();
                }

                JObject json = JObject.Parse(temp_json_string);
                string dsname = (json["Active_DSN"]).ToString();
                string connstrr = (((JArray)json["ConnectionStrings"])[0]["value"]).ToString();

                //if (connstrr != null && connstrr != "")
                //{
                //    constr = ("dsn=" + connstrr + ";");
                //}
                conn = new OdbcConnection(connstrr);
                //conn.Open();
                //bool yyy = true;
                //conn.Close();
            }
            catch (Exception ex)
            {
#pragma warning disable CS0436 // Type conflicts with imported type
                Utility.AppendLog(stack, "Unable to establish connection: " + ex.Message);
#pragma warning restore CS0436 // Type conflicts with imported type
            }
        }

        private string AppendParameters(object[] parameters)
        {
            string result = string.Empty;
            if (parameters != null)
            {
                result += "   \r\nParameters: ";
                for (int i = 0; i < parameters.Length; i++)
                {
                    result += parameters[i].ToString() + ", ";
                }
            }
            return result;
        }

        #endregion

        public bool TestConn()
        {
            bool result = false;
            //OdbcConnection conn = null;
            try
            {
                //conn = new OdbcConnection(constr);
                string sqlstr = "select top 1 name from testable";
                OdbcCommand cmd = new OdbcCommand(sqlstr, conn);
                conn.Open();
                string x = cmd.ExecuteScalar().ToString();
                result = true;
                //conn.Open();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public string ExecuteScalar(string sqlstr, string method)
        {
            string stack = "(DB_class)executeScalar() --> " + method;
            string result = "";
            try
            {
                OdbcCommand cmd = new OdbcCommand(sqlstr, conn);
                conn.Open();
                result = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                result = "Error";
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public DataTable ExecuteTable(string selectString, string method, object[] parameters = null)
        {
            DataTable dt = new DataTable();

            string stack = "(DB_Class)executeTable() --> " + method + "  \r\n Query: " + selectString;
            stack += AppendParameters(parameters);

            if (parameters == null)
            {
                conn.Open();

                using (var cmd = new OdbcCommand(selectString, conn))
                {
                    using (var oda = new OdbcDataAdapter(cmd))
                    {
                        try
                        {
                            oda.Fill(dt);
                            //dt.Load(reader);
                        }
                        catch (Exception ex)
                        {
#pragma warning disable CS0436 // Type conflicts with imported type
                            Utility.AppendLog("Error while performing da.Fill(" + selectString + "); --> " + stack, ex.Message);
#pragma warning restore CS0436 // Type conflicts with imported type
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                
            }
            else
            {
                string current = "";

                conn.Open();

                using (var cmd = new OdbcCommand(selectString, conn))
                {
                    using (var oda = new OdbcDataAdapter(cmd))
                    {

                        try
                        {
                            foreach (var p in parameters)
                            {
                                current += p.ToString() + ", ";

                                cmd.Parameters.AddWithValue("?", p);
                                //oda.SelectCommand.Parameters.AddWithValue("?", p);
                            }   
                            oda.Fill(dt);
                        }
                        catch (Exception ex)
                        {
                            string paramterResults = "";
                            for (int i = 0; i < oda.SelectCommand.Parameters.Count - 1; ++i)
                            {
                                paramterResults += oda.SelectCommand.Parameters[i].Value.GetType() + ": " + oda.SelectCommand.Parameters[i].Value.ToString() + ", ";
                            }
                            stack += paramterResults;
#pragma warning disable CS0436 // Type conflicts with imported type
                            Utility.AppendLog("Error while performing da.Fill(" + selectString + "); --> " + stack, ex.Message.ToString());
#pragma warning restore CS0436 // Type conflicts with imported type
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return dt;
        }

        public int NonQuery(string dbString, string method)
        {
            string stack = "(DB_class)NonQuery() --> " + method;
            int id = 0;

            try
            {
                conn.Open();
                OdbcCommand cmd = new OdbcCommand(dbString, conn);
                id = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //IOClass.appendLog("Error while performing cmd.ExecuteNonQuery() --> nonQuery --> " + methodName, ex.Message.ToString());
                Utility.AppendLog(stack, ex.Message.ToString());
                id = 0;
            }
            finally
            {
                conn.Close();
            }

            //cmd.CommandText = "SELECT @@IDENTITY";

            //try
            //{
            //    id = Convert.ToInt32(cmd.ExecuteScalar());
            //}
            //catch (Exception)
            //{
            //    //IOClass.appendLog("Error while performing id = Convert.ToInt32(cmd.ExecuteScalar()) --> nonQuery --> " + methodName, ex.Message.ToString());
            //    id = 0;
            //}

            
            
            return id;
        }
    }
}