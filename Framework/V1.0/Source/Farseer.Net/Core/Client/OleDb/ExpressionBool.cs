using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class ExpressionBool<TEntity> : DbExpressionBoolProvider<TEntity> where TEntity : class, new()
    {
        public ExpressionBool(IQuery query, IQueue queryQueue) : base(query, queryQueue) { }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            base.VisitMethodCall(m);
            if (ClearCallSql()) { return m; }

            #region 字段、参数、值类型
            Type fieldType = null;
            Type paramType = null;
            string fieldName = null;
            string paramName = null;

            if (m.Arguments.Count > 0)
            {
                if (m.Object == null)
                {
                    if (!m.Arguments[0].Type.IsGenericType || m.Arguments[0].Type.GetGenericTypeDefinition() == typeof(Nullable<>)) { fieldType = m.Arguments[0].Type; paramType = m.Arguments[1].Type; fieldName = SqlList.Pop(); paramName = SqlList.Pop(); }
                    else { paramType = m.Arguments[0].Type; fieldType = m.Arguments[1].Type; paramName = SqlList.Pop(); fieldName = SqlList.Pop(); }
                }
                else
                {
                    if (!m.Object.Type.IsGenericType || m.Object.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        fieldType = m.Object.Type;
                        if (m.Arguments.Count > 0)
                        {
                            paramType = m.Arguments[0].Type;
                            paramName = SqlList.Pop();
                        }
                        fieldName = SqlList.Pop();
                    }
                    else { paramType = m.Object.Type; fieldType = m.Arguments[0].Type; paramName = SqlList.Pop(); fieldName = SqlList.Pop(); }
                }
            }
            #endregion

            switch (m.Method.Name.ToUpper())
            {
                case "CONTAINS": VisitMethodContains(fieldType, fieldName, paramType, paramName); break;
                case "STARTSWITH": VisitMethodStartswith(fieldType, fieldName, paramType, paramName); break;
                case "ENDSWITH": VisitMethodEndswith(fieldType, fieldName, paramType, paramName); break;
                case "ISEQUALS": VisitMethodIsEquals(fieldType, fieldName, paramType, paramName); break;
                default:
                    {
                        if (m.Arguments.Count == 0 && m.Object != null) { return m; }
                        throw new Exception(string.Format("暂不支持该方法的SQL转换：" + m.Method.Name.ToUpper()));
                    }
            }
            return m;
        }

        /// <summary>
        /// Contains方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        private void VisitMethodContains(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            if (paramType != null && (!paramType.IsGenericType || paramType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                #region 搜索值串的处理
                var param = QueryQueue.Param.Find(o => o.ParameterName == paramName);
                if (param != null && Regex.IsMatch(param.Value.ToString(), @"[\d]+") && (Type.GetTypeCode(fieldType) == TypeCode.Int16 || Type.GetTypeCode(fieldType) == TypeCode.Int32 || Type.GetTypeCode(fieldType) == TypeCode.Decimal || Type.GetTypeCode(fieldType) == TypeCode.Double || Type.GetTypeCode(fieldType) == TypeCode.Int64 || Type.GetTypeCode(fieldType) == TypeCode.UInt16 || Type.GetTypeCode(fieldType) == TypeCode.UInt32 || Type.GetTypeCode(fieldType) == TypeCode.UInt64))
                {
                    param.Value = "," + param.Value + ",";
                    param.DbType = DbType.String;
                    if (Query.DbProvider.KeywordAegis("").Length > 0) { fieldName = "','+" + fieldName.Substring(1, fieldName.Length - 2) + "+','"; }
                    else { fieldName = "','+" + fieldName + "+','"; }
                }
                #endregion

                SqlList.Push(String.Format("CHARINDEX({0},{1}) > 0", paramName, fieldName));
            }
            else
            {

                // not
                var notValue = "";
                if (SqlList.First().Equals("Not")) { notValue = SqlList.Pop(); }

                if (Type.GetTypeCode(fieldType) == TypeCode.String) { CurrentDbParameter.Value = "'" + CurrentDbParameter.Value.ToString().Replace(",", "','") + "'"; }
                SqlList.Push(String.Format("{0} {1} IN ({2})", fieldName, notValue, paramName));
            }
        }

        /// <summary>
        /// StartSwith方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        private void VisitMethodStartswith(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // !=
            var notValue = "1";
            if (SqlList.First().Equals("Not")) { SqlList.Pop(); notValue = "-1"; }

            SqlList.Push(String.Format("CHARINDEX({1},{0}) = {2}", fieldName, notValue, paramName));
        }

        /// <summary>
        /// EndSwith方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        private void VisitMethodEndswith(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // not
            var notValue = "";
            if (SqlList.First().Equals("Not")) { notValue = SqlList.Pop(); }

            SqlList.Push(String.Format("{0} {1} LIKE {2}", fieldName, notValue, paramName));
            CurrentDbParameter.Value = string.Format("%{0}", CurrentDbParameter.Value);
        }

        /// <summary>
        /// IsEquals方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        private void VisitMethodIsEquals(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // !=
            var notValue = "=";
            if (SqlList.First().Equals("Not")) { SqlList.Pop(); notValue = "<>"; }

            SqlList.Push(String.Format("{0} {1} {2}", fieldName, notValue, paramName));
        }
    }
}
