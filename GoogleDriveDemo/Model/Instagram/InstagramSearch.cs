using GoogleDriveDemo.Model.Common;
using HttpUtilLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Instagram
{
    class InstamgramBean
    {
        public string id;
        public object user;
        public Dictionary<string,Dictionary<string,string>>images;
        public bool user_has_liked;
        public object likes;
        public object users_in_photo;
    }

    class InstagramBeans
    {
        public object pagination;
        public List<InstamgramBean> data;
    }


    public class InstagramSearch
    {
        public void SearchFile(InstamgramClient instamgramClient, Action<ImageInfo> searchResult)
        {
            if(instamgramClient == null)
            {
                return;
            }
            NetClient netClient = new NetClient();
            string currentPath = "/users/self/media/recent/";
            string currentHost = "api.instagram.com/v1";
            string photoList = instamgramClient.SearchCurrent(currentHost, currentPath);
            if(!string.IsNullOrEmpty(photoList))
            {
                InstagramBeans instagramBeans = JsonConvert.DeserializeObject<InstagramBeans>(photoList);
                if(instagramBeans != null && instagramBeans.data != null)
                {
                    foreach (var image in instagramBeans.data)
                    {
                        if (image.images != null && image.images.ContainsKey("standard_resolution"))
                        {

                            int width = int.Parse(image.images["standard_resolution"]["width"]);
                            int height = int.Parse(image.images["standard_resolution"]["height"]);
                            string url = image.images["standard_resolution"]["url"];
                            ImageInfo imageInfo = new ImageInfo() {
                                Width = width,
                                Height = height,
                                Url = url
                            };
                            searchResult?.Invoke(imageInfo);

                        }
                       
                    }
                }

            }
          



        }
    }
}
