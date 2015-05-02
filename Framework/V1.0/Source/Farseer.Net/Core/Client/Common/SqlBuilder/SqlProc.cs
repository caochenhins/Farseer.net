using System.Collections.Generic;
using System.Linq;
using FS.Core.Infrastructure;

namespace FS.Core.Client.Common.SqlBuilder
{
    public class SqlProc : IBuilderSqlProc
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

        public virtual void CreateParam<TEntity>(TEntity entity) where TEntity : class,new()
        {
            if (entity == null) { return; }
            QueueSql.Param = new List<System.Data.Common.DbParameter>();

            foreach (var kic in QueueSql.FieldMap.MapList.Where(o => o.Value.FieldAtt.IsInParam || o.Value.FieldAtt.IsOutParam))
            {
                var obj = kic.Key.GetValue(entity, null);

                QueueSql.Param.Add(QueueManger.DbProvider.CreateDbParam(kic.Value.FieldAtt.Name, obj, kic.Key.PropertyType, kic.Value.FieldAtt.IsOutParam));
            }
        }
    }
}