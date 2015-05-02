using System;
using System.Text;
using FS.Core.Data.Table;
using FS.Core.Infrastructure;

namespace FS.Core.Client.Common.SqlBuilder
{
    public class SqlOper : SqlQuery, IBuilderSqlOper
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlOper(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public virtual void Delete()
        {
            QueueSql.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueueSql.Sql.AppendFormat("DELETE FROM {0} {1}", QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql);
        }

        public virtual void Insert<TEntity>(TEntity entity) where TEntity : class,new()
        {
            QueueSql.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            QueueSql.Sql.AppendFormat("INSERT INTO {0} {1}", QueueSql.Name, strinsertAssemble);
        }

        public virtual void InsertIdentity<TEntity>(TEntity entity) where TEntity : class,new()
        {
            QueueSql.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);
            QueueSql.Sql.AppendFormat("INSERT INTO {0} {1}", QueueSql.Name, strinsertAssemble);
        }

        public virtual void Update<TEntity>(TEntity entity) where TEntity : class,new()
        {
            QueueSql.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strAssemble = Visit.Assign(entity);

            // 主键如果有值，则需要 去掉主键的赋值、并且加上主键的条件
            if (QueueSql.FieldMap.PrimaryState.Key != null)
            {
                var value = QueueSql.FieldMap.PrimaryState.Key.GetValue(entity, null);
                if (value != null)
                {
                    if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql += " AND "; }
                    strWhereSql += string.Format("{0} = {1}", QueueSql.FieldMap.PrimaryState.Value.FieldAtt.Name, value);
                }
            }
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