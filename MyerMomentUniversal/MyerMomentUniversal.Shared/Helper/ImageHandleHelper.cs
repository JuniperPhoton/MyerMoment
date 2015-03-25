using ChaoFunctionRT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MyerMomentUniversal.Helper
{
    public class ImageHandleHelper
    {
        public uint outputWidth;
        public uint outputHeight;
        public IRandomAccessStream outputStream;

        public ImageHandleHelper()
        {
            
        }

        /// <summary>
        /// 压缩图片尺寸，直接修改原来的值
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static void CompressImageSize(ref uint width, ref uint height)
        {
            
            var factor = (double)height / width;
            
            if (width >= 4000)
            {
                width = (uint)(width * 0.4);
                height = (uint)(height * 0.4);
            }
            if (width >= 3000 && width < 4000)
            {
                if(factor.ToString().StartsWith("0.7"))
                {
                    width = (uint)(width * 0.3);
                    height = (uint)(height * 0.3);
                }
                else
                {
                    width = (uint)(width * 0.5);
                    height = (uint)(height * 0.5);
                }
                
            }
            else if (width >= 2000 & width < 3000)
            {
                width = (uint)(width * 0.6);
                height = (uint)(height * 0.6);
            }
            else if (width >= 1000 & width < 2000)
            {
                width = (uint)(width * 0.7);
                height = (uint)(height * 0.7);
            }
        }

        public void CompressImage(uint scaledLong,uint width,uint height)
        {
            this.outputHeight = height;
            this.outputWidth = width;

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

        public async Task ResizeImageAsync(IRandomAccessStream sourceStream, uint scaleLong)
        {
            try
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                uint height = decoder.PixelHeight;
                uint width = decoder.PixelWidth;

                double rate;
                uint destHeight = height;
                uint destWidth = width;

                if (width > height && width>scaleLong)
                {
                    rate = scaleLong / (double)width;
                    destHeight = width > scaleLong ? (uint)(rate * height) : height;
                    destWidth = scaleLong;
                }
                else if(height>width && height>scaleLong)
                {
                    rate = scaleLong / (double)height;
                    destWidth = height > scaleLong ? (uint)(rate * width) : width;
                    destHeight = scaleLong;
                }

                outputHeight = destHeight;
                outputWidth = destWidth;

                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledWidth = destWidth,
                    ScaledHeight = destHeight
                };

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                var tempfile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.GenerateUniqueName);
                IRandomAccessStream destStream = await tempfile.OpenAsync(FileAccessMode.ReadWrite);
                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(destStream, decoder);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, destWidth, destHeight, decoder.DpiX, decoder.DpiY, pixelData.DetachPixelData());
                await encoder.FlushAsync();

                //REMEMBER
                destStream.Seek(0);

                await tempfile.DeleteAsync(StorageDeleteOption.PermanentDelete);

                outputStream = destStream;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
            }
        }


    }
}
