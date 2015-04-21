
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Resources;
using WeiboSDKForWinRT;
using System.Threading.Tasks;
using Lumia.Imaging;
using Lumia.Imaging.Artistic;
using JP.Utils.Debug;
using JP.Utils.Data;
using MyerMomentUniversal.ViewModel; 


namespace MyerMomentUniversal
{

    public sealed partial class ImageHandlePage : Page
    {

        private bool _isInStyleMode = false;
        private bool _isInShareMode = false;
        private bool _isInErrorMode = false;
        private bool _isInMoreLineMode = false;
        private bool _isInEditMode = false;
        private bool _isInCropMode = false;
        private bool _isInFilterMode = false;

        private bool _isFromShareTarget = false;

        private double _styleAngle = 0;
        private double _text1Angle = 0;
        private double _text2Angle = 0;
        private double _text3Angle = 0;

        private TextBox _currentTextBox = null;
        private int _currentTextViewFlag = 1;

        //用于样式和文字的手势
        private CompositeTransform _compositeTransform1 = new CompositeTransform();
        private CompositeTransform _compositeTransform2 = new CompositeTransform();
        private CompositeTransform _compositeTransform3 = new CompositeTransform();
        private CompositeTransform _compositeTransformStyle = new CompositeTransform();

        //储存图像相关的数据
        private ImageHandleHelper _imageHandleHelper;

        private SelectedRegion selectedRegion;

        //传进来的源文件
        private StorageFile sourceImageFile = null;
        private StorageFile afterCropImageFile = null;
        string tempFileName = "";

        private FilterKind currentFilter = FilterKind.Original;

        private SelectedRegionShape SelectedShape = SelectedRegionShape.Free;

        double cornerSize;
        double CornerSize
        {
            get
            {
                if (cornerSize <= 0)
                {
                    cornerSize = (double)Application.Current.Resources["Size"];
                }
                return cornerSize;
            }
        }

        private bool _isLoadImage = false;

        /// <summary>
        /// The previous points of all the pointers.
        /// </summary>
        Dictionary<uint, Point?> pointerPositionHistory = new Dictionary<uint, Point?>();

        public ImageHandlePage()
        {
            this.InitializeComponent();
   
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            _imageHandleHelper = new ImageHandleHelper();

            selectRegion.ManipulationMode = ManipulationModes.Scale |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            styleImage.RenderTransform = _compositeTransformStyle;

            selectedRegion = new SelectedRegion { MinSelectRegionSize = 2 * CornerSize };
            this.DataContext = selectedRegion;

            this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

#if WINDOWS_PHONE_APP
            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;
            BottomMessageDialog.Control.BottomMessageDialog dialog = new BottomMessageDialog.Control.BottomMessageDialog();
            dialog.ShowDialog("CANCEL", "Do you want to leave?", (s,e) =>
                {

                },
                (s2,e2) =>
                {

                });
#endif

            ConfigLang();
            ConfigQuality();
            ConfigFilter();
            ConfigStyle();
        }

        #region Configuration
        private void ConfigQuality()
        {
#if WINDOWS_PHONE_APP
            var qualitySetting = LocalSettingHelper.GetValue("QualityCompress");
            if (qualitySetting == "0")
            {
                _imageHandleHelper.ScaleLong = 1500;

                Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                var firmwareVersion = deviceInfo.SystemFirmwareVersion;

                //针对 Lumia 1020 进行配置
                if (deviceInfo.SystemProductName.Contains("RM-875") 
                    || deviceInfo.SystemProductName.Contains("RM-876") 
                    || deviceInfo.SystemProductName.Contains("RM-877"))
                {
                    _imageHandleHelper.ScaleLong = 1024;
                }
            }
#endif
        }

