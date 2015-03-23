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


    }
}
