using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using System.IO;
using Windows.UI.Xaml;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using GalaSoft.MvvmLight;
using JP.Utils.Data;
using JP.Utils.Debug;
using Windows.ApplicationModel.Resources;

namespace MyerMomentUniversal.Model
{
    public class MomentStyle:ViewModelBase
    {
        private const int IMAGE_NUM = 17;

        private Uri thumbUri;
        private Uri fullSizeUri;

        private string _nameID;
        public string NameID
        {
            get { return _nameID; }
            set { _nameID = value; RaisePropertyChanged(() => NameID); }
        }

        private BitmapImage _fullSizeImage;
        public BitmapImage FullSizeImage
        {
            get { return _fullSizeImage; }
            set { _fullSizeImage = value; RaisePropertyChanged(() => FullSizeImage); }
        }

        private BitmapImage _previewImage;
        public BitmapImage PreviewImage
        {
            get
            {
                return _previewImage;
            }
            set
            {
                _previewImage = value;
                RaisePropertyChanged(() => PreviewImage);
            }
        }

        private bool _isDownloaded;
        public bool IsDownloaded
        {
           get
            {
                return _isDownloaded;
            }
            set
            {
                _isDownloaded = value;
                RaisePropertyChanged(() => IsDownloaded);
                RaisePropertyChanged(() => DownloadHint);
            }
        }

        public string DownloadHint
        {
            get
            {
                var downStr = ResourceLoader.GetForCurrentView().GetString("DownloadHint");
                var downedStr = ResourceLoader.GetForCurrentView().GetString("DownloadedHint");
                if (IsDownloaded)
                {
                    return downedStr;
                }
                else return downStr;
            }
        }
       
        public static int ImageNum { get; set; } = 1;
        public BitmapImage RandomBackGrd
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();

                bitmap.UriSource = new Uri("ms-appx:///Asset/Backgrd/" + ImageNum + ".jpg");

                ImageNum++;
                if (ImageNum > IMAGE_NUM) ImageNum = 1;

                return bitmap;
            }
        }

        private Visibility _isDownloadingVisibility;
        public Visibility IsDownloadingVisibility
        {
            get
            {
                return _isDownloadingVisibility;
            }
            set
            {
                _isDownloadingVisibility = value;
                RaisePropertyChanged(() => IsDownloadingVisibility);
            }
        }

        private Visibility _canUninstallVisibility;
        public Visibility CanUninstallVisibility
        {
            get
            {
                return _canUninstallVisibility;
            }
            set
            {
                _canUninstallVisibility = value;
                RaisePropertyChanged(() => CanUninstallVisibility);
            }
        }

        /// <summary>
        /// From local storage
        /// </summary>
        /// <param name="nameID"></param>
        public MomentStyle(string nameID)
        {
            NameID = nameID;

            FullSizeImage = new BitmapImage();
            FullSizeImage.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".png", UriKind.RelativeOrAbsolute);

            PreviewImage = new BitmapImage();
            PreviewImage.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".jpg", UriKind.RelativeOrAbsolute);

            IsDownloaded = true;
            IsDownloadingVisibility = Visibility.Collapsed;
            CanUninstallVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// From web
        /// </summary>
        /// <param name="nameID"></param>
        /// <param name="thumbUri"></param>
        /// <param name="fullSizeUri"></param>
        public MomentStyle(string nameID,Uri thumbUri,Uri fullSizeUri)
        {
            this.NameID = nameID;
            this.thumbUri = thumbUri;
            this.fullSizeUri = fullSizeUri;

            IsDownloaded = false;
            IsDownloadingVisibility = Visibility.Collapsed;
            CanUninstallVisibility = Visibility.Visible;
        }

        public void SetUpInstalledStyle()
        {
            PreviewImage = new BitmapImage();
            PreviewImage.UriSource = thumbUri;

            FullSizeImage = new BitmapImage();
            FullSizeImage.UriSource = fullSizeUri;
        }


        public async Task CheckThumbExistAndSaveAsync()
        {
            await ExceptionHelper.TryExecute(async () =>
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);

                var thumbFile = await StorageFileHandleHelper.TryGetFile(folder, this.NameID + "2" + ".png");
                if (thumbFile != null)
                {
                    PreviewImage = new BitmapImage();
                    PreviewImage.UriSource = new Uri(thumbFile.Path);
                }
                else
                {
                    await GetStyle(StyleFileType.Thumb);
                }
            }); 
        }

        public async Task<bool> GetStyle(StyleFileType type)
        {
            try
            {
                IsDownloadingVisibility = Visibility.Visible;

                var fileName = this.NameID + (type == StyleFileType.Thumb ? "2" : "") + ".png";
                var file = await DownLoadAndSaveAsync(fileName, type==StyleFileType.Thumb?thumbUri:fullSizeUri);

                if (file == null) return false;

                var fileStream = await GetStreamFromFileAsync(file);
                if (type == StyleFileType.Thumb)
                {
                    PreviewImage = new BitmapImage();
                    await PreviewImage.SetSourceAsync(fileStream);
                }
                else
                {
                    FullSizeImage = new BitmapImage();
                    await FullSizeImage.SetSourceAsync(fileStream);
                    IsDownloaded = true;
                }
                IsDownloadingVisibility = Visibility.Collapsed;
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private async Task<StorageFile> DownLoadAndSaveAsync(string name, Uri uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response =await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var stream = buffer.AsStream();

                    using (var mem = new MemoryStream())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await stream.CopyToAsync(mem);
                        mem.Seek(0, SeekOrigin.Begin);
                        var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());

                        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
                        var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                        using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            var data = (await decoder.GetPixelDataAsync()).DetachPixelData();

                            try
                            {
                                Guid decodeID = BitmapEncoder.PngEncoderId;

                                var encoder = await BitmapEncoder.CreateAsync(decodeID, fileStream);
                                encoder.SetPixelData(
                                    BitmapPixelFormat.Bgra8,
                                    BitmapAlphaMode.Premultiplied,
                                    decoder.OrientedPixelWidth,
                                    decoder.OrientedPixelHeight,
                                    decoder.DpiX,
                                    decoder.DpiY,
                                    data);

                                await encoder.FlushAsync();
                                return file;
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }
                }
                else return null;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private async Task<IRandomAccessStream> GetStreamFromFileAsync(StorageFile fileToOpen)
        {
            return await ExceptionHelper.TryExecute(async() =>
            {
                var fileStream = await fileToOpen.OpenStreamForReadAsync();

                return fileStream.AsRandomAccessStream();
            });
           
        }

    }

    public enum StyleFileType
    {
        Thumb,
        FullSize,
    }
}
