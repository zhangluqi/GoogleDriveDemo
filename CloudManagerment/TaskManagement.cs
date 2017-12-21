using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudObject;

namespace CloudManagerment
{
    public static class TaskManagement
    {
        private static readonly IList<CloudTask> Tasks = new List<CloudTask>();

        public static CloudTask GetOneTask(Guid taskId)
        {
            return Tasks.FirstOrDefault(t => t.CloudId.Equals(taskId));
        }

        public static CloudTask AddTask(string source, Task task, CancellationTokenSource cancel,string target, long position, Guid cloudId)
        {
            var atask = new CloudTask()
            {
                TaskId = Guid.NewGuid(),
                CancelSignal = cancel,
                Task = task,
                Source = source,
                Target= target,
                Position= position,
                CloudId = cloudId
            };
            Tasks.Add(atask);
            return atask;
        }

        public static CloudTask RemoveTask(Guid taskId)
        {
            var task = GetOneTask(taskId);
            Tasks.Remove(task);
            return task;
        }
    }
}
