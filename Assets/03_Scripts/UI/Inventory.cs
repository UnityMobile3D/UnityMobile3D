using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SOEntryUI;


public class Inventory : BaseUI, IContainer
{
    enum eInventoryButton
    {
        //장착
        None,
        Equippped,
        AutoEquipped,
        End,
    }

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Inventory; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }

    [SerializeField] private SlotContainer m_pInterfaceSlotContainer;
    [SerializeField] private SlotContainer m_pEquipSlotContainer;

    [SerializeField] private Container m_pInevenContainer;
    [SerializeField] private ButtonUI m_pCloseButton;

    [SerializeField] private List<eUIType> m_listCategoryType;
    [SerializeField] private List<ButtonUI> m_listCategoryButton;
    [SerializeField] private List<ButtonUI> m_listOptionButton = new List<ButtonUI>();

    //인벤에서 인터페이스 , 장비창
    Dictionary<uint, int> m_hashItemCount = new Dictionary<uint, int>();
    Dictionary<uint, CategoryData> m_hashCategoryData = new Dictionary<uint, CategoryData>();

    //Test
    [SerializeField] private SOEntryUI[] m_arrTestData;

    [SerializeField] private Color m_pBaseColor;

    protected override void Awake()
    {
        base.Awake();

        m_pCloseButton.OnDownEvt += close_tap;
        m_pInevenContainer.OnSelectEvt += select;


        button_option();
      
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

                if(m_hashItemCount.TryGetValue((uint)ListData[j].Id, out int iCurCount))
                    m_hashItemCount[(uint)ListData[j].Id] += testValue;
                else
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
        m_pEquipSlotContainer?.UnActiveSlot();
    }


    public int GetCategoryIdx(eUIType _eUIType)
    {
        for (int i = 0; i < m_listCategoryType.Count; ++i)
        {
            if (m_listCategoryType[i] == _eUIType)
                return i;
        }
        return -1;
    }
    private void close_tap()
    {
        //레이까지 제거하기 위해서
        gameObject.SetActive(false);
    }

    public void AddDataInventroy(SOEntryUI _pData, int _iAmount)
    {
        uint iUITypeCode = _pData.GetUITypeCode();
    }

    public void AddItem(SOEntryUI _pSOItem)
    {
        uint iID = _pSOItem.GetUIHashCode();
        if ((iID & (uint)eUIType.Item) == (uint)eUIType.Item)
        {
            m_pInevenContainer.AddData(_pSOItem);
        }   
    }

    private void select()
    {
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
        if (pTargetView.SOEntryUI == null)
            return;

        //해당 아이템 별 분기처리
        switch(pTargetView.SOEntryUI.Type)
        {
            case eUIType.Item:
                select_item();
                break;
            case eUIType.Equip:
                select_equip();
                break;
            default:
                return;
        }
    }

    private void select_item()
    {
        //전송할 데이터 예약
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
        int iAmount = m_hashItemCount[(uint)pTargetView.SOEntryUI.Id];
        DataService.Instance.StartPickData(this, pTargetView.SOEntryUI, pTargetView.SlotIdx, iAmount, GetCategoryIdx(eUIType.Item));


        //인터페이스로 해당 내 아이템 전송 요청
        int iItemIdx= m_pInterfaceSlotContainer.GetSlotIdx(eUIType.Item);
        m_pInterfaceSlotContainer.ActiveSlotAndAddData(pTargetView.SOEntryUI.GetUIHashCode(), iItemIdx);   
    }

    private void select_equip()
    {
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
       
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
        m_pEquipSlotContainer.ActiveSlot(pTargetView.SOEntryUI.GetUIHashCode());
    }

    private void equipped()
    {
        m_pInevenContainer.ChanageSelect();

        ButtonUI pButton =  m_listOptionButton[(int)eInventoryButton.Equippped];
        if (m_pInevenContainer.IsOnSelect)
            pButton.m_pTextMeshProUGUI.color = Color.yellow;
        else
            pButton.m_pTextMeshProUGUI.color = m_pBaseColor;
    }

    private void auto_equipped()
    {

    }

    private void button_option()
    {
        //기존 텍스쳐 색상 캐싱
        TextMeshProUGUI pTextMeshPro = m_listCategoryButton[0].GetComponentInChildren<TextMeshProUGUI>();
        Color TextColor = pTextMeshPro.color;
        pTextMeshPro.color = Color.yellow;

        //카테고리 버튼
        for (int i = 0; i < m_listCategoryType.Count; ++i)
        {
            CategoryData pCategoryData = m_pInevenContainer.GetCategoryData(i);
            if (pCategoryData == null)
                break;

            m_hashCategoryData.Add((uint)m_listCategoryType[i], pCategoryData);

            int idx = i; //캡처용 복사본
            m_listCategoryButton[i].OnClickEvt += () =>
            {
                //색상 변경
                for (int j = 0; j < m_listCategoryButton.Count; ++j)
                {
                    if (j == idx)
                        m_listCategoryButton[j].m_pTextMeshProUGUI.color = Color.yellow;
                    else
                        m_listCategoryButton[j].m_pTextMeshProUGUI.color = TextColor;
                }

                m_pInevenContainer.ChanageCategory(idx);
            };
        }

        //인벤토리 옵션 버튼
        m_listOptionButton[(int)eInventoryButton.Equippped].OnClickEvt += equipped;
        m_listOptionButton[(int)eInventoryButton.AutoEquipped].OnClickEvt += auto_equipped;
    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pInevenContainer.GetDataIdx(_iDataIdx, _iCategoryIdx);
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pEntryData = GetData(_iDataIdx, _iCategoryIdx);
        if (pEntryData == null)
            return 0;

        int iAmount = 0;
        if (m_hashItemCount.TryGetValue((uint)pEntryData.Id, out iAmount) == false)
            return 0;

        if (m_pInevenContainer.IsCanDuplication(_iCategoryIdx) == true)
            iAmount = 1;

        return iAmount;
    }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0)
    {
        if (_pSoData == null)
            return 0;

        if (m_hashItemCount.TryGetValue((uint)_pSoData.Id, out int iAmount) == false)
            return 0;

        if (m_pInevenContainer.IsCanDuplication(_iCategoryIdx) == true)
            iAmount = 1;
        return iAmount;
    }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        //if (_pSOData.Type != eUIType.Item)
        //    return false;
        uint iID = (uint)_pSOData.Id;

        if (m_hashItemCount.TryGetValue(iID, out int iCount) == true)
            m_hashItemCount[iID] += _iAmount;
        else
            m_hashItemCount.Add(iID, _iAmount);

        m_pInevenContainer.AddData(_pSOData, _iCategoryIdx, _iDataIdx);

        return true;
    }


    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }

    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //1.만약 바꾸는거라면 슬롯에 있는 데이터를 다시 인벤토리에 넣어야함
        //2.컨테이너에서 중복으로 감시하기 때문에 거기서 반환 값을 통해서 감시하기 굳이 인벤토리에서도 있는지 체크하지말기
        int iDataId = m_pInevenContainer.GetTargetSlot().SOEntryUI.Id;
        m_hashItemCount.Remove((uint)iDataId);

        m_pInevenContainer.DeleteData(_iDataIdx, _iCategoryIdx);
        m_pInevenContainer.ClearTarget();
        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }


}
