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
        public override IBuilderSqlQuery CreateBuilderSqlQuery(IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlQuery(queueManger, queueSql);
        }

        public override IBuilderSqlOper CreateBuilderSqlOper(IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlOper(queueManger, queueSql);
        }
    }
}
