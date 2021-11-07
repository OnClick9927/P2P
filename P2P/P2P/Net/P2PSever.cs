using P2P.Packets;
using System.Net;
using P2P.Message;

namespace P2P.Net
{
    public class P2PSever : P2PMessageHandler
    {
        public P2PSever(int port,int bufferSize) : base(bufferSize, port)
        {
        }


        protected override void OnP2PMessage(IPEndPoint point, IMessage message)
        {
            if (message is LoginRequest)
            {
                OnLogin(point,message as LoginRequest);
            }
            else if (message is LogoutRequest)
            {
                OnLogout(message as LogoutRequest);

            }
            else if (message is HolePunchingRequest)
            {
                OnHolePunchingRequest(point,message as HolePunchingRequest);

            }
        }

        private void OnHolePunchingRequest(IPEndPoint point, HolePunchingRequest message)
        {
            User userA = users.Find(message.id);
            User userB = users.Find(message.targetid);
            HolePunchingCommand msgHolePunching = new HolePunchingCommand() { 
            
                point= point
            };
            this.SendMessage(userB.point,msgHolePunching); 
        }
        private void OnLogout(LogoutRequest message)
        {
            users.Remove(message.id);
            OnUserCollectionChanged();
        }
        private void OnLogin(IPEndPoint point, LoginRequest message)
        {
            IPEndPoint userEndPoint = new IPEndPoint(point.Address, point.Port);
            User user = new User()
            {
                id = message.id,
                point = userEndPoint
            };
            users.Add(user);
            OnUserCollectionChanged();
        }
        protected override void OnCustomMessage(IPEndPoint point, Packet packet) { }
        protected virtual void OnUserCollectionChanged() {
            UserCollectionResponse response = new UserCollectionResponse()
            {
                collection = this.users
            };
            foreach (User u in users)
            {
                this.SendMessage(u.point, response);
            }
        }

    }
}
