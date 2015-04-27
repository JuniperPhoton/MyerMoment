using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace MyerMomentUniversal
{
    public sealed partial class AboutFlyoutPage : SettingsFlyout
    {
        public AboutFlyoutPage()
        {
            this.InitializeComponent();

            VersionHLB.Content = (string)(App.Current.Resources["AppVersion"]);
        }

        private async void EmailClick(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=dengweichao@hotmail.com&subject=MyerMoment for PC Feedback&body=");

            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }


        private async void ReviewClick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
                            new Uri("ms-windows-store:reviewapp?appid=126a1e6d-0f68-4b89-a67c-fe3d204508ec"));
        }

        private async void TapToStore(object sender, TappedRoutedEventArgs e)
        {
            var mailto = new Uri("http://www.windowsphone.com/s?appid=52172f00-cfe1-4695-b4f7-a2be0c1bfacc");

            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }


    }
}
