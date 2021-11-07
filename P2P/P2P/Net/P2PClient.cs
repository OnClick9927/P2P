using System.Threading.Tasks;
using P2P.Packets;
using System.Net;
using P2P.Message;

namespace P2P.Net
{
    public class P2PClient : P2PMessageHandler
    {
        private readonly string uid;
        private readonly IPEndPoint host;
        private readonly int holePunchingTime;

        public P2PClient(string uid, string severIP,int severPort, int bufferSize = 4096, int holePunchingTime = 2000) : base(bufferSize, 0)
        {
            this.uid = uid;
            this.host = new IPEndPoint(IPAddress.Parse(severIP), severPort);
            this.holePunchingTime = holePunchingTime;
        }

        public void SendMessageToHost(IMessage message)
        {
            SendMessage(host, message);
        }
        public void SendMessageToHost(ushort pkgCount, uint pkgID, PacketType pkgType, byte[] buffer)
        {
            SendMessage(host, pkgCount, pkgID, pkgType, buffer);
        }

        protected void SendUIDMessage(IPEndPoint point, UIDMessage message)
        {
            message.id = uid;
            SendMessage(point,message);
        }
        protected void SendUIDMessageToHost(UIDMessage message)
        {
            SendUIDMessage(host,message);
        }
        protected void SendUIDMessage(User user, UIDMessage message)
        {
            SendUIDMessage(user.point, message);
        }

        protected override void OnP2PMessage(IPEndPoint point, IMessage message)
        {
            if (message is UserCollectionResponse)
            {
                OnUserCollection(message as UserCollectionResponse);
            }
            else if (message is HolePunchingCommand)
            {
                OnUserHolePunchingCommand(message as HolePunchingCommand);
            }
            else if (message is NatFakeMessage)
            {
                OnP2PHolePunchingTest(point, message as NatFakeMessage);
            }
            else if (message is HolePunchingSucess)
            {
                OnP2PHolePunchingSucess(point, message as HolePunchingSucess);
            }
        }

        private void OnUserCollection(UserCollectionResponse message)
        {
            users.Clear();
            foreach (User item in message.collection)
                users.Add(item);
            OnUserCollectionChanged();
        }
        protected virtual void OnUserHolePunchingCommand(HolePunchingCommand message)
        {
            NatFakeMessage msgTest = new NatFakeMessage();
            this.SendUIDMessage(message.point, msgTest);
        }
        private void OnP2PHolePunchingTest(IPEndPoint point, NatFakeMessage message)
        {
            UpdateConnection(message.id, point);
            HolePunchingSucess response = new HolePunchingSucess();
            this.SendUIDMessage(point, response);
        }
        protected virtual void OnP2PHolePunchingSucess(IPEndPoint point, HolePunchingSucess message)
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
            this.SendUIDMessageToHost(lgoutMsg);
        }

        private void Login()
        {
            LoginRequest lgoutMsg = new LoginRequest();
            this.SendUIDMessageToHost(lgoutMsg);
        }


        public async void HolePunching(User user)
        {
            HolePunchingRequest msg = new HolePunchingRequest()
            {
                targetid = user.id
            };
            this.SendUIDMessageToHost(msg);
            await Task.Delay(holePunchingTime);
            NatFakeMessage confirmMessage = new NatFakeMessage();
            this.SendUIDMessage(user, confirmMessage);
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
