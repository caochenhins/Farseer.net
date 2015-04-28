using Demo.VO.Members;
using FS.Core.Data.Proc;
using FS.Mapping.Table.Attribute;

namespace Demo.PO
{
    public class Proc : ProcContext<Proc>
    {
        [Column(Name = "sp_Info_User")]
        public ProcSet<InfoUserVO> InfoUser { get; set; }

        [Column(Name = "sp_Insert_User")]
        public ProcSet<InsertUserVO> InsertUser { get; set; }

        [Column(Name = "sp_List_User")]
        public ProcSet<ListUserVO> ListUser { get; set; }

        [Column(Name = "sp_Value_User")]
        public ProcSet<ValueUserVO> ValueUser { get; set; }
    }
}
