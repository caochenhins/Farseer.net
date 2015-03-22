namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 视图支持的SQL方法
    /// </summary>
    public interface ISqlQueryProc<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 查询单条记录
        /// </summary>
        void CreateParam(TEntity entity);
    }
}
