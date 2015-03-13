using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Core.Context
{
    public class TableSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TableContext<TEntity> _tableContext;

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
            _tableContext.Query.QueryQueue.ExpSelect = _tableContext.Query.QueryQueue.ExpSelect == null ? _tableContext.Query.QueryQueue.ExpSelect = select : Expression.Add(_tableContext.Query.QueryQueue.ExpSelect, select);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            //_tableContext.Query.QueryQueue.ExpWhere = _tableContext.Query.QueryQueue.ExpWhere == null ? _tableContext.Query.QueryQueue.ExpWhere = where : Expression.Add(_tableContext.Query.QueryQueue.ExpWhere, where);
            return this;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            //_tableContext.Query.QueryQueue.ExpOrderBy = _tableContext.Query.QueryQueue.ExpOrderBy == null ? _tableContext.Query.QueryQueue.ExpOrderBy = desc : Expression.Add(_tableContext.Query.QueryQueue.ExpOrderBy, desc);
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            //_tableContext.Query.QueryQueue.ExpOrderBy = _tableContext.Query.QueryQueue.ExpOrderBy == null ? _tableContext.Query.QueryQueue.ExpOrderBy = asc : Expression.Add(_tableContext.Query.QueryQueue.ExpOrderBy, asc);
            return this;
        }

        public TableSet<TEntity> Append(Expression<Func<TEntity, object>> fieldName, object fieldValue)
        {
            //_tableContext.Query.QueryQueue.ExpAssign = _tableContext.Query.QueryQueue.ExpAssign == null ? _tableContext.Query.QueryQueue.ExpAssign = asc : Expression.Add(_tableContext.Query.QueryQueue.ExpAssign, asc);
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
            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().ToList();
            return _tableContext.Query.QueryQueue.ExecuteList<TEntity>();
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <returns></returns>
        public TEntity ToInfo()
        {
            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().ToInfo();
            return _tableContext.Query.QueryQueue.ExecuteInfo<TEntity>();
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
                _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Update(entity);
                _tableContext.Query.Append();
            }
            else
            {
                _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Update(entity);
                _tableContext.Query.QueryQueue.Execute();
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
                _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Insert(entity);
                _tableContext.Query.Append();
            }
            else
            {
                _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Insert(entity);
                _tableContext.Query.QueryQueue.Execute();
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
                if (_tableContext.Database.DataType == Data.DataBaseType.SqlServer) { _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().BulkCopy(lst); }
                //else { _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Insert(lst); }

                _tableContext.Query.Append();
            }
            else
            {
                // 如果是MSSQLSER，则启用BulkCopy
                if (_tableContext.Database.DataType == Data.DataBaseType.SqlServer) { _tableContext.Query.QueryQueue.SqlQuery<TEntity>().BulkCopy(lst); }
                //else { _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Insert(lst); }

                _tableContext.Query.QueryQueue.Execute();
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
                _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Delete();
                _tableContext.Query.Append();
            }
            else
            {
                _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Delete();
                _tableContext.Query.QueryQueue.Execute();
            }
        }
        /// <summary>
        /// 查询数量
        /// </summary>
        public int Count()
        {
            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Count();
            return _tableContext.Query.QueryQueue.ExecuteQuery<int>();
        }
        /// <summary>
        /// 累计和
        /// </summary>
        public T Sum<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Sum操作时，fieldName参数不能为空！"); }
            _tableContext.Query.QueryQueue.ExpSelect = fieldName;

            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Sum();
            return _tableContext.Query.QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询最大数
        /// </summary>
        public T Max<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Max操作时，fieldName参数不能为空！"); }
            _tableContext.Query.QueryQueue.ExpSelect = fieldName;

            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Max();
            return _tableContext.Query.QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询最小数
        /// </summary>
        public T Min<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Min操作时，fieldName参数不能为空！"); }
            _tableContext.Query.QueryQueue.ExpSelect = fieldName;

            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Min();
            return _tableContext.Query.QueryQueue.ExecuteQuery<T>();
        }
        /// <summary>
        /// 查询单个值
        /// </summary>
        public T Value<T>(Expression<Func<TEntity, object>> fieldName)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName", "查询Value操作时，fieldName参数不能为空！"); }
            _tableContext.Query.QueryQueue.ExpSelect = fieldName;

            _tableContext.Query.QueryQueue.SqlQuery<TEntity>().Value();
            return _tableContext.Query.QueryQueue.ExecuteQuery<T>();
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
            if (_tableContext.Query.QueryQueue.ExpAssign == null) { throw new ArgumentNullException("ExpAssign", "+=字段操作时，必须先执行AddUp的另一个重载版本！"); }

            //  判断是否启用合并提交
            if (_tableContext.IsMergeCommand)
            {
                _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().AddUp();
                _tableContext.Query.Append();
            }
            else
            {
                _tableContext.Query.QueryQueue.SqlQuery<TEntity>().AddUp();
                _tableContext.Query.QueryQueue.Execute();
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
                _tableContext.Query.QueryQueue.LazyAct = () => _tableContext.Query.QueryQueue.SqlQuery<TEntity>().BulkCopy(lst);
                _tableContext.Query.Append();
            }
            else
            {
                _tableContext.Query.QueryQueue.SqlQuery<TEntity>().BulkCopy(lst);
                _tableContext.Query.QueryQueue.Execute();
            }
        }
        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