        //配置Style 列表
        private void ConfigStyle()
        {
            var styleList = new StylesViewModel();
            styleList.ConfigStyleListAsync();

            foreach (var style in styleList.Styles)
            {
                Button styleBtn = new Button();
                styleBtn.BorderThickness = new Thickness(0);
                styleBtn.Style = (App.Current.Resources["ButtonStyle2"] as Style);
                styleBtn.MinHeight = styleBtn.MinWidth = 10;
                styleBtn.Height = styleBtn.Width = 70;
                styleBtn.VerticalAlignment = VerticalAlignment.Top;
                styleBtn.Margin = new Thickness(10, 10, 0, 0);
                styleBtn.Click += ((sender, e) =>
                {
                    //点击Style 按钮的操作
                    styleImage.Source = style.FullSizeImage;

                    //所有Text 都隐藏
                    textGrid1.Visibility = Visibility.Collapsed;
                    textGrid2.Visibility = Visibility.Collapsed;
                    textGrid3.Visibility = Visibility.Collapsed;

                    //隐藏操作按钮
                    MoreLineBtn.Visibility = Visibility.Collapsed;
                    styleImage.Visibility = Visibility.Visible;
                    increaseStyleBtn.Visibility = Visibility.Visible;
                    decreaseStyleBtn.Visibility = Visibility.Visible;
                });

                Border border = new Border();
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = style.PreviewImge;
                border.Background = brush;

                styleBtn.Content = border;

                styleSP.Children.Add(styleBtn);
            }
        }

        //配置滤镜
        private void ConfigFilter()
        {
            int index = -1;
            var list = FilterFactory.GetFilterList();
            foreach(var filter in list)
            {
                index++;

                var btn = new Button() { Width = 70, Height = 70, BorderThickness = new Thickness(0),Margin=new Thickness(10,10,0,0), MinHeight = 10, MinWidth = 10, VerticalAlignment = VerticalAlignment.Top };
                btn.Click += this.ApplyFilterClick;
                btn.Tag = index;
                btn.Style = (App.Current.Resources["ButtonStyle2"] as Style);

                var border = new Border();
                ImageBrush brush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Asset/Filter/" + filter+".jpg"));
                brush.ImageSource = bitmap;
                border.Background = brush;

                var tb = new TextBlock() { Text = filter,VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=HorizontalAlignment.Center };
                tb.Foreground = new SolidColorBrush(Colors.White);
                border.Child = tb;
                btn.Content = border;

                filterSP.Children.Add(btn);
            }
        }

        /// <summary>
        /// 配置语言
        /// </summary>
        private void ConfigLang()
        {
            var loader = ResourceLoader.GetForCurrentView();
            //editTB.Text = loader.GetString("EditHeader");
            saveTB.Text = loader.GetString("SaveAsCopyBtn");
            cancelTB.Text = loader.GetString("CancleBtn");
            cancelTB2.Text = loader.GetString("CancleBtn");
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
            contentTB.PlaceholderText = loader.GetString("FontPlaceHolderText");
            nextTB.Text = loader.GetString("ContinueBtn");
        }

        /// <summary>
        /// 根据当前分辨率决定ContentGrid的高度
        /// </summary>
        private void UpdatePageLayout()
        {
            #if WINDOWS_PHONE_APP
            styleSV.Height = filterSV.Height = 120;

            if (Frame.ActualHeight < 720)
            {
                contentGrid.Height = 260;
                contentSV.Height = 200;
                
            }
            #elif WINDOWS_APP
            contentGrid.Height = 320;
            contentSV.Height = 300;
            shareBtn.MaxWidth = 300;
            //systemBtn.MaxWidth = 300;
            backHomeBtn.MaxWidth = 300;
            familySV.Height = familySV2.Height = 60;
            //styleSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            //filterSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            #endif
        }

#endregion

        #region Function
        private void TapBlack(object sender, TappedRoutedEventArgs e)
        {
            HandleBack();
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
            _compositeTransformStyle.ScaleX += 0.2;
            _compositeTransformStyle.ScaleY += 0.2;
            
            //new MessageDialog(_scaleTransformStyle.ScaleX + ", y" + _scaleTransformStyle.ScaleY).ShowAsync();
        }

