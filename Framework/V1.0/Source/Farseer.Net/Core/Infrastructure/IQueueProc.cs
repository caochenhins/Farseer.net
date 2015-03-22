using System;
using System.Collections.Generic;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueueProc : IQueue
    {
        /// <summary>
        /// 延迟执行SQL生成
        /// </summary>
        Action<IQueueProc> LazyAct { get; set; }

        ISqlQueryProc<TEntity> SqlQuery<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库
        /// </summary>
        int Execute<TEntity>(TEntity entity = null) where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回List<T>）
        /// </summary>
        List<TEntity> ExecuteList<TEntity>(TEntity entity = null) where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回Info）
        /// </summary>
        TEntity ExecuteInfo<TEntity>(TEntity entity = null) where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回T）
        /// </summary>
        T ExecuteValue<TEntity, T>(TEntity entity = null, T defValue = default(T)) where TEntity : class, new();
    }
}
