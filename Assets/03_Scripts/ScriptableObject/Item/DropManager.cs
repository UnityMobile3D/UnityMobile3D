using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager m_Instance;

    // (ID->SOItemUI 매핑 쓰고 싶으면)

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(this);
            return;
        }
        m_Instance = this;
    }

    public void Drop(SODropTable _pTable, in Vector3 _vCenterPos)
    {
        if (_pTable == null)
            return;

        for (int i = 0; i < _pTable.DropEntries.Count; ++i)
        {
            DropEntry pEntry = _pTable.DropEntries[i];

            if (pEntry.Item == null)
                continue;

            //확률 검사
            if (Random.value > pEntry.Probability)
                continue;

            int iCount = Random.Range(pEntry.MinCount, pEntry.MaxCount + 1);

            for (int j = 0; j < iCount; ++j)
                SpawnWorldItem(pEntry.Item, _vCenterPos);
        }
    }

    private void SpawnWorldItem(SOItem pItem, in Vector3 _vCenterPos)
    {
        //랜덤 위치
        Vector3 vPos = _vCenterPos + new Vector3(
            Random.Range(-0.5f, 0.5f),
            0.0f,
            Random.Range(-0.5f, 0.5f));

        // 1) SOItem이 들고 있는 프리팹으로 생성
        GameObject pItemObj = 
            ObjectPoolManager.m_Instance.GetObject(pItem.ItemObject.AssetGUID, vPos, Vector3.zero);
    }
}