﻿using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class ExpressionNew : DbExpressionNewProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public ExpressionNew(IQueueManger queueManger, IQueue queue) : base(queueManger, queue) { }
    }
}
