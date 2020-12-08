using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using Proto.Unity;
using Utility;
public class clickItem : MonoBehaviour
{
    public BaseItem item;

    public Text m_Text;
    public void Awake()
    {
        m_Text = GameObjectRelate.SearchChild(this.transform, "Text").GetComponent<Text>();
    }
    public void onClickItem()
    {
        ItemEvent m_itemevent = new ItemEvent();

        m_itemevent.Optype = 1; // 减少 1
        m_itemevent.Uid = item.m_uid;
        m_itemevent.Count = 1;

        GameManager.Instance.m_Net.SendMessage(m_itemevent, EventType.ITEM);
    }

    void Update()
    {
        if (item.m_count > 1) {
            m_Text.text = item.m_count.ToString();
        } else {
            m_Text.text = "";
        }
    }

}
