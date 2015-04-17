using System.Data.Linq.Mapping;
using Demo.VO.Members;
using FS.Core.Data.Proc;
using FS.Mapping.Table;

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

    [DB("sp_Info_User")]
    public class InfoUserPO : ProcContext<InfoUserPO, InfoUserVO> { }

    [DB("sp_Insert_User")]
    public class InsertUserPO : ProcContext<InsertUserPO, InsertUserVO> { }

    [DB("sp_List_User")]
    public class ListUserPO : ProcContext<ListUserPO, ListUserVO> { }

    [DB("sp_Value_User")]
    public class ValueUserPO : ProcContext<ValueUserPO, ValueUserVO> { }
}
