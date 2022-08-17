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
    public partial class MyRemindersPage : ContentPage
    {

        ObservableCollection<Reminder> reminders;
        private async Task LoadReminders()
        {
            string username = (App.Current as App).CurrentUser.UserData.firstName;

            reminders = new ObservableCollection<Reminder>( await DbComunication.GetRemindersForUser(username));

            inboxListView.ItemsSource = reminders;

        }
        private async Task InitializeReminders()
        {
            await LoadReminders();

            inboxListView.ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                
                Label reminderLabel = new Label();
                reminderLabel.SetBinding(Label.TextProperty, "reminder");
                reminderLabel.FontAttributes = FontAttributes.Bold;
                Label timeLabel = new Label();
                timeLabel.SetBinding(Label.TextProperty, new MultiBinding
                {
                    Bindings = new Collection<BindingBase>
                    {
                        new Binding("start_time"),
                        new Binding("end_time")
                    },
                    StringFormat = "from {0:dd/MM/yy hh:mm} until {1:dd/MM/yy hh:mm}"
                });
                timeLabel.FontAttributes = FontAttributes.Italic;
                /*
                 * new Binding("PersonId", BindingMode.OneWay,
                        null, null, "Guid: {0}")
                 */
                var reminderView = new StackLayout
                {
                    Padding = new Thickness(5, 5),
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children =
                        {
                            new StackLayout
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Spacing = 0,
                                Children =
                                {
                                    reminderLabel,
                                    timeLabel
                                }
                            }
                        }
                };

                StackLayout final = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        reminderView
                    }
                };

                ViewCell cell = new ViewCell
                {
                    View = final
                };

                return cell;
            });
        }

        public MyRemindersPage()
        {
            InitializeComponent();
            
        }



        private async void IDK_button(object sender, EventArgs e)
        {
            await InitializeReminders();
        }
        private async void DeleteButtonClick(object sender, EventArgs e)
        {
            var curReminder = inboxListView.SelectedItem as Reminder;
            var delete = await DisplayAlert("Deleting Message", $"Are you sure you want to delete the reminder?", "Delete Message", "Cancel");
            int rows_affected = -1;
            if (delete)
            {
                await LoadingHandler.StartLoading(Navigation, "Deleting reminder...");
                rows_affected = await DeleteReminder(curReminder);
                await LoadingHandler.FinishLoading(Navigation);
                if (rows_affected > 0)
                {
                    await DisplayAlert("Success!", "Reminder deleted successfully", "Okay");
                }
                else
                {
                    await DisplayAlert("Failure", "Problem while deleting reminder", "Okay");
                }
            }

            inboxListView.SelectedItem = null;
            inboxListView.ItemsSource = reminders;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeReminders();
        }

        private async Task<int> DeleteReminder(Reminder msg)
        {
            reminders.Remove(msg);
            return await DbComunication.DeleteReminderUser(msg);
        }
    }
}