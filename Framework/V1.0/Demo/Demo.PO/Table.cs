using Demo.VO.Members;
using FS.Core.Data.Table;
using FS.Mapping.Context.Attribute;

namespace Demo.PO
{
    [Context()]
    public class Table : TableContext<Table>
    {
        [Set(Name = "Members_User")]
        public TableSet<UserVO> User { get; set; }

        [Set(Name = "Members_Role", IsCache = true)]
        public TableSet<UserRoleVO> UserRole { get; set; }
    }
}