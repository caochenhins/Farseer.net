using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Core.Infrastructure;

namespace Demo.VO.Members
{
    public class UserRoleVO : IEntity
    {
        public int? ID { get; set; }
        public string Caption { get; set; }
        public string Descr { get; set; }
    }
}
