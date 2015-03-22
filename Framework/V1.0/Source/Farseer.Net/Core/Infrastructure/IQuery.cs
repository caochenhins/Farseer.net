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
        DbContext Context { get; }

        /// <summary>
        /// 数据库提供者
        /// </summary>
        DbProvider DbProvider { get; set; }

        /// <summary>
        /// 返回所有组队列的参数Param
        /// </summary>
        List<DbParameter> Param { get; }
    }
}