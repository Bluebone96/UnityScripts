using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class BagPanel : MonoBehaviour
{
    public Transform m_basebag;
    public List<Transform> m_basebagcells;
    
    public Dictionary<uint, BaseItem> m_baseItems;

    void Start()
    {
        m_baseItems = Inventory.Instance.m_baseItems;

        m_basebag = GameObjectRelate.SearchChild(this.transform, "BaseBag");
        m_basebagcells = GameObjectRelate.SearchChildsPartName(m_basebag, "UCell");

        Inventory.Instance.InitBags(m_basebagcells);
    }

}


