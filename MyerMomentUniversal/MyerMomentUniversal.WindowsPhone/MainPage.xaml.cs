using ChaoFunctionRT;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WeiboSDKForWinRT;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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

    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;
        private bool iscomComDirty = false;
        private bool isposComDirty = false;
        private bool iscolorComDirty = false;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;

            ConfigLang();

            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;

            VersionHLB.Content = (string)(App.Current.Resources["AppVersion"]);
        }

        private void ConfigLang()
        {
            
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            StartPivotItem.Header = loader.GetString("StartHeader");
            SettingPivotItem.Header = loader.GetString("PersonalizeHeader");
            AboutPivotItem.Header = loader.GetString("AboutHeader");
            pickHintTextblock.Text = loader.GetString("PickPhotoHint");
            savingQualityTextblock.Text = loader.GetString("SavingQuality");
            savingPositionTextblock.Text = loader.GetString("SavingPosition");
            compressTextblock.Text = loader.GetString("LowHint");
            oriTextblock.Text = loader.GetString("HighHint");
            savedPictureTextblock.Text = loader.GetString("SavedPictureHint");
            folderTextblock.Text = loader.GetString("MyerMomentFolderHint");
            cameraRollTextblock.Text = loader.GetString("CameraRollHint");
            rateTextblock.Text = loader.GetString("RateHint");
            //errorLogTextblock.Text = loader.GetString("SendLogHint");
            feedbackTextblock.Text = loader.GetString("SendEmailHint");
            //creditTB.Text = loader.GetString("CreditHint");
            //importHint.Text = loader.GetString("ImportHint");
            //importTB.Text = loader.GetString("ImportHeader");
            tileColorTextblock.Text = loader.GetString("TileColorHeader");
            transparantTextblock.Text = loader.GetString("TransparantHint");
            solidcolorTextblock.Text = loader.GetString("ThemeHint");
            
        }

        #region FEEDBACK
        private async void EmailClick(object sender, RoutedEventArgs e)
        {
            EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
            EmailMessage mes = new EmailMessage();
            mes.To.Add(rec);
            mes.Subject = "MyerMoment for Phone Feedback";
            await EmailManager.ShowComposeNewEmailAsync(mes);
        }


        private async void ReviewClick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
                            new Uri("ms-windows-store:reviewapp?appid=126a1e6d-0f68-4b89-a67c-fe3d204508ec"));
        }

        private void ManageClick(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(FontPage));
        }
        #endregion

        private void OpenPhotoClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.PickSingleFileAndContinue();
        }

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

        private void CancelAuthClick(object sender,RoutedEventArgs e)
        {
            var oauthClient = new ClientOAuth();
            if(oauthClient.IsAuthorized)
            {
                oauthClient.IsAuthorized = false;
            }
        }

        private void qualityCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (iscomComDirty == false)
            {
                iscomComDirty = true;
                return;
            }
            var combox = sender as ComboBox;
            if (combox != null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("QualityCompress", selectedIndex.ToString());
            }
        }

        private void colorCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (iscolorComDirty == false)
            {
                iscolorComDirty = true;
                return;
            }
            var combox = sender as ComboBox;
            if (combox != null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("TileColor", selectedIndex.ToString());

                switch (selectedIndex)
                {
                    case 1:
                        {
                            var smallTileContent = TileContentFactory.CreateTileSquare71x71Image();
                            smallTileContent.Image.Src = "ms-appx:///Assets/newLogo_tran_71.png";

                            //medium
                            var mediumTileContent = TileContentFactory.CreateTileSquare150x150Image();
                            mediumTileContent.RequireSquare71x71Content = true;
                            mediumTileContent.Square71x71Content = smallTileContent;
                            mediumTileContent.Image.Src = "ms-appx:///Assets/newLogo_tran.png";
                            mediumTileContent.Branding = TileBranding.Name;

                            var notification = mediumTileContent.CreateNotification();
                            
                            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
                        }
                        break;
                    case 0:
                        {
                            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                        }
                        break;
                }
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            SplashStory.Begin();
            Frame.BackStack.Clear();

            var quality = LocalSettingHelper.GetValue("QualityCompress");
            if (quality != null)
            {
                qualityCom.SelectedIndex = int.Parse(quality);
            }

            var position = LocalSettingHelper.GetValue("Position");
            if (position != null)
            {
                positionCom.SelectedIndex = int.Parse(position);
            }

            var color = LocalSettingHelper.GetValue("TileColor");
            if (color != null)
            {
                colorCom.SelectedIndex = int.Parse(color);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }


    }
}
