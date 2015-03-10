using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Infrastructure
{
    public interface ISqlQuery
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        void Delete();
        /// <summary>
        /// 生成SQL
        /// </summary>
        T ToInfo<T>() where T : class, new();
        /// <summary>
        /// 生成SQL
        /// </summary>
        void Insert<T>(T entity) where T : class,new();
        /// <summary>
        /// 生成SQL
        /// </summary>
        List<T> ToList<T>() where T : class, new();
        /// <summary>
        /// 生成SQL
        /// </summary>
        void Update<T>(T entity) where T : class,new();
    }
}
