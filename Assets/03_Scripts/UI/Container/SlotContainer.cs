using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotContainer : MonoBehaviour, IContainer
{ 
    [SerializeField] private List<Slot> m_listSlot = new List<Slot>();

    Dictionary<int, int> m_hashItemCount = new Dictionary<int,int>();

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


    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        if (m_listSlot[_iDataIdx] == null)
            return null;

        return m_listSlot[_iDataIdx].SOTarget;
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx);
        if (pData == null)
            return 0;

        int iAmount = 1;
        if(m_hashItemCount.TryGetValue(pData.Id, out iAmount) == false)
            return 0;

        return iAmount;
    }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0)
    {
        int iAmount = 0;
        if (m_hashItemCount.TryGetValue(_pSoData.Id, out iAmount) == false)
            return 0;

        return iAmount;
    }


    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0) 
    {
        //기존에 누른 데이터
        if (_pSOData == null)
            return false;

        if (m_listSlot.Count <= _iDataIdx || m_hashItemCount.ContainsKey(_pSOData.Id))
            return false;


        //해당 데이터 사입 후 들어온 갯수 기록
        m_hashItemCount.TryAdd(_pSOData.Id, 0);
        m_hashItemCount[_pSOData.Id] += _iAmount;

        m_listSlot[_iDataIdx].Bind(_pSOData);
      
        UnActiveSlot();

        return true;
    }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0)
    {
        if (m_listSlot[_iDataIdx].SOTarget == null)
            return false;

        int iDataId = m_listSlot[_iDataIdx].SOTarget.Id;
        m_hashItemCount[iDataId] -= _iAmount;

        if (m_hashItemCount[iDataId] <= 0)
            return false;

        return true;
    }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx);
        if (pData == null)
            return false;

        m_hashItemCount.Remove(pData.Id);
        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }
}
