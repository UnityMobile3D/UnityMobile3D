using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CategoryData
{
    public List<SOEntryUI> m_ListData = new List<SOEntryUI>();

    [SerializeField] private bool m_bCanDuplication = true; //중복 허용할지(장비 템 창, 스킬 창)
    
    public HashSet<SOEntryUI> m_setData = null; //중복 확인을 위한 해쉬
    public bool IsCanDuplication { get => m_bCanDuplication; }
    public int m_iCurrentRemnantData = 0;
    public bool IsFull => m_iCurrentRemnantData <= 0;
    public int m_iCategoryIdx = 0;
    public int GetRemnantDataIdx()
    {
        for (int i = 0; i < m_ListData.Count; ++i)
        {
            if (m_ListData[i] == null)
                return i;
        }

        return -1;
    }
}

public class Container : ButtonUI
{
    //UI 컨테이너 (스킬창, 인벤토리 창)

    //view담당 (고정된 슬롯을 렌더링하게 슬롯이 100개면 보이는구간만 렌더링되게)
    //data 담당 (SO를 활용해서 초기 데이터 저장)
    //controll은 UGUI pointer에서 담당
    //카테고리별로 슬롯뷰는 동일하되 데이터는 따로 보여줄 수 있게 


    private IContainer m_IOwner;

    [Header("CONTANIER")]
    private RectMask2D m_pRectMask;

    [SerializeField] private RectTransform m_pContainerView; // 프레임(마스크) Rect
    [SerializeField] private RectTransform m_pContentView;  // 셀들이 붙는 부모 Rect

    //[SerializeField] private List<SOEntryUI> m_listData = new List<SOEntryUI>(); //실세 데이터
    [SerializeField] private List<CategoryData> m_listCategoryData = new List<CategoryData>();
    private List<SlotView> m_listView = new List<SlotView>();
    [SerializeField] private int m_iCurrentCategoryIdx = 0;
    public int CurrentCategoryIdx { get=> m_iCurrentCategoryIdx;}

    [SerializeField] private int m_iCategoryCount = 0;
    public int CategoryCount { get => m_iCategoryCount; }
    //public List<SOEntryUI> ListData { get => m_listData; }

    [SerializeField] private SlotView m_pSlotPrefab; //셀 프리팹
    [SerializeField] private Vector2 m_vStep; //셀 사이 간격(x=열 간, y=행 간)
    [SerializeField] private Vector2 m_vPadding;  // L,T,R,B (컨테이너 내부 여백)

    [Header("TargetSLOT")]
    private SlotView m_pTargetSlot;//id로 바꿀 수 있음 (매니저에서 가져오게 아니면 그냥 SO들고있기)
    [SerializeField] public PED onSelect;
    public event Action OnSelectEvt;

    [SerializeField] private GameObject m_pSelectFramePrefab;
    private RectTransform m_pFrameRectTrasnform;
    private Image m_pFrameImage;

    
    [Header("SLOT")]
    [SerializeField] private int m_iSlotColCount = 3;
    [SerializeField] private int m_iSlotRowCount = 2;

    [SerializeField] private Vector2 m_vSlotSize = Vector2.zero;

    //부드러운 이동을 위해 버퍼를 +1씩더 만들기
    private int m_iColCount = -1;
    private int m_iRowCount = -1;

    //현재 바라보고있는 행 위치
    private int m_iCurRow = 0;

    [Header("DRAG")]
    private Vector2 m_vDragCurPosition = Vector2.zero; //저번 프레임과 이번 프레임의 차이를 구하기 위해

    private Vector2 m_vContainerDragPosition = Vector2.zero;
    private Vector2 m_vViewCurDrageLine = Vector2.zero;//현재 드래그 라인
    private Vector2 m_vContaninerSize = Vector2.zero; //전체 컨테이너 크기


    //가장 높은 인덱스에 순차적으로 넣기 위해서 내림차순
    //PriorityQueue<uint> m_pqSlotIdx = new PriorityQueue<uint>()/*(Comparer<uint>.Create((a,b)=> b.CompareTo(a)))*/;

    //[SerializeField] private bool m_bCanDuplication = true; //중복 허용할지(장비 템 창, 스킬 창)
    //private HashSet<SOEntryUI> m_setData = null; //중복 확인을 위한 해쉬
    //public bool IsCanDuplication { get => m_bCanDuplication;}
    //public int m_iCurrentRemnantData = 0;
    //public bool IsFull => m_iCurrentRemnantData <= 0;


    //빌드 전용
    public bool Run = false;
    protected override void Awake()
    {
        base.Awake();

        Build();
        
        if(m_pSelectFramePrefab != null)
        {
            GameObject pFrameObejct = Instantiate(m_pSelectFramePrefab, m_pContentView);
            m_pFrameImage = pFrameObejct?.GetComponent<Image>();
            m_pFrameImage.enabled = false;

            m_pFrameRectTrasnform = pFrameObejct?.GetComponent<RectTransform>();
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // 에디터 미리보기 전용
        if (Application.isPlaying || Run) 
            return;

        //재 진입 방지 , 예외가 나면 Run이 false가 안될 수 있으므로 finally에서 false로
        Run = true;                    
        EditorApplication.delayCall += () =>
        {
            try
            {
                if (this == null) 
                    return;

                Build();
            }
            finally
            {
                Run = false;
            }
        };
    }
#endif

    /*/////////////////////////////////////
                    Conatiner
     *////////////////////////////////////

    private SOEntryUI find_data_idx(int _iDataIdx, int _iCategoryIdx = 0)
    {
        CategoryData pCategoryData = GetCategoryData(_iCategoryIdx);

        if(pCategoryData == null)
            return null;

        return pCategoryData.m_ListData[_iDataIdx];
    }

    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        CategoryData pCategoryData = GetCategoryData(_iCategoryIdx);
        if (pCategoryData == null || pCategoryData.m_ListData[_iDataIdx] == null)
            return false;

        SOEntryUI pDeleteData = pCategoryData.m_ListData[_iDataIdx];
        //중복 허용이 안된다면 set에서도 제거
        if (pCategoryData.IsCanDuplication == false)
            pCategoryData.m_setData.Remove(pDeleteData);

        pCategoryData.m_ListData[_iDataIdx] = null;
        ++pCategoryData.m_iCurrentRemnantData;

        //데이터 새로 바인딩
        BindData(_iCategoryIdx);

        return true;
    }

    //-1이면 남는 데이터 인덱스에 넣기 , 0이면 기본 데이터 리스트에 넣기
    public bool AddData(SOEntryUI _pSOEntryUI , int _iCategoryIdx = 0, int _iIdx = -1)
    {
        CategoryData pCategoryData = GetCategoryData(_iCategoryIdx);

        if (pCategoryData == null || pCategoryData.IsFull == true)
            return false;
       

        //중복 허용되고 내 리스트에 이미 해당 데이터가 있다면
        if(pCategoryData.IsCanDuplication == false && pCategoryData.m_setData.Contains(_pSOEntryUI))
            return false;

        if(_iIdx == -1)
        {
            //남는 자리
            int iRemIdx = pCategoryData.GetRemnantDataIdx();
            if(iRemIdx == -1)
                return false;

            pCategoryData.m_ListData[iRemIdx] = _pSOEntryUI;
        }
        else
        {
            //지정된 자리
            if (pCategoryData.m_ListData[_iIdx] != null)
                return false;

            pCategoryData.m_ListData[_iIdx] = _pSOEntryUI;
        }
     
        if(pCategoryData.IsCanDuplication == false)
            pCategoryData.m_setData.Add(_pSOEntryUI);

        BindData(_iCategoryIdx);

        --pCategoryData.m_iCurrentRemnantData;

        return true;
    }
  

    private void Build()
    {
        m_IOwner = GetComponentInParent<IContainer>();

        clear_data();

        m_pRectMask = GetComponent<RectMask2D>();
        m_pRectMask.padding = new Vector4(m_vPadding.x, m_vPadding.y, m_vPadding.x, m_vPadding.y);

        //슬롯 최대치 보정
        clamp_slot();

        sort_data();

        m_iCategoryCount = m_listCategoryData.Count;

        //남은 슬롯 수 체크
        for (int i = 0; i < m_listCategoryData.Count; ++i)
        {
            int iRemnantData = 0;
            for (int j = 0; j < m_listCategoryData[i].m_ListData.Count; ++j)
            {
                if (m_listCategoryData[i].m_ListData[j] == null)
                    ++iRemnantData;
            }
            m_listCategoryData[i].m_iCurrentRemnantData = iRemnantData;
            m_listCategoryData[i].m_iCategoryIdx = i;
        }

        //슬롯 바인딩
        BindData(m_iCurrentCategoryIdx);
    }

    //슬롯 최대 계수 지정 어차피 보여질 부분만 만들기 때문에 불필요하게 더 늘리지 않기
    private void clamp_slot()
    {
        float fViewWidth = m_pContainerView.rect.width;
        float fViewHeight = m_pContainerView.rect.height;

        float fLeft = m_vPadding.x;
        float fRight = m_vPadding.x;
        float fTop = m_vPadding.y;
        float fBot = m_vPadding.y;

        Vector2 vStep = m_vSlotSize + m_vStep;

        int iMaxCols = Mathf.Max(1, Mathf.FloorToInt((fViewWidth - fLeft - fRight + m_vStep.x) / vStep.x));
        int iMaxRows = Mathf.Max(1, Mathf.FloorToInt((fViewHeight - fTop - fBot + m_vStep.y) / vStep.y));

        if (m_iSlotColCount > iMaxCols) 
            m_iSlotColCount = iMaxCols;
        if (m_iSlotRowCount > iMaxRows) 
            m_iSlotRowCount = iMaxRows;
    }    

    public void sort_data()
    {
        //부드럽게 이동을 위한 뒤에 버퍼까지 계산
        m_iRowCount = m_iSlotRowCount > 0 ? m_iSlotRowCount + 1 : 0;
        m_iColCount = m_iSlotColCount;

        //슬롯 프리팹 생성
        var prefabRT = (RectTransform)m_pSlotPrefab.transform;
        Vector2 vPadding = m_vPadding;

        vPadding.x = m_vSlotSize.x / 2.0f + m_vPadding.x;
        vPadding.y = m_vSlotSize.y / 2.0f + m_vPadding.y;

        Vector2 vStep = m_vSlotSize + m_vStep;

        for (int i = 0; i < m_iRowCount; ++i)
        {
            for (int j = 0; j < m_iColCount; ++j)
            {
                SlotView pSlot = Instantiate(m_pSlotPrefab, m_pContentView);
                pSlot.Init(this);

                var pRect = (RectTransform)pSlot.transform;

                pRect.anchoredPosition = new Vector2(
                    vPadding.x + vStep.x * j,
                    -vPadding.y - vStep.y * i
                );

                pRect.sizeDelta = m_vSlotSize;
                m_listView.Add(pSlot);
            }
        }

        //모든 데이터는 0번을 기준을 값은 데이터 크기를 가진다
        CategoryData pBaseCategoryData = m_listCategoryData[0];
        List<SOEntryUI> pListData= pBaseCategoryData.m_ListData;

        //실제 크기는 슬롯 수가 아닌, 데이터 수에 따라
        int iRowSize = pListData.Count >= m_listView.Count ?
            (pListData.Count / m_iColCount) : m_iRowCount;

        iRowSize -= m_iSlotRowCount;
        if (iRowSize < 0)
            iRowSize = 0;

        m_vContaninerSize.x = vStep.x * m_iColCount;
        m_vContaninerSize.y = vStep.y * iRowSize;

        for(int i = 0; i<m_listCategoryData.Count; ++i)
        {
            CategoryData pCategoryDate = m_listCategoryData[i];
            pListData = pCategoryDate.m_ListData;

            if (pCategoryDate.IsCanDuplication == false)
            {
                //중복 허용이 안된다면 중복된 데이터는 삭제
                pCategoryDate.m_setData = new HashSet<SOEntryUI>();
                for (int j = 0; j < pListData.Count; ++j)
                {
                    if (pListData[j] == null)
                        continue;

                    if (pCategoryDate.m_setData.Contains(pListData[j]))
                        pListData[j] = null;
                    else
                        pCategoryDate.m_setData.Add(pListData[j]);
                }
            }

        }

    }

    public void BindData(int _iCategoryIdx)
    {
        List<SOEntryUI> pListData = GetListData(_iCategoryIdx);
        if (pListData == null)
            return;

        //보이는 구간 업데이트
        int iStartIdx = m_iCurRow * m_iColCount;

       
        for(int i = 0; i<m_listView.Count; ++i)
        {
            if (iStartIdx + i >= pListData.Count)
                return;

            int iDataIdx = iStartIdx + i;
            m_listView[i].Bind(pListData[iDataIdx], iDataIdx);
        }
    }
  


    /*/////////////////////////////////////
                  Input 
   *////////////////////////////////////

    public override void OnBeginDrag(PointerEventData e)
    {
        if (m_pFrameImage != null)
            m_pFrameImage.enabled = false;

        m_vDragCurPosition = e.position;
    }

