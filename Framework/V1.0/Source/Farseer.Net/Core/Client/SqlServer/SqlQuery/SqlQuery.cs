using System;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer.SqlQuery
{
    /// <summary>
    /// 查询支持的SQL方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlQuery<TEntity> : Common.SqlQuery.SqlQuery<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlQuery(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var map = CacheManger.GetTableMap(typeof(TEntity));
            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            QueueSql.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strOrderBySql) && string.IsNullOrWhiteSpace(map.IndexName)) { throw new Exception("当未指定排序方式时，必须要指定 主键字段"); }

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", map.IndexName) : strOrderBySql);

            QueueSql.Sql.AppendFormat("SELECT {1} FROM (SELECT {0} {1},ROW_NUMBER() OVER({2}) as Row FROM {3} {4}) a WHERE Row BETWEEN {5} AND {6};", strDistinctSql, strSelectSql, strOrderBySql, QueueSql.Name, strWhereSql, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }
    }
}