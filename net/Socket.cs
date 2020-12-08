using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Google.Protobuf;
using Proto.Unity;
public class ClientSocket : Singleton<ClientSocket> 
{

    public Socket m_socket;
    private byte[] m_recvbuf = new byte[4096];
    // 左闭右开
    private MsgHead m_msgHeadRecv = new MsgHead();

    private byte[] m_sendbuf = new byte[100];   // 目前只有玩家上传更新操作会用到
    private MsgHead m_msgHeadSend = new MsgHead();

    private Message m_message; 
    private int m_maxlen = 4096;
    private int m_head = 0;
    private int m_tail = 0;

    public SocketError m_errorCode = 0;

    public  int ConnectServer(string server, int port)
    {   
        Debug.Log("connect socket" + server + port);
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1);
        m_socket.Connect(server, port);
        
        if (m_socket != null) {
            m_socket.Blocking = true;
            return 0;
        }
        return -1;
    }

    public void DisConnect()
    {
        m_socket.Close();
    }

    public void setID(UInt32 _id) 
    {
        m_msgHeadSend.setId(_id);
    }
    
    public int Send(byte[] _data, UInt32 _len, int _offset = 0)
    {

        int left = (int)_len;
        int n = 0;
        int off = _offset;
        
        Debug.Log("send bytes : " + n + " left byte: " + left + " offset :" + off);

        while (left > 0) {
            if ((n =m_socket.Send(_data, off, left, SocketFlags.None, out m_errorCode)) < 0) {
                if (m_errorCode == SocketError.WouldBlock 
                    || m_errorCode == SocketError.Interrupted
                    || m_errorCode == SocketError.AlreadyInProgress) {
                    continue;
                } else {
                    Debug.Log("send data failed");
                    return -1; // break, 返回未发送字节？
                }
            }

            left -= n;
            off += n;
            Debug.Log("send bytes : " + n + " left byte: " + left + "offset :" + off);
        }

        return left; // 0
    }


    public int RecvMessage()
    {
    //     int left = _len;
    //     int n = 0;

    //     int off = _offset;
    //     Debug.Log("recv bytes : " + _len + "offset :" + off + "data length is" + _data.Length);

    //     while (left > 0) {
    //         if ((n = m_socket.Receive(_data, off, left, SocketFlags.None, out m_errorCode)) < 0) {
    //             if (m_errorCode == SocketError.WouldBlock 
    //                 || m_errorCode == SocketError.Interrupted
    //                 || m_errorCode == SocketError.AlreadyInProgress) {
    //                 continue;
    //             } else {
    //                 Debug.Log("send data failed");
    //                 return -1; 
    //             }
    //         }
    //         left -= n;
    //         off += n;
    //     }

    //     Debug.Log( " recv " + _len +  " data sucess");

        
        if (MsgHead.headsize() > (m_tail - m_head))
        {
            recv();
            return -1;
        }
        else 
        {
            m_msgHeadRecv.DeCode(m_recvbuf, m_head);
            if (m_msgHeadRecv.m_len > (m_tail - m_head - MsgHead.headsize())) {
                recv();
                Debug.Log("buffsize is " + (m_tail - m_head) + " less len " + m_msgHeadRecv.m_len);
                return -1;
            } else {
                Debug.Log("buff first 4 bytes is " + m_recvbuf[m_head] + m_recvbuf[m_head + 1] + m_recvbuf[m_head + 2] + m_recvbuf[m_head +3]);
                m_head += (int)MsgHead.headsize();
                m_message.m_type = m_msgHeadRecv.getType();
                m_message.m_usrid = m_msgHeadRecv.getId();
                m_message.m_datalen = m_msgHeadRecv.m_len;
                m_message.m_data = new byte [m_msgHeadRecv.m_len];
                Array.Copy(m_recvbuf, m_head, m_message.m_data, 0, m_msgHeadRecv.m_len);
                m_head += (int) m_msgHeadRecv.m_len;
                Debug.Log("enqueue, type is" + m_message.m_type + "id is " + m_message.m_usrid);
                GameManager.g_mQueue.Enqueue(m_message);
                
                return 1;
            }
        }

    }


    
    private int recv() {
        if (m_tail >= m_maxlen - 100) {
            if (m_head <= 100) {
                // recvbuf is full
            } else {
                // TODO 是否支持内存重叠？
                Array.Copy(m_recvbuf, m_head, m_recvbuf, 0, m_tail - m_head);
                m_tail -= m_head;
                m_head = 0;
            }
        }

        int left = m_maxlen - m_tail;
        int n;
        
        while (left > 0) {
            if ((n = m_socket.Receive(m_recvbuf, m_tail, left, SocketFlags.None, out m_errorCode)) <= 0) {
                // if (m_errorCode == SocketError.WouldBlock 
                //     || m_errorCode == SocketError.Interrupted
                //     || m_errorCode == SocketError.AlreadyInProgress) {
                //         break;
                // } else {
                //     return -1;
                // }
                return -1;
            }

            left -= n;
            m_tail += n;
        }

        return 0;
    }

    public int SendMessage<T>(T _protobuffer, EventType mode) where T : global::Google.Protobuf.IMessage<T>, new()
    {
        Debug.Log("sendmessage type " + mode);
        
        m_msgHeadSend.setType(mode);

        byte[] tmp = Google.Protobuf.MessageExtensions.ToByteArray(_protobuffer);

        m_msgHeadSend.m_len = (UInt32)tmp.Length;

        Debug.Log("protobuf size is " + _protobuffer.CalculateSize());

        Debug.Log("head.type is" + m_msgHeadSend.m_type + "head.id is" + m_msgHeadSend.m_usrid + "protobuf size is " + m_msgHeadSend.m_len);
        
        m_msgHeadSend.EnCode(m_sendbuf, 0);

        tmp.CopyTo(m_sendbuf, MsgHead.headsize());
        if (Send(m_sendbuf, m_msgHeadSend.m_len + MsgHead.headsize()) < 0) {
            return -1;
        }

        return 0;
    }

    // public int SendMessage(Op)



};


