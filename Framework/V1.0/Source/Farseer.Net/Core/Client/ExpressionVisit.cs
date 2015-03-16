using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Context;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client
{
    /// <summary>
    /// 数据库字段解析器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ExpressionVisit<TEntity> where TEntity : class,new()
    {
        private readonly DbExpressionNewProvider<TEntity> _expFields;
        private readonly DbExpressionBoolProvider<TEntity> _expWhere;
        private readonly IQueryQueue _queryQueue;
        private readonly IQuery _query;

        /// <summary>
        /// 禁止无参数构造器
        /// </summary>
        private ExpressionVisit() { }

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="query">数据库持久化</param>
        /// <param name="queryQueue">每一次的数据库查询，将生成一个新的实例</param>
        /// <param name="expNewProvider">提供ExpressionNew表达式树的解析</param>
        /// <param name="expBoolProvider">提供ExpressionBinary表达式树的解析</param>
        public ExpressionVisit(IQuery query, IQueryQueue queryQueue, DbExpressionNewProvider<TEntity> expNewProvider, DbExpressionBoolProvider<TEntity> expBoolProvider)
        {
            _expFields = expNewProvider;
            _expWhere = expBoolProvider;
            _queryQueue = queryQueue;
            _query = query;
            if (_queryQueue.Param == null) { _queryQueue.Param = new List<DbParameter>(); }
        }

        /// <summary>
        /// 赋值解析器
        /// </summary>
        /// <param name="entity">实体类</param>
        public string Assign(TEntity entity)
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
                var newParam = _query.DbProvider.CreateDbParam(_queryQueue.Index + "_" + kic.Value.Column.Name, obj, _query.Param, _queryQueue.Param);

                //  添加参数到列表
                sb.AppendFormat("{0} = {1} ,", _query.DbProvider.KeywordAegis(kic.Key.Name), newParam.ParameterName);
            }

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 赋值解析器
        /// </summary>
        /// <param name="expAssign">单个字段的赋值</param>
        public string Assign(Dictionary<Expression, object> expAssign)
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
        /// <summary>
        /// 插入字段解析器
        /// </summary>
        /// <param name="entity">实体类</param>
        public string Insert(TEntity entity)
        {
            var map = TableMapCache.GetMap(entity);
            //  字段
            var strFields = new StringBuilder();
            //  值
            var strValues = new StringBuilder();

            //var lstParam = QueryQueue.Param;

            //  迭代实体赋值情况
            foreach (var kic in map.ModelList.Where(o => o.Value.IsDbField))
            {
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  查找组中是否存在已有的参数，有则直接取出

                var newParam = _query.DbProvider.CreateDbParam(_queryQueue.Index + "_" + kic.Value.Column.Name, obj, _query.Param, _queryQueue.Param);

                //  添加参数到列表
                strFields.AppendFormat("{0},", _query.DbProvider.KeywordAegis(kic.Key.Name));
                strValues.AppendFormat("{0},", newParam.ParameterName);
            }
            //QueryQueue.Param = lstParam;
            return "(" + strFields.Remove(strFields.Length - 1, 1) + ") VALUES (" + strValues.Remove(strValues.Length - 1, 1) + ")";
        }
        /// <summary>
        /// 排序解析器
        /// </summary>
        /// <param name="lstExp">多个排序字段(true:正序；false：倒序）</param>
        /// <returns></returns>
        public string OrderBy(Dictionary<Expression, bool> lstExp)
        {
            if (lstExp == null || lstExp.Count == 0) { return null; }
            var sb = new StringBuilder();
            foreach (var keyValue in lstExp)
            {
                _expFields.Visit(keyValue.Key);
                _expFields.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
                _expFields.SqlList.Clear();
                if (sb.Length <= 0) continue;
                sb = sb.Remove(sb.Length - 1, 1); sb.Append(string.Format(" {0}", keyValue.Value ? "ASC," : "DESC,"));
            }

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 字段筛选解析器
        /// </summary>
        /// <param name="lstExp">多个字段</param>
        /// <returns></returns>
        public string Select(List<Expression> lstExp)
        {
            if (lstExp == null || lstExp.Count == 0) { return null; }
            lstExp.ForEach(exp => _expFields.Visit(exp));

            var sb = new StringBuilder();
            _expFields.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
        /// <summary>
        /// 条件解析器
        /// </summary>
        /// <param name="exp">条件</param>
        /// <returns></returns>
        public string Where(Expression exp)
        {
            _expWhere.Visit(exp);

            var sb = new StringBuilder();
            _expWhere.SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}