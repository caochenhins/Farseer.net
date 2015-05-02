using FS.Core.Infrastructure;

namespace FS.Core.Client.OleDb
{
    public class ExpressionBool : Common.ExpressionBool
    {
        public ExpressionBool(IQueueManger queueManger, IQueueSql queueSql) : base(queueManger, queueSql) { }
    }
}
