using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Gmail
{
    public class GmailLoad
    {
        /// <summary>
        /// Get and store attachment from Message with given ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="messageId">ID of Message containing attachment.</param>
        /// <param name="outputDir">Directory used to store attachments.</param>
        public  void GetAttachments(GmailService service, String userId, String messageId, String outputDir)
        {
            try
            {
                Message message = service.Users.Messages.Get(userId, messageId).Execute();
                IList<MessagePart> parts = message.Payload.Parts;
                foreach (MessagePart part in parts)
                {
                    if (!String.IsNullOrEmpty(part.Filename))
                    {
                        String attId = part.Body.AttachmentId;
                        MessagePartBody attachPart = service.Users.Messages.Attachments.Get(userId, messageId, attId).Execute();

                        // Converting from RFC 4648 base64 to base64url encoding
                        // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                        String attachData = attachPart.Data.Replace('-', '+');
                        attachData = attachData.Replace('_', '/');

                        byte[] data = Convert.FromBase64String(attachData);
                        File.WriteAllBytes(System.IO.Path.Combine(outputDir, part.Filename), data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

        }

    }
}
