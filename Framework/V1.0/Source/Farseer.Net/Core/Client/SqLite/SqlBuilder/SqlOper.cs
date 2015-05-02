using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite.SqlBuilder
{
    public sealed class SqlOper : Common.SqlBuilder.SqlOper
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void InsertIdentity<TEntity>(TEntity entity)
        {
            base.InsertIdentity(entity);
            QueueSql.Sql.AppendFormat(";SELECT last_insert_rowid();");
        }
    }
}