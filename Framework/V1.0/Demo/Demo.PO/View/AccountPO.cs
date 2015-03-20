using System.Data.Linq.Mapping;
using Demo.Common;
using FS.Core.Context;
using FS.Mapping.Table;

namespace Demo.PO.View
{
    [DB(Name = "View_Account")]
    public class AccountPO : ViewContext<AccountPO>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 登陆IP
        /// </summary>
        [Column(Name = "getdate()")]
        public string GetDate { get; set; }
    }
}