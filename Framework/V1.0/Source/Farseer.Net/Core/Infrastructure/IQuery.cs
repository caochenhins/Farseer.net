using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Context;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库持久化
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        DbContext TableContext { get; }

        /// <summary>
        /// 当前所有持久化列表
        /// </summary>
        //List<IQueryQueue> GroupQueryQueueList { get; set; }

        /// <summary>
        /// 根据索引，返回IQueryQueue
        /// </summary>
        /// <param name="index"></param>
        //IQueryQueue GetQueryQueue(int index);

        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        IQueryQueue QueryQueue { get; }

        /// <summary>
        /// 数据库提供者
        /// </summary>
        DbProvider DbProvider { get; set; }

        /// <summary>
        /// 返回所有组队列的参数Param
        /// </summary>
        List<DbParameter> Param { get; }

        /// <summary>
        /// 提交所有GetQueryQueue，完成数据库交互
        /// </summary>
        int Commit();

        /// <summary>
        /// 清除当前队列
        /// </summary>
        void Clear();

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        void Append();
    }
}