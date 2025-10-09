using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTree : BaseUI
{
    private Canvas m_pSkillCanvas;

    [SerializeField] private ButtonUI m_pCloseButton;
    [SerializeField] private ButtonUI m_pSeleteButton;
    [SerializeField] private Container m_pSkillContainer;

    [SerializeField] private PlayerInterface m_pPlayerInterface;

    //그냥 static canvas에서 slot 관리 여기서 slotManager에 SO를 던져주면 거기서 해당 슬롯 SkillType에 맞는 슬롯 활성화

    protected override void Awake()
    {
        base.Awake();

        m_pSkillCanvas = GetComponent<Canvas>();

        //close selete 함수 바인딩
        m_pCloseButton.OnUpEvt += close_tap;

        //컨테이너에서 스킬 눌렸다면 가져올 수 있게
        m_pSkillContainer.OnSelectEvt += select_skill;

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
    private void select_skill()
    {

        SlotView pTargetSlot = m_pSkillContainer.GetTargetSlot();
        m_pPlayerInterface.ActiveSkillSlot(pTargetSlot.SOEntryUI);

    }
    private void deselete_skill()
    {
    }

 

    
}
