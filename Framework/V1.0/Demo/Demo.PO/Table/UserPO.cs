using System.Data.Linq.Mapping;
using Demo.Common;
using Demo.VO.Members;
using FS.Core.Context;
using FS.Mapping.Table;

namespace Demo.PO.Table
{
    [DB(Name = "Members_User")]
    public class UserPO : TableContext<UserVO>
    {
    }
}