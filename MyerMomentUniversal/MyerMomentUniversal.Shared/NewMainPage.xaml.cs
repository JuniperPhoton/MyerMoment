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
using Windows.System;

namespace MyerMomentUniversal
{

    public sealed partial class NewMainPage : Page
#if WINDOWS_PHONE_APP 
        ,IFileOpenPickerContinuable 
#endif
    {
        private bool _isNavigate = false;

        public NewMainPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

        }

        private async void PickClick(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
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

#elif WINDOWS_PHONE_APP
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.PickSingleFileAndContinue();
#endif
        }
#if WINDOWS_PHONE_APP
        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count == 0)
            {
                return;
            }
            switch (args.Kind)
            {
                case ActivationKind.PickFileContinuation:
                    {
                        var data = new PageNavigateData();
                        data.file = args.Files[0];
                        data.isFromShare = false;
                        Frame.Navigate(typeof(ImageHandlePage), data);
                    }
                    break;
            }
        }
#endif

        private void GoSettingClick(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
            SettingFlyoutPage settingpage = new SettingFlyoutPage();
            settingpage.ShowIndependent();
#else
            Frame.Navigate(typeof(SettingPage));
#endif
        }

        private void GoAboutClick(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
            AboutFlyoutPage aboutpage = new AboutFlyoutPage();
            aboutpage.ShowIndependent();
#else
            Frame.Navigate(typeof(AboutPage));
#endif
        }

        private void ToStoreClick(object sender,RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            Frame.Navigate(typeof(StorePage));
#else 
            (new StoreFlyout()).ShowIndependent();
#endif
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
#endif
            Frame.BackStack.Clear();

            if(!_isNavigate)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                NavigateStory.Completed += ((sc, ec) =>
                  {
                      if(!LocalSettingHelper.HasValue("Goodbye6"))
                      {
                          GoodbyeStory.Begin();
                          LocalSettingHelper.AddValue("Goodbye6", true);
                      }
                  });
                NavigateStory.Begin();
                _isNavigate = true;
                BackgrdStory.Begin();
            } 
        }

        private void GoodbyeClick(object sender,RoutedEventArgs e)
        {
            GoodbyeBackStory.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#endif
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           await Launcher.LaunchUriAsync(new Uri("https://www.microsoft.com/store/apps/9nblggh5fp0h"));
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
