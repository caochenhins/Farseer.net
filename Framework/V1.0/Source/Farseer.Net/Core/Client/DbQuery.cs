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
    public class DbQuery : IQuery
    {
        public DbProvider DbProvider { get; set; }
        public TableContext TableContext { get; private set; }
        public DbQuery(TableContext tableContext, DbProvider dbProvider)
        {
            TableContext = tableContext;
            GroupQueryQueueList = new List<IQueryQueue>();
            DbProvider = dbProvider;
            Clear();
        }

        /// <summary>
        /// 组列表
        /// </summary>
        private List<IQueryQueue> GroupQueryQueueList { get; set; }
        private IQueryQueue _queryQueue;
        public IQueryQueue QueryQueue { get { return _queryQueue ?? (_queryQueue = DbProvider.CreateQueryQueue(GroupQueryQueueList.Count, this)); } }
        public IQueryQueue GetQueryQueue(int index)
        {
            return GroupQueryQueueList[index];
        }

        public List<DbParameter> Param
        {
            get
            {
                var lst = new List<DbParameter>();
                //if (GroupQueryQueueList.Count == 0) { return null; }
                GroupQueryQueueList.Where(o => o.Param != null).Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        public void Append()
        {
            if (_queryQueue != null) { GroupQueryQueueList.Add(_queryQueue); }
            Clear();
        }

        public int Commit()
        {
            var sb = new StringBuilder();
            foreach (var queryQueue in GroupQueryQueueList)
            {
                // 查看是否延迟加载
                if (queryQueue.LazyAct != null) { queryQueue.LazyAct(queryQueue); }
                if (queryQueue.Sql != null) { sb.AppendLine(queryQueue.Sql + ";"); }
            }

            if (Param.Count > DbProvider.ParamsMaxLength) { throw new Exception(string.Format("SQL参数过多，当前数据库类型，最多支持：{0}个，目前生成了{1}个", DbProvider.ParamsMaxLength, Param.Count)); }
            var result = TableContext.Database.ExecuteNonQuery(CommandType.Text, sb.ToString(), Param == null ? null : Param.ToArray());

            // 清除队列
            GroupQueryQueueList.ForEach(o => o.Dispose());
            GroupQueryQueueList.Clear();
            Clear();
            return result;
        }

        public void Clear()
        {
            if (_queryQueue != null) { _queryQueue = null; }
        }
    }
}
