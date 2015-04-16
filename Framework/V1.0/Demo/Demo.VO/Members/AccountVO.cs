using System.Data.Linq.Mapping;

namespace Demo.VO.Members
{
    public class AccountVO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Column(IsDbGenerated = true)]
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