using System.Collections.Generic;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库持久化
    /// </summary>
    public interface IQueryTable : IQueryView
    {
        /// <summary>
        /// 当前所有持久化列表
        /// </summary>
        List<IQueueTable> GroupQueueList { get; set; }

        /// <summary>
        /// 根据索引，返回IQueryQueue
        /// </summary>
        /// <param name="index"></param>
        IQueueTable GetQueue(int index);

        new IQueueTable Queue { get; }

        /// <summary>
        /// 提交所有GetQueue，完成数据库交互
        /// </summary>
        int Commit();

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        void Append();
    }
}