using System;

namespace CloudObject
{
    public class Cloud
    {
        public string CloudDriveId { get; set; }

        public string CloudEmail { get; set; }

        public string CloudService { get; set; }

        public string CloudToken { get; set; }

        public DateTime CloudExpiresAt { get; set; }

        public int CloudExpiresIn { get; set; }
    }
}
