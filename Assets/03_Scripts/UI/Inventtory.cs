using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOEntryUI;

public class Inventtory : BaseUI
{
    [SerializeField] private PlayerInterface m_pPlayerInterface;
    [SerializeField] private Container m_pInevenContainer;
    [SerializeField] private ButtonUI m_pCloseButton;
 
    //인벤에서 인터페이스 , 장비창
    
    protected override void Awake()
    {
        base.Awake();

        m_pCloseButton.OnDownEvt += close_tap;
        m_pInevenContainer.OnSelectEvt += select_item;

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
            //m_pContainer.
        }
        
    }


    private void select_item()
    {
        SlotView pTargetSlot = m_pInevenContainer.GetTargetSlot();
        m_pPlayerInterface.ActiveSkillSlot(pTargetSlot.SOEntryUI);

    }

}
