using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.SqlBuilder
{
    public class SqlOper : Common.SqlBuilder.SqlOper
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void Insert<TEntity>(TEntity entity)
        {
            base.Insert(entity);

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = QueueSql.Map.PrimaryState.Key != null && QueueSql.Map.PrimaryState.Key.GetValue(entity, null) != null;
            if (indexHaveValue && !string.IsNullOrWhiteSpace(QueueSql.Map.PrimaryState.Value.FieldAtt.Name))
            {
                QueueSql.Sql = new StringBuilder(string.Format("SET IDENTITY_INSERT {0} ON ; {1} ; SET IDENTITY_INSERT {0} OFF;", QueueSql.Name, QueueSql.Sql));
            }
        }

        public override void InsertIdentity<TEntity>(TEntity entity)
        {
            Insert(entity);
            QueueSql.Sql.AppendFormat("SELECT @@IDENTITY;");
        }
    }
}