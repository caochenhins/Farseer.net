using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        private ViewQueue Queue { get { return _context.QueueManger.GetQueue(_name); } }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ViewSet() { }
        public ViewSet(ViewContext context, string name)
        {
            _context = context;
            _name = name;
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

        #region 查询
        /// <summary>
        /// 查询多条记录（不支持延迟加载）
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public List<TEntity> ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueueManger.SqlQuery<TEntity>(Queue).ToList(top, isDistinct, isRand);
            return QueueManger.ExecuteList<TEntity>(Queue);
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
            QueueManger.SqlQuery<TEntity>(Queue).ToList(pageSize, pageIndex, isDistinct);
            return QueueManger.ExecuteList<TEntity>(Queue);
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
            var queue = Queue;
            recordCount = Count();
            Queue.ExpOrderBy = queue.ExpOrderBy;
            Queue.ExpSelect = queue.ExpSelect;
            Queue.ExpWhere = queue.ExpWhere;
            return ToList(pageSize, pageIndex, isDistinct);
        }
        /// <summary>
        /// 查询单条记录（不支持延迟加载）
        /// </summary>
        public TEntity ToInfo()
        {
            QueueManger.SqlQuery<TEntity>(Queue).ToInfo();
            return QueueManger.ExecuteInfo<TEntity>(Queue);
        }
        /// <summary>
        /// 查询数量（不支持延迟加载）
        /// </summary>
        public int Count(bool isDistinct = false, bool isRand = false)
        {
            QueueManger.SqlQuery<TEntity>(Queue).Count();
            return QueueManger.ExecuteQuery<int>(Queue);
        }
        /// <summary>
        /// 查询数据是否存在（不支持延迟加载）
        /// </summary>
        public bool IsHaving()
        {
            return Count() > 0;
        }
        /// <summary>
        /// 累计和（不支持延迟加载）
        /// </summary>
        public T Sum<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Sum();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询最大数（不支持延迟加载）
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Max();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询最小数（不支持延迟加载）
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Min();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询单个值（不支持延迟加载）
        /// </summary>
        public T Value<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Value();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        #endregion
    }
}
