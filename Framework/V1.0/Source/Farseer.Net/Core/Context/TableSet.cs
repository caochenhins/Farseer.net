using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

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
            _tableContext.Query = DbFactory.CreateQuery(_tableContext);
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

        public TableSet<TEntity> Append(Expression<Func<TEntity, object>> fieldName, object fieldValue)
        {
            // QueryQueue.ExpAssign = QueryQueue.ExpAssign == null ? QueryQueue.ExpAssign = asc : Expression.Add(QueryQueue.ExpAssign, asc);
            return this;
        }

        #endregion

        #region 查询
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <returns></returns>
        public List<TEntity> ToList()
        {
            QueryQueue.SqlQuery<TEntity>().ToList();
            return QueryQueue.ExecuteList<TEntity>();
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <returns></returns>
        public TEntity ToInfo()
        {
            QueryQueue.SqlQuery<TEntity>().ToInfo();
            return QueryQueue.ExecuteInfo<TEntity>();
        }
        /// <summary>
        /// 修改
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
        /// 插入
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
        /// 插入
        /// </summary>
        /// <param name="lst"></param>
        public List<TEntity> Insert(List<TEntity> lst)
        {
            if (lst == null) { throw new ArgumentNullException("lst", "插入操作时，lst参数不能为空！"); }
            //  判断是否启用合并提交
            if (_tableContext.IsMergeCommand)
            {
                // 如果是MSSQLSER，则启用BulkCopy
                if (_tableContext.Database.DataType == Data.DataBaseType.SqlServer) { QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().BulkCopy(lst); }
                //else { QueryQueue.LazyAct = () => QueryQueue.SqlQuery<TEntity>().Insert(lst); }

                Query.Append();
            }
            else
            {
                // 如果是MSSQLSER，则启用BulkCopy
                if (_tableContext.Database.DataType == Data.DataBaseType.SqlServer) { QueryQueue.SqlQuery<TEntity>().BulkCopy(lst); }
                //else { QueryQueue.SqlQuery<TEntity>().Insert(lst); }

                QueryQueue.Execute();
            }
            return lst;
        }
        /// <summary>
        /// 删除
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
        /// 查询数量
        /// </summary>
        public int Count()
        {
            QueryQueue.SqlQuery<TEntity>().Count();
            return QueryQueue.ExecuteQuery<int>();
        }
        /// <summary>
        /// 累计和
        /// </summary>
        public T Sum<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Sum();
            return QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询最大数
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Max();
            return QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询最小数
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Min();
            return QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询单个值
        /// </summary>
        public T Value<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            Select(fieldName);

            QueryQueue.SqlQuery<TEntity>().Value();
            return QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 添加或者减少某个字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public void AddUp(Expression<Func<TEntity, object>> fieldName, object fieldValue)
        {
            Append(fieldName, fieldValue).AddUp();
        }
        /// <summary>
        /// 添加或者减少某个字段
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
        /// <summary>
        /// 使用数据库特性进行大批量插入操作
        /// </summary>
        public void BulkCopy(List<TEntity> lst)
        {
            if (lst == null || lst.Count == 0) { throw new ArgumentNullException("lst", "指添加操作时，lst参数不能为空！"); }

            //  判断是否启用合并提交
            if (_tableContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().BulkCopy(lst);
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().BulkCopy(lst);
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
