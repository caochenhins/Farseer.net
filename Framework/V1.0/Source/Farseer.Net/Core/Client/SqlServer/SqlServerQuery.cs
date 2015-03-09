using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using FS.Core.Context;
using FS.Core.Infrastructure;
using System.Linq;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQuery : IQuery
    {
        /// <summary>
        /// 组列表
        /// </summary>
        private List<IQueryQueue> GroupQueryQueueList { get; set; }
        public TableContext TableContext { get; private set; }
        public IQueryQueue QueryQueue { get; set; }
        public DbProvider DbProvider { get; set; }
        public SqlServerQuery(TableContext tableContext)
        {
            TableContext = tableContext;
            GroupQueryQueueList = new List<IQueryQueue>();
            DbProvider = new SqlServerProvider();
            Clear();
        }
        public IQueryQueue GetQueryQueue(int index)
        {
            return GroupQueryQueueList[index];
        }

        public IList<DbParameter> Param
        {
            get
            {
                if (GroupQueryQueueList.Count == 0) { return null; }
                var lst = new List<DbParameter>();
                GroupQueryQueueList.Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        public void Append()
        {
            if (QueryQueue != null) { GroupQueryQueueList.Add(QueryQueue); }
            QueryQueue = new SqlServerQueryQueue(GroupQueryQueueList.Count, this);
        }

        public int Commit()
        {
            var sb = new StringBuilder();
            foreach (var queryQueue in GroupQueryQueueList)
            {
                sb.AppendLine(queryQueue.Sql + ";");
            }
            var result = TableContext.Database.ExecuteNonQuery(CommandType.Text, sb.ToString(), Param == null ? null : ((List<DbParameter>)Param).ToArray());

            // 清除队列
            GroupQueryQueueList.ForEach(o => o.Dispose());
            GroupQueryQueueList.Clear();
            return result;
        }

        public void Clear()
        {
            if (QueryQueue != null) { QueryQueue.Dispose(); }
            QueryQueue = new SqlServerQueryQueue(GroupQueryQueueList.Count, this);
        }
    }
}
