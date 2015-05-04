using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.Common.SqlBuilder
{
    /// <summary>
    /// 查询支持的SQL方法
    /// </summary>
    public class SqlQuery : IBuilderSqlQuery
    {
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly IQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly IQueue Queue;
        /// <summary>
        /// 数据库字段解析器总入口，根据要解析的类型，再分散到各自负责的解析器
        /// </summary>
        protected readonly ExpressionVisit Visit;

        /// <summary>
        /// 查询支持的SQL方法
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public SqlQuery(IQueueManger queueManger, IQueue queue)
        {
            QueueManger = queueManger;
            Queue = queue;
            Visit = new ExpressionVisit(queueManger, Queue);
        }

        public virtual void ToEntity()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
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
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
            }
            else if (string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Queue.Sql.AppendFormat("SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
            }
            else
            {
                Queue.Sql.AppendFormat("SELECT * FROM (SELECT {0} {1} {2} FROM {3} {4} ORDER BY NEWID()) a {5}", strDistinctSql, strTopSql, strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
            }
        }

        public virtual void ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1) { ToList(pageSize, isDistinct); return; }

            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;
            Queue.Sql = new StringBuilder();

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? string.Format("{0} ASC", Queue.FieldMap.PrimaryState.Value.FieldAtt.Name) : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }

            Queue.Sql.AppendFormat("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} {1} FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, Queue.Name, strWhereSql, strOrderBySql, strOrderBySqlReverse);
        }

        public virtual void Count(bool isDistinct = false)
        {
            Queue.Sql = new StringBuilder();
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT {0} Count(0) FROM {1} {2}", strDistinctSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
        }

        public virtual void Sum()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT SUM({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
        }

        public virtual void Max()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MAX({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
        }

        public virtual void Min()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Queue.Sql.AppendFormat("SELECT MIN({0}) FROM {1} {2}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql);
        }

        public virtual void GetValue()
        {
            Queue.Sql = new StringBuilder();
            var strSelectSql = Visit.Select(Queue.ExpSelect);
            var strWhereSql = Visit.Where(Queue.ExpWhere);
            var strOrderBySql = Visit.OrderBy(Queue.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Queue.Sql.AppendFormat("SELECT TOP 1 {0} FROM {1} {2} {3}", strSelectSql, QueueManger.DbProvider.KeywordAegis(Queue.Name), strWhereSql, strOrderBySql);
        }
    }
}