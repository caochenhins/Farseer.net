using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FS.Core.Infrastructure;

namespace FS.Extend
{
    public static class WebDropDownListExtend
    {/// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="parentID">所属上级节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue, int parentID, bool isUsePrefix = true) where TInfo : ICate, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, parentID, 0, null, false, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="where">筛选条件</param>
        /// <param name="isContainsSub">筛选条件是否包含子节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue = 0, Func<TInfo, bool> where = null, bool isContainsSub = false, bool isUsePrefix = true) where TInfo : ICate, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, 0, 0, where, isContainsSub, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     递归绑定
        /// </summary>
        private static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int parentID, int tagNum, Func<TInfo, bool> where, bool isContainsSub, bool isUsePrefix) where TInfo : ICate, new()
        {
            List<TInfo> lst;

            lst = lstCate.FindAll(o => o.ParentID == parentID);
            if (lst == null || lst.Count == 0) { return; }

            if ((parentID == 0 || isContainsSub) && where != null) { lst = lst.Where(where).ToList(); }
            if (lst == null || lst.Count == 0) { return; }

            foreach (var info in lst)
            {
                var text = isUsePrefix ? new string('　', tagNum) + "├─" + info.Caption : info.Caption;

                ddl.Items.Add(new ListItem { Value = info.ID.ToString(), Text = text });
                lstCate.Bind(ddl, info.ID.Value, tagNum + 1, where, isContainsSub, isUsePrefix);
            }
        }
    }
}
