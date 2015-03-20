using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;
using FS.Extend;

namespace FS.Core.Context
{
    /// <summary>
    /// 存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public class ProcSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ProcContext<TEntity> _procContext;
        protected virtual IQueryQueue QueryQueue { get { return _procContext.Query.QueryQueue; } }

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        protected ProcSet() { }

        internal ProcSet(ProcContext<TEntity> procContext): this()
        {
            _procContext = procContext;
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public void Execute()
        { }

        /// <summary>
        /// 执行存储过程（返回单条记录）
        /// </summary>
        public TEntity ToInfo()
        {
            return null;
        }

        /// <summary>
        /// 执行存储过程（返回多条记录）
        /// </summary>
        public List<TEntity> ToList()
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
