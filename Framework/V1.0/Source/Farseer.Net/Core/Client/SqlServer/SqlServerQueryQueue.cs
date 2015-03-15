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
        private readonly IQuery _query;
        public Dictionary<Expression, bool> ExpOrderBy { get; set; }
        public Guid ID { get; set; }
        public int Index { get; set; }
        public List<Expression> ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public Expression ExpAssign { get; set; }
        public StringBuilder Sql { get; set; }
        public List<DbParameter> Param { get; set; }
        public Action<IQueryQueue> LazyAct { get; set; }
        public SqlServerQueryQueue(int index, IQuery queryProvider)
        {
            ID = Guid.NewGuid();
            Index = index;
            _query = queryProvider;
        }

        public ISqlQuery<TEntity> SqlQuery<TEntity>() where TEntity : class,new()
        {
            return new SqlServerSqlQuery<TEntity>(_query, this, _query.TableContext.TableName);
        }

        public void Append()
        {
            _query.Append();
        }
        public int Execute()
        {
            var param = Param == null ? null : Param.ToArray();
            var result = Sql.Length < 1 ? 0 : _query.TableContext.Database.ExecuteNonQuery(CommandType.Text, Sql.ToString(), param);
            _query.Clear();
            return result;
        }
        public List<T> ExecuteList<T>() where T : class, new()
        {
            var param = Param == null ? null : Param.ToArray();
            List<T> lst;
            using (var reader = _query.TableContext.Database.GetReader(CommandType.Text, Sql.ToString(), param))
            {
                lst = reader.ToList<T>();
                reader.Close();
            }
            _query.Clear();
            return lst;
        }
        public T ExecuteInfo<T>() where T : class, new()
        {
            var param = Param == null ? null : Param.ToArray();
            T t;
            using (var reader = _query.TableContext.Database.GetReader(CommandType.Text, Sql.ToString(), param))
            {
                t = reader.ToInfo<T>();
                reader.Close();
            }
            _query.Clear();
            return t;
        }
        public T ExecuteQuery<T>()
        {
            var param = Param == null ? null : Param.ToArray();
            var value = _query.TableContext.Database.ExecuteScalar(CommandType.Text, Sql.ToString(), param);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void Dispose()
        {
            if (Sql != null) { Sql.Clear(); Sql = null; }

            ExpOrderBy = null;
            ExpSelect = null;
            ExpWhere = null;

            GC.SuppressFinalize(this);
        }
    }
}
