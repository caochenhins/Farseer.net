using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using FS.Mapping.Context;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 提供ExpressionNew表达式树的解析
    /// </summary>
    /// <typeparam name="TEntity">实体类</typeparam>
    public abstract class DbExpressionNewProvider<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 实体类映射
        /// </summary>
        protected readonly FieldMap Map = typeof(TEntity);
        /// <summary>
        /// 条件堆栈
        /// </summary>
        public readonly Stack<string> SqlList = new Stack<string>();
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly IQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly IQueueSql QueueSql;
        /// <summary>
        /// 是否是字段筛选
        /// </summary>
        protected bool IsSelect;

        /// <summary>
        /// 提供ExpressionNew表达式树的解析
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public DbExpressionNewProvider(IQueueManger queueManger, IQueueSql queueSql)
        {
            QueueManger = queueManger;
            QueueSql = queueSql;
        }

        /// <summary>
        /// 清除当前所有数据
        /// </summary>
        public void Clear()
        {
            SqlList.Clear();
        }
        public virtual Expression Visit(Expression exp, bool? isSelect = null)
        {
            if (exp == null) { return null; }
            if (isSelect != null) { IsSelect = isSelect.GetValueOrDefault(); }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New: return VisitNew((NewExpression)exp);
                case ExpressionType.MemberAccess: return CreateFieldName((MemberExpression)exp);
                case ExpressionType.Convert: return Visit(((UnaryExpression)exp).Operand);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        protected virtual Expression CreateFieldName(MemberExpression m)
        {
            if (m == null) return null;

            var keyValue = Map.GetState(m.Member.Name);
            if (keyValue.Key == null) { return CreateFieldName((MemberExpression)m.Expression); }

            // 加入Sql队列
            string filedName;
            if (!QueueManger.DbProvider.IsField(keyValue.Value.FieldAtt.Name))
            {
                filedName = IsSelect ? keyValue.Value.FieldAtt.Name + " as " + keyValue.Key.Name : keyValue.Value.FieldAtt.Name;
            }
            else { filedName = QueueManger.DbProvider.KeywordAegis(keyValue.Value.FieldAtt.Name); }
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