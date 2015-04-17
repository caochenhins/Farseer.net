using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using FS.Configs;
using FS.Core;
using FS.Core.Data;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     ORM 映射关系
    /// </summary>
    public class TableMap
    {
        /// <summary>
        ///     获取所有属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldMapState> ModelList;

        /// <summary>
        /// 类关系映射
        /// </summary>
        /// <param name="dbIndex"></param>
        public void Map(int dbIndex)
        {
            ClassInfo = new DBAttribute(null, dbIndex);
        }

        /// <summary>
        /// 类关系映射
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <param name="dataVer"></param>
        /// <param name="commandTimeout"></param>
        public void Map(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, string dataVer = "2008", int commandTimeout = 30)
        {
            ClassInfo = new DBAttribute(null, connectionString, dbType, dataVer, commandTimeout);
        }

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public TableMap(Type type)
        {
            Type = type;
            ModelList = new Dictionary<PropertyInfo, FieldMapState>();

            #region 类属性
            var attrs = Type.GetCustomAttributes(typeof(DBAttribute), false);
            ClassInfo = ((DBAttribute)attrs[0]);
            if (string.IsNullOrEmpty(ClassInfo.Name)) { ClassInfo.Name = Type.Name; }
            #endregion

            #region 变量属性

            //遍历所有属性变量,取得对应使用标记名称
            //无加标记时，则为不使用该变量。
            foreach (var propertyInfo in Type.GetProperties())
            {
                var fieldMapState = new FieldMapState();

                // 是否带属性
                attrs = propertyInfo.GetCustomAttributes(false);
                foreach (var item in attrs)
                {
                    // 加入属性
                    fieldMapState.IsDbField = !(item is NotJoinAttribute);
                    // 数据类型
                    if (item is DataTypeAttribute) { fieldMapState.DataType = (DataTypeAttribute)item; continue; }
                    // 字段映射
                    if (item is ColumnAttribute) { fieldMapState.Column = (ColumnAttribute)item; continue; }
                    // 属性扩展
                    if (item is PropertyExtendAttribute) { fieldMapState.PropertyExtend = ((PropertyExtendAttribute)item).PropertyExtend; continue; }
                    // 存储过程参数
                    if (item is ProcAttribute) { fieldMapState.IsOutParam = ((ProcAttribute)item).IsOutParam; fieldMapState.IsInParam = ((ProcAttribute)item).IsInParam; continue; }
                }
                //if (fieldMapState.Display == null) { fieldMapState.Display = new DisplayAttribute { Name = propertyInfo.Name }; }
                //if (fieldMapState.Display.Name.IsNullOrEmpty()) { fieldMapState.Display.Name = propertyInfo.Name; }

                if (fieldMapState.Column == null) { fieldMapState.Column = new ColumnAttribute { Name = propertyInfo.Name }; }
                if (string.IsNullOrEmpty(fieldMapState.Column.Name)) { fieldMapState.Column.Name = propertyInfo.Name; }

                if (fieldMapState.IsDbField && fieldMapState.Column.IsDbGenerated) { IndexName = fieldMapState.Column.Name; } else { fieldMapState.Column.IsDbGenerated = false; }

                //添加属变量标记名称
                ModelList.Add(propertyInfo, fieldMapState);
            }
            #endregion
        }

        /// <summary>
        ///     类属性
        /// </summary>
        public DBAttribute ClassInfo { get; set; }

        /// <summary>
        ///     自增ID
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        private Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator TableMap(Type type)
        {
            return TableMapCache.GetMap(type);
        }

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        public KeyValuePair<PropertyInfo, FieldMapState> GetModelInfo(string fieldName = "")
        {
            return string.IsNullOrEmpty(fieldName) ? ModelList.FirstOrDefault(oo => oo.Value.Column.IsDbGenerated) : ModelList.FirstOrDefault(oo => oo.Key.Name == fieldName);
        }

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="propertyInfo">属性变量</param>
        /// <returns></returns>
        public string GetFieldName(PropertyInfo propertyInfo)
        {
            return ModelList[propertyInfo].Column.Name;
        }
    }
}