using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FS.Extend
{
    public static class Extends
    {
        /// <summary>
        ///     检查是否存在该类型的子窗体
        /// </summary>
        /// <param name="form">Windows窗体对象</param>
        /// <param name="childFormName">窗体名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExist(this Form form, string childFormName)
        {
            foreach (var frm in form.MdiChildren)
            {
                if (frm.GetType().Name == childFormName)
                {
                    frm.Activate();
                    return true;
                }
            }
            return false;
        }
    }
}
