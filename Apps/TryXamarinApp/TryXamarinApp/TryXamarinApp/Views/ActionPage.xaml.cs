using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Views.Actions;
using TryXamarinApp.Views.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionPage : ContentPage
    {
        public ActionPage()
        {
            this.BindingContext = (App.Current as App).CurrentUser;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //HttpClient httpClient = new HttpClient();
            Guid id = (App.Current as App).CurrentUser.PersonId;
            Uri uri = new Uri($"https://avatars.dicebear.com/api/bottts/{id}.png");

            //var resp = await httpClient.GetAsync(uri);
            //var imgStream = await resp.Content.ReadAsStreamAsync();
            //ImageSource src = ImageSource.FromUri(uri);
            UriImageSource source = new UriImageSource();
            source.Uri = uri;
            profileImage.Source = source;/*(App.Current as App).ProfilePic*/;
        }
        private void AddPictureButtonClick(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddPicturePage());
        }
        private async void LeaveMessageButtonClick(object sender, EventArgs e)
        {
            //to others
            //string[] options = {
            //"tal",
            //"may",
            //"alon",
            //"mazal",
            //"isar",
            //"noam",
            //"afik",
            //"nitzan",
            //"amos",
            //"keren", };
            //string action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, options);

            await Navigation.PushAsync(new LeaveMessagePage());
            //dialog box
        }
        private async void SetReminderButtonClick(object sender, EventArgs e)
        {
            //to self
            //HttpClient httpClient = new HttpClient();

            //var r = await httpClient.GetAsync((App.Current as App).BaseUrl + @"/api/ExampleURL");

            await Navigation.PushAsync(new SetReminderPage());

        }

        private async void MyRemindersButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyRemindersPage());
        }

        private async void MyInboxButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyInboxPage());
        }
        private async void ChangePasswordButtonClick(object sender, EventArgs e)
        {
            ContentPage changePasswordPage = new ChangePasswordPage();
            await Navigation.PushAsync(changePasswordPage);
        }
        private void SettingsButtonClick(object sender, EventArgs e)
        {
            //logic

            Navigation.PushAsync(new SettingsPage());
        }

        private async void LogoutButtonClick(object sender, EventArgs e)
        {
            //logic


            (App.Current as App).DumpUser();
            
            Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync();
        }
        
    }
}