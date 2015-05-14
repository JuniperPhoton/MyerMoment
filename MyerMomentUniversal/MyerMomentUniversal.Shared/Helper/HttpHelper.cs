#define WEB
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
        private static string BaseUri = "http://121.41.21.21/ghost/MyerMoment/public/images/Styles/";
        private static string GetAllStylesRequestUrl = "http://121.41.21.21/moment/styles";
        private static string StyleDir = "http://121.0.0.1:3000/public/images/styles/";


        private static string DeployUriBase =
#if WEB
            BaseUri;
#else
            StyleDir;
#endif


        public static Uri GetUri(string name)
        {
            return new Uri(DeployUriBase + name);
        }

        public async static Task<string[]> GetAllStylesAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                var stylesStr = await client.GetStringAsync(new Uri(GetAllStylesRequestUrl));
                var styles = stylesStr.Split(',');
                return styles;
            }
            catch(Exception)
            {
                return new string[0] { };
            }
        }

    }
}
