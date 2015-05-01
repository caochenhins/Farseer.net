using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FS.Core;
using FS.Core.Infrastructure;

namespace FS.Extend
{
    public static partial class ListExtend
    {
        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(this IEnumerable lst, string sign = ",")
        {
            if (lst == null) { return String.Empty; }

            var str = new StringBuilder();
            foreach (var item in lst) { str.Append(item + sign); }
            return str.ToString().DelEndOf(sign);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, int pageSize, int pageIndex = 1)
        {
            if (pageSize == 0) { return lst.ToList(); }

            #region 计算总页数

            var allCurrentPage = 0;
            var recordCount = lst.Count();
            if (pageIndex < 1) { pageIndex = 1; return lst.Take(pageSize).ToList(); }
            if (pageSize < 1) { pageSize = 10; }

            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }

            #endregion

            return lst.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs, int pageSize, int pageIndex = 1) where TInfo : IEntity
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID.GetValueOrDefault())), pageSize, pageIndex);
        }
        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst">集合</param>
        /// <returns></returns>
        public static DataTable ToTable<TInfo>(this List<TInfo> lst) where TInfo : class
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = CacheManger.GetFieldMap(lst[0].GetType());
            var lstFields = map.MapList.Where(o => o.Value.FieldAtt.IsMap);
            foreach (var field in lstFields)
            {
                var type = field.Key.PropertyType;
                // 对   List 类型处理
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
                dt.Columns.Add(field.Value.FieldAtt.Name, type);
            }

            foreach (var info in lst)
            {
                dt.Rows.Add(dt.NewRow());
                foreach (var field in lstFields)
                {
                    var value = info.GetValue(field.Key.Name, (object)null);
                    if (value == null) { continue; }
                    if (!dt.Columns.Contains(field.Value.FieldAtt.Name)) { dt.Columns.Add(field.Value.FieldAtt.Name); }
                    dt.Rows[dt.Rows.Count - 1][field.Value.FieldAtt.Name] = value;
                }
            }
            return dt;
        }
    }
}
