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

    //�׳� static canvas���� slot ���� ���⼭ slotManager�� SO�� �����ָ� �ű⼭ �ش� ���� SkillType�� �´� ���� Ȱ��ȭ

    protected override void Awake()
    {
        base.Awake();

        m_pSkillCanvas = GetComponent<Canvas>();

        //close selete �Լ� ���ε�
        m_pCloseButton.OnUpEvt += close_tap;

        //�����̳ʿ��� ��ų ���ȴٸ� ������ �� �ְ�
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
        //���̱��� �����ϱ� ���ؼ�
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
