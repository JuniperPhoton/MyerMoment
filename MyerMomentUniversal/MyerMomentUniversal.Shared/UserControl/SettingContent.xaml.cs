using JP.Utils.Data;
using NotificationsExtensions.TileContent;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;


namespace MyerMomentUniversal
{
    public sealed partial class SettingContent : UserControl
    {
        private bool iscomComDirty = false;
        private bool isposComDirty = false;
        private bool iscolorComDirty = false;
        private bool islangComDirty = false;

        public SettingContent()
        {
            this.InitializeComponent();

//#if WINDOWS_APP
//            tileTitle.Visibility = colorCom.Visibility = Visibility.Collapsed;
//#endif

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

            var lang = LocalSettingHelper.GetValue("AppLang");
            if (lang.Contains("zh"))
            {
                langCom.SelectedIndex = 1;
            }
            else langCom.SelectedIndex = 0;
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

        private void lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (islangComDirty == false)
            {
                islangComDirty = true;
                return;
            }
            var combox = sender as ComboBox;
            if (combox != null)
            {
                var selectedIndex = combox.SelectedIndex;
                LocalSettingHelper.AddValue("AppLang", selectedIndex.ToString());
                ChangeLanguage();
            }
        }

        private void ChangeLanguage()
        {
            if (langCom.SelectedIndex == 1)
            {
                ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                LocalSettingHelper.AddValue("AppLang", "zh-CN");
                var resourceContext = ResourceContext.GetForCurrentView();
                resourceContext.Reset();
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                LocalSettingHelper.AddValue("AppLang", "en-US");
                var resourceContext = ResourceContext.GetForCurrentView();
                resourceContext.Reset();
            }
        }

    }
}
