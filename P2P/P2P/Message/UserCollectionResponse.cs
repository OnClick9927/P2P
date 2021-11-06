using System;

namespace P2P.Message
{
    [Serializable]
    public class UserCollectionResponse : IS2CMessage
    {
        public UserCollection collection;
    }
}
