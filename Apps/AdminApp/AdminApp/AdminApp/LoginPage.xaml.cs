using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdminApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButtonClick(object sender, EventArgs e)
        {

            string username = usernameEntry.Text;
            string password = passwordEntry.Text;
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Missing Details", "Please fill the username and the password fields", "Okay");
                return;
            }


            //search for uname in DB
            await LoadingHandler.StartLoading(Navigation, "Logging in");
            bool verified = await DbCommunication.VerifyAdmin(username, password);
            
            if (!verified)
            {
                await LoadingHandler.FinishLoading(Navigation);
                await DisplayAlert("Bummer", "Trouble while signing in. Check your username/password again", "sorry bro, try again");
                return;
            }

            Admin admin = new Admin { username = username, password = password };
            Constants.CurAdmin = admin;
            ContentPage actionPage = new ActionPage();
            actionPage.BindingContext = admin;
            await LoadingHandler.FinishLoading(Navigation);
            Navigation.InsertPageBefore(actionPage, Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync();

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