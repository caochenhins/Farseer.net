using FS.Mapping.Table;
using FS.Mapping.Table.Attribute;

namespace Demo.VO.Members
{
    public class InsertUserVO 
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