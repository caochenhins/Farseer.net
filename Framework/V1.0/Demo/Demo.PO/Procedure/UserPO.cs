using FS.Core.Context;
using FS.Mapping.Table;

namespace Demo.PO.Procedure
{
    [DB(Name = "sp_Insert_User")]
    public class InsertUserPO : ProcContext<InsertUserPO>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Proc(IsOutParam = true)]
        public int? ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Proc()]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Proc()]
        public string PassWord { get; set; }
    }
}