using System;
using System.Collections.Generic;
using FS.Configs;
using FS.Mapping.Table;
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
        private static readonly Dictionary<Type, TableMap> MapList = new Dictionary<Type, TableMap>();
        /// <summary>
        ///     缓存所有验证的实体类
        /// </summary>
        private static readonly Dictionary<Type, VerifyMap> VerifyMapList = new Dictionary<Type, VerifyMap>();
        /// <summary>
        /// 连接字符串缓存
        /// </summary>
        private static readonly Dictionary<int, string> ConnList = new Dictionary<int, string>();

        /// <summary>
        ///     返回实体类映射的信息
        /// </summary>
        /// <param name="type">实体类</param>
        public static TableMap GetTableMap(Type type)
        {
            if (MapList.ContainsKey(type)) return MapList[type];
            lock (LockObject)
            {
                if (!MapList.ContainsKey(type))
                {
                    MapList.Add(type, new TableMap(type));
                }
            }

            return MapList[type];
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

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            MapList.Clear();
            VerifyMapList.Clear();
        }
    }
}
