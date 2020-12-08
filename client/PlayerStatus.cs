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

    void Awake()
    {   
        m_protoInfo = new PlayerInfo();
        m_protoInfo.Op.H = 0;
        m_protoInfo.Op.V = 0;
        m_protoInfo.Speed = 0;
        move = new Vector3();
        move = Vector3.zero;
    }
    public bool CheckStatus(PlayerInfo _info)
    {
        m_protoInfo = _info;
        Vector3 pos = new Vector3(m_protoInfo.PosX, 0, m_protoInfo.PosZ);
        
        if (Mathf.Abs(pos.x - transform.position.x) > 1 
            || Mathf.Abs(pos.y - transform.position.y) > 1
            || Mathf.Abs(pos.z - transform.position.z) > 1)
        {
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition,pos, m_protoInfo.Speed);
        }
        //        m_Queue.Enqueue(new PlayerInfo(m_protoInfo));
        
        return true;
    }

    void Start()
    {
        
    }
    void FixedUpdate()
    {
        move.x = m_protoInfo.Op.H  * m_protoInfo.Speed;

        move.z = m_protoInfo.Op.V * m_protoInfo.Speed;

        // Debug.Log("player fixed update move.x = " + move.x + "move.z = " + move.z);
        transform.Translate(move * Time.fixedDeltaTime, Space.World);
    }

    public void localOpData(Operation _op)
    {
        m_protoInfo.Op = _op;
    }


};