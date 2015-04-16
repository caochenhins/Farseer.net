using FS.Mapping.Table;

namespace FS.Core.Data.View
{
    /// <summary>
    /// 视图上下文
    /// </summary>
    public class ViewContext<TEntity> : ViewContext where TEntity : class, new()
    {
        /// <summary>
        /// 通过TEntity的特性，获取数据库配置
        /// </summary>
        public ViewContext(string tableName = null) : this(0, tableName) { }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        /// <param name="tableName">表名称</param>
        public ViewContext(int dbIndex, string tableName = null) : base(dbIndex) { Init(tableName); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        /// <param name="tableName">表名称</param>
        public ViewContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30, string tableName = null) : base(connectionString, dbType, commandTimeout) { Init(tableName); }
        
        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="database">数据库执行</param>
        /// <param name="tableName">表名称</param>
        public ViewContext(DbExecutor database, string tableName = null) : base(database) { Init(tableName); }

        /// <summary>
        /// 强类型实体对象
        /// </summary>
        public ViewSet<TEntity> Set { get; private set; }

        /// <summary>
        /// 提供快捷的数据库执行
        /// 根据实体类设置的特性，访问数据库
        /// </summary>
        public static ViewSet<TEntity> Data
        {
            get
            {
                return new ViewContext<TEntity>().Set;
            }
        }

        private void Init(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) { tableName = TableMapCache.GetMap(this.GetType()).ClassInfo.Name; }
            Set = new ViewSet<TEntity>(this, tableName);
        }
    }
}