using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.VersionControl;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Protocol;

public class NetworkManager :MonoBehaviour
{
    ServerSession m_refSession = new ServerSession();

    public static NetworkManager m_Instance = null;
    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);
    }

    public void Send(IMessage _Ipacket)
    {
        string strMsgName = _Ipacket.Descriptor.Name.Replace("_", string.Empty);
        MsgId eID = (MsgId)Enum.Parse(typeof(MsgId), strMsgName);

        ushort sSize = (ushort)_Ipacket.CalculateSize();
        byte[] arrSendBuffer = new byte[sSize + 4]; //패킷 사이즈, 패킷 아이디
        Array.Copy(BitConverter.GetBytes((sSize + 4)), 0, arrSendBuffer, 0, sizeof(ushort));

        ushort protocolId = (ushort)eID;
        Array.Copy(BitConverter.GetBytes(protocolId), 0, arrSendBuffer, 2, sizeof(ushort));
        Array.Copy(_Ipacket.ToByteArray(), 0, arrSendBuffer, 4, sSize);

        m_refSession.Send(new ArraySegment<byte>(arrSendBuffer));
    }


    public void StartNet()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return m_refSession; },
            1);
    }

    private void Update()
    {
        List<PacketMessage> list = PacketQueue.m_Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(m_refSession, packet.Message);
        }
    }


    private void OnApplicationQuit()
    {
        m_refSession.DisConnect();
    }

}
