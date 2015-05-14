using JP.Utils.Debug;
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

        public StylesViewModel StylesVM;
        public StoreContentControl()
        {
            this.InitializeComponent();

            StylesVM = new StylesViewModel();
            this.DataContext = StylesVM;
            GetNewStyle();
        }

        private async void GetNewStyle()
        {
            await ExceptionHelper.TryExecute(async () =>
            {
                var names = await HttpHelper.GetAllStylesAsync();
                await StylesVM.ConfigWebStyle(names);
            });
            
        }

        private async void DownloadClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = btn.Tag as string;

            await StylesVM.DownloadFullsizeCommand(tag);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var image = grid.Children.FirstOrDefault() as Image;

            image.Width = (installedListView.ActualWidth-30)/2;
            image.Height = image.Width;

            var image2 = grid.Children.LastOrDefault() as Image;
            image2.Width = image2.Height = (image.Width) / 2;
        }
    }
}
