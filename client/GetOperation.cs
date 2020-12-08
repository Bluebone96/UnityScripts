using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Proto.Unity;

public class GetOperation : MonoBehaviour
{
    public Operation m_opNew;
    public Operation m_opOld;
    
    public MsgHead  msg;

    public byte[] buf;

    void Awake()
    {

    }

    void Start()
    {
        buf = new byte[50];
        msg = new MsgHead();
        m_opNew = new Operation();
        m_opOld = new Operation();

        msg.setId(GameManager.Instance.mainPlayer.m_protoInfo.Id);
        msg.setType(EventType.UPDATE);
        m_opNew.H = 0;
        m_opNew.V = 0;
        m_opOld.H = 0;
        m_opOld.V = 0;

    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) {
            m_opNew.V = 1;
        } else if (Input.GetKey(KeyCode.S)) {
            m_opNew.V = -1;
        } else {
            m_opNew.V = 0;
        }
    
        
        if (Input.GetKey(KeyCode.D)) {
            m_opNew.H = 1;
        } else if (Input.GetKey(KeyCode.A)) {
            m_opNew.H = -1;
        } else {
            m_opNew.H = 0;
        }
    
        if (m_opNew.H != m_opOld.H || m_opNew.V != m_opOld.V) {
            m_opOld.H = m_opNew.H;
            m_opOld.V = m_opNew.V;

            Debug.Log("H:" + m_opNew.H + " V: " + m_opNew.V);
            Debug.Log("player op H: " + GameManager.Instance.mainPlayer.m_protoInfo.Op.H + " Old H: " + m_opOld.H);
            Debug.Log("player op V: " + GameManager.Instance.mainPlayer.m_protoInfo.Op.V + " Old V: " + m_opOld.V);
            GameManager.Instance.m_Net.SendMessage(m_opNew, EventType.UPDATE);
        }
    }


    void OperationUpdate()
    {
        
        // byte[] tmp =  m_opNew.ToByteArray();

        // tmp.CopyTo(buf, MsgHead.headsize());

        // GameManager.Instance.m_Net.Send()

    }

}
