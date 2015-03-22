using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Context;
using FS.Core.Infrastructure;

namespace FS.Core.Query
{
    public class DbQueryProc : IQueryProc
    {
        public DbContext Context { get; private set; }
        public DbProvider DbProvider { get; set; }
        public DbQueryProc(DbContext tableContext, DbProvider dbProvider)
        {
            Context = tableContext;
            DbProvider = dbProvider;
            Clear();
        }
        private IQueueProc _queryQueue;
        public IQueueProc Queue { get { return _queryQueue ?? (_queryQueue = DbProvider.CreateQueue(0, this)); } }

        public List<DbParameter> Param
        {
            get
            {
                return Queue.Param;
            }
        }

        public void Clear()
        {
            if (_queryQueue != null) { _queryQueue = null; }
        }
    }
}
