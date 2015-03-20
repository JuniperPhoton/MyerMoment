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


namespace MyerMomentUniversal
{

    public sealed partial class ImageHandlePage : Page
    {
        private string savedFileName = "";

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

        private double _qualityScale;

        //private double _imageBorderH;
        //private double _imageBorderW;

        private TranslateTransform _translateTransform = new TranslateTransform();
        private ScaleTransform _scaleTransform = new ScaleTransform();
        private TransformGroup _transformGroup = new TransformGroup();

        public ImageHandlePage()
        {
            this.InitializeComponent();
   
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            ConfigLang();

            DataTransferManager.GetForCurrentView().DataRequested += dataTransferManager_DataRequested;

            switch(LocalSettingHelper.GetValue("Quality"))
            {
                case "0": _qualityScale = 0.7; break;
                case "1": _qualityScale = 0.9; break;
                case "2": _qualityScale = 1.0; break;
            }

            _transformGroup.Children.Add(_translateTransform);
            _transformGroup.Children.Add(_scaleTransform);

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

        }

        #region FUNCTION
        private void IncreaseFontsizeClick(object sender,RoutedEventArgs e)
        {
            if(textGrid.Visibility==Visibility.Visible)
            {
                if (TextView.FontSize < 100) TextView.FontSize += 5;
            }
            else if(foodGrid.Visibility==Visibility.Visible)
            {
                foodGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;
            }
            else if(sceneGrid.Visibility==Visibility.Visible)
            {
                sceneGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;
            }
            else if(aloneGrid.Visibility==Visibility.Visible)
            {
                aloneGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;

            }
            else if (thanksGrid.Visibility == Visibility.Visible)
            {
                thanksGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;
            }
            else if (braveGrid.Visibility == Visibility.Visible)
            {
                braveGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX += 0.2;
                _scaleTransform.ScaleY += 0.2;
            }
            else if (dinnerGrid.Visibility == Visibility.Visible)
            {
                dinnerGrid.RenderTransform = _transformGroup;
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
            else if (foodGrid.Visibility == Visibility.Visible)
            {
                foodGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
            else if (sceneGrid.Visibility == Visibility.Visible)
            {

                sceneGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
            else if (aloneGrid.Visibility == Visibility.Visible)
            {
                aloneGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
            else if (thanksGrid.Visibility == Visibility.Visible)
            {
                thanksGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
            else if (dinnerGrid.Visibility == Visibility.Visible)
            {
                dinnerGrid.RenderTransform = _transformGroup;
                _scaleTransform.ScaleX -= 0.2;
                _scaleTransform.ScaleY -= 0.2;
            }
            else if (braveGrid.Visibility == Visibility.Visible)
            {
                braveGrid.RenderTransform = _transformGroup;
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

            switch(text)
            {
                case "Plain":
                    {
                        textGrid.Visibility = Visibility.Visible;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Collapsed;
                        familyBtn.Visibility = colorBtn.Visibility= Visibility.Visible;
                    };break;
                case "Food":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Visible;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Collapsed;
                    }; break;
                case "Scene":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Visible;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Collapsed;
                    }; break;
                case "Alone":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Visible;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Collapsed;
                    }break;
                case "Thanks":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Visible;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Collapsed;
                    } break;
                case "Dinner":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Visible;
                        braveGrid.Visibility = Visibility.Collapsed;
                    } break;
                case "Brave":
                    {
                        textGrid.Visibility = Visibility.Collapsed;
                        foodGrid.Visibility = Visibility.Collapsed;
                        sceneGrid.Visibility = Visibility.Collapsed;
                        aloneGrid.Visibility = Visibility.Collapsed;
                        thanksGrid.Visibility = Visibility.Collapsed;
                        dinnerGrid.Visibility = Visibility.Collapsed;
                        braveGrid.Visibility = Visibility.Visible;
                    } break;
            }
        }

        #endregion

        #region TEXT_MANI
        private void TextView_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid.ManipulationDelta -= TextView_ManipulationDelta;
            textGrid.ManipulationDelta += TextView_ManipulationDelta;

            foodGrid.ManipulationDelta -= TextView_ManipulationDelta;
            foodGrid.ManipulationDelta += TextView_ManipulationDelta;

            sceneGrid.ManipulationDelta -= TextView_ManipulationDelta;
            sceneGrid.ManipulationDelta += TextView_ManipulationDelta;

            aloneGrid.ManipulationDelta -= TextView_ManipulationDelta;
            aloneGrid.ManipulationDelta += TextView_ManipulationDelta;

            thanksGrid.ManipulationDelta -= TextView_ManipulationDelta;
            thanksGrid.ManipulationDelta += TextView_ManipulationDelta;

            dinnerGrid.ManipulationDelta -= TextView_ManipulationDelta;
            dinnerGrid.ManipulationDelta += TextView_ManipulationDelta;

            braveGrid.ManipulationDelta -= TextView_ManipulationDelta;
            braveGrid.ManipulationDelta += TextView_ManipulationDelta;


            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_translateTransform);
            _transformGroup.Children.Add(_scaleTransform);

