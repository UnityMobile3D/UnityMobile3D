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

    [SerializeField] protected SOMonsterInfo m_SOMonsterInfo = null;
    public SOMonsterInfo SOMonsterInfo => m_SOMonsterInfo;

    protected CooldownModule m_pCollDownModule = null;

    virtual protected void Awake()
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


    virtual protected void Update()
    {
        m_pCollDownModule.UpdateCooldown();

        m_pBHTree.Evaluate();
    }

    MonsterSkillInfo GetMonsterSkillInfo(int _iIdx)
    {
        return m_SOMonsterInfo.skillinfo[_iIdx];
    }

    //기본 몬스터 공격
    public void SpawnAttackObject()
    {
        Spawn(Vector3.zero);
    }

    public void Spawn(Vector3 _vSpawnPos)
    {
        int iTargetIdx = m_pCollDownModule.TargetIdx;
     
        //몬스터 스킬 정보를 통해서 타겟 몬스터 어택 오브젝트 레퍼런스의 아이디를 가져오기
        MonsterSkillInfo pSkillInfo = m_SOMonsterInfo.skillinfo[iTargetIdx];
        string strKey = pSkillInfo.SOPoolEntry.prefabRef.AssetGUID;

        //위치 방향 잡기
   
        if (pSkillInfo.SpawnCurPos == true)
            _vSpawnPos = gameObject.transform.position;

        Vector3 vDir = pSkillInfo.AttackDir;
        if (pSkillInfo.PlayerDir == true)
            vDir = (m_pBlackbard.Target.transform.position - _vSpawnPos).normalized;
        vDir *= pSkillInfo.SpawnDiff;

        GameObject pAttackObj = ObjectPoolManager.m_Instance.GetObject(
            strKey, _vSpawnPos + vDir, pSkillInfo.SpawnRot);

        if (pAttackObj == null)
            return;

        if (pAttackObj.TryGetComponent<MonsterAttackObject>(out var pInfo) == false)
        {
            ObjectPoolManager.m_Instance.PushObject(strKey, pAttackObj);
            return;
        }

        pInfo.SetInfo(pSkillInfo);
        pInfo.SetDir(vDir);
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        
    }
}
