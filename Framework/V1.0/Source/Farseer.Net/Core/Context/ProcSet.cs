using System;
using System.Collections.Generic;
using FS.Core.Infrastructure;

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
        private IQueryProc Query { get { return _procContext.Query; } }
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
        public void Execute(TEntity entity)
        {
            if (entity == null) { throw new ArgumentNullException("entity", "执行操作时，参数不能为空！"); }

            //  判断是否启用合并提交
            if (_procContext.IsMergeCommand)
            {
                QueryQueue.LazyAct = (queryQueue) => queryQueue.SqlQuery<TEntity>().Update(entity);
                Query.Append();
            }
            else
            {
                QueryQueue.SqlQuery<TEntity>().Update(entity);
                QueryQueue.Execute();
            }
        }

        /// <summary>
        /// 执行存储过程（返回单条记录）
        /// </summary>
        public TEntity ToInfo(TEntity entity)
        {
            return null;
        }

        /// <summary>
        /// 执行存储过程（返回多条记录）
        /// </summary>
        public List<TEntity> ToList(TEntity entity)
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
