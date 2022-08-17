using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryXamarinApp.Classes;
using TryXamarinApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.Views.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveMessagePage : ContentPage
    {
        public LeaveMessagePage()
        {
            InitializeComponent();
            List<Person> people;
            //people = namesToPerson(
            //    new string[]
            //        {"tal", "may", "alon" },
            //    new string[]
            //        {"levi", "buhadana", "labrisch" });


            people = new List<Person>(
                (App.Current as App).Persons
                .Values.Cast<Person>()
                .Where<Person>(p=>p.PersonId!=(App.Current as App).CurrentUser.PersonId)
                .ToArray()
                );

            people.Sort();
            
            contactsListView.ItemsSource = people;

            contactsListView.ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                Label nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, new Binding("FullName",BindingMode.OneWay,
                    null, null, "Name: {0}"));

                Label idLabel = new Label();
                idLabel.SetBinding(Label.TextProperty,
                    new Binding("Name", BindingMode.OneWay,
                        null, null, "Username: {0}"));
                // Return an assembled ViewCell.
                Image image = new Image();
                var source = new UriImageSource();
                source.SetBinding(UriImageSource.UriProperty, new Binding("PersonId", BindingMode.OneWay, null, null, "https://avatars.dicebear.com/api/bottts/{0}.png"));
                image.Source = source;

                StackLayout contactsView = new StackLayout
                {
                    Padding = new Thickness(0, 5),
                    Orientation = StackOrientation.Horizontal,
                    Children =
                        {
                            new StackLayout
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Spacing = 0,
                                Children =
                                {
                                    nameLabel,
                                    idLabel
                                }
                            }
                        }
                };


                StackLayout final = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        image,
                        contactsView
                    }
                };
                
                ViewCell cell = new ViewCell
                {
                    View = final
                };

                return cell;

            });
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Message msg = new Message();
            msg.timestamp = DateTime.Now;
            Person msgSender = (App.Current as App).CurrentUser;
            Person msgReciever = (contactsListView.SelectedItem as Person);
            msg.sender = msgSender.FullName;
            msg.senderID = msgSender.PersonId;
            msg.reciever = msgReciever.FullName;
            msg.recieverID = msgReciever.PersonId;
            msg.message = messageEditor.Text;

            await LoadingHandler.StartLoading(Navigation, $"Sending message to {msg.reciever}");
            int rows_affected = await DbComunication.InsertMessageUser(msg);
            await LoadingHandler.FinishLoading(Navigation);

            if (rows_affected > 0)
            {
                await DisplayAlert("Success", "Message sent successfully", "Okay");
            }
            else
            {
                await DisplayAlert("Failure", "Problem while sending message", "Okay");
            }
        }
    }
}