using System.Data.Common;
using FS.Core.Client.OleDb.SqlBuilder;
using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core.Client.OleDb
{
    public class OleDbProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OleDb"); }
        }
        public override IBuilderSqlQuery CreateBuilderSqlQuery(ContextMap contextMap, IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlQuery(queueManger, queueSql);
        }

        public override IBuilderSqlProc CreateBuilderSqlProc(ContextMap contextMap, IQueueManger queueManger, IQueue queueSql)
        {
            return new SqlProc(queueManger, queueSql);
        }

        public override IBuilderSqlOper CreateBuilderSqlOper(ContextMap contextMap, IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlOper(queueManger, queueSql);
        }
    }
}
