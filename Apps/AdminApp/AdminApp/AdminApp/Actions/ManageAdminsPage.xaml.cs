using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdminApp.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageAdminsPage : ContentPage
    {
        bool firstLoad = true;
        ObservableCollection<Admin> admins;
        public ManageAdminsPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (firstLoad)
            {
                admins = new ObservableCollection<Admin>(await Utils.GetAllAdmins());
                admins.Remove(admins.First((x) => x.username.Equals(Constants.CurAdmin.username)));
                adminsListView.ItemsSource = admins;
                adminsListView.ItemTemplate = new DataTemplate(() =>
                {

                    var final = new StackLayout();

                    Label label = new Label();
                    label.SetBinding(Label.TextProperty, new Binding("username", BindingMode.OneWay, null, null, "Username: {0}"));
                    final.Children.Add(label);
                    ViewCell cell = new ViewCell
                    {
                        View = final
                    };

                    return cell;
                });
            }
            firstLoad = false;
        }
        private async void DeleteButtonClick(object sender, EventArgs e)
        {

            var curAdmin = adminsListView.SelectedItem as Admin;

            await DeleteAdmin(curAdmin);
        }
        private async void AddButtonClick(object sender, EventArgs e)
        {
            ContentPage adminCreationPage = new AddAdminPage();
            await Navigation.PushAsync(adminCreationPage);
            firstLoad = true;
        }

        private async Task DeleteAdmin(Admin admin)
        {
            var delete = await DisplayAlert("Deletion", $"Are you sure you want to delete '{admin.username}'", "Delete", "Cancel Deletion");
            if (delete)
            {
                string url = Constants.BaseUrl + @"/api/DeleteAdmin?" + $"username={admin.username}";
                using (HttpClient httpClient = new HttpClient())
                {
                    var resp = await httpClient.GetAsync(url);
                }
                admins.Remove(admin);
            }
            else
            {
                await DisplayAlert("Deletion Canceled", "You canceled the deletion", "Okay");
            }
        }
    }
}