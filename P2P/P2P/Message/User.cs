using System;
using System.Net;
namespace P2P.Message
{
    [Serializable]
    public class User
    {
        public string id;
        public IPEndPoint point;
    }
}
