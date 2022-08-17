using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TryXamarinApp.Utils;
using TryXamarinApp.Classes;
using System.Collections.ObjectModel;

namespace TryXamarinApp.Views.Actions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyInboxPage : ContentPage
    {

        ObservableCollection<Message> messages;
        private async Task LoadMessages()
        {
            Guid userId = (App.Current as App).CurrentUser.PersonId;

            messages = new ObservableCollection<Message>( await DbComunication.GetMessagesForUser(userId));

            inboxListView.ItemsSource = messages;
        }
        private async Task InitializeMessages()
        {
            await LoadMessages();
         
            inboxListView.ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                Label fromLabel = new Label();
                fromLabel.SetBinding(Label.TextProperty, new Binding("sender", BindingMode.OneWay,
                    null, null, "From {0}"));
                fromLabel.FontAttributes = FontAttributes.Bold;
                Label msgLabel = new Label();
                msgLabel.SetBinding(Label.TextProperty, "message");

                var messageView = new StackLayout
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
                                    fromLabel,
                                    msgLabel
                                }
                            }
                        }
                };

                Image image = new Image();
                var source = new UriImageSource();
                source.SetBinding(UriImageSource.UriProperty,new Binding("senderID",BindingMode.OneWay,null,null, "https://avatars.dicebear.com/api/bottts/{0}.png"));
                image.Source = source;

                StackLayout final = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        image,
                        messageView
                    }
                };
                ViewCell cell = new ViewCell
                {
                    View = final
                };

                return cell;
            });
        }

        public MyInboxPage()
        {

            InitializeComponent();
            
        }



        private async void IDK_button(object sender, EventArgs e)
        {
            await InitializeMessages();
        }
        private async void DeleteButtonClick(object sender, EventArgs e)
        {
            var curMsg = inboxListView.SelectedItem as Message;
            var delete = await DisplayAlert("Deleting Message", $"Are you sure you want to delete message from {curMsg.sender}?", "Delete Message", "Cancel");
            int rows_affected = -1;
            if (delete)
            {
                await LoadingHandler.StartLoading(Navigation, "Deleting message...");
                rows_affected = await DeleteMessage(curMsg);
                await LoadingHandler.FinishLoading(Navigation);
                if (rows_affected > 0)
                {
                    await DisplayAlert("Success!", "Message deleted successfully", "Okay");
                }
                else
                {
                    await DisplayAlert("Failure", "Problem while deleting message", "Okay");
                }
            }
            inboxListView.SelectedItem = null;
            inboxListView.ItemsSource = messages;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeMessages();
        }

        private async Task<int> DeleteMessage(Message msg)
        {
            messages.Remove(msg);
            return await DbComunication.DeleteMessageUser(msg);
        }
    }
}