using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using FS.Mapping.Table;

namespace FS.Core.Infrastructure
{
    /// <summary>
    ///     数据库表达式树解析器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DbVisit<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     实体类映射
        /// </summary>
        internal TableMap Map = typeof(TEntity);

        /// <summary>
        ///     条件堆栈
        /// </summary>
        protected Stack<string> SqlList = new Stack<string>();

        protected readonly IQueryQueue QueryQueue;
        protected readonly DbProvider DbProvider;
        protected readonly IList<DbParameter> LstParam;

        public DbVisit(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam)
        {
            QueryQueue = queryQueue;
            DbProvider = dbProvider;
            LstParam = lstParam;
        }
        /// <summary>
        /// 访问表达式树
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected abstract Expression Visit(Expression exp);
    }
}