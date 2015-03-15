using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Extend
{
    public static class StringExtend
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
    }
}
