using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 提供ExpressionBinary表达式树的解析
    /// </summary>
    public abstract class DbExpressionBoolProvider
    {
        /// <summary>
        ///  条件堆栈
        /// </summary>
        public readonly Stack<string> SqlList = new Stack<string>();
        /// <summary>
        ///  参数个数（标识）
        /// </summary>
        private int _paramsCount;
        /// <summary>
        /// 当前字段名称
        /// </summary>
        private string _currentFieldName;
        /// <summary>
        /// 当前值参数
        /// </summary>
        protected DbParameter CurrentDbParameter;
        /// <summary>
        /// 队列管理模块
        /// </summary>
        protected readonly IQueueManger QueueManger;
        /// <summary>
        /// 包含数据库SQL操作的队列
        /// </summary>
        protected readonly IQueueSql QueueSql;

        /// <summary>
        /// 是否包括Not条件
        /// </summary>
        protected bool IsNot;

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queueSql">包含数据库SQL操作的队列</param>
        public DbExpressionBoolProvider(IQueueManger queueManger, IQueueSql queueSql)
        {
            QueueManger = queueManger;
            QueueSql = queueSql;
            if (QueueSql.Param == null) { QueueSql.Param = new List<DbParameter>(); }
        }

        /// <summary>
        /// 清除当前所有数据
        /// </summary>
        public void Clear()
        {
            CurrentDbParameter = null;
            _currentFieldName = null;
            _paramsCount = 0;
            SqlList.Clear();
        }

        public Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess: exp = VisitConvertExp(exp); break;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return VisitLambda((LambdaExpression)exp);
                case ExpressionType.SubtractChecked: return CreateBinary((BinaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract: return CreateBinary((BinaryExpression)exp);
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert: return Visit(((UnaryExpression)exp).Operand);
                case ExpressionType.Call: return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Constant: return CreateDbParam((ConstantExpression)exp);
                case ExpressionType.MemberAccess: return CreateFieldName((MemberExpression)exp);
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs: return VisitUnary((UnaryExpression)exp);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        /// <summary>
        /// 操作符号
        /// </summary>
        /// <param name="nodeType">表达式树类型</param>
        protected string CreateOperate(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal: return "=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.AndAlso: return "AND";
                case ExpressionType.OrElse: return "OR";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.And: return "&";
                case ExpressionType.Or: return "|";
            }
            throw new NotSupportedException(nodeType + "的类型，未定义操作符号！");
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected virtual Expression CreateBinary(BinaryExpression bexp)
        {
            if (bexp == null) { return null; }

            // 先解析字段
            if (bexp.Left.NodeType == ExpressionType.MemberAccess || (bexp.Left.NodeType != ExpressionType.MemberAccess && bexp.Right.NodeType != ExpressionType.MemberAccess))
            {
                Visit(bexp.Left);
                Visit(bexp.Right);
            }
            else
            {
                Visit(bexp.Right);
                Visit(bexp.Left);
            }

            var right = SqlList.Pop();
            var left = SqlList.Pop();

            if (bexp.NodeType == ExpressionType.AndAlso || bexp.NodeType == ExpressionType.OrElse)
            {
                right = SqlTrue(right);
                left = SqlTrue(left);
            }

            SqlList.Push(String.Format("({0} {1} {2})", left, CreateOperate(bexp.NodeType), right));

            return bexp;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected virtual Expression CreateDbParam(ConstantExpression cexp)
        {
            if (cexp == null) return null;
            if (string.IsNullOrWhiteSpace(_currentFieldName)) { throw new Exception("当前字段名称为空，无法生成字段参数"); }

            // 非字符串，不使用参数
            //if (!(cexp.Value is string))
            //{
            //    int len;
            //    var type = DbProvider.GetDbType(cexp.Value, out len);
            //    SqlList.Push(DbProvider.ParamConvertValue(cexp.Value, type).ToString());

            //}
            //else
            {
                _paramsCount++;
                //  查找组中是否存在已有的参数，有则直接取出
                var newParam = QueueManger.DbProvider.CreateDbParam(QueueSql.Index + "_" + _paramsCount + "_" + _currentFieldName, cexp.Value, QueueManger.Param, QueueSql.Param);
                CurrentDbParameter = newParam;
                SqlList.Push(newParam.ParameterName);
            }
            _currentFieldName = string.Empty;
            return cexp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected virtual Expression CreateFieldName(MemberExpression m)
        {
            if (m == null) return null;
            if (m.NodeType == ExpressionType.Constant) { return Visit(VisitConvertExp(m)); }

            var keyValue = QueueSql.FieldMap.GetState(m.Member.Name);
            // 解析带SQL函数的字段
            if (keyValue.Key == null) { return CreateFunctionFieldName(m); }

            // 加入Sql队列
            _currentFieldName = keyValue.Value.FieldAtt.Name;
            var filedName = QueueManger.DbProvider.KeywordAegis(_currentFieldName);
            SqlList.Push(filedName);
            return m;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名（带SQL函数的字段）
        /// </summary>
        protected virtual Expression CreateFunctionFieldName(MemberExpression m)
        {
            switch (m.Member.Name)
            {
                case "Count":
                case "Length":
                    {
                        var exp = CreateFieldName((MemberExpression)m.Expression);
                        SqlList.Push(string.Format("LEN({0})", SqlList.Pop()));
                        return exp;
                    }
            }
            return CreateFieldName((MemberExpression)m.Expression);
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not) { IsNot = true; }
            return Visit((u).Operand);
        }

        /// <summary>
        ///     将变量转换成值
        /// </summary>
        protected Expression VisitConvertExp(Expression exp)
        {
            if (exp is BinaryExpression || !IsFieldValue(exp)) { return exp; }
            return Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke(null), exp.Type);
        }

        /// <summary>
        ///     判断是字段，还是值类型
        /// </summary>
        protected bool IsFieldValue(Expression exp)
        {
            if (exp == null) { return false; }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return IsFieldValue(((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                    {
                        var callExp = (MethodCallExpression)exp;
                        // oXXXX.Call(....)
                        if (callExp.Object != null && !IsFieldValue(callExp.Object)) { return false; }
                        foreach (var item in callExp.Arguments) { if (!IsFieldValue(item)) { return false; } }
                        return true;
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memExp = (MemberExpression)exp;
                        // o.XXXX
                        return memExp.Expression == null || IsFieldValue(memExp.Expression);
                        //if (memExp.Expression.NodeType == ExpressionType.Constant) { return true; }
                        //if (memExp.Expression.NodeType == ExpressionType.MemberAccess) { return IsCanCompile(memExp.Expression); }
                        //break;
                    }
                case ExpressionType.Parameter: return !exp.Type.IsClass && !exp.Type.IsAbstract && !exp.Type.IsInterface;
                case ExpressionType.Convert: return IsFieldValue(((UnaryExpression)exp).Operand);
                case ExpressionType.ArrayIndex:
                case ExpressionType.ListInit:
                case ExpressionType.Constant: { return true; }
            }
            return false;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object == null)
            {
                for (var i = m.Arguments.Count - 1; i > 0; i--)
                {
                    var exp = m.Arguments[i];
                    //while (exp != null && exp.NodeType == ExpressionType.Call)
                    //{
                    //    exp = ((MethodCallExpression)exp).Object;
                    //}
                    Visit(exp);
                }
                Visit(m.Arguments[0]);
            }
            else
            {
                // 如果m.Object能压缩，证明不是字段（必须先解析字段，再解析值）
                var result = IsFieldValue(m.Object);

                if (!result) { Visit(m.Object); }
                for (var i = 0; i < m.Arguments.Count; i++) { Visit(m.Arguments[i]); }
                if (result) { Visit(m.Object); }
            }
            return m;
        }

        protected MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment: return VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding: return VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding: return VisitMemberListBinding((MemberListBinding)binding);
            }
            throw new Exception(string.Format("类型：(MemberBindingType){0}，不存在。", binding.BindingType));
        }

        protected IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = VisitBinding(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<MemberBinding>(count);
                    for (var i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = VisitElementInitializer(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<ElementInit>(count);
                    for (var i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual IEnumerable<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> sequence = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = Visit(original[num]);
                if (sequence != null)
                {
                    sequence.Add(item);
                }
                else if (item != original[num])
                {
                    sequence = new List<Expression>(count);
                    for (var i = 0; i < num; i++)
                    {
                        sequence.Add(original[i]);
                    }
                    sequence.Add(item);
                }
                num++;
            }
            if (sequence != null)
            {
                return (ReadOnlyCollection<Expression>)(IEnumerable)sequence;
            }
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            return lambda;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var expression = Visit(assignment.Expression);
            return expression != assignment.Expression ? Expression.Bind(assignment.Member, expression) : assignment;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        /// <summary>
        ///     清除值为空的条件，并给与1!=1的SQL
        /// </summary>
        protected virtual bool ClearCallSql()
        {
            if (QueueSql.Param != null && QueueSql.Param.Count > 0 && string.IsNullOrWhiteSpace(QueueSql.Param.Last().Value.ToString()))
            {
                QueueSql.Param.RemoveAt(QueueSql.Param.Count - 1);
                SqlList.Pop();
                SqlList.Pop();
                SqlList.Push("1<>1");
                return true;
            }
            return false;
        }

        /// <summary>
        ///     当存在true 时，特殊处理
        /// </summary>
        protected virtual string SqlTrue(string sql)
        {
            var dbParam = QueueSql.Param.FirstOrDefault(o => o.ParameterName == sql);
            if (dbParam != null)
            {
                var result = dbParam.Value.ToString().Equals("true");
                QueueSql.Param.RemoveAll(o => o.ParameterName == sql);
                return result ? "1=1" : "1<>1";
            }
            return sql;
        }
    }
}
