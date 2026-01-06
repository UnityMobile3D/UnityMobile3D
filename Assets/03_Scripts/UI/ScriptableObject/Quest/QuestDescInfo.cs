using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDescInfo : BaseUI
{
    [SerializeField] private ButtonUI m_pCloseButton = null;
    [SerializeField] private ButtonUI m_pCompletedButton = null;
    [SerializeField] private ButtonUI m_pGiveUPButton = null;

    [SerializeField] private TextMeshProUGUI m_pTMPQuestTitle = null;
    [SerializeField] private TextMeshProUGUI m_pTMPQuestDesc = null;
    [SerializeField] private TextMeshProUGUI m_pTMPQuestProgress = null;

    
    private QuestState m_pOwner = null;

    protected override void Awake()
    {
        base.Awake();

        m_pGiveUPButton.OnClickEvt += giveup_quest;
    }

    public void SetOwner(QuestState _pOwner)
    {
        m_pOwner = _pOwner;
    }
    public void SettingQuest(SOQuestUI _pTargetQuest, float _fProgress)
    {
        m_pTMPQuestTitle.text = _pTargetQuest.QuestTitle.GetLocalizedString(); 
        m_pTMPQuestDesc.text = _pTargetQuest.QuestDescription.GetLocalizedString();

        m_pTMPQuestProgress.text = $"진행도 ({_fProgress}%)";
    }

    private void giveup_quest()
    {
        //m_pOwner
    }
    
}
