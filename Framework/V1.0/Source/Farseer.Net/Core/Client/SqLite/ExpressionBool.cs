using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqLite
{
    public class ExpressionBool<TEntity> : Common.ExpressionBool<TEntity> where TEntity : class, new()
    {
        public ExpressionBool(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }
    }
}
