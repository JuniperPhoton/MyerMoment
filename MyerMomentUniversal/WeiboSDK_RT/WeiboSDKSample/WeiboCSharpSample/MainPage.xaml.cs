using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WeiboSDKForWinRT;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WeiboCSharpSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string picPath = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();

            InitData();
        }

        private void InitData()
        {
            // TODO:编译运行之前需要开放平台参数.
            SdkData.AppKey = "1789969413";
            SdkData.AppSecret = "32f53228d6c2f2f798b216a1a0830667";
            SdkData.RedirectUri = "https://api.weibo.com/oauth2/default.html"; 

            // prepare the pic to be shared.
            CopyToIso("Assets/weibo.png", "weibo");
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var oauthClient = new ClientOAuth();
            // 判断是否已经授权或者授权是否过期.
            if (oauthClient.IsAuthorized == false)
            {
                oauthClient.LoginCallback += (isSucces, err, response) =>
                {
                    if (isSucces)
                    {
                        // TODO: deal the OAuth result.
                        this.statusRun.Text = "Congratulations, Authorized successfully!";
                        this.ResultRun.Text = string.Format("AccesssToken:{0}, ExpriesIn:{1}, Uid:{2}",
                            response.AccessToken, response.ExpriesIn, response.Uid);
                    }
                    else
                    {
                        // TODO: handle the err.
                        this.statusRun.Text = err.errMessage;
                    }
                };
                oauthClient.BeginOAuth();
            }
        }

        private async void TimelineBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            var engine = new SdkNetEngine();
            ISdkCmdBase cmdBase = new CmdTimelines()
            {
                Count = "20",
                Page = "1"
            };
            var result = await engine.RequestCmd(SdkRequestType.FRIENDS_TIMELINE, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                this.ResultRun.Text = result.content;
                this.statusRun.Text = "Fetch friend's Timeline successed！";
            }
            else
            {
                // TODO: deal the error.
                this.statusRun.Text = "Fetch friend's Timeline failed！";
            }
            progressBar.IsIndeterminate = false;
        }

        private async void NoPicPostBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            var engine = new SdkNetEngine();
            ISdkCmdBase cmdBase = new CmdPostMessage()
            {
                Status = "test for post message without picture"
            };

            var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE, cmdBase);
            if (result.errCode == SdkErrCode.SUCCESS)
            {
                this.ResultRun.Text = result.content;
                this.statusRun.Text = "Post a message without picture successed！";
            }
            else
            {
                // TODO: deal the error.
                this.statusRun.Text = "Post a message without picture failed！";
            }
            progressBar.IsIndeterminate = false;
        }

        private async void MsgWithPicBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            var engine = new SdkNetEngine();
            ISdkCmdBase cmdBase = new CmdPostMsgWithPic()
            {
                Status = "test for post message with picture",
                PicPath = picPath
            };

            var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE_PIC, cmdBase);
            if (result.errCode == SdkErrCode.SUCCESS)
            {
                this.ResultRun.Text = result.content;
                this.statusRun.Text = "Post a message with picture successed！";
            }
            else
            {
                // TODO: deal the error.
                this.statusRun.Text = "Post a message with picture failed！";
            }
            progressBar.IsIndeterminate = false;
        }

       
        private async void CopyToIso(string source, string dest)
        {
            try
            {
                // Get image buffer.
                var uri = new Uri("ms-appx:///" + source);
                StorageFile sourceFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var dataBuffer = await Windows.Storage.FileIO.ReadBufferAsync(sourceFile);

                StorageFolder picFolder = ApplicationData.Current.LocalFolder;
                // write buffer to storage.
                StorageFile file = await picFolder.CreateFileAsync(sourceFile.Name, CreationCollisionOption.OpenIfExists);

                await Windows.Storage.FileIO.WriteBufferAsync(file, dataBuffer);
                picPath = file.Path;
            }
            catch (Exception err)
            {
                // Note: exception  happed.
            }
        }

        private async void TestRemainApi(SdkRequestType type)
        {
            progressBar.IsIndeterminate = true;
            var engine = new SdkNetEngine();
            ISdkCmdBase cmdbase = null;

            switch (type)
            {
                case SdkRequestType.FRIENDSHIP_CREATE:
                    {
                        cmdbase = new CmdFriendShip()
                        {
                            ScreenName = "JoelJay"
                        };
                    }
                    break;
                case SdkRequestType.FRIENDSHIP_DESDROY:
                    {
                        cmdbase = new CmdFriendShip()
                        {
                            ScreenName = "JoelJay"
                        };
                    }
                    break;
                case SdkRequestType.FRIENDSHIP_SHOW:
                    {
                        cmdbase = new CmdFriendShip()
                        {
                            SourceScreenName = "花之粥",
                            ScreenName = "JoelJay"
                        };
                    }
                    break;
                case SdkRequestType.AT_USERS:
                    {
                        cmdbase = new CmdAtUsers()
                        {
                            Keyword = "a"
                        };
                    }
                    break;
                case SdkRequestType.USER_TIMELINE:
                    {
                        cmdbase = new CmdUserTimeline()
                        {
                            UserId = "2111525197",
                            Count = "20",
                            Page = "1"
                        };
                    }
                    break;
                case SdkRequestType.SHORTEN_URL:
                    {
                        cmdbase = new CmdShortenUrl()
                        {
                            OriginalUrl = "http://wwww.baidu.com"
                        };
                    }
                    break;
                case SdkRequestType.OTHER_API:
                    {
                        cmdbase = new CmdFriendList()
                        {
                            Count = "20",
                            Uid = "2111525197",
                        };
                    }
                    break;
                default:
                    break;
            }
            var result = await engine.RequestCmd(type, cmdbase);
            if (result.errCode == SdkErrCode.SUCCESS)
            {
                this.ResultRun.Text = result.content;
            }
            else
            {
                this.statusRun.Text = "the api didn't work correctly.";
            }
            progressBar.IsIndeterminate = false;
        }

        int count = 0;
        private void OtherAPI_Click(object sender, RoutedEventArgs e)
        {
            SdkRequestType type = SdkRequestType.NULL_TYPE;
            if (count == 0)
                type = SdkRequestType.USER_TIMELINE;
            else if (count == 1)
                type = SdkRequestType.FRIENDSHIP_CREATE;
            else if (count == 2)
                type = SdkRequestType.FRIENDSHIP_DESDROY;
            else if (count == 3)
                type = SdkRequestType.FRIENDSHIP_SHOW;
            else if (count == 4)
                type = SdkRequestType.SHORTEN_URL;
            else if (count == 5)
                type = SdkRequestType.OTHER_API;

            TestRemainApi(type);
            count++;
        }
    }
}
