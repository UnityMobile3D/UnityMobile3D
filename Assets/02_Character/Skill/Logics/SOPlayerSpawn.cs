using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using SkillContext = SkillRunner.SkillContext;


[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerSpawn", fileName = "SOPlayerSpawn")]
public class SOPlayerSpawn : SOSkillLogic
{
    public AssetReference AttackObjectReference = null;
    public override bool UpdateSkill(SkillContext _pSkillContext)
    {
        SkillRunner pPlayerSkill = _pSkillContext.skill;
        TargetingProfile pTargetPro = pPlayerSkill.RunSkill.Option.targetingProfile;

        Vector3 vSpawnPos = pPlayerSkill.gameObject.transform.position;
        Vector3 vDir = pPlayerSkill.gameObject.transform.forward;
        if (pTargetPro.playerFacing == true)
            vSpawnPos += (vDir * pTargetPro.spawnDistance);

        vSpawnPos+= pTargetPro.offset;

        GameObject pAttackObject = 
            ObjectPoolManager.m_Instance.GetObject(AttackObjectReference.AssetGUID, vSpawnPos, Vector3.zero);

        if (pAttackObject.TryGetComponent<SkillAttackObject>(out SkillAttackObject pAttackComp) == true)
        {
            //스킬 초기화
            pAttackComp.SetInfo(pPlayerSkill.RunSkill, AttackObjectReference.AssetGUID);
            pAttackComp.SetDir(vDir);

            _pSkillContext.runSkillObject = pAttackObject;
            _pSkillContext.chargeEvents = pAttackComp.ChargeEvents;

            for(int i = 0; i<_pSkillContext.chargeEvents.Count; ++i)
                _pSkillContext.chargeEvents[i].StartEvent();
            
            return true;
        }
        else
        {
            ObjectPoolManager.m_Instance.PushObject(AttackObjectReference.AssetGUID, pAttackObject);
            return false;
        } 
    }
}
