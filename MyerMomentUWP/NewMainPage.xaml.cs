using MyerMomentUniversal.Model;
using System;
using System.Threading.Tasks;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
using Windows.ApplicationModel.Email;
#endif
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel.Activation;
using MyerMomentUniversal.Helper;
using JP.Utils.Data;
using MyerMomentUWP.Base;

namespace MyerMomentUniversal
{

    public sealed partial class NewMainPage : BasePage
    {
        private bool _isNavigate = false;

        public NewMainPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

        }

        private async void PickClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var data = new PageNavigateData();
                data.file = file;
                data.isFromShare = false;
                Frame.Navigate(typeof(ImageHandlePage), data);
            }
        }

        private void GoSettingClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void GoAboutClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.BackStack.Clear();

            if(!_isNavigate)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                _isNavigate = true;
                BackgrdStory.Begin();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

       
#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
            else
            {
                App.Current.Exit();
            }
        }
#endif

    }
}
