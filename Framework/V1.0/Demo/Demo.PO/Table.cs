using Demo.VO.Members;
using FS.Core.Data.Table;
using FS.Mapping.Table;

namespace Demo.PO
{
    public class Table : TableContext
    {
        public Table() : base(0) { }

        [DB(Name = "Members_User")]
        public TableSet<UserVO> User { get; set; }
    }

    [DB(Name = "Members_User")]
    public class UserPO : TableContext<UserVO> { }
}