using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Classes;

namespace TryXamarinApp.Utils
{
    public class DbComunication
    {
        public static readonly string MirrorId = (App.Current as App).PersonGroup;
        private static HttpClient _client = null;
        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                }
                return _client;
            }
        }

        public static async Task<bool> VerifyPassword(Person person, string password)
        {
            string auth = PasswordHandler.Hash(password);
            string url = (App.Current as App).BaseUrl + @"/api/VerifyPassword?" + $"mirrorId={MirrorId}&username={person.Name}&auth={auth}";
            var resp = await Client.GetAsync(url);
            var content = await resp.Content.ReadAsStringAsync();
            bool res;
            bool convert = bool.TryParse(content, out res);
            return convert && res;   
        }

        public static async Task<bool> ChangePassword(Person person, string oldPassword, string newPassword)
        {
            string oldAuth = PasswordHandler.Hash(oldPassword);
            string newAuth = PasswordHandler.Hash(newPassword);
            string url = (App.Current as App).BaseUrl + @"/api/ChangePassword?" + $"mirrorId={MirrorId}&username={person.Name}&curAuth={oldAuth}&newAuth={newAuth}";
            var resp = await Client.GetAsync(url);
            var content = await resp.Content.ReadAsStringAsync();
            bool res;
            bool convert = bool.TryParse(content, out res);
            return convert && res;
        }

        public static async Task<List<Message>> GetMessagesForUser(Guid userId)
        {

            string url = (App.Current as App).BaseUrl + @"/api/SqlSelectMessages?" + $"mirrorId={MirrorId}&userId={userId}";

            var resp = await Client.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<Message>>(json);

            return messages;
        }

        public static async Task<int> InsertMessageUser(Message message)
        {
            var jsonMsg = JsonConvert.SerializeObject(message);

            string url = (App.Current as App).BaseUrl + @"/api/SqlInsertMessage?" + $"mirrorId={MirrorId}";

            var data = new StringContent(jsonMsg);
            var resp = await Client.PostAsync(url, data);

            var content = await resp.Content.ReadAsStringAsync();

            return int.Parse(content);
        }
        public static async Task<int> DeleteMessageUser(Message message)
        {
            var jsonMsg = JsonConvert.SerializeObject(message);

            string url = (App.Current as App).BaseUrl + @"/api/SqlDeleteMessage?" + $"mirrorId={MirrorId}";

            var data = new StringContent(jsonMsg);
            var resp = await Client.PostAsync(url, data);

            var content = await resp.Content.ReadAsStringAsync();

            return int.Parse(content);
        }

        public static async Task<List<Reminder>> GetRemindersForUser(string username)
        {
            string url = (App.Current as App).BaseUrl + @"/api/SqlSelectReminders?" + $"mirrorId={MirrorId}&username={username}";

            var resp = await Client.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            var reminders = JsonConvert.DeserializeObject<List<Reminder>>(json);

            return reminders;
        }

        public static async Task<int> InsertReminderUser(Reminder reminder)
        {
            var jsonMsg = JsonConvert.SerializeObject(reminder);

            string url = (App.Current as App).BaseUrl + @"/api/SqlInsertReminder?" + $"mirrorId={MirrorId}";

            var data = new StringContent(jsonMsg);
            var resp = await Client.PostAsync(url, data);

            var content = await resp.Content.ReadAsStringAsync();

            return int.Parse(content);
        }
        public static async Task<int> DeleteReminderUser(Reminder reminder)
        {
            var jsonMsg = JsonConvert.SerializeObject(reminder);

            string url = (App.Current as App).BaseUrl + @"/api/SqlDeleteReminder?" + $"mirrorId={MirrorId}";

            var data = new StringContent(jsonMsg);
            var resp = await Client.PostAsync(url, data);
        
            var content = await resp.Content.ReadAsStringAsync();

            return int.Parse(content);
        }
    }


}
