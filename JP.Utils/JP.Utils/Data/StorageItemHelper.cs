using JP.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JP.Utils.Data
{
    public class StorageFileHandleHelper
    {
        public static async Task<StorageFile> TryGetFile(StorageFolder folder,string filename)
        {
            if(folder==null || string.IsNullOrEmpty(filename))
            {
                return null;
            }

            try
            {
                var file = await folder.GetFileAsync(filename);
                return file;
            }
            catch(System.IO.FileNotFoundException)
            {
                return null;
            }
        }
    }
}
