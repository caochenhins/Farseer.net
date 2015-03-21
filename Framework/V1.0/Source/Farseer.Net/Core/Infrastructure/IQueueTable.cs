using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueueTable : IQueueView
    {
        /// <summary>
        /// 赋值字段的表达式树
        /// </summary>
        Dictionary<Expression, object> ExpAssign { get; set; }

        /// <summary>
        /// 表支持的SQL方法
        /// </summary>
        //void Append();
        new ISqlQueryTable<TEntity> SqlQuery<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 延迟执行SQL生成
        /// </summary>
        Action<IQueueTable> LazyAct { get; set; }
    }
}
