using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Queue
{
    public class DbQueueProc : IQueueProc
    {
        private readonly IQueryProc _query;
        public Guid ID { get; set; }
        public int Index { get; set; }
        public List<DbParameter> Param { get; set; }

        public DbQueueProc(int index, IQueryProc queryProvider)
        {
            ID = Guid.NewGuid();
            Index = index;
            _query = queryProvider;
            Param = new List<DbParameter>();
        }

        /// <summary>
        /// 将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="entity">实体类</param>
        private void SetParamToEntity<TEntity>(TEntity entity) where TEntity : class,new()
        {
            if (entity == null) { return; }
            var map = TableMapCache.GetMap(entity);
            foreach (var kic in map.ModelList.Where(o => o.Value.IsOutParam))
            {
                kic.Key.SetValue(entity, Param.Find(o => o.ParameterName == _query.DbProvider.ParamsPrefix + kic.Value.Column.Name).Value.ConvertType(kic.Key.PropertyType), null);
            }
        }
        public int Execute<TEntity>(TEntity entity = null) where TEntity : class,new()
        {
            var param = Param == null ? null : Param.ToArray();
            var result = _query.Context.Database.ExecuteNonQuery(CommandType.StoredProcedure, _query.Context.Name, param);
            SetParamToEntity(entity);

            _query.Clear();
            return result;
        }

        public List<TEntity> ExecuteList<TEntity>(TEntity entity = null) where TEntity : class,new()
        {
            var param = Param == null ? null : Param.ToArray();
            List<TEntity> lst;
            using (var reader = _query.Context.Database.GetReader(CommandType.StoredProcedure, _query.Context.Name, param))
            {
                lst = reader.ToList<TEntity>();
                reader.Close();
            }

            SetParamToEntity(entity);
            _query.Clear();
            return lst;
        }

        public TEntity ExecuteInfo<TEntity>(TEntity entity = null) where TEntity : class,new()
        {
            var param = Param == null ? null : Param.ToArray();
            TEntity t;
            using (var reader = _query.Context.Database.GetReader(CommandType.StoredProcedure, _query.Context.Name, param))
            {
                t = reader.ToInfo<TEntity>();
                reader.Close();
            }

            SetParamToEntity(entity);
            _query.Clear();
            return t;
        }

        public T ExecuteQuery<TEntity, T>(TEntity entity = null, T defValue = default(T)) where TEntity : class,new()
        {
            var param = Param == null ? null : Param.ToArray();
            var value = _query.Context.Database.ExecuteScalar(CommandType.StoredProcedure, _query.Context.Name, param);
            var t = (T)Convert.ChangeType(value, typeof(T));

            SetParamToEntity(entity);
            _query.Clear();
            return t;
        }

        public Action<IQueueProc> LazyAct { get; set; }
        public ISqlQueryProc<TEntity> SqlQuery<TEntity>() where TEntity : class,new()
        {
            return _query.DbProvider.CreateSqlQuery<TEntity>(_query, this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}