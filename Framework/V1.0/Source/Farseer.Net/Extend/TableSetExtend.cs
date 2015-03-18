using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Core.Context;
using FS.Core.Data;

namespace FS.Extend
{
    public static class TableSetExtend
    {
        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.ToSelectList(0, select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, int top, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.Select(select).ToList(top).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, int top, Expression<Func<TEntity, T?>> select)
            where T : struct
            where TEntity : class, new()
        {
            return ts.Select(select).ToList(top).Select(select.Compile()).ToList().ConvertType<List<T>>();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">事务</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, List<int> IDs, Expression<Func<TEntity, T>> select) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID)).ToSelectList(select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">事务</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, List<int> IDs, int top, Expression<Func<TEntity, T>> select) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID)).ToSelectList(top, select);
        }
    }
}
