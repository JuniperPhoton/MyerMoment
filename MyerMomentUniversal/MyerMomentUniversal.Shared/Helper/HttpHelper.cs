
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MyerMomentUniversal.Helper
{
    public class HttpHelper
    {
        private static string SendDeviceInfoUri = "http://121.41.21.21/";
        private static Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();


        public async static Task<bool> SendDeviceInfo(string photoinfo)
        {
            string content = "";
#if WINDOWS_PHONE_APP
            content="?deviceInfo="+ deviceInfo.SystemFirmwareVersion+"&deviceName="+deviceInfo.FriendlyName+"&osVersion="+deviceInfo.OperatingSystem+ photoinfo;
#else
            content = "&device_name=" + deviceInfo.FriendlyName + "&os_version=" + deviceInfo.OperatingSystem + photoinfo;
#endif
            
            HttpClient client = new HttpClient();
            var response=await client.GetAsync(new Uri(SendDeviceInfoUri+content,UriKind.Absolute));
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;
        }



    }
}
