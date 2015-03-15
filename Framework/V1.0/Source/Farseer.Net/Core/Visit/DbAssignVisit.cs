using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Context;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Visit
{
    /// <summary>
    /// 组装赋值SQL
    /// </summary>
    public abstract class DbAssignVisit 
    {
        protected readonly IQueryQueue QueryQueue;
        protected readonly IQuery Query;

        public DbAssignVisit(IQuery query, IQueryQueue queryQueue)
        {
            QueryQueue = queryQueue;
            Query = query;
        }

        public string Execute<TEntity>(TEntity entity) where TEntity : class,new()
        {
            var map = TableMapCache.GetMap(entity);
            var sb = new StringBuilder();

            //  迭代实体赋值情况
            foreach (var kic in map.ModelList.Where(o => o.Value.IsDbField))
            {
                // 如果主键有值，则取消修改主键的SQL
                if (kic.Value.Column.IsDbGenerated) { continue; }
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  查找组中是否存在已有的参数，有则直接取出
                var newParam = Query.DbProvider.CreateDbParam(QueryQueue.Index + "_" + kic.Value.Column.Name, obj, Query.Param, QueryQueue.Param);

                //  添加参数到列表
                sb.AppendFormat("{0} = {1} ,", Query.DbProvider.KeywordAegis(kic.Key.Name), newParam.ParameterName);
            }

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        public string Execute(Expression expAssign, ref List<DbParameter> param)
        {
            var sb = new StringBuilder();
            return sb.ToString();

            //  查找组中是否存在已有的参数，有则直接取出
            //var newParam = DbProvider.CreateDbParam(kic.Value.Column.Name, obj, lstParam, param, QueryQueue.Index);
            //sb.AppendFormat("{0} = {0} + {1} ,", DbProvider.KeywordAegis(kic.Key.Name), newParam.ParameterName);


            //  迭代实体赋值情况
            //foreach (var kic in map.ModelList.Where(o => o.Value.IsDbField))
            //{
            //    // 如果主键有值，则取消修改主键的SQL
            //    if (kic.Value.Column.IsDbGenerated) { continue; }
            //    var obj = kic.Key.GetValue(entity, null);
            //    if (obj == null || obj is TableSet<TEntity>) { continue; }

            //    //  查找组中是否存在已有的参数，有则直接取出
            //    var newParam = DbProvider.CreateDbParam(kic.Value.Column.Name, obj, lstParam, param, QueryQueue.Index);

            //    //  添加参数到列表
            //    sb.AppendFormat("{0} = {0} + {1} ,", DbProvider.KeywordAegis(kic.Key.Name), newParam.ParameterName);
            //}

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}