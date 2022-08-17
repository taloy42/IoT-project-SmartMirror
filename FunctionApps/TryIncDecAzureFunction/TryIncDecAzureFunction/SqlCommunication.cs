using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections.Generic;

namespace TryIncDecAzureFunction
{
    public static class SqlCommunication
    {

        readonly static string DataSource = @"messages.database.windows.net";//your server
        readonly static string Database = "messages"; //your database name
        readonly static string Username = "MSGadmin"; //username of server to connect
        readonly static string Password = "zubur123!"; //password

        readonly static string ConnectionString = $"Data Source={DataSource};Initial Catalog="
                        + $"{Database};Persist Security Info=True;User ID={Username};Password={Password}";

        public static readonly string MESSAGES_TABLE_NAME = "messages";
        public static readonly string REMINDERS_TABLE_NAME = "reminders";
        public static readonly string AUTH_TABLE_NAME = "auth";

        [FunctionName("SqlInsertMessage")]
        public static async Task<int> SqlInsertMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql INSERT");

            string mirrorId = req.Query["mirrorId"];
            string jsonMsg = req.Query["jsonMsg"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            jsonMsg = jsonMsg ?? data?.jsonMsg ?? requestBody;

            string tablename = GetTableName(MESSAGES_TABLE_NAME, mirrorId);

            Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);

            int rows_affected = InsertIntoQuery(tablename,msg);

            return rows_affected;
        }

        [FunctionName("SqlSelectMessages")]
        public static async Task<string> SqlSelectMessages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            /*
             * input - 
             *   
             *  output -
             *      json of a list of messeges where @username is the reciever
             */
            log.LogInformation("C# HTTP trigger Sql SELECT");

            string mirrorId = req.Query["mirrorId"];
            string userId = req.Query["userId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;
            mirrorId = mirrorId ?? data?.mirrorId;

            string tablename = GetTableName(MESSAGES_TABLE_NAME, mirrorId);

            var messages = SelectFromWhereJson<Message>(tablename,$"recieverID='{userId}'");

            return messages;
        }
        public static async Task<string> SqlSelectAdmins()
        {

            string tablename = GetTableName(AUTH_TABLE_NAME, "admin");

            var admins = SelectFromWhereJson<Auth>(tablename);

            return admins;
        }

        [FunctionName("SqlDeleteMessage")]
        public static async Task<int> SqlDeleteMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string mirrorId = req.Query["mirrorId"];
            string jsonMsg = req.Query["jsonMsg"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            jsonMsg = jsonMsg ?? data?.jsonMsg ?? requestBody;

            string tablename = GetTableName(MESSAGES_TABLE_NAME, mirrorId);

            Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);
            
            
            int rows_affected = DeleteFrom(tablename, msg);

