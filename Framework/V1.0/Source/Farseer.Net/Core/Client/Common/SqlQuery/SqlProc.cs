using System.Collections.Generic;
using System.Linq;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.Common.SqlQuery
{
    public class SqlProc<TEntity> : IDbSqlProc<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly IQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly IQueue QueueSql;

        public SqlProc(IQueueManger queueManger, IQueue queueSql)
        {
            QueueSql = queueSql;
            QueueManger = queueManger;
        }

        public virtual void CreateParam(TEntity entity)
        {
            if (entity == null) { return; }
            QueueSql.Param = new List<System.Data.Common.DbParameter>();

            var map = CacheManger.GetTableMap(typeof(TEntity));
            foreach (var kic in map.ModelList.Where(o => o.Value.IsInParam || o.Value.IsOutParam))
            {
                var obj = kic.Key.GetValue(entity, null);

                QueueSql.Param.Add(QueueManger.DbProvider.CreateDbParam(kic.Value.Column.Name, obj, kic.Key.PropertyType, kic.Value.IsOutParam));
            }
        }
    }
}