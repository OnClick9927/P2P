using System;
using System.Net;

namespace P2P.Message
{
    [Serializable]
    public class S2C_HolePunchingCommand : IS2CMessage
    {
        public IPEndPoint point;
    }
}
