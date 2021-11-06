using System.Net;
namespace P2P
{
    public class Const
    {
        public static int bufferSize { get; set; } = 4096;
        public static int severPort { get; set; }


        //下面，client专用
        public static string severIp { get; set; }
        public static int holePunchingTime { get; set; } = 2000;
        public static string uid { get; set; }

        private static IPEndPoint _host;
        public static IPEndPoint host
        {
            get
            {
                if (_host == null)
                {
                    _host = new IPEndPoint(IPAddress.Parse(severIp), severPort);
                }
                return _host;
            }
        }
    }
}
