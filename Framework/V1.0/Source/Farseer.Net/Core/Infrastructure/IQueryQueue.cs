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
    public interface IQueryQueue : IDisposable
    {
        /// <summary>
        /// 当前组索引
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// 查询字段的表达式树
        /// </summary>
        List<Expression> ExpSelect { get; set; }
        /// <summary>
        /// 排序字段的表达式树(true:正序；false：倒序）
        /// </summary>
        Dictionary<Expression, bool> ExpOrderBy { get; set; }
        /// <summary>
        /// 条件字段的表达式树
        /// </summary>
        Expression ExpWhere { get; set; }
        /// <summary>
        /// 赋值字段的表达式树
        /// </summary>
        Expression ExpAssign { get; set; }
        /// <summary>
        /// 当前生成的SQL
        /// </summary>
        StringBuilder Sql { get; set; }
        /// <summary>
        /// 当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; set; }

        /// <summary>
        /// 当前队列立即交互数据库
        /// </summary>
        int Execute();

        /// <summary>
        /// 当前队列立即交互数据库（返回List<T>）
        /// </summary>
        List<T> ExecuteList<T>() where T : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回Info）
        /// </summary>
        T ExecuteInfo<T>() where T : class, new();

        /// <summary>
        /// 当前队列立即交互数据库（返回T）
        /// </summary>
        T ExecuteQuery<T>();

        /// <summary>
        /// 延迟执行SQL生成
        /// </summary>
        Action LazyAct { get; set; }

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        void Append();

        ISqlQuery<TEntity> SqlQuery<TEntity>() where TEntity : class, new();
    }
}
