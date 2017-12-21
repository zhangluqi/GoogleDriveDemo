using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace GoogleDriveDemo.Model.Box
{
    public class OAuth2Response
    {
        internal OAuth2Response(string accessToken, string expiresIn, string restrictedTo, string refreshToken, string tokenType)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new AccessViolationException("error accessToken");
            }
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RestrictedTo = restrictedTo;
            RefreshToken = refreshToken;
            TokenType = tokenType;
        }

        //{"access_token":"LNvnmBBG7RhnYzS6RQVbw4pTonjqP7KL","expires_in":3821,"restricted_to":[],"refresh_token":"DkYqwK2Eun917iXofF5ruEHdsTgkj0a1YhwzfDNQsCWFEPIDgw4wVxaoTV2LcnK2","token_type":"bearer"}
        public string AccessToken { get; [CompilerGenerated] private set; }
        public string ExpiresIn { get; [CompilerGenerated] private set; }
        public string RestrictedTo { get; [CompilerGenerated] private set; }
        public string RefreshToken { get; [CompilerGenerated] private set; }
        public string TokenType { get; [CompilerGenerated] private set; }
    }
}
