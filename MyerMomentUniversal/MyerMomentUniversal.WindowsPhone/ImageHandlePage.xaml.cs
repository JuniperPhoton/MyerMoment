using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using MyerMomentUniversal.ViewModel;
using Windows.UI.Xaml.Media;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Input;
using ChaoFunctionRT;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Text;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MyerMomentUniversal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImageHandlePage : Page
    {
      
        private bool _isInColorMode = false;
        private bool _isInFontFamilyMode = false;

        private double _angle = 0;

        private uint _height;
        private uint _width;
        private int _dpiX;
        private int _dpiY;

        private double _imageBorderH;
        private double _imageBorderW;

        private TranslateTransform _tranTemplete = new TranslateTransform();

        public ImageHandlePage()
        {
            this.InitializeComponent();
        }

        #region FUNCTION
        private void IncreaseFontsizeClick(object sender,RoutedEventArgs e)
        {
            TextView.FontSize += 5;
        }

        private void DecreaseFontsizeClick(object sender, RoutedEventArgs e)
        {
            TextView.FontSize -= 5;
        }

        private void BoldFontClick(object sender,RoutedEventArgs e)
        {
            if (TextView.FontWeight.Weight == Windows.UI.Text.FontWeights.Bold.Weight)
            {
                TextView.FontWeight = Windows.UI.Text.FontWeights.Normal;
            }
            else TextView.FontWeight = Windows.UI.Text.FontWeights.Bold;
        }

        private void ItalicFontClick(object sender, RoutedEventArgs e)
        {
            if (TextView.FontStyle == Windows.UI.Text.FontStyle.Normal)
            {
                TextView.FontStyle = Windows.UI.Text.FontStyle.Italic;
            }
            else TextView.FontStyle = Windows.UI.Text.FontStyle.Normal;
        }

        private void ColorClick(object sender,RoutedEventArgs e)
        {
            if(_isInColorMode)
            {
                ColorOutStory.Begin();
                _isInColorMode = false;
            }
            else
            {
                ColorInStory.Begin();
                _isInColorMode = true;
            }
        }

        private void FontColorClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var border = btn.Content as Border;
            if(border!=null)
            {
                TextView.Foreground = border.Background;
            }
        }
        #endregion

        #region TEXT_MANI
        private void TextView_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid.ManipulationDelta -= TextView_ManipulationDelta;
            textGrid.ManipulationDelta += TextView_ManipulationDelta;

            textGrid.ManipulationCompleted -= TextView_ManipulationCompleted;
            textGrid.ManipulationCompleted += TextView_ManipulationCompleted;

            textGrid.RenderTransform = _tranTemplete;
        }

        void TextView_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var x = _tranTemplete.X;
            var y = _tranTemplete.Y;
            if (x > _imageBorderW ) _tranTemplete.X= _imageBorderW;
            if (x < -_imageBorderW) _tranTemplete.X = -_imageBorderW;
            if (y > _imageBorderH) _tranTemplete.Y = _imageBorderH;
            if (y < -_imageBorderH) _tranTemplete.Y = -_imageBorderH+TextView.ActualHeight;

        }

        private void TextView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
                _tranTemplete.X += e.Delta.Translation.X;
                _tranTemplete.Y += e.Delta.Translation.Y;
        }

        private void TextView_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {

        }

        #endregion

        #region OPERATE
        private void RotateClick(object sender,RoutedEventArgs e)
        {
            _angle += 90;

            var finalAngle = _angle;
            startAngle.Value = finalAngle - 90;
            endAngle.Value = finalAngle;

            var newwidth = _height;
            var newheight = _width;

            this._height = newheight;
            this._width = newwidth;
            

            RotateStory.Begin();
        }

        private void SaveClick(object sender,RoutedEventArgs e)
        {
            SaveImage(renderGrid);
        }

        private async void ShowImage(StorageFile file)
        {
            try
            {
                //ShowProgressRingVisibility = Visibility.Visible;
                TextView.Visibility = Visibility.Collapsed;

                var fileStream = await file.OpenAsync(FileAccessMode.Read);

                //从文件流里创建解码器
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                this._dpiX = (int)decoder.DpiX;
                this._dpiY = (int)decoder.DpiY;
                this._height = decoder.OrientedPixelHeight;
                this._width = decoder.OrientedPixelWidth;

                //显示图像
                var bitmap = new BitmapImage();
                bitmap.SetSource(fileStream);
                image.Source = bitmap;

                TextView.Visibility = Visibility.Visible;

                image.UpdateLayout();
                this._imageBorderH = (image.ActualHeight / 2);
                this._imageBorderW = (image.ActualWidth / 2);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
            }
        }

        private async void SaveImage(UIElement elementToRender)
        {
            try
            {
                MaskGrid.Visibility = Visibility.Visible;

                uint targetWidth = this._width;
                uint targetHeight = this._height;

                var fileToSave = await KnownFolders.SavedPictures.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);

                CachedFileManager.DeferUpdates(fileToSave);
                using (var fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(elementToRender, (int)targetWidth, (int)targetHeight);

                    var pixels = await bitmap.GetPixelsAsync();

                    var propertySet = new BitmapPropertySet();

                    var qualityValue = new BitmapTypedValue(1.0, PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream, propertySet);

                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                MaskGrid.Visibility = Visibility.Collapsed;

                var rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                MaskGrid.Visibility = Visibility.Collapsed;
                //new MessageDialog(e.Message.ToString(), "Error").ShowAsync();
            }


        }

        private async void CancelClick(object sender,RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("You are going to give up editing the photo.", "CONFIRM");

            var rootFrame = Window.Current.Content as Frame;

            md.Commands.Add(new UICommand("YES", act =>
            {
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }));
            md.Commands.Add(new UICommand("CANCEL", act =>
            {
                return;
            }));
            await md.ShowAsync();
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            if(e.Parameter!=null)
            {
                FileOpenPickerContinuationEventArgs args = e.Parameter as FileOpenPickerContinuationEventArgs;
                if (args.Files.Count == 0) return;

                ShowImage(args.Files.FirstOrDefault());
                //Messenger.Default.Send<GenericMessage<StorageFile>>(new GenericMessage<StorageFile>(args.Files[0]), "PickedFile");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if(_isInFontFamilyMode==true || _isInColorMode==true)
            {
                if (_isInColorMode)
                {
                    ColorOutStory.Begin();
                    _isInColorMode = false;
                }
                return;
            }
            CancelClick(null, null);
        }

        private void TextView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void textGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            maskBorder.Visibility = Visibility.Collapsed;
            TextView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void textGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            maskBorder.Visibility = Visibility.Visible;

        }

        
    }
}

