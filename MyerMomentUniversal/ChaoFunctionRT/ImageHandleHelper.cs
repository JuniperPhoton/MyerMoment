using ChaoFunctionRT;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;


namespace ChaoFunctionRT
{
    public class ImageHandleHelper
    {
        public uint outputWidth;
        public uint outputHeight;
        public IRandomAccessStream outputStream;

        public uint Height { get; set; }
        public uint Width { get; set; }
        public int DpiX { get; set; }
        public int DpiY { get; set; }
        public Guid EncodeID { get; set; }
        public string FileName { get; set; }
        public int ScaleLong { get; set; }
        public BitmapAlphaMode AlphaMode { get; set; }
        public BitmapPixelFormat PixelFormat { get; set; }
        public string SavedFileName { get; set; }

        public ImageHandleHelper()
        {
            
        }

        /// <summary>
        /// 打开图像文件，获取BitmapImage
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        public async Task<BitmapImage> GetBitmapFromFileAsync(StorageFile file)
        {
            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                //从文件流里创建解码器
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                //获取目前图像的信息
                this.DpiX = (int)decoder.DpiX;
                this.DpiY = (int)decoder.DpiY;
                this.Height = decoder.OrientedPixelHeight;
                this.Width = decoder.OrientedPixelWidth;
                this.FileName = file.Name;
                this.PixelFormat = decoder.BitmapPixelFormat;
                this.AlphaMode = decoder.BitmapAlphaMode;

                switch (file.FileType)
                {
                    case ".jpg": this.EncodeID = BitmapEncoder.JpegEncoderId; break;
                    case ".png": this.EncodeID = BitmapEncoder.PngEncoderId; break;
                }

                //显示图像
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(fileStream);

                return bitmap;
            }
        }

        /// <summary>
        /// 保存UIElement元素
        /// </summary>
        /// <param name="elementToRender">UIElement</param>
        /// <returns></returns>
        public async Task<bool> SaveImageAsync(UIElement elementToRender)
        {
            try
            {
                uint targetWidth = Width;
                uint targetHeight = Height;

                #if WINDOWS_PHONE_APP
                //压缩图像
                if (LocalSettingHelper.GetValue("QualityCompress") == "0")
                {
                    var imagehelper = new ImageHandleHelper();
                    imagehelper.CompressImage((uint)ScaleLong, targetWidth, targetHeight);
                    targetWidth = imagehelper.outputWidth;
                    targetHeight = imagehelper.outputHeight;
                }
                #endif

                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(elementToRender, (int)(targetWidth), (int)(targetHeight));

                var pixels = await bitmap.GetPixelsAsync();

                //处理保存的位置
                var positon = LocalSettingHelper.GetValue("Position");
                StorageFile fileToSave = null;
                switch (positon)
                {
                    case "0": fileToSave = await KnownFolders.SavedPictures.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName); break;
                    case "1":
                        {
                            var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                            fileToSave = await folderToSave.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                        }; break;
                    case "2":
                        {
                            fileToSave = await KnownFolders.CameraRoll.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                        }; break;
                    default:
                        {
                            var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                            fileToSave = await folderToSave.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                        }; break;
                }
                if (fileToSave == null) return false;

                SavedFileName = fileToSave.Name;

                //CachedFileManager.DeferUpdates(fileToSave);
                using (IRandomAccessStream fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {

                    var encoder = await BitmapEncoder.CreateAsync(EncodeID, fileStream);
                    encoder.BitmapTransform.ScaledHeight = targetHeight;
                    encoder.BitmapTransform.ScaledWidth = targetWidth;
                    
                    encoder.SetPixelData(PixelFormat, AlphaMode, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, DpiX, DpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }

                return true;
                //await CachedFileManager.CompleteUpdatesAsync(fileToSave);
            }
            catch (Exception)
            {
                return false;
            }
          
        }

        /// <summary>
        /// 压缩图像
        /// </summary>
        /// <param name="scaledLong">压缩后的相对长度</param>
        /// <param name="width">原来的宽</param>
        /// <param name="height">原来的高</param>
        public void CompressImage(uint scaledLong,uint width,uint height)
        {
            this.outputHeight = height;
            this.outputWidth = width;

            if(width==height)
            {
                this.outputHeight = scaledLong;
                this.outputWidth = scaledLong;
                return;
            }

            if (width > height && width>scaledLong)
            {
                var factor = (double)scaledLong / width;
                width =(uint)( width * factor);
                height = (uint)(height * factor);
                this.outputWidth = width;
                this.outputHeight = height;
            }
            else if(height>width && height>scaledLong)
            {
                var factor = (double)scaledLong / height;
                width = (uint)(width * factor);
                height = (uint)(height * factor);
                this.outputWidth = width;
                this.outputHeight = height;
            }
        }

        public string OutputDecoderInfo()
        {
            return "&oriWidth=" + Width + "&oriHeight=" + Height + "&outputWidth=" + outputHeight +
                "&outputHeight" + outputHeight + "&dpiX=" + DpiX + "&dpiY=" + DpiY;
        }
    }
}
