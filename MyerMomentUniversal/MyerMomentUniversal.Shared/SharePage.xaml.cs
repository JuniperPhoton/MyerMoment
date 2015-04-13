using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace MyerMomentUniversal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SharePage : Page, IWebAuthenticationContinuable
    {
        private StorageFile fileToShare = null;

        public SharePage()
        {
            this.InitializeComponent();

            SdkData.AppKey = "1789969413";
            SdkData.AppSecret = "32f53228d6c2f2f798b216a1a0830667";
            SdkData.RedirectUri = "https://api.weibo.com/oauth2/default.html";
        }

        private void ConfigUI(bool isSuccess)
        {
            if (isSuccess)
            {
                backHomeBtn.Visibility = Visibility.Visible;
                retryBtn.Visibility = Visibility.Collapsed;
                ring.IsActive = false;
                statusTB.Text = "Successfully Share :D";
            }
            else
            {
                backHomeBtn.Visibility = Visibility.Visible;
                retryBtn.Visibility = Visibility.Visible;
                ring.IsActive = false;
                statusTB.Text = "Fail to Share :-(";
            }
        }

        private void BackHomeClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void RetryClick(object sender, RoutedEventArgs e)
        {
            ConfigWeiboShare();
        }

        private async Task ShowImage()
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

        private async void ShareImage()
        {
            if (!ChaoFunctionRT.NetworkHelper.HasNetWork())
            {
                ConfigUI(false);
                return;
            }
            var engine = new SdkNetEngine();
            CmdPostMsgWithPic cmdBase = new CmdPostMsgWithPic();
            cmdBase.Status = "Post a picture with #MyerMoment#";
            if (fileToShare != null)
            {
                cmdBase.PicPath = fileToShare.Path;
                var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE_PIC, cmdBase);
                if (result.errCode == 0)
                {
                    ConfigUI(true);
                }
                else
                {
                    ConfigUI(false);
                }
            }
            else
            {

            }
        }

        private void ConfigWeiboShare()
        {
            if (!ChaoFunctionRT.NetworkHelper.HasNetWork())
            {
                ConfigUI(false);
                return;
            }
            var oauthClient = new ClientOAuth();
            // 判断是否已经授权或者授权是否过期.
            if (oauthClient.IsAuthorized == false)
            {
                oauthClient.LoginCallback += (isSucces, err, response) =>
                {
                    if (isSucces)
                    {
                        ShareImage();
                    }
                    else
                    {

                    }
                };
                oauthClient.BeginOAuth();
            }
            else
            {
                ShareImage();
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(PageNavigateData))
            {
                this.fileToShare = (e.Parameter as PageNavigateData).file;

                await ShowImage();
                ConfigWeiboShare();
                //ShareImage();
            }
        }

        public void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var oauthClient = new ClientOAuth();
            oauthClient.LoginCallback += (isSucces, err, response) =>
            {
                if (isSucces)
                {
                }
                else
                {
                }

            };
            oauthClient.continueAuth(args.WebAuthenticationResult); 
        }
    }
}
