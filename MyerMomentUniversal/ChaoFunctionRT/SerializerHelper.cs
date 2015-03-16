using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ChaoFunctionRT
{
    public class SerializerHelper
    {
        /// <summary>
        /// 序列化为JSON格式并保存在独立储存里
        /// 注意ImageSource无法序列化
        /// 需要在属性添加Attribute[IgnoreDataMember]来避免序列化ImageSource
        /// </summary>
        /// <typeparam name="T">需要序列化的类的类型</typeparam>
        /// <param name="objectToBeSer">被序列化的对象</param>
        /// <param name="fileName">要保存到独立储存的文件名</param>
 
        public async static Task<bool> SerializerToJSON<T>(object objectToBeSer, string fileName,bool isReplace)
        {
            try
            {
                T objecttojson = (T)objectToBeSer;
                string jsonString;
                using (var ms = new MemoryStream())
                {
                    new DataContractJsonSerializer(objecttojson.GetType()).WriteObject(ms, objecttojson);
                    jsonString = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                }

                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(fileName, (isReplace ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.GenerateUniqueName));
                await FileIO.WriteTextAsync(file, jsonString);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化获得的对象</typeparam>
        /// <param name="filename">储存在独立储存的文件名</param>
        /// <returns>返回反序列化后的对象，如果文件不存在，返回一个Object类型的对象</returns>
        public async static Task<T> DeSerializerFromJSON<T>(string filename)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(filename);
                string jsonString = await FileIO.ReadTextAsync(file);

                if (!String.IsNullOrEmpty(jsonString))
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                    {
                        T objectToReturn = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                        return objectToReturn;
                    }
                }
                else return default(T);
            }
            catch(Exception)
            {
                return default(T);
            }

        }
    }
}
