using FS.Core.Context;
using FS.Mapping.Table;

namespace Demo.PO.Proc
{
    [DB(Name = "sp_Value_User")]
    public class ValueUserPO : ProcContext<ValueUserPO>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Proc()]
        public int? ID { get; set; }
    }
}