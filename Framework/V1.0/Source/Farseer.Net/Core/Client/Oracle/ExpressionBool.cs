using FS.Core.Infrastructure;

namespace FS.Core.Client.Oracle
{
    public class ExpressionBool<TEntity> : Common.ExpressionBool<TEntity> where TEntity : class, new()
    {
        public ExpressionBool(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }
    }
}
