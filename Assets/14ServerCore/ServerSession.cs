using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DummyClient
{


    public class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint _refEndPoint)
        {
            Debug.Log($"Connected");
        }

        public override void OnDisConnected(EndPoint _refEndPoint)
        {
            
        }

        public override void OnRecvPacket(ArraySegment<byte> _arrBuffer)
        {
            PacketManager.Instance.OnRecvPacket(this, _arrBuffer);
        }


        public override void OnSend(int _iBytes)
        {
        }
    }
}
