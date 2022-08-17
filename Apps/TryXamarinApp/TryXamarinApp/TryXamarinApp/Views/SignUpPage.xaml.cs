using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TryXamarinApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void RegisterButtonClick(object sender, EventArgs e)
        {
            //add registering logic
            string uname = userNameEntry.Text;
            string firstName = firstNameEntry.Text;
            string lastName = lastNameEntry.Text;
            string personGroupId = personGroupIdEntry.Text;
            string pw = passwordEntry.Text;
            string pwVerify = verifyEntry.Text;

            List<string> fields = new List<string> { uname,firstName, lastName, personGroupId ,pw,pwVerify};
            if (fields.Any(x => string.IsNullOrEmpty(x))){
                await DisplayAlert("Error while signing up", "All fields are neccessary. Please fill them and try again", "Try again");
                return;
            }
            if (!pw.Equals(pwVerify)){
                await DisplayAlert("Error while signing up", "Passwords dont match!", "Try again");
                return;
            }
            string auth = PasswordHandler.Hash(pw);
            HttpClient httpClient = new HttpClient();
            var data = new {firstName=firstName, lastName=lastName};
            string url = (App.Current as App).
                BaseUrl + 
                @"/api/CreatePerson?" +
                $"personGroupId={personGroupId}&username={uname}&auth={auth}&jsonData={JsonConvert.SerializeObject(data)}";
            await LoadingHandler.StartLoading(Navigation, "Creating user...");
            var res = await httpClient.GetAsync(url);
            await LoadingHandler.FinishLoading(Navigation);
            await DisplayAlert("Success", "User Created Successfully", "Okay");
            var cont = await res.Content.ReadAsStringAsync();
            await Navigation.PopAsync();
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

        private void NameTextChanged(object sender, TextChangedEventArgs e)
        {
            string pattern = @"^([a-zA-Z]+[-\sa-zA-Z]*)+$";
            bool isValid = Regex.IsMatch(e.NewTextValue, pattern);

            if (e.NewTextValue.Length > 0)
            {
                (sender as Entry).Text = isValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
            }
        }
    }
}