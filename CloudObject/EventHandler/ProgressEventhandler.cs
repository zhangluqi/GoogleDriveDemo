namespace CloudObject.EventHandler
{
    public class ProgressEventhandler
    {
        public ProgressEventhandler(string couldId, string taskId, decimal progress, decimal speed)
        {
            CouldId = couldId;
            TaskId = taskId;
            Progress = progress;
            Speed = speed;
        }

        public string CouldId { get; set; }

        public string TaskId { get; set; }

        public decimal Progress { get; set; }

        public decimal Speed { get; set; }
    }
}
