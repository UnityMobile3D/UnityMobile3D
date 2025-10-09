using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOItemUI;
using static SOSkillUI;

public class ItemSlot : Slot
{
    [SerializeField] private SOItemUI m_pSOItem;

    [Header("Item Type")]
    [SerializeField] private eItemType m_eItemType = eItemType.None;

    public SOItemUI SOItem { get => m_pSOItem; }

    private Values m_tItemValue = new Values();

    protected override void Awake()
    {
        base.Awake();

        m_iUIType |= (uint)m_eItemType << 16;
    }

    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);

        m_pSOItem = _pSOTarget as SOItemUI;

        SetCoolTime(m_pSOItem.Cooldown);
    }

    public override void Using()
    {
        m_bCanUse = false;
    }
}
