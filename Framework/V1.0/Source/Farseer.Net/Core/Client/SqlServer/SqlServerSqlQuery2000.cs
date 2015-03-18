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

        public override void ToList(int pageSize, int pageIndex)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize); return; }

            var map = TableMapCache.GetMap<TEntity>();
            var strSelectSql = Visit.Select(QueryQueue.ExpSelect);
            var strWhereSql = Visit.Where(QueryQueue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueryQueue.ExpOrderBy);
            QueryQueue.Sql = new StringBuilder();

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }

            QueryQueue.Sql.AppendFormat("SELECT TOP {1} {0} FROM (SELECT TOP {2} {0} FROM {3} {4} {5}) a  {6};", strSelectSql, pageSize, pageSize * pageIndex, TableName, strWhereSql, strOrderBySql, strOrderBySqlReverse);
        }
    }
}
