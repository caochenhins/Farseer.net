using System.Data;
using FS.Configs;
using FS.Core.Data;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Context
{
    /// <summary>
    /// 存储过程上下文
    /// </summary>
    public class ProcContext<TEntity> : DbContext where TEntity : class, new()
    {
        /// <summary>
        /// 通过TEntity的特性，获取数据库配置
        /// </summary>
        public ProcContext(string tableName = null) : this(TableMapCache.GetMap(typeof(ProcContext<TEntity>)).ClassInfo.ConnStr, TableMapCache.GetMap(typeof(ProcContext<TEntity>)).ClassInfo.DataType, TableMapCache.GetMap(typeof(ProcContext<TEntity>)).ClassInfo.CommandTimeout, tableName) { }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        /// <param name="tableName">表名称</param>
        public ProcContext(int dbIndex, string tableName = null) : this(DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout, tableName) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        /// <param name="tableName">表名称</param>
        public ProcContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30, string tableName = null) : this(new DbExecutor(connectionString, dbType, commandTimeout), tableName) { }

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="database">数据库执行</param>
        /// <param name="tableName">表名称</param>
        public ProcContext(DbExecutor database, string tableName = null) : base(database, tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) { Name = TableMapCache.GetMap<TEntity>().ClassInfo.Name; }
            ProcSet = new ProcSet<TEntity>(this);
            IsMergeCommand = true;
            Query = DbFactory.CreateQueryProc(this);
        }

        /// <summary>
        /// 数据库查询支持
        /// </summary>
        internal protected IQueryProc Query { get; set; }

        /// <summary>
        /// true:启用合并执行命令、并延迟加载
        /// </summary>
        internal protected bool IsMergeCommand { get; private set; }

        /// <summary>
        /// 强类型实体对象
        /// </summary>
        public ProcSet<TEntity> ProcSet { get; private set; }

        /// <summary>
        /// 提供快捷的数据库执行
        /// 根据实体类设置的特性，访问数据库
        /// </summary>
        public static ProcSet<TEntity> Data
        {
            get
            {
                return new ProcContext<TEntity>() { IsMergeCommand = false }.ProcSet;
            }
        }

        /// <summary>
        /// 保存修改
        /// IsMergeCommand=true时：只提交一次SQL到数据库
        /// </summary>
        /// <param name="isOlation">默认启用事务操作</param>
        public void SaveChanges(bool isOlation = true)
        {
            // 开启或关闭事务
            if (isOlation) { Database.OpenTran(IsolationLevel.Serializable); }
            else { Database.CloseTran(); }

            // 如果开启了事务，则关闭
            if (isOlation)
            {
                Database.Commit();
                Database.CloseTran();
            }
        }
    }
}