using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 包含数据库SQL操作的队列
    /// </summary>
    public interface IQueueSql : IQueue
    {
        StringBuilder Sql { get; set; }
        Dictionary<Expression, bool> ExpOrderBy { get; set; }
        List<Expression> ExpSelect { get; set; }
        Expression ExpWhere { get; set; }
    }
}