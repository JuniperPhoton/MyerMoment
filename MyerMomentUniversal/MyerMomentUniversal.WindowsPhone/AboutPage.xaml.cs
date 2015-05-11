using DialogExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace MyerMomentUniversal
{
  
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();

            VersionHLB.Content = (string)(App.Current.Resources["AppVersion"]);

            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;

            VersionHLB.Content = (App.Current.Resources["AppVersion"]) as string;
        }

        private void DonateClick(object sender,RoutedEventArgs e)
        {
            var dialog = new BottomDialog((senderl, el) =>
              {
                  var dia = senderl as BottomDialog;
                  dia.Hide();
              }, (senderr, er) =>
             {
                 var dia = senderr as BottomDialog;
                 dia.Hide();
             });
            dialog.TitleContent = "确认捐赠";
            dialog.ContentContent = "此自愿行为将帮助作者改善 MyerMoment 和 MyerList 的质量。";
            dialog.VerticalContentAlignment = VerticalAlignment.Top;
            dialog.Show();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            StatusBar.GetForCurrentView().BackgroundOpacity = 0;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }
    }
}
