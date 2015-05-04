using System.Data.Common;
using FS.Core.Client.OleDb.SqlBuilder;
using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class OleDbProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OleDb"); }
        }
        public override IBuilderSqlQuery CreateBuilderSqlQuery(IQueueManger queueManger, IQueue queue)
        {
            return new SqlQuery(queueManger, queue);
        }

        public override IBuilderSqlOper CreateBuilderSqlOper(IQueueManger queueManger, IQueue queue)
        {
            return new SqlOper(queueManger, queue);
        }
    }
}
