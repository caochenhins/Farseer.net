using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.SqlBuilder
{
    public class SqlProc : Common.SqlBuilder.SqlProc
    {
        public SqlProc(IQueueManger queueManger, IQueue queueSql) : base(queueManger, queueSql) { }
    }
}