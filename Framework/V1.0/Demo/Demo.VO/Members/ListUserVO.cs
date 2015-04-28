using FS.Mapping.Table;
using FS.Mapping.Table.Attribute;

namespace Demo.VO.Members
{
    public class ListUserVO
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