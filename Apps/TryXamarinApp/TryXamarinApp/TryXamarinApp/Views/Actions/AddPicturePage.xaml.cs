using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;
using TryXamarinApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TryXamarinApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPicturePage : ContentPage
    {
        private byte[] _array;
        public AddPicturePage()
        {
            InitializeComponent();
        }
        private async void UploadPhotoClick(object sender, EventArgs e)
        {
            var baseUrl = (App.Current as App).BaseUrl;
            var curPerson = (App.Current as App).CurrentUser;
            var curPersonGroup = (App.Current as App).PersonGroup;
            await LoadingHandler.StartLoading(Navigation, "Uploading Photo...");
            await SendStreamAsPOST(baseUrl + @"/api/AddFaceToPerson?"+ $"personGroupId={curPersonGroup}&guid={curPerson.PersonId}", _array, "name", "fileName");
            await LoadingHandler.FinishLoading(Navigation);

            await DisplayAlert("Success", "Photo added successfully", "Okay");
        }

        private async void ImageButtonClick(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Upload Photo", "Cancel", null, "Choose from Gallery", "Open Camera");

            MediaFile selectedImageFile = null;

            switch (action)
            {
                case "Choose from Gallery":
                    selectedImageFile = await UploadFromGallery();
                    break;
                case "Open Camera":
                    selectedImageFile = await CapturePhoto();
                    break;
                default:
                    break;
            }
            if (selectedImageFile == null)
            {
                return;
            }
            if (Shlomke == null)
            {
                await DisplayAlert("Error", "Blob", "Ok");
                return;
            }

            var ms = new MemoryStream();
            selectedImageFile.GetStream().CopyTo(ms);
            _array = ms.ToArray();

            var s = new MemoryStream(_array);
            Shlomke.Source = ImageSource.FromStream(() => s);

        }

        public static async Task<HttpResponseMessage> SendStreamAsPOST(string url, byte[] imgBytes, string name, string fileName)
        {
            if (name == null || fileName == null)
            {
                throw new Exception("need to specify name and fileName");
            }
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent content = new MultipartFormDataContent();
            HttpContent img = new ByteArrayContent(imgBytes);
            content.Add(img, name: name, fileName: fileName);

            return await httpClient.PostAsync(url, content);
        }

        private async Task<MediaFile> UploadFromGallery()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Not supported", "Your device does not current ly support this functionality", "Ok");
                return null;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            if (Shlomke == null)
            {
                await DisplayAlert("Error", "Blob", "Ok");
                return null;
            }

            return selectedImageFile;
        }

        private async Task<MediaFile> CapturePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Not supported", "Your device does not current ly support this functionality", "Ok");
                return null;
            }

            var mediaOptions = new StoreCameraMediaOptions
            {
                Directory = "Pictures",
                Name = $"{DateTime.UtcNow}.jpg",
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

            if (Shlomke == null)
            {
                await DisplayAlert("Error", "Blob", "Ok");
                return null;
            }

            return selectedImageFile;
        }
    }
}