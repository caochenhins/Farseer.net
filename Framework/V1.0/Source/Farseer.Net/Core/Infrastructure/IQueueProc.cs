using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueueProc : IQueue
    {
        /// <summary>
        /// 延迟执行SQL生成
        /// </summary>
        Action<IQueueProc> LazyAct { get; set; }
    }
}
