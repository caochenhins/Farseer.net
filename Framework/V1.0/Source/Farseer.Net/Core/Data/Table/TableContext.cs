using System.ComponentModel;
using System.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Data.Table
{
    public class TableContext : BaseContext
    {
        /// <summary>
        /// 使用DB特性设置数据库信息
        /// </summary>
        protected TableContext()
        {
            InstanceProperty();
        }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected TableContext(int dbIndex) : base(dbIndex) { InstanceProperty(); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected TableContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : base(connectionString, dbType, commandTimeout) { InstanceProperty(); }

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
        private void InstanceProperty()
        {
            IsMergeCommand = true;
            QueueManger = new TableQueueManger(DataBase);
            InstanceProperty(this, "TableSet`1");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.DataBase.Dispose();
                QueueManger.DataBase = null;
            }
        }
    }
}
