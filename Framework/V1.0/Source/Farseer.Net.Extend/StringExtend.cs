using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FS.Extend
{
    /// <summary>
    ///     String扩展工具
    /// </summary>
    public static class StringExtend
    {
        /// <summary>
        ///     指定清除标签的内容
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="tag">标签</param>
        /// <param name="options">选项</param>
        public static string ClearString(this string str, string tag, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) { return string.Empty; }
            return tag.IsNullOrEmpty() ? str : Regex.Replace(str, tag, "", options);
        }

        /// <summary>
        ///     指定清除标签的内容
        /// </summary>
        /// <param name="strs">内容</param>
        /// <param name="tag">标签</param>
        /// <param name="options">选项</param>
        public static string[] ClearString(this string[] strs, string tag, RegexOptions options = RegexOptions.None)
        {
            for (var i = 0; i < strs.Length; i++)
            {
                strs[i] = ClearString(strs[i], tag, options);
            }
            return strs;
        }

        /// <summary>
        ///     替换字符串
        /// </summary>
        /// <param name="str">数据源</param>
        /// <param name="tag">要搜索的字符串</param>
        /// <param name="newString">代替的数据</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public static string Replace(this string str, string tag, string newString, RegexOptions options)
        {
            return Regex.Replace(str, tag.Replace("|", "\\|"), newString, options);
        }

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
            if (splitString.Len() == 0)
            {
                for (var i = 0; i < str.Length; i++)
                {
                    lst.Add(str.Substring(i, 1).ConvertType(defValue));
                }
            }
            else
            {
                var strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(splitString);
                foreach (var item in strArray) { lst.Add(item.ConvertType(defValue)); }
            }
            return lst;
        }

        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static List<Enum> ToEnumList(this string str, Type type, string splitString = ",")
        {
            var lst = new List<Enum>();
            if (string.IsNullOrWhiteSpace(str)) { return lst; }

            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (splitString.Len() == 0)
            {
                for (var i = 0; i < str.Length; i++) { lst.Add((Enum)str.Substring(i, 1).ConvertType(type)); }
            }
            else
            {
                var strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(splitString);
                foreach (var item in strArray) { lst.Add((Enum)item.ConvertType(type)); }
            }
            return lst;
        }

        /// <summary>
        ///     是否不为Null或者Empty
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        public static bool IsHaving(this string str)
        {
            return str != null && str.Trim().Length > 0;
        }

        /// <summary>
        ///     是否为Null或者Empty或者空白字符
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        ///     对比开头字符是否一致
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="value">要对比的字符串</param>
        /// <returns></returns>
        public static bool IsStartsWith(this string str, string value)
        {
            return !string.IsNullOrWhiteSpace(str) && str.ToLower().StartsWith(value.ToLower());
        }

        /// <summary>
        ///     对比开头字符是否一致
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="value">要对比的字符串</param>
        /// <returns></returns>
        public static bool IsEndsWith(this string str, string value)
        {
            return !string.IsNullOrWhiteSpace(str) && str.ToLower().EndsWith(value.ToLower());
        }

        /// <summary>
        /// 获取json
        /// </summary>
        /// <param name="json">json字符串</param>
        public static Dictionary<TKey, TValue> GetJson<TKey, TValue>(this string json)
        {
            var dic = new Dictionary<TKey, TValue>();
            if (json.IsNullOrEmpty()) { return dic; }
            foreach (var item in json.Split('&'))
            {
                var s = item.Split('=');
                if (s.Length != 2) { continue; }
                var key = s[0].ConvertType<TKey>();
                var value = s[1].ConvertType<TValue>();

                dic[key] = value;
            }
            return dic;
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">对应的键值</param>
        public static string GetJsonValue(this string json, object key)
        {
            return GetJsonValue(json, key, string.Empty);
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">对应的键值</param>
        /// <param name="defValue">默认值</param>
        public static T GetJsonValue<T>(this string json, object key, T defValue)
        {
            if (json.IsNullOrEmpty()) { return defValue; }

            var reg = new Regex(string.Format(@"(?<=\b{0}\b=)[^&]*", key), RegexOptions.IgnoreCase);
            var match = reg.Match(json);
            if (match != null && match.Success)
            {
                return match.Value.ConvertType(defValue);
            }

            return defValue;
        }

        /// <summary>
        ///     获取json Key
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="value">对应的键值</param>
        public static string GetJsonKey(this string json, object value)
        {
            return GetJsonKey(json, value, string.Empty);
        }

        /// <summary>
        ///     获取json Key
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="value">对应的值</param>
        /// <param name="defValue">默认值</param>
        public static T GetJsonKey<T>(this string json, object value, T defValue)
        {
            if (json.IsNullOrEmpty()) { return defValue; }

            var reg = new Regex(string.Format(@"\b[^=^&]*\b(?=\b={0}\b)", value), RegexOptions.IgnoreCase);
            var match = reg.Match(json);
            if (match != null && match.Success)
            {
                return match.Value.ConvertType(defValue);
            }

            return defValue;
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="key">对应的键值</param>
        /// <param name="value">值</param>
        public static string SetJsonValue(this string json, object key, object value)
        {
            if (json.IsNullOrEmpty()) { json = string.Empty; }
            if (value == null) { value = string.Empty; }

            var type = value.GetType();
            if (type.Name.IsEquals("Nullable`1")) { type = Nullable.GetUnderlyingType(type); }
            if (value is Enum) { value = (int)value; }
            if (value is IList) { value = ((IList)value).ToString(","); }

            if (!json.IsNullOrEmpty())
            {
                var reg = new Regex(string.Format(@"\b{0}\b=[^&]*[&^&]*", key), RegexOptions.IgnoreCase);
                json = reg.Replace(json, "");
            }

            if (json.Length > 0 && !json.EndsWith("&")) { json += "&"; }
            return json + string.Format("{0}={1}", key, value);
        }

        /// <summary>
        ///     分隔字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="splitString">分隔符号</param>
        /// <returns></returns>
        public static string[] Split(this string str, string splitString = ",")
        {
            if (splitString.Len() == 1) { return str.Split(splitString[0]); }
            return str.Split(new string[1] { splitString }, StringSplitOptions.None);
        }

        /// <summary>
        ///     是否包括str2字段串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="str2">要包括的字符串</param>
        public static bool IsContains(this string str, string str2)
        {
            return str.IndexOf(str2, StringComparison.OrdinalIgnoreCase) > -1;
            //Regex regex = new Regex(str, RegexOptions.IgnoreCase);
            //return regex.IsMatch(str2);
        }

        /// <summary>
        ///     返回字符串的长度（用于SQL的比较）
        /// </summary>
        /// <param name="str">要比较的字段值</param>
        /// <returns></returns>
        public static int Len(this string str)
        {
            if (str == null) { return 0; }
            return str.Length;
        }

        /// <summary>
        /// 去除前后字符串
        /// </summary>
        /// <param name="source">要去除的字符源</param>
        /// <param name="leftIndex">开始位置</param>
        /// <param name="rightIndex">结束位置</param>
        public static string CutTrim(this string source, int leftIndex, int rightIndex = 0)
        {
            if (leftIndex > 0) { source = source.SubString(leftIndex); }
            if (rightIndex > 0) { return source.SubString(0, source.Length - rightIndex); }
            return source;
        }

        /// <summary>
        /// 去除前后字符串
        /// </summary>
        /// <param name="source">要去除的字符源</param>
        /// <param name="leftIndex">开始位置</param>
        /// <param name="rightStr">结束位置字符串</param>
        public static string CutTrim(this string source, int leftIndex, string rightStr = null)
        {
            if (leftIndex > 0) { source = source.SubString(leftIndex); }
            if (!string.IsNullOrWhiteSpace(rightStr)) { return source.SubString(0, source.LastIndexOf(rightStr)); }
            return source;
        }

        /// <summary>
        /// 去除前后字符串
        /// </summary>
        /// <param name="source">要去除的字符源</param>
        /// <param name="leftStr">开始位置字符串</param>
        /// <param name="rightStr">结束位置字符串</param>
        public static string CutTrim(this string source, string leftStr, string rightStr = null)
        {
            if (!string.IsNullOrWhiteSpace(leftStr)) { source = source.SubString(source.IndexOf(leftStr) + leftStr.Length); }
            if (!string.IsNullOrWhiteSpace(rightStr)) { return source.SubString(0, source.LastIndexOf(rightStr)); }
            return source;
        }

        /// <summary>
        /// 去除前后字符串
        /// </summary>
        /// <param name="source">要去除的字符源</param>
        /// <param name="leftStr">开始位置字符串</param>
        /// <param name="rightIndex">结束位置</param>
        public static string CutTrim(this string source, string leftStr, int rightIndex = 0)
        {
            if (!string.IsNullOrWhiteSpace(leftStr)) { source = source.SubString(source.IndexOf(leftStr) + leftStr.Length); }
            if (rightIndex > 0) { return source.SubString(0, source.Length - rightIndex); }
            return source;
        }

        /// <summary>
        ///     截取字符串长，超过指定长度，用tag代替。
        ///     一个汉字，长度为2
        /// </summary>
        /// <param name="source">要截取的字符串</param>
        /// <param name="length">截取长度(一个汉字，长度为2)</param>
        /// <param name="tag">超出长度时显示的字符</param>
        /// <param name="isAlwaysShowTag">如果没有超出，是否也显示</param>
        /// <returns></returns>
        public static string CutString(this string source, int length, string tag = "", bool isAlwaysShowTag = false)
        {
            var str = new StringBuilder();
            foreach (var item in source)
            {
                if (Encoding.Default.GetBytes(str.ToString()).Length >= length) { return str + tag; }
                str.Append(item);
            }
            if (isAlwaysShowTag) { str.Append(tag); }
            return str.ToString();
        }

        /// <summary>
        ///     当NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="str">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string WhileNullOrEmpty(this string str, string newString)
        {
            return string.IsNullOrWhiteSpace(str) ? newString : str;
        }

        /// <summary>
        ///     当不为NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="str">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string WhileNotNullOrEmpty(this string str, string newString)
        {
            return string.IsNullOrWhiteSpace(str) ? str : newString;
        }

        /// <summary>
        /// 判断ID是否存在于str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool Contains(this string str, int? ID)
        {
            return ("," + str + ",").Contains("," + ID.ToString() + ",");
            //return str.ToList(0).Contains(ID);
        }

        /// <summary>
        /// 迭代字符
        /// </summary>
        /// <param name="str">要迭代的字符串</param>
        /// <param name="count">次数</param>
        public static string For(this string str, int count = 1)
        {
            var strs = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                strs.Append(str);
            }
            return strs.ToString();
        }
    }
}