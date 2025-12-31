using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMonster : Monster
{
   
    override protected void Awake()
    {
        base.Awake();
    }

    override protected void Start()
    {
        base.Start();
    }

    override public void MonsterUpdate()
    {
        base.MonsterUpdate();
    }
   
    public override void TakeDamage(AttackInfo _pAttackInfo)
    {
        DamageManager.m_Instance.Damaged(m_pMonsterInfo, _pAttackInfo);

        m_pBlackbard.HpRatio = (float)m_pMonsterInfo.HP / m_pMonsterInfo.MaxHp;

        if (m_pMonsterInfo.HP <= 0.0f)
        {
            ClearAttackObject();
            Dead();
        } 
    }

}
