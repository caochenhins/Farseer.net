﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FS.Core;
using FS.Core.Infrastructure;

namespace FS.Extend
{
    /// <summary>
    ///     DataTable扩展工具
    /// </summary>
    public static class DataTableExtend
    {
        /// <summary>
        ///     对DataTable排序
        /// </summary>
        /// <param name="dt">要排序的表</param>
        /// <param name="sort">要排序的字段</param>
        public static DataTable Sort(this DataTable dt, string sort = "ID DESC")
        {
            var rows = dt.Select("", sort);
            var tmpDt = dt.Clone();

            foreach (var row in rows)
            {
                tmpDt.ImportRow(row);
            }
            return tmpDt;
        }

        /// <summary>
        ///     对DataTable分页
        /// </summary>
        /// <param name="dt">源表</param>
        /// <param name="pageSize">每页显示的记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public static DataTable Split(this DataTable dt, int pageSize = 20, int pageIndex = 1)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            var dtNew = dt.Clone();

            int firstIndex;

            #region 计算 开始索引

            if (pageIndex == 1)
            {
                firstIndex = 0;
            }
            else
            {
                firstIndex = pageSize * (pageIndex - 1);
                //索引超出记录总数时，返回空的表格
                if (firstIndex > dt.Rows.Count)
                {
                    return dtNew;
                }
            }

            #endregion

            #region 计算 结束索引

            var endIndex = pageSize + firstIndex;
            if (endIndex > dt.Rows.Count)
            {
                endIndex = dt.Rows.Count;
            }

            #endregion

            for (var i = firstIndex; i < endIndex; i++)
            {
                dtNew.ImportRow(dt.Rows[i]);
            }
            return dtNew;
        }

        /// <summary>
        ///     DataTable倒序
        /// </summary>
        /// <param name="dt">源DataTable</param>
        public static DataTable Reverse(this DataTable dt)
        {
            var tmpDt = dt.Clone();

            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                tmpDt.ImportRow(dt.Rows[i]);
            }
            return tmpDt;
        }

        /// <summary>
        ///     DataTable深度复制
        /// </summary>
        /// <param name="dt">要排序的表</param>
        public static DataTable CloneData(this DataTable dt)
        {
            var newTable = dt.Clone();
            dt.Rows.ToRows().ForEach(newTable.ImportRow);
            return newTable;
        }

        /// <summary>
        ///     DataTable转换为实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DataTable dt) where TEntity : class, new()
        {
            var list = new List<TEntity>();
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            foreach (DataRow dr in dt.Rows)
            {
                // 赋值字段
                var t = new TEntity();
                foreach (var kic in map.MapList)
                {
                    if (!kic.Key.CanWrite) { continue; }
                    var filedName = !DbProvider.IsField(kic.Value.FieldAtt.Name) ? kic.Key.Name : kic.Value.FieldAtt.Name;
                    if (dr.Table.Columns.Contains(filedName))
                    {
                        kic.Key.SetValue(t, dr[filedName].ConvertType(kic.Key.PropertyType), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        ///     将DataRowCollection转成List[DataRow]
        /// </summary>
        /// <param name="drc">DataRowCollection</param>
        public static List<DataRow> ToRows(this DataRowCollection drc)
        {
            var lstRow = new List<DataRow>();
            if (drc == null) { return lstRow; }
            lstRow.AddRange(drc.Cast<DataRow>());
            return lstRow;
        }

        /// <summary>
        ///     将DataRow转成实体类
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="dr">源DataRow</param>
        public static TEntity ToInfo<TEntity>(this DataRow dr) where TEntity : class,new()
        {
            var map = CacheManger.GetFieldMap(typeof(TEntity));
            var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

            //赋值字段
            foreach (var kic in map.MapList)
            {
                if (dr.Table.Columns.Contains(kic.Value.FieldAtt.Name))
                {
                    if (!kic.Key.CanWrite) { continue; }
                    kic.Key.SetValue(t, dr[kic.Value.FieldAtt.Name].ConvertType(kic.Key.PropertyType), null);
                }
            }
            return t ?? new TEntity();
        }
    }
}