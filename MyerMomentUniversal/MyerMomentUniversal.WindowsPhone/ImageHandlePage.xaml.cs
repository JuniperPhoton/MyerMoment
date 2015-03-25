using ChaoFunctionRT;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


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

        private bool _isConfigStyle = false;

        private double _angle = 0;

        private uint _height;
        private uint _width;
        private int _dpiX;
        private int _dpiY;
        private Guid _encoderID;
        private string fileName;
        private int scaleLong;
        private BitmapAlphaMode alphaMode;
        private BitmapPixelFormat pixelFormat;

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

            _transformGroupStyle.Children.Add(_translateTransformStyle);
            _transformGroupStyle.Children.Add(_scaleTransformStyle);
            styleImage.RenderTransform = _transformGroupStyle;

            ConfigQuality();
            ConfigStyle();
            
        }

        #region CONFIGURATION
        private void ConfigQuality()
        {
            var qualitySetting = LocalSettingHelper.GetValue("QualityCompress");
            if (qualitySetting == "0")
            {
                scaleLong = 1500;

                Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                var firmwareVersion = deviceInfo.SystemFirmwareVersion;

                if (deviceInfo.SystemProductName.Contains("RM-875") 
                    || deviceInfo.SystemProductName.Contains("RM-876") 
                    || deviceInfo.SystemProductName.Contains("RM-877"))
                {
                    scaleLong = 1300;
                }
                
            }
        }

        private void ConfigStyle()
        {
            styleList = new MomentStyleList();
            foreach (var style in styleList.Styles)
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
        }

        /// <summary>
        /// 配置语言
        /// </summary>
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

        /// <summary>
        /// 根据当前分辨率决定ContentGrid的高度
        /// </summary>
        private void UpdatePageLayout()
        {
            if (Frame.ActualHeight < 720)
            {
                contentGrid.Height = 260;
                contentSV.Height = 200;
            }
        }

        #endregion

        #region FUNCTION

        private void RotateClick(object sender, RoutedEventArgs e)
        {
            _angle += 90;

            var finalAngle = _angle;
            startAngle.Value = finalAngle - 90;
            endAngle.Value = finalAngle;

            var temp = _height;
            _height = _width;
            _width = temp;

            RotateStory.Begin();
        }
        /// <summary>
        /// 增加文字大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseFontsizeClick(object sender,RoutedEventArgs e)
        {
            if(_currentTextBox!=null)
            {
                if (_currentTextBox.FontSize < 100) _currentTextBox.FontSize += 5;
            }
        }

        /// <summary>
        /// 减少文字大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecreaseFontsizeClick(object sender, RoutedEventArgs e)
        {
            if (_currentTextBox != null)
            {
                if (_currentTextBox.FontSize >10 ) _currentTextBox.FontSize -= 5;
            }
        }

        /// <summary>
        /// 增加样式大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseStyleClick(object sender,RoutedEventArgs e)
        {
            _scaleTransformStyle.ScaleX += 0.2;
            _scaleTransformStyle.ScaleY += 0.2;
        }

        /// <summary>
        /// 减少样式大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecreaseStyleClick(object sender, RoutedEventArgs e)
        {
            _scaleTransformStyle.ScaleX -= 0.2;
            _scaleTransformStyle.ScaleY -= 0.2;
        }

        /// <summary>
        /// 进入多行管理模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 进入样式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToStyleClick(object sender,RoutedEventArgs e)
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

        /// <summary>
        /// 改变文字颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontColorClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var border = btn.Content as Border;
            if(border!=null && _currentTextBox!=null)
            {
                _currentTextBox.Foreground = border.Background;
            }
        }

        /// <summary>
        /// 改变文字字体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontFamilyClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var textblock = btn.Content as TextBlock;
            if(textblock!=null && _currentTextBox!=null)
            {
                _currentTextBox.FontFamily = textblock.FontFamily;
            }
            
        }

        /// <summary>
        /// 改变文字样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 添加/删除新的文段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            if(btn!=null)
            {
                var border = btn.Content as Border;
                var tb = border.Child as TextBlock;
                var tbText = tb.Text;

                var currentTextGrid = this.FindName("textGrid" + tbText) as Grid;
                if (currentTextGrid.Visibility == Visibility.Collapsed)
                {
                    currentTextGrid.Visibility = Visibility.Visible;
                    VisualStateManager.GoToState(btn, "Using", false);
                }
                else
                {
                    currentTextGrid.Visibility = Visibility.Collapsed;
                    VisualStateManager.GoToState(btn, "NotUsing", false);
                }
                
            }
        }

        /// <summary>
        /// TextBox 改变的时候改变文段内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conentChanged(object sender,TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if(_currentTextBox!=null)
            {
                _currentTextBox.Text = tb.Text;
            }
        }

        #endregion

        #region TEXT_MANI 关于所有手势操作

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
                if(_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                else if(_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }
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

        #region DISPLAY AND SAVE
        
        private void SaveClick(object sender,RoutedEventArgs e)
        {
            if(TextView1.Text==string.Empty)
            {
                TextView1.Visibility = Visibility.Collapsed;
            }
            if (TextView2.Text == string.Empty)
            {
                TextView2.Visibility = Visibility.Collapsed;
            }
            if (TextView3.Text == string.Empty)
            {
                TextView3.Visibility = Visibility.Collapsed;
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
                    this.fileName = file.Name;
                    this.pixelFormat = decoder.BitmapPixelFormat;
                    this.alphaMode = decoder.BitmapAlphaMode;

                    switch (file.FileType)
                    {
                        case ".jpg": _encoderID = BitmapEncoder.JpegEncoderId; break;
                        case ".png": _encoderID = BitmapEncoder.PngEncoderId; break;
                    }

                    ring.IsActive = true;

                    //显示图像
                    var bitmap = new BitmapImage();
                    await  bitmap.SetSourceAsync(fileStream);
                    image.Source = bitmap;
                }
                ring.IsActive = false;

                TextView1.Visibility = Visibility.Visible;
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
                if (LocalSettingHelper.GetValue("QualityCompress") == "0")
                {
                    var imagehelper = new ImageHandleHelper();
                    imagehelper.CompressImage((uint)scaleLong, targetWidth, targetHeight);
                    targetWidth = imagehelper.outputWidth;
                    targetHeight = imagehelper.outputHeight;
                }

                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(renderGrid, (int)(targetWidth), (int)(targetHeight));

                var pixels =await bitmap.GetPixelsAsync();

                //处理保存的位置
                var positon = LocalSettingHelper.GetValue("Position");
                StorageFile fileToSave = null;
                switch(positon)
                {
                    case "0": fileToSave = await KnownFolders.SavedPictures.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName); break;
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

                    var encoder = await BitmapEncoder.CreateAsync(_encoderID, fileStream);
                    encoder.BitmapTransform.ScaledHeight = targetHeight;
                    encoder.BitmapTransform.ScaledWidth = targetWidth;

                    encoder.SetPixelData(pixelFormat, alphaMode, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
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
                //await new MessageDialog(e.Message).ShowAsync();
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