        /// <summary>
        /// 减少样式大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecreaseStyleClick(object sender, RoutedEventArgs e)
        {
            _compositeTransformStyle.ScaleX -= 0.2;
            _compositeTransformStyle.ScaleY -= 0.2;
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

        private void ToFilterClick(object sender,RoutedEventArgs e)
        {
            if(_isInFilterMode)
            {
                FilterOutStory.Begin();
                _isInFilterMode = false;
            }
            else
            {
                FilterInStory.Begin();
                _isInFilterMode = true;
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

            if (text =="Custom"|| text=="自定义")
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

        private void TapMask(object sender,TappedRoutedEventArgs e)
        {
            HandleBack();
        }

        private void cropClick(object sender, RoutedEventArgs e)
        {
            this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.imageCanvas.Height = image.ActualHeight;
            this.imageCanvas.Width = image.ActualWidth;
            this.selectedRegion.OuterRect = new Rect(0, 0, image.ActualWidth, image.ActualHeight);

            this.selectedRegion.ResetCorner(0, 0, image.ActualWidth, image.ActualHeight);

            this.SelectedShape = SelectedRegionShape.Free;

            CropInStory.Begin();
            _isInCropMode = true;
        }
        #endregion

        #region Crop Command

        private void SetToSquareClick(object sender, RoutedEventArgs e)
        {
            if (SelectedShape == SelectedRegionShape.Square || SelectedShape == SelectedRegionShape.Free)
            {
                this.selectedRegion.ResetCorner(0, 0, image.ActualWidth, image.ActualHeight);
            }

            var topLeftX = Canvas.GetLeft(topLeftCorner);
            var bottomLeftX = Canvas.GetLeft(bottomLeftCorner);

            var topRightX = Canvas.GetLeft(topRightCorner);
            var bottomRightX = Canvas.GetLeft(bottomRightCorner);

            var topLeftY = Canvas.GetTop(topLeftCorner);
            var bottomLeftY = Canvas.GetTop(bottomLeftCorner);

            var topRightY = Canvas.GetTop(topRightCorner);
            var bottomRightY = Canvas.GetTop(bottomRightCorner);

            //the photo is in wide
            if (image.ActualWidth > image.ActualHeight)
            {
                var widthChanged = -(topRightX - (topLeftX + image.ActualHeight));

                this.SelectedShape = SelectedRegionShape.Square;
                selectedRegion.UpdateCorner((string)topRightCorner.Tag, widthChanged, 0);

            }
            else if(image.ActualHeight>image.ActualWidth)
            {
                var heightChanged = -(bottomRightY - (topLeftY + image.ActualWidth));
                this.SelectedShape = SelectedRegionShape.Square;
                selectedRegion.UpdateCorner((string)bottomRightCorner.Tag, 0, heightChanged);
            }
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            this.selectedRegion.ResetCorner(0, 0, image.ActualWidth, image.ActualHeight);
            this.SelectedShape = SelectedRegionShape.Free;
        }

        private void ChangeCanvasSize(Size previousSize,Size currentSize)
        {
            this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.imageCanvas.Height = currentSize.Height;
            this.imageCanvas.Width = currentSize.Width;
            this.selectedRegion.OuterRect = new Rect(0, 0, currentSize.Width, currentSize.Height);

            this.selectedRegion.ResetCorner(0, 0, currentSize.Width, currentSize.Height);
        }

        void sourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isLoadImage)
            {
                _isLoadImage = true;
                return;
            }
            if (e.PreviousSize.Height <= 0 || e.PreviousSize.Width <= 0)
            {
                return;
            }
            if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
            {
                this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.selectedRegion.OuterRect = Rect.Empty;
                this.selectedRegion.ResetCorner(0, 0, 0, 0);
            }
            else
            {
                this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Visible;

                this.imageCanvas.Height = e.NewSize.Height;
                this.imageCanvas.Width = e.NewSize.Width;
                this.selectedRegion.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

                if (e.PreviousSize.IsEmpty || double.IsNaN(e.PreviousSize.Height) || e.PreviousSize.Height <= 0)
                {
                    this.selectedRegion.ResetCorner(0, 0, e.NewSize.Width, e.NewSize.Height);
                }
                else
                {
                    double scale = e.NewSize.Height / e.PreviousSize.Height;
                    this.selectedRegion.ResizeSelectedRect(scale);
                }
            }
        }

        #endregion

        #region Event Method
        private void AddCornerEvents(Control corner)
        {
            corner.PointerPressed += Corner_PointerPressed;
            corner.PointerMoved += Corner_PointerMoved;
            corner.PointerReleased += Corner_PointerReleased;
        }

        private void RemoveCornerEvents(Control corner)
        {
            corner.PointerPressed -= Corner_PointerPressed;
            corner.PointerMoved -= Corner_PointerMoved;
            corner.PointerReleased -= Corner_PointerReleased;
        }
        #endregion

        #region Select Region methods

        /// <summary>
        /// If a pointer presses in the corner, it means that the user starts to move the corner.
        /// 1. Capture the pointer, so that the UIElement can get the Pointer events (PointerMoved,
        ///    PointerReleased...) even the pointer is outside of the UIElement.
        /// 2. Record the start position of the move.
        /// </summary>
        private void Corner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            (sender as UIElement).CapturePointer(e.Pointer);

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);

