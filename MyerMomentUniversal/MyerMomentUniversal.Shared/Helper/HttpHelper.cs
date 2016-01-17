#define WEB
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MyerMomentUniversal.Helper
{
    public class HttpHelper
    {
        private static string GetAllStylesRequestUrl = "http://121.41.21.21/moment/styles";

        public async static Task<JArray> GetAllStylesAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                var resp = await client.GetAsync(new Uri(GetAllStylesRequestUrl));
                if(resp.IsSuccessStatusCode)
                {
                    var content =await resp.Content.ReadAsStringAsync();
                    JObject job = JObject.Parse(content);
                    JArray array = job["styles"] as JArray;

                    return array;
                }
               else
                {
                    return default(JArray);
                }
            }
            catch(Exception)
            {
                return default(JArray);
            }
        }

    }
}
