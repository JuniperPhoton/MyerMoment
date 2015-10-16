using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiboSDKForWinRT;

namespace WeiboCSharpSample
{
    public class CmdFriendList : ICustomCmdBase
    {
        private string _uid = string.Empty;
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        private string _cursor = string.Empty;
        public string Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        private string _trimStatus = string.Empty;
        public string TrimStatus
        {
            get { return _trimStatus; }
            set { _trimStatus = value; }
        }

        private string count = string.Empty;
        public string Count
        {
            get { return count; }
            set { count = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/friendships/friends.json";
            request.Method = Method.GET;
            if (Uid.Length > 0)
            {
                request.AddParameter("uid", Uid);
            }
            if (Cursor.Length > 0)
            {
                request.AddParameter("cursor", Cursor);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
            if (TrimStatus.Length > 0)
                request.AddParameter("trim_status", TrimStatus);
        }
    }
}
