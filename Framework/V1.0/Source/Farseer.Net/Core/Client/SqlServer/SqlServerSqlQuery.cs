using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Visit;
using FS.Core.Infrastructure;
using FS.Core.Visit;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery<TEntity> : ISqlQuery<TEntity> where TEntity : class,new()
    {
        private readonly IQueryQueue _queryQueue;
        private readonly string _tableName;
        private readonly IQuery _query;

        public SqlServerSqlQuery(IQuery query, IQueryQueue queryQueue, string tableName)
        {
            _query = query;
            _queryQueue = queryQueue;
            _tableName = tableName;
        }

        public void ToInfo()
        {
            _queryQueue.Sql = new StringBuilder();
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);
            var strOrderBySql = new SqlServerOrderByVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpOrderBy);


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
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);
            var strOrderBySql = new SqlServerOrderByVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpOrderBy);

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
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);

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
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);


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
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);


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
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);


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
            var strSelectSql = new SqlServerSelectVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);
            var strOrderBySql = new SqlServerOrderByVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpOrderBy);


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
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);

            _queryQueue.Sql.AppendFormat("DELETE FROM {0} ", _query.DbProvider.KeywordAegis(_tableName));
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql)); }
        }

        public void Insert(TEntity entity)
        {
            _queryQueue.Sql = new StringBuilder();
            var strinsertAssemble = new SqlServerInsertVisit(_query, _queryQueue).Execute(entity);

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
            var strinsertAssemble = new SqlServerInsertVisit(_query, _queryQueue).Execute(entity);

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
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);
            var strAssemble = new SqlServerAssignVisit(_query, _queryQueue).Execute(entity);

            _queryQueue.Sql.AppendFormat("UPDATE {0} SET ", _query.DbProvider.KeywordAegis(_tableName));
            _queryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }
        }

        public void AddUp()
        {
            _queryQueue.Sql = new StringBuilder();
            var strWhereSql = new SqlServerWhereVisit<TEntity>(_query, _queryQueue).Execute(_queryQueue.ExpWhere);
            var strAssemble = new SqlServerAssignVisit(_query, _queryQueue).Execute(_queryQueue.ExpAssign);

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