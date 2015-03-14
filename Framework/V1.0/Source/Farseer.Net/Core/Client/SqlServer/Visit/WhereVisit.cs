using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Visit
{
    public class WhereVisit<TEntity> : DbVisit<TEntity> where TEntity : class, new()
    {
        public WhereVisit(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam) : base(queryQueue, dbProvider, lstParam) { }

        public string Execute(Expression exp, ref IList<DbParameter> param)
        {
            Visit(exp);

            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        protected override Expression Visit(Expression exp)
        {
            throw new System.NotImplementedException();
        }
    }
}
