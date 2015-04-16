using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Core.Infrastructure;
using FS.Extend;

namespace FS.Core.Data.View
{
    public class ViewQueueManger : IQueueManger
    {
        public DbExecutor DataBase { get; internal set; }
        public DbProvider DbProvider { get; set; }
        public ViewQueueManger(DbExecutor database)
        {
            DataBase = database;
            DbProvider = DbFactory.CreateDbProvider(database.DataType);
            Clear();
        }

        private ViewQueue _queue;
        /// <summary>
        /// 获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="name">表名称</param>
        public ViewQueue GetQueue(string name)
        {
            return _queue ?? (_queue = new ViewQueue(0, name));
        }

        public List<DbParameter> Param
        {
            get
            {
                return _queue.Param;
            }
        }

        private void Clear()
        {
            _queue = null;
        }

        /// <summary>
        /// 创建SQL查询
        /// </summary>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public IDbSqlQuery<TEntity> SqlQuery<TEntity>(IQueueSql queue) where TEntity : class,new()
        {
            return DbProvider.CreateSqlQuery<TEntity>(this, queue);
        }
        public int Execute(IQueueSql queue)
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            var result = queue.Sql.Length < 1 ? 0 : DataBase.ExecuteNonQuery(CommandType.Text, queue.Sql.ToString(), param);

            Clear();
            return result;
        }
        public List<TEntity> ExecuteList<TEntity>(IQueueSql queue) where TEntity : class, new()
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            List<TEntity> lst;
            using (var reader = DataBase.GetReader(CommandType.Text, queue.Sql.ToString(), param))
            {
                lst = reader.ToList<TEntity>();
                reader.Close();
            }
            DataBase.Close(false);

            Clear();
            return lst;
        }
        public TEntity ExecuteInfo<TEntity>(IQueueSql queue) where TEntity : class, new()
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            TEntity t;
            using (var reader = DataBase.GetReader(CommandType.Text, queue.Sql.ToString(), param))
            {
                t = reader.ToInfo<TEntity>();
                reader.Close();
            }
            DataBase.Close(false);

            Clear();
            return t;
        }
        public T ExecuteQuery<T>(IQueueSql queue, T defValue = default(T))
        {
            var param = queue.Param == null ? null : queue.Param.ToArray();
            var value = DataBase.ExecuteScalar(CommandType.Text, queue.Sql.ToString(), param);
            var t = (T)Convert.ChangeType(value, typeof(T));

            Clear();
            return t;
        }
    }
}