            // Record the start point of the pointer.
            pointerPositionHistory[pt.PointerId] = pt.Position;

            e.Handled = true;
        }

        /// <summary>
        /// If a pointer which is captured by the corner moves，the select region will be updated.
        /// </summary>
        void Corner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);
            uint ptrId = pt.PointerId;

            if (pointerPositionHistory.ContainsKey(ptrId) && pointerPositionHistory[ptrId].HasValue)
            {
                Point currentPosition = pt.Position;
                Point previousPosition = pointerPositionHistory[ptrId].Value;

                double xUpdate = currentPosition.X - previousPosition.X;
                double yUpdate = currentPosition.Y - previousPosition.Y;

                if (SelectedShape == SelectedRegionShape.Free)
                {
                    this.selectedRegion.UpdateCorner((sender as ContentControl).Tag as string, xUpdate, yUpdate);
                    pointerPositionHistory[ptrId] = currentPosition;
                }

                pointerPositionHistory[ptrId] = currentPosition;
            }

            e.Handled = true;
        }

        /// <summary>
        /// The pressed pointer is released, which means that the move is ended.
        /// 1. Release the Pointer.
        /// 2. Clear the position history of the Pointer.
        /// </summary>
        private void Corner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            uint ptrId = e.GetCurrentPoint(this).PointerId;
            if (this.pointerPositionHistory.ContainsKey(ptrId))
            {
                this.pointerPositionHistory.Remove(ptrId);
            }

            (sender as UIElement).ReleasePointerCapture(e.Pointer);


            e.Handled = true;
        }

        /// <summary>
        /// The user manipulates the selectRegion. The manipulation includes
        /// 1. Translate
        /// 2. Scale
        /// </summary>
        void selectRegion_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.selectedRegion.UpdateSelectedRect(e.Delta.Scale, e.Delta.Translation.X, e.Delta.Translation.Y);
            e.Handled = true;
        }

        #endregion

        #region 关于所有手势操作

        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateClick(object sender, RoutedEventArgs e)
        {
            _styleAngle += 90;

            var finalAngle = _styleAngle;
            startAngle.Value = finalAngle - 90;
            endAngle.Value = finalAngle;

            //旋转后记得改宽高
            var temp = _imageHandleHelper.Height;
            _imageHandleHelper.Height = _imageHandleHelper.Width;
            _imageHandleHelper.Width = temp;

            RotateStory.Begin();

            //ChangeCanvasSize(new Size(_imageHandleHelper.Height, _imageHandleHelper.Width), new Size(_imageHandleHelper.Width, _imageHandleHelper.Height));
        }

        /// <summary>
        /// 旋转文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateTextClick(object sender, RoutedEventArgs e)
        {
            var flag = _currentTextViewFlag;
            switch (flag)
            {
                case 1:
                    {
                        _text1Angle += 90;
                        _compositeTransform1.Rotation = _text1Angle;
                    }
                    break;
                case 2:
                    {
                        _text2Angle += 90;
                        _compositeTransform2.Rotation = _text2Angle;
                    }
                    break;
                case 3:
                    {
                        _text3Angle += 90;
                        _compositeTransform3.Rotation = _text3Angle;
                    }
                    break;
            }
        }

        private void TextView1_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid1.ManipulationDelta -= TextView1_ManipulationDelta;
            textGrid1.ManipulationDelta += TextView1_ManipulationDelta;

            textGrid1.RenderTransform = _compositeTransform1;
        }

        private void TextView1_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _compositeTransform1.TranslateX += e.Delta.Translation.X/_compositeTransform1.ScaleX;
            _compositeTransform1.TranslateY += e.Delta.Translation.Y/ _compositeTransform1.ScaleY;

            _compositeTransform1.CenterX = 0;
            _compositeTransform1.CenterY = 0;
        }

        private void textGrid1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isInEditMode)
            {
                //在其他模式下先exit
                if (_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                else if (_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }

                //进入编辑模式
                EditInStory.Begin();
                _isInEditMode = true;
                _currentTextViewFlag = 1;

                _compositeTransform1.CenterX = 0;
                _compositeTransform1.CenterY = 0;
            }

            //已经在编辑模式下了
            _currentTextBox = TextView1;
            contentTB.Text = TextView1.Text;

#if WINDOWS_APP
            contentTB.Focus(FocusState.Programmatic);
#endif
        }

        private void TextView2_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid2.ManipulationDelta -= TextView2_ManipulationDelta;
            textGrid2.ManipulationDelta += TextView2_ManipulationDelta;

            textGrid2.RenderTransform = _compositeTransform2;
        }

        private void TextView2_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _compositeTransform2.TranslateX += e.Delta.Translation.X/ _compositeTransform2.ScaleX;
            _compositeTransform2.TranslateY += e.Delta.Translation.Y/ _compositeTransform2.ScaleY;

            _compositeTransform2.CenterX = 0;
            _compositeTransform2.CenterY = 0;
        }

        private void textGrid2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isInEditMode)
            {
                if (_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                else if (_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }
                EditInStory.Begin();
                _isInEditMode = true;
                _currentTextViewFlag = 2;

                _compositeTransform2.CenterX = 0;
                _compositeTransform2.CenterY = 0;
            }
            _currentTextBox = TextView2;
            contentTB.Text = TextView2.Text;

#if WINDOWS_APP
            contentTB.Focus(FocusState.Programmatic);
#endif
        }

        private void TextView3_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            textGrid3.ManipulationDelta -= TextView3_ManipulationDelta;
            textGrid3.ManipulationDelta += TextView3_ManipulationDelta;

            textGrid3.RenderTransform = _compositeTransform3;
        }

        private void TextView3_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _compositeTransform3.TranslateX += e.Delta.Translation.X/ _compositeTransform3.ScaleX;
            _compositeTransform3.TranslateY += e.Delta.Translation.Y/ _compositeTransform3.ScaleY;

            _compositeTransform3.CenterX = 0;
            _compositeTransform3.CenterY = 0;
        }

        private void textGrid3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isInEditMode)
            {
                if (_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                else if (_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }
                EditInStory.Begin();
                _isInEditMode = true;

                _compositeTransform3.CenterX = 0;
                _compositeTransform3.CenterY = 0;

                _compositeTransform3.CenterX = 0;
                _compositeTransform3.CenterY = 0;
            }
            _currentTextBox = TextView3;
            contentTB.Text = TextView3.Text;
            _currentTextViewFlag = 3;

#if WINDOWS_APP
            contentTB.Focus(FocusState.Programmatic);
#endif
        }

        private void StyleView_OnPointerEntered(object sender,PointerRoutedEventArgs e)
        {
            
            styleImage.ManipulationDelta -= StyleView_ManipulationDelta;
            styleImage.ManipulationDelta += StyleView_ManipulationDelta;
        }

        private void StyleView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _compositeTransformStyle.TranslateX += e.Delta.Translation.X;
            _compositeTransformStyle.TranslateY += e.Delta.Translation.Y;
        }

#endregion

        #region 滤镜
        private async void ApplyFilterClick(object sender,RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = (int)btn.Tag;
            await ApplyFilterAsync((FilterKind)tag);
        }    

        private async Task ApplyFilterAsync(FilterKind kindToApply)
        {
            if(kindToApply==FilterKind.Original)
            {
                ShowImage(this.afterCropImageFile ?? sourceImageFile);
                return;
            }

            else
            {
                ring.IsActive = true;
                _isLoadImage = false;

                try
                {
                    var fileToApply = afterCropImageFile ?? sourceImageFile;
                    var bitmap = await FilterApplyHelper.ApplyFilterAsync(kindToApply, _imageHandleHelper.Width, _imageHandleHelper.Height, fileToApply);
                    if (bitmap != null)
                    {
                        image.Source = bitmap;
                    }

                    this.currentFilter = kindToApply;
                    ring.IsActive = false;
                }
                catch(Exception e)
                {
                    var task=new MessageDialog(e.Message).ShowAsync();
                }
                
            }
        }

        #endregion

        #region 裁剪 保存 显示图像

        /// <summary>
        /// Save the cropped image.
        /// </summary>
        private async void CropClick(object sender, RoutedEventArgs e)
        {
            MaskGrid.Visibility = Visibility.Visible;

            double widthScale = imageCanvas.Width / _imageHandleHelper.Width;
            double heightScale = imageCanvas.Height / _imageHandleHelper.Height;

            var fileToSave = await ImageHandleHelper.GetTempFileToSave(_imageHandleHelper.FileName);

            if (fileToSave != null)
            {
                await CropBitmap.SaveCroppedBitmapAsync(
                    afterCropImageFile??sourceImageFile,
                    fileToSave,
                    new Point(this.selectedRegion.SelectedRect.X / widthScale, this.selectedRegion.SelectedRect.Y / heightScale),
                    new Size(this.selectedRegion.SelectedRect.Width / widthScale, this.selectedRegion.SelectedRect.Height / heightScale));

                MaskGrid.Visibility = Visibility.Collapsed;

                this.afterCropImageFile = fileToSave;
                ShowImage(this.afterCropImageFile);

                await ApplyFilterAsync(currentFilter);
                
                tempFileName = fileToSave.Name;

                CropOutStory.Begin();
                imageCanvas.Visibility = Visibility.Collapsed;
                _isInCropMode = false;
                _isLoadImage = false;
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="file">选取后的文件</param>
        private async void ShowImage(StorageFile file)
        {
            TextView1.Visibility = Visibility.Collapsed;
            ring.IsActive = true;
            sourceImageFile = file;
            try
            {
                var bitmap = await _imageHandleHelper.GetBitmapFromFileAsync(file);
                image.Source = bitmap;
            }
            catch (Exception e)
            {
                //new MessageDialog(e.Message).ShowAsync();
            }
            finally
            {
                ring.IsActive = false;
                TextView1.Visibility = Visibility.Visible;
            }
           
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        private async void SaveImage()
        {
            try
            {
                MaskGrid.Visibility = Visibility.Visible;

                var result = await _imageHandleHelper.SaveImageAsync(renderGrid);
                switch(result)
                {
                    case ImageSaveResult.FailToGetPixels:
                        {
                            throw new GetPixelsException();
                        };
                }

                if (tempFileName != "") await ImageHandleHelper.DeleteTempFile(tempFileName);
                
                MaskGrid.Visibility = Visibility.Collapsed;
                ShareGrid.Visibility = Visibility.Visible;
                _isInShareMode = true;

                //从分享打开后，不能再次分享
               // if (_isFromShareTarget) shareBtn.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                var task=ImageHandleHelper.DeleteFailedImage(_imageHandleHelper.FileName);

                if(e.GetType()==typeof(GetPixelsException))
                {
                    MaskGrid.Visibility = Visibility.Collapsed;
                    ErrorGrid.Visibility = Visibility.Visible;
                    _isInErrorMode = true;
                    var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                    errorHintTB.Text = loader.GetString("getpixelsErrorHint");
                    retryBtn.Visibility = Visibility.Collapsed;
                    return;
                }

                var task2 = ExceptionHelper.WriteRecord(e);
                MaskGrid.Visibility = Visibility.Collapsed;
                ErrorGrid.Visibility = Visibility.Visible;
                _isInErrorMode = true;

                //var sendTask=HttpHelper.SendDeviceInfo("");
            }
        }

        private async void CancelClick(object sender,RoutedEventArgs e)
        {
            var loader=Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var title = loader.GetString("DiscardTitle");
            var content = loader.GetString("DiscardContent");
            var discardBtn = loader.GetString("DiscardOK");
            var discardCancel = loader.GetString("DiscardCancel");

            var rootFrame = Window.Current.Content as Frame;

            MessageDialog md = new MessageDialog(content, title);
            md.Commands.Add(new UICommand(discardBtn, act =>
            {
                if (_isFromShareTarget) App.Current.Exit();
                Frame.Navigate(typeof(MainPage));
            }));
            md.Commands.Add(new UICommand(discardCancel, act =>
            {
                return;
            }));
            
            await md.ShowAsync();
        }

        private void DropCropClick(object sender,RoutedEventArgs e)
        {
            CropOutStory.Begin();
            imageCanvas.Visibility = Visibility.Collapsed;
            _isInCropMode = false;
           // _isLoadImage = false;
        }

        private void backErrorClick(object sender,RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Collapsed;
            _isInErrorMode = false;
        }

        private async void ShareClick(object sender, RoutedEventArgs e)
        {
            try
            {  
                PageNavigateData data = new PageNavigateData();
                data.isFromShare = false;
                data.file = await GetFileToShare();
                Frame.Navigate(typeof(SharePage),data);
            }
            catch (Exception ee)
            {
                var task = ExceptionHelper.WriteRecord(ee);
            }
        }

        private async Task<StorageFile> GetFileToShare()
        {
            var positon = LocalSettingHelper.GetValue("Position");
            StorageFile fileToGet = null;
            switch (positon)
            {
                case "0": fileToGet = await KnownFolders.SavedPictures.GetFileAsync(_imageHandleHelper.SavedFileName); break;
                case "1":
                    {
                        var folderToGet = await KnownFolders.PicturesLibrary.GetFolderAsync("MyerMoment");
                        fileToGet = await folderToGet.GetFileAsync(_imageHandleHelper.SavedFileName);
                    }; break;
                case "2":
                    {
                        fileToGet = await KnownFolders.CameraRoll.GetFileAsync(_imageHandleHelper.SavedFileName);
                    }; break;
            }
            if (fileToGet == null) return null;
            else return fileToGet;
        }



        private void BackHomeClick(object sender, RoutedEventArgs e)
        {
            if (_isFromShareTarget) App.Current.Exit();
            Frame.Navigate(typeof(MainPage));
        }

        private void retryClick(object sender,RoutedEventArgs e)
        {
            SaveClick(null, null);

            ErrorGrid.Visibility = Visibility.Collapsed;
            _isInErrorMode = false;

            LocalSettingHelper.AddValue("QualityCompress", "0");

        }

        #endregion

        #region Navigate Override
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif
            UpdatePageLayout();

            // Handle the pointer events of the corners. 
            AddCornerEvents(topLeftCorner);
            AddCornerEvents(topRightCorner);
            AddCornerEvents(bottomLeftCorner);
            AddCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta += selectRegion_ManipulationDelta;

            image.SizeChanged += sourceImage_SizeChanged;

            if(e.Parameter!=null)
            {
                
                if (e.Parameter.GetType() == typeof(PageNavigateData))
                {
                    var file = (e.Parameter as PageNavigateData).file;
                    this._isFromShareTarget = (e.Parameter as PageNavigateData).isFromShare;
                   
                    if (file != null) ShowImage(file);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#endif
            // Handle the pointer events of the corners. 
            RemoveCornerEvents(topLeftCorner);
            RemoveCornerEvents(topRightCorner);
            RemoveCornerEvents(bottomLeftCorner);
            RemoveCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta -= selectRegion_ManipulationDelta;

            image.SizeChanged -= sourceImage_SizeChanged;

        }

        private bool HandleBack()
        {
            if (_isInStyleMode || _isInShareMode || _isInErrorMode || _isInMoreLineMode || _isInEditMode || _isInCropMode || _isInFilterMode)
            {
                if (_isInStyleMode)
                {
                    MovieOutStory.Begin();
                    _isInStyleMode = false;
                }
                if(_isInCropMode)
                {
                    CropOutStory.Begin();
                    imageCanvas.Visibility = Visibility.Collapsed;
                    _isInCropMode = false;
                }
                if(_isInFilterMode)
                {
                    FilterOutStory.Begin();
                    _isInFilterMode = false;
                }
                if (_isInShareMode)
                {
                    BackHomeClick(null, null);
                }
                if (_isInErrorMode)
                {
                    backErrorClick(null, null);
                }
                if (_isInMoreLineMode)
                {
                    MoreLineOutStory.Begin();
                    _isInMoreLineMode = false;
                }
                if (_isInEditMode)
                {
                    EditOutStory.Begin();
                    _isInEditMode = false;
                    _currentTextBox = null;
                    contentTB.Text = "";
                }
                return true;
            }
            else return false;
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (HandleBack()) return;
            CancelClick(null, null);
        }
#endif

#endregion

    }
}

