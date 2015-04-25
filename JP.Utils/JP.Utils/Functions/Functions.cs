using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace JP.Utils.Functions
{
    public static class Functions
    {
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(strIn, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
    }
}
