using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            
            InitializeComponent();
        }

        public LoadingPage(string msg)
        {
            InitializeComponent();
            messageLabel.Text = msg;  
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = !activityIndicator.IsRunning;
        }
    }
}