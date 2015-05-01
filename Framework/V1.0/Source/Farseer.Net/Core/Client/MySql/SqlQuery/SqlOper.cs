using System;
using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.MySql.SqlQuery
{
    public sealed class SqlOper<TEntity> : Common.SqlQuery.SqlOper<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void InsertIdentity(TEntity entity)
        {
            base.InsertIdentity(entity);
            QueueSql.Sql.AppendFormat("SELECT @@IDENTITY;");
        }
    }
}