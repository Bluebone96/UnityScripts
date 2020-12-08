using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using Google.Protobuf;
using Proto.Unity;
public class EventHandler : Singleton<EventHandler>
{
    public delegate int Foo(Message msg);

    public Dictionary<EventType, Foo> Handles;


    public PlayerInfo m_info;
    
    public Message m_message;
    public ClientSocket m_Net;
    public EventHandler()
    {
        Handles = new Dictionary<EventType, Foo>();
        m_Net = ClientSocket.Instance;
        Handles.Add(EventType.ALLINFO,new Foo(this.PlayerInit));

        Handles.Add(EventType.EXIT, new Foo(this.PlayerExit));
        
        Handles.Add(EventType.ALIVE, new Foo(this.PlayerAlive));
        
        Handles.Add(EventType.UPDATE, new Foo(this.PlayerUpdate));

        Handles.Add(EventType.SYNC, new Foo(this.PlayerSync));

        Handles.Add(EventType.GATESERVER, new Foo(this.ConnectGateServer));
    }


    public void  AddHandle(EventType _type, Foo _fun)
    {
        Handles.Add(_type, _fun);
    }
    public int DoSomething()
    {

        if (GameManager.g_mQueue.Count != 0) {

            m_message = GameManager.g_mQueue.Dequeue();
            Debug.Log("Dequeue, type is" + m_message.m_type + " id is " + m_message.m_usrid);
            return Handles[m_message.m_type](m_message);
        }
        return -1;
    }


    int PlayerInit(Message _msg)
    {

        GameObject player = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));

        player.tag = "MainPlayer";

        GameObject camera = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));

        camera.transform.SetParent(player.transform);

        PlayerInfo p = player.GetComponent<PlayerStatus>().m_protoInfo;

        Debug.Log("ParserFrom the data");
        
        GameManager.Instance.fuckall = PlayerAllFuckInfo.Parser.ParseFrom(_msg.m_data);
        
        p = GameManager.Instance.fuckall.Baseinfo;

        Inventory.Instance.Init(GameManager.Instance.fuckall.Baginfo);

        Debug.Log("ParserFrom the data end");

        GameManager.Instance.mainPlayer = player.GetComponent<PlayerStatus>();

        player.transform.position = new Vector3(p.PosX, 0, p.PosZ);

        player.name = p.Name;

        GameManager.Instance.AllPlayers.Add(_msg.m_usrid, player.GetComponent<PlayerStatus>());

        GameManager.Instance.m_Net.setID(_msg.m_usrid);
        
        camera.AddComponent<GetOperation>();

        Debug.Log("player init complete");

        return 0;
    }

    int ConnectGateServer(Message _msg)
    {
        GameManager.Instance.GamePause();
        m_Net.DisConnect();
        ServerInfo gateserver = ServerInfo.Parser.ParseFrom(_msg.m_data);
        if (m_Net.ConnectServer(gateserver.MIp, gateserver.MPort) < 0) {
            return -1;
        }

        MsgHead msg_head = new MsgHead
        {
            m_usrid = _msg.m_usrid
        };

        byte[] data = new byte[16];

        msg_head.EnCode(data, 0);

        m_Net.Send(data, 16);

        UIManager.Instance.ClosePanel("LoginPanel");
        UIManager.Instance.ShowPanel("PanelMain");
        GameManager.Instance.GameStart();
        return 0;
    }

    int PlayerExit(Message _msg)
    {
        GameManager.Instance.ExitGame();
        return 1;
    }

    int PlayerAlive(Message _msg)
    {
        m_Net.Send(_msg.m_data, _msg.m_datalen);
        return 2;
    }
    int PlayerUpdate(Message _msg)
    {
        m_Net.Send(_msg.m_data, _msg.m_datalen);
        return 3;
    }

    int PlayerSync(Message _msg)
    {
        Debug.Log("sync player player id is " + _msg.m_usrid);
        
        UInt32 id = _msg.m_usrid;
        
        if (GameManager.Instance.AllPlayers.ContainsKey(id)) {

            PlayerStatus p = GameManager.Instance.AllPlayers[id];
            GameManager.Instance.AllPlayers[id].CheckStatus(PlayerInfo.Parser.ParseFrom(_msg.m_data));
        } else {
            GameObject p = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
            
            PlayerInfo info = p.GetComponent<PlayerStatus>().m_protoInfo;

            info = PlayerInfo.Parser.ParseFrom(_msg.m_data);
            p.name = info.Name;
            p.transform.position = new Vector3(info.PosX, 0, info.PosZ);
            GameManager.Instance.AllPlayers.Add(id, p.GetComponent<PlayerStatus>());
        }

        return 4;
    }


};

