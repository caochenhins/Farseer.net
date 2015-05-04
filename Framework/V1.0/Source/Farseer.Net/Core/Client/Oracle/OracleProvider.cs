using System.Data.Common;
using FS.Core.Client.Oracle.SqlBuilder;
using FS.Core.Infrastructure;

namespace FS.Core.Client.Oracle
{
    public class OracleProvider : DbProvider
    {
        public override string ParamsPrefix
        {
            get { return ":"; }
        }

        public override string KeywordAegis(string fieldName)
        {
            return fieldName;
        }

        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OracleClient"); }
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
