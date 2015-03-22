using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Core.Queue;
using FS.Mapping.Table;

namespace FS.Core.Client.OleDb
{
    public class OleDbProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OleDb"); }
        }
    }
}
