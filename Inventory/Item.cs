using UnityEngine;
using Utility;
using System.Collections.Generic;
using Google.Protobuf;
using Proto.Unity;

public enum ItemType {
    MONEEY = 0,
    CONSUME = 1,

    EQUIP = 2
};

public enum AttributeType
{
    HP  =0,
    MP =1,
    ATK =2,
}

public abstract class BaseItem
{
    public uint m_uid { get; set; }
    public string m_name { get; set; }
    public ItemType m_type { get; set;}
    public int m_flags { get; set;}
    public uint m_price { get; set; }

    public uint m_count { get; set; }

    public BaseItem(uint _uid, string _name, ItemType _type, int _flags, uint _count, uint _value)
    {
        this.m_uid = _uid;
        this.m_name = _name;
        this.m_flags = _flags;
        this.m_type = _type;
        this.m_count = _count;
        this.m_price = _value;
    }

    virtual public void initItem()
    {
        // TODO
    }
    public void addItem(uint n)
    {
        m_count += n;
    }

    public void delItem(uint n)
    {
        m_count -= n;
    }

    public abstract uint getAttribute(AttributeType _type);
    public abstract void setAttribute(AttributeType _type, uint _v);
};


public class ConsumeItem : BaseItem
{
    public Dictionary<AttributeType, uint> m_mAttribute = new Dictionary<AttributeType, uint>();
    public ConsumeItem(uint _uid, string _name, ItemType _type, int _flags, uint _count, uint _value) : base(_uid, _name, _type, _flags, _count, _value)
    {

    }

    public override void initItem() 
    {

    }
    public void setAttribute()
    {

    }
    
    public override uint getAttribute(AttributeType _type)
    {
        return m_mAttribute[_type];
    }

    public override void setAttribute(AttributeType _type, uint _v) {
        m_mAttribute[_type] = _v;
    }


}


public class EquipItem : BaseItem
{
    public Dictionary<AttributeType, uint> m_mAttribute  = new Dictionary<AttributeType, uint>();
    public EquipItem(uint _uid, string _name, ItemType _type, int _flags, uint _count, uint _value) : base(_uid, _name, _type, _flags, _count, _value)
    {

    }

    public override uint getAttribute(AttributeType _type)
    {
        return m_mAttribute[_type];
    }

    public override void setAttribute(AttributeType _type, uint _v) {
        m_mAttribute[_type] = _v;
    }
}

public class Money : BaseItem
{
    public Money(uint _uid, string _name, ItemType _type, int _flags, uint _count, uint _value) : base(_uid, _name, _type, _flags, _count, _value)
    {
        
    }

    public override uint getAttribute(AttributeType _type)
    {
        return 0;
    }

    public override void setAttribute(AttributeType _type, uint _v) {
        // null
    }
}



public class ItemFactory : Singleton<ItemFactory>
{
    public BaseItem CreateItem(ItemInfo _itemPb)
    {
        BaseItem item = null;
        switch (_itemPb.MType)
        {
            case 0:
                item =  new Money(_itemPb.MUid, "money", ItemType.MONEEY, 0, _itemPb.MCount, _itemPb.MPrice);
                break;
            case 1:
                item = new ConsumeItem(_itemPb.MUid, "consume", ItemType.CONSUME, 0, _itemPb.MCount, _itemPb.MPrice);
                item.setAttribute(AttributeType.HP, _itemPb.MHp);
                break;
            case 2:
                item = new ConsumeItem(_itemPb.MUid, "equip", ItemType.EQUIP, 0, _itemPb.MCount, _itemPb.MPrice);
                item.setAttribute(AttributeType.ATK, _itemPb.MAtk);
                break;
            default:
                Debug.Log("item type error");
                break;
        }
        return item;
    }
}