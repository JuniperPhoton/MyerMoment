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

namespace MyerMomentUniversal.Model
{
    public class MomentStyle:ViewModelBase
    {
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
            }
        }

        public int imageNum;
        public BitmapImage RandomBackGrd
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.UriSource =new Uri("ms-appx:///Asset/Backgrd/" +imageNum+ ".jpg");
                return bitmap;
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
        }

        public async Task CheckThumbExistAndSaveAsync()
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);

            var thumbFile = await StorageFileHandleHelper.TryGetFile(folder, this.NameID + ".jpg"); 
            if(thumbFile!=null)
            {
                PreviewImage = new BitmapImage();
                PreviewImage.UriSource =new Uri(thumbFile.Path);
            }
            else
            {
                await GetStyle(ImageFileType.Jpeg);
            }
        }

        public async Task GetStyle(ImageFileType type)
        {
            var ext = type == ImageFileType.Jpeg ? "jpg" : "png";
            var file = await DownLoadAndSaveAsync(NameID + "."+ ext, ext, thumbUri);
            var fileStream = await GetStreamFromFileAsync(file);

            if(type == ImageFileType.Jpeg)
            {
                PreviewImage = new BitmapImage();
                await PreviewImage.SetSourceAsync(fileStream);
            }
            else
            {
                FullSizeImage = new BitmapImage();
                await FullSizeImage.SetSourceAsync(fileStream);
            }
        }

        private async Task<StorageFile> DownLoadAndSaveAsync(string name,string fileType, Uri uri)
        {
            HttpClient client = new HttpClient();
            var buffer = await client.GetBufferAsync(uri);
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
                        Guid decodeID;
                        switch(fileType)
                        {
                            case "jpg": decodeID = BitmapEncoder.JpegEncoderId; break;
                            case "png": decodeID = BitmapEncoder.PngEncoderId; break;
                            default: decodeID = BitmapEncoder.JpegEncoderId; break;
                        }
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
                    catch(Exception)
                    {
                        return null;
                    }
                }
            }
        }

        private async Task<IRandomAccessStream> GetStreamFromFileAsync(StorageFile fileToOpen)
        {
            var fileStream = await fileToOpen.OpenStreamForReadAsync();
            
           return fileStream.AsRandomAccessStream();
           
        }

    }

    public enum ImageFileType
    {
        Png,
        Jpeg
    }
}
