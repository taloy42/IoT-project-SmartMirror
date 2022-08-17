using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Classes;
using TryXamarinApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string newPassword, oldPassword, passwordVerify;
            newPassword = newPasswordEntry.Text;
            oldPassword = oldPasswordEntry.Text;
            passwordVerify = passwordVerifyEntry.Text;
            Person person = (App.Current as App).CurrentUser;
            if (!oldPassword.Equals(person.Password))
            {
                await DisplayAlert("Failure", "Old password is incorrect", "Okay");
                return;
            }
            if (!newPassword.Equals(passwordVerify))
            {
                await DisplayAlert("Failure", "New passwords don't match", "Okay");
                return;
            }
            if (oldPassword.Equals(newPassword))
            {
                await DisplayAlert("Failure", "New password has to be different from old password", "Okay");
                return;
            }

            await LoadingHandler.StartLoading(Navigation, "Changing Password...");
            bool success = await DbComunication.ChangePassword(person, oldPassword, newPassword);
            await LoadingHandler.FinishLoading(Navigation); 
            if (success)
            {
                await DisplayAlert("Success", "Paswword changed successfully", "Okay");
            }
            else
            {
                await DisplayAlert("Failure", "There was a problem while changing your password", "Try again");
            }
        }
    }
}