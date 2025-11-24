using Game.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using SkillContext = Skill.SkillContext;


[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerSpawn", fileName = "SOPlayerSpawn")]
public class SOPlayerSpawn : SOSkillLogic
{
    public AssetReference AttackObjectReference = null;
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        
        Skill pPlayerSkill = _pSkillContext.skill;
        TargetingProfile pTargetPro = pPlayerSkill.CurrentSkill.Option.targetingProfile;

        Vector3 vSpawnPos = pPlayerSkill.transform.position;
        if (pTargetPro.playerFacing == true)
            vSpawnPos += vSpawnPos + (pPlayerSkill.transform.forward * pTargetPro.spawnDistance);

        vSpawnPos+= pTargetPro.offset;

        GameObject pAttackObject = 
            ObjectPoolManager.m_Instance.GetObject(AttackObjectReference.AssetGUID, vSpawnPos, Vector3.zero);

        if (pAttackObject.TryGetComponent<SkillAttackObject>(out SkillAttackObject pAttackComp) == false)
        {
            pAttackComp.SetSpawnKey(AttackObjectReference.AssetGUID);
            return true;
        }
        else
        {
            ObjectPoolManager.m_Instance.PushObject(AttackObjectReference.AssetGUID, pAttackObject);
            return false;
        } 
    }
}
