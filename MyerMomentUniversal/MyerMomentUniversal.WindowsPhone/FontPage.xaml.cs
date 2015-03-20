using MyerMomentUniversal.ViewModel;
using Scheduler.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MyerMomentUniversal
{
   
    public sealed partial class FontPage : Page, IFileOpenPickerContinuable
    {
        FontViewModel fontVM;

        public FontPage()
        {
            this.InitializeComponent();

            fontVM = new FontViewModel();
            this.DataContext = fontVM;

            TextBlock tb = new TextBlock();
            
        }


        #region NAVIGATE
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            //ReadFontsList();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if(Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
        #endregion

        public void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count == 0)
            {
                return;
            }
            switch (args.Kind)
            {
                case ActivationKind.PickFileContinuation:
                    {
                        var file = args.Files[0];

                        fontVM.SaveFont(file);
                    } break;
            }
        }
    }
}
