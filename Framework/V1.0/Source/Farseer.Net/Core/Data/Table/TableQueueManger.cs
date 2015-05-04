using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;
using FS.Extend;
using FS.Mapping.Context;

namespace FS.Core.Data.Table
{
    /// <summary>
    /// 队列管理
    /// </summary>
    public class TableQueueManger : IQueueManger
    {
        /// <summary>
        /// 当前所有持久化列表
        /// </summary>
        private readonly List<Queue> _groupQueueList;
        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        private Queue _queue;
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
                _groupQueueList.Where(o => o.Param != null).Select(o => o.Param).ToList().ForEach(o => o.ForEach(oo =>
                {
                    if (!lst.Exists(x => oo.ParameterName == x.ParameterName)) { lst.Add(oo); }
                }));
                return lst;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">数据库操作</param>
        /// <param name="contextMap">映射关系</param>
        public TableQueueManger(DbExecutor database, ContextMap contextMap)
        {
            DataBase = database;
            ContextMap = contextMap;

            DbProvider = DbProvider.CreateInstance(database.DataType);
            _groupQueueList = new List<Queue>();
            Clear();
        }

        /// <summary>
        /// 获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="name">表名称</param>
        public Queue GetQueue(string name, FieldMap map)
        {
            return _queue ?? (_queue = new Queue(_groupQueueList.Count, name, map, this));
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
            foreach (var queryQueue in _groupQueueList)
            {
                // 查看是否延迟加载
                if (queryQueue.LazyAct != null) { queryQueue.LazyAct(queryQueue); }
                if (queryQueue.Sql != null) { sb.AppendLine(queryQueue.Sql + ";"); }
            }

            if (Param.Count > DbProvider.ParamsMaxLength) { throw new Exception(string.Format("SQL参数过多，当前数据库类型，最多支持：{0}个，目前生成了{1}个", DbProvider.ParamsMaxLength, Param.Count)); }
            var result = DataBase.ExecuteNonQuery(CommandType.Text, sb.ToString(), Param == null ? null : Param.ToArray());

            // 清除队列
            _groupQueueList.ForEach(o => o.Dispose());
            _groupQueueList.Clear();
            Clear();
            return result;
        }

        /// <summary>
        /// 清除当前队列
        /// </summary>
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