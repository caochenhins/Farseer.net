using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return ToList(lst.Where(o => IDs.Contains(o.ID)), pageSize, pageIndex);
        }
    }
}
