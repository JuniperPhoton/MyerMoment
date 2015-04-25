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

        /// <summary>
        /// 初始化Style类
        /// </summary>  
        /// <param name="nameID">图片的名字ID</param>
        /// <param name="isFromWeb">是否来自Web的导入</param>
        public MomentStyle(string nameID)
        {
            NameID = nameID;

            FullSizeImage = new BitmapImage();
            FullSizeImage.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".png", UriKind.RelativeOrAbsolute);

            PreviewImage = new BitmapImage();
            PreviewImage.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".jpg", UriKind.RelativeOrAbsolute);
        }

        public MomentStyle(string nameID,Uri thumbUri,Uri fullSizeUri)
        {
            this.NameID = nameID;
            this.thumbUri = thumbUri;
            this.fullSizeUri = fullSizeUri;
            
        }

        /// <summary>
        /// 检查当前样式是否在已经存在本地
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public static async Task<bool> CheckStyleExist(string styleName)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MyerMomentStyleList", CreationCollisionOption.OpenIfExists);
            var thumbFile = await folder.GetFileAsync(styleName + ".jpg");
            return thumbFile == null ? false : true;
        }

        public async Task CheckStyleExistAndSaveAsync()
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MyerMomentStyleList", CreationCollisionOption.OpenIfExists);
            var thumbFile = await folder.GetFileAsync(this.NameID+".jpg");
            if(thumbFile!=null)
            {
                PreviewImage = new BitmapImage();
                PreviewImage.UriSource =new Uri(thumbFile.Path);

                var fullSizeFile = await folder.GetFileAsync(this.NameID + ".png");
                if (fullSizeFile != null)
                {
                    FullSizeImage = new BitmapImage();
                    FullSizeImage.UriSource = new Uri(fullSizeFile.Path);
                }
            }
            else
            {
                await SaveWebStyleToStorageAsync();
            }

        }

        

        public async Task SaveWebStyleToStorageAsync()
        {
            var thumbFile = await DownLoadAndSaveAsync(NameID + ".jpg", "jpg", thumbUri);
            var fullsizeFile = await DownLoadAndSaveAsync(NameID + ".png", "png", fullSizeUri);

            var thumbStream = await GetStreamFromFileAsync(thumbFile);
            var fullsizeStream = await GetStreamFromFileAsync(fullsizeFile);

            PreviewImage = new BitmapImage();
            await PreviewImage.SetSourceAsync(thumbStream);

            FullSizeImage = new BitmapImage();
            await FullSizeImage.SetSourceAsync(fullsizeStream);
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

                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MyerMomentStyleList", CreationCollisionOption.OpenIfExists);
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
                    catch(Exception e)
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
}
