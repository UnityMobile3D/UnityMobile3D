using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolAble
{
    [SerializeField] private SOItem m_pItem;
    public SOItem GetItem() => m_pItem;

    private int iPlayerLayer = -1;

    private float m_fCurTime = 0.0f;
    private float m_fLifeTime = 10.0f;

    private int m_iPushPoolCount = 1;
    [SerializeField] private bool m_bDontDestroy = false;
    public void OnSpawn()
    {
        m_fCurTime = 0.0f;
        m_iPushPoolCount = 1;
    }
    public void OnDespawn()
    {

    }

    private void Update()
    {
        if (m_bDontDestroy == true)
            return;

        m_fCurTime += Time.deltaTime;
        if (m_fCurTime > m_fLifeTime)
            PushObjectPool();

    }

    public int GetItemID()
    {
        return m_pItem.ItemID;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SOEntryUI pItemData = ItemDataManager.m_Instance.GetItemData(m_pItem.ItemID);
            if (DataService.m_Instance.TryAddData(eContainerType.Inventory, pItemData, 1) == true)
                PushObjectPool();
        }
    }

    public void PushObjectPool()
    {
        if (m_iPushPoolCount <= 0)
            return;

        --m_iPushPoolCount;
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pItem.ItemObject.AssetGUID, gameObject);
    }
}
