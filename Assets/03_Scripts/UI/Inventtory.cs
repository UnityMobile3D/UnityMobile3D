using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOEntryUI;

public class Inventtory : BaseUI
{
    [SerializeField] private PlayerInterface m_pPlayerInterface;
    [SerializeField] private Container m_pInevenContainer;
    [SerializeField] private ButtonUI m_pCloseButton;
 
    //�κ����� �������̽� , ���â
    
    protected override void Awake()
    {
        base.Awake();

        m_pCloseButton.OnDownEvt += close_tap;
        m_pInevenContainer.OnSelectEvt += select_item;

    }

    private void close_tap()
    {
        //���̱��� �����ϱ� ���ؼ�
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
