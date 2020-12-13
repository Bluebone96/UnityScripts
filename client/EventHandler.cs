using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using Google.Protobuf;
using Proto.Unity;
using System.Threading;

public class EventHandler : Singleton<EventHandler>
{
    public delegate int Foo(Message msg);

    public Dictionary<EventType, Foo> Handles;


    public PlayerInfo m_info;

    public GameObject player;
    
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

            if (GameManager.g_mQueue.TryDequeue(out m_message)) {
                Debug.Log("Dequeue, type is" + m_message.m_type + " id is " + m_message.m_usrid);
                return Handles[m_message.m_type](m_message);
            } else {
                Thread.Sleep(10);
            }

        }
        return -1;
    }


    int PlayerInit(Message _msg)
    {

        player = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));

        player.tag = "MainPlayer";

        GameObject camera = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));

        camera.transform.SetParent(player.transform);

        
        // PlayerInfo p = player.GetComponent<PlayerStatus>().m_protoInfo;

        Debug.Log("ParserFrom the data");
        
        GameManager.Instance.fuckall = PlayerAllFuckInfo.Parser.ParseFrom(_msg.m_data);

        // Debug.Log(GameManager.Instance.fuckall.Baseinfo);
        
        //player.GetComponent<PlayerStatus>().m_protoInfo = GameManager.Instance.fuckall.Baseinfo;

        player.GetComponent<PlayerStatus>().init(GameManager.Instance.fuckall.Baseinfo);

        // Debug.Log("+++++++++++++++" + player.GetComponent<PlayerStatus>().m_protoInfo.Name);

        // Debug.Log("player speed = " + player.GetComponent<PlayerStatus>().m_protoInfo.Speed);

        Inventory.Instance.Init(GameManager.Instance.fuckall.Baginfo);

        Debug.Log("ParserFrom the data end");

        GameManager.Instance.mainPlayer = player.GetComponent<PlayerStatus>();
        GameManager.Instance.mainPlayerID = _msg.m_usrid;
        
        GameManager.Instance.AllPlayers.Add(_msg.m_usrid, GameManager.Instance.mainPlayer);

        player.transform.position = new Vector3(player.GetComponent<PlayerStatus>().m_protoInfo.PosX, 0, player.GetComponent<PlayerStatus>().m_protoInfo.PosZ);
        player.name = player.GetComponent<PlayerStatus>().m_protoInfo.Name;

        GameManager.Instance.m_Net.setID(_msg.m_usrid);
        
        camera.AddComponent<GetOperation>();

        Debug.Log("player init complete");

        return 0;
    }

    int ConnectGateServer(Message _msg)
    {
        Debug.Log("connect gate server start");
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
        Debug.Log("connect gate server end");
        return 0;
    }

    int PlayerExit(Message _msg)
    {
        Debug.Log("======================================");
        Debug.Log("player exit id = " + _msg.m_usrid);

        UnityEngine.Object.Destroy(GameManager.Instance.AllPlayers[_msg.m_usrid].gameObject);
        GameManager.Instance.AllPlayers.Remove(_msg.m_usrid);

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
        if (id  == GameManager.Instance.mainPlayerID)
        {
            // PlayerInfo info = PlayerInfo.Parser.ParseFrom(_msg.m_data);
            // info.Op = GameManager.Instance.mainPlayer.m_protoInfo.Op;
            // GameManager.Instance.mainPlayer.CheckStatus(info);
        } 
        else if (GameManager.Instance.AllPlayers.ContainsKey(id)) 
        {
            PlayerStatus p = GameManager.Instance.AllPlayers[id];

            GameManager.Instance.AllPlayers[id].CheckStatus(PlayerInfo.Parser.ParseFrom(_msg.m_data));
        } 
        else 
        {
            GameObject p = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
            
            PlayerInfo info = PlayerInfo.Parser.ParseFrom(_msg.m_data);

            p.GetComponent<PlayerStatus>().init(info);

            p.name = info.Name;
            p.transform.position = new Vector3(info.PosX, 0, info.PosZ);
            GameManager.Instance.AllPlayers.Add(id, p.GetComponent<PlayerStatus>());
        }

        return 4;
    }


};

