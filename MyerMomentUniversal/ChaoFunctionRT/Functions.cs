using System.Text.RegularExpressions;

namespace ChaoFunctionRT
{
    public class Functions
    {
        /// <summary>
        /// 邮箱判定是否为真
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(strIn, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
    }
}
