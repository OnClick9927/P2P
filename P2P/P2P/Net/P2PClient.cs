using System.Threading.Tasks;
using P2P.Packets;
using System.Net;
using P2P.Message;

namespace P2P.Net
{
    public class P2PClient : P2PMessageHandler
    {
        private string id { get { return Const.uid; } }


        public void SendMessageToHost(IMessage message)
        {
            SendMessage(Const.host, message);
        }
        public void SendMessageToHost(ushort pkgCount, uint pkgID, PacketType pkgType, byte[] buffer)
        {
            SendMessage(Const.host, pkgCount, pkgID, pkgType, buffer);
        }

        protected void SendC2SMessageToHost(C2SMessage message)
        {
            message.id = id;
            SendMessageToHost(message);
        }

        protected override UDPEntity CreateUdp()
        {
            return new UDPEntity(this, 0);
        }
        protected override void OnP2PMessage(IPEndPoint point, IMessage message)
        {
            if (message is UserCollectionResponse)
            {
                OnUserCollection(message as UserCollectionResponse);
            }
            else if (message is S2C_HolePunchingCommand)
            {
                OnUserHolePunchingCommand(message as S2C_HolePunchingCommand);
            }
            else if (message is P2PHolePunchingTest)
            {
                OnP2PHolePunchingTest(point,message as P2PHolePunchingTest);
            }
            else if (message is P2PHolePunchingSucess)
            {
                OnP2PHolePunchingSucess(point,message as P2PHolePunchingSucess);
            }
        }

        private void OnUserCollection(UserCollectionResponse message)
        {
            users.Clear();
            foreach (User item in message.collection)
                users.Add(item);
            OnUserCollectionChanged();
        }
        private void OnUserHolePunchingCommand(S2C_HolePunchingCommand message)
        {
            P2PHolePunchingTest msgTest = new P2PHolePunchingTest() { 
               id=id
            };
            this.SendMessage(message.point, msgTest);
        }
        private void OnP2PHolePunchingTest(IPEndPoint point,P2PHolePunchingTest message)
        {
            UpdateConnection(message.id, point);
            P2PHolePunchingSucess response = new P2PHolePunchingSucess() { 
                id=id
            };
            this.SendMessage(point,response);
        }
        protected virtual void OnP2PHolePunchingSucess(IPEndPoint point, P2PHolePunchingSucess message)
        {
            UpdateConnection(message.id, point);
        }
        private void UpdateConnection(string user, IPEndPoint ep)
        {
            User remoteUser = users.Find(user);
            if (remoteUser != null)
            {
                remoteUser.point = ep;
            }
            OnUserCollectionChanged();
        }
        private void Logout()
        {
            LogoutRequest lgoutMsg = new LogoutRequest();
            this.SendC2SMessageToHost(lgoutMsg);
        }

        private void Login()
        {
            LoginRequest lgoutMsg = new LoginRequest();
            this.SendC2SMessageToHost(lgoutMsg);
        }


        public async void HolePunching(User user)
        {
            HolePunchingRequest msg = new HolePunchingRequest()
            {
                targetid = user.id
            };
            this.SendC2SMessageToHost(msg);
            await Task.Delay(Const.holePunchingTime);
            P2PHolePunchingTest confirmMessage = new P2PHolePunchingTest()
            {
                id = id
            };
            this.SendMessage(user, confirmMessage);
        }
        public override void Start()
        {
            base.Start();
            Login();
        }
        public override void Stop()
        {
            Logout();
            base.Stop();
        }
        protected virtual void OnUserCollectionChanged() { }
        protected override void OnCustomMessage(IPEndPoint point, Packet packet) { }

    }
}
