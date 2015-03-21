using System;
using System.Collections.Generic;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery<TEntity> : SqlServerSqlQueryView<TEntity>, ISqlQueryTable<TEntity> where TEntity : class,new()
    {
        public SqlServerSqlQuery(IQueryTable query, IQueueTable queue, string tableName) : base(query, queue, tableName){ }

        public virtual void Delete()
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("DELETE FROM {0} {1}", Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Insert(TEntity entity)
        {
            Queue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { Queue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", TableName); }

            Queue.Sql.AppendFormat("INSERT INTO {0} {1}", TableName, strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { Queue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", TableName); }
        }

        public virtual void InsertIdentity(TEntity entity)
        {
            Queue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { Queue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", TableName); }

            Queue.Sql.AppendFormat("INSERT INTO {0} {1};{2}", TableName, strinsertAssemble, Query.DbProvider.CurrentIdentity);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { Queue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF; ", TableName); }
        }

        public virtual void Update(TEntity entity)
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strAssemble = Visit.Assign(entity);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", Query.DbProvider.KeywordAegis(TableName), strAssemble, strWhereSql);
        }

        public virtual void AddUp()
        {
            if (((IQueueTable)Queue).ExpAssign == null || ((IQueueTable)Queue).ExpAssign.Count == 0) { throw new Exception("赋值的参数不能为空！"); }
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            #region 字段赋值
            var sqlAssign = new StringBuilder();
            foreach (var keyValue in ((IQueueTable)Queue).ExpAssign)
            {
                var strAssemble = Visit.Assign(keyValue.Key);
                var strs = strAssemble.Split(',');
                foreach (var s in strs) { sqlAssign.AppendFormat("{0} = {0} + {1},", s, keyValue.Value); }
            }
            if (sqlAssign.Length > 0) { sqlAssign = sqlAssign.Remove(sqlAssign.Length - 1, 1); }
            #endregion

            Queue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", Query.DbProvider.KeywordAegis(TableName), sqlAssign, strWhereSql);
        }
    }
}