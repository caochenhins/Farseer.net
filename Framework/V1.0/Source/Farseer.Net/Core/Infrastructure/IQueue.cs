using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueue : IDisposable
    {
        /// <summary>
        /// 当前队列的ID
        /// </summary>
        Guid ID { get; set; }
        /// <summary>
        /// 当前组索引
        /// </summary>
        int Index { get; set; }
        
        /// <summary>
        /// 当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; set; }

        /// <summary>
        /// 当前队列立即交互数据库
        /// </summary>
        int Execute();

        /// <summary>
        /// 当前队列立即交互数据库（返回List<T>）
        /// </summary>
        List<TEntity> ExecuteList<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回Info）
        /// </summary>
        TEntity ExecuteInfo<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回T）
        /// </summary>
        T ExecuteQuery<T>(T defValue = default(T));
    }
}
