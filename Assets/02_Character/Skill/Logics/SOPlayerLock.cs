using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillContext = SkillRunner.SkillContext;


[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerLock", fileName = "SOPlayerLock")]

public class SOPlayerLock : SOSkillLogic
{
    public bool bLockMove = false;
    public bool bLockRot = false;
   
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        Rigidbody pPlayerRigid = _pSkillContext.skill.OwnerPlayer.RigidBody;

        if(bLockRot == true)
            pPlayerRigid.constraints |= RigidbodyConstraints.FreezeRotationY;
        else
            pPlayerRigid.constraints &= ~RigidbodyConstraints.FreezeRotationY;
        
        pPlayerRigid.velocity = bLockMove == true ? Vector3.zero : pPlayerRigid.velocity;
        
        return true;
    }
}
