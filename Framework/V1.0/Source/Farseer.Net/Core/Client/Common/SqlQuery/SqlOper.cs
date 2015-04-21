using System;
using System.Text;
using FS.Core.Data.Table;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.Common.SqlQuery
{
    public class SqlOper<TEntity> : IDbSqlOper<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly IQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly IQueueSql QueueSql;
        /// <summary>
        /// 数据库字段解析器总入口，根据要解析的类型，再分散到各自负责的解析器
        /// </summary>
        protected readonly ExpressionVisit<TEntity> Visit;

        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql)
        {
            QueueManger = queueManger;
            QueueSql = queueSql;
            Visit = new ExpressionVisit<TEntity>(queueManger, QueueSql);
        }

        public virtual void Delete()
        {
            QueueSql.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueueSql.Sql.AppendFormat("DELETE FROM {0} {1}", QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql);
        }

        public virtual void Insert(TEntity entity)
        {
            QueueSql.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            QueueSql.Sql.AppendFormat("INSERT INTO {0} {1}", QueueSql.Name, strinsertAssemble);
        }

        public virtual void InsertIdentity(TEntity entity)
        {
            QueueSql.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);
            QueueSql.Sql.AppendFormat("INSERT INTO {0} {1}", QueueSql.Name, strinsertAssemble);
        }

        public virtual void Update(TEntity entity)
        {
            QueueSql.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strAssemble = Visit.Assign(entity);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueueSql.Sql.AppendFormat("UPDATE {0} SET {1} {2}", QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strAssemble, strWhereSql);
        }

        public virtual void AddUp()
        {
            if (((TableQueue)QueueSql).ExpAssign == null || ((TableQueue)QueueSql).ExpAssign.Count == 0) { throw new Exception("赋值的参数不能为空！"); }
            QueueSql.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            #region 字段赋值
            var sqlAssign = new StringBuilder();
            foreach (var keyValue in ((TableQueue)QueueSql).ExpAssign)
            {
                var strAssemble = Visit.Assign(keyValue.Key);
                var strs = strAssemble.Split(',');
                foreach (var s in strs) { sqlAssign.AppendFormat("{0} = {0} + {1},", s, keyValue.Value); }
            }
            if (sqlAssign.Length > 0) { sqlAssign = sqlAssign.Remove(sqlAssign.Length - 1, 1); }
            #endregion

            QueueSql.Sql.AppendFormat("UPDATE {0} SET {1} {2}", QueueManger.DbProvider.KeywordAegis(QueueSql.Name), sqlAssign, strWhereSql);
        }
    }
}