using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestSlotView : SlotView
{
    private long QuestID = -1;

    private SOQuestUI m_pSOQuest = null;
    public SOQuestUI SOQuest { get => m_pSOQuest; }

    override protected void Awake()
    {
        base.Awake();
    }

    public override void Bind(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        m_pSOQuest = _pEntryUI as SOQuestUI;
        if (m_pSOQuest == null)
            return;

        base.Bind(m_pSOQuest, _iSlotIdx);

        m_pTextMeshProUGUI.text = m_pSOQuest.QuestTitle.GetLocalizedString();
    }

}