            return rows_affected;
        }

        [FunctionName("DeleteAdmin")]
        public static async Task<int> DeleteAdminHTTP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string username = req.Query["username"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            username = username ?? data?.username;

            string tablename = GetTableName(AUTH_TABLE_NAME, "admin");


            int rows_affected = DeleteFromWhere(tablename, $"username='{username}'");

            return rows_affected;
        }

        [FunctionName("SqlInsertReminder")]
        public static async Task<int> SqlInsertReminder(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql INSERT");

            string mirrorId = req.Query["mirrorId"];
            string jsonRmndr = req.Query["jsonReminder"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            jsonRmndr = jsonRmndr ?? data?.jsonReminder ?? requestBody;


            string tablename = GetTableName(REMINDERS_TABLE_NAME, mirrorId);

            Reminder reminder = JsonConvert.DeserializeObject<Reminder>(jsonRmndr);

            int rows_affected = InsertIntoQuery(tablename, reminder);

            return rows_affected;
        }

        [FunctionName("SqlSelectReminders")]
        public static async Task<string> SqlSelectReminders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            /*
             * input - 
             *   
             *  output -
             *      json of a list of messeges where @username is the reciever
             */
            log.LogInformation("C# HTTP trigger Sql SELECT");

            string mirrorId = req.Query["mirrorId"];
            string username = req.Query["username"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            username = username ?? data?.username;
            mirrorId = mirrorId ?? data?.mirrorId;

            string tablename = GetTableName(REMINDERS_TABLE_NAME, mirrorId);

            var messages = SelectFromWhereJson<Reminder>(tablename, $"username='{username}'");

            return messages;
        }

        [FunctionName("SqlDeleteReminder")]
        public static async Task<int> SqlDeleteReminder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string mirrorId = req.Query["mirrorId"];
            string jsonReminder = req.Query["jsonReminder"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            jsonReminder = jsonReminder ?? data?.jsonReminder ?? requestBody;

            string tablename = GetTableName(REMINDERS_TABLE_NAME, mirrorId);

            Reminder reminder = JsonConvert.DeserializeObject<Reminder>(jsonReminder);


            int rows_affected = DeleteFrom(tablename, reminder);

            return rows_affected;
        }

        [FunctionName("VerifyPassword")]
        public static async Task<bool> VerifyPasswordForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string mirrorId = req.Query["mirrorId"];
            string username = req.Query["username"];
            string auth = req.Query["auth"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            username = username ?? data?.username;
            auth = auth ?? data?.auth;

            string tablename = GetTableName(AUTH_TABLE_NAME, mirrorId);
            var l = SelectFromWhere<Auth>(tablename, $"username='{username}'");
            if (l.Count==0) return false;
            Auth tableAuth = l[0];
            return tableAuth.pwauth.Equals(auth);
        }

        [FunctionName("ChangePassword")]
        public static async Task<bool> ChangePasswordForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string mirrorId = req.Query["mirrorId"];
            string username = req.Query["username"];
            string curAuth = req.Query["curAuth"];
            string newAuth = req.Query["newAuth"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            mirrorId = mirrorId ?? data?.mirrorId;
            username = username ?? data?.username;
            curAuth = curAuth ?? data?.curAuth;
            newAuth = newAuth ?? data?.newAuth;

            string tablename = GetTableName(AUTH_TABLE_NAME, mirrorId);

            Auth tableAuth = SelectFromWhere<Auth>(tablename, $"username='{username}'")[0];
            if (tableAuth.pwauth.Equals(curAuth))
            {
                Auth auth = new Auth();
                auth.userid = tableAuth.userid;
                auth.username = tableAuth.username;
                auth.pwauth = newAuth;
                int rows_affected = UpdateEntry(tablename, tableAuth, auth);
                return rows_affected > 0;
            }
            return false;
        }
        [FunctionName("VerifyAdmin")]
        public static async Task<bool> VerifyAdmin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sql DELETE");

            string username = req.Query["username"];
            string auth = req.Query["auth"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            username = username ?? data?.username;
            auth = auth ?? data?.curAuth;

            string tablename = GetTableName(AUTH_TABLE_NAME, "admin");
            var l = SelectFromWhere<Auth>(tablename, $"username='{username}'");
            if (l.Count == 0) return false;
            Auth tableAuth = l[0];
            return tableAuth.pwauth.Equals(auth);
        }


        private static string[] getMembersOfType(object obj)
        {
            List<string> members = new List<string>();
            foreach (var item in obj.GetType().GetMembers())
            {
                if (item.MemberType == MemberTypes.Property)
                {
                    members.Add(item.Name);
                }
            }

            return members.ToArray();
        }
        public static List<T> SelectFromWhere<T>(string tablename, string condition = "1=1") where T : class, new()
        {
            //var rowTemplate = new { timestamp = DateTime.MinValue, sender = "", reciver = "", message = "" };

            string[] members = getMembersOfType(new T());
            string fields = string.Join(", ", members);

            List<T> list = new List<T>();
            string query = $"SELECT {fields} FROM {tablename} WHERE {condition};";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var t = DataReaderTo<T>(reader);
                        list.Add(t);
                    }
                }
            }

            return list;
        }

        //private static void ExecuteQuery(string query)
        //{
        //    using (SqlConnection conn = new SqlConnection(ConnectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}
        private static string SelectFromWhereJson<T>(string tablename, string condition = "1=1") where T : class, new()
        {
            var list = SelectFromWhere<T>(tablename, condition);
            string json = JsonConvert.SerializeObject(list);

            return json;
        }

        public static int InsertIntoQuery<T>(string tablename, T obj) where T : class, new()
        {
            string[] members = getMembersOfType(new T());
            string fields = string.Join(", ", members);
            string[] valStrs = new string[members.Length];
            //string date_format = "yyyyMMdd hh:mm:ss tt";
            //for (int i = 0; i < vals.Length; i++)
            //{
            //    object val = obj.GetType().GetProperty(members[i]).GetValue(obj);
            //    if (val.GetType() == typeof(DateTime))
            //    {
            //        valStrs[i] = $"'{((DateTime)val).ToString(date_format)}'";
            //    } else if (val.GetType() == typeof(string))
            //    {
            //        valStrs[i] = $"'{(string)val}'";
            //    } else //hope for the best
            //    {
            //        valStrs[i] = val.ToString();
            //    }

            //}

            for (int i = 0; i < members.Length; i++)
            {
                valStrs[i] = $"@{members[i]}";
            }

            string values = string.Join(", ", valStrs);
            string query = $"INSERT INTO {tablename} ({fields}) VALUES ({values});";
            int rows_affected = -1;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {

                    for (int i = 0; i < members.Length; i++)
                    {
                        command.Parameters.Add(new SqlParameter(members[i], obj.GetType().GetProperty(members[i]).GetValue(obj)));
                    }
                    rows_affected = command.ExecuteNonQuery();
                }
            }
            return rows_affected;
        }

        private static int UpdateEntry<T>(string tablename, T oldObj, T newObj) where T : class, new()
        {
            string[] members = getMembersOfType(new T());
            string fields = string.Join(", ", members);
            string[] valSetStrs = new string[members.Length];
            string[] valCondStrs = new string[members.Length];
            //string date_format = "yyyyMMdd hh:mm:ss tt";
            //for (int i = 0; i < vals.Length; i++)
            //{
            //    object val = obj.GetType().GetProperty(members[i]).GetValue(obj);
            //    if (val.GetType() == typeof(DateTime))
            //    {
            //        valStrs[i] = $"'{((DateTime)val).ToString(date_format)}'";
            //    } else if (val.GetType() == typeof(string))
            //    {
            //        valStrs[i] = $"'{(string)val}'";
            //    } else //hope for the best
            //    {
            //        valStrs[i] = val.ToString();
            //    }

            //}

            for (int i = 0; i < members.Length; i++)
            {
                valSetStrs[i] = $"{members[i]}=@{members[i]}Set";
                valCondStrs[i] = $"{members[i]}=@{members[i]}Cond";
            }

            string valuesSet = string.Join(", ", valSetStrs);
            string valuesCond = string.Join(" AND ", valCondStrs);
            string query = $"UPDATE {tablename} SET {valuesSet} WHERE ({valuesCond});";
            int rows_affected = -1;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {

                    for (int i = 0; i < members.Length; i++)
                    {
                        command.Parameters.Add(new SqlParameter(members[i] + "Set", newObj.GetType().GetProperty(members[i]).GetValue(newObj)));
                        command.Parameters.Add(new SqlParameter(members[i]+"Cond", oldObj.GetType().GetProperty(members[i]).GetValue(oldObj)));
                    }
                    rows_affected = command.ExecuteNonQuery();
                }
            }
            return rows_affected;
        }
        private static int DeleteFrom<T>(string tablename, T obj) where T : class, new()
        {
            string[] members = getMembersOfType(new T());
            string[] conditionStrings = new string[members.Length];
            //string date_format = "yyyyMMdd hh:mm:ss tt";

            for (int i = 0; i < members.Length; i++)
            {
                conditionStrings[i] = $"({members[i]}=@{members[i]})";
            }

            string conditions = string.Join(" AND ", conditionStrings);
            //string conditions = $"timestamp = @timestamp";
            string query = $"DELETE FROM {tablename} WHERE ({conditions});";
            int rows_affected = -1;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {

                    for (int i = 0; i < members.Length; i++)
                    {
                        //command.Parameters.Add(new SqlParameter(members[i], obj.GetType().GetProperty(members[i]).GetValue(obj)));
                        command.Parameters.AddWithValue(members[i], obj.GetType().GetProperty(members[i]).GetValue(obj));
                    }
                    //command.Parameters.Add(new SqlParameter("timestamp", (obj as Message).timestamp));
                    rows_affected = command.ExecuteNonQuery();
                }
            }

            return rows_affected;
        }

        private static int DeleteFromWhere(string tablename, string cond)
        {
            

            //string conditions = $"timestamp = @timestamp";
            string query = $"DELETE FROM {tablename} WHERE ({cond});";
            int rows_affected = -1;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {

                    
                    //command.Parameters.Add(new SqlParameter("timestamp", (obj as Message).timestamp));
                    rows_affected = command.ExecuteNonQuery();
                }
            }

            return rows_affected;
        }

        private static T DataReaderTo<T>(IDataReader dataReader) where T : class, new()
        {
            T t = new T();
            string[] properties = getMembersOfType(t);
            for (int i = 0; i < properties.Length; i++)
            {
                t.GetType().GetProperty(properties[i])?.SetValue(t, dataReader[i]);
            }

            return t;

        }

        public static string GetTableName(string tablename, string mirrorId)
        {
            return $"{mirrorId}_{tablename}";
        }


        public static void CreatePersonGroupTables(string personGroupId)
        {
            string queryMessages = $"CREATE TABLE {GetTableName(MESSAGES_TABLE_NAME, personGroupId)} (timestamp datetime, sender varchar(max), reciever varchar(max), message varchar(max), recieverID uniqueidentifier, senderID uniqueidentifier);";
            string queryReminders = $"CREATE TABLE {GetTableName(REMINDERS_TABLE_NAME, personGroupId)} (username varchar(max), start_time datetime, end_time datetime, reminder varchar(max), userid uniqueidentifier);";
            string queryAuth = $"CREATE TABLE {GetTableName(AUTH_TABLE_NAME, personGroupId)} (userid uniqueidentifier, username varchar(max), pwauth varchar(max));";
            
            string[] queries = {queryMessages, queryReminders, queryAuth};
            
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (var query in queries)
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void DeletePersonGroupTables(string personGroupId)
        {
            string queryMessages = $"DROP TABLE {GetTableName(MESSAGES_TABLE_NAME, personGroupId)};";
            string queryReminders = $"CREATE TABLE {GetTableName(REMINDERS_TABLE_NAME, personGroupId)};";
            string queryAuth = $"CREATE TABLE {GetTableName(AUTH_TABLE_NAME, personGroupId)};";         

            string[] queries = { queryMessages, queryReminders, queryAuth };

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (var query in queries)
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
