using Syncfusion.XForms.Expander;
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
    public partial class ManagePersonGroupsPage : ContentPage
    {
        ObservableCollection<PersonGroup> personGroups;
        bool firstLoad = true;
        public ManagePersonGroupsPage()
        {
            InitializeComponent();
        }

        public ListView PersonGroupToListView(PersonGroup personGroup)
        {
            ListView listView = new ListView();
            listView.ItemsSource = personGroup.Persons;
            listView.ItemTemplate = new DataTemplate(() =>
            {
                Label nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, new Binding("FullName", BindingMode.OneWay, null, null, "Name: {0}"));

                Label usernameLabel = new Label();
                usernameLabel.SetBinding(Label.TextProperty, new Binding("Name", BindingMode.OneWay, null, null, "Username: {0}"));

                Label idLabel = new Label();
                idLabel.SetBinding(Label.TextProperty, new Binding("PersonId", BindingMode.OneWay, null, null, "Guid: {0}"));


                var final = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Children =
                    {
                        nameLabel,
                        usernameLabel,
                        idLabel
                    }
                };



                ViewCell cell = new ViewCell
                {
                    View = final
                };

                return cell;
            });
            return listView;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (firstLoad)
            {
                personGroups = new ObservableCollection<PersonGroup>(await Utils.GetAllPersonGroups());
                personGroupsListView.ItemsSource = personGroups;
                personGroupsListView.ItemTemplate = new DataTemplate(() =>
                {

                    var final = new StackLayout();
                    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk4NjUwQDMyMzAyZTMyMmUzMG13Q05XNlU3ckcyeXNYVTVkRFg4aXF5SEdGQWRiemRjeGF4Zld3RVU1N289");
                    SfExpander expander = new SfExpander();

                    StackLayout header = new StackLayout
                    {
                        Orientation= StackOrientation.Vertical
                    };
                    Grid content = new Grid();
                    Label headerIdLabel = new Label();
                    headerIdLabel.SetBinding(Label.TextProperty, new Binding("PersonGroupId", BindingMode.OneWay,
                        null,null,"Guid: {0}"));

                    Label headerNameLabel = new Label();
                    headerNameLabel.SetBinding(Label.TextProperty, new Binding("Name", BindingMode.OneWay,
                        null, null, "Name: {0}"));

                    header.Children.Add(headerNameLabel);
                    header.Children.Add(headerIdLabel);
                    expander.Header = header;

                    ListView persons = new ListView();
                    persons.HasUnevenRows = true;
                    persons.VerticalOptions = LayoutOptions.FillAndExpand;
                    persons.SetBinding(ListView.ItemsSourceProperty, "Persons");
                    persons.ItemTemplate = new DataTemplate(() =>
                    {
                        Label nameLabel = new Label();
                        nameLabel.SetBinding(Label.TextProperty, new Binding("FullName", BindingMode.OneWay, null, null, "Name: {0}"));

                        Label usernameLabel = new Label();
                        usernameLabel.SetBinding(Label.TextProperty, new Binding("Name", BindingMode.OneWay, null, null, "Username: {0}"));

                        Label idLabel = new Label();
                        idLabel.SetBinding(Label.TextProperty, new Binding("PersonId", BindingMode.OneWay, null, null, "Guid: {0}"));


                        var final2 = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                nameLabel,
                                usernameLabel,
                                idLabel
                            }
                        };



                        ViewCell cell2 = new ViewCell
                        {
                            View = final2
                        };

                        return cell2;
                    });
                    persons.HasUnevenRows = true;
                    content.Children.Add(persons);
                    expander.Content = content;

                    final.Children.Add(expander);
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

            var id = await DisplayPromptAsync("Delete Person Group", "Enter Person Group Id to delete");
            if (string.IsNullOrEmpty(id))
            {
                await DisplayAlert("Deletion Canceled", "You canceled the deletion", "Okay");
            }
            else if (personGroups.Any((x)=>x.PersonGroupId == id))
            {
                await DeletePersonGroup(id);
            }
            else {
                await DisplayAlert("Deletion Canceled", "Person Group does not exist", "Okay");
            }
        }
        private async void AddButtonClick(object sender, EventArgs e)
        {
            var id = await DisplayPromptAsync("Add Person Group", "Enter Person Group Id to create");
            if (string.IsNullOrEmpty(id))
            {
                await DisplayAlert("Creation Canceled", "You canceled the creation", "Okay");
            }
            else if (personGroups.Any((x) => x.PersonGroupId == id))
            {
                await DisplayAlert("Creation Canceled", "Person Group Id already exists", "Okay");
            }
            else
            {
                var name = await DisplayPromptAsync("Add Person Group", "Enter Person Group Name to delete");
                if (string.IsNullOrEmpty(name))
                {
                    await DisplayAlert("Creation Canceled", "You canceled the creation", "Okay");
                }
                else if (personGroups.Any((x) => x.Name == name))
                {
                    await DisplayAlert("Creation Canceled", "Person Group Name already exists", "Okay");
                }
                else
                {
                    await CreatePersonGroup(id, name);
                }
            }
        }

        private async Task DeletePersonGroup(string personGroupId)
        {
            var delete = await DisplayAlert("Deletion", $"Are you sure you want to delete '{personGroupId}'", "Delete", "Cancel Deletion");
            if (delete)
            {
                var toDelete = personGroups.First((x) => x.PersonGroupId == personGroupId);
                string url = Constants.BaseUrl + @"/api/DeletePersonGroup?" + $"personGroupId={personGroupId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    var resp = await httpClient.GetAsync(url);
                }
                if (personGroups.Contains(toDelete)) personGroups.Remove(toDelete);
            }
            else
            {
                await DisplayAlert("Deletion Canceled", "You canceled the deletion", "Okay");
            }
        }
        private async Task CreatePersonGroup(string personGroupId, string personGroupName)
        {
            PersonGroup newPersonGroup = new PersonGroup { PersonGroupId = personGroupId , Name= personGroupName, Persons = new List<Person>()};
            
            string url = Constants.BaseUrl + @"/api/CreatePersonGroup?" + $"personGroupId={personGroupId}&personGroupName={personGroupName}";
            using (HttpClient httpClient = new HttpClient())
            {
                var resp = await httpClient.GetAsync(url);
            }
            personGroups.Add(newPersonGroup);
        }
    }
}