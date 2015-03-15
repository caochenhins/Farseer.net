using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Visit
{
    public abstract class DbSelectVisit<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     实体类映射
        /// </summary>
        protected readonly TableMap Map = typeof(TEntity);

        /// <summary>
        ///     条件堆栈
        /// </summary>
        protected readonly Stack<string> SqlList = new Stack<string>();

        protected readonly IQueryQueue QueryQueue;
        protected readonly IQuery Query;

        public DbSelectVisit(IQuery query, IQueryQueue queryQueue)
        {
            QueryQueue = queryQueue;
            Query = query;
        }

        public string Execute(List<Expression> lstExp)
        {
            if (lstExp == null || lstExp.Count == 0) { return null; }
            lstExp.ForEach(exp => Visit(exp));

            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New: return VisitNew((NewExpression)exp);
                case ExpressionType.MemberAccess: return CreateFieldName((MemberExpression)exp);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        protected virtual Expression CreateFieldName(MemberExpression m)
        {
            if (m == null) return null;

            var keyValue = Map.GetModelInfo(m.Member.Name);
            if (keyValue.Key == null) { return CreateFieldName((MemberExpression)m.Expression); }

            // 加入Sql队列
            string filedName;
            if (!Query.DbProvider.IsField(keyValue.Value.Column.Name)) { filedName = keyValue.Value.Column.Name + " as " + keyValue.Key.Name; }
            else { filedName = Query.DbProvider.KeywordAegis(keyValue.Value.Column.Name); }
            SqlList.Push(filedName);
            return m;
        }

        protected virtual void VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            var num = 0;
            var count = original.Count;
            while (num < count) { Visit(original[num]); num++; }
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            VisitExpressionList(nex.Arguments);
            return nex;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            return Visit(lambda.Body);
        }
    }
}