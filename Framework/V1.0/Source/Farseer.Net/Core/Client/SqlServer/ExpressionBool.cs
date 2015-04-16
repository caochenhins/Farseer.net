using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class ExpressionBool<TEntity> : Common.ExpressionBool<TEntity> where TEntity : class, new()
    {
        public ExpressionBool(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }
    }
}
