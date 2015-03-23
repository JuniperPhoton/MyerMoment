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

        private bool _isInStyleMode = false;
        private bool _isInShareMode = false;
        private bool _isInErrorMode = false;
        private bool _isInMoreLineMode = false;
        private bool _isInEditMode = false;

        private bool _isFromShareTarget = false;

        private double _angle = 0;

        private uint _height;
        private uint _width;
        private int _dpiX;
        private int _dpiY;
        private Guid _encoderID;

        private TextBox _currentTextBox = null;

        private TranslateTransform _translateTransform1 = new TranslateTransform();
        private ScaleTransform _scaleTransform1 = new ScaleTransform();
        private TransformGroup _transformGroup1 = new TransformGroup();

        private TranslateTransform _translateTransform2 = new TranslateTransform();
        private ScaleTransform _scaleTransform2 = new ScaleTransform();
        private TransformGroup _transformGroup2 = new TransformGroup();
        
        private TranslateTransform _translateTransform3 = new TranslateTransform();
        private ScaleTransform _scaleTransform3 = new ScaleTransform();
        private TransformGroup _transformGroup3 = new TransformGroup();

        private TranslateTransform _translateTransformStyle = new TranslateTransform();
        private ScaleTransform _scaleTransformStyle = new ScaleTransform();
        private TransformGroup _transformGroupStyle = new TransformGroup();

        private MomentStyleList styleList;

        public ImageHandlePage()
        {
            this.InitializeComponent();
   
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            ConfigLang();

            DataTransferManager.GetForCurrentView().DataRequested += dataTransferManager_DataRequested;

            styleList = new MomentStyleList();
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

                        textGrid1.Visibility = Visibility.Collapsed;
                        textGrid2.Visibility = Visibility.Collapsed;
                        textGrid3.Visibility = Visibility.Collapsed;
                        MoreLineBtn.Visibility = Visibility.Collapsed;
                        styleImage.Visibility = Visibility.Visible;
                        increaseStyleBtn.Visibility = Visibility.Visible;
                        decreaseStyleBtn.Visibility = Visibility.Visible;
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

            _transformGroupStyle.Children.Add(_translateTransformStyle);
            _transformGroupStyle.Children.Add(_scaleTransformStyle);
            styleImage.RenderTransform = _transformGroupStyle;

           
        }

        private void ConfigLang()
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            editTB.Text = loader.GetString("EditHeader");
            saveTB.Text = loader.GetString("SaveAsCopyBtn");
            cancelTB.Text = loader.GetString("CancleBtn");
            TextView1.PlaceholderText = loader.GetString("FontPlaceHolderText");
            TextView2.PlaceholderText = loader.GetString("FontPlaceHolderText");
            TextView3.PlaceholderText = loader.GetString("FontPlaceHolderText");
            savedTB.Text = loader.GetString("PhotoSavedHint");
            savingTB.Text = loader.GetString("SavingHint");
            shareTB.Text = loader.GetString("ShareHint");
            backhomeTB.Text = loader.GetString("BackToHomeHint");
            retryTB.Text = loader.GetString("ChangeToCompressHint");
            errorHintTB.Text = loader.GetString("CompressHint");
            backTB.Text = loader.GetString("BackErrorHint");
        }

        private void UpdatePageLayout()
        {
            if (Frame.ActualHeight < 720)
            {
                contentGrid.Height = 260;
                contentSV.Height = 200;
            }
        }

        #region FUNCTION

        private void IncreaseFontsizeClick(object sender,RoutedEventArgs e)
        {
            if(_currentTextBox!=null)
            {
                if (_currentTextBox.FontSize < 100) _currentTextBox.FontSize += 5;
            }
        }

        private void DecreaseFontsizeClick(object sender, RoutedEventArgs e)
        {
            if (_currentTextBox != null)
            {
                if (_currentTextBox.FontSize >10 ) _currentTextBox.FontSize -= 5;
            }
        }

        private void IncreaseStyleClick(object sender,RoutedEventArgs e)
        {
            _scaleTransformStyle.ScaleX += 0.2;
            _scaleTransformStyle.ScaleY += 0.2;
        }

        private void DecreaseStyleClick(object sender, RoutedEventArgs e)
        {
            _scaleTransformStyle.ScaleX -= 0.2;
            _scaleTransformStyle.ScaleY -= 0.2;
        }

        private void fontLineClick(object sender, RoutedEventArgs e)
        {
            if(!_isInMoreLineMode)
            {
                MoreLineInStory.Begin();
                _isInMoreLineMode = true;

                if(textGrid1.Visibility==Visibility.Visible)
                {
                    VisualStateManager.GoToState(defaultLineBtn, "Using", false);
                }
               
            }
            else
            {
                MoreLineOutStory.Begin();
                _isInMoreLineMode = false;
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
            if(border!=null && _currentTextBox!=null)
            {
                _currentTextBox.Foreground = border.Background;
            }
        }

        private void FontFamilyClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var textblock = btn.Content as TextBlock;
            if(textblock!=null && _currentTextBox!=null)
            {
                _currentTextBox.FontFamily = textblock.FontFamily;
            }
            
        }

        private void StyleClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var border = btn.Content as Border;
            var tb = border.Child as TextBlock;
            var text = tb.Text;

            if(text=="Plain")
            {
                textGrid1.Visibility = Visibility.Visible;
                MoreLineBtn.Visibility = Visibility.Visible;
                styleImage.Visibility = Visibility.Collapsed;

                increaseStyleBtn.Visibility = Visibility.Collapsed;
                decreaseStyleBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void LineClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            if(btn!=null)
            {
                var border = btn.Content as Border;
                var tb = border.Child as TextBlock;
                var tbText = tb.Text;

                var textGrid1 = this.FindName("textGrid" + tbText) as Grid;
                if (textGrid1.Visibility == Visibility.Collapsed)
                {
                    textGrid1.Visibility = Visibility.Visible;
                    VisualStateManager.GoToState(btn, "Using", false);
                }
                else
                {
                    textGrid1.Visibility = Visibility.Collapsed;
                    VisualStateManager.GoToState(btn, "NotUsing", false);
                }
                
            }
        }

        private void conentChanged(object sender,TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if(_currentTextBox!=null)
            {
                _currentTextBox.Text = tb.Text;
            }
        }

        #endregion

        #region TEXT_MANI
        private void TextView1_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid1.ManipulationDelta -= TextView1_ManipulationDelta;
            textGrid1.ManipulationDelta += TextView1_ManipulationDelta;

            _transformGroup1 = new TransformGroup();
            _transformGroup1.Children.Add(_translateTransform1);
            _transformGroup1.Children.Add(_scaleTransform1);

            textGrid1.RenderTransform = _transformGroup1;
        }

        private void TextView1_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _translateTransform1.X += e.Delta.Translation.X;
            _translateTransform1.Y += e.Delta.Translation.Y;
        }

        private void textGrid1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(!_isInEditMode)
            {
                EditInStory.Begin();
                _isInEditMode = true;
            }
            _currentTextBox = TextView1;
            contentTB.Text = TextView1.Text;
        }

        private void TextView2_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid2.ManipulationDelta -= TextView2_ManipulationDelta;
            textGrid2.ManipulationDelta += TextView2_ManipulationDelta;

            _transformGroup2 = new TransformGroup();
            _transformGroup2.Children.Add(_translateTransform2);
            _transformGroup2.Children.Add(_scaleTransform2);

            textGrid2.RenderTransform = _transformGroup2;
        }

        private void TextView2_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _translateTransform2.X += e.Delta.Translation.X;
            _translateTransform2.Y += e.Delta.Translation.Y;
        }

        private void textGrid2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isInEditMode)
            {
                EditInStory.Begin();
                _isInEditMode = true;
            }
            _currentTextBox = TextView2;
            contentTB.Text = TextView2.Text;
        }

        private void TextView3_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid3.ManipulationDelta -= TextView3_ManipulationDelta;
            textGrid3.ManipulationDelta += TextView3_ManipulationDelta;

            _transformGroup3 = new TransformGroup();
            _transformGroup3.Children.Add(_translateTransform3);
            _transformGroup3.Children.Add(_scaleTransform3);

            textGrid3.RenderTransform = _transformGroup3;
        }

        private void TextView3_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _translateTransform3.X += e.Delta.Translation.X;
            _translateTransform3.Y += e.Delta.Translation.Y;
        }

        private void textGrid3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isInEditMode)
            {
                EditInStory.Begin();
                _isInEditMode = true;
            }
            _currentTextBox = TextView3;
            contentTB.Text = TextView3.Text;
        }

        private void StyleView_OnPointerEntered(object sender,PointerRoutedEventArgs e)
        {
            styleImage.RenderTransform = _transformGroupStyle;
            styleImage.ManipulationDelta -= StyleView_ManipulationDelta;
            styleImage.ManipulationDelta += StyleView_ManipulationDelta;
        }

        private void StyleView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _translateTransformStyle.X += e.Delta.Translation.X;
            _translateTransformStyle.Y += e.Delta.Translation.Y;
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
            if(TextView1.Text==string.Empty)
            {
                TextView1.Visibility = Visibility.Collapsed;
            }
            SaveImage();
        }

        private async void ShowImage(StorageFile file)
        {
            try
            {
                TextView1.Visibility = Visibility.Collapsed;

                using(var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    //从文件流里创建解码器
                    var decoder = await BitmapDecoder.CreateAsync(fileStream);

                    //获取目前图像的信息
                    this._dpiX = (int)decoder.DpiX;
                    this._dpiY = (int)decoder.DpiY;
                    this._height = decoder.OrientedPixelHeight;
                    this._width = decoder.OrientedPixelWidth;

                    switch (file.FileType)
                    {
                        case ".jpg": _encoderID = Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId; break;
                        case ".png": _encoderID = BitmapEncoder.PngEncoderId; break;
                    }

                    ring.IsActive = true;

                    //显示图像
                    var bitmap = new BitmapImage();
                    var task = bitmap.SetSourceAsync(fileStream);
                    await task;
                    image.Source = bitmap;

                    ring.IsActive = false;

                    TextView1.Visibility = Visibility.Visible;

                    fileStream.Dispose();
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
            }
        }

        private async void SaveImage()
        {
            try
            {
                MaskGrid.Visibility = Visibility.Visible;

                uint targetWidth = this._width;
                uint targetHeight = this._height;

                //压缩图像
                if(LocalSettingHelper.GetValue("QualityCompress")=="0")
                {
                    ImageHandleHelper.CompressImageSize(ref targetWidth, ref targetHeight);
                }

                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(renderGrid, (int)(targetWidth), (int)(targetHeight));

                var pixels = await bitmap.GetPixelsAsync();

                //处理保存的位置
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

                this._savedFileName = fileToSave.Name;

                
                //CachedFileManager.DeferUpdates(fileToSave);
                using (IRandomAccessStream fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var propertySet = new Windows.Graphics.Imaging.BitmapPropertySet();
                    var qualityValue = new Windows.Graphics.Imaging.BitmapTypedValue(
                        1.0, // Maximum quality
                        Windows.Foundation.PropertyType.Single
                        );

                    propertySet.Add("ImageQuality", qualityValue);


                    var encoder = await BitmapEncoder.CreateAsync(_encoderID, fileStream, propertySet);
                    encoder.BitmapTransform.ScaledHeight = targetHeight;
                    encoder.BitmapTransform.ScaledWidth = targetWidth;

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }
                //await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                MaskGrid.Visibility = Visibility.Collapsed;
                ShareGrid.Visibility = Visibility.Visible;

                if (_isFromShareTarget) shareBtn.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                MaskGrid.Visibility = Visibility.Collapsed;
                ErrorGrid.Visibility = Visibility.Visible;
                _isInErrorMode = true;

                //DeleteCorpurrtedFile();
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

        private void ShareToWechatClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //int scene = SendMessageToWX.Req.WXSceneChooseByUser;

                //WXTextMessage message = new WXTextMessage();
                //message.Title = "文本";
                //message.Text = "这是一段文本内容";
                //SendMessageToWX.Req req = new SendMessageToWX.Req(message, scene);
                //IWXAPI api = WXAPIFactory.CreateWXAPI("wx3b34a14b66641dfd");
                //api.SendReq(req);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
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

        private async Task DeleteCorpurrtedFile()
        {
            try
            {
                var positon = LocalSettingHelper.GetValue("Position");
                StorageFile fileToDelete = null;
                switch (positon)
                {
                    case "0": fileToDelete = await KnownFolders.SavedPictures.GetFileAsync(_savedFileName); break;
                    case "1":
                        {
                            var folderToSave = await Windows.Storage.KnownFolders.PicturesLibrary.GetFolderAsync("MyerMoment");
                            fileToDelete = await folderToSave.GetFileAsync(_savedFileName);
                        }; break;
                    case "2":
                        {
                            fileToDelete = await KnownFolders.CameraRoll.GetFileAsync(_savedFileName);
                        }; break;
                }
                if (fileToDelete == null) return;
                await fileToDelete.DeleteAsync();
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
            }
            
               
        }

        #endregion

        #region NAVIGATE
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            UpdatePageLayout();

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
            if (_isInStyleMode || _isInShareMode || _isInErrorMode || _isInMoreLineMode || _isInEditMode)
            {
              
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
                if(_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                if(_isInEditMode)
                {
                    EditOutStory.Begin();
                    _isInEditMode = false;
                    _currentTextBox = null;
                    contentTB.Text = "";
                }
                return;
            }
            CancelClick(null, null);
        }
        #endregion

    }
}

