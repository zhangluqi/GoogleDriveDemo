using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Gmail
{
    public class GmailSearch
    {

        /// <summary>
        /// 获取文件列表
        /// </summary>
        public void SearchMail(GmailService gmailService, Action<Message> searchResult)
        {
            if (gmailService == null)
            {
                return;
            }

            List<Message> messsageList = ListMessages(gmailService, "me", "");
            //这里目前只找到了重新请求一次的方式
            if (messsageList != null)
            {
                foreach (var m in messsageList)
                {
                    Message m2 = gmailService.Users.Messages.Get("me", m.Id).Execute();

                    //这里只是为了看获取Full跟上面有什么区别
                    var request = gmailService.Users.Messages.Get("me", m.Id);
                    request.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                    Message detailMessage = request.Execute();
                    if(detailMessage != null && detailMessage.Payload != null && detailMessage.Payload.Parts != null)
                    {
                        foreach (var item in detailMessage.Payload.Parts)
                        {
                            if(item.Body!= null && item.Body.Data != null)
                            {
                                try
                                {
                                    var base64EncodedBytes = System.Convert.FromBase64String(item.Body.Data);
                                    string test = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                                    // Debug.WriteLine(item.Body.);
                                    Debug.WriteLine(test);
                                }catch(Exception ex)
                                {
                                    Debug.WriteLine("Decode gmail exception:"+ex.ToString());
                                }
                              
                            }
                        }
                    }
                   
                    if (m2 != null)
                    {
                        searchResult?.Invoke(m2);
                    }
                }
            }


        }


        /// List all Messages of the user's mailbox matching the query.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="query">String used to filter Messages returned.</param>
        private List<Message> ListMessages(GmailService service, String userId, String query)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);
            // request.Fields = "nextPageToken, files(id,payload)"; //这个地方目前加了筛选规则还有问题 需要进一步研究
            //  request.Q = query;
            do
            {
                try
                {
                    ListMessagesResponse response = request.Execute();
                    result.AddRange(response.Messages);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

    }
}
