using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using UmengSocialSDK;
using ChaoFunctionRT;
using Windows.UI.ViewManagement;

namespace MyerMomentUniversal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SharePage : Page
    {
        string fileName;

        public SharePage()
        {
            this.InitializeComponent();
        }

        private async void ShowImage(string fileName)
        {
            var file = await Windows.Storage.KnownFolders.SavedPictures.GetFileAsync(fileName);
            using(var fileStream = await file.OpenStreamForReadAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(fileStream.AsRandomAccessStream());
                this.image.Source = bitmap;
            }
        }

        private async void ShareClick(object sender,RoutedEventArgs e)
        {
            try
            {
                UmengClient umengClient = new SinaWeiboClient("5506e8eafd98c52426000587");

                var file = await Windows.Storage.KnownFolders.SavedPictures.GetFileAsync(fileName);
                using (var fileStream = await file.OpenStreamForReadAsync())
                {
                    byte[] imageData = new byte[fileStream.Length];
                    fileStream.Read(imageData, 0, imageData.Length);

                    var pic = new UmengPicture(imageData, "Share image with #MyerMoment#");
                    var task = umengClient.SharePictureAsync(pic, true);

                    await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

                    var result=await task;

                    await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();

                    if(result.Status==ShareStatus.Success)
                    {
                        BackHomeClick(null, null);
                    }

                }
            }
            catch(Exception ee)
            {
                var task = ExceptionHelper.WriteRecord(ee);
            }
            
        }

        private void BackHomeClick(object sender,RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            fileName = e.Parameter as String;
            ShowImage(fileName);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if(Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }
    }
}
