using UnityEngine;
using System;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

using Google.Protobuf;
using Proto.Unity;
public class GameManager : SingletonObject<GameManager>
{
    // Start is called before the first frame update
    public static ConcurrentQueue<Message> g_mQueue;  // 消息队列 socket收到消息后放进去， eventhandler 取出来进行处理
    public ClientSocket m_Net;
    public EventHandler m_eventhandler;


    public Dictionary<UInt32, PlayerStatus> AllPlayers;

    public PlayerStatus mainPlayer;
    public UInt32 mainPlayerID;

    public PlayerAllFuckInfo fuckall;

    // public bool pause = true;
    
    // int frame = 1;
    void Awake()
    {
        fuckall = new PlayerAllFuckInfo();
        AllPlayers = new Dictionary<UInt32, PlayerStatus>();
        m_Net = ClientSocket.Instance;
        m_eventhandler = EventHandler.Instance;
        g_mQueue = new ConcurrentQueue<Message>();
    }



    void Start()
    {
        if (m_Net.ConnectServer("192.168.80.3", 2222) < 0) {
            Debug.Log("Connect Server failed!");
            // TODO 不进入 update
        } else {
            Debug.Log("connect server sucess!");
        }
        GamePause();
    }


    void Update()
    {
        m_eventhandler.DoSomething();
    }



    public int LoginVerify(string name, string password)
    {

        Debug.Log("try login");
        Authentication authentication = new Authentication();
        authentication.Name = name;
        authentication.Password = password;
        Debug.Log("name: " + authentication.Name + " password: " + authentication.Password);

        if (m_Net.SendMessage(authentication, EventType.LOGIN_REQUEST) < 0) {
            Debug.Log("send Authentication message failed");
            return -1;
        }
        GameStart();
        return 0;
    } 



    public void ExitGame()
    {
        MsgHead msg = new MsgHead();
        msg.m_type = (uint)EventType.EXIT;
        msg.m_len = 0;
        msg.m_usrid = mainPlayerID;
        msg.m_errID = 0;
        byte[] temp = new byte[MsgHead.headsize()];
        msg.EnCode(temp, 0);
        m_Net.Send(temp, MsgHead.headsize(), 0);
        m_Net.DisConnect();
        Invoke(nameof(Application.Quit), 1f);
    }

    public void GamePause()
    {
        Time.timeScale = 0;
        // pause = true;
    }

    public void GameStart()
    {
        Time.timeScale = 1;
        // pause = false;
    }
}
