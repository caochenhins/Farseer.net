using System.Collections.Generic;
using System.Linq;
using FS.Core.Infrastructure;
using FS.Core.Set;
using FS.Mapping.Table;
using FS.Extend;

namespace FS.Core.Client.MySql.SqlQuery
{
    public sealed class SqlQueryProc<TEntity> : ISqlQueryProc<TEntity> where TEntity : class,new()
    {
        private readonly IQueryProc _query;
        private readonly IQueueProc _queue;

        public SqlQueryProc(IQueryProc query, IQueueProc queue)
        {
            _queue = queue;
            _query = query;
        }

        public void CreateParam(TEntity entity)
        {
            if (entity == null) { return; }
            _queue.Param = new List<System.Data.Common.DbParameter>();

            var map = TableMapCache.GetMap(entity);
            foreach (var kic in map.ModelList.Where(o => o.Value.IsInParam || o.Value.IsOutParam))
            {
                var obj = kic.Key.GetValue(entity, null);

                _queue.Param.Add(_query.DbProvider.CreateDbParam(kic.Value.Column.Name, obj, kic.Key.PropertyType, kic.Value.IsOutParam));
            }
        }
    }
}