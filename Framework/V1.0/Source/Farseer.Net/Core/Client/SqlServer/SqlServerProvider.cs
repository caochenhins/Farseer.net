using System.Data.Common;
using FS.Core.Client.SqlServer.SqlBuilder;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public override IBuilderSqlQuery CreateBuilderSqlQuery(IQueueManger queueManger, IQueueSql queueSql)
        {
            switch (queueManger.ContextMap.ContextProperty.DataVer)
            {
                case "2000": return new SqlQuery2000(queueManger, queueSql);
            }
            return new SqlQuery(queueManger, queueSql);
        }

        public override IBuilderSqlProc CreateBuilderSqlProc(IQueueManger queueManger, IQueue queueSql)
        {
            return new SqlProc(queueManger, queueSql);
        }

        public override IBuilderSqlOper CreateBuilderSqlOper(IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlOper(queueManger, queueSql);
        }
    }
}
