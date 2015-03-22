using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 每一次的数据库查询，将生成一个新的实例
    /// </summary>
    public interface IQueueView : IQueue
    {
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
        /// 当前生成的SQL
        /// </summary>
        StringBuilder Sql { get; set; }

        ISqlQueryView<TEntity> SqlQuery<TEntity>() where TEntity : class, new();

        /// <summary>
        /// 当前队列立即交互数据库
        /// </summary>
        int Execute();

        /// <summary>
        /// 当前队列立即交互数据库（返回List<TEntity>）
        /// </summary>
        List<TEntity> ExecuteList<TEntity>() where TEntity : class,new();

        /// <summary>
        /// 当前队列立即交互数据库（返回Info）
        /// </summary>
        TEntity ExecuteInfo<TEntity>() where TEntity : class,new();

        /// <summary>
        /// 当前队列立即交互数据库（返回T）
        /// </summary>
        T ExecuteQuery<T>(T defValue = default(T));
    }
}
