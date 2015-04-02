using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Core.Set;

namespace FS.Extend
{
    public static class ViewSetExtend
    {
        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        /// <param name="select">字段选择器</param>
        public static List<T> ToSelectList<TEntity, T>(this ViewSet<TEntity> ts, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.ToSelectList(0, select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="select">字段选择器</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this ViewSet<TEntity> ts, int top, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.Select(select).ToList(top).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this ViewSet<TEntity> ts, List<int> IDs, Expression<Func<TEntity, T>> select)
            where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).ToSelectList(select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="top">限制显示的数量</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this ViewSet<TEntity> ts, List<int> IDs, int top, Expression<Func<TEntity, T>> select)
            where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).ToSelectList(top, select);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).IsHaving();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TEntity>(this ViewSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).IsHaving();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static int Count<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TEntity>(this ViewSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).Count();
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static TEntity ToInfo<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).ToInfo();
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNexTEntity<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousInfo<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this ViewSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).ToList(0);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">ViewSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="fieldName">筛选字段</param>
        /// <param name="defValue">不存在时默认值</param>
        public static T Value<TEntity, T>(this ViewSet<TEntity> ts, int? ID, Expression<Func<TEntity, object>> fieldName, T defValue = default(T)) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).Value(fieldName, defValue);
        }
    }
}
