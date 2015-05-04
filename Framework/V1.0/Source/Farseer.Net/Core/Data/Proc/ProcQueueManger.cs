using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Context;

namespace FS.Core.Data.Proc
{
    /// <summary>
    /// 队列管理
    /// </summary>
    public class ProcQueueManger : IQueueManger
    {
        /// <summary>
        /// 当前所有持久化列表
        /// </summary>
        private readonly List<ProcQueue> _groupQueueList;
        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        private ProcQueue _queue;
        /// <summary>
        /// 数据库操作
        /// </summary>
        public DbExecutor DataBase { get; internal set; }
        /// <summary>
        /// 数据库提供者（不同数据库的特性）
        /// </summary>
        public DbProvider DbProvider { get; set; }
        /// <summary>
        /// 映射关系
        /// </summary>
        public ContextMap ContextMap { get; set; }
        /// <summary>
        /// 所有队列的参数
        /// </summary>
        public List<DbParameter> Param
        {
            get
            {
                var lst = new List<DbParameter>();
                _groupQueueList.Where(o => o.Param != null).Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">数据库操作</param>
        /// <param name="contextMap">映射关系</param>
        public ProcQueueManger(DbExecutor database, ContextMap contextMap)
        {
            DataBase = database;
            ContextMap = contextMap;
            DbProvider = DbProvider.CreateInstance(database.DataType);
            _groupQueueList = new List<ProcQueue>();
            Clear();
        }

        /// <summary>
        /// 获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="name">表名称</param>
        public ProcQueue GetQueue(string name, FieldMap map)
        {
            return _queue ?? (_queue = new ProcQueue(_groupQueueList.Count, name, map, this));
        }

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        public void Append()
        {
            if (_queue != null) { _groupQueueList.Add(_queue); }
            Clear();
        }

        /// <summary>
        /// 提交所有GetQueue，完成数据库交互
        /// </summary>
        public int Commit()
        {
            var sb = new StringBuilder();
            foreach (var queue in _groupQueueList)
            {
                // 查看是否延迟加载
                if (queue.LazyAct != null) { queue.LazyAct(queue); }
                var result = DataBase.ExecuteNonQuery(CommandType.StoredProcedure, queue.Name, queue.Param == null ? null : queue.Param.ToArray());
                queue.Dispose();
            }

            // 清除队列
            _groupQueueList.Clear();
            Clear();
            return 0;
        }

        /// <summary>
        /// 清除当前队列
        /// </summary>
        private void Clear()
        {
            _queue = null;
        }

        /// <summary>
        /// 将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="queue">每一次的数据库查询，将生成一个新的实例</param>
        /// <param name="entity">实体类</param>
        private void SetParamToEntity<TEntity>(IQueue queue, TEntity entity) where TEntity : class,new()
        {
            if (entity == null) { return; }
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            foreach (var kic in map.MapList.Where(o => o.Value.FieldAtt.IsOutParam))
            {
                kic.Key.SetValue(entity, queue.Param.Find(o => o.ParameterName == DbProvider.ParamsPrefix + kic.Value.FieldAtt.Name).Value.ConvertType(kic.Key.PropertyType), null);
            }
        }

        /// <summary>
        /// 存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queue"></param>
        /// <param name="entity"></param>
        private List<DbParameter> CreateParam<TEntity>(IQueue queue, TEntity entity) where TEntity : class,new()
        {
            queue.Param = new List<DbParameter>();
            if (entity == null) { return queue.Param; }

            foreach (var kic in queue.FieldMap.MapList.Where(o => o.Value.FieldAtt.IsInParam || o.Value.FieldAtt.IsOutParam))
            {
                var obj = kic.Key.GetValue(entity, null);

                queue.Param.Add(DbProvider.CreateDbParam(kic.Value.FieldAtt.Name, obj, kic.Key.PropertyType, kic.Value.FieldAtt.IsOutParam));
            }
            return queue.Param;
        }


        public int Execute<TEntity>(IQueue queue, TEntity entity = null) where TEntity : class,new()
        {
            var param = CreateParam(queue, entity).ToArray();
            var result = DataBase.ExecuteNonQuery(CommandType.StoredProcedure, queue.Name, param);
            SetParamToEntity(queue, entity);

            Clear();
            return result;
        }
        public List<TEntity> ExecuteList<TEntity>(IQueue queue, TEntity entity = null) where TEntity : class,new()
        {
            var param = CreateParam(queue, entity).ToArray();
            List<TEntity> lst;
            using (var reader = DataBase.GetReader(CommandType.StoredProcedure, queue.Name, param))
            {
                lst = reader.ToList<TEntity>();
                //reader.Close();
            }
            DataBase.Close(false);
            SetParamToEntity(queue, entity);
            Clear();
            return lst;
        }
        public TEntity ExecuteInfo<TEntity>(IQueue queue, TEntity entity = null) where TEntity : class,new()
        {
            var param = CreateParam(queue, entity).ToArray();
            TEntity t;
            using (var reader = DataBase.GetReader(CommandType.StoredProcedure, queue.Name, param))
            {
                t = reader.ToInfo<TEntity>();
                //reader.Close();
            }
            DataBase.Close(false);

            SetParamToEntity(queue, entity);
            Clear();
            return t;
        }
        public T ExecuteValue<TEntity, T>(IQueue queue, TEntity entity = null, T defValue = default(T)) where TEntity : class, new()
        {
            var param = CreateParam(queue, entity).ToArray();
            var value = DataBase.ExecuteScalar(CommandType.StoredProcedure, queue.Name, param);
            var t = (T)Convert.ChangeType(value, typeof(T));

            SetParamToEntity(queue, entity);
            Clear();
            return t;
        }
    }
}