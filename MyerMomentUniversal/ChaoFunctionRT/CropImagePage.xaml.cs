using ChaoFunctionRT;
using MyerMomentUniversal.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MyerMomentUniversal.Model;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MyerMomentUniversal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CropImagePage : Page
    {
        SelectedRegion selectedRegion;

        // The current image file to be cropped.
        StorageFile sourceImageFile = null;
        string fileName = "";

        private SelectedRegionShape SelectedShape = SelectedRegionShape.Free;

        private PageNavigateData navigateData = new PageNavigateData();

        uint sourceImagePixelWidth;
        uint sourceImagePixelHeight;

        /// <summary>
        /// The size of the corners.
        /// </summary>
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

        /// <summary>
        /// The previous points of all the pointers.
        /// </summary>
        Dictionary<uint, Point?> pointerPositionHistory = new Dictionary<uint, Point?>();

        public CropImagePage()
        {
            this.InitializeComponent();

            selectRegion.ManipulationMode = ManipulationModes.Scale |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            selectedRegion = new SelectedRegion { MinSelectRegionSize = 2 * CornerSize };
            this.DataContext = selectedRegion;

#if WINDOWS_PHONE_APP
            StatusBar.GetForCurrentView().ForegroundColor = (App.Current.Resources["MomentThemeBlack"] as SolidColorBrush).Color;
#endif

            ConfigLang();

            
        }

        #region CONFIG
        private void ConfigLang()
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.cancelTB.Text = loader.GetString("CancleBtn");
            this.cropingTB.Text = loader.GetString("CroppingHint");
            this.nextTB.Text = loader.GetString("ContinueBtn");
            this.cropHeaderTB.Text= loader.GetString("CropHeader");
        }
        #endregion

        #region COMMAND

        private void SetToSquareClick(object sender, RoutedEventArgs e)
        {

            if (SelectedShape == SelectedRegionShape.Square || SelectedShape==SelectedRegionShape.Free)
            {
                this.selectedRegion.ResetCorner(0, 0, sourceImage.ActualWidth, sourceImage.ActualHeight);
            }

            var topLeftX = Canvas.GetLeft(topLeftCorner);
            var bottomLeftX = Canvas.GetLeft(bottomLeftCorner);

            var topRightX=Canvas.GetLeft(topRightCorner);
            var bottomRightX=Canvas.GetLeft(bottomRightCorner);

            var topLeftY = Canvas.GetTop(topLeftCorner);
            var bottomLeftY = Canvas.GetTop(bottomLeftCorner);

            var topRightY=Canvas.GetTop(topRightCorner);
            var bottomRightY=Canvas.GetTop(bottomRightCorner);

            
            
            //the photo is in wide
            if (sourceImage.ActualWidth > sourceImage.ActualHeight)
            {
                var widthChanged = -(topRightX - (topLeftX + sourceImage.ActualHeight));

                this.SelectedShape = SelectedRegionShape.Square;
                selectedRegion.UpdateCorner((string)topRightCorner.Tag, widthChanged, 0);

            }
            else //the photo is in height
            {
                var heightChanged = -(topRightY - (topLeftY + sourceImage.ActualWidth));
                this.SelectedShape = SelectedRegionShape.Square;
                selectedRegion.UpdateCorner((string)bottomRightCorner.Tag, 0, -heightChanged);
            }

            
        }

        private void ResetClick(object sender,RoutedEventArgs e)
        {
            this.selectedRegion.ResetCorner(0, 0, sourceImage.ActualWidth, sourceImage.ActualHeight);
            this.SelectedShape = SelectedRegionShape.Free;

            //VisualStateManager.GoToState(squareBtn, "OffState", false);
            //VisualStateManager.GoToState(wideBtn, "OffState", false);
            //VisualStateManager.GoToState(resetBtn, "OnState", false);
        }

        #endregion

        #region EVENT METHOD
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

        #region SHOW AND CROP IMAGE

        /// <summary>
        /// Let user choose an image and load it.
        /// </summary>
        async private void ShowImage(StorageFile file)
        {
            var imgFile = file;
            fileName = imgFile.Name;
            if (imgFile != null)
            {
                this.sourceImage.Source = null;
                this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    this.sourceImageFile = imgFile;
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(fileStream);
                    sourceImage.Source = bitmap;
                }

                //if (this.sourceImagePixelHeight < 2 * this.CornerSize || this.sourceImagePixelWidth < 2 * this.CornerSize)
                //{
                //    return;
                //}
                //else
                //{
                //    double sourceImageScale = 1;

                //    if (this.sourceImagePixelHeight < this.sourceImageGrid.ActualHeight &&
                //        this.sourceImagePixelWidth < this.sourceImageGrid.ActualWidth)
                //    {
                //        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                //    }
                //    else
                //    {
                //        sourceImageScale = Math.Min(this.sourceImageGrid.ActualWidth / this.sourceImagePixelWidth,
                //             this.sourceImageGrid.ActualHeight / this.sourceImagePixelHeight);
                //        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                //    }

                //    this.sourceImage.Source = await CropBitmap.GetCroppedBitmapAsync(
                //        this.sourceImageFile,
                //        new Point(0, 0),
                //        new Size(this.sourceImagePixelWidth, this.sourceImagePixelHeight),
                //        sourceImageScale);
                //}

            }
        }

        /// <summary>
        /// This event will be fired when 
        /// 1. An new image is opened.
        /// 2. The source of the sourceImage is set to null.
        /// 3. The view state of this application is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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


        /// <summary>
        /// Save the cropped image.
        /// </summary>
        private async void CropClick(object sender, RoutedEventArgs e)
        {
            MaskGrid.Visibility = Visibility.Visible;

            double widthScale = imageCanvas.Width / this.sourceImagePixelWidth;
            double heightScale = imageCanvas.Height / this.sourceImagePixelHeight;


            var fileToSave =await ImageHandleHelper.GetFileToSaved(fileName,CreationCollisionOption.GenerateUniqueName);

            if (fileToSave != null)
            {
                await CropBitmap.SaveCroppedBitmapAsync(
                    sourceImageFile,
                    fileToSave,
                    new Point(this.selectedRegion.SelectedRect.X / widthScale, this.selectedRegion.SelectedRect.Y / heightScale),
                    new Size(this.selectedRegion.SelectedRect.Width / widthScale, this.selectedRegion.SelectedRect.Height / heightScale));

                MaskGrid.Visibility = Visibility.Collapsed;

                navigateData.file = fileToSave;

                Frame.Navigate(typeof(ImageHandlePage), navigateData);
            }
        }

        private async void CancelClick(object sender, RoutedEventArgs e)
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var title = loader.GetString("DiscardTitle");
            var content = loader.GetString("DiscardContent");
            var discardBtn = loader.GetString("DiscardOK");
            var discardCancel = loader.GetString("DiscardCancel");

            var rootFrame = Window.Current.Content as Frame;

            MessageDialog md = new MessageDialog(content, title);
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

                if(SelectedShape==SelectedRegionShape.Free)
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

        #region NAVIGATE
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif

            if (e.Parameter != null)
            {

                if (e.Parameter.GetType() == typeof(StorageFile))
                {
                    StorageFile args = e.Parameter as StorageFile;

                    ShowImage(args);
                }

                if (e.Parameter.GetType() == typeof(ShareOperation))
                {
                    var shareOperation = e.Parameter as ShareOperation;
                    var items = await shareOperation.Data.GetStorageItemsAsync();

                    var firstItem = items.FirstOrDefault();
                    if (firstItem != null)
                    {
                        var path = firstItem.Path;
                        var fileToOpen = await StorageFile.GetFileFromPathAsync(path);
                        ShowImage(fileToOpen);

                        navigateData.isFromShare = true;

                        shareOperation.ReportCompleted();
                    }

                }
            }


            // Handle the pointer events of the corners. 
            AddCornerEvents(topLeftCorner);
            AddCornerEvents(topRightCorner);
            AddCornerEvents(bottomLeftCorner);
            AddCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta += selectRegion_ManipulationDelta;

            this.sourceImage.SizeChanged += sourceImage_SizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

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

            this.sourceImage.SizeChanged -= sourceImage_SizeChanged;

        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            
            CancelClick(null, null);
        }
#endif
        #endregion
    }
}
