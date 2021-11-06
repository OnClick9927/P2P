using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;

namespace P2P.Net
{
    public class UDPEntity
    {
        private UdpClient _udp;
        private readonly IP2PMessageHandler handler;

        public UDPEntity(IP2PMessageHandler handler,int port)
        {
            _udp = new UdpClient(port);
            handler.udp = this;
            this.handler = handler;
        }
        private async void Recieve()
        {
            try
            {
                if (_udp!=null)
                {
                    UdpReceiveResult result = await _udp.ReceiveAsync();
                    var endpoint = result.RemoteEndPoint;
                    var buffer = result.Buffer;
                    if (handler!=null && buffer!=null && buffer.Length!=0)
                    {
                        handler.OnMessage(endpoint, buffer);
                    }
                }
            }
   
            catch (Exception)
            {

            }
            finally
            {
                if (_udp != null)
                {
                    Recieve();
                }
            }

        }
        public void Stop()
        {
            try
            {
                _udp.Close();
                _udp = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Start()
        {
            Recieve();
        }

        public async Task<int> SendMessage(IPEndPoint remoteIP,byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return 0;
           return await _udp.SendAsync(buffer, buffer.Length, remoteIP);
        }
    }
}
