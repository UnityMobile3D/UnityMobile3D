using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour , IContainer
{
    [SerializeField] private SlotContainer m_pEquipSlotContainer = null;

    Dictionary<int, int> m_hashItemCount = new Dictionary<int, int>();

    private void Awake()
    {

    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pEquipSlotContainer.GetData(_iDataIdx, _iCategoryIdx);
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx);
        if (pData == null)
            return 0;

        int iAmount = 1;
        if (m_hashItemCount.TryGetValue(pData.Id, out iAmount) == false)
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

        var pListSlot = m_pEquipSlotContainer.SlotList;
        if (pListSlot.Count <= _iDataIdx || m_hashItemCount.ContainsKey(_pSOData.Id))
            return false;


        //해당 데이터 사입 후 들어온 갯수 기록
        m_hashItemCount.TryAdd(_pSOData.Id, 0);
        m_hashItemCount[_pSOData.Id] += _iAmount;

        m_pEquipSlotContainer.AddData(_iDataIdx, _pSOData, _iCategoryIdx);

        m_pEquipSlotContainer.UnActiveSlot();

        // 장비 능력치 player에게 적용

        return true;
    }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0)
    {
        return true;
    }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx, _iCategoryIdx);
        if (pData == null)
            return false;


        delete(pData.Id, _iDataIdx, _iCategoryIdx);

        return true;
    }

    private void delete(int _iDataId, int _iDataIdx, int _iCategoryIdx = 0)
    {
        m_pEquipSlotContainer.DeleteData(_iDataIdx, _iCategoryIdx);
        m_hashItemCount.Remove(_iDataId);
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }


}
