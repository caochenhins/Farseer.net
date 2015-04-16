using System;
using System.Collections.Generic;
using FS.Mapping.Verify;

namespace FS.Extend
{
    public static partial class ModelExtend
    {
        /// <summary>
        ///     查找对象属性值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static T GetValue<TInfo, T>(this TInfo info, string propertyName, T defValue = default(T)) where TInfo : class
        {
            if (info == null) { return defValue; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanRead) { return defValue; }
                return property.GetValue(info, null).ConvertType(defValue);
            }
            return defValue;
            //if (info == null) { return defValue; }
            //Mapping Map = typeof(TInfo);
            //var lstModel = Map.ModelList.Where(o => o.Key.Name.IsEquals(propertyName));
            //return lstModel.Count() == 0 ? defValue : lstModel.FirstOrDefault().Key.GetValue(info, null).ConvertType(defValue);
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static void SetValue<TInfo>(this TInfo info, string propertyName, object objValue) where TInfo : class
        {
            if (info == null) { return; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanWrite) { return; }
                property.SetValue(info, objValue.ConvertType(property.PropertyType), null);
            }

            //if (info == null) { return; }
            //Mapping Map = typeof(TInfo);
            //var lstModel = Map.ModelList.Where(o => o.Key.Name.IsEquals(propertyName));
            //if (lstModel.Count() == 0) { return; }
            //lstModel.FirstOrDefault().Key.SetValue(info, objValue, null);
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        /// <param name="info">任意对象</param>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        public static TInfo TestData<TInfo>(this TInfo info, int subCount = 10)
        {
            return TestData(info, 1, subCount);
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        /// <param name="level">防止无限层</param>
        private static TInfo TestData<TInfo>(this TInfo info, int level, int subCount)
        {
            var type = info.GetType();
            foreach (var item in type.GetProperties())
            {
                if (!item.CanWrite) { continue; }

                var argumType = item.PropertyType;
                // 去掉Nullable的类型
                if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) { argumType = argumType.GetGenericArguments()[0]; }

                try
                {
                    // 对   List 类型处理
                    if (item.PropertyType.IsGenericType)
                    {
                        // 动态构造List
                        var objLst = Activator.CreateInstance(item.PropertyType);
                        // List元素
                        var objItem = Activator.CreateInstance(argumType.GetGenericArguments()[0]);

                        // 防止无限层递归
                        if (level < 3)
                        {
                            for (var i = 0; i < subCount; i++)
                            {
                                item.PropertyType.GetMethod("Add").Invoke(objLst, new[] { TestData(objItem, level + 1, subCount) });
                            }
                        }
                        item.SetValue(info, objLst, null);
                        continue;
                    }
                    // 普通成员
                    item.SetValue(info, TestData(argumType), null);
                }
                catch
                {

                }
            }
            return info;
        }

        /// <summary>
        /// 返回基本类型的数据
        /// </summary>
        /// <param name="argumType"></param>
        /// <returns></returns>
        public static object TestData(Type argumType)
        {
            var rand = new Random(DateTime.Now.Second);

            var val = new object();
            // 对   List 类型处理
            if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return TestData(argumType.GetGenericArguments()[0]); }
            switch (Type.GetTypeCode(argumType))
            {
                case TypeCode.Boolean: val = rand.Next(0, 1) == 0; break;
                case TypeCode.DateTime: val = new DateTime(rand.Next(2000, DateTime.Now.Year), rand.Next(1, 12), rand.Next(1, 30), rand.Next(0, 23), rand.Next(0, 59), rand.Next(0, 59)); break;
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64: val = rand.Next(255); break;
                default: val = "测试数据"; break;
            }
            return val.ConvertType(argumType);
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TInfo>(this TInfo info, Action<string, string> tip = null, string url = "") where TInfo : IVerification
        {
            if (info == null) { return false; }
            //if (tip == null) { tip = new Terminator().Alert; }
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result)
            {
                var lst = new List<string>();
                foreach (var item in dicError) { lst.AddRange(item.Value); }

                tip(lst.ToString("<br />"), url);
            }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TInfo>(this TInfo info, Action<Dictionary<string, List<string>>> tip) where TInfo : IVerification
        {
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result)
            {
                tip(dicError);
            }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；vakue：错误消息</param>
        /// <param name="info">要检测的实体</param>
        public static bool Check<TEntity>(this TEntity info, out Dictionary<string, List<string>> dicError) where TEntity : IVerification
        {
            dicError = new Dictionary<string, List<string>>();
            var map = VerifyMapCache.GetMap(typeof(TEntity));
            foreach (var kic in map.ModelList)
            {
                var lstError = new List<string>();
                var value = kic.Key.GetValue(info, null);

                // 是否必填
                if (kic.Value.Required != null && !kic.Value.Required.IsValid(value))
                {
                    lstError.Add(kic.Value.Required.ErrorMessage);
                    //dicError.Add(kic.Key.Name, lstError);
                }

                //if (value == null) { continue; }

                // 字符串长度判断
                if (kic.Value.StringLength != null && !kic.Value.StringLength.IsValid(value))
                {
                    lstError.Add(kic.Value.StringLength.ErrorMessage);
                }

                // 值的长度
                if (kic.Value.Range != null && !kic.Value.Range.IsValid(value))
                {
                    lstError.Add(kic.Value.Range.ErrorMessage);
                }

                // 正则
                if (kic.Value.RegularExpression != null && !kic.Value.RegularExpression.IsValid(value))
                {
                    lstError.Add(kic.Value.RegularExpression.ErrorMessage);
                }

                if (lstError.Count > 0)
                {
                    dicError.Add(kic.Key.Name, lstError);
                }
            }
            return dicError.Count == 0;
        }
    }
}
