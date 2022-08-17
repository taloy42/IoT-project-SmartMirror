using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminApp
{
    public class Utils
    {
        public static async Task<IList<PersonGroup>> GetAllPersonGroups()
        {
            IList<PersonGroup> personGroups = null;
            using (HttpClient httpClient = new HttpClient())
            {
                string url = Constants.BaseUrl + @"/api/ListAllPersonGroups?" + $"key={"brurya"}";

                var resp = await httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    return null;
                }
                var json = await resp.Content.ReadAsStringAsync();
                json = json.Replace("\"{", "{");
                json = json.Replace("\\\"", "\"");
                json = json.Replace("}\"", "}");
                personGroups = JsonConvert.DeserializeObject<IList<PersonGroup>>(json);
            }

            foreach (var personGroup in personGroups)
            {
                personGroup.Persons = await GetUsersInPersonGroupAsync(personGroup.PersonGroupId);
            }

            return personGroups;
        }

        public static async Task<IList<Admin>> GetAllAdmins()
        {
            IList<Admin> admins = null;
            using (HttpClient httpClient = new HttpClient())
            {
                string url = Constants.BaseUrl + @"/api/ListAllAdmins?" + $"key=brurya";

                var resp = await httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    return new List<Admin>();
                }
                var json = await resp.Content.ReadAsStringAsync();
                json = json.Replace("\"{", "{");
                json = json.Replace("\\\"", "\"");
                json = json.Replace("}\"", "}");
                admins = JsonConvert.DeserializeObject<IList<Admin>>(json);
            }

            return admins;
        }

        public static async Task<IList<Person>> GetUsersInPersonGroupAsync(string personGroupId)
        {

            IList<Person> persons = null;
            using (HttpClient httpClient = new HttpClient())
            {
                string url = Constants.BaseUrl + @"/api/ListAllPersons?" + $"personGroupId={personGroupId}&key={"brurya"}";

                var resp = await httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    return null;
                }
                var json = await resp.Content.ReadAsStringAsync();
                json = json.Replace("\"{", "{");
                json = json.Replace("\\\"", "\"");
                json = json.Replace("}\"", "}");
                var userDataDef = new { firstName = "", lastName = "" };
                var def = new[] { new { Name = "", PersonId = new Guid(), PersistedFaceIds = new[] { new Guid() }, UserData = userDataDef } };
                persons = JsonConvert.DeserializeObject<IList<Person>>(json);
            }
            return persons;
        }
    }
}
