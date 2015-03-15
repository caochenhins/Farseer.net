using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Core.Visit;

namespace FS.Core.Client.SqlServer.Visit
{
    public class SqlServerAssignVisit : DbAssignVisit
    {
        public SqlServerAssignVisit(IQuery query, IQueryQueue queryQueue) : base(query, queryQueue) { }
    }
}
