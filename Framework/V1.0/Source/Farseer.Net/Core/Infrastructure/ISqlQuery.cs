using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库支持的SQL方法
    /// </summary>
    public interface ISqlQuery
    {
        /// <summary>
        /// 查询单条记录
        /// </summary>
        void ToInfo();
        /// <summary>
        /// 查询多条记录
        /// </summary>
        void ToList();
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
        void InsertIdentity<TEntity>(TEntity entity) where TEntity : class, new();
        /// <summary>
        /// 修改
        /// </summary>
        void Update<TEntity>(TEntity entity) where TEntity : class,new();
        /// <summary>
        /// 查询数量
        /// </summary>
        void Count();

        /// <summary>
        /// 累计和
        /// </summary>
        void Sum();
        /// <summary>
        /// 查询最大数
        /// </summary>
        void Max();
        /// <summary>
        /// 查询最小数
        /// </summary>
        void Min();
        /// <summary>
        /// 查询单个值
        /// </summary>
        void Value();
        /// <summary>
        /// 添加或者减少某个字段
        /// </summary>
        void AddUp();
        /// <summary>
        /// 使用数据库特性进行大批量插入操作
        /// </summary>
        void BulkCopy<TEntity>(List<TEntity> lst) where TEntity : class,new();
    }
}
