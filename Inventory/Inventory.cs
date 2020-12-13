using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Proto.Unity;
using Utility;
public class Inventory : Singleton<Inventory>
{
    public PlayerBag m_bagPB;
    private ItemEvent m_itemevent;

    public Dictionary<uint, BaseItem> m_baseItems;
    public uint[] baseBag;

    public uint[] equipBag;

    public uint[] moneyBag;

    public void Init(PlayerBag _bag)
    {
        if (_bag == null) {
            m_bagPB = new PlayerBag();
        } else {
            m_bagPB = _bag;
        }
        Debug.Log("start init bag");
        baseBag = new uint[30];
        equipBag = new uint[10];
        moneyBag = new uint [4];
        m_baseItems = new Dictionary<uint, BaseItem>();
        int i = 0;
        foreach(ItemInfo item in m_bagPB.Items) {
            Debug.Log("item id = " + item.MItemid);
            m_baseItems.Add(item.MItemid, ItemFactory.Instance.CreateItem(item));

            baseBag[i++] = item.MItemid; // 先全放在基础背包里，装备 和 金币 背包 还需要在 protobuf 里增加字段
        }
        m_itemevent = new ItemEvent();

        EventHandler.Instance.AddHandle(EventType.ITEM, ItemUpdate);
    }

    public void ClickEquip()
    {

    }


    public void ClickConsume()
    {

    }

    public void Click(uint _uid)
    {

    }

    public int ItemUpdate(Message _msg)
    {
        Debug.Log("Item Event");
        m_itemevent = ItemEvent.Parser.ParseFrom(_msg.m_data);
        switch (m_itemevent.Optype)
        {
            case 0:
                m_baseItems[m_itemevent.Uid].addItem(m_itemevent.Count);
                break;
            case 1:
                m_baseItems[m_itemevent.Uid].delItem(m_itemevent.Count);
                break;
            default:
                Debug.Log("itemevent undefined");
                break;
        }
        return (int)EventType.ITEM;
    }

    public void InitBags(List<Transform> UCells)
    {
        int i = 0;        
        foreach (uint id  in m_baseItems.Keys)
        {
            GameObject obj = AssetRelate.ResourcesLoadCheckNull<GameObject>("Prefabs/Items/" + getstring(id));
            if (obj == null) {
                return;
            }

            GameObject item = GameObjectRelate.InstantiateGameObject(UCells[i++].gameObject, obj);
            clickItem click = item.GetComponent<clickItem>();
            
            click.item = m_baseItems[id];
        }
    }

    string getstring(uint _uid)
    {
        uint configID = _uid & 0xffff;
        switch (configID)
        {
            case 0:
                return "Money";
            case 1:
                return "HP";
            case 2:
                return "MP";
            case 3:
                return "Sword";
            case 4:
                return "Shield";
            default:
                Debug.Log("undefined");
                return "null";
        }
    }

}




