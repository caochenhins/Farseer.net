using System.Data.Common;
using FS.Core.Client.SqLite.SqlBuilder;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite
{
    public class SqLiteProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SQLite"); }
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
