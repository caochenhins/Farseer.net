using System.Data.Common;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerProvider : DbProvider
    {
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

        public override IQueryQueue CreateQueryQueue(int index, IQuery query)
        {
            return new DbQueryQueue(index, query);
        }

        public override ISqlQuery<TEntity> CreateSqlQuery<TEntity>(IQuery query, IQueryQueue queryQueue, string tableName)
        {
            var map = TableMapCache.GetMap<TEntity>();
            switch (map.ClassInfo.DataVer)
            {
                case "2000": return new SqlServerSqlQuery2000<TEntity>(query, queryQueue, tableName);
            }
            return new SqlServerSqlQuery<TEntity>(query, queryQueue, tableName);
        }
    }
}
