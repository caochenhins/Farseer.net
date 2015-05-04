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
        public override IBuilderSqlQuery CreateBuilderSqlQuery(IQueueManger queueManger, IQueue queue)
        {
            return new SqlQuery(queueManger, queue);
        }

        public override IBuilderSqlOper CreateBuilderSqlOper(IQueueManger queueManger, IQueue queue)
        {
            return new SqlOper(queueManger, queue);
        }
    }
}
