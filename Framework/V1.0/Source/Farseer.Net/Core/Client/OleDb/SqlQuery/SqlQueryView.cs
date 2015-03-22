using System;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.OleDb.SqlQuery
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
            if (isDistinct && isRand) { strSelectSql += ",Rnd(-(TestID+\" & Rnd() & \")) as newid "; }

            if (!isRand)
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql);
            }
            else
            {
                Queue.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} {2} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))) a {5}", strDistinctSql, strTopSql, strSelectSql, Query.DbProvider.KeywordAegis(TableName), strWhereSql, strOrderBySql);
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

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }

            Queue.Sql.AppendFormat("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} {1} FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, TableName, strWhereSql, strOrderBySql, strOrderBySqlReverse);
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