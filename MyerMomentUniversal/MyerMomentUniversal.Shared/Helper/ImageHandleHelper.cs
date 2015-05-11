
using JP.Utils.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;


namespace MyerMomentUniversal.Helper
{
    public class ImageHandleHelper
    {
        private uint outputWidth;
        private uint outputHeight;
        private IRandomAccessStream outputStream;

        private int exceptionFlag = 0; //the result is either 0 or 1

        public uint Height { get; set; }
        public uint Width { get; set; }
        public int DpiX { get; set; }
        public int DpiY { get; set; }
        public Guid EncodeID { get; set; }
        public string FileCopyName { get; set; } //图像文件原来的名字
        public string FileToSavedName { get; set; } //将要保存的名字
        public string SavedFileName { get; set; } //保存的图片的名字
        public int ScaleLong { get; set; }
        public BitmapAlphaMode AlphaMode { get; set; }
        public BitmapPixelFormat PixelFormat { get; set; }
        private BitmapTransform BitmapTransform { get; set; }

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
          
            //return ImageHandleLib.ImageHandleHelper.OpenImageFile(file);

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                //从文件流里创建解码器
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                //获取目前图像的信息
                this.DpiX = (int)decoder.DpiX;
                this.DpiY = (int)decoder.DpiY;
                this.Height = decoder.OrientedPixelHeight;
                this.Width = decoder.OrientedPixelWidth;
                this.FileCopyName = file.Name;
                this.PixelFormat = decoder.BitmapPixelFormat;
                this.AlphaMode = decoder.BitmapAlphaMode;

                switch (file.FileType.ToLower())
                {
                    case ".jpg": this.EncodeID = BitmapEncoder.JpegEncoderId; break;
                    case ".png": this.EncodeID = BitmapEncoder.PngEncoderId; break;
                    default:this.EncodeID = BitmapEncoder.JpegEncoderId;break;
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
        public async Task<ImageSaveResult> SaveImageAsync(UIElement elementToRender)
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
                await bitmap.RenderAsync(elementToRender, (int)(targetWidth), 0);
                var pixels = await bitmap.GetPixelsAsync();

                exceptionFlag++; //now it's 1

                StorageFile fileToSave = await GetFileToSave(this.FileCopyName,CreationCollisionOption.GenerateUniqueName);
               
                if (fileToSave == null) return ImageSaveResult.FileNotOpen;

                SavedFileName = fileToSave.Name;

                CachedFileManager.DeferUpdates(fileToSave);
                using (IRandomAccessStream fileStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(EncodeID, fileStream);
                    encoder.BitmapTransform.ScaledHeight = targetHeight;
                    encoder.BitmapTransform.ScaledWidth = targetWidth;
                    
                    encoder.SetPixelData(PixelFormat, AlphaMode, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, DpiX, DpiY, pixels.ToArray());
                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(fileToSave);

                return ImageSaveResult.Successful;
            }
            catch (Exception e)
            {
                if (exceptionFlag == 0) return ImageSaveResult.FailToGetPixels;
                else return ImageSaveResult.FailToFlush;
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

            var minus = Math.Abs((int)width - (int)height);
            if (minus < 10)
            {
                this.outputHeight = 1200;
                this.outputWidth = 1200;
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

        public async static Task<StorageFile> GetFileToSave(string fileName,CreationCollisionOption createoption)
        {
            var positon = LocalSettingHelper.GetValue("Position");
            StorageFile fileToSave = null;
            switch (positon)
            {
                case "0": fileToSave = await KnownFolders.SavedPictures.CreateFileAsync(fileName, createoption); break;
                case "1":
                    {
                        var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                        fileToSave = await folderToSave.CreateFileAsync(fileName, createoption);
                    }; break;
                case "2":
                    {
                        fileToSave = await KnownFolders.CameraRoll.CreateFileAsync(fileName, createoption);
                    }; break;
                default:
                    {
                        var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                        fileToSave = await folderToSave.CreateFileAsync(fileName, createoption);
                    }; break;
            }

            return fileToSave;
        }

        public async static Task<StorageFile> GetTempFileToSave(string fileName)
        {
            var fileToSave = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            return fileToSave;
        }

        public async static Task<bool> DeleteFailedImage(string fileName)
        {
            var positon = LocalSettingHelper.GetValue("Position");
            StorageFile fileToDelete = null;
            switch (positon)
            {
                case "0": fileToDelete = await KnownFolders.SavedPictures.GetFileAsync(fileName); break;
                case "1":
                    {
                        var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                        fileToDelete = await folderToSave.GetFileAsync(fileName);
                    }; break;
                case "2":
                    {
                        fileToDelete = await KnownFolders.CameraRoll.GetFileAsync(fileName);
                    }; break;
                default:
                    {
                        var folderToSave = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerMoment", CreationCollisionOption.OpenIfExists);
                        fileToDelete = await folderToSave.GetFileAsync(fileName);
                    }; break;
            }

            if (fileToDelete == null) return false;

            await fileToDelete.DeleteAsync();

            return true;
        }

        public async static Task<bool> DeleteTempFile(string fileName)
        {
            var folder = await ApplicationData.Current.TemporaryFolder.GetFolderAsync("Temp");
            var file = await folder.GetFileAsync(fileName);
            if (file != null)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            else return false;
        }
    }

}
