﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FS.Core.Client.MySql;
using FS.Core.Client.OleDb;
using FS.Core.Client.Oracle;
using FS.Core.Client.SqLite;
using FS.Core.Client.SqlServer;
using FS.Core.Data;
using FS.Core.Data.Table;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库提供者（不同数据库的特性）
    /// </summary>
    public abstract class DbProvider
    {
        /// <summary>
        ///     支持一次传输最多的参数个数
        /// </summary>
        public virtual int ParamsMaxLength { get { return 2100; } }

        /// <summary>
        ///     参数前缀
        /// </summary>
        public virtual string ParamsPrefix { get { return "@"; } }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract DbProviderFactory GetDbProviderFactory { get; }

        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName">字符名称</param>
        public virtual string KeywordAegis(string fieldName) { return String.Format("[{0}]", fieldName); }

        /// <summary>
        /// 判断是否为字段。还是组合字段。
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public static bool IsField(string fieldName)
        {
            return new Regex("^[a-z0-9_-]+$", RegexOptions.IgnoreCase).IsMatch(fieldName.Replace("(", "\\(").Replace(")", "\\)"));
        }

        #region 创建参数
        /// <summary>
        /// 将C#值转成数据库能存储的值
        /// </summary>
        /// <param name="valu"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ParamConvertValue(object valu, DbType type)
        {
            if (valu == null) { return null; }

            // 时间类型转换
            if (type == DbType.DateTime)
            {
                DateTime dtValue; DateTime.TryParse(valu.ToString(), out dtValue);
                if (dtValue == DateTime.MinValue) { valu = new DateTime(1900, 1, 1); }
            }
            // 枚举类型转换
            if (valu is Enum) { valu = Convert.ToInt32(valu); }

            // List类型转换成字符串并以,分隔
            if (valu.GetType().IsGenericType)
            {
                var sb = new StringBuilder();
                // list类型
                if (valu.GetType().GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    var enumerator = ((IEnumerable)valu).GetEnumerator();
                    while (enumerator.MoveNext()) { sb.Append(enumerator.Current + ","); }
                }
                else
                {
                    if (valu.GetType().GetGenericArguments()[0] == typeof(int))
                    {
                        var enumerator = ((IEnumerable<int?>)valu).GetEnumerator();
                        while (enumerator.MoveNext()) { sb.Append(enumerator.Current.GetValueOrDefault() + ","); }
                    }
                }
                if (sb.Length > 0) { valu = sb.Remove(sb.Length - 1, 1).ToString(); }
            }
            return valu;
        }

        /// <summary>
        /// 根据值，返回类型
        /// </summary>
        /// <param name="valu">参数值</param>
        /// <param name="len">参数长度</param>
        /// <returns></returns>
        public DbType GetDbType(object valu, out int len)
        {
            if (valu == null) { valu = String.Empty; }

            var type = valu.GetType();
            if (type.Name.Equals("Nullable`1")) { type = Nullable.GetUnderlyingType(type); }
            if (type.BaseType != null && type.BaseType.Name == "Enum") { len = 1; return DbType.Byte; }
            switch (type.Name)
            {
                case "DateTime": len = 8; return DbType.DateTime;
                case "Boolean": len = 1; return DbType.Boolean;
                case "Int32": len = 4; return DbType.Int32;
                case "Int16": len = 2; return DbType.Int16;
                case "Decimal": len = 8; return DbType.Decimal;
                case "Byte": len = 1; return DbType.Byte;
                case "Long":
                case "Float":
                case "Double": len = 8; return DbType.Decimal;
                default: len = valu.ToString().Length; return DbType.String;
            }
        }

        /// <summary>
        /// 根据值，返回类型
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        /// <returns></returns>
        public DbType GetDbType(Type type, out int len)
        {
            if (type.Name.Equals("Nullable`1")) { type = Nullable.GetUnderlyingType(type); }
            if (type.BaseType != null && type.BaseType.Name == "Enum") { len = 1; return DbType.Byte; }
            switch (type.Name)
            {
                case "DateTime": len = 8; return DbType.DateTime;
                case "Boolean": len = 1; return DbType.Boolean;
                case "Int32": len = 4; return DbType.Int32;
                case "Int16": len = 2; return DbType.Int16;
                case "Decimal": len = 8; return DbType.Decimal;
                case "Byte": len = 1; return DbType.Byte;
                case "Long":
                case "Float":
                case "Double": len = 8; return DbType.Decimal;
                default: len = 0; return DbType.String;
            }
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, DbType type, int len, bool output = false)
        {
            var param = GetDbProviderFactory.CreateParameter();
            param.DbType = type;
            param.ParameterName = ParamsPrefix + name;
            param.Value = ParamConvertValue(valu, type);
            if (len > 0) param.Size = len;
            if (output) param.Direction = ParameterDirection.Output;
            return param;
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, bool output = false)
        {
            if (valu == null) { valu = String.Empty; }

            int len;
            var type = GetDbType(valu, out len);
            return CreateDbParam(name, valu, type, len, output);

        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, Type valueType, bool output = false)
        {
            int len;
            var type = GetDbType(valueType, out len);
            return CreateDbParam(name, valu, type, len, output);

        }

        /// <summary>
        ///     获取该实体类的参数
        /// </summary>
        /// <param name="entity">实体类</param>
        public IList<DbParameter> GetParameter<TEntity>(TEntity entity) where TEntity : class,new()
        {
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            var lst = new List<DbParameter>();

            foreach (var kic in map.MapList.Where(o => o.Value.FieldAtt.IsMap))
            {
                var obj = kic.Key.GetValue(entity, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                //  添加参数到列表
                lst.Add(CreateDbParam(kic.Value.FieldAtt.Name, obj));
            }

            return lst;
        }

        /// <summary>
        /// 从已有的参数列表中取出参数，如果不存在则创建。
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="lstQueueMangerParam">已加入的参数</param>
        /// <param name="lstQueueParam">当前加入的参数</param>
        public DbParameter CreateDbParam(string name, object valu, List<DbParameter> lstQueueMangerParam, List<DbParameter> lstQueueParam)
        {
            int len;
            var type = GetDbType(valu, out len);
            var newValu = ParamConvertValue(valu, type);

            //  查找组中是否存在已有的参数，没有则创建新的
            var newParam = (lstQueueMangerParam == null ? null : lstQueueMangerParam.Find(o => o.DbType == type && o.Value.GetType() == newValu.GetType() && o.Value.ToString() == newValu.ToString()));// ?? lstNewParam.ToList().Find(o => o.Value == valu && o.DbType == type);
            if (newParam == null)
            {
                newParam = CreateDbParam(name, valu, type, len);
                newParam.ParameterName = ParamsPrefix + name;
            }
            lstQueueParam.Add(newParam);
            return newParam;
        }
        #endregion

        /// <summary>
        /// 创建SQL查询
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public abstract IBuilderSqlQuery CreateBuilderSqlQuery(IQueueManger queueManger, IQueue queue);

        /// <summary>
        /// 创建SQL执行
        /// </summary>
        /// <param name="queueManger">队列管理模块</param>
        /// <param name="queue">包含数据库SQL操作的队列</param>
        public abstract IBuilderSqlOper CreateBuilderSqlOper(IQueueManger queueManger, IQueue queue);

        /// <summary>
        /// 返回数据库类型名称
        /// </summary>
        public static DbProvider CreateInstance(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.OleDb: return new OleDbProvider();
                case DataBaseType.MySql: return new MySqlProvider();
                case DataBaseType.SQLite: return new SqLiteProvider();
                case DataBaseType.Oracle: return new OracleProvider();
            }
            return new SqlServerProvider();
        }

        /// <summary>
        /// 返回数据库类型名称
        /// </summary>
        public static DbProvider CreateInstance<TEntity>() where TEntity : BaseContext, new()
        {
            var dbType = CacheManger.GetContextMap(typeof(TEntity)).ContextProperty.DataType;
            switch (dbType)
            {
                case DataBaseType.OleDb: return new OleDbProvider();
                case DataBaseType.MySql: return new MySqlProvider();
                case DataBaseType.SQLite: return new SqLiteProvider();
                case DataBaseType.Oracle: return new OracleProvider();
            }
            return new SqlServerProvider();
        }
    }
}