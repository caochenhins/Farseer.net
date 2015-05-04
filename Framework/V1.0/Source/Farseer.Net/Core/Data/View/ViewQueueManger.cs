using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Context;

namespace FS.Core.Data.View
{
    public class ViewQueueManger : IQueueManger
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        public DbExecutor DataBase { get; private set; }
        /// <summary>
        /// 数据库提供者（不同数据库的特性）
        /// </summary>
        public DbProvider DbProvider { get; set; }
        /// <summary>
        /// 映射关系
        /// </summary>
        public ContextMap ContextMap { get; set; }
        private Queue _queue;

        public ViewQueueManger(DbExecutor database, ContextMap contextMap)
        {
            DataBase = database;
            ContextMap = contextMap;
            DbProvider = DbProvider.CreateInstance(database.DataType);
            Clear();
        }

        /// <summary>
        /// 获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="name">表名称</param>
        public Queue GetQueue(string name, FieldMap map)
        {
            return _queue ?? (_queue = new Queue(0, name, map, this));
        }

        public int Commit() { return -1; }

        public List<DbParameter> Param
        {
            get
            {
                return _queue.Param;
            }
        }

        public void Clear()
        {
            _queue = null;
        }

        public int Execute(IQueue queue)
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            var result = queue.Sql.Length < 1 ? 0 : DataBase.ExecuteNonQuery(CommandType.Text, queue.Sql.ToString(), param);

            Clear();
            return result;
        }
        public DataTable ExecuteTable(IQueue queue)
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            var table = DataBase.GetDataTable(CommandType.Text, queue.Sql.ToString(), param);
            Clear();
            return table;
        }
        public TEntity ExecuteInfo<TEntity>(IQueue queue) where TEntity : class, new()
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            TEntity t;
            using (var reader = DataBase.GetReader(CommandType.Text, queue.Sql.ToString(), param))
            {
                t = reader.ToInfo<TEntity>();
                //reader.Close();
            }
            DataBase.Close(false);

            Clear();
            return t;
        }
        public T ExecuteQuery<T>(IQueue queue, T defValue = default(T))
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            var value = DataBase.ExecuteScalar(CommandType.Text, queue.Sql.ToString(), param);
            var t = (T)Convert.ChangeType(value, typeof(T));

            Clear();
            return t;
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                DataBase.Dispose();
                DataBase = null;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
