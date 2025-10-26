using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : BaseUI, IContainer
{

    [SerializeField] private ButtonUI m_pCloseButton;
    [SerializeField] private ButtonUI m_pSeleteButton;
    [SerializeField] private Container m_pItemContainer;

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Store; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }

    protected override void Awake()
    {
        base.Awake();

        //close selete 함수 바인딩
        //m_pCloseButton.OnUpEvt += close_tap;

        m_pItemContainer.OnSelectEvt += select_shapitem;

    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        //m_pPlayerInterface.
    }
    private void close_tap()
    {
        //레이까지 제거하기 위해서
        gameObject.SetActive(false);

    }
    private void select_shapitem()
    {
        SlotView pTarget = m_pItemContainer.GetTargetSlot();
        SOShapItem pShapItem = pTarget.SOEntryUI as SOShapItem;

        if (pShapItem != null)
        {
           //데이터 매니저에서 플레이어 코인 값 가져오기 가져왔다면 비교 후 DataService를 통해서 전달
        }
    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0) { return null; }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0) { return -1; }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0) { return -1; }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //기존 스킬창에서 스킬이 지워지는 일은 없음
        m_pItemContainer.ClearTarget();
        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx) { return false; }


}
