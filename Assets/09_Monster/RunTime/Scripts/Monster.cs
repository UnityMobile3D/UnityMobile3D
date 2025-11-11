using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Monster : MonoBehaviour
{
    [SerializeField] private Blackboard m_pBlackbard = new Blackboard();
    private BehaviorTree m_pBHTree = null;

    [SerializeField] private SOMonsterInfo m_SOMonsterInfo = null;
    public SOMonsterInfo SOMonsterInfo => m_SOMonsterInfo;

    private CooldownModule m_pCollDownModule = null;

    private void Awake()
    {
        m_pBlackbard.Self = transform;
        m_pBlackbard.Agent = GetComponent<NavMeshAgent>();
        m_pBlackbard.AnimBridge = GetComponent<AnimationBridge>();
        m_pBHTree= GetComponent<BehaviorTree>();

        m_pBHTree.Init(m_pBlackbard, this);

        m_pCollDownModule = new CooldownModule();
        m_pCollDownModule.Init(this);
        m_pBlackbard.CooldownModule = m_pCollDownModule;

    }


    private void Update()
    {
        m_pCollDownModule.UpdateCooldown();

        m_pBHTree.Evaluate();
    }

    public void SpawnAttackObject()
    {
        int iTargetIdx = m_pCollDownModule.TargetIdx;
        if (m_SOMonsterInfo.skillinfo.Count <= iTargetIdx)
            return;

        //몬스터 스킬 정보를 통해서 타겟 몬스터 어택 오브젝트 레퍼런스의 아이디를 가져오기
        MonsterSkillInfo pSkillInfo = m_SOMonsterInfo.skillinfo[iTargetIdx];
        string strKey = pSkillInfo.SOPoolEntry.prefabRef.AssetGUID;

        //위치 방향 잡기
        Vector3 vSpawnPos = pSkillInfo.SpawnPos;
        if(pSkillInfo.SpawnCurPos == true)
            vSpawnPos = gameObject.transform.position;

        Vector3 vDir = pSkillInfo.AttackDir;
        if(pSkillInfo.PlayerDir == true)
            vDir = (m_pBlackbard.Target.transform.position - vSpawnPos).normalized;
        vDir *= pSkillInfo.SpawnDiff;

        GameObject pAttackObj = ObjectPoolManager.m_Instance.GetObject(
            strKey, vSpawnPos + vDir, pSkillInfo.SpawnRot);

        if (pAttackObj == null)
            return;

        if(pAttackObj.TryGetComponent<MonsterAttackObject>(out var pInfo) == false)
        {
            ObjectPoolManager.m_Instance.PushObject(strKey, pAttackObj);
            return;
        }

        pInfo.SetInfo(pSkillInfo);
        pInfo.SetDir(vDir);
        
    }
    
}
