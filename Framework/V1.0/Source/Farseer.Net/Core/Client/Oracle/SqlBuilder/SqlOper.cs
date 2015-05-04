using FS.Core.Infrastructure;

namespace FS.Core.Client.Oracle.SqlBuilder
{
    public sealed class SqlOper : Common.SqlBuilder.SqlOper
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueue queue) : base(queueManger, queue) { }

        public override void InsertIdentity<TEntity>(TEntity entity)
        {
            base.InsertIdentity(entity);
            Queue.Sql.AppendFormat("SELECT @@IDENTITY ");
        }
    }
}