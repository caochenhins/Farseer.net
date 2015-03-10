using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQueryQueue : IQueryQueue
    {
        private readonly IQuery _queryProvider;
        public Expression ExpOrderBy { get; set; }
        public int Index { get; set; }
        public Expression ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public StringBuilder Sql { get; set; }
        public IList<DbParameter> Param { get; set; }

        public SqlServerQueryQueue(int index, IQuery queryProvider)
        {
            Index = index;
            _queryProvider = queryProvider;
        }

        public int Execute()
        {
            var result = Sql.Length < 1 ? 0 : _queryProvider.TableContext.Database.ExecuteNonQuery(CommandType.Text, Sql.ToString(), Param == null ? null : ((List<DbParameter>)Param).ToArray());
            _queryProvider.Clear();
            return result;
        }

        public void Dispose()
        {
            Sql.Clear();
            ExpOrderBy = null;
            ExpSelect = null;
            ExpWhere = null;
            Sql = null;
        }
    }
}
