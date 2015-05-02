namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 表支持的SQL方法
    /// </summary>
    public interface IBuilderSqlOper<in TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 删除
        /// </summary>
        void Delete();
        /// <summary>
        /// 插入
        /// </summary>
        void Insert(TEntity entity);
        /// <summary>
        /// 插入
        /// </summary>
        void InsertIdentity(TEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        void Update(TEntity entity);
        /// <summary>
        /// 添加或者减少某个字段
        /// </summary>
        void AddUp();
    }
}
