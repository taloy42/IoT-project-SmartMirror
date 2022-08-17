using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TryXamarinApp.Classes;
using TryXamarinApp.views;
using TryXamarinApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TryXamarinApp
{
    public partial class App : Application
    {

        public Dictionary<string, Person> Persons { get; set;}
        public Person CurrentUser { get; set; }
        public string PersonGroup { get; set; }
        public string Key { get; set; }

        private Person _emptyperson;
        public Person EmptyPerson { get { return _emptyperson; } }

        public string BaseUrl { get; set; }
        public ImageSource ProfilePic { get; set; }

        public ContentPage CurrentContentPage { get; set; }

        public void DumpUser()
        {
            CurrentUser = EmptyPerson;
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
            //MainPage = new NavigationPage(new LoadingPage());

        }
        public async Task<bool> UpdateAllUsersAsync(string personGroup)
        {
            PersonGroup = personGroup;
            Persons = await GetAllUsersAsync();
            return Persons != null;
        }
        private async Task<Dictionary<string, Person>> GetAllUsersAsync()
        {
            Dictionary<string, Person> res = new Dictionary<string, Person>();

            //string[] unames =
            //{
            //    "taloy",
            //    "maybu",
            //    "alonlab",
            //};
            //string[] fnames =
            //{
            //    "Tal",
            //    "May",
            //    "Alon",
            //};
            //string[] lnames =
            //{
            //    "Levi",
            //    "Bohadana",
            //    "Labrisch"
            //};

            //string[] ids =
            //{
            //    @"04cf9250-61f3-432d-8bf6-f0f497f6cfaf",
            //    @"9e390fc1-40f5-4524-b351-32546209c5ec",
            //    @"be66e246-aff6-4684-b2f7-bafc2ede153f"
            //};

            //for (int i = 0; i < unames.Length; i++)
            //{
            //    res[unames[i]] = new Person(fnames[i], lnames[i], new Guid(ids[i]));
            //}
            using (HttpClient httpClient = new HttpClient())
            {
                string url = BaseUrl + @"/api/ListAllPersons?" + $"personGroupId={PersonGroup}&key={Key}";

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
                var persons = JsonConvert.DeserializeObject<IList<Person>>(json);

                foreach (var person in persons)
                {
                    res[person.Name] = person;
                }
            }
            return res;
        }

        protected override async void OnStart()
        {
            //BaseUrl = @"http://10.0.2.2:7071";
            //Preferences.Clear();
            BaseUrl = @"https://tryincdecazurefunction20220420002219.azurewebsites.net";
            Key = "brurya";
            if (Preferences.ContainsKey("person_group"))
            {
                PersonGroup = Preferences.Get("person_group", "");
                Persons = await GetAllUsersAsync();
            }
            else
            {
                Persons = null;
            }
            //foreach (var person in Persons.Values)
            //{
            //    person.Pic = await GetPicForString(person.Name);
            //}
            var userData = new UserDataType();
            userData.firstName = "John";
            userData.lastName = "Doe";
            _emptyperson = new Person("Johndoe", Guid.Empty, userData, null);

            DumpUser();
        }
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
