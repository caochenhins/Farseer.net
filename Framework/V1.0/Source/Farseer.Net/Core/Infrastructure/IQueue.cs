using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// 当前队列的ID
        /// </summary>
        Guid ID { get; set; }
        /// <summary>
        /// 当前组索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; set; }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        string Name { get; set; }
    }
}
