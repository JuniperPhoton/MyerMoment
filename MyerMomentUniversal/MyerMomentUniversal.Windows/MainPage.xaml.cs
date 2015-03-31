using ChaoFunctionRT;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


namespace MyerMomentUniversal
{

    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        private bool iscomComDirty = false;
        private bool isposComDirty = false;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            
            ConfigLang();
            VersionHLB.Content = (string)(App.Current.Resources["AppVersion"]);

        }

        private void ConfigLang()
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            
        }



        private async void OpenPhotoClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            var file=await picker.PickSingleFileAsync();
            if(file!=null)
            {
                Frame.Navigate(typeof(ImageHandlePage), file);
            }
            
        }

        private void positionCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isposComDirty == false)
            {
                isposComDirty = true;
                return;
            }
            var combox = sender as ComboBox;
            if (combox != null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("Position", selectedIndex.ToString());
            }
        }
        private async void EmailClick(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=dengweichao@hotmail.com&subject=MyerMoment for PC Feedback&body=");

            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }


        private async void ReviewClick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
                            new Uri("ms-windows-store:reviewapp?appid=126a1e6d-0f68-4b89-a67c-fe3d204508ec"));
        }

        private async void TapToStore(object sender, TappedRoutedEventArgs e)
        {
            var mailto = new Uri("http://www.windowsphone.com/s?appid=52172f00-cfe1-4695-b4f7-a2be0c1bfacc");

            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //SplashStory.Begin();
            Frame.BackStack.Clear();
        }

    }
}
