
using UnityEngine;
using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using Google.Protobuf;

public class PacketHandler
{
    //해당 스레드는 메인 스레드가 아닌 네트워크 스레드에서 처리하는 영역이기 때문에
    //유니티에서 유니티 Gameobject를 건드리면 안됨

    public static void S_ChatHandler(PacketSession _refSession,  IMessage _iPacket)
    {
        
    }
}

