using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Core.Queue;
using FS.Mapping.Table;

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
    }
}
