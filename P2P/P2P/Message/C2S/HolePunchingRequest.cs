using System;

namespace P2P.Message
{
    [Serializable]
    public class HolePunchingRequest : C2SMessage
    {
        public string targetid;
    }
}
