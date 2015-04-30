using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace FS.Configs
{
    /// <summary>
    ///     配置管理工具
    /// </summary>
    public class BaseConfigs<T> where T : class, new()
    {
        /// <summary>
        ///     锁对象
        /// </summary>
        private static object m_LockHelper = new object();

        /// <summary>
        ///     配置文件路径
        /// </summary>
        private static string filePath;

        /// <summary>
        ///     配置文件名称
        /// </summary>
        private static string fileName;

        /// <summary>
        ///     配置变量
        /// </summary>
        protected static T m_ConfigEntity;

        /// <summary>
        ///     Config修改时间
        /// </summary>
        private static DateTime FileLastWriteTime;

        /// <summary>
        ///     加载配置文件的时间（60分钟重新加载）
        /// </summary>
        private static DateTime LoadTime;

        /// <summary>
        ///     获取配置文件所在路径，支持自定义路径
        /// </summary>
        private static string FilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = (AppDomain.CurrentDomain.BaseDirectory + "/App_Data/").Replace("/", "\\");
                }
                return filePath;
            }
            set
            {
                filePath = value.Replace("/", "\\");
                if (filePath.EndsWith("/"))
                {
                    filePath += "/";
                }
            }
        }

        /// <summary>
        ///     获取配置文件所在路径
        /// </summary>
        private static string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = string.Format("{0}.config",
                                             typeof(T).Name.EndsWith("Config", true, null)
                                                 ? typeof(T).Name.Substring(0, typeof(T).Name.Length - 6)
                                                 : typeof(T).Name);
                }
                return fileName;
            }
        }

        /// <summary>
        ///     配置变量
        /// </summary>
        public static T ConfigEntity
        {
            get
            {
                if (m_ConfigEntity == null || ((DateTime.Now - LoadTime).TotalMinutes > 60 && FileLastWriteTime != File.GetLastWriteTime(FilePath + FileName)))
                {
                    LoadConfig();
                }
                return m_ConfigEntity;
            }
        }

        /// <summary>
        ///     加载(反序列化)指定对象类型的配置对象
        /// </summary>
        public static void LoadConfig()
        {
            //不存在则自动接创建
            if (!File.Exists(FilePath + FileName))
            {
                var t = new T();
                foreach (var fieldEntity in t.GetType().GetFields()) { DynamicAddItem(fieldEntity, t); }
                foreach (var property in t.GetType().GetProperties()) { DynamicAddItem(property, t); }
                SaveConfig(t);
            }
            FileLastWriteTime = File.GetLastWriteTime(FilePath + FileName);

            lock (m_LockHelper)
            {
                m_ConfigEntity = Deserialize(FilePath + FileName);
                LoadTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 动态添加List元素
        /// </summary>
        /// <param name="fieldInfo">字段类型</param>
        /// <param name="entity">所属实体变量</param>
        /// <param name="methodName">方法名称，默认是Add</param>
        private static void DynamicAddItem(FieldInfo fieldInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!fieldInfo.FieldType.IsGenericType) { return; }
            var lstVal = fieldInfo.GetValue(entity);
            // 空时，反射创建
            if (lstVal == null) { lstVal = Activator.CreateInstance(fieldInfo.FieldType); fieldInfo.SetValue(entity, lstVal); }

            // 获取执行方法
            var method = fieldInfo.FieldType.GetMethod(methodName);
            if (method == null) { return; }

            // 反射创建子元素
            var item = Activator.CreateInstance(fieldInfo.FieldType.GetGenericArguments()[0]);
            method.Invoke(lstVal, new[] { item });

            // 添加子元素到List中
            foreach (var field in lstVal.GetType().GetFields()) { DynamicAddItem(field, lstVal); }
            foreach (var property in lstVal.GetType().GetProperties()) { DynamicAddItem(property, lstVal); }
        }

        /// <summary>
        /// 动态添加List元素
        /// </summary>
        /// <param name="propertyInfo">字段类型</param>
        /// <param name="entity">所属实体变量</param>
        /// <param name="methodName">方法名称，默认是Add</param>
        private static void DynamicAddItem(PropertyInfo propertyInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!propertyInfo.PropertyType.IsGenericType) { return; }
            var lstVal = propertyInfo.GetValue(entity, null);
            // 空时，反射创建
            if (lstVal == null) { lstVal = Activator.CreateInstance(propertyInfo.PropertyType); propertyInfo.SetValue(entity, lstVal, null); }

            // 获取执行方法
            var method = propertyInfo.PropertyType.GetMethod(methodName);
            if (method == null) { return; }

            // 反射创建子元素
            var item = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments()[0]);

            // 添加子元素到List中
            method.Invoke(lstVal, new[] { item });
            foreach (var field in lstVal.GetType().GetFields()) { DynamicAddItem(field, lstVal); }
            foreach (var property in lstVal.GetType().GetProperties()) { DynamicAddItem(property, lstVal); }
        }

        /// <summary>
        ///     保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="t">Config配置</param>
        public static bool SaveConfig(T t = null)
        {
            if (t == null) { t = ConfigEntity; }
            var result = Serialize(t, FilePath + FileName);
            return result;
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static T Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="filePath">文件路径</param>
        public static bool Serialize(T t, string filePath)
        {
            var succeed = false;
            FileStream fs = null;
            try
            {
                Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var serializer = new XmlSerializer(t.GetType());
                serializer.Serialize(fs, t);
                succeed = true;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return succeed;
        }
    }
}