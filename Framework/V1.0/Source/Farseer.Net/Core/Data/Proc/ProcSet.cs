﻿using System.Collections.Generic;
using FS.Mapping.Context;

namespace FS.Core.Data.Proc
{
    /// <summary>
    /// 存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ProcSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly ProcContext _context;

        private ProcQueueManger QueueManger { get { return (ProcQueueManger)_context.QueueManger; } }
        private Queue Queue { get { return _context.QueueManger.GetQueue(_name, _map); } }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 实体类映射
        /// </summary>
        private readonly FieldMap _map;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private ProcSet() { }
        public ProcSet(ProcContext context)
        {
            _context = context;
            _map = typeof(TEntity);
            var contextState = _context.ContextMap.GetState(this.GetType());
            _name = contextState.Value.SetAtt.Name;
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public T GetValue<T>(TEntity entity = null, T t = default(T))
        {
            return QueueManger.ExecuteValue(Queue, entity, t);
        }

        /// <summary>
        /// 执行存储过程（不返回值）
        /// </summary>
        public void Execute(TEntity entity = null)
        {
            QueueManger.Execute(Queue, entity);
        }

        /// <summary>
        /// 执行存储过程（返回单条记录）
        /// </summary>
        public TEntity ToEntity(TEntity entity = null)
        {
            return QueueManger.ExecuteInfo(Queue, entity);
        }

        /// <summary>
        /// 执行存储过程（返回多条记录）
        /// </summary>
        public List<TEntity> ToList(TEntity entity = null)
        {
            return QueueManger.ExecuteList(Queue, entity);
        }
    }
}