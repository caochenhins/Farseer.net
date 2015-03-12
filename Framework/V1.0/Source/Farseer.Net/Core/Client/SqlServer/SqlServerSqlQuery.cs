using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Assemble;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery : ISqlQuery
    {
        private readonly IQueryQueue _queryQueue;
        private readonly DbProvider _dbProvider;
        private readonly string _tableName;
        private readonly IList<DbParameter> _lstParam;

        public SqlServerSqlQuery(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam, string tableName)
        {
            _queryQueue = queryQueue;
            _dbProvider = dbProvider;
            _tableName = tableName;
            _lstParam = lstParam;
        }

        public void ToInfo()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT TOP 1 {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }
        }

        public void ToList()
        {
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpOrderBy);

            _queryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }
        }

        public void Count()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);

            _queryQueue.Sql.Append(string.Format("SELECT Count(0) "));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Sum()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT SUM({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Max()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT MAX({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Min()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            _queryQueue.Sql.Append(string.Format("SELECT MIN({0}) ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }
        }

        public void Value()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("SELECT TOP 1 {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("FROM {0} ", _dbProvider.KeywordAegis(_tableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }
        }

        public void Delete()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);

            _queryQueue.Sql.AppendFormat("DELETE FROM {0} ", _dbProvider.KeywordAegis(_tableName));
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql)); }

            _queryQueue.Param = param;
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strinsertAssemble = new InsertAssemble(_queryQueue, _dbProvider, _lstParam).Execute(entity, ref param);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _tableName); }

            _queryQueue.Sql.AppendFormat("INSERT INTO {0} ", _tableName);
            _queryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", _tableName); }

            _queryQueue.Param = param;
        }

        public void InsertIdentity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strinsertAssemble = new InsertAssemble(_queryQueue, _dbProvider, _lstParam).Execute(entity, ref param);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _tableName); }

            _queryQueue.Sql.AppendFormat("INSERT INTO {0} ", _tableName);
            _queryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF; ", _tableName); }

            _queryQueue.Sql.AppendFormat("SELECT @@IDENTITY;");
            _queryQueue.Param = param;
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);
            var strAssemble = new AssignAssemble(_queryQueue, _dbProvider, _lstParam).Execute(entity, ref param);

            _queryQueue.Sql.AppendFormat("UPDATE {0} SET ", _dbProvider.KeywordAegis(_tableName));
            _queryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }

            _queryQueue.Param = param;
        }

        public void AddUp()
        {
            _queryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpWhere, ref param);
            var strAssemble = new AssignAssemble(_queryQueue, _dbProvider, _lstParam).Execute(_queryQueue.ExpAssign, ref param);

            _queryQueue.Sql.AppendFormat("UPDATE {0} SET ", _dbProvider.KeywordAegis(_tableName));
            _queryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }

            _queryQueue.Param = param;
        }

        public void BulkCopy<TEntity>(List<TEntity> lst) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }
    }
}