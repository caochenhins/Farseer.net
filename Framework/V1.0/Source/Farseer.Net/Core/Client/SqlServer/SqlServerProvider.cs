using System.Data.Common;
using FS.Core.Infrastructure;
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

        public override IQueueTable CreateQueryQueue(int index, IQueryTable query)
        {
            return new DbQueueTable(index, query);
        }

        public override ISqlQueryTable<TEntity> CreateSqlQuery<TEntity>(IQueryTable query, IQueueTable queue, string tableName)
        {
            var map = TableMapCache.GetMap<TEntity>();
            switch (map.ClassInfo.DataVer)
            {
                case "2000": return new SqlServerSqlQuery2000<TEntity>(query, queue, tableName);
            }
            return new SqlServerSqlQuery<TEntity>(query, queue, tableName);
        }

        public override ISqlQueryView<TEntity> CreateSqlQuery<TEntity>(IQueryView query, IQueueView queue, string tableName)
        {
            throw new System.NotImplementedException();
        }
    }
}
