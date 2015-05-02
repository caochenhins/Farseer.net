using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb.SqlBuilder
{
    public sealed class SqlProc : Common.SqlBuilder.SqlProc
    {
        public SqlProc(IQueueManger queueManger, IQueue queueSql) : base(queueManger, queueSql) { }
    }
}