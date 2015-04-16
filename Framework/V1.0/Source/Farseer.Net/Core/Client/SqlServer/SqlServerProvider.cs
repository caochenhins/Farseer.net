using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public override IDbSqlQuery<TEntity> CreateSqlQuery<TEntity>(IQueueManger queueManger, IQueueSql queueSql)
        {
            var map = TableMapCache.GetMap(typeof(TEntity));
            switch (map.ClassInfo.DataVer)
            {
                case "2000": return new SqlQuery2000<TEntity>(queueManger, queueSql);
            }
            return new SqlQuery<TEntity>(queueManger, queueSql);
        }

        public override IDbSqlProc<TEntity> CreateSqlProc<TEntity>(IQueueManger queueManger, IQueue queueSql)
        {
            return new SqlProc<TEntity>(queueManger, queueSql);
        }

        public override IDbSqlOper<TEntity> CreateSqlOper<TEntity>(IQueueManger queueManger, IQueueSql queueSql)
        {
            return new SqlOper<TEntity>(queueManger, queueSql);
        }
    }
}
