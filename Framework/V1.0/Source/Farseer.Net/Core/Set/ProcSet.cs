using System;
using System.Collections.Generic;
using FS.Core.Context;
using FS.Core.Infrastructure;

namespace FS.Core.Set
{
    /// <summary>
    /// 存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ProcSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ProcContext<TEntity> _procContext;
        private IQueryProc Query { get { return _procContext.Query; } }
        private IQueueProc Queue { get { return _procContext.Query.Queue; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ProcSet() { }

        internal ProcSet(ProcContext<TEntity> procContext)
            : this()
        {
            _procContext = procContext;
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public T Value<T>(TEntity entity = null, T t = default(T))
        {
            Queue.SqlQuery<TEntity>().CreateParam(entity);
            return Queue.ExecuteValue(entity, t);
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public void Execute(TEntity entity = null)
        {
            Queue.SqlQuery<TEntity>().CreateParam(entity);
            Queue.Execute(entity);
        }

        /// <summary>
        /// 执行存储过程（返回单条记录）
        /// </summary>
        public TEntity ToInfo(TEntity entity = null)
        {
            Queue.SqlQuery<TEntity>().CreateParam(entity);
            return Queue.ExecuteInfo(entity);
        }

        /// <summary>
        /// 执行存储过程（返回多条记录）
        /// </summary>
        public List<TEntity> ToList(TEntity entity = null)
        {
            Queue.SqlQuery<TEntity>().CreateParam(entity);
            return Queue.ExecuteList(entity);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}