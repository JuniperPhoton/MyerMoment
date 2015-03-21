using System;
using System.Collections.Generic;
using System.Text;

namespace MyerMomentUniversal.Helper
{
    public class ImageHandleHelper
    {
        public static void CompressImageSize(ref uint targetWidth, ref uint targetHeight)
        {
            
            if (targetWidth > targetHeight)
            {
                if (targetWidth >= 4000)
                {
                    targetWidth = (uint)(targetWidth * 0.4);
                    targetHeight = (uint)(targetHeight * 0.4);
                }
                if (targetWidth >= 3000 && targetWidth<4000)
                {
                    targetWidth = (uint)(targetWidth * 0.5);
                    targetHeight = (uint)(targetHeight * 0.5);
                }
                else if (targetWidth >= 2000 & targetWidth < 3000)
                {
                    targetWidth = (uint)(targetWidth * 0.7);
                    targetHeight = (uint)(targetHeight * 0.7);
                }
                else if (targetWidth >= 1000 & targetWidth < 2000)
                {
                    targetWidth = (uint)(targetWidth * 0.95);
                    targetHeight = (uint)(targetHeight * 0.95);
                }
            }
            if (targetHeight > targetWidth)
            {
                if (targetHeight >= 4000)
                {
                    targetHeight = (uint)(targetHeight * 0.4);
                    targetWidth = (uint)(targetWidth * 0.4);
                }
                if (targetHeight >= 3000 && targetHeight<4000)
                {
                    targetHeight = (uint)(targetHeight * 0.5);
                    targetWidth = (uint)(targetWidth * 0.5);
                }
                else if (targetHeight >= 2000 & targetHeight < 3000)
                {
                    targetHeight = (uint)(targetHeight * 0.7);
                    targetWidth = (uint)(targetWidth * 0.7);
                }
                else if (targetHeight >= 1000 & targetHeight < 2000)
                {
                    targetHeight = (uint)(targetHeight * 0.95);
                    targetWidth = (uint)(targetWidth * 0.95);
                }
            }
        }
    }
}
