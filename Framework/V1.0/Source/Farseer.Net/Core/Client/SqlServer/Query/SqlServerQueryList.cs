using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Assemble;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;
using FS.Extend;

namespace FS.Core.Client.SqlServer.Query
{
    /// <summary>
    /// 组查询队列（支持批量提交SQL）
    /// </summary>
    public class SqlServerQueryList : IQueryQueueList
    {
        private readonly IQuery _queryProvider;

        public SqlServerQueryList(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public List<T> Query<T>() where T : class, new()
        {
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpOrderBy);

            _queryProvider.QueryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("SELECT {0} ", strSelectSql));

            _queryProvider.QueryQueue.Sql.Append(string.Format("FROM {0} ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }

            List<T> t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString()))
            {
                t = reader.ToList<T>();
                reader.Close();
            }
            _queryProvider.Clear();
            return t;
        }
    }
}
