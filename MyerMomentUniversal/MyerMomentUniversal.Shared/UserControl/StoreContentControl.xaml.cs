using MyerMomentUniversal.Helper;
using MyerMomentUniversal.ViewModel;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyerMomentUniversal
{
    public sealed partial class StoreContentControl : UserControl
    {

        StylesViewModel StylesVM;
        public StoreContentControl()
        {
            this.InitializeComponent();

            StylesVM = new StylesViewModel();
            this.DataContext = StylesVM;
        }

        private async void GetAllStyle()
        {
            

            //var names=await HttpHelper.GetAllStylesAsync();
            //styleListVM.ConfigWebStyle(names);

            //this.DataContext = styleListVM;

        }
    }
}
