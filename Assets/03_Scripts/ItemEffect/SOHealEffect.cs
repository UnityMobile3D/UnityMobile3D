using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Effects/Heal")]
public class SOHealEffect : SOItemEffect
{
    public override void Apply(ref EffectContext _tEffectCnt)
    {
        GameObject pTarget = _tEffectCnt.pTarget;
        if(pTarget != null)
        {
            //�ش� ������Ʈ�� IHeal �������̽� ����
        }
    }
}

