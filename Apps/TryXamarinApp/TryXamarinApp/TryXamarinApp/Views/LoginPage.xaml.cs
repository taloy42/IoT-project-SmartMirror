using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TryXamarinApp.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Net.Http;

using Newtonsoft.Json;
using Xamarin.Essentials;
using TryXamarinApp.Views;
using TryXamarinApp.Utils;

namespace TryXamarinApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());

        }
        private async void ShowAllButtonClick(object sender, EventArgs e)
        {
            var l = (App.Current as App).Persons;
            int n = l.Count;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"number of users: {n}");
            foreach (var pair in l)
            {
                var p = pair.Value;
                sb.AppendLine($"\n==={p.Name}===\n{JsonConvert.SerializeObject(p.UserData)}");
            }

            var usersStr = sb.ToString();
            await DisplayAlert("all users", usersStr, "dismiss");
        }
        private async void DetectButtonClick(object sender, EventArgs e)
        {

            if (!Preferences.ContainsKey("person_group") || (App.Current as App).Persons == null)
            {
                string msg = "You have no Person Group Id defined, or the Person Group Id does not exist. Enter one to be able to login";

                var result = await DisplayPromptAsync("Change Person Group Id", msg, "Save", "Cancel");
                if (String.IsNullOrEmpty(result))
                {
                    await DisplayAlert("Not Changed", "Person Group did not change", "Okay");
                }
                else
                {
                    await LoadingHandler.StartLoading(Navigation, "Updating Person Group...");
                    bool success = await (App.Current as App).UpdateAllUsersAsync(result);
                    await LoadingHandler.FinishLoading(Navigation);
                    if (success)
                    {
                        Preferences.Set("person_group", result);
                        await DisplayAlert("Changed", $"Person Group changed successfully to {result}", "Okay");
                    }
                    else
                    {
                        await DisplayAlert("Not Changed", $"There was an error while trying to access the Person Group", "Okay");
                    }
                }
                return;
                
            }
            else
            {
                MediaFile selectedImageFile = await CapturePhoto();

                if (selectedImageFile == null)
                {
                    return;
                }
                var imageStream = new MemoryStream();
                selectedImageFile.GetStream().CopyTo(imageStream);

                var arr = imageStream.ToArray();

                string url = (App.Current as App).BaseUrl + @"/api/TryToMatchDetails?personGroupId=" + (App.Current as App).PersonGroup;

                await LoadingHandler.StartLoading(Navigation, "Detecting...");
                var resp = await SendStreamAsPOST(url, arr, "image", "image");
                await LoadingHandler.FinishLoading(Navigation);

                var contentString = await resp.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(contentString);
                //await DisplayAlert("title", person.Name + "\n" + JsonConvert.SerializeObject(person.UserData), "dismiss");
                if (person.PersonId == Guid.Empty)
                {
                    await DisplayAlert("Not Detected", "Could not detect you. Please try again or sign up if you are not registerd.", "Okay");
                    return;
                }

                (App.Current as App).CurrentUser = person;
                ContentPage actionPage = new ActionPage();
                Navigation.InsertPageBefore(actionPage, Navigation.NavigationStack[0]);
                await Navigation.PopToRootAsync();


            }
        }
        private async Task<MediaFile> CapturePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Not supported", "Your device does not current ly support this functionality", "Ok");
                return null;
            }

            var mediaOptions = new StoreCameraMediaOptions
            {
                Directory = "Pictures",
                Name = $"{DateTime.UtcNow}.jpg",
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

            return selectedImageFile;
        }
        private async Task<MediaFile> UploadFromGallery()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Not supported", "Your device does not current ly support this functionality", "Ok");
                return null;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            return selectedImageFile;
        }
        public static async Task<HttpResponseMessage> SendStreamAsPOST(string url, byte[] imgBytes, string name, string fileName)
        {
            if (name == null || fileName == null)
            {
                throw new Exception("need to specify name and fileName");
            }
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent content = new MultipartFormDataContent();
            HttpContent img = new ByteArrayContent(imgBytes);
            content.Add(img, name: name, fileName: fileName);

            return await httpClient.PostAsync(url, content);
        }
        private async void LoginButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(password.Text) || string.IsNullOrEmpty(username.Text))
            {
                await DisplayAlert("Missing Details", "Please fill the username and the password fields", "Okay");
                return;
            }
            if ((!Preferences.ContainsKey("person_group")) || (App.Current as App).Persons == null)
            {
                string msg = "You have no Person Group Id defined, or the Person Group Id does not exist. Enter one to be able to login";

                var result = await DisplayPromptAsync("Change Person Group Id", msg, "Save", "Cancel");
                if (string.IsNullOrEmpty(result))
                {
                    await DisplayAlert("Not Changed", "Person Group did not change", "Okay");
                }
                else
                {
                    Preferences.Set("person_group", result);
                    await (App.Current as App).UpdateAllUsersAsync(result);
                    await DisplayAlert("Changed", $"Person Group changed successfully to {result}", "Okay");
                }
                return;
            }
            //logic
            string uname = username.Text;
            //string pw = password.Text;
            Dictionary<string, Person> persons = (App.Current as App).Persons;
            if ((uname?.Length ?? 0) == 0 || !persons.ContainsKey(uname))
            {
                await DisplayAlert("Bummer", "Trouble while signing in. Check your username/password again", "sorry bro, try again");
                return;
            }

            //search for uname in DB
            Person person = persons[uname];
            if (!(await DbComunication.VerifyPassword(person, password.Text)))
            {
                await DisplayAlert("Bummer", "Trouble while signing in. Check your username/password again", "sorry bro, try again");
                return;
            }
            person.Password = password.Text;
            (App.Current as App).CurrentUser = person;
            ContentPage actionPage = new ActionPage();
            Navigation.InsertPageBefore(actionPage, Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync();
        }
        private async void SettingsButtonClick(object sender, EventArgs e)
        {
            ContentPage settingsPage = new SettingsPage();
            await Navigation.PushAsync(settingsPage);
        }
        public async void ChangePersonGroupButtonClick(object sender, EventArgs e)
        {
            bool hasPersonGroup = Preferences.ContainsKey("person_group");
            string msg;
            if (hasPersonGroup)
            {
                msg = $"Current Person Group is {Preferences.Get("person_group", "Poop")}, enter new Person Group Id";
            }
            else
            {
                msg = "You have no Person Group Id defined, or the Person Group Id does not exist. Enter one to be able to login";
            }
            var result = await DisplayPromptAsync("Change Person Group Id", msg, "Save", "Cancel");
            if (String.IsNullOrEmpty(result))
            {
                await DisplayAlert("Not Changed", $"Person Group did not change", "Okay");
            }
            else
            {
                await LoadingHandler.StartLoading(Navigation, "Updating Person Group...");
                bool success = await (App.Current as App).UpdateAllUsersAsync(result);
                await LoadingHandler.FinishLoading(Navigation);
                if (success)
                {
                    Preferences.Set("person_group", result);
                    await DisplayAlert("Changed", $"Person Group changed successfully to {result}", "Okay");
                }
                else
                {
                    await DisplayAlert("Not Changed", $"There was an error while trying to access the Person Group", "Okay");
                }
            }

        }

        private void UsernameTextChanged(object sender, TextChangedEventArgs e)
        {
            string pattern = @"^[a-zA-Z0-9_]+$";
            bool isValid = Regex.IsMatch(e.NewTextValue, pattern);

            if (e.NewTextValue.Length > 0)
            {
                (sender as Entry).Text = isValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
            }
        }
    }
}