using ChaoFunctionRT;
using Scheduler.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MyerMomentUniversal
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;        

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;

            
            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;

            var quality = LocalSettingHelper.GetValue("Quality");
            if(quality!=null)
            {
                qualityCom.SelectedIndex = int.Parse(quality);
            }

            var position = LocalSettingHelper.GetValue("Position");
            if(position!=null)
            {
                positionCom.SelectedIndex = int.Parse(position);
            }
        }

        private async void EmailClick(object sender,RoutedEventArgs e)
        {
            EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
            EmailMessage mes = new EmailMessage();
            mes.To.Add(rec);
            mes.Subject = "MyerMoment feedback";
            await EmailManager.ShowComposeNewEmailAsync(mes);
        }

        private async void SendLogClick(object sender,RoutedEventArgs e)
        {
            EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
            EmailMessage mes = new EmailMessage();
            var error = await ExceptionHelper.ReadRecord();
            mes.Body = error;
            mes.To.Add(rec);
            mes.Subject = "MyerMoment error log";
            await EmailManager.ShowComposeNewEmailAsync(mes);
        }

        private void ReviewClick(object sender,RoutedEventArgs e)
        {

        }

        private void OpenPhotoClick(object sender,RoutedEventArgs e )
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
            if(args.Files.Count==0)
            {
                return;
            }
            switch (args.Kind)
            {
                case ActivationKind.PickFileContinuation:
                    {
                        Frame.Navigate(typeof(ImageHandlePage), args);
                    } break;
                case ActivationKind.ShareTarget:
                    {
                        Frame.Navigate(typeof(ImageHandlePage), args);
                    }break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            SplashStory.Begin();
            Frame.BackStack.Clear();
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

        private void qualityCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            if(combox!=null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("Quality", selectedIndex.ToString());
            }
        }

        private void positionCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            if(combox!=null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("Position", selectedIndex.ToString());
            }
        }

        
    }
}
