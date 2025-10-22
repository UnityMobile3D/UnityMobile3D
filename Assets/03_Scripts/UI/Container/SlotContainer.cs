using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotContainer : MonoBehaviour
{ 
    [SerializeField] private List<Slot> m_listSlot = new List<Slot>();
    public List<Slot> SlotList => m_listSlot;

    public void Awake()
    {
        int iCount = 0;
        for(int i = 0; i < m_listSlot.Count; i++)
            m_listSlot[i].SetSlotIdx(iCount++);
    }

    public void ActiveSlot(uint _iUIHashCode)
    {
        foreach (Slot pSlot in m_listSlot)
        {
           uint iSlotUICode = pSlot.GetSlotHashCode();
            pSlot.SetRaycast(false);

            if (_iUIHashCode == iSlotUICode)
                pSlot.ActiveSlot();
        }
    }
    public void UnActiveSlot()
    {
        for(int i = 0; i<m_listSlot.Count; ++i)
        {
            m_listSlot[i]?.SetRaycast(true);
            m_listSlot[i]?.UnActiveSlot();
        }
    }

    public void AddData(int _iDataIdx, SOEntryUI _pSOData, int _iCeategoryIdx = 0)
    {
        if (_iDataIdx >= m_listSlot.Count)
            return;

        m_listSlot[_iDataIdx].Bind(_pSOData);
    }

    public void DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        if (_iDataIdx >= m_listSlot.Count)
            return;

        m_listSlot[_iDataIdx].Bind(null);
    }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        if (m_listSlot[_iDataIdx] == null)
            return null;

        return m_listSlot[_iDataIdx].SOTarget;
    }



}