            textGrid.RenderTransform = _transformGroup;
            sceneGrid.RenderTransform = _transformGroup;
            foodGrid.RenderTransform = _transformGroup;
            aloneGrid.RenderTransform = _transformGroup;
            thanksGrid.RenderTransform = _transformGroup;
            dinnerGrid.RenderTransform = _transformGroup;
            braveGrid.RenderTransform = _transformGroup;
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

                CachedFileManager.DeferUpdates(fileToSave);
                using (var fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(elementToRender, (int)targetWidth, (int)targetHeight);

                    var pixels = await bitmap.GetPixelsAsync();

                    var propertySet = new BitmapPropertySet();

                    var qualityValue = new BitmapTypedValue(_qualityScale, PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream, propertySet);
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                MaskGrid.Visibility = Visibility.Collapsed;
                ShareGrid.Visibility = Visibility.Visible;

                this.savedFileName = fileToSave.Name;

                if (_isInShareMode) shareBtn.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                MaskGrid.Visibility = Visibility.Collapsed;
                ErrorGrid.Visibility = Visibility.Visible;
                _isInErrorMode = true;
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
                #region UMENG
                //UmengClient umengClient = new SinaWeiboClient("5506e8eafd98c52426000587");

                //var file = await Windows.Storage.KnownFolders.SavedPictures.GetFileAsync(savedFileName);
                //using (var fileStream = await file.OpenStreamForReadAsync())
                //{
                //    byte[] imageData = new byte[fileStream.Length];
                //    fileStream.Read(imageData, 0, imageData.Length);

                //    var pic = new UmengPicture(imageData, "Share image #MyerMoment#");
                //    var task = umengClient.SharePictureAsync(pic, true);
                    
                //    var result = await task;

                //    var resState = result.Status;
                //    switch (resState)
                //    {
                //        case ShareStatus.Success:
                //            {
                //                BackHomeClick(null, null);
                //            }; break;
                //        case ShareStatus.UserCanceled:
                //            {
                //                BackHomeClick(null, null);
                //            }; break;
                //        case ShareStatus.Error:
                //            {
                //            };break;
                //    }

                //}
                #endregion

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
                    case "0": fileToGet = await KnownFolders.SavedPictures.GetFileAsync(savedFileName); break;
                    case "1":
                        {
                            var folderToGet = await KnownFolders.PicturesLibrary.GetFolderAsync("MyerMoment");
                            fileToGet = await folderToGet.GetFileAsync(savedFileName);
                        }; break;
                    case "2":
                        {
                            fileToGet = await KnownFolders.CameraRoll.GetFileAsync(savedFileName);
                        }; break;
                }
                if (fileToGet == null) return;

                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(fileToGet);
                request.Data.SetStorageItems(storageItems);
            }
            catch (Exception ee)
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
            if (_isInShareMode) App.Current.Exit();
            Frame.Navigate(typeof(MainPage));
        }

        #endregion

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

                        _isInShareMode = true;
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
            if(_isInFontFamilyMode || _isInColorMode || _isInStyleMode || _isInShareMode || _isInErrorMode)
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
 
    }
}

