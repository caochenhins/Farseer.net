using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 持久化当前数据库查询
    /// </summary>
    public interface IQueryQueue : IDisposable
    {
        /// <summary>
        /// 当前组索引
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// 查询字段的表达式树
        /// </summary>
        Expression ExpSelect { get; set; }
        /// <summary>
        /// 排序字段的表达式树
        /// </summary>
        Expression ExpOrderBy { get; set; }
        /// <summary>
        /// 条件字段的表达式树
        /// </summary>
        Expression ExpWhere { get; set; }
        /// <summary>
        /// 当前生成的SQL
        /// </summary>
        StringBuilder Sql { get; set; }
        /// <summary>
        /// 当前生成的参数
        /// </summary>
        IList<DbParameter> Param { get; set; }

        /// <summary>
        /// 非合并SQL下，立即执行
        /// </summary>
        /// <returns></returns>
        int Execute();
    }
}
