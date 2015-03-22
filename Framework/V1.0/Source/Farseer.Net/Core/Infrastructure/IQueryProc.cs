namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库持久化
    /// </summary>
    public interface IQueryProc : IQuery
    {
        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        IQueueProc Queue { get; }

        /// <summary>
        /// 清除当前队列
        /// </summary>
        void Clear();
    }
}