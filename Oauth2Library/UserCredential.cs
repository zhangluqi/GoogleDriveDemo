using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oauth2Library
{
    public class UserCredential
    {
        private string _accessToken;

        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        private string _reshToken;

        public string ReshToken
        {
            get { return _reshToken; }
            set { _reshToken = value; }
        }


        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _tokenType;

        public string TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }



    }
}
