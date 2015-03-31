using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ChaoFunctionRT
{
    public class ExceptionUtils
    {
        /// <summary>
        /// 写入独立储存，文件名为record.log
        /// </summary>
        /// <param name="e">EX</param>
        /// <param name="content">附带的信息</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        public async static Task<bool> WriteRecord(Exception e,string content="")
        {
            try
            {
                var localfolder = ApplicationData.Current.LocalFolder;
                var file = await localfolder.CreateFileAsync("recored.log", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, "EXCEPTION:" + Environment.NewLine + e.Message + Environment.NewLine + "MESSAGE:" + Environment.NewLine + content);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读出独立储存的文件
        /// </summary>
        /// <returns>返回读出的字符串，如果出错则返回NULL</returns>
        public async static Task<string> ReadRecord()
        {
            try
            {
                var localfolder = ApplicationData.Current.LocalFolder;
                var file = await localfolder.GetFileAsync("recored.log");
                string text=await FileIO.ReadTextAsync(file,UnicodeEncoding.Utf8);
                if(!String.IsNullOrEmpty(text))
                {
                    return text;
                }
                else
                {
                    return "";
                }
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
