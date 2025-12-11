using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShieldObject : SkillObject
{
    private int m_iShielCount = 2;
    private int m_iCurShielCount = 2;

    private float m_fDefendValue = 10;
    private float m_fCurDefend = 0;
    public override void OnSpawn()
    {
        m_bIsSkillActive = true;
        base.OnSpawn();
    }
    public override void OnDespawn()
    {
        m_pOwner.OwnerPlayer.m_pHitEvent -= shield;
        m_iCurShielCount = 0;
        m_fCurDefend = 0.0f;
        m_bIsSkillActive = false;
        base.OnDespawn();
    }

    public override void Init()
    {
        m_pOwner.OwnerPlayer.m_pHitEvent += shield;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetInfo(SOSKill _pSkillInfo, string _strKey)
    {
        base.SetInfo(_pSkillInfo, _strKey);
        m_fDefendValue = _pSkillInfo.Defense.baseDefense;
        m_iShielCount = _pSkillInfo.Defense.defenseCount;
    }

    private bool shield(AttackInfo _pAttackInfo)
    {
        m_iCurShielCount += 1;
        m_fCurDefend += _pAttackInfo.Damage;

        if(m_fCurDefend > m_fDefendValue || m_iCurShielCount > m_iShielCount)
        {
            PushObjectPool();
            return true;
        }

        return false;
    }
}
