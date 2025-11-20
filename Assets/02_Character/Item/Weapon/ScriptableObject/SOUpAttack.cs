using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Effect
{
    [CreateAssetMenu(menuName = "SO/Item/Effect/UpAttack")]
    public class SOUpAttack : SOItemEffect
    {
        public override void Apply(EffectContext _tEffectCnt)
        {
            if(_tEffectCnt.pTarget == null)
                return;

            if (_tEffectCnt.pTarget.TryGetComponent<Health>(out var pHealth) == true)
                pHealth.AddAttackPower(_tEffectCnt.Value.Int);
        }
    };   
}

