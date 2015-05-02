using System.Collections.Generic;

namespace FS.Core.Data.Proc
{
    /// <summary>
    /// 存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ProcSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ProcContext _context;

        private ProcQueueManger QueueManger { get { return _context.QueueManger; } }
        private ProcQueue Queue { get { return _context.QueueManger.GetQueue(_name); } }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ProcSet() { }
        public ProcSet(ProcContext context)
        {
            _context = context;
            var contextState = _context.ContextMap.GetState(this.GetType());
            _name = contextState.Value.SetAtt.Name;
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public T GetValue<T>(TEntity entity = null, T t = default(T))
        {
            QueueManger.SqlQuery<TEntity>(Queue).CreateParam(entity);
            return QueueManger.ExecuteValue(Queue, entity, t);
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public void Execute(TEntity entity = null)
        {
            QueueManger.SqlQuery<TEntity>(Queue).CreateParam(entity);
            QueueManger.Execute(Queue, entity);
        }

        /// <summary>
        /// 执行存储过程（返回单条记录）
        /// </summary>
        public TEntity ToEntity(TEntity entity = null)
        {
            QueueManger.SqlQuery<TEntity>(Queue).CreateParam(entity);
            return QueueManger.ExecuteInfo(Queue, entity);
        }

        /// <summary>
        /// 执行存储过程（返回多条记录）
        /// </summary>
        public List<TEntity> ToList(TEntity entity = null)
        {
            QueueManger.SqlQuery<TEntity>(Queue).CreateParam(entity);
            return QueueManger.ExecuteList(Queue, entity);
        }
    }
}