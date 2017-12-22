using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CloudManagerment.Model
{
    #region OneDrive Search Model
    [DataContract]
    public class DriveList
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "Service")]
        public string Service { get; set; }

        [DataMember(Name = "Token")]
        public string Token { get; set; }

        [DataMember(Name = "ExpiresAt")]
        public string ExpiresAt { get; set; }

        [DataMember(Name = "ExpiresIn")]
        public string ExpiresIn { get; set; }
    }
    #endregion
}
