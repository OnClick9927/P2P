using System.Net;

namespace P2P.Net
{
    public interface IP2PMessageHandler
    {
        UDPEntity udp { get; set; }
        void OnMessage(IPEndPoint point, byte[] buffer);
    }
}
