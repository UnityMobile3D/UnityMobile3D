using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/Charge", fileName = "SOPlayerCharge")]
public class SOPlayerCharge : SOSkillLogic
{
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        float fChargeTime = _pSkillContext.skill.CurrentSkill.Option.chargetime;
        if(_pSkillContext.time>= fChargeTime)
            return true;
        
        return true;
    }
}
