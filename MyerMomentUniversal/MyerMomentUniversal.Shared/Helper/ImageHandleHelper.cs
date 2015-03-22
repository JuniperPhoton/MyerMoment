using System;
using System.Collections.Generic;
using System.Text;

namespace MyerMomentUniversal.Helper
{
    public class ImageHandleHelper
    {
        /// <summary>
        /// 压缩图片尺寸，直接修改原来的值
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static void CompressImageSize(ref uint width, ref uint height)
        {
            
            if (width > height)
            {
                if (width >= 4000)
                {
                    width = (uint)(width * 0.4);
                    height = (uint)(height * 0.4);
                }
                if (width >= 3000 && width<4000)
                {
                    width = (uint)(width * 0.5);
                    height = (uint)(height * 0.5);
                }
                else if (width >= 2000 & width < 3000)
                {
                    width = (uint)(width * 0.7);
                    height = (uint)(height * 0.7);
                }
                else if (width >= 1000 & width < 2000)
                {
                    width = (uint)(width * 0.95);
                    height = (uint)(height * 0.95);
                }
            }
            if (height > width)
            {
                if (height >= 4000)
                {
                    height = (uint)(height * 0.4);
                    width = (uint)(width * 0.4);
                }
                if (height >= 3000 && height<4000)
                {
                    height = (uint)(height * 0.5);
                    width = (uint)(width * 0.5);
                }
                else if (height >= 2000 & height < 3000)
                {
                    height = (uint)(height * 0.7);
                    width = (uint)(width * 0.7);
                }
                else if (height >= 1000 & height < 2000)
                {
                    height = (uint)(height * 0.95);
                    width = (uint)(width * 0.95);
                }
            }
        }


    }
}
