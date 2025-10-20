using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//(이벤토리 슬롯)
public class SlotView : ButtonUI
{
    //하이라이트도 나중에 추가
    [SerializeField] private Image m_pIcon = null;
    private RectTransform m_pRectTransform = null;
    private int m_iID = -1; //SOID(현재는 사용하지 않음)
    private int m_iSlotIdx = -1; //내 컨테이너에서 몇번째 슬롯인지
    private SOEntryUI m_pTargetSO = null;

    private Container m_pContainer = null;
    [SerializeField] private TextMeshProUGUI m_pCountBadge = null;

    public SOEntryUI SOEntryUI { get => m_pTargetSO; }
    public int SlotIdx { get => m_iSlotIdx; }

    public void Init(Container _pContainer)
    {
        m_pContainer = _pContainer;
        m_pRectTransform = GetComponent<RectTransform>();

        if(m_pIcon == null)
            m_pIcon = GetComponentInChildren<Image>();
    }

    public void Bind(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        //데이터 바인딩
        if (_pEntryUI != null)
        {
            m_pTargetSO = _pEntryUI;

            m_pIcon.sprite = _pEntryUI.Icon;
            m_pIcon.enabled = true;

            m_iID = _pEntryUI.Id;
        }
        else
        {
            m_pTargetSO = null;
            m_pIcon.enabled = false;

            m_iID = -1;
        }

        m_iSlotIdx = _iSlotIdx;

        update_count();
    }

    override public void OnBeginDrag(PointerEventData e)
    {
        base.OnBeginDrag(e);
        m_pContainer.OnBeginDrag(e);
    }

    override public void OnDrag(PointerEventData e)
    {
        base.OnDrag(e);
        m_pContainer.OnDrag(e);
    }

    override public void OnEndDrag(PointerEventData e)
    {
        base.OnEndDrag(e);
        m_pContainer.OnEndDrag(e);
    }

    public override void OnPointerClick(PointerEventData e)
    {
        if(m_pContainer != null)
        {
            m_pContainer.SetTargetSlot(this);
        }
    }

    private void update_count()
    {
        int iCount = m_pContainer?.GetCount(m_iSlotIdx) ?? 0;

        bool bShow = iCount > 1; // 1개 이하면 보통 표기 안 함
        if (bShow)
        {
            m_pCountBadge.enabled = true;
            m_pCountBadge.text = iCount.ToString();
        }
        else
            m_pCountBadge.enabled = false;

    }
}
