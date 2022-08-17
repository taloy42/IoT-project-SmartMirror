using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Utils;
using TryXamarinApp.Views.Settings;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        private async void ChangePasswordButtonClick(object sender, EventArgs e)
        {
            ContentPage changePasswordPage = new ChangePasswordPage();
            await Navigation.PushAsync(changePasswordPage);
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
        private async void CustomizeMirrorButtonClick(object sender, EventArgs e)
        {
            /*string choice =*/ await DisplayActionSheet("Choose Color", "Fuck Me", null,
                "Red", "Blue", "Green", "Black", "White", "Yellow", "ColorBlind");
        }
        private async void BusSettingsButtonClick(object sender, EventArgs e)
        {
            await DisplayAlert("Not Implemented", "This section is not implemented yet", "Fuck Me");
        }
        private async void SaveButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private async void CancelButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}