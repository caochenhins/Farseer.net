using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Visit
{
    public abstract class DbOrderByVisit<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     实体类映射
        /// </summary>
        protected readonly TableMap Map = typeof(TEntity);

        /// <summary>
        ///     条件堆栈
        /// </summary>
        protected readonly Stack<string> SqlList = new Stack<string>();

        /// <summary>
        ///     参数个数（标识）
        /// </summary>
        protected int m_ParamsCount;

        protected readonly IQueryQueue QueryQueue;
        protected readonly IQuery Query;
        protected List<DbParameter> ParamsList;
        public DbOrderByVisit(IQuery query, IQueryQueue queryQueue)
        {
            QueryQueue = queryQueue;
            Query = query;
        }

        public string Execute(Expression exp)
        {
            Visit(exp);

            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess: exp = VisitConvert(exp); break;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return VisitLambda((LambdaExpression)exp);
                case ExpressionType.SubtractChecked: return VisitBinary((BinaryExpression)exp);
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
                case ExpressionType.Subtract: return VisitBinary((BinaryExpression)exp);
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert: return Visit(((UnaryExpression)exp).Operand);
                case ExpressionType.Call: return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Constant: return VisitConstant((ConstantExpression)exp);
                case ExpressionType.MemberAccess: return VisitMemberAccess((MemberExpression)exp);







                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp);


                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);


                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);


                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);


                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);

                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        /// <summary>
        /// 操作符号
        /// </summary>
        /// <param name="nodeType">表达式树类型</param>
        protected string VisitOperate(ExpressionType nodeType)
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
        protected virtual Expression VisitBinary(BinaryExpression bexp)
        {
            if (bexp == null) { return null; }

            Visit(bexp.Left);
            Visit(bexp.Right);

            var right = SqlNot(SqlList.Pop());
            var left = SqlNot(SqlList.Pop());

            if (bexp.NodeType == ExpressionType.AndAlso || bexp.NodeType == ExpressionType.OrElse)
            {
                right = SqlTrue(right);
                left = SqlTrue(left);
            }

            SqlList.Push(String.Format("({0} {1} {2})", left, VisitOperate(bexp.NodeType), right));

            return bexp;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected virtual Expression VisitConstant(ConstantExpression cexp)
        {
            if (cexp == null) return null;
            m_ParamsCount++;
            var paramName = String.Format("{0}Parms_{1}", Query.DbProvider.ParamsPrefix, m_ParamsCount.ToString());
            SqlList.Push(paramName);
            ParamsList.Add(Query.DbProvider.CreateDbParam(paramName.Substring(Query.DbProvider.ParamsPrefix.Length), cexp.Value));

            return cexp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;

            switch (m.NodeType)
            {
                //局部变量
                case ExpressionType.Constant: return Visit(VisitConvert(m));
                default:
                    {
                        var keyValue = Map.GetModelInfo(m.Member.Name);
                        if (keyValue.Key == null)
                        {
                            switch (m.Member.Name)
                            {
                                case "Length":
                                    {
                                        var exp = VisitMemberAccess((MemberExpression)m.Expression);
                                        SqlList.Push(string.Format("LEN({0})", SqlList.Pop()));
                                        return exp;
                                    }
                            }
                            return VisitMemberAccess((MemberExpression)m.Expression);
                        }

                        // 加入Sql队列
                        var filedName = Query.DbProvider.KeywordAegis(keyValue.Value.Column.Name);
                        SqlList.Push(filedName);
                        return m;
                    }
            }
        }

        /// <summary>
        ///     数组值
        /// </summary>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            foreach (var ex in na.Expressions)
            {
                Visit(ex);
            }
            return null;
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not)
            {
                SqlList.Push("Not");
            }
            return Visit((u).Operand);
        }

        /// <summary>
        ///     将变量转换成值
        /// </summary>
        protected virtual Expression VisitConvert(Expression exp)
        {
            if (exp is BinaryExpression) { return exp; }
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = (MemberExpression)exp;
                // o.XXXX 成员
                if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter && memberExp.Expression.Type.IsClass) { return exp; }
            }

            IsCanCompile(exp);

            var lambda = Expression.Lambda(exp);
            return Expression.Constant(lambda.Compile().DynamicInvoke(null), exp.Type);

            //return !IsCanCompile(exp) ? exp : Expression.Constant(lambda.Compile().DynamicInvoke(null), exp.Type);
        }

        /// <summary>
        ///     是否允许执行转换
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCanCompile(Expression exp)
        {
            if (exp == null)
            {
                return false;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return IsCanCompile(((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                    {
                        var callExp = (MethodCallExpression)exp;
                        if (callExp.Object != null && !IsCanCompile(callExp.Object)) { return false; }
                        foreach (var item in callExp.Arguments) { if (!IsCanCompile(item)) { return false; } }
                        return true;
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memExp = (MemberExpression)exp;
                        return memExp.Expression == null || IsCanCompile(memExp.Expression);
                        //if (memExp.Expression.NodeType == ExpressionType.Constant) { return true; }
                        //if (memExp.Expression.NodeType == ExpressionType.MemberAccess) { return IsCanCompile(memExp.Expression); }
                        //break;
                    }
                case ExpressionType.Parameter: return !exp.Type.IsClass;
                case ExpressionType.Convert: return IsCanCompile(((UnaryExpression)exp).Operand);
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
                Visit(m.Object);
                for (var i = 0; i < m.Arguments.Count; i++)
                {
                    var exp = m.Arguments[i];
                    //while (exp != null && exp.NodeType == ExpressionType.Call)
                    //{
                    //    exp = ((MethodCallExpression)exp).Object;
                    //}
                    Visit(exp);
                }
            }
            return m;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
            }
            throw new Exception(string.Format("类型：(MemberBindingType){0}，不存在。", binding.BindingType));
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
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

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = Visit(c.Test);
            var ifTrue = Visit(c.IfTrue);
            var ifFalse = Visit(c.IfFalse);
            if (((test == c.Test) && (ifTrue == c.IfTrue)) && (ifFalse == c.IfFalse))
            {
                return c;
            }
            return Expression.Condition(test, ifTrue, ifFalse);
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

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> arguments = VisitExpressionList(iv.Arguments);
            var expression = Visit(iv.Expression);
            if ((arguments == iv.Arguments) && (expression == iv.Expression))
            {
                return iv;
            }
            return Expression.Invoke(expression, arguments);
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = Visit(lambda.Body);










            if (body != lambda.Body)
            {
                try
                {
                    return Expression.Lambda(lambda.Type, Expression.Convert(body, typeof(object)), lambda.Parameters);
                }
                catch
                {
                    return lambda.Body;
                }
            }
            return lambda;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            var newExpression = VisitNew(init.NewExpression);
            var initializers = VisitElementInitializerList(init.Initializers);
            if ((newExpression == init.NewExpression) && (initializers == init.Initializers))
            {
                return init;
            }
            return Expression.ListInit(newExpression, initializers);
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var expression = Visit(assignment.Expression);
            return expression != assignment.Expression ? Expression.Bind(assignment.Member, expression) : assignment;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var newExpression = VisitNew(init.NewExpression);
            var bindings = VisitBindingList(init.Bindings);
            if ((newExpression == init.NewExpression) && (bindings == init.Bindings))
            {
                return init;
            }
            return Expression.MemberInit(newExpression, bindings);
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

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> arguments = VisitExpressionList(nex.Arguments);
            if (arguments == nex.Arguments)
            {
                return nex;
            }
            return nex.Members != null ? Expression.New(nex.Constructor, arguments, nex.Members) : Expression.New(nex.Constructor, arguments);
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expression = Visit(b.Expression);
            return expression != b.Expression ? Expression.TypeIs(expression, b.TypeOperand) : b;
        }

        /// <summary>
        ///     清除值为空的条件，并给与1!=1的SQL
        /// </summary>
        protected virtual bool ClearCallSql()
        {
            if (ParamsList != null && ParamsList.Count > 0 && string.IsNullOrWhiteSpace(ParamsList.Last().Value.ToString()))
            {
                ParamsList.RemoveAt(ParamsList.Count - 1);
                SqlList.Pop();
                SqlList.Pop();
                SqlList.Push("1<>1");
                return true;
            }
            return false;
        }

        /// <summary>
        ///     当存在Not 时，特殊处理
        /// </summary>
        protected virtual string SqlNot(string sql)
        {
            var lst = new List<string> { sql };
            // 当存在Not 时，特殊处理
            while (SqlList.Count > 0 && SqlList.First().Equals("Not")) { lst.Add(SqlList.Pop()); }
            lst.Reverse();

            var sb = new StringBuilder(sql);
            lst.ForEach(o => sb.Append(o + " "));

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        /// <summary>
        ///     当存在true 时，特殊处理
        /// </summary>
        protected virtual string SqlTrue(string sql)
        {
            var dbParam = ParamsList.FirstOrDefault(o => o.ParameterName == sql);
            if (dbParam != null)
            {
                var result = dbParam.Value.ToString().Equals("true");
                ParamsList.RemoveAll(o => o.ParameterName == sql);
                return result ? "1=1" : "1<>1";
            }
            return sql;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct Buffer<TElement>
        {
            internal TElement[] items;
            internal int count;

            internal Buffer(IEnumerable<TElement> source)
            {
                TElement[] array = null;
                var length = 0;
                var is2 = source as ICollection<TElement>;
                if (is2 != null)
                {
                    length = is2.Count;
                    if (length > 0)
                    {
                        array = new TElement[length];
                        is2.CopyTo(array, 0);
                    }
                }
                else
                {
                    foreach (var local in source)
                    {
                        if (array == null)
                        {
                            array = new TElement[4];
                        }
                        else if (array.Length == length)
                        {
                            var destinationArray = new TElement[length * 2];
                            Array.Copy(array, 0, destinationArray, 0, length);
                            array = destinationArray;
                        }
                        array[length] = local;
                        length++;
                    }
                }
                items = array;
                count = length;
            }

            internal TElement[] ToArray()
            {
                if (count == 0)
                {
                    return new TElement[0];
                }
                if (items.Length == count)
                {
                    return items;
                }
                var destinationArray = new TElement[count];
                Array.Copy(items, 0, destinationArray, 0, count);
                return destinationArray;
            }
        }
    }
}
