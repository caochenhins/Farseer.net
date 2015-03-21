using System;
using System.Collections.Generic;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQueryView<TEntity> : ISqlQueryView<TEntity> where TEntity : class,new()
    {
        protected readonly IQueryView Query;
        protected readonly IQueueView Queue;
        protected readonly string TableName;
        protected readonly ExpressionVisit<TEntity> Visit;

        public SqlServerSqlQueryView(IQueryView query, IQueueView queryQueue, string tableName)
        {
            Query = query;
            Queue = queryQueue;
            TableName = tableName;
            Visit = new ExpressionVisit<TEntity>(query, queryQueue, new SqlServerExpressionNew<TEntity>(query, queryQueue), new SqlServerExpressionBool<TEntity>(query, queryQueue));
        }

        public virtual void ToInfo()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }

        public virtual void ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            if (isDistinct && isRand) { strSelectSql += ",NEWID() as newid "; }

            if (!isRand)
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
            }
            else
            {
                Queue.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()) a {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
            }
        }

        public virtual void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var map = TableMapCache.GetMap<TEntity>();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            Queue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);

            Queue.Sql.AppendFormat("SELECT {1} FROM (SELECT {0} {1},ROW_NUMBER() OVER({2}) as Row FROM {3} {4}) a WHERE Row BETWEEN {5} AND {6};", strDistinctSql, strSelectSql, strOrderBySql, TableName, strWhereSql, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }

        public virtual void Count(bool isDistinct = false)
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT {0} Count(0) FROM {1} {2}", strDistinctSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Sum()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT SUM({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Max()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MAX({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Min()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MIN({0}) FROM {1} {2}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
        }

        public virtual void Value()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }
    }
}