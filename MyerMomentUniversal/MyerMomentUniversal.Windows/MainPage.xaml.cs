using ChaoFunctionRT;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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


       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //SplashStory.Begin();
            Frame.BackStack.Clear();
        }

    }
}
