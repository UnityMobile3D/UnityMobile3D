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

    override protected void Update()
    {
        base.Update();
    }

    public override void TakeDamage(AttackInfo _pAttackInfo)
    {
        MinusHP((int)_pAttackInfo.Damage * -1);

        if (m_pMonsterInfo.HP <= 0.0f)
        {
            ClearAttackObject();
            Dead();
        } 
    }

}
