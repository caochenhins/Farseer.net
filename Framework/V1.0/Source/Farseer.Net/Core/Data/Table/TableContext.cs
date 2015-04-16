using System;
using System.Data;
using System.Data.Linq.Mapping;
using FS.Configs;

namespace FS.Core.Data.Table
{
    public class TableContext : IDisposable
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected TableContext(int dbIndex = 0) : this(DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected TableContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : this(new DbExecutor(connectionString, dbType, commandTimeout)) { }

        /// <summary>
        /// 数据库
        /// </summary>
        /// <param name="database">数据库执行</param>
        protected TableContext(DbExecutor database)
        {
            IsMergeCommand = true;
            QueueManger = new TableQueueManger(database);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.DataBase.Dispose();
                QueueManger.DataBase = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 队列管理
        /// </summary>
        protected internal TableQueueManger QueueManger { get; private set; }

        /// <summary>
        /// true:启用合并执行命令、并延迟加载
        /// </summary>
        internal protected bool IsMergeCommand { get; set; }

        /// <summary>
        /// 保存修改
        /// IsMergeCommand=true时：只提交一次SQL到数据库
        /// </summary>
        /// <param name="isOlation">默认启用事务操作</param>
        public int SaveChanges(bool isOlation = true)
        {
            // 开启或关闭事务
            if (isOlation) { QueueManger.DataBase.OpenTran(IsolationLevel.Serializable); }
            else { QueueManger.DataBase.CloseTran(); }

            var result = QueueManger.Commit();
            // 如果开启了事务，则关闭
            if (isOlation)
            {
                QueueManger.DataBase.Commit();
                QueueManger.DataBase.CloseTran();
            }
            return result;
        }

        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        public virtual void InstanceProperty()
        {
            var types = this.GetType().GetProperties();
            foreach (var type in types)
            {
                var a = type.PropertyType.Name == "TableSet`1";
                var user = Activator.CreateInstance(type.PropertyType, true);


                //var xx = this.GetType().InvokeMember(type.PropertyType);
                //type.SetValue()
            }
        }
    }

    public class UserXVO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Column(IsDbGenerated = true)]
        public int? ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        public int? LoginCount { get; set; }

        /// <summary>
        /// 登陆IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 登陆IP
        /// </summary>
        [Column(Name = "getdate()")]
        public string GetDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
