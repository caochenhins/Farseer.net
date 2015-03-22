using FS.Core.Context;
using FS.Mapping.Table;

namespace Demo.PO.Proc
{
    [DB(Name = "sp_Info_User")]
    public class InfoUserPO : ProcContext<InfoUserPO>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Proc()]
        public int? ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
    }
}