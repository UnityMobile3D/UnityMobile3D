using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/WaitAnimation", fileName = "SOWaitAnimation")]
public class SOWaitAnimation : SOSkillLogic
{
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        if(_pSkillContext.hitEvent == true)
        {
            return true;
        }
        return false;
    }
}
