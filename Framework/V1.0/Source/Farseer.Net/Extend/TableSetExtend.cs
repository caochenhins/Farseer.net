using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Core.Data.Table;

namespace FS.Extend
{
    public static class TableSetExtend
    {
        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        /// <param name="select">字段选择器</param>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.ToSelectList(0, select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="select">字段选择器</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, int top, Expression<Func<TEntity, T>> select) where TEntity : class, new()
        {
            return ts.Select(select).ToList(top).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, List<int> IDs, Expression<Func<TEntity, T>> select)
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
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public static List<T> ToSelectList<TEntity, T>(this TableSet<TEntity> ts, List<int> IDs, int top, Expression<Func<TEntity, T>> select)
            where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).ToSelectList(top, select);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static TEntity Update<TEntity>(this TableSet<TEntity> ts, int? ID, TEntity info) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).Update(info);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static TEntity Update<TEntity>(this TableSet<TEntity> ts, List<int> IDs, TEntity info) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).Update(info);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).IsHaving();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TEntity>(this TableSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).IsHaving();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static int Count<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TEntity>(this TableSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).Count();
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static TEntity ToInfo<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).ToInfo();
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNexTEntity<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousInfo<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this TableSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).ToList(0);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ts"></param>
        /// <param name="ID">o => o.ID == ID</param>
        public static void AddUp<TEntity, T>(this TableSet<TEntity> ts, int? ID, Expression<Func<TEntity, object>> select, T fieldValue)
            where T : struct
            where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.Where(o => o.ID == ID).AddUp(select, fieldValue);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">o => o.ID == ID</param>
        public static void AddUp<TEntity, T>(this TableSet<TEntity> ts, int? ID, T fieldValue)
            where T : struct
            where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.AddUp(ID, null, fieldValue);
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static void Delete<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.Where(o => o.ID == ID).Delete();
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static void Delete<TEntity>(this TableSet<TEntity> ts, List<int> IDs) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault())).Delete();
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static void Copy<TEntity>(this TableSet<TEntity> ts, Action<TEntity> acTEntity = null) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            var lst = ts.ToList();
            foreach (var info in lst)
            {
                info.ID = null;
                if (acTEntity != null) acTEntity(info);
                ts.Insert(info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="ID">o => o.ID == ID</param>
        public static void Copy<TEntity>(this TableSet<TEntity> ts, int? ID, Action<TEntity> act = null) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.Where(o => o.ID == ID);
            ts.Copy(act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        public static void Copy<TEntity>(this TableSet<TEntity> ts, List<int> IDs, Action<TEntity> act = null) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            ts.Where(o => IDs.Contains(o.ID.GetValueOrDefault()));
            ts.Copy(act);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="fieldName">筛选字段</param>
        /// <param name="defValue">不存在时默认值</param>
        public static T Value<TEntity, T>(this TableSet<TEntity> ts, int? ID, Expression<Func<TEntity, object>> fieldName, T defValue = default(T)) where TEntity : class, Core.Infrastructure.IEntity, new()
        {
            return ts.Where(o => o.ID == ID).Value(fieldName, defValue);
        }
    }
}
