using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;
using FS.Extend;

namespace FS.Core.Context
{
    public class TableSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TableContext<TEntity> _tableContext;
        private IQuery Query { get { return _tableContext.Query; } }
        private IQueryQueue QueryQueue { get { return _tableContext.Query.QueryQueue; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }

        internal TableSet(TableContext<TEntity> tableContext)
            : this()
        {
            _tableContext = tableContext;
        }

        #region 条件

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            if (QueryQueue.ExpSelect == null) { QueryQueue.ExpSelect = new List<Expression>(); }
            if (select != null) { QueryQueue.ExpSelect.Add(select); }
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            QueryQueue.ExpWhere = QueryQueue.ExpWhere == null ? QueryQueue.ExpWhere = where : Expression.Add(QueryQueue.ExpWhere, where);
            return this;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            if (QueryQueue.ExpOrderBy == null) { QueryQueue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (desc != null) { QueryQueue.ExpOrderBy.Add(desc, false); }
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            if (QueryQueue.ExpOrderBy == null) { QueryQueue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (asc != null) { QueryQueue.ExpOrderBy.Add(asc, true); }
            return this;
        }

        public TableSet<TEntity> Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            if (QueryQueue.ExpAssign == null) { QueryQueue.ExpAssign = new Dictionary<Expression, object>(); }
            if (fieldName != null) { QueryQueue.ExpAssign.Add(fieldName, fieldValue); }
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
            QueryQueue.SqlQuery<TEntity>().ToList(top, isDistinct, isRand);
            return QueryQueue.ExecuteList<TEntity>();
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
            QueryQueue.SqlQuery<TEntity>().ToList(pageSize, pageIndex, isDistinct);
            return QueryQueue.ExecuteList<TEntity>();
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
            var queue = QueryQueue;
            recordCount = Count();
            QueryQueue.ExpOrderBy = queue.ExpOrderBy;
            QueryQueue.ExpSelect = queue.ExpSelect;
            QueryQueue.ExpWhere = queue.ExpWhere;
            return ToList(pageSize, pageIndex, isDistinct);
        }
        /// <summary>
        /// 查询单条记录（不支持延迟加载）
        /// </summary>
        public TEntity ToInfo()
        {
            QueryQueue.SqlQuery<TEntity>().ToInfo();
            return QueryQueue.ExecuteInfo<TEntity>();
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
            if (_tableContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().Update(entity);
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().Update(entity);
                QueryQueue.Execute();
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
            if (_tableContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().Insert(entity);
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().Insert(entity);
                QueryQueue.Execute();
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

            QueryQueue.SqlQuery<TEntity>().InsertIdentity(entity);
            identity = QueryQueue.ExecuteQuery<int>();

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
            if (_tableContext.Database.DataType == Data.DataBaseType.SqlServer)
            {
                _tableContext.Database.ExecuteSqlBulkCopy(_tableContext.TableName, lst.ToTable());
                return lst;
            }
            lst.ForEach(entity =>
            {
                QueryQueue.SqlQuery<TEntity>().Insert(entity);
                QueryQueue.Execute();
            });
            return lst;
        }
        /// <summary>
        /// 删除（支持延迟加载）
        /// </summary>
        public void Delete()
        {
            //  判断是否启用合并提交
            if (_tableContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().Delete();
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().Delete();
                QueryQueue.Execute();
            }
        }
        /// <summary>
        /// 查询数量（不支持延迟加载）
        /// </summary>
        public int Count(bool isDistinct = false, bool isRand = false)
        {
            QueryQueue.SqlQuery<TEntity>().Count();
            return QueryQueue.ExecuteQuery<int>();
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

            QueryQueue.SqlQuery<TEntity>().Sum();
            return QueryQueue.ExecuteQuery(defValue);
        }
        /// <summary>
        /// 查询最大数（不支持延迟加载）
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Max();
            return QueryQueue.ExecuteQuery(defValue);
        }
        /// <summary>
        /// 查询最小数（不支持延迟加载）
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Min();
            return QueryQueue.ExecuteQuery(defValue);
        }
        /// <summary>
        /// 查询单个值（不支持延迟加载）
        /// </summary>
        public T Value<T>(Expression<Func<TEntity, object>> fieldName, T defValue = default(T))
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Value();
            return QueryQueue.ExecuteQuery(defValue);
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
        public void AddUp()
        {
            if (QueryQueue.ExpAssign == null) { throw new ArgumentNullException("ExpAssign", "+=字段操作时，必须先执行AddUp的另一个重载版本！"); }

            //  判断是否启用合并提交
            if (_tableContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().AddUp();
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().AddUp();
                QueryQueue.Execute();
            }
        }
        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
