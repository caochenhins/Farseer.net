using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Assemble;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer.Query
{
    /// <summary>
    /// 组查询队列（支持批量提交SQL）
    /// </summary>
    public class SqlServerQueryInsert : IQueryQueueInsert
    {
        private readonly IQuery _queryProvider;

        public SqlServerQueryInsert(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public void Query<T>(T entity) where T : class,new()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strinsertAssemble = new InsertAssemble(_queryProvider).Execute(entity, ref param);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryProvider.QueryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _queryProvider.TableContext.TableName); }

            _queryProvider.QueryQueue.Sql.AppendFormat("INSERT INTO {0} ", _queryProvider.TableContext.TableName);
            _queryProvider.QueryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryProvider.QueryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", _queryProvider.TableContext.TableName); }

            _queryProvider.QueryQueue.Param = param;

            // 非合并提交，则直接提交
            if (!_queryProvider.TableContext.IsMergeCommand) { _queryProvider.QueryQueue.Execute(); return; }

            _queryProvider.Append();
        }
    }
}
