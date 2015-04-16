using Demo.VO.Members;
using FS.Core.Data.View;
using FS.Mapping.Table;

namespace Demo.PO
{
    public class View : ViewContext
    {
        public View() : base(0) { }

        [DB(Name = "View_Account")]
        public ViewSet<AccountVO> Account { get; set; }
    }

    [DB(Name = "View_Account")]
    public class AccountPO : ViewContext<AccountVO> { }
}