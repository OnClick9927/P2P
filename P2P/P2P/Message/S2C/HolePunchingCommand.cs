using System;
using System.Net;

namespace P2P.Message
{
    [Serializable]
    public class HolePunchingCommand : IS2CMessage
    {
        public IPEndPoint point;
    }
}
