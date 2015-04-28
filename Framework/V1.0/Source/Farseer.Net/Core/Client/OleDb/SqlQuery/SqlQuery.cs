using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.OleDb.SqlQuery
{
    public class SqlQuery<TEntity> : Common.SqlQuery.SqlQuery<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public SqlQuery(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }

        public override void ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            QueueSql.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(QueueSql.ExpSelect);
            var strWhereSql = Visit.Where(QueueSql.ExpWhere);
            var strOrderBySql = Visit.OrderBy(QueueSql.ExpOrderBy);
            var strTopSql = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            if (isDistinct && isRand) { strSelectSql += ",Rnd(-(TestID+\" & Rnd() & \")) as newid "; }

            if (!isRand)
            {
                QueueSql.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                QueueSql.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql);
            }
            else
            {
                QueueSql.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} {2} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))) a {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql);
            }
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

            QueueSql.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, QueueManger.DbProvider.KeywordAegis(QueueSql.Name), strWhereSql, strOrderBySql);
        }
    }
}