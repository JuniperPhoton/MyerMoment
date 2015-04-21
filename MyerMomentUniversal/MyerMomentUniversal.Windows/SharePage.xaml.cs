using JP.Utils.Network;
using MyerMomentUniversal.Model;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyerMomentUniversal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SharePage : Page
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
            if (!NetworkHelper.HasNetWork())
            {
                //shareControl.ShowErrorGrid();
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter.GetType()==typeof(PageNavigateData))
            {
                shareControl.fileToShare = (e.Parameter as PageNavigateData).file;
                await shareControl.ShowImageAsync();
                ConfigWeiboShare();
            }
        }
    }
}
