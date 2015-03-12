using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class OrderByAssemble  : SqlAssemble
    {
        public OrderByAssemble(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam) : base(queryQueue, dbProvider, lstParam) { }

        public string Execute(Expression exp)
        {
            return "";
        }
    }
}
