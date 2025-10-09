using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Container : ButtonUI
{
    //UI 컨테이너 (스킬창, 인벤토리 창)

    //view담당 (고정된 슬롯을 렌더링하게 슬롯이 100개면 보이는구간만 렌더링되게)
    //data 담당 (SO를 활용해서 초기 데이터 저장)
    //controll은 UGUI pointer에서 담당

    //잘라낼 클리핑 영역
    /*
    RectMask2D가 Viewport 사각형을 기준으로 자른다
    시각적으로 잘릴 뿐, 바깥의 셀도 그려지긴 하므로(제출됨) 진짜 비용 절감은 가상화로 한다
    마스크 영역 밖은 레이캐스트도 차단된다(입력도 안 잡힘).
    */

    [Header("CONTANIER")]
    private RectMask2D m_pRectMask;

    [SerializeField] private RectTransform m_pContainerView; // 프레임(마스크) Rect
    [SerializeField] private RectTransform m_pContentView;  // 셀들이 붙는 부모 Rect

    [SerializeField] private List<SOEntryUI> m_listData = new List<SOEntryUI>();
    private List<SlotView> m_listView = new List<SlotView>();

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
    private Vector2 m_vDragCurPosition = Vector2.zero;

    private Vector2 m_vContainerDragPosition = Vector2.zero;
    private Vector2 m_vViewCurDrageLine = Vector2.zero;//현재 드래그 라인

    private Vector2 m_vContaninerSize = Vector2.zero; //전체 컨테이너 크기

    //가장 높은 인덱스에 순차적으로 넣기 위해서 내림차순
    PriorityQueue<uint> m_pqSlotIdx = new PriorityQueue<uint>(Comparer<uint>.Create((a,b)=> b.CompareTo(a)));

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


    private int find_data_idx(SOEntryUI _pDataUI)
    {
        for (int i = 0; i < m_listData.Count; ++i)
        {
            if (m_listData[i] == _pDataUI)
                return i;
        }

        return -1;
    }

    public bool DeleteData(SOEntryUI _pSOEntryUI)
    {
        int iIdx = find_data_idx(_pSOEntryUI);
        if (iIdx == -1)
            return false;

        m_listData[iIdx] = null;
        m_pqSlotIdx.Enqueue((uint)iIdx);

        //데이터 새로 바인딩
        bind_data();

        return true;
    }

    private bool AddData(SOEntryUI _pSOEntryUI)
    {
        if (m_pqSlotIdx.IsEmpty())
            return false;

        int iFindIdx = find_data_idx(_pSOEntryUI);
        //이미 존재
        if (iFindIdx != -1)
            return false;

        //첫번째 자리
        uint iFirstIdx = m_pqSlotIdx.Dequeue();
        m_listData[(int)iFirstIdx] = _pSOEntryUI;

        return true;
    }



    private void Build()
    {
        clear_data();

        m_pRectMask = GetComponent<RectMask2D>();
        m_pRectMask.padding = new Vector4(m_vPadding.x, m_vPadding.y, m_vPadding.x, m_vPadding.y);

        //슬롯 최대치 보정
        clamp_slot();

        //부드럽게 이동을 위한 뒤에 버퍼까지 계산
        m_iRowCount = m_iSlotRowCount > 0 ? m_iSlotRowCount + 1 : 0;
        m_iColCount = m_iSlotColCount;

        //슬롯 프리팹 생성
        var prefabRT = (RectTransform)m_pSlotPrefab.transform;
        Vector2 vPadding = m_vPadding;

        vPadding.x = m_vSlotSize.x /2.0f + m_vPadding.x;
        vPadding.y = m_vSlotSize.y /2.0f  + m_vPadding.y;

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

        //실제 크기는 슬롯 수가 아닌, 데이터 수에 따라
        int iRowSize = m_listData.Count >= m_listView.Count ? 
            (m_listData.Count / m_iColCount) : m_iRowCount;

        iRowSize -= m_iSlotRowCount;
        if(iRowSize < 0)
            iRowSize = 0;

        m_vContaninerSize.x = vStep.x * m_iColCount;
        m_vContaninerSize.y = vStep.y * iRowSize;

        //슬롯 바인딩
        bind_data();

        //남은 슬롯 수 체크
        for(int i = 0; i<m_listData.Count; ++i)
        {
            if (m_listData[i] == null)
                m_pqSlotIdx.Enqueue((uint)i);
        }

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

    private void bind_data()
    {
        //보이는 구간 업데이트
        int iStartIdx = m_iCurRow * m_iColCount;

        for(int i = 0; i<m_listView.Count; ++i)
        {
            if (iStartIdx + i >= m_listData.Count)
                return;

            int iDataIdx = iStartIdx + i;
            m_listView[i].Bind(m_listData[iDataIdx]);
        }
    }
    //셀 사이즈 + 양끝 시작 끝 간격 + 셀 사이 간격

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

        m_vViewCurDrageLine += new Vector2(0, vPositionDelta.y); //view 입장에서 위치
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
            bind_data();
        }
    }
    public void SetTargetSlot(SlotView _pTargetSlot)
    {
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

    private void clear_data()
    {
        //기존 리스트 삭제 (에디터 버전 오브젝트 삭제)
        for (int i = m_pContentView.childCount - 1; i >= 0; --i)
            Undo.DestroyObjectImmediate(m_pContentView.GetChild(i).gameObject);
        m_listView.Clear();
    }

}
