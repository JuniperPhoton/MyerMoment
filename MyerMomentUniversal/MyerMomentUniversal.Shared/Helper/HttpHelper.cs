
using MyerMomentUniversal.Model;
using Newtonsoft.Json.Linq;
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
        private static string GetAllStylesRequestUrl = "http://127.0.0.1:3000/styles";

        public static Uri GetUri(string name, string extwithdot)
        {
            return new Uri("http://121.41.21.21/moment/styles/" + name + extwithdot);
        }

        public async static Task<string[]> GetAllStylesAsync()
        {
            HttpClient client = new HttpClient();
            var stylesStr = await client.GetStringAsync(new Uri(GetAllStylesRequestUrl));
            var styles = stylesStr.Split(',');
            return styles;
        }

    }
}
