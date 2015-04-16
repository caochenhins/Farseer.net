using System;
using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Infrastructure;

namespace FS.Core.Data.Proc
{
    public class ProcQueue : IQueue, IDisposable
    {
        public Guid ID { get; set; }
        public int Index { get; set; }
        public List<DbParameter> Param { get; set; }
        public string Name { get; set; }
        public ProcQueue(int index, string name)
        {
            ID = Guid.NewGuid();
            Index = index;
            Name = name;
            Param = new List<DbParameter>();
        }

        public Action<ProcQueue> LazyAct { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}