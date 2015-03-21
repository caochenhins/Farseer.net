using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Context;
using FS.Core.Infrastructure;

namespace FS.Core.Client
{
    public class DbQueryTable : IQueryTable
    {
        public DbContext Context { get; private set; }
        public DbProvider DbProvider { get; set; }
        public DbQueryTable(DbContext tableContext, DbProvider dbProvider)
        {
            Context = tableContext;
            GroupQueueList = new List<IQueueTable>();
            DbProvider = dbProvider;
            Clear();
        }

        /// <summary>
        /// 组列表
        /// </summary>
        public List<IQueueTable> GroupQueueList { get; set; }
        private IQueueTable _queryQueue;
        public IQueueTable Queue { get { return _queryQueue ?? (_queryQueue = DbProvider.CreateQueryQueue(GroupQueueList.Count, this)); } }
        public IQueueTable GetQueue(int index)
        {
            return GroupQueueList[index];
        }

        public List<DbParameter> Param
        {
            get
            {
                var lst = new List<DbParameter>();
                //if (GroupQueueList.Count == 0) { return null; }
                GroupQueueList.Where(o => o.Param != null).Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        public void Append()
        {
            if (_queryQueue != null) { GroupQueueList.Add(_queryQueue); }
            Clear();
        }

        public int Commit()
        {
            var sb = new StringBuilder();
            foreach (var queryQueue in GroupQueueList)
            {
                // 查看是否延迟加载
                if (queryQueue.LazyAct != null) { queryQueue.LazyAct(queryQueue); }
                if (queryQueue.Sql != null) { sb.AppendLine(queryQueue.Sql + ";"); }
            }

            if (Param.Count > DbProvider.ParamsMaxLength) { throw new Exception(string.Format("SQL参数过多，当前数据库类型，最多支持：{0}个，目前生成了{1}个", DbProvider.ParamsMaxLength, Param.Count)); }
            var result = Context.Database.ExecuteNonQuery(CommandType.Text, sb.ToString(), Param == null ? null : Param.ToArray());

            // 清除队列
            GroupQueueList.ForEach(o => o.Dispose());
            GroupQueueList.Clear();
            Clear();
            return result;
        }

        IQueueView IQueryView.Queue
        {
            get { return Queue; }
        }

        public void Clear()
        {
            if (_queryQueue != null) { _queryQueue = null; }
        }
    }
}
