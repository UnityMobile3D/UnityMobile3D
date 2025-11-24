using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerLock", fileName = "SOPlayerLock")]

public class SOPlayerLock : SOSkillLogic
{
    public bool bLockMove = false;
    public bool bLockRot = false;
   
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        Rigidbody pPlayerRigid = _pSkillContext.skill.OwnerPlayer.RigidBody;

        pPlayerRigid.freezeRotation = bLockRot;
        pPlayerRigid.velocity = bLockMove == true ? Vector3.zero : pPlayerRigid.velocity;
        
        return true;
    }
}
