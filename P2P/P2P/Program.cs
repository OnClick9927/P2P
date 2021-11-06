using System.Linq;
using System.Net.NetworkInformation;
using System;
using P2P.Net;

namespace P2P
{
    class Program
    {
        static void Main(string[] args)
        {
            Const.bufferSize = 4096;
            Const.holePunchingTime = 2000;
            Const.severIp = "127.0.0.1";
            Const.severPort = 5004;
            Const.uid = SystemInfo.id;
            P2PClient client = new P2PClient();
            P2PSever sever = new P2PSever();
            while (true)
            {

            }
        }
    }
}
