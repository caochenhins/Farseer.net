using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FS.Extend
{
    public static partial class StringExtend
    {
        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static List<T> ToList<T>(this string str, T defValue, string splitString = ",")
        {
            var lst = new List<T>();
            if (string.IsNullOrWhiteSpace(str)) { return lst; }

            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (string.IsNullOrWhiteSpace(splitString))
            {
                for (var i = 0; i < str.Length; i++)
                {
                    lst.Add(str.Substring(i, 1).ConvertType(defValue));
                }
            }
            else
            {
                var strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(new string[1] { splitString }, StringSplitOptions.None);
                foreach (var item in strArray) { lst.Add(item.ConvertType(defValue)); }
            }
            return lst;
        }

        /// <summary>
        ///     删除指定最后的字符串
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="strChar">要删除的字符串</param>
        public static string DelEndOf(this string str, string strChar)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(strChar)) { return str; }
            var strLower = str.ToLower();
            var strCharLower = strChar.ToLower();

            if (strLower.EndsWith(strCharLower))
            {
                var index = strLower.LastIndexOf(strCharLower, StringComparison.Ordinal);
                if (index > -1) { str = str.Substring(0, index); }
            }
            return str;
        }

        /// <summary>
        ///     删除指定最后的字符串(直到找到为止)
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="strChar">要删除的字符串</param>
        public static string DelLastOf(this string str, string strChar)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(strChar)) { return str; }
            var index = str.LastIndexOf(strChar, StringComparison.Ordinal);
            return index > -1 ? str.Substring(0, index) : str;
        }

        /// <summary>
        ///     从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度(负数，则获取全部)</param>
        public static string SubString(this string str, int startIndex, int length = 0)
        {
            if (startIndex < 0) { return str; }
            if (str.Length <= startIndex) { return string.Empty; }
            if (length < 1) { return str.Substring(startIndex); }
            return str.Length < startIndex + length ? str.Substring(startIndex) : str.Substring(startIndex, length);
        }

        /// <summary>
        ///     截取到tag字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="tag">截取到的字符串</param>
        public static string SubString(this string str, string tag)
        {
            return str.IndexOf(tag) > -1 ? str.Substring(0, str.IndexOf(tag)) : str;
        }

        /// <summary>
        ///     比较两者是否相等，不考虑大小写,两边空格
        /// </summary>
        /// <param name="str">对比一</param>
        /// <param name="str2">对比二</param>
        /// <returns></returns>
        public static bool IsEquals(this string str, string str2)
        {
            if (str == str2)
            {
                return true;
            }
            if (str == null || str2 == null)
            {
                return false;
            }
            if (str.Trim().Length != str2.Trim().Length)
            {
                return false;
            }
            return String.Compare(str.Trim(), str2.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        ///     将字符串转换成Array型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static T[] ToArray<T>(this string str, T defValue, string splitString = ",")
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            var strArray = new string[str.Length];
            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (string.IsNullOrEmpty(splitString))
            {
                var c = str.ToCharArray();
                for (var i = 0; i < c.Length; i++)
                {
                    strArray[i] = c[i].ConvertType("");
                }
            }

            else
            {
                strArray = Regex.Split(str, Regex.Escape(splitString), RegexOptions.IgnoreCase);
            }

            var lst = new T[strArray.Length];

            for (var i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].IsType<T>())
                {
                    lst[i] = strArray[i].ConvertType(default(T));
                }
                else if (defValue != null)
                {
                    lst[i] = defValue;
                }
            }

            return lst;
        }
    }
}
