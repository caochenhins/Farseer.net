using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Context;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer.Assemble
{
    /// <summary>
    /// 组装赋值SQL
    /// </summary>
    public class InsertAssemble : SqlAssemble
    {
        public InsertAssemble(IQuery queryProvider) : base(queryProvider) { }

        public string Execute<TEntity>(TEntity entity, ref IList<DbParameter> param) where TEntity : class,new()
        {
            var map = TableMapCache.GetMap(entity);
            var lstParam = (List<DbParameter>)QueryProvider.Param ?? new List<DbParameter>();
            //  字段
            var strFields = new StringBuilder();
            //  值
            var strValues = new StringBuilder();

            //  迭代实体赋值情况
            foreach (var kic in map.ModelList.Where(o => o.Value.IsDbField))
            {
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  查找组中是否存在已有的参数，有则直接取出
                var newParam = QueryProvider.DbProvider.CreateDbParam(kic.Value.Column.Name, obj, lstParam, param, QueryProvider.QueryQueue.Index);

                //  添加参数到列表
                strFields.AppendFormat("{0},", QueryProvider.DbProvider.KeywordAegis(kic.Key.Name));
                strValues.AppendFormat("{0},", newParam.ParameterName);
            }

            return "(" + strFields.Remove(strFields.Length - 1, 1) + ") VALUES (" + strValues.Remove(strValues.Length - 1, 1) + ")";
        }
    }
}