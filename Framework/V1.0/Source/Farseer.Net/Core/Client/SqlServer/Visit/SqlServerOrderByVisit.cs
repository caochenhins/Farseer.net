using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Core.Visit;

namespace FS.Core.Client.SqlServer.Visit
{
    public class SqlServerOrderByVisit<TEntity> : DbOrderByVisit<TEntity> where TEntity : class, new()
    {
        public SqlServerOrderByVisit(IQuery query, IQueryQueue queryQueue) : base(query, queryQueue) { }
    }
}
