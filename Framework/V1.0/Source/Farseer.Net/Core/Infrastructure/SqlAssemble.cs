using System.Collections.Generic;
using System.Data.Common;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// Sql片断组装
    /// </summary>
    public abstract class SqlAssemble
    {
        protected readonly IQueryQueue QueryQueue;
        protected readonly DbProvider DbProvider;
        protected readonly IList<DbParameter> LstParam;

        protected SqlAssemble(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam)
        {
            QueryQueue = queryQueue;
            DbProvider = dbProvider;
            LstParam = lstParam;
        }
    }
}
