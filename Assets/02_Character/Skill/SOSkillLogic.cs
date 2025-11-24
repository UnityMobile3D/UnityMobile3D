using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillContext = Skill.SkillContext;

//[CreateAssetMenu(menuName = "SO/Profiles/Logic", fileName = "LogicProfile")]
public abstract class SOSkillLogic : ScriptableObject
{
    public abstract bool UpdateSkill(SkillContext _pSkillContext);
}
