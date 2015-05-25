using JP.Utils.Network;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Threading;
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

    public sealed partial class SharePage : Page, IWebAuthenticationContinuable
    {
        public SharePage()
        {
            this.InitializeComponent();

            SdkData.AppKey = "1789969413";
            SdkData.AppSecret = "32f53228d6c2f2f798b216a1a0830667";
            SdkData.RedirectUri = "https://api.weibo.com/oauth2/default.html";
        }

        private void ConfigWeiboShare()
        {
            try
            {
                if (!NetworkHelper.HasNetWork())
                {
                    shareControl.ShowErrorGrid();
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

                        }
                    };
                    oauthClient.BeginOAuth();
                }
                else
                {
                    //ShareImage();
                }
            }
            catch(Exception)
            {

            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
 	        HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if (e.Parameter.GetType() == typeof(PageNavigateData))
            {
                shareControl.fileToShare = (e.Parameter as PageNavigateData).file;
                await shareControl.ShowImageAsync();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.6);
                timer.Tick += ((sendert, et) =>
                    {
                        ConfigWeiboShare();
                        timer.Stop();
                    });
                timer.Start();
            }
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
        }

        public void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            try
            {
                var oauthClient = new ClientOAuth();
                oauthClient.LoginCallback += (async (isSucces, err, response) =>
                {
                    if (isSucces)
                    {
                        await shareControl.ShowImageAsync();
                    }
                    else
                    {
                        shareControl.ShowErrorGrid();
                    }
                });
                oauthClient.continueAuth(args.WebAuthenticationResult);
            }
            catch(Exception e)
            {

            }
        }
    }
}
