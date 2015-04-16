using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Data;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 队列管理模块
    /// </summary>
    public interface IQueueManger
    {
        /// <summary>
        /// 数据库提供者（不同数据库的特性）
        /// </summary>
        DbProvider DbProvider { get; set; }

        /// <summary>
        /// 返回所有队列的参数Param
        /// </summary>
        List<DbParameter> Param { get; }

        /// <summary>
        /// 数据库操作
        /// </summary>
        DbExecutor DataBase { get;  }
    }
}