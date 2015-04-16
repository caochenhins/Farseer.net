using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;

namespace FS.Core.Data.Table
{
    public class TableQueue : IQueueSql
    {
        public Guid ID { get; set; }
        public int Index { get; set; }
        public Dictionary<Expression, bool> ExpOrderBy { get; set; }
        public List<Expression> ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public Dictionary<Expression, object> ExpAssign { get; set; }
        public StringBuilder Sql { get; set; }
        public List<DbParameter> Param { get; set; }
        public string Name { get; set; }
        public Action<TableQueue> LazyAct { get; set; }
        public TableQueue(int index, string name)
        {
            ID = Guid.NewGuid();
            Index = index;
            Name = name;
            Param = new List<DbParameter>();
        }
        public void Dispose()
        {
            if (Sql != null) { Sql.Clear(); Sql = null; }

            ExpOrderBy = null;
            ExpSelect = null;
            ExpWhere = null;

            GC.SuppressFinalize(this);
        }
    }
}
