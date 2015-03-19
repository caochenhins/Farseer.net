using System;
using System.Collections.Generic;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery<TEntity> : ISqlQuery<TEntity> where TEntity : class,new()
    {
        protected readonly IQueryQueue QueryQueue;
        protected readonly string TableName;
        protected readonly IQuery Query;
        protected readonly ExpressionVisit<TEntity> Visit;

        public SqlServerSqlQuery(IQuery query, IQueryQueue queryQueue, string tableName)
        {
            Query = query;
            QueryQueue = queryQueue;
            TableName = tableName;
            Visit = new ExpressionVisit<TEntity>(query, queryQueue, new SqlServerExpressionNew<TEntity>(query, queryQueue), new SqlServerExpressionBool<TEntity>(query, queryQueue));
        }

        public virtual void ToInfo()
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            QueryQueue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }

        public virtual void ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            if (isDistinct && isRand) { strSelectSql += ",NEWID() as newid "; }

            if (!isRand)
            {
                QueryQueue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                QueryQueue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
            }
            else
            {
                QueryQueue.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()) a {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
            }
        }

        public virtual void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var map = TableMapCache.GetMap<TEntity>();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            QueryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);

            QueryQueue.Sql.AppendFormat("SELECT {1} FROM (SELECT {0} {1},ROW_NUMBER() OVER({2}) as Row FROM {3} {4}) a WHERE Row BETWEEN {5} AND {6};", strDistinctSql, strSelectSql, strOrderBySql, TableName, strWhereSql, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }

        public virtual void Count(bool isDistinct = false)
        {
            QueryQueue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("SELECT {0} Count(0) FROM {1} {2}", strDistinctSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Sum()
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("SELECT SUM({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Max()
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("SELECT MAX({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Min()
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("SELECT MIN({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Value()
        {
            QueryQueue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            QueryQueue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }

        public virtual void Delete()
        {
            QueryQueue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("DELETE FROM {0} {1}", Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Insert(TEntity entity)
        {
            QueryQueue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { QueryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", TableName); }

            QueryQueue.Sql.AppendFormat("INSERT INTO {0} {1}", TableName, strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { QueryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", TableName); }
        }

        public virtual void InsertIdentity(TEntity entity)
        {
            QueryQueue.Sql = new StringBuilder();
            var strinsertAssemble = Visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { QueryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", TableName); }

            QueryQueue.Sql.AppendFormat("INSERT INTO {0} {1};{2}", TableName, strinsertAssemble, Query.DbProvider.CurrentIdentity);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { QueryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF; ", TableName); }
        }

        public virtual void Update(TEntity entity)
        {
            QueryQueue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strAssemble = Visit.Assign(entity);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            QueryQueue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", Query.DbProvider.KeywordAegis(TableName), strAssemble, strWhereSql);
        }

        public virtual void AddUp()
        {
            if (QueryQueue.ExpAssign == null || QueryQueue.ExpAssign.Count == 0) { throw new Exception("赋值的参数不能为空！"); }
            QueryQueue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            #region 字段赋值
            var sqlAssign = new StringBuilder();
            foreach (var keyValue in QueryQueue.ExpAssign)
            {
                var strAssemble = Visit.Assign(keyValue.Key);
                var strs = strAssemble.Split(',');
                foreach (var s in strs) { sqlAssign.AppendFormat("{0} = {0} + {1},", s, keyValue.Value); }
            }
            if (sqlAssign.Length > 0) { sqlAssign = sqlAssign.Remove(sqlAssign.Length - 1, 1); }
            #endregion

            QueryQueue.Sql.AppendFormat("UPDATE {0} SET {1} {2}", Query.DbProvider.KeywordAegis(TableName), sqlAssign, strWhereSql);
        }
    }
}