using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eUIType = SOEntryUI.eUIType;

public class SlotContainer : MonoBehaviour
{ 
    [SerializeField] private List<Slot> m_listSlot = new List<Slot>();
    public List<Slot> SlotList => m_listSlot;

    private IContainer m_pOwner = null;
    public void Awake()
    {
        int iCount = 0;
        for(int i = 0; i < m_listSlot.Count; i++)
            m_listSlot[i].SetSlotIdx(iCount++);

        m_pOwner = GetComponentInParent<IContainer>();
    }

    public void ActiveSlot(uint _iUIHashCode)
    {
        foreach (Slot pSlot in m_listSlot)
        {
           uint iSlotUICode = pSlot.GetSlotHashCode();
           //pSlot.SetRaycast(false);

            if (_iUIHashCode == iSlotUICode)
                pSlot.ActiveSlot();
        }
    }

    public void ActiveSlotAndAddData(uint _iUIHashCode, int _iSlotIdx)
    {
        Slot pSlot = m_listSlot[_iSlotIdx];
        uint iSlotUICode = pSlot.GetSlotHashCode();

        if (_iUIHashCode == iSlotUICode)
            DataService.Instance.TryDropDataAndSwap(m_pOwner, pSlot.SlotIdx);
    }

    public void UnActiveSlot()
    {
        for(int i = 0; i<m_listSlot.Count; ++i)
        {
            //m_listSlot[i]?.SetRaycast(false);
            m_listSlot[i]?.UnActiveSlot();
        }
    }

    public int GetSlotIdx(eUIType _eUIType)
    {
        for(int i = 0; i<m_listSlot.Count; ++i)
        {
            if (m_listSlot[i].eUIType == _eUIType)
                return i;
        }
        return -1;
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
