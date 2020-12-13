using System;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Proto.Unity;
using UnityEngine;



public struct Message {
    public EventType m_type {get; set;}
    public UInt32 m_usrid { get; set;}
    public UInt32 m_datalen {get; set;}
    public byte[] m_data { get; set;}
}

public enum EventType {
    LOGIN = 0,
    EXIT = 1,
    ALIVE = 2,
    UPDATE = 3,
    SYNC = 4,
    CHAT = 5,
    ITEM  = 6,

    // ITEMADD = 7,
    // ITEMDEL =8,

    // EQUIP =9,
    // UNEQUIP =10,
    

    LOGIN_REQUEST = 7,
    LOGIN_SUCCESS = 8,
    LOGIN_FAILED = 9,

    ALLINFO = 16,
    GATESERVER = 20
};

public class MsgHead {

    /*
     * tag : value
     *  0  :  登录
     *  1  :  离线
     *  2  :  心跳包
     *  3  :  操作
     *  4  :  状态 同步
     *  5  :  聊天
     *  x  :  登录 验证有效， 此时 m_len 为 客户端在服务端的序号。
     */
    // public uint m_tag {set; get;} 

    public UInt32 m_type {get; set;}
    public UInt32 m_len {set; get;}
    public UInt32 m_usrid {get; set; }

    public UInt32 m_errID {get; set;}
    public static UInt32 headsize() { return 16; }

    public MsgHead()
    {
        m_type = 0;
        m_len = 0;
        m_errID = 0;
        m_usrid = 0; 
    }
    
    public int EnCode(byte[] _arr, int _offset)
    {
        byte[] tmp = BitConverter.GetBytes(Endian.hton_32(m_type));
        tmp.CopyTo(_arr, _offset);

        tmp = BitConverter.GetBytes(Endian.hton_32(m_len));
        tmp.CopyTo(_arr, _offset + 4);

        tmp = BitConverter.GetBytes(Endian.hton_32(m_usrid));
        tmp.CopyTo(_arr, _offset + 8);
        
        tmp = BitConverter.GetBytes(Endian.hton_32(m_errID));
        tmp.CopyTo(_arr, _offset + 12);
        return 0;
    }
    public int DeCode(byte[] _arr, int _offset)
    {
        m_type = Endian.ntoh_32((UInt32)BitConverter.ToInt32(_arr, _offset));
        m_len = Endian.ntoh_32((UInt32)BitConverter.ToInt32(_arr, _offset + 4));
        m_usrid = Endian.ntoh_32((UInt32)BitConverter.ToInt32(_arr, _offset + 8));
        m_errID= Endian.ntoh_32((UInt32)BitConverter.ToInt32(_arr, _offset + 12));
        return 0;
    }

    public void setType(EventType _t) { m_type = (UInt32)_t; }
    public void setId(UInt32 _id) { m_usrid = _id; }

    public UInt32 getId() { return m_usrid; }
    public EventType getType() { return (EventType) m_type; }

};


public class Endian {
    static bool is_little()
    {
        int i = 1;
        byte[] endian = BitConverter.GetBytes(i);
        return endian[0] == 1;
    }

    public static UInt32 hton_32 (UInt32 _x)
    {
        if (is_little()) {
            return ((_x & 0xff000000u) >> 24) | ((_x & 0x00ff0000u) >> 8) 
                | ((_x & 0x0000ff00u) << 8) | ((_x & 0x000000ffu) << 24);
        }
        return _x;
    }

    public static  UInt32 ntoh_32(UInt32 _x)
    {
        return hton_32(_x);
    }

};

// public enum MsgFlag {
//     INVALIED = -1,

//     INACTIVE = 0,
//     ACTIVE = 1
// };

// public class Message {
//     public MsgFlag m_flag;

//     public MsgHead m_head;
//     public byte[] m_data;
//     public void Decode()
//     {
//         m_head.DeCode(m_data, 0);
//     }

//     public void Encode()
//     {
//         m_head.EnCode(m_data, 0);
//     }
// };
