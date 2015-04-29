using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}