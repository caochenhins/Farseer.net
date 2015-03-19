using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    /// <summary>
    /// 针对SqlServer 2000 数据库 提供
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlServerSqlQuery2000<TEntity> : SqlServerSqlQuery<TEntity> where TEntity : class,new()
    {
        public SqlServerSqlQuery2000(IQuery query, IQueryQueue queryQueue, string tableName) : base(query, queryQueue, tableName) { }

        public override void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var map = TableMapCache.GetMap<TEntity>();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;
            QueryQueue.Sql = new StringBuilder();

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }

            QueryQueue.Sql.AppendFormat("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} {1} FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, TableName, strWhereSql, strOrderBySql, strOrderBySqlReverse);
        }
    }
}
