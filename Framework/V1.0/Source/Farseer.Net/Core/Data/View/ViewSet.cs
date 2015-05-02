using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Context;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 视图操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ViewSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ViewContext _context;

        private ViewQueueManger QueueManger { get { return _context.QueueManger; } }
        private ViewQueue Queue { get { return _context.QueueManger.GetQueue(_name, _map); } }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 实体类映射
        /// </summary>
        private readonly FieldMap _map;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ViewSet() { }
        public ViewSet(ViewContext context)
        {
            _context = context;
            _map = typeof(TEntity);
            var contextState = _context.ContextMap.GetState(this.GetType());
            _name = contextState.Value.SetAtt.Name;
        }

        #region 条件

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public ViewSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            if (Queue.ExpSelect == null) { Queue.ExpSelect = new List<Expression>(); }
            if (select != null) { Queue.ExpSelect.Add(select); }
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public ViewSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            Queue.ExpWhere = Queue.ExpWhere == null ? Queue.ExpWhere = where : Expression.Add(Queue.ExpWhere, where);
            return this;
        }
        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        private ViewSet<TEntity> Where<T>(Expression<Func<IEntity<T>, bool>> where)
        {
            Queue.ExpWhere = Queue.ExpWhere == null ? Queue.ExpWhere = where : Expression.Add(Queue.ExpWhere, where);
            return this;
        }

        /// <summary>
        /// 倒序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="desc">字段选择器</param>
        public ViewSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            if (Queue.ExpOrderBy == null) { Queue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (desc != null) { Queue.ExpOrderBy.Add(desc, false); }
            return this;
        }

        /// <summary>
        /// 正序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="asc">字段选择器</param>
        public ViewSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            if (Queue.ExpOrderBy == null) { Queue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (asc != null) { Queue.ExpOrderBy.Add(asc, true); }
            return this;
        }

        #endregion

        #region ToTable

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public DataTable ToTable(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueueManger.SqlBuilder(Queue).ToList(top, isDistinct, isRand);
            return QueueManger.ExecuteTable(Queue);
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <returns></returns>
        public DataTable ToTable(int pageSize, int pageIndex, bool isDistinct = false)
        {
            QueueManger.SqlBuilder(Queue).ToList(pageSize, pageIndex, isDistinct);
            return QueueManger.ExecuteTable(Queue);
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="recordCount">总记录数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public DataTable ToTable(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            var queue = Queue;
            recordCount = Count();
            Queue.ExpOrderBy = queue.ExpOrderBy;
            Queue.ExpSelect = queue.ExpSelect;
            Queue.ExpWhere = queue.ExpWhere;
            return ToTable(pageSize, pageIndex, isDistinct);
        }

        #endregion

        #region ToList

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public List<TEntity> ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            return ToTable(top, isDistinct, isRand).ToList<TEntity>();
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <returns></returns>
        public List<TEntity> ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            return ToTable(pageSize, pageIndex, isDistinct).ToList<TEntity>();
        }

        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="recordCount">总记录数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public List<TEntity> ToList(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            return ToTable(pageSize, pageIndex, out recordCount, isDistinct).ToList<TEntity>();
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        public List<TEntity> ToList<T>(List<T> lstIDs)
        {
            return Where<T>(o => lstIDs.Contains(o.ID)).ToList(0);
        }

        #endregion

        #region ToSelectList
        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        /// <param name="select">字段选择器</param>
        public List<T> ToSelectList<T>(Expression<Func<TEntity, T>> select)
        {
            return ToSelectList(0, select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="select">字段选择器</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public List<T> ToSelectList<T>(int top, Expression<Func<TEntity, T>> select)
        {
            return Select(select).ToList(top).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public List<T> ToSelectList<T>(List<T> lstIDs, Expression<Func<TEntity, T>> select)
        {
            return Where<T>(o => lstIDs.Contains(o.ID)).ToSelectList(select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="top">限制显示的数量</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">实体类的属性</typeparam>
        public List<T> ToSelectList<T>(List<T> lstIDs, int top, Expression<Func<TEntity, T>> select)
        {
            return Where<T>(o => lstIDs.Contains(o.ID)).ToSelectList(top, select);
        }
        #endregion

        #region ToEntity

        /// <summary>
        /// 查询单条记录（不支持延迟加载）
        /// </summary>
        public TEntity ToEntity()
        {
            QueueManger.SqlBuilder(Queue).ToEntity();
            return QueueManger.ExecuteInfo<TEntity>(Queue);
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        public TEntity ToEntity<T>(T ID)
        {
            return Where<T>(o => o.ID.Equals(ID)).ToEntity();
        }
        #endregion

        #region Count

        /// <summary>
        /// 查询数量（不支持延迟加载）
        /// </summary>
        public int Count(bool isDistinct = false, bool isRand = false)
        {
            QueueManger.SqlBuilder(Queue).Count();
            return QueueManger.ExecuteQuery<int>(Queue);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        public int Count<T>(T ID)
        {
            return Where<T>(o => o.ID.Equals(ID)).Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public int Count<T>(List<T> lstIDs)
        {
            return Where<T>(o => lstIDs.Contains(o.ID)).Count();
        }

        #endregion

        #region IsHaving

        /// <summary>
        /// 查询数据是否存在（不支持延迟加载）
        /// </summary>
        public bool IsHaving()
        {
            return Count() > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool IsHaving<T>(T ID)
        {
            return Where<T>(o => o.ID.Equals(ID)).IsHaving();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool IsHaving<T>(List<T> lstIDs)
        {
            return Where<T>(o => lstIDs.Contains(o.ID)).IsHaving();
        }

        #endregion

        #region GetValue

        /// <summary>
        /// 查询单个值（不支持延迟加载）
        /// </summary>
        public T GetValue<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlBuilder(Queue).GetValue();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }

        /// <summary>
        ///     查询单个值（不支持延迟加载）
        /// </summary>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <param name="fieldName">筛选字段</param>
        /// <param name="defValue">不存在时默认值</param>
        public T GetValue<T>(T ID, Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            return Where<T>(o => o.ID.Equals(ID)).GetValue(fieldName, defValue);
        }

        #endregion

        #region 聚合

        /// <summary>
        /// 累计和（不支持延迟加载）
        /// </summary>
        public T Sum<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlBuilder(Queue).Sum();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }

        /// <summary>
        /// 查询最大数（不支持延迟加载）
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlBuilder(Queue).Max();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询最小数（不支持延迟加载）
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlBuilder(Queue).Min();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }

        #endregion
    }
}
