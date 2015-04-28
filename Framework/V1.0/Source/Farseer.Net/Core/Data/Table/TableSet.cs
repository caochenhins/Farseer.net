using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Extend;
using System.Data;

namespace FS.Core.Data.Table
{
    /// <summary>
    /// 表操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class TableSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TableContext _context;
        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        private readonly string _name;

        private TableQueueManger QueueManger { get { return _context.QueueManger; } }
        private TableQueue Queue { get { return _context.QueueManger.GetQueue(_name); } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }
        public TableSet(TableContext context, string name)
        {
            _context = context;
            _name = name;
        }

        #region 条件
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            if (Queue.ExpSelect == null) { Queue.ExpSelect = new List<Expression>(); }
            if (select != null) { Queue.ExpSelect.Add(select); }
            return this;
        }
        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            Queue.ExpWhere = Queue.ExpWhere == null ? Queue.ExpWhere = where : Expression.Add(Queue.ExpWhere, where);
            return this;
        }
        /// <summary>
        /// 倒序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="desc">字段选择器</param>
        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
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
        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            if (Queue.ExpOrderBy == null) { Queue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (asc != null) { Queue.ExpOrderBy.Add(asc, true); }
            return this;
        }
        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            if (Queue.ExpAssign == null) { Queue.ExpAssign = new Dictionary<Expression, object>(); }
            if (fieldName != null) { Queue.ExpAssign.Add(fieldName, fieldValue); }
            return this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> Append<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue)
            where T : struct
        {
            if (Queue.ExpAssign == null) { Queue.ExpAssign = new Dictionary<Expression, object>(); }
            if (fieldName != null) { Queue.ExpAssign.Add(fieldName, fieldValue); }
            return this;
        }

        /// <summary>
        /// 字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> Append<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue)
            where T : struct
        {
            if (Queue.ExpAssign == null) { Queue.ExpAssign = new Dictionary<Expression, object>(); }
            if (fieldName != null) { Queue.ExpAssign.Add(fieldName, fieldValue); }
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
        public DataTable ToTable(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueueManger.SqlQuery<TEntity>(Queue).ToList(top, isDistinct, isRand);
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
            QueueManger.SqlQuery<TEntity>(Queue).ToList(pageSize, pageIndex, isDistinct);
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
        public T Sum<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Sum();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询最大数（不支持延迟加载）
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Max();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询最小数（不支持延迟加载）
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).Min();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }
        /// <summary>
        /// 查询单个值（不支持延迟加载）
        /// </summary>
        /// <param name="fieldName">筛选字段</param>
        /// <param name="defValue">不存在时默认值</param>
        public T GetValue<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueueManger.SqlQuery<TEntity>(Queue).GetValue();
            return QueueManger.ExecuteQuery(Queue, defValue);
        }

        /// <summary>
        /// 修改（支持延迟加载）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Update(TEntity entity)
        {
            if (entity == null) { throw new ArgumentNullException("entity", "更新操作时，参数不能为空！"); }

            //  判断是否启用合并提交
            if (_context.IsMergeCommand)
            {
                Queue.LazyAct = (queryQueue) => QueueManger.SqlOper<TEntity>(queryQueue).Update(entity);
                QueueManger.Append();
            }
            else
            {
                QueueManger.SqlOper<TEntity>(Queue).Update(entity);
                QueueManger.Execute(Queue);
            }
            return entity;
        }
        /// <summary>
        /// 插入（支持延迟加载）
        /// </summary>
        /// <param name="entity"></param>
        public TEntity Insert(TEntity entity)
        {
            if (entity == null) { throw new ArgumentNullException("entity", "插入操作时，参数不能为空！"); }
            //  判断是否启用合并提交
            if (_context.IsMergeCommand)
            {
                Queue.LazyAct = (queryQueue) => QueueManger.SqlOper<TEntity>(queryQueue).Insert(entity);
                QueueManger.Append();
            }
            else
            {
                QueueManger.SqlOper<TEntity>(Queue).Insert(entity);
                QueueManger.Execute(Queue);
            }
            return entity;
        }
        /// <summary>
        /// 插入（不支持延迟加载）
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="identity">返回新增的</param>
        public TEntity Insert(TEntity entity, out int identity)
        {
            if (entity == null) { throw new ArgumentNullException("entity", "插入操作时，参数不能为空！"); }

            QueueManger.SqlOper<TEntity>(Queue).InsertIdentity(entity);
            identity = QueueManger.ExecuteQuery<int>(Queue);

            return entity;
        }
        /// <summary>
        /// 插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"></param>
        public List<TEntity> Insert(List<TEntity> lst)
        {
            if (lst == null) { throw new ArgumentNullException("lst", "插入操作时，lst参数不能为空！"); }

            // 如果是MSSQLSER，则启用BulkCopy
            if (QueueManger.DataBase.DataType == Data.DataBaseType.SqlServer)
            {
                QueueManger.DataBase.ExecuteSqlBulkCopy(_name, lst.ToTable());
                return lst;
            }
            lst.ForEach(entity =>
            {
                QueueManger.SqlOper<TEntity>(Queue).Insert(entity);
                QueueManger.Execute(Queue);
            });
            return lst;
        }
        /// <summary>
        /// 删除（支持延迟加载）
        /// </summary>
        public void Delete()
        {
            //  判断是否启用合并提交
            if (_context.IsMergeCommand)
            {
                Queue.LazyAct = (queryQueue) => QueueManger.SqlOper<TEntity>(queryQueue).Delete();
                QueueManger.Append();
            }
            else
            {
                QueueManger.SqlOper<TEntity>(Queue).Delete();
                QueueManger.Execute(Queue);
            }
        }
        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public void AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            Append(fieldName, fieldValue).AddUp();
        }

        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public void AddUp< T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue)
            where T : struct
        {
            Append(fieldName, fieldValue).AddUp();
        }
        /// <summary>
        /// 添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public void AddUp()
        {
            if (Queue.ExpAssign == null) { throw new ArgumentNullException("ExpAssign", "+=字段操作时，必须先执行AddUp的另一个重载版本！"); }

            //  判断是否启用合并提交
            if (_context.IsMergeCommand)
            {
                Queue.LazyAct = (queryQueue) => QueueManger.SqlOper<TEntity>(queryQueue).AddUp();
                QueueManger.Append();
            }
            else
            {
                QueueManger.SqlOper<TEntity>(Queue).AddUp();
                QueueManger.Execute(Queue);
            }
        }
        #endregion
    }
}
