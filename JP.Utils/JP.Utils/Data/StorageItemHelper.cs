using JP.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JP.Utils.Data
{
    public class StorageItemHelper
    {
        public static StorageFile CheckFileExist(StorageFolder folder,string filename)
        {
            if(folder==null || string.IsNullOrEmpty(filename))
            {
                return default(StorageFile);
            }

            StorageFile fileToReturn = default(StorageFile);
            ExceptionHelper.TryExecute<StorageFile>(async() =>
            {
               var file=await folder.GetFileAsync(filename);
                fileToReturn = file;
            }, (e) =>
             {
                 fileToReturn = default(StorageFile);
             });

            return fileToReturn;

        }
    }
}
