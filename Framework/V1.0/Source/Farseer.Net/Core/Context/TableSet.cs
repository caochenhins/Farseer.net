using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;
using FS.Extend;

namespace FS.Core.Context
{
    /// <summary>
    /// 表操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableSet<TEntity> : ViewSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TableContext<TEntity> _tableContext;
        private IQuery Query { get { return _tableContext.Query; } }
        protected override IQueryQueue QueryQueue { get { return _tableContext.Query.QueryQueue; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }

        internal TableSet(TableContext<TEntity> tableContext): this()
        {
            _tableContext = tableContext;
        }

        #region 条件
        public new TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            if (QueryQueue.ExpSelect == null) { QueryQueue.ExpSelect = new List<Expression>(); }
            if (select != null) { QueryQueue.ExpSelect.Add(select); }
            return this;
        }
        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public new TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            QueryQueue.ExpWhere = QueryQueue.ExpWhere == null ? QueryQueue.ExpWhere = where : Expression.Add(QueryQueue.ExpWhere, where);
            return this;
        }
        /// <summary>
        /// 倒序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="desc">字段选择器</param>
        public new TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            if (QueryQueue.ExpOrderBy == null) { QueryQueue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (desc != null) { QueryQueue.ExpOrderBy.Add(desc, false); }
            return this;
        }
        /// <summary>
        /// 正序查询
        /// </summary>
        /// <typeparam name="TKey">实体类属性类型</typeparam>
        /// <param name="asc">字段选择器</param>
        public new TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            if (QueryQueue.ExpOrderBy == null) { QueryQueue.ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (asc != null) { QueryQueue.ExpOrderBy.Add(asc, true); }
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
            if (QueryQueue.ExpAssign == null) { QueryQueue.ExpAssign = new Dictionary<Expression, object>(); }
            if (fieldName != null) { QueryQueue.ExpAssign.Add(fieldName, fieldValue); }
            return this;
        }
        #endregion

        #region 查询
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
    }
}
