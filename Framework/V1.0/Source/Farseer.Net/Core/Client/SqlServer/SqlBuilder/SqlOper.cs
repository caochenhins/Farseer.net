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
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueue queue) : base(queueManger, queue) { }

        public override void Insert<TEntity>(TEntity entity)
        {
            base.Insert(entity);

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = Queue.FieldMap.PrimaryState.Key != null && Queue.FieldMap.PrimaryState.Key.GetValue(entity, null) != null;
            if (indexHaveValue && !string.IsNullOrWhiteSpace(Queue.FieldMap.PrimaryState.Value.FieldAtt.Name))
            {
                Queue.Sql = new StringBuilder(string.Format("SET IDENTITY_INSERT {0} ON ; {1} ; SET IDENTITY_INSERT {0} OFF;", Queue.Name, Queue.Sql));
            }
        }

        public override void InsertIdentity<TEntity>(TEntity entity)
        {
            Insert(entity);
            Queue.Sql.AppendFormat("SELECT @@IDENTITY;");
        }
    }
}