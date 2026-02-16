
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
using UnityEditor.Profiling.Memory.Experimental;

public class PacketHandler
{
    //해당 스레드는 메인 스레드가 아닌 네트워크 스레드에서 처리하는 영역이기 때문에
    //유니티에서 유니티 Gameobject를 건드리면 안됨

    public static void S_EnterGameHandler(PacketSession _refSession,  IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_EnterGame pkt = _iPacket as S_EnterGame;
    }

    public static void S_LeaveGameHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_LeaveGame pkt = _iPacket as S_LeaveGame;
    }

    public static void S_SpawnHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_Spawn pkt = _iPacket as S_Spawn;
    }


    public static void S_DespawnHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_Despawn pkt = _iPacket as S_Despawn;
    }


    public static void S_MoveHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_Move pkt = _iPacket as S_Move;
    }

    public static void S_OtherMoveHandler(PacketSession _refSession, IMessage _iPacket)
    {
        ServerSession refServerSession = _refSession as ServerSession;
        S_Other_Move pkt = _iPacket as S_Other_Move;

        if(pkt.Success == false)
        {
            Transform tr = GameManager.m_Instance.Player.GetComponent<Transform>();
            PositionInfo p = pkt.PosInfo;
            tr.position = new Vector3(p.PosX, p.PosY, p.PosZ);
        }
    }



}

