
using System;
using System.Collections.Generic;
using System.Threading;

namespace P2P.Packets
{

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class PacketReader
    {
        private PacketQueue packetQueue;
        private LockParam lockParam;
        public int count { get { return packetQueue.count; } }
        public PacketReader(int capacity = 128)
        {
            if (capacity < 128) capacity = 128;
            capacity += 1;
            packetQueue = new PacketQueue(capacity);
            lockParam = new LockParam();
        }
        public bool Set(byte[] buff, int offset, int size)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return packetQueue.Set(buff, offset, size);
            }
        }
        public List<Packet> Get()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return packetQueue.Get();
            }
        }
        public void Clear()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                packetQueue.Clear();
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
