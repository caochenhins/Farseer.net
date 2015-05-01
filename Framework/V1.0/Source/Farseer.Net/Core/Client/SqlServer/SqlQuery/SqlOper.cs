using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.SqlQuery
{
    public class SqlOper<TEntity> : Common.SqlQuery.SqlOper<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void Insert(TEntity entity)
        {
            base.Insert(entity);

            var map = CacheManger.GetFieldMap(typeof(TEntity));

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = map.PrimaryState.Key != null && map.PrimaryState.Key.GetValue(entity, null) != null;
            if (indexHaveValue && !string.IsNullOrWhiteSpace(map.PrimaryState.Value.FieldAtt.Name))
            {
                QueueSql.Sql = new StringBuilder(string.Format("SET IDENTITY_INSERT {0} ON ; {1} ; SET IDENTITY_INSERT {0} OFF;", QueueSql.Name, QueueSql.Sql));
            }
        }

        public override void InsertIdentity(TEntity entity)
        {
            Insert(entity);
            QueueSql.Sql.AppendFormat("SELECT @@IDENTITY;");
        }
    }
}