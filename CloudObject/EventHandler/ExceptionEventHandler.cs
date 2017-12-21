namespace CloudObject.EventHandler
{
    public class ExceptionEventHandler
    {
        public ExceptionEventHandler(string cloudId, string taskId, string excepption)
        {
            CloudId = cloudId;
            TaskId = taskId;
            Excepption = excepption;
        }

        public string CloudId { get; set; }

        public string TaskId { get; set; }

        public string Excepption { get; set; }
    }
}
