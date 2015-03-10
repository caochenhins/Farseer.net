using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Client.SqlServer.Assemble;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerSqlQuery : ISqlQuery
    {
        private readonly IQuery _queryProvider;

        public SqlServerSqlQuery(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public void Delete()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpWhere, ref param);

            _queryProvider.QueryQueue.Sql.AppendFormat("DELETE FROM {0} ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName));
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryProvider.QueryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql)); }

            _queryProvider.QueryQueue.Param = param;

            // 非合并提交，则直接提交
            if (!_queryProvider.TableContext.IsMergeCommand) { _queryProvider.QueryQueue.Execute(); return; }

            _queryProvider.Append();
        }

        public T ToInfo<T>() where T : class, new()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("SELECT TOP 1 {0} ", strSelectSql));

            _queryProvider.QueryQueue.Sql.Append(string.Format("FROM {0} ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }
            T t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString()))
            {
                t = reader.ToInfo<T>();
                reader.Close();
            }
            _queryProvider.Clear();
            return t;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strinsertAssemble = new InsertAssemble(_queryProvider).Execute(entity, ref param);

            var map = TableMapCache.GetMap(entity);

            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryProvider.QueryQueue.Sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", _queryProvider.TableContext.TableName); }

            _queryProvider.QueryQueue.Sql.AppendFormat("INSERT INTO {0} ", _queryProvider.TableContext.TableName);
            _queryProvider.QueryQueue.Sql.Append(strinsertAssemble);

            if (!string.IsNullOrWhiteSpace(map.IndexName) && indexHaveValue) { _queryProvider.QueryQueue.Sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", _queryProvider.TableContext.TableName); }

            _queryProvider.QueryQueue.Param = param;

            // 非合并提交，则直接提交
            if (!_queryProvider.TableContext.IsMergeCommand) { _queryProvider.QueryQueue.Execute(); return; }

            _queryProvider.Append();
        }

        public List<T> ToList<T>() where T : class, new()
        {
            IList<DbParameter> param = new List<DbParameter>();
            var strSelectSql = new SelectAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpWhere, ref param);
            var strOrderBySql = new OrderByAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpOrderBy);

            _queryProvider.QueryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("SELECT {0} ", strSelectSql));

            _queryProvider.QueryQueue.Sql.Append(string.Format("FROM {0} ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("WHERE {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("ORDERBY {0} ", strOrderBySql));
            }

            List<T> t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString()))
            {
                t = reader.ToList<T>();
                reader.Close();
            }
            _queryProvider.Clear();
            return t;
        }

        public void Update<T>(T entity) where T : class, new()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();
            IList<DbParameter> param = new List<DbParameter>();
            var strWhereSql = new WhereAssemble(_queryProvider).Execute(_queryProvider.QueryQueue.ExpWhere, ref param);
            var strAssemble = new AssignAssemble(_queryProvider).Execute(entity, ref param);

            _queryProvider.QueryQueue.Sql.AppendFormat("UPDATE {0} SET ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName));
            _queryProvider.QueryQueue.Sql.Append(strAssemble);
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { _queryProvider.QueryQueue.Sql.Append(string.Format(" WHERE {0} ", strWhereSql)); }

            _queryProvider.QueryQueue.Param = param;

            // 非合并提交，则直接提交
            if (!_queryProvider.TableContext.IsMergeCommand) { _queryProvider.QueryQueue.Execute(); return; }

            _queryProvider.Append();
        }
    }
}
