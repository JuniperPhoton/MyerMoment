using System;
using System.Collections.Generic;
using System.Text;

namespace MyerMomentUniversal.Helper
{
    public enum ImageSaveResult
    {
        Successful=0,
        FailToGetPixels=1,
        FailToFlush=2,
        FileNotOpen=3
    }
}
