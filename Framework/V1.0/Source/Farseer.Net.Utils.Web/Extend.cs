using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FS.Core.Context;
using FS.Core.Infrastructure;
using FS.Core.Set;
using FS.Utils.WebForm.Repeater;

namespace FS.Extend
{
    public static partial class Extend
    {
        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="rpt">Repeater</param>
        /// <returns></returns>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, Repeater rpt)
        {
            rpt.PageCount = lst.Count();
            return lst.ToList(rpt.PageSize, rpt.PageIndex);
        }

        /// <summary>
        ///     IEnumerable绑定到Repeater
        /// </summary>
        /// <param name="rpt">QynRepeater</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="lst">IEnumerable</param>
        public static void Bind(this Repeater rpt, IEnumerable lst, int recordCount = -1)
        {
            rpt.DataSource = lst;
            rpt.DataBind();

            if (recordCount > -1)
            {
                rpt.PageCount = recordCount;
            }
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="rpt">Repeater</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> IDs, Repeater rpt) where TEntity : IEntity
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID.GetValueOrDefault())), rpt);
        }

        /// <summary>
        ///     通用的分页方法(多条件)
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <param name="rpt">Repeater带分页控件</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this TableSet<TEntity> ts, Repeater rpt) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            int recordCount;
            var lst = ts.ToList(rpt.PageSize, rpt.PageIndex, out recordCount);
            rpt.PageCount = recordCount;

            return lst;
        }
    }
}
