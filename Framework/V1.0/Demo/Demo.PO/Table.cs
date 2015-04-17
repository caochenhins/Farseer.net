using System.Data.Linq.Mapping;
using Demo.VO.Members;
using FS.Core.Data.Table;
using FS.Mapping.Table;

namespace Demo.PO
{
    public class Table : TableContext<Table>
    {
        //public Table() : base(0) { }

        [Column(Name = "Members_User")]
        public TableSet<UserVO> User { get; set; }
    }

    [DB("Members_User")]
    public class UserPO : TableContext<UserPO, UserVO> { }
}