using System;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.MySql.SqlQuery
{
    public class SqlQueryView<TEntity> : ISqlQueryView<TEntity> where TEntity : class,new()
    {
        protected readonly IQueryView Query;
        protected readonly IQueueView Queue;
        protected readonly string TableName;
        protected readonly ExpressionVisit<TEntity> Visit;

        public SqlQueryView(IQueryView query, IQueueView queryQueue, string tableName)
        {
            Query = query;
            Queue = queryQueue;
            TableName = tableName;
            Visit = new ExpressionVisit<TEntity>(query, queryQueue, new ExpressionNew<TEntity>(query, queryQueue), new ExpressionBool<TEntity>(query, queryQueue));
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

            Queue.Sql.AppendFormat("SELECT {0} FROM {1} {2} {3} LIMIT 1", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }

        public virtual void ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("LIMIT {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            if (isDistinct && isRand) { strSelectSql += ",Rand() as newid "; }

            if (!isRand)
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} {4} {5}", strDistinctSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql, strTopSql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} ORDER BY Rand() {4}", strDistinctSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strTopSql);
            }
            else
            {
                Queue.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} FROM {2} {3} ORDER BY Rand() {5}) a {4}", strDistinctSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql, strTopSql);
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
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} {4} LIMIT {5},{6}", strDistinctSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql, pageSize * (pageIndex - 1), pageSize);
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

            Queue.Sql.AppendFormat("SELECT {0} FROM {1} {2} {3} LIMIT 1", strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
        }
    }
}