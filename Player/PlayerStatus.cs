using System;
using System.Collections;
using Google.Protobuf;
using Proto.Unity;

using UnityEngine;

// public struct PlayerStatus {
//     public uint m_id {get; set;}
//     public string m_name {get; set;}
//     public float m_hp {get; set;}
//     public float m_mp {get; set;}

//     public float speed {get; set;}

//     public Vector3 m_position {get; set;}
//     public Vector3 m_rotation {get; set;}

//     public byte[] op {get; set; }

// };


public class PlayerStatus : MonoBehaviour {

//    public Queue m_Queue = new Queue(10);

    public PlayerInfo m_protoInfo;
    // PlayerStatus m_status;
    Vector3 move;

    private Animator m_anim;
    private int m_idIDLE;
    private int m_idWALK;


    // void Awake()
    // {   
    //     // awake 在创建之后运行，即在一个函数中用Instantiate 创建处来的实体，在函数中初始化后。结束函数，开始调用awake。重复初始化
    //     // m_protoInfo = new PlayerInfo();

    // }
    public bool CheckStatus(PlayerInfo _info)
    {
        m_protoInfo = _info;
        Vector3 pos = new Vector3(m_protoInfo.PosX, 0, m_protoInfo.PosZ);
        
        if (Mathf.Abs(pos.x - transform.position.x) > 1 
            || Mathf.Abs(pos.y - transform.position.y) > 1
            || Mathf.Abs(pos.z - transform.position.z) > 1)
        {
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, pos, m_protoInfo.Speed * 5);
        }
        //        m_Queue.Enqueue(new PlayerInfo(m_protoInfo));
        
        return true;
    }

    public void init(PlayerInfo myInfo)
    {
        m_protoInfo  = myInfo;
        m_protoInfo.Op = new Operation();
        Debug.Log("player name: " + m_protoInfo.Name + " id: " + m_protoInfo.Id);
        Debug.Log("player posx: " + m_protoInfo.PosX + " poxz: " + m_protoInfo.PosZ);
        Debug.Log("player speed: " + m_protoInfo.Speed);
        move = Vector3.zero;

        m_anim = GetComponent<Animator>();
        
        m_idIDLE = Animator.StringToHash("IDLE");
        m_idWALK = Animator.StringToHash("WALK");
    }


    // void Start()
    // {
    //     Debug.Log("player speed = " + m_protoInfo.Speed);
    // }
    void Update()
    {
        // Debug.Log("player speed " + m_protoInfo.Speed + "op h " + m_protoInfo.Op.H + " v " + m_protoInfo.Op.V);
        move.x = m_protoInfo.Op.H  * m_protoInfo.Speed;

        move.z = m_protoInfo.Op.V * m_protoInfo.Speed;
        if (move.x == 0 && move.z == 0) {
            m_anim.SetTrigger(m_idIDLE);
        } else {
            m_anim.SetTrigger(m_idWALK);
        }
        // Debug.Log("player fixed update move.x = " + move.x + "move.z = " + move.z);
        transform.Translate(move * Time.deltaTime, Space.World);
    }


};