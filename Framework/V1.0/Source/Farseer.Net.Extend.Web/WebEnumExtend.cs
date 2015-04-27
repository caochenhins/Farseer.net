using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FS.Extend
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static class WebEnumExtend
    {
        /// <summary>
        ///     枚举转ListItem
        /// </summary>
        public static List<SelectListItem> ToSelectListItem(this Type enumType)
        {
            var lst = new List<SelectListItem>();
            foreach (int value in Enum.GetValues(enumType))
            {
                lst.Add(new SelectListItem
                {
                    Value = value.ToString(),
                    Text = EnumExtend.GetName((Enum)Enum.ToObject(enumType, value))
                });
            }
            return lst;
        }
    }
}