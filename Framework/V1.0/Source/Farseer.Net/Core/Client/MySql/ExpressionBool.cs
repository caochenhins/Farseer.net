using FS.Core.Infrastructure;

namespace FS.Core.Client.MySql
{
    public class ExpressionBool : Common.ExpressionBool
    {
        public ExpressionBool(IQueueManger queueManger, IQueue queue) : base(queueManger, queue) { }
    }
}
