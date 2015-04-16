using FS.Core.Infrastructure;

namespace FS.Core.Client.Common
{
    public class ExpressionNew<TEntity> : DbExpressionNewProvider<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public ExpressionNew(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }
    }
}