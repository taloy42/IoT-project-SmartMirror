using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdminApp
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
            Admin admin = Constants.CurAdmin;
            if (!oldPassword.Equals(admin.password))
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
            bool success = await DbCommunication.ChangePassword(admin, oldPassword, newPassword);
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