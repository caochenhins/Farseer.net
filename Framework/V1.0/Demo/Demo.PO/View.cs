using System.Data.Linq.Mapping;
using Demo.VO.Members;
using FS.Core.Data.View;
using FS.Mapping.Table;

namespace Demo.PO
{
    public class View : ViewContext<View>
    {
        public View() : base(0) { }

        [Column(Name = "View_Account")]
        public ViewSet<AccountVO> Account { get; set; }
    }

    [DB("View_Account")]
    public class AccountPO : ViewContext<AccountPO, AccountVO> { }
}