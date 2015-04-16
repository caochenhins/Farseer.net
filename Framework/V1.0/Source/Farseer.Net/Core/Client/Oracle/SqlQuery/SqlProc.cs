using FS.Core.Infrastructure;

namespace FS.Core.Client.Oracle.SqlQuery
{
    public sealed class SqlProc<TEntity> : Common.SqlQuery.SqlProc<TEntity> where TEntity : class,new()
    {
        public SqlProc(IQueueManger queueManger, IQueue queueSql) : base(queueManger, queueSql) { }
    }
}