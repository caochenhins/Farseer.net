using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using FS.Mapping.Table;

namespace FS.Extend
{
    public static class IDataReaderExtend2
    {
        public static List<T> ToList2<T>(this IDataReader reader) where T : class, new()
        {
            return EntityConverter<T>.ToList(reader);
        }
        public static T ToInfo2<T>(this IDataReader reader) where T : class, new()
        {
            return EntityConverter<T>.ToList(reader).FirstOrDefault();
        }

        #region Static Readonly Fields
        private static readonly MethodInfo DataRecordItemGetterInt = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo DataRecordGetOrdinal = typeof(IDataRecord).GetMethod("GetOrdinal");
        private static readonly MethodInfo DataRecordHaveName = typeof(IDataReaderExtend).GetMethod("HaveName");
        private static readonly MethodInfo DataReaderRead = typeof(IDataReader).GetMethod("Read");
        private static readonly MethodInfo ConvertIsDbNull = typeof(Convert).GetMethod("IsDBNull");
        private static readonly MethodInfo DataRecordGetDateTime = typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecordGetDecimal = typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecordGetDouble = typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecordGetByte = typeof(IDataRecord).GetMethod("GetByte");
        private static readonly MethodInfo DataRecordGetInt32 = typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecordGetInt64 = typeof(IDataRecord).GetMethod("GetInt64");
        private static readonly MethodInfo DataRecordGetString = typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecordIsDbNull = typeof(IDataRecord).GetMethod("IsDBNull");
        #endregion

        private class EntityConverter<T> where T : class, new()
        {
            private struct DbColumnInfo
            {
                private readonly string _propertyName;
                public readonly string ColumnName;
                public readonly Type Type;
                public readonly MethodInfo SetMethod;

                public DbColumnInfo(PropertyInfo prop, ColumnAttribute attr)
                {
                    _propertyName = prop.Name;
                    ColumnName = attr.Name ?? prop.Name;
                    Type = prop.PropertyType;
                    SetMethod = prop.GetSetMethod(false);
                }
            }

            private static Converter<IDataReader, List<T>> _batchDataLoader;

            private static Converter<IDataReader, List<T>> BatchDataLoader
            {
                get
                {
                    return _batchDataLoader ?? (_batchDataLoader = CreateBatchDataLoader(new List<DbColumnInfo>(GetProperties())));
                }
            }

            private static IEnumerable<DbColumnInfo> GetProperties()
            {
                var dbResult = Attribute.GetCustomAttribute(typeof(T), typeof(DBAttribute), true) as DBAttribute;
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (prop.GetIndexParameters().Length > 0) { continue; }
                    var setMethod = prop.GetSetMethod(false);
                    if (setMethod == null) { continue; }
                    var attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute), true) as ColumnAttribute;
                    if (dbResult != null)
                    {
                        if (attr == null) { attr = new ColumnAttribute(); }
                    }
                    else if (attr == null) { continue; }
                    yield return new DbColumnInfo(prop, attr);
                }
            }

            internal static List<T> ToList(IDataReader reader)
            {
                return BatchDataLoader(reader);
            }
            #region Init Methods

            /// <summary>
            /// 实例List，并添加Item
            /// </summary>
            /// <param name="columnInfoes"></param>
            /// <returns></returns>
            private static Converter<IDataReader, List<T>> CreateBatchDataLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(String.Empty, typeof(List<T>), new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var list = il.DeclareLocal(typeof(List<T>));
                var item = il.DeclareLocal(typeof(T));
                var exit = il.DefineLabel();
                var loop = il.DefineLabel();
                // List<T> list = new List<T>();
                il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, list);
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                LocalBuilder[] colIndices = GetColumnIndices(il, columnInfoes);
                // while (arg.Read()) {
                il.MarkLabel(loop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, DataReaderRead);
                il.Emit(OpCodes.Brfalse, exit);
                //      T item = new T { %Property% =  };
                BuildItem(il, columnInfoes, item, colIndices);
                //      list.Add(item);
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
                // }
                il.Emit(OpCodes.Br, loop);
                il.MarkLabel(exit);
                // return list;
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, List<T>>)dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
            }

            /// <summary>
            /// 找到对应字段映射
            /// </summary>
            /// <param name="il"></param>
            /// <param name="columnInfoes"></param>
            /// <returns></returns>
            private static LocalBuilder[] GetColumnIndices(ILGenerator il, IList<DbColumnInfo> columnInfoes)
            {
                var colIndices = new LocalBuilder[columnInfoes.Count];
                for (var i = 0; i < colIndices.Length; i++)
                {
                    // int %index% = arg.GetOrdinal(%ColumnName%);
                    colIndices[i] = il.DeclareLocal(typeof(int));
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
                    //il.Emit(OpCodes.Stloc_S, colIndices);
                    il.Emit(OpCodes.Callvirt, DataRecordHaveName);
                    il.Emit(OpCodes.Stloc_S, colIndices[i]);
                }
                return colIndices;
            }

            private static void BuildItem(ILGenerator il, List<DbColumnInfo> columnInfoes, LocalBuilder item, LocalBuilder[] colIndices)
            {
                // T item = new T();
                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, item);
                for (var i = 0; i < colIndices.Length; i++)
                {
                    var type = columnInfoes[i].Type;
                    // 带?号的类型
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // 得到参数里的类型
                        var argumentsType = type.GetGenericArguments()[0];
                        // item.%Property% = arg.IsDBNull ? default(int?) : (int?)arg.GetInt32(%index%);
                        if (IsCompatibleType(argumentsType, typeof(int))) { ReadNullableInt32(il, item, columnInfoes, colIndices, i); continue; }
                        // item.%Property% = arg.IsDBNull ? default(long?) : (long?)arg.GetInt64(%index%);
                        if (IsCompatibleType(argumentsType, typeof(long))) { ReadNullableInt64(il, item, columnInfoes, colIndices, i); continue; }
                        // item.%Property% = arg.IsDBNull ? default(decimal?) : (int?)arg.GetDecimal(%index%);
                        if (IsCompatibleType(argumentsType, typeof(decimal))) { ReadNullableDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]); continue; }
                        // item.%Property% = arg.IsDBNull ? default(DateTime?) : (int?)arg.GetDateTime(%index%);
                        if (IsCompatibleType(argumentsType, typeof(DateTime))) { ReadNullableDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]); continue; }
                        if (IsCompatibleType(argumentsType, typeof(Enum)))
                        {
                            var underlyingType = Enum.GetUnderlyingType(argumentsType);
                            if (underlyingType == typeof(byte)) { ReadNullableByte(il, item, columnInfoes, colIndices, i); continue; }
                            if (underlyingType == typeof(int)) { ReadNullableInt32(il, item, columnInfoes, colIndices, i); continue; }
                            if (underlyingType == typeof(Int64)) { ReadNullableInt64(il, item, columnInfoes, colIndices, i); continue; }
                        }
                    }
                    else
                    {
                        // item.%Property% = arg.GetInt32(%index%);
                        if (IsCompatibleType(type, typeof(int))) { ReadInt32(il, item, columnInfoes, colIndices, i); continue; }
                        // item.%Property% = arg.GetInt64(%index%);
                        if (IsCompatibleType(type, typeof(long))) { ReadInt64(il, item, columnInfoes, colIndices, i); continue; }
                        // item.%Property% = arg.GetDecimal(%index%);
                        if (IsCompatibleType(type, typeof(decimal))) { ReadDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]); continue; }
                        // item.%Property% = arg.GetDateTime(%index%);
                        if (type == typeof(DateTime)) { ReadDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]); continue; }
                    }

                    // item.%Property% = (%PropertyType%)arg[%index%];
                    ReadObject(il, item, columnInfoes, colIndices, i);
                }
            }

            /// <summary>
            /// 判断两者类型是否相等
            /// </summary>
            /// <param name="t1">要判断的类型</param>
            /// <param name="t2">目标类型</param>
            private static bool IsCompatibleType(Type t1, Type t2)
            {
                if (t1 == t2) { return true; }
                if (t1.IsEnum && Enum.GetUnderlyingType(t1) == t2) { return true; }
                if (t1.IsEnum && t1.BaseType == t2) { return true; }
                var u1 = Nullable.GetUnderlyingType(t1);
                var u2 = Nullable.GetUnderlyingType(t2);
                if (u1 != null && u2 != null) { return IsCompatibleType(u1, u2); }
                return false;
            }
            private static void ReadNullableByte(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                var intNull = il.DefineLabel();
                var intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetByte);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt32);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }
            private static void ReadNullableInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                var intNull = il.DefineLabel();
                var intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt32);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }


            private static void ReadInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt64);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }
            private static void ReadNullableInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                var intNull = il.DefineLabel();
                var intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt64);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }


            private static void ReadDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDecimal);
                il.Emit(OpCodes.Callvirt, setMethod);
            }
            private static void ReadNullableDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(decimal?));
                var decimalNull = il.DefineLabel();
                var decimalCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, decimalNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDecimal);
                il.Emit(OpCodes.Call, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
                il.Emit(OpCodes.Br_S, decimalCommon);
                il.MarkLabel(decimalNull);
                il.Emit(OpCodes.Initobj, typeof(decimal?));
                il.MarkLabel(decimalCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }


            private static void ReadDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDateTime);
                il.Emit(OpCodes.Callvirt, setMethod);
            }
            private static void ReadNullableDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(DateTime?));
                Label dtNull = il.DefineLabel();
                Label dtCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, dtNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDateTime);
                il.Emit(OpCodes.Call, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
                il.Emit(OpCodes.Br_S, dtCommon);
                il.MarkLabel(dtNull);
                il.Emit(OpCodes.Initobj, typeof(DateTime?));
                il.MarkLabel(dtCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadObject(ILGenerator il, LocalBuilder item, IList<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var common = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordItemGetterInt);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Call, ConvertIsDbNull);
                il.Emit(OpCodes.Brfalse_S, common);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldnull);
                il.MarkLabel(common);
                il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            #endregion
        }
    }

    /// <summary>
    ///     IDataReader扩展工具
    /// </summary>
    public static class IDataReaderExtend
    {
        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this IDataReader reader) where T : class, new()
        {
            var list = new List<T>();
            var Map = TableMapCache.GetMap<T>();
            T t;

            while (reader.Read())
            {
                t = (T)Activator.CreateInstance(typeof(T));

                //赋值字段
                foreach (var kic in Map.ModelList)
                {
                    if (reader.HaveName(kic.Value.Column.Name))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        var type = kic.Key.PropertyType;
                        if (kic.Key.PropertyType.IsGenericType && kic.Key.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            type = kic.Key.PropertyType.GetGenericArguments()[0];
                        }
                        kic.Key.SetValue(t, Convert.ChangeType(reader[kic.Value.Column.Name], type), null);
                    }
                }

                list.Add(t);
            }
            reader.Close();
            reader.Dispose();
            return list;
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="T">实体类</typeparam>
        public static T ToInfo<T>(this IDataReader reader) where T : class, new()
        {
            var map = TableMapCache.GetMap<T>();

            var t = (T)Activator.CreateInstance(typeof(T));
            var isHaveValue = false;

            if (reader.Read())
            {
                //赋值字段
                foreach (var kic in map.ModelList)
                {
                    if (reader.HaveName(kic.Value.Column.Name))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(t, Convert.ChangeType(reader[kic.Value.Column.Name], kic.Key.PropertyType), null);
                        isHaveValue = true;
                    }
                }
            }
            reader.Close();
            reader.Dispose();
            return isHaveValue ? t : null;
        }

        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(name)) { return true; }
            }
            return false;
        }
    }
}