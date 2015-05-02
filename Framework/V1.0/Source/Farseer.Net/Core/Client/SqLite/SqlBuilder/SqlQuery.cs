using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite.SqlBuilder
{
    public class SqlQuery : Common.SqlBuilder.SqlQuery
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlQuery(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void ToEntity()
        {
            QueueSql.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            QueueSql.Sql.AppendFormat("SELECT {0} FROM {1} {2} {3} LIMIT 0,1", strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql);
        }

        public override void ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueueSql.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("LIMIT {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            if (isDistinct && isRand) { strSelectSql += ",Rand() as newid "; }

            if (!isRand)
            {
                QueueSql.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} {4} {5}", strDistinctSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql, strTopSql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                QueueSql.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} ORDER BY Rand() {4}", strDistinctSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strTopSql);
            }
            else
            {
                QueueSql.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} FROM {2} {3} ORDER BY Rand() {5}) a {4}", strDistinctSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql, strTopSql);
            }
        }

        public override void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            QueueSql.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            QueueSql.Sql.AppendFormat("SELECT {0} {1} FROM {2} {3} {4} LIMIT {5},{6}", strDistinctSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql, pageSize * (pageIndex - 1), pageSize);
        }

        public override void GetValue()
        {
            QueueSql.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            QueueSql.Sql.AppendFormat("SELECT {0} FROM {1} {2} {3} LIMIT 0,1", strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql);
        }
    }
}