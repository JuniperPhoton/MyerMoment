using JP.Utils.Data;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyerMomentUniversal
{
    public sealed partial class SettingPage : Page
    {
        private bool iscomComDirty = false;
        private bool isposComDirty = false;
        private bool iscolorComDirty = false;

        public SettingPage()
        {
            this.InitializeComponent();

            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;
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

                            //wide
                            var wideTileContent = TileContentFactory.CreateTileWide310x150Image();
                            wideTileContent.Square150x150Content = mediumTileContent;
                            wideTileContent.Image.Src = "ms-appx:///Asset/Tile/LOGO_WIDE_TRAN.png";
                            wideTileContent.Branding = TileBranding.None;

                            var notification = wideTileContent.CreateNotification();

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
            StatusBar.GetForCurrentView().BackgroundOpacity = 0;

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
