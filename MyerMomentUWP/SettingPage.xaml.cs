using DialogExt;
using JP.Utils.Data;
using MyerMomentUWP.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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


namespace MyerMomentUniversal
{
    public sealed partial class SettingPage : BasePage
    {
        private bool iscomComDirty = false;
        private bool isposComDirty = false;
        private bool islangComDirty = false;

        public SettingPage()
        {
            this.InitializeComponent();

            if(ApiInformationHelper.HasStatusBar)
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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(ApiInformationHelper.HasStatusBar)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0;
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
