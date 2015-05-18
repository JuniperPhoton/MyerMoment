using JP.Utils.Debug;
using JP.Utils.Network;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.ViewModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace MyerMomentUniversal
{
    public sealed partial class StoreContentControl : UserControl
    {

        public StylesViewModel StylesVM;
        public StoreContentControl()
        {
            this.InitializeComponent();

            if(DesignMode.DesignModeEnabled)
            {
                loadingGrid.Visibility = Visibility.Collapsed;
            }

            StylesVM = new StylesViewModel();
            this.DataContext = StylesVM;

           GetNewStyle();
            
        }

        private async void GetNewStyle()
        {
            await ExceptionHelper.TryExecute(async () =>
            {
                var namesArray = await HttpHelper.GetAllStylesAsync();
                await StylesVM.ConfigWebStyle(namesArray);
            });
            
        }

        private async void DownloadClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = btn.Tag as string;

            await StylesVM.DownloadFullsizeCommand(tag);
        }

        private async void DeleteClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = btn.Tag as string;
            await StylesVM.DeleteStyle(tag);
        }

        private void RefreshClick(object sender,RoutedEventArgs e)
        {
            GetNewStyle();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var backgrdImage = grid.Children.FirstOrDefault() as Image;

            backgrdImage.Width = (installedListView.ActualWidth-30)/2;
            backgrdImage.Height = backgrdImage.Width;

            var styleImage = grid.Children.ElementAt(2) as Image;
            styleImage.Width = styleImage.Height = (backgrdImage.Width) / 1.7;
        }

    }
}
