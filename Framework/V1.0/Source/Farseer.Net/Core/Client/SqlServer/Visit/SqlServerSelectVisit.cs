using System;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Core.Visit;

namespace FS.Core.Client.SqlServer.Visit
{
    public class SqlServerSelectVisit<TEntity> : DbSelectVisit<TEntity> where TEntity : class, new()
    {
        public SqlServerSelectVisit(IQuery query, IQueryQueue queryQueue) : base(query, queryQueue) { }
    }
}
