using Azi.Amazon.CloudDrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Amazon
{
    public class AuthAmazon
    {
        public async void test()
        {
            string clientId = "amzn1.application-oa2-client.d5581ef181e7426a92dcbd1ccbb5e89c";
            string secret = "8180ad1b1e6688f5247a94f4d7a58d4c08d1abd6df3dd4cd576990725281d91b";
            var amazon = new AmazonDrive(clientId, secret);
            var result = await amazon.AuthenticationByExternalBrowser(CloudDriveScopes.ReadAll | CloudDriveScopes.Write, TimeSpan.FromMinutes(1)).ConfigureAwait(false);
          

        }
    }
}
