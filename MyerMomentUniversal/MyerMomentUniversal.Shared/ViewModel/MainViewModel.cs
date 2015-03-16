using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;
using System;
using Windows.Graphics.Display;
using System.IO;
using ChaoFunctionRT;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;


namespace MyerMomentUniversal.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        //private InMemoryRandomAccessStream _memStream;
        //private BitmapDecoder _decoder;
        private double _angle = 0;

        private uint _height;
        private uint _width;
        private int _dpiX;
        private int _dpiY;

        private Visibility _showProgressRingVisibility;
        public Visibility ShowProgressRingVisibility
        {
            get
            {
                return _showProgressRingVisibility;
            }
            set
            {
                if(_showProgressRingVisibility!=value)
                {
                    _showProgressRingVisibility = value;
                    RaisePropertyChanged(() => ShowProgressRingVisibility);
                }
            }
        }

        private Visibility _textViewVisibility;
        public Visibility TextViewVisibilty
        {
            get
            {
                return _textViewVisibility;
            }
            set
            {
                if(_textViewVisibility!=value)
                {
                    _textViewVisibility = value;
                    RaisePropertyChanged(() => TextViewVisibilty);
                }
            }
        }

        private Visibility _isSavingVisibility;
        public Visibility IsSavingVisibility
        {
            get
            {
                return _isSavingVisibility;
            }
            set
            {
                if(_isSavingVisibility!=value)
                {
                    _isSavingVisibility = value;
                    RaisePropertyChanged(() => IsSavingVisibility);
                }
            }
        }
        
        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        private RelayCommand _rotateCommand;
        public RelayCommand RotateCommand
        {
            get
            {
                if (_rotateCommand != null) return _rotateCommand;
                return _rotateCommand = new RelayCommand(() =>
                {
                    try
                    {
                        _angle += 90;
                        Messenger.Default.Send<GenericMessage<double>>(new GenericMessage<double>(_angle), "Rotate");
                    }
                    catch(Exception e)
                    {
                        var task = ExceptionHelper.WriteRecord(e);
                    }
                   
                });
            }
        }

        //private RelayCommand _okCommand;
        //public RelayCommand OKCommand
        //{
        //    get
        //    {
        //        if (_okCommand != null) return _okCommand;
        //        return _okCommand = new RelayCommand(async() =>
        //        {
        //            try
        //            {
        //                ShowProgressBar("Saving...");

        //                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(_memStream, _decoder);
        //                int _lastAngle = (int)_angle % 360;
        //                switch (_lastAngle.ToString())
        //                {
        //                    case "90": encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees; break;
        //                    case "180": encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise180Degrees; break;
        //                    case "270": encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees; break;
        //                    default: encoder.BitmapTransform.Rotation = BitmapRotation.None; break;
        //                }
        //                await encoder.FlushAsync();

        //                _memStream.Seek(0);

        //                var folderForSaveFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
        //                var fileToSave = await folderForSaveFile.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);

        //                using (var stream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
        //                {

        //                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
        //                    var _encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

        //                    //获取像素数据
        //                    var decoder = await BitmapDecoder.CreateAsync(_memStream);
        //                    var pixels = await decoder.GetPixelDataAsync();

        //                    _encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)decoder.PixelWidth, (uint)decoder.PixelHeight, logicalDpi, logicalDpi, pixels.DetachPixelData());

        //                    await _encoder.FlushAsync();
                            
        //                    _memStream.Dispose();

        //                    HideProgressBar();

        //                    var rootFrame = Window.Current.Content as Frame;
        //                    if(rootFrame.CanGoBack)
        //                    {
        //                        rootFrame.GoBack();
        //                    }
        //                }
        //            }
        //            catch(Exception e)
        //            {
        //                var task = ExceptionHelper.WriteRecord(e);
        //            }
        //        });
        //    }
        //}

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand != null) return _cancelCommand;
                return _cancelCommand = new RelayCommand(async() =>
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
                });
            }
        }

        public MainViewModel()
        {
            Image = new BitmapImage();

            IsSavingVisibility = Visibility.Collapsed;

            Messenger.Default.Register<GenericMessage<StorageFile>>(this, "PickedFile", act =>
                {
                    ShowImage(act.Content);
                });

            Messenger.Default.Register<GenericMessage<UIElement>>(this, "RenderGrid", act =>
                {
                    SaveImage(act.Content);
                });
        }

        private async void ShowImage(StorageFile file)
        {
            try
            {
                ShowProgressRingVisibility = Visibility.Visible;
                TextViewVisibilty = Visibility.Collapsed;

                var fileStream = await file.OpenAsync(FileAccessMode.Read);
                
                //从文件流里创建解码器
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                this._dpiX = (int)decoder.DpiX;
                this._dpiY = (int)decoder.DpiY;
                this._height = decoder.OrientedPixelHeight;
                this._width =decoder.OrientedPixelWidth;

               
                //显示图像
                var task = Image.SetSourceAsync(fileStream);
                task.Completed += (sendert, et) =>
                    {
                        ShowProgressRingVisibility = Visibility.Collapsed;
                        TextViewVisibilty = Visibility.Visible;
                    };

            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
            }
        }

        private async void SaveImage(UIElement elementToRender)
        {
            try
            {
                IsSavingVisibility = Visibility.Visible;

                uint targetWidth = this._width;
                uint targetHeight = this._height;

                var fileToSave = await KnownFolders.SavedPictures.CreateFileAsync("MyerMoment.jpg", CreationCollisionOption.GenerateUniqueName);
                
                CachedFileManager.DeferUpdates(fileToSave);
                using (var fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(elementToRender,(int)targetWidth,(int)targetHeight);

                    var pixels =await bitmap.GetPixelsAsync();

                    var propertySet = new BitmapPropertySet();

                    var qualityValue = new BitmapTypedValue(0.75,PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream,propertySet);

                    var logicalDpi=DisplayInformation.GetForCurrentView().LogicalDpi;

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, _dpiX, _dpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                IsSavingVisibility = Visibility.Collapsed;

                var rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                IsSavingVisibility = Visibility.Collapsed;
                //new MessageDialog(e.Message.ToString(), "Error").ShowAsync();
            }
            

        }

#if WINDOWS_PHONE_APP
        private async void ShowProgressBar(string textToHint,bool goBack=false,double timeToStop=0)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = textToHint;
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            if (timeToStop == 0) return;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(timeToStop);
            timer.Tick += (async (s, k) =>
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                if(goBack)
                {
                    var rootFrame = Window.Current.Content as Frame;
                    if (rootFrame.CanGoBack)
                    {
                        rootFrame.GoBack();
                    }
                }
               
                timer.Stop();
            });
            timer.Start();
        }

        private async void HideProgressBar()
        {
            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

#endif
    }
}