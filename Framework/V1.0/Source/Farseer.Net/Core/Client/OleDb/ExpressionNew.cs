using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class ExpressionNew<TEntity> : DbExpressionNewProvider<TEntity> where TEntity : class, new()
    {
        public ExpressionNew(IQuery query, IQueue queryQueue) : base(query, queryQueue) { }
    }
}
