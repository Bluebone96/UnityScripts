using UnityEngine;
using System;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Proto.Unity;
public class GameManager : SingletonObject<GameManager>
{
    // Start is called before the first frame update
    public static Queue<Message> g_mQueue;  // 消息队列 socket收到消息后放进去， eventhandler 取出来进行处理
    public ClientSocket m_Net;
    public EventHandler m_eventhandler;


    public Dictionary<UInt32, PlayerStatus> AllPlayers;

    public PlayerStatus mainPlayer;
    public UInt32 mainPlayerID;

    public PlayerAllFuckInfo fuckall;

    public bool pause = true;
    
    // int frame = 1;
    void Awake()
    {
        fuckall = new PlayerAllFuckInfo();
        AllPlayers = new Dictionary<UInt32, PlayerStatus>();
        m_Net = ClientSocket.Instance;
        m_eventhandler = EventHandler.Instance;
        g_mQueue = new Queue<Message>(100);
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


    void FixedUpdate()
    {
        m_Net.RecvMessage();

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
        Application.Quit();
    }

    public void GamePause()
    {
        Time.timeScale = 0;
        pause = true;
    }

    public void GameStart()
    {
        Time.timeScale = 1;
        pause = false;
    }
}
