using System.Data.Common;
using FS.Core.Client.SqlServer.SqlQuery;
using FS.Core.Infrastructure;
using FS.Core.Queue;
using FS.Mapping.Table;

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
    }
}
