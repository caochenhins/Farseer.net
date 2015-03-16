using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery<TEntity> : ISqlQuery<TEntity> where TEntity : class,new()
    {
        private readonly IQueryQueue _queryQueue;
        private readonly string _tableName;
        private readonly IQuery _query;
        private readonly ExpressionVisit<TEntity> _visit;

        public SqlServerSqlQuery(IQuery query, IQueryQueue queryQueue, string tableName)
        {
            _query = query;
            _queryQueue = queryQueue;
            _tableName = tableName;
            _visit = new ExpressionVisit<TEntity>(query, queryQueue, new SqlServerExpressionNew<TEntity>(query, queryQueue), new SqlServerExpressionBool<TEntity>(query, queryQueue));
        }

        public void ToInfo()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);
            var strOrderBySql = _visit.OrderBy(_queryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT TOP 1 {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDER BY {0} ", strOrderBySql));
            }
        }

        public void ToList()
        {
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);
            var strOrderBySql = _visit.OrderBy(_queryQueue.ExpOrderBy);

            _queryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDER BY {0} ", strOrderBySql));
            }
        }

        public void Count()
        {
            _queryQueue.Sql = new StringBuilder();
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);

            _queryQueue.Sql.Append(string.Format("SELECT Count(0) "));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Sum()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT SUM({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Max()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT MAX({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Min()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT MIN({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Value()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = _visit.Select(_queryQueue.ExpSelect);
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);
            var strOrderBySql = _visit.OrderBy(_queryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT TOP 1 {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _query.DbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDER BY {0} ", strOrderBySql));
            }
        }

        public void Delete()
        {
            _queryQueue.Sql = new StringBuilder();
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);

            _queryQueue.Sql.AppendFormat("DELETE FROM {0} ", _query.DbProvider.KeywordAegis(_tableName));
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql)); }
        }

        public void Insert(TEntity entity)
        {
            _queryQueue.Sql = new StringBuilder();
            var strinsertAssemble = _visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _tableName); }

            _queryQueue.Sql.AppendFormat("INSERT INTO {0} ", _tableName);
            _queryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", _tableName); }
        }

        public void InsertIdentity(TEntity entity)
        {
            _queryQueue.Sql = new StringBuilder();
            var strinsertAssemble = _visit.Insert(entity);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _tableName); }

            _queryQueue.Sql.AppendFormat("INSERT INTO {0} ", _tableName);
            _queryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF; ", _tableName); }

            _queryQueue.Sql.AppendFormat("SELECT @@IDENTITY;");
        }

        public void Update(TEntity entity)
        {
            _queryQueue.Sql = new StringBuilder();
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);
            var strAssemble = _visit.Assign(entity);

            _queryQueue.Sql.AppendFormat("UPDATE {0} SET ", _query.DbProvider.KeywordAegis(_tableName));
            _queryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }
        }

        public void AddUp()
        {
            _queryQueue.Sql = new StringBuilder();
            var strWhereSql = _visit.Where(_queryQueue.ExpWhere);
            var strAssemble = _visit.Assign(_queryQueue.ExpAssign);

            _queryQueue.Sql.AppendFormat("UPDATE {0} SET ", _query.DbProvider.KeywordAegis(_tableName));
            _queryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }
        }

        public void BulkCopy(List<TEntity> lst)
        {
            throw new NotImplementedException();
        }
    }
}