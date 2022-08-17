using Newtonsoft.Json;
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
    public partial class SetReminderPage : ContentPage
    {
        public SetReminderPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Reminder reminder = new Reminder();

            reminder.username = (App.Current as App).CurrentUser.UserData.firstName;
            reminder.start_time = new DateTime(startDatePicker.Date.Ticks + startTimePicker.Time.Ticks);
            reminder.end_time = new DateTime(endDatePicker.Date.Ticks + endTimePicker.Time.Ticks);
            reminder.reminder = reminderEditor.Text;
            reminder.userid = (App.Current as App).CurrentUser.PersonId;

            await LoadingHandler.StartLoading(Navigation, "Setting reminder...");
            int rows_affected = await DbComunication.InsertReminderUser(reminder);
            await LoadingHandler.FinishLoading(Navigation);
            if (rows_affected > 0)
            {
                await DisplayAlert("Success", "Reminder set successfully", "Okay");
            }
            else
            {
                await DisplayAlert("Failure", "Problem while setting reminder", "Okay");
            }
        }
    }
}