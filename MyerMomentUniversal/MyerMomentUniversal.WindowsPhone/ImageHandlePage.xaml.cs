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
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using System.Collections.Generic;
using UmengSocialSDK;
using Windows.ApplicationModel.DataTransfer;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using Windows.UI;


namespace MyerMomentUniversal
{

    public sealed partial class ImageHandlePage : Page
    {
        private string _savedFileName = "";

        private bool _isInColorMode = false;
        private bool _isInFontFamilyMode = false;
        private bool _isInStyleMode = false;
        private bool _isInShareMode = false;
        private bool _isInErrorMode = false;

        private bool _isFromShareTarget = false;

        private double _angle = 0;

        private uint _height;
        private uint _width;
        private int _dpiX;
        private int _dpiY;

        private string _selectedText = "";

        private TranslateTransform _translateTransform = new TranslateTransform();
        private ScaleTransform _scaleTransform = new ScaleTransform();
        private TransformGroup _transformGroup = new TransformGroup();

        private StyleList styleList;

        public ImageHandlePage()
        {
            this.InitializeComponent();
   
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            ConfigLang();

            DataTransferManager.GetForCurrentView().DataRequested += dataTransferManager_DataRequested;

            _transformGroup.Children.Add(_translateTransform);
            _transformGroup.Children.Add(_scaleTransform);

            styleList = new StyleList();
            foreach(var style in styleList.Styles)
            {
                Button styleBtn = new Button();
                styleBtn.BorderThickness = new Thickness(0);
                styleBtn.Style = (App.Current.Resources["ButtonStyle2"] as Style);
                styleBtn.MinHeight = styleBtn.MinWidth = 10;
                styleBtn.Height = styleBtn.Width = 70;
                styleBtn.VerticalAlignment = VerticalAlignment.Top;
                styleBtn.Margin = new Thickness(10, 10, 0, 0);
                styleBtn.Click += ((senderb, eb) =>
                    {
                        styleImage.Source = style.Image;
                        familyBtn.Visibility = colorBtn.Visibility = Visibility.Collapsed;

                        textGrid.Visibility = Visibility.Collapsed;
                        styleGrid.Visibility = Visibility.Visible;
                    });

                Border border = new Border();
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = style.PreviewImge;
                border.Background = brush;

                TextBlock tb = new TextBlock();
                tb.Text = style.NameID;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.Foreground = new SolidColorBrush(Colors.White);

                border.Child = tb;
                styleBtn.Content = border;

                styleSP.Children.Add(styleBtn);
            }
        }

        private void ConfigLang()
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            editTB.Text = loader.GetString("EditHeader");
            saveTB.Text = loader.GetString("SaveAsCopyBtn");
            cancelTB.Text = loader.GetString("CancleBtn");
            styleTB.Text = loader.GetString("StyleHeader");
            familyTB.Text = loader.GetString("FontFamilyHeader");
            colorTB.Text = loader.GetString("FontColorHeader");
            TextView.PlaceholderText = loader.GetString("FontPlaceHolderText");
            savedTB.Text = loader.GetString("PhotoSavedHint");
            savingTB.Text = loader.GetString("SavingHint");
            shareTB.Text = loader.GetString("ShareHint");
            backhomeTB.Text = loader.GetString("BackToHomeHint");
            retryTB.Text = loader.GetString("ChangeToCompressHint");
            errorHintTB.Text = loader.GetString("CompressHint");
            backTB.Text = loader.GetString("BackErrorHint");
        }

        #region FUNCTION
        private void IncreaseFontsizeClick(object sender,RoutedEventArgs e)
        {
            if(textGrid.Visibility==Visibility.Visible)
            {
                if (TextView.FontSize < 100) TextView.FontSize += 5;
            }
            else if(styleGrid.Visibility==Visibility.Visible)
            {
                styleGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;
            }
        }

        private void DecreaseFontsizeClick(object sender, RoutedEventArgs e)
        {
            if (textGrid.Visibility == Visibility.Visible)
            {
                if (TextView.FontSize > 10) TextView.FontSize -= 5;
            }
            else if (styleGrid.Visibility == Visibility.Visible)
            {
                styleGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
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

        private void FamilyClick(object sender,RoutedEventArgs e)
        {
            if(_isInFontFamilyMode)
            {
                FamilyOutStory.Begin();
                _isInFontFamilyMode = false;
            }
            else
            {
                FamilyInStory.Begin();
                _isInFontFamilyMode = true;
            }
        }

        private void MovieClick(object sender,RoutedEventArgs e)
        {
            if(_isInStyleMode)
            {
                MovieOutStory.Begin();
                _isInStyleMode = false;
            }
            else
            {
                MovieInStory.Begin();
                _isInStyleMode = true;
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

        private void FontFamilyClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var textblock = btn.Content as TextBlock;
            TextView.FontFamily = textblock.FontFamily;
        }

        private void StyleClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var border = btn.Content as Border;
            var tb = border.Child as TextBlock;
            var text = tb.Text;

            familyBtn.Visibility = colorBtn.Visibility= Visibility.Collapsed;

            if(text=="Plain")
            {
                textGrid.Visibility = Visibility.Visible;
                styleGrid.Visibility = Visibility.Collapsed;

                familyBtn.Visibility = colorBtn.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region TEXT_MANI
        private void TextView_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid.ManipulationDelta -= TextView_ManipulationDelta;
            textGrid.ManipulationDelta += TextView_ManipulationDelta;

            styleGrid.ManipulationDelta -= TextView_ManipulationDelta;
            styleGrid.ManipulationDelta += TextView_ManipulationDelta;

            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_translateTransform);
            _transformGroup.Children.Add(_scaleTransform);

            styleGrid.RenderTransform = _transformGroup;
        }

        private void TextView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _translateTransform.X += e.Delta.Translation.X;
            _translateTransform.Y += e.Delta.Translation.Y;
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
            if(TextView.Text==string.Empty)
            {
                TextView.Visibility = Visibility.Collapsed;
            }
            SaveImage(renderGrid);
        }

        private async void ShowImage(StorageFile file)
        {
            try
            {
                TextView.Visibility = Visibility.Collapsed;

                using(var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    //从文件流里创建解码器
                    var decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this._dpiX = (int)decoder.DpiX;
                    this._dpiY = (int)decoder.DpiY;
                    this._height = decoder.OrientedPixelHeight;
                    this._width = decoder.OrientedPixelWidth;

                    ring.IsActive = true;

                    //显示图像
                    var bitmap = new BitmapImage();
                    var task = bitmap.SetSourceAsync(fileStream);
                    await task;
                    image.Source = bitmap;

                    ring.IsActive = false;

                    TextView.Visibility = Visibility.Visible;

                    image.UpdateLayout();
                    //this._imageBorderH = (image.ActualHeight / 2);
                    //this._imageBorderW = (image.ActualWidth / 2);
                }
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


                if(LocalSettingHelper.GetValue("QualityCompress")=="0")
                {
                    ImageHandleHelper.CompressImageSize(ref targetWidth, ref targetHeight);
                }

                var positon = LocalSettingHelper.GetValue("Position");
                StorageFile fileToSave = null;
                switch(positon)
                {
                    case "0": fileToSave = await KnownFolders.SavedPictures.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName); break;
                    case "1":
                        {
                            var folderToSave = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                            fileToSave = await folderToSave.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);
                        };break;
                    case "2":
                        {
                            fileToSave = await KnownFolders.CameraRoll.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);
                        };break;
                    default:
                        {
                            var folderToSave = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                            fileToSave = await folderToSave.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);
                        };break;
                }
                if (fileToSave == null) return;

               
                //CachedFileManager.DeferUpdates(fileToSave);
                using (IRandomAccessStream fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite), 
                    memStream = new InMemoryRandomAccessStream())
                {
                    
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(elementToRender, (int)(targetWidth), (int)(targetHeight));

                    var pixels = await bitmap.GetPixelsAsync();

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, memStream);
                    encoder.BitmapTransform.ScaledHeight = (uint)(targetHeight);
                    encoder.BitmapTransform.ScaledWidth = (uint)(targetWidth);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
                    await encoder.FlushAsync();

                    memStream.Seek(0);
                    fileStream.Seek(0);
                    fileStream.Size = 0;
                    await RandomAccessStream.CopyAsync(memStream, fileStream);
                }
                //await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                MaskGrid.Visibility = Visibility.Collapsed;
                ShareGrid.Visibility = Visibility.Visible;

                this._savedFileName = fileToSave.Name;

                if (_isFromShareTarget) shareBtn.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                MaskGrid.Visibility = Visibility.Collapsed;
                ErrorGrid.Visibility = Visibility.Visible;
                _isInErrorMode = true;

                //new MessageDialog(e.Message).ShowAsync();
            }
        }

        private async void CancelClick(object sender,RoutedEventArgs e)
        {
            var loader=Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var title = loader.GetString("DiscardTitle");
            var content = loader.GetString("DiscardContent");
            var discardBtn = loader.GetString("DiscardOK");
            var discardCancel = loader.GetString("DiscardCancel");

            MessageDialog md = new MessageDialog(content, title);

            var rootFrame = Window.Current.Content as Frame;

            md.Commands.Add(new UICommand(discardBtn, act =>
            {
                if (_isFromShareTarget) App.Current.Exit();
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }));
            md.Commands.Add(new UICommand(discardCancel, act =>
            {
                return;
            }));
            
            await md.ShowAsync();
        }

        private void backErrorClick(object sender,RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Collapsed;
            _isInErrorMode = false;
        }

        private void ShareClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ee)
            {
                var task = ExceptionHelper.WriteRecord(ee);
            }

        }

        private async void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "MyerMoment";
            request.Data.Properties.Description = "image From MyerMoment";
            DataRequestDeferral deferral = request.GetDeferral();

            try
            {
                var positon = LocalSettingHelper.GetValue("Position");
                StorageFile fileToGet = null;
                switch (positon)
                {
                    case "0": fileToGet = await KnownFolders.SavedPictures.GetFileAsync(_savedFileName); break;
                    case "1":
                        {
                            var folderToGet = await KnownFolders.PicturesLibrary.GetFolderAsync("MyerMoment");
                            fileToGet = await folderToGet.GetFileAsync(_savedFileName);
                        }; break;
                    case "2":
                        {
                            fileToGet = await KnownFolders.CameraRoll.GetFileAsync(_savedFileName);
                        }; break;
                }
                if (fileToGet == null) return;

                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(fileToGet);
                request.Data.SetStorageItems(storageItems);
            }
            catch (Exception)
            {
                ShareGrid.Visibility = Visibility.Collapsed;
                _isInShareMode = false;

                ErrorGrid.Visibility = Visibility.Visible;
                _isInErrorMode = true;

            }
            finally
            {
                deferral.Complete();
            }
        }

        private void BackHomeClick(object sender, RoutedEventArgs e)
        {
            if (_isFromShareTarget) App.Current.Exit();
            Frame.Navigate(typeof(MainPage));
        }

        private void retryClick(object sender,RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Collapsed;
            _isInErrorMode = false;

            LocalSettingHelper.AddValue("QualityCompress", "0");

            SaveClick(null, null);
        }

        #endregion

        #region NAVIGATE
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            if(e.Parameter!=null)
            {
                if (e.Parameter.GetType() == typeof(FileOpenPickerContinuationEventArgs))
                {
                    FileOpenPickerContinuationEventArgs args = e.Parameter as FileOpenPickerContinuationEventArgs;
                    if (args.Files.Count == 0) return;

                    ShowImage(args.Files[0]);
                }
                if (e.Parameter.GetType() == typeof(ShareOperation))
                {
                    var shareOperation = e.Parameter as ShareOperation;
                    var items=await shareOperation.Data.GetStorageItemsAsync();
                   
                    var firstItem = items.FirstOrDefault();
                    if(firstItem!=null)
                    {
                        var path = firstItem.Path;
                        var fileToOpen = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
                        ShowImage(fileToOpen);

                        _isFromShareTarget = true;
                    }
                    
                }  
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (_isInFontFamilyMode || _isInColorMode || _isInStyleMode || _isInShareMode || _isInErrorMode)
            {
                if (_isInColorMode)
                {
                    ColorOutStory.Begin();
                    _isInColorMode = false;
                }
                if(_isInFontFamilyMode)
                {
                    FamilyOutStory.Begin();
                    _isInFontFamilyMode = false;
                }
                if (_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }
                if (_isInShareMode)
                {
                    ShareGrid.Visibility = Visibility.Collapsed;
                    return;
                }
                if(_isInErrorMode)
                {
                    backErrorClick(null, null);
                }
                return;
            }
            CancelClick(null, null);
        }
        #endregion

    }
}

