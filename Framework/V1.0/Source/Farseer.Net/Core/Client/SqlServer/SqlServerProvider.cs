using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Core.Queue;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {
        public override string CurrentIdentity
        {
            get { return "SELECT @@IDENTITY;"; }
        }

        public override int ParamsMaxLength
        {
            get { return 2100; }
        }

        public override string ParamsPrefix
        {
            get { return "@"; }
        }

        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public override string KeywordAegis(string fieldName)
        {
            return string.Format("[{0}]", fieldName);
        }

        public override IQueueTable CreateQueue(int index, IQueryTable query)
        {
            return new DbQueueTable(index, query);
        }

        public override IQueueView CreateQueue(int index, IQueryView query)
        {
            return new DbQueueView(index, query);
        }

        public override IQueueProc CreateQueue(int index, IQueryProc query)
        {
            return new DbQueueProc(index, query);
        }

        public override ISqlQueryTable<TEntity> CreateSqlQuery<TEntity>(IQueryTable query, IQueueTable queue, string tableName)
        {
            return new SqlQueryTable<TEntity>(query, queue, tableName);
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

        public override ISqlQueryProc<TEntity> CreateSqlQuery<TEntity>(IQueryProc query, IQueueProc queue)
        {
            return new SqlQueryProc<TEntity>(query, queue);
        }
    }
}
