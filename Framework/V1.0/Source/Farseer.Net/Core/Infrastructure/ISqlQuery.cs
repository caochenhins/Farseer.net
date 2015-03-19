using System.Collections.Generic;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库支持的SQL方法
    /// </summary>
    public interface ISqlQuery<TEntity> where TEntity : class,new()
    {
        /// <summary>
        /// 查询单条记录
        /// </summary>
        void ToInfo();
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        void ToList(int top = 0, bool isDistinct = false, bool isRand = false);
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        void ToList(int pageSize, int pageIndex, bool isDistinct = false);
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
        /// 查询数量
        /// </summary>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        void Count(bool isDistinct = false);
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
        //void BulkCopy(List<TEntity> lst);
    }
}
