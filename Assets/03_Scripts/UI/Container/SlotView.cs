using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotView : ButtonUI
{
    //하이라이트도 나중에 추가

    private Image m_pIcon;
    private RectTransform m_pRectTransform;
    private int m_iID;
    private SOEntryUI m_pTargetSO;

    private Container m_pOwner;

    public SOEntryUI SOEntryUI { get => m_pTargetSO; }
    public void Init(Container _pOwner)
    {
        m_pOwner = _pOwner;
        m_pRectTransform = GetComponent<RectTransform>();
        m_pIcon = GetComponent<Image>();
    }

    public void Bind(SOEntryUI _pEntryUI)
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
    }

    override public void OnBeginDrag(PointerEventData e)
    {
        base.OnBeginDrag(e);
        m_pOwner.OnBeginDrag(e);
    }

    override public void OnDrag(PointerEventData e)
    {
        base.OnDrag(e);
        m_pOwner.OnDrag(e);
    }

    override public void OnEndDrag(PointerEventData e)
    {
        base.OnEndDrag(e);
        m_pOwner.OnEndDrag(e);
    }

    public override void OnPointerClick(PointerEventData e)
    {
        if(m_pOwner != null)
        {
            m_pOwner.SetTargetSlot(this);
        }
    }

}
