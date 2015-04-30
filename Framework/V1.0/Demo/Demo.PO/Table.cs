using Demo.VO.Members;
using FS.Core.Data.Table;
using FS.Mapping.Table.Attribute;

namespace Demo.PO
{
    public class Table : TableContext<Table>
    {
        [Column(Name = "Members_User")]
        public TableSet<UserVO> User { get; set; }

        [Column(Name = "Members_Role", IsCache = true)]
        public TableSet<UserRoleVO> UserRole { get; set; }
    }
}