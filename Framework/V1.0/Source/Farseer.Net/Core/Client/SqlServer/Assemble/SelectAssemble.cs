using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class SelectAssemble   : SqlAssemble
    {
        public SelectAssemble(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam) : base(queryQueue, dbProvider, lstParam) { }

        public string Execute(Expression exp)
        {
            return "";
        }
        public string Execute(List<Expression> exp)
        {
            return "";
        }
    }
}
