using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core.Data
{
    public class Queue : IQueue
    {
        public Guid ID { get; set; }
        public int Index { get; set; }
        public Dictionary<Expression, bool> ExpOrderBy { get; set; }
        public List<Expression> ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public StringBuilder Sql { get; set; }
        public List<DbParameter> Param { get; set; }
        public string Name { get; set; }
        public FieldMap FieldMap { get; set; }
        public IBuilderSqlOper SqlBuilder { get; set; }
        public Action<Queue> LazyAct { get; set; }
        public Dictionary<Expression, object> ExpAssign { get; set; }
        public Queue(int index, string name, FieldMap map, IQueueManger queueManger)
        {
            ID = Guid.NewGuid();
            Index = index;
            Name = name;
            Param = new List<DbParameter>();
            FieldMap = map;
            SqlBuilder = queueManger.DbProvider.CreateBuilderSqlOper(queueManger, this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                if (Sql != null) { Sql.Clear(); Sql = null; }

                ExpOrderBy = null;
                ExpSelect = null;
                ExpWhere = null;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
