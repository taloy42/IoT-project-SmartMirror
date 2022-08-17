using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminApp
{
    public class DbCommunication
    {
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
        public static async Task<bool> VerifyAdmin(string username, string password)
        {
            string auth = PasswordHandler.Hash(password);
            string url = Constants.BaseUrl + @"/api/VerifyAdmin?" + $"username={username}&auth={auth}";
            var resp = await Client.GetAsync(url);
            var content = await resp.Content.ReadAsStringAsync();
            bool res;
            bool convert = bool.TryParse(content, out res);
            return convert && res;
        }
        public static async Task<bool> ChangePassword(Admin admin, string oldPassword, string newPassword)
        {
            string oldAuth = PasswordHandler.Hash(oldPassword);
            string newAuth = PasswordHandler.Hash(newPassword);
            string url = Constants.BaseUrl + @"/api/ChangePassword?" + $"mirrorId=admin&username={admin.username}&curAuth={oldAuth}&newAuth={newAuth}";
            var resp = await Client.GetAsync(url);
            var content = await resp.Content.ReadAsStringAsync();
            bool res;
            bool convert = bool.TryParse(content, out res);
            return convert && res;
        }
    }
}
