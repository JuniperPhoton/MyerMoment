using Lumia.Imaging;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MyerMomentUniversal.Helper
{
    public class FilterApplyHelper
    {
        public async static Task<WriteableBitmap> ApplyFilterAsync(FilterKind kind,uint width,uint height,StorageFile fileToApply)
        {
            //初始化一个可写的新的Bitmap
            var _imageBitmap = new WriteableBitmap((int)width, (int)height);

            using (var fileStream = await fileToApply.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    fileStream.Seek(0);

                    //创建新的流，初始化FilterEffect
                    var imageSource = new RandomAccessStreamImageSource(fileStream);
                    var _effect = new FilterEffect(imageSource);

                    //把滤镜加入FilterEffect
                    var filter = FilterFactory.CreateFilterEffect(kind);
                    if (filter != null) _effect.Filters = new[] { filter };

                    // Render the image to a WriteableBitmap. 
                    var renderer = new WriteableBitmapRenderer(_effect, _imageBitmap);
                    _imageBitmap = await renderer.RenderAsync();
                    _imageBitmap.Invalidate();

                    return _imageBitmap;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
