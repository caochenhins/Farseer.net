using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerExpressionNew<TEntity> : DbExpressionNewProvider<TEntity> where TEntity : class, new()
    {
        public SqlServerExpressionNew(IQuery query, IQueue queryQueue) : base(query, queryQueue) { }
    }
}
