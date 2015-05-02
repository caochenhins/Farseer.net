namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 表支持的SQL方法
    /// </summary>
    public interface IBuilderSqlOper : IBuilderSqlQuery
    {
        /// <summary>
        /// 删除
        /// </summary>
        void Delete();
        /// <summary>
        /// 插入
        /// </summary>
        void Insert<TEntity>(TEntity entity) where TEntity : class,new();
        /// <summary>
        /// 插入
        /// </summary>
        void InsertIdentity<TEntity>(TEntity entity) where TEntity : class,new();
        /// <summary>
        /// 修改
        /// </summary>
        void Update<TEntity>(TEntity entity) where TEntity : class,new();
        /// <summary>
        /// 添加或者减少某个字段
        /// </summary>
        void AddUp();
    }
}
