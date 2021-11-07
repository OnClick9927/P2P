using System.Collections.Generic;
using P2P.Packets;
using System.Net;
using P2P.Message;

namespace P2P.Net
{
    public abstract class P2PMessageHandler : IP2PMessageHandler
    {
        private class PacketReaders
        {
            private int bufferSize;
            private Dictionary<IPEndPoint, PacketReader> readers;
            public PacketReaders(int bufferSize)
            {
                this.bufferSize = bufferSize;
                readers = new Dictionary<IPEndPoint, PacketReader>();
            }

            public List<Packet> ReadBuffer(IPEndPoint point, byte[] buffer)
            {
                PacketReader reader;
                if (!readers.TryGetValue(point, out reader))
                {
                    reader = new PacketReader(bufferSize);
                    readers.Add(point, reader);
                }
                reader.Set(buffer, 0, buffer.Length);
                return reader.Get();
            }
            public void Clear()
            {
                readers.Clear();
            }
        }


        UDPEntity IP2PMessageHandler.udp { get; set; }
        public UserCollection users { get; private set; }
        private PacketReaders readers;
        private int bufferSize;
        private int port;

        protected P2PMessageHandler(int bufferSize,int port)
        {
            this.bufferSize = bufferSize;
            this.port = port;
        }
        public virtual void Start()
        {
            (this as IP2PMessageHandler).udp = new UDPEntity(this,port);
            readers = new PacketReaders(bufferSize);
            users = new UserCollection();
            (this as IP2PMessageHandler).udp.Start();
        }
        public virtual void Stop()
        {
            (this as IP2PMessageHandler).udp.Stop();
            users.Clear();
            readers.Clear();
        }
        void IP2PMessageHandler.OnMessage(IPEndPoint point, byte[] buffer)
        {
            var packets = readers.ReadBuffer(point, buffer);
            if (packets == null || packets.Count == 0) return;
            foreach (var item in packets)
            {
                switch ((PacketType)item.pkgType)
                {
                    case PacketType.P2P:
                        object msgObj = ObjectSerializer.Deserialize(item.message);
                        OnP2PMessage(point, msgObj as IMessage);
                        break;
                    case PacketType.Custom:
                        OnCustomMessage(point, item);
                        break;
                    default:
                        break;
                }
            }
        }
        protected abstract void OnP2PMessage(IPEndPoint point, IMessage message);
        protected abstract void OnCustomMessage(IPEndPoint point, Packet packet);


        private void SendMessage(IPEndPoint remoteIP,Packet packet)
        {
            (this as IP2PMessageHandler).udp.SendMessage(remoteIP, packet.Pack());
        }
        public void SendMessage(IPEndPoint remoteIP, ushort pkgCount, uint pkgID, PacketType pkgType, byte[] buffer)
        {
            Packet packet = new Packet(pkgCount, pkgID, (byte)pkgType, buffer);
            SendMessage(remoteIP, packet);
        }
        public void SendMessage(IPEndPoint remoteIP, IMessage message)
        {
            SendMessage(remoteIP, 1, 0, PacketType.P2P, ObjectSerializer.Serialize(message));
        }
        public void SendMessage(User user, ushort pkgCount, uint pkgID, PacketType pkgType, byte[] buffer)
        {
            SendMessage(user.point, pkgCount, pkgID, pkgType, buffer);
        }
        public void SendMessage(User user, IMessage message) {
            SendMessage(user.point, message);
        }
    }
}
