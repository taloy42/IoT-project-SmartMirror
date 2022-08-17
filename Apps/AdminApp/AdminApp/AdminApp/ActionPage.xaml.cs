using AdminApp.Actions;
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
    public partial class ActionPage : ContentPage
    {
        public ActionPage()
        {
            InitializeComponent();
        }

        private async void LogoutButtonClick(object sender, EventArgs e)
        {
            //logic


            //(App.Current as App).DumpUser();

            Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync();
        }

        private async void ChangePasswordButtonClick(object sender, EventArgs e)
        {
            ContentPage changePasswordPage = new ChangePasswordPage();
            await Navigation.PushAsync(changePasswordPage);
        }
        private async void PersonGroupsButtonClick(object sender, EventArgs e)
        {
            ContentPage changePasswordPage = new ManagePersonGroupsPage();
            await Navigation.PushAsync(changePasswordPage);
        }
        private async void AdminsButtonClick(object sender, EventArgs e)
        {
            ContentPage changePasswordPage = new ManageAdminsPage();
            await Navigation.PushAsync(changePasswordPage);
        }
    }
}