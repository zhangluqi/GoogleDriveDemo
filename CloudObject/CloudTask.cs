using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudObject
{
    public sealed class CloudTask
    {
        public Guid TaskId { get; set; }
        public Task Task { get; set; }
        public CancellationTokenSource CancelSignal { get; set; } 
        public string Source { get; set; }
        public string Target { get; set; }
        public long Position { get; set; }
        public Guid CloudId { get; set; }

    }
}
