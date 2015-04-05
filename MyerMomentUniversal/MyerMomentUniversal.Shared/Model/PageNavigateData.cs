using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace MyerMomentUniversal.Model
{
    public class PageNavigateData
    {
        public StorageFile file;
        public bool isFromShare;

        public PageNavigateData()
        {
            file = null;
            isFromShare = false;
        }

        public PageNavigateData(StorageFile file,bool isFromShare)
        {
            this.file = file;
            this.isFromShare = isFromShare;
        }
    }
}
