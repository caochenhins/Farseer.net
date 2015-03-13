using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class SelectAssemble<TInfo> : DbVisit<TInfo> where TInfo : class, new()
    {
        public SelectAssemble(IQueryQueue queryQueue, DbProvider dbProvider, IList<DbParameter> lstParam) : base(queryQueue, dbProvider, lstParam) { }

        public string Execute(Expression exp)
        {
            Visit(exp);

            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New: return VisitNew((NewExpression)exp);
                case ExpressionType.MemberAccess: return VisitMemberAccess((MemberExpression)exp);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        private Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;

            var keyValue = Map.GetModelInfo(m.Member.Name);
            if (keyValue.Key == null) { return VisitMemberAccess((MemberExpression)m.Expression); }

            // 加入Sql队列

            string filedName;
            if (!DbProvider.IsField(keyValue.Value.Column.Name)) { filedName = keyValue.Value.Column.Name + " as " + keyValue.Key.Name; }
            else { filedName = DbProvider.KeywordAegis(keyValue.Value.Column.Name); }
            SqlList.Push(filedName);
            return m;
        }

        private void VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            var num = 0;
            var count = original.Count;
            while (num < count) { Visit(original[num]); num++; }
        }

        private NewExpression VisitNew(NewExpression nex)
        {
            VisitExpressionList(nex.Arguments);
            return nex;
        }
        private Expression VisitLambda(LambdaExpression lambda)
        {
            return Visit(lambda.Body);
        }
    }
}