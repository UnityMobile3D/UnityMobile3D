using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOSkillUI;

public class SkillSlot : Slot
{
    [SerializeField] private SOSkillUI m_pSOSkill;

    [Header("Skill Type")]
    [SerializeField] private eSkillType m_eSkillType = eSkillType.None;

    public SOSkillUI SOSkill { get => m_pSOSkill; }

    protected override void Awake()
    {
        base.Awake();

        m_iUIType |= (uint)m_eSkillType << 8;
    }

    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);

        m_pSOSkill = _pSOTarget as SOSkillUI;

        SetCoolTime(m_pSOSkill.Cooldown);
    }

    public override void Using()
    {
        m_bCanUse = false;
    }
}
