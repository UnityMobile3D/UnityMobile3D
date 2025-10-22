using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOEntryUI;

public class Inventory : BaseUI, IContainer
{
    [SerializeField] private SlotContainer m_pSlotContainer;
    [SerializeField] private Container m_pInevenContainer;
    [SerializeField] private ButtonUI m_pCloseButton;
 
    //인벤에서 인터페이스 , 장비창
    Dictionary<uint, int> m_hashItemCount = new Dictionary<uint, int>();
    //Test
    [SerializeField] private SOEntryUI[] m_arrTestData;
    protected override void Awake()
    {
        base.Awake();

        m_pCloseButton.OnDownEvt += close_tap;
        m_pInevenContainer.OnSelectEvt += select_item;

    }
    protected void Start()
    {
        //현재 인벤토리에서 아이템들 수 확인
        int testValue = -1;
        for(int i = 0; i<m_pInevenContainer.CategoryCount; ++i)
        {
            testValue += 2;
            CategoryData pCategoryData = m_pInevenContainer.GetCategoryData(i);
            List<SOEntryUI> ListData = pCategoryData.m_ListData;

            for (int j = 0; j < ListData.Count; ++j)
            {
                if (ListData[j] == null)
                    continue;

                m_hashItemCount.Add((uint)ListData[j].Id, testValue);
            }
        }
        m_pInevenContainer.BindData(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            int i = UnityEngine.Random.Range(0, m_arrTestData.Length - 1);
            AddItem(m_arrTestData[i]);
        }
    }

    private void OnDisable()
    {
        m_pSlotContainer?.UnActiveSlot();
    }
    private void close_tap()
    {
        //레이까지 제거하기 위해서
        gameObject.SetActive(false);
    }

    public void AddItem(SOEntryUI _pSOItem)
    {
        uint iID = _pSOItem.GetUIHashCode();
        if ((iID & (uint)eUIType.Item) == (uint)eUIType.Item)
        {
            m_pInevenContainer.AddData(_pSOItem);
        }   
    }

    private void select_item()
    {
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
        if (pTargetView.SOEntryUI == null)
            return;

        //현재 데이터 갯수 만큼 인터페이스 넘겨주기
        //중복 허용하지 않는다면 무조건 1
        int iCount = 1;

        int iCurInvenIdx = m_pInevenContainer.CurrentCategoryIdx;
        if (m_pInevenContainer.IsCanDuplication(iCurInvenIdx) == false)
        { 
            if (m_hashItemCount.TryGetValue((uint)pTargetView.SOEntryUI.Id, out iCount) == false)
                return;
        }
      
        //데이터 서비스에서 지금 눌린 데이터 참조
        DataService.Instance.StartPickData(this, pTargetView.SOEntryUI, pTargetView.SlotIdx, iCount);

        //인터페이스 매니저를 만들어서 해당 클래스에게 요청하는 식으로 변경
        m_pSlotContainer.ActiveSlot(pTargetView.SOEntryUI.GetUIHashCode());
    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pInevenContainer.GetDataIdx(_iDataIdx, _iCategoryIdx);
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pEntryData = GetData(_iDataIdx);
        if (pEntryData == null)
            return 0;

        int iAmount = 0;
        if (m_hashItemCount.TryGetValue((uint)pEntryData.Id, out iAmount) == false)
            return 0;

        //인벤토리창에 아이템 부분은 중복이 혀용 안됨 단 장비는 가능
        if (m_pInevenContainer.IsCanDuplication(_iCategoryIdx) == true)
            iAmount = 1;

        return iAmount;
    }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0) { return -1; }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        //if (_pSOData.Type != eUIType.Item)
        //    return false;
        
        uint iID = (uint)_pSOData.Id;

        if (m_hashItemCount.TryGetValue(iID, out int iCount) == true)
            m_hashItemCount[iID] += _iAmount;
        else
            m_hashItemCount.Add(iID, _iAmount);

        m_pInevenContainer.AddData(_pSOData, _iDataIdx);

        return true;
    }


    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }

    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //1.만약 바꾸는거라면 슬롯에 있는 데이터를 다시 인벤토리에 넣어야함
        //2.컨테이너에서 중복으로 감시하기 때문에 거기서 반환 값을 통해서 감시하기 굳이 인벤토리에서도 있는지 체크하지말기
        int iDataId = m_pInevenContainer.GetTargetSlot().SOEntryUI.Id;
        m_hashItemCount.Remove((uint)iDataId);

        m_pInevenContainer.DeleteData(_iDataIdx);
        m_pInevenContainer.ClearTarget();
        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }
}
