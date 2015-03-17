using System.Collections.Generic;
using System.Web;
using FS.Extend;

namespace FS.Utils
{
    /// <summary>
    ///     解释Url
    /// </summary>
    public partial class Url
    {
        /// <summary>
        ///     对Url字符，进去编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     对Url字符，进去解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     参数编码
        /// </summary>
        public static string ParmsEncode(string parms)
        {
            var lstParms = new List<string>();
            foreach (var strs in parms.Split('&'))
            {
                var index = strs.IndexOf('=');
                if (index > -1)
                {
                    lstParms.Add(strs.SubString(0, index + 1) + UrlEncode(strs.SubString(index + 1, -1)));
                }
            }
            return lstParms.ToString("&");
        }
    }
}