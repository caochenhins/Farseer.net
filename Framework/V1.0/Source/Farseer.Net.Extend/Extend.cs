using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Linq;
using FS.Core;
using FS.Extend.Infrastructure;
using FS.Mapping.Context;

namespace FS.Extend
{
    /// <summary>
    ///     扩展工具
    /// </summary>
    public static class ExtendExtend
    {
        /// <summary>
        ///     将XML转成实体
        /// </summary>
        public static List<TEntity> ToList<TEntity>(this XElement element) where TEntity : class
        {
            var orm = CacheManger.GetFieldMap(typeof(TEntity));
            var list = new List<TEntity>();
            Type type;

            TEntity t;

            foreach (var el in element.Elements())
            {
                t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                //赋值字段
                foreach (var kic in orm.MapList)
                {
                    type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) { continue; }
                    if (kic.Value.PropertyExtend == eumPropertyExtend.Attribute)
                    {
                        if (el.Attribute(kic.Value.FieldAtt.Name) == null) { continue; }
                        kic.Key.SetValue(t, el.Attribute(kic.Value.FieldAtt.Name).Value.ConvertType(type), null);
                    }
                    else if (kic.Value.PropertyExtend == eumPropertyExtend.Element)
                    {
                        if (el.Element(kic.Value.FieldAtt.Name) == null) { continue; }
                        kic.Key.SetValue(t, el.Element(kic.Value.FieldAtt.Name).Value.ConvertType(type), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds">源DataSet</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this DataSet ds) where T : class,new()
        {
            return ds.Tables.Count == 0 ? null : ds.Tables[0].ToList<T>();
        }

        /// <summary>
        ///     扩展 Dictionary 根据Value反向查找Key的方法
        /// </summary>
        /// <param name="list">Dictionary对象</param>
        /// <param name="t2">键值</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static T1 GetKey<T1, T2>(this Dictionary<T1, T2> list, T2 t2)
        {
            foreach (var obj in list)
            {
                if (obj.Value.Equals(t2)) return obj.Key;
            }
            return default(T1);
        }

        /// <summary>
        ///     设置创建人信息
        /// </summary>
        /// <param name="createInfo">被赋值的实体</param>
        /// <param name="createID">创建人ID</param>
        /// <param name="createName">创建人名称</param>
        public static void SetCreateInfo(this ICreateInfo createInfo, int? createID = 0, string createName = "")
        {
            //createInfo.CreateIP = Req.GetIP();
            createInfo.CreateAt = DateTime.Now;
            createInfo.CreateID = createID;
            createInfo.CreateName = createName;
        }

        /// <summary>
        ///     设置修改人信息
        /// </summary>
        /// <param name="updateInfo">被赋值的实体</param>
        /// <param name="updateID">创建人ID</param>
        /// <param name="updateName">创建人名称</param>
        public static void SetUpdateInfo(this IUpdateInfo updateInfo, int? updateID = 0, string updateName = "")
        {
            //updateInfo.UpdateIP = Req.GetIP();
            updateInfo.UpdateAt = DateTime.Now;
            updateInfo.UpdateID = updateID;
            updateInfo.UpdateName = updateName;
        }

        /// <summary>
        ///     设置审核人信息
        /// </summary>
        /// <param name="auditInfo">被赋值的实体</param>
        /// <param name="auditID">创建人ID</param>
        /// <param name="auditName">创建人名称</param>
        public static void SetAuditInfo(this IAuditInfo auditInfo, int? auditID = 0, string auditName = "")
        {
            //auditInfo.AuditIP = Req.GetIP();
            auditInfo.AuditAt = DateTime.Now;
            auditInfo.AuditID = auditID;
            auditInfo.AuditName = auditName;
        }

        /// <summary>
        ///     Func 转换成 Predicate 对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="source">源Func对象</param>
        public static Predicate<T> ToPredicate<T>(this Func<T, bool> source) where T : class
        {
            return new Predicate<T>(source);
        }

        /// <summary>
        ///     转换成Json格式 x=x&x=x
        /// </summary>
        /// <param name="dic">Dictionary对象</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static string ToJson<T1, T2>(this Dictionary<T1, T2> dic)
        {
            var sb = new StringBuilder();
            foreach (var item in dic)
            {
                sb.Append(String.Format("{0}={1}&", item.Key, item.Value));
            }
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}