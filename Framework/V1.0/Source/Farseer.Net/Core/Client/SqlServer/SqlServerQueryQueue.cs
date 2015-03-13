using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;
using FS.Extend;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQueryQueue : IQueryQueue
    {
        private readonly IQuery _queryProvider;
        public Expression ExpOrderBy { get; set; }
        public int Index { get; set; }
        public Expression ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public Expression ExpAssign { get; set; }
        public StringBuilder Sql { get; set; }
        public IList<DbParameter> Param { get; set; }
        
        public Action LazyAct { get; set; }
        public SqlServerQueryQueue(int index, IQuery queryProvider)
        {
            Index = index;
            _queryProvider = queryProvider;
        }

        private ISqlQuery _sqlQuery;
        public ISqlQuery SqlQuery { get { return _sqlQuery ?? (_sqlQuery = new SqlServerSqlQuery(this, _queryProvider.DbProvider, Param, _queryProvider.TableContext.TableName)); } }

        public void Append()
        {
            _queryProvider.Append();
        }
        public int Execute()
        {
            var param = Param == null ? null : ((List<DbParameter>) Param).ToArray();
            var result = Sql.Length < 1 ? 0 : _queryProvider.TableContext.Database.ExecuteNonQuery(CommandType.Text, Sql.ToString(), param);
            _queryProvider.Clear();
            return result;
        }
        public List<T> ExecuteList<T>() where T : class, new()
        {
            var param = Param == null ? null : ((List<DbParameter>)Param).ToArray();
            List<T> lst ;
            using (var reader = _queryProvider.TableContext.Database.GetReader(CommandType.Text, Sql.ToString(), param))
            {
                lst = reader.ToList<T>();
                reader.Close();
            }
            _queryProvider.Clear();
            return lst;
        }
        public T ExecuteInfo<T>() where T : class, new()
        {
            var param = Param == null ? null : ((List<DbParameter>)Param).ToArray();
            T t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(CommandType.Text, Sql.ToString(), param))
            {
                t = reader.ToInfo<T>();
                reader.Close();
            }
            _queryProvider.Clear();
            return t;
        }
        public T ExecuteQuery<T>()
        {
            var param = Param == null ? null : ((List<DbParameter>)Param).ToArray();
            var value = _queryProvider.TableContext.Database.ExecuteScalar(CommandType.Text, Sql.ToString(), param);
            return (T)Convert.ChangeType(value, typeof (T));
        }

        public void Dispose()
        {
            if (Sql != null) { Sql.Clear(); Sql = null; }
            if (_sqlQuery != null) { _sqlQuery = null; }

            ExpOrderBy = null;
            ExpSelect = null;
            ExpWhere = null;

            GC.SuppressFinalize(this);
        }
    }
}
