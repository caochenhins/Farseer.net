using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Core.Queue;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {

        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public override ISqlQueryView<TEntity> CreateSqlQuery<TEntity>(IQueryView query, IQueueView queue, string tableName)
        {
            var map = TableMapCache.GetMap<TEntity>();
            switch (map.ClassInfo.DataVer)
            {
                case "2000": return new SqlQueryView2000<TEntity>(query, queue, tableName);
            }
            return new SqlQueryView<TEntity>(query, queue, tableName);
        }
    }
}
