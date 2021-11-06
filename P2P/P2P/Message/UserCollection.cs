using System;
using System.Collections;
namespace P2P.Message
{
    [Serializable]
    public class UserCollection : CollectionBase
    {
        public void Add(User user)
        {
            Remove(user.id);
            InnerList.Add(user);
        }
        public User Find(string id)
        {
            foreach (User user in this)
            {
                if (string.Compare(id, user.id, true) == 0)
                {
                    return user;
                }
            }
            return null;
        }
        public void Remove(string id)
        {
            var data = Find(id);
            if (data!=null)
            {
                InnerList.Remove(data);
            }

        }
    }
}
