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
            //해당 오브젝트에 IHeal 인터페이스 접근
        }
    }
}

