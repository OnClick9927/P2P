
using System;

namespace P2P.Packets
{
    class PacketHeader
    {
        public UInt32 pkgID { get; set; }
        public byte pkgType { get; set; }
        public UInt16 pkgCount { get; set; } /*= 1;*/
        public UInt32 messageLen { get; internal set; }
    }
}
