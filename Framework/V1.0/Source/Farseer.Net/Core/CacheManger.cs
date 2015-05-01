using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Configs;
using FS.Mapping.Context;
using FS.Mapping.Verify;

namespace FS.Core
{
    /// <summary>
    /// 框架缓存管理
    /// </summary>
    public abstract class CacheManger
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object LockObject = new object();
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, ContextMap> ContextMapList = new Dictionary<Type, ContextMap>();
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, FieldMap> FieldMapList = new Dictionary<Type, FieldMap>();
        /// <summary>
        ///     缓存所有验证的实体类
        /// </summary>
        private static readonly Dictionary<Type, VerifyMap> VerifyMapList = new Dictionary<Type, VerifyMap>();
        /// <summary>
        /// 连接字符串缓存
        /// </summary>
        private static readonly Dictionary<int, string> ConnList = new Dictionary<int, string>();

        /// <summary>
        ///     返回Context映射的信息
        /// </summary>
        /// <param name="type">实体类</param>
        public static ContextMap GetContextMap(Type type)
        {
            if (ContextMapList.ContainsKey(type)) return ContextMapList[type];
            lock (LockObject)
            {
                if (!ContextMapList.ContainsKey(type))
                {
                    ContextMapList.Add(type, new ContextMap(type));
                }
            }

            return ContextMapList[type];
        }
        /// <summary>
        ///     返回Field映射的信息
        /// </summary>
        /// <param name="type">实体类</param>
        public static FieldMap GetFieldMap(Type type)
        {
            if (FieldMapList.ContainsKey(type)) return FieldMapList[type];
            lock (LockObject)
            {
                if (!FieldMapList.ContainsKey(type))
                {
                    FieldMapList.Add(type, new FieldMap(type));
                }
            }

            return FieldMapList[type];
        }

        /// <summary>
        ///     返回验证的实体类映射的信息
        /// </summary>
        /// <param name="type">IVerification实体类</param>
        public static VerifyMap GetVerifyMap(Type type)
        {
            if (VerifyMapList.ContainsKey(type)) return VerifyMapList[type];
            lock (LockObject)
            {
                if (!VerifyMapList.ContainsKey(type))
                {
                    VerifyMapList.Add(type, new VerifyMap(type));
                }
            }

            return VerifyMapList[type];
        }
        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static string CreateConnString(int dbIndex = 0)
        {
            if (ConnList.ContainsKey(dbIndex)) return ConnList[dbIndex];
            lock (LockObject)
            {
                if (ConnList.ContainsKey(dbIndex)) return ConnList[dbIndex];

                DbInfo dbInfo = dbIndex;
                ConnList.Add(dbIndex, DbFactory.CreateConnString(dbInfo.DataType, dbInfo.UserID, dbInfo.PassWord, dbInfo.Server, dbInfo.Catalog,
                    dbInfo.DataVer, dbInfo.ConnectTimeout, dbInfo.PoolMinSize, dbInfo.PoolMaxSize,
                    dbInfo.Port));
            }

            return ConnList[dbIndex];


        }

        private static InstanceCache InstanceCacheEx { get; set; }

        /// <summary>
        /// 获取缓存的反射结构
        /// </summary>
        /// <param name="key">对象类型</param>
        /// <param name="param">构造函数参数</param>
        /// <returns></returns>
        public static object GetInstance(Type key, params object[] param)
        {
            if (InstanceCacheEx == null) { InstanceCacheEx = new InstanceCache(); }
            return InstanceCacheEx.Cache(key, param);
        }

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            ContextMapList.Clear();
            FieldMapList.Clear();
            VerifyMapList.Clear();
            ConnList.Clear();
        }
    }

    /// <summary>
    /// 缓存反射结构
    /// </summary>
    class InstanceCache
    {
        private readonly Dictionary<Type, Func<object>> _dicEx = new Dictionary<Type, Func<object>>();
        public object Cache(Type key, params object[] param)
        {
            Func<object> value = null;

            if (_dicEx.TryGetValue(key, out value))
            {
                return value();
            }
            else
            {
                value = CreateInstance(key, param);
                _dicEx[key] = value;
                return value();
            }
        }

        private static Func<object> CreateInstance(Type type, params object[] param)
        {
            var newExp = Expression.New(type);
            var lambdaExp = Expression.Lambda<Func<object>>(newExp, null);
            var func = lambdaExp.Compile();
            return func;
        }
    }
}
