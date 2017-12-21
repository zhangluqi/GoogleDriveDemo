using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Facebook
{
    public class FacebookSearch
    {

        public void SearchFile(FacebookClient facebookClient, Action<FacebookInfo> searchResult) {
            // dynamic user = facebookClient.Get("/me");

             UserPhotos(facebookClient,"me" , searchResult);

             UserVideos(facebookClient, "me", searchResult);
        }



        private void UserVideos(FacebookClient fb, string userID, Action<FacebookInfo> searchResult)
        {
            ArrayList imageSize; //for gettig highest pixell of picture
            dynamic albums = fb.Get("me/videos"); // THİS code is rigth runnig
            //dynamic albums = fb.Get(userID + "/albums");
            foreach (dynamic albumInfo in albums.data)
            {
                //Get the Pictures inside the album this gives JASON objects list that has photo attributes 
                // described here http://developers.facebook.com/docs/reference/api/photo/
                dynamic albumsPhotos = fb.Get(albumInfo.id + "/photos");
                foreach (dynamic rs in albumsPhotos.data)
                {
                    dynamic photoInfo = fb.Get(rs.id + "?fields=images");
                    imageSize = new ArrayList();
                    foreach (dynamic rsa in photoInfo.images)
                    {
                        imageSize.Add(rsa.height);
                    }
                    int highestImage = Convert.ToInt32(imageSize[0]);
                    foreach (dynamic rsa in photoInfo.images)
                    {
                        //这里有多个尺寸的
                        if (rsa.height == highestImage)
                        {
                            FacebookInfo facebookInfo = new FacebookInfo();
                            facebookInfo.Url = rsa.source;

                            //facebookInfo.Width = rsa.
                            //   facebookInfo.Height = 
                            searchResult?.Invoke(facebookInfo);
                        }
                    }

                }
            }
        }


        private void UserPhotos(FacebookClient fb, string userID, Action<FacebookInfo> searchResult)
        {

            ArrayList imageSize; //for gettig highest pixell of picture
            dynamic albums = fb.Get("me/albums"); // THİS code is rigth runnig
            //dynamic albums = fb.Get(userID + "/albums");
            foreach (dynamic albumInfo in albums.data)
            {
                //Get the Pictures inside the album this gives JASON objects list that has photo attributes 
                // described here http://developers.facebook.com/docs/reference/api/photo/
                dynamic albumsPhotos = fb.Get(albumInfo.id + "/photos");
                foreach (dynamic rs in albumsPhotos.data)
                {
                    dynamic photoInfo = fb.Get(rs.id + "?fields=images");
                    imageSize = new ArrayList();
                    foreach (dynamic rsa in photoInfo.images)
                    {
                        imageSize.Add(rsa.height);
                    }
                    int highestImage = Convert.ToInt32(imageSize[0]);
                    foreach (dynamic rsa in photoInfo.images)
                    {
                        //这里有多个尺寸的
                        if (rsa.height == highestImage)
                        {
                            FacebookInfo facebookInfo = new FacebookInfo();
                            facebookInfo.Url = rsa.source;

                            //facebookInfo.Width = rsa.
                            //   facebookInfo.Height = 
                            searchResult?.Invoke(facebookInfo);
                        }
                    }

                }
            }
        }
    }
}