    public override void OnDrag(PointerEventData e)
    {
        Vector2 vPositionDelta =  e.position - m_vDragCurPosition;
       
        m_vDragCurPosition = e.position;

        m_vViewCurDrageLine += new Vector2(0, vPositionDelta.y);      //view 입장에서 위치
        m_vContainerDragPosition += new Vector2(0, vPositionDelta.y); //컨테이너 전체 입장에서 위치
            
        //스탭 오류 수정
        Vector2 vStep = m_vSlotSize + m_vStep;
     
        //나머지 연산 = 내 컨테이너 전체 사이즈에서 현재까지 움직인 양
        m_vContainerDragPosition.y = Mathf.Clamp(m_vContainerDragPosition.y, 0f, m_vContaninerSize.y);

        // --- 현재 행/뷰 오프셋 계산 ---
        // 행(0-base)
        int iRow = Mathf.FloorToInt(m_vContainerDragPosition.y / vStep.y);
      
        // % 대신 Repeat을 쓰면 안전
        float fContentPosY = Mathf.Repeat(m_vContainerDragPosition.y, vStep.y);

        m_vViewCurDrageLine = new Vector2(0.0f, fContentPosY);

        // 실제 이동
        m_pContentView.anchoredPosition = new Vector2(0.0f, m_vViewCurDrageLine.y);

        if(m_iCurRow != iRow)
        {
            m_iCurRow = iRow;
            BindData(m_iCurrentCategoryIdx);
        }
    }
    public void SetTargetSlot(SlotView _pTargetSlot)
    {
        if(_pTargetSlot.SOEntryUI == null)
            return;

        //해당 슬롯에 프레임 장착
        if (m_pFrameImage != null)
        {
            m_pFrameImage.enabled = true;
            MoveFrameToSlot(_pTargetSlot.GetComponent<RectTransform>());
        }

        m_pTargetSlot = _pTargetSlot;

        //콜백함수
        onSelect?.Invoke();
        OnSelectEvt?.Invoke();
    }

    public SlotView GetTargetSlot()
    {
        return m_pTargetSlot;
    }

    public void MoveFrameToSlot(RectTransform _pSlotRect)
    {
        // 프레임 설정
        m_pFrameRectTrasnform.anchoredPosition = _pSlotRect.anchoredPosition;
        m_pFrameRectTrasnform.SetAsLastSibling(); // 항상 위로
    }

    

    public void ClearTarget()
    {
        if (m_pFrameImage != null)
            m_pFrameImage.enabled = false;

        m_pTargetSlot = null;
    }

    private void clear_data()
    {
        //기존 리스트 삭제 (에디터 버전 오브젝트 삭제)
        for (int i = m_pContentView.childCount - 1; i >= 0; --i)
            Undo.DestroyObjectImmediate(m_pContentView.GetChild(i).gameObject);

        m_listView.Clear();
    }

    /*/////////////////////////////////////
                  Data Category
    *////////////////////////////////////

    public List<SOEntryUI> GetListData(int _iCategoryIdx)
    {

        CategoryData pCategoryData = GetCategoryData(_iCategoryIdx);
        if (pCategoryData == null)
            return null;

        if(pCategoryData.m_ListData ==null)
            return null;

        return pCategoryData.m_ListData;
    }

  
    public CategoryData GetCategoryData(int _iCategoryIdx)
    {
        if (_iCategoryIdx >= m_listCategoryData.Count)
            return null;

        return m_listCategoryData[_iCategoryIdx];
    }

    public SOEntryUI GetDataIdx(int _iDataIdx, int _iCategoryIdx)
    {
        List<SOEntryUI> pListData = GetListData(_iCategoryIdx);
        if (pListData == null || pListData[_iDataIdx] == null)
            return null;

        return pListData[_iDataIdx];
    }

    public void ChanageCategory(int _iCategoryIdx)
    {
        if (m_iCategoryCount <= _iCategoryIdx)
            return;

        m_iCurrentCategoryIdx = _iCategoryIdx;
        BindData(m_iCurrentCategoryIdx);
    }

    public bool IsCanDuplication(int _iCategoryIdx)
    {
        CategoryData pCategoryData = GetCategoryData(_iCategoryIdx);
        if(pCategoryData == null) 
            return false;

        return pCategoryData.IsCanDuplication;
    }

    public int GetCount(int _iDataIdx)
    {
        if (m_IOwner == null)
            return -1;

        int iAmount = m_IOwner.GetDataAmount(_iDataIdx, m_iCurrentCategoryIdx);
        return iAmount;
    }

    public int GetCount(SOEntryUI _pEntryUI)
    {
        int iAmount = m_IOwner.GetDataAmount(_pEntryUI, m_iCurrentCategoryIdx);
        return iAmount;
    }

    
}
