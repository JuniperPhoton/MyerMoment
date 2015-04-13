using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace MyerMomentUniversal
{
    public sealed partial class ShareControl : UserControl
    {
        public StorageFile fileToShare = null;

        public ShareControl()
        {
            this.InitializeComponent();
        }

        private void ConfigLang()
        {

        }

        public void ShowSharingGrid()
        {
            sharingGrid.Visibility = Visibility.Visible;
            errorGrid.Visibility = Visibility.Collapsed;
            successGrid.Visibility = Visibility.Collapsed;
        }
        public void HideSharingGrid()
        {
            sharingGrid.Visibility = Visibility.Collapsed;
        }

        public void ShowErrorGrid()
        {
            sharingGrid.Visibility = Visibility.Collapsed;
            errorGrid.Visibility = Visibility.Visible;
            successGrid.Visibility = Visibility.Collapsed;
        }
        public void HideErrorGrid()
        {
            errorGrid.Visibility = Visibility.Collapsed;
        }

        public void ShowSuccessGrid()
        {
            sharingGrid.Visibility = Visibility.Collapsed;
            errorGrid.Visibility = Visibility.Collapsed;
            successGrid.Visibility = Visibility.Visible;
        }
        public void HideSuccessGrid()
        {
            successGrid.Visibility = Visibility.Collapsed;
        }

        public void BackHomeClick(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if(rootFrame!=null)
            {
                rootFrame.Navigate(typeof(MainPage));
            }
        }

        public void shareClick(object sender, RoutedEventArgs e)
        {
            ShareImage();
        }

        public async Task ShowImageAsync()
        {
            if (this.fileToShare != null)
            {
                using (var fileStream = await fileToShare.OpenStreamForReadAsync())
                {
                    BitmapImage bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(fileStream.AsRandomAccessStream());
                    image.Source = bitmap;
                }
            }
        }

        public async void ShareImage()
        {
            if (!ChaoFunctionRT.NetworkHelper.HasNetWork())
            {
                ShowErrorGrid();
                return;
            }

            var engine = new SdkNetEngine();
            CmdPostMsgWithPic cmdBase = new CmdPostMsgWithPic();
            if (!String.IsNullOrEmpty(contentTB.Text))
            {
                cmdBase.Status = contentTB.Text;
            }
            else
            {
#if WINDOWS_APP
                cmdBase.Status = "使用 Windows 版 #MyerMoment# 分享一张图片. ";
#elif WINDOWS_PHONE_APP
                cmdBase.Status="使用 Windows Phone 版 #MyerMoment# 分享一张图片. ";
#endif
            }
            if (fileToShare != null)
            {
                cmdBase.PicPath = fileToShare.Path;

                sharingGrid.Visibility = Visibility.Visible;

                var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE_PIC, cmdBase);
                if (result.errCode == 0)
                {
                    ShowSuccessGrid();
                }
                else
                {
                    ShowErrorGrid();
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key==VirtualKey.Enter)
            {
                shareClick(null, null);
                
            }
        }
    }
}
