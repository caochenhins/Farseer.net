using System.Data.Common;
using FS.Core.Client.MySql.SqlQuery;
using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core.Client.MySql
{
    public class MySqlProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("MySql.Data.MySqlClient"); }
        }

        public override string KeywordAegis(string fieldName)
        {
            return string.Format("`{0}`", fieldName);
        }
        public override IBuilderSqlQuery<TEntity> CreateBuilderSqlQuery<TEntity>(ContextMap contextMap, IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlQuery<TEntity>(queueManger, queueSql);
        }

        public override IBuilderSqlProc<TEntity> CreateBuilderSqlProc<TEntity>(ContextMap contextMap, IQueueManger queueManger, IQueue queueSql)
        {
            return new SqlProc<TEntity>(queueManger, queueSql);
        }

        public override IBuilderSqlOper<TEntity> CreateBuilderSqlOper<TEntity>(ContextMap contextMap, IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlOper<TEntity>(queueManger, queueSql);
        }
    }
}
