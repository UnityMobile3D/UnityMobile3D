using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Monster : MonoBehaviour
{
    [SerializeField] protected Blackboard m_pBlackbard = new Blackboard();
    private BehaviorTree m_pBHTree = null;

    [SerializeField] protected SOMonsterInfo m_SOMonsterInfo = null;
    public SOMonsterInfo SOMonsterInfo => m_SOMonsterInfo;

    protected CooldownModule m_pCollDownModule = null;

    private Dictionary<int, List<GameObject>> m_hashAttackObject = new();
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

        //내 공격 오브젝트만큼 크기 늘리기
        
        for(int i = 0; i<m_SOMonsterInfo.skillinfo.Count; ++i)
            m_hashAttackObject.Add(i, new List<GameObject>());
        
    }

    virtual protected void Start()
    {
        m_pBlackbard.Target = GameManager.m_Instance.Player.gameObject.transform;
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

    public GameObject Spawn(Vector3 _vSpawnPos)
    {
        int iTargetIdx = m_pCollDownModule.TargetIdx;
     
        //몬스터 스킬 정보를 통해서 타겟 몬스터 어택 오브젝트 레퍼런스의 아이디를 가져오기
        MonsterSkillInfo pSkillInfo = m_SOMonsterInfo.skillinfo[iTargetIdx];
        string strKey = pSkillInfo.SOPoolEntry.prefabRef.AssetGUID;

        //위치 방향 잡기
        if (pSkillInfo.SpawnCurPos == true)
            _vSpawnPos = gameObject.transform.position;
        else if (pSkillInfo.SpawnPlayerPos == true)
            _vSpawnPos = m_pBlackbard.Target.transform.position;

        Vector3 vDir = pSkillInfo.AttackDir;
        if (pSkillInfo.PlayerDir == true)
            vDir = (m_pBlackbard.Target.transform.position - _vSpawnPos).normalized;
        vDir *= pSkillInfo.SpawnDiff;

        GameObject pAttackObj = ObjectPoolManager.m_Instance.GetObject(
            strKey, _vSpawnPos + vDir, pSkillInfo.SpawnRot);

        if (pAttackObj == null)
            return null;

        if (pAttackObj.TryGetComponent<MonsterAttackObject>(out var pInfo) == false)
        {
            ObjectPoolManager.m_Instance.PushObject(strKey, pAttackObj);
            return null;
        }

        pInfo.SetInfo(pSkillInfo);
        pInfo.SetDir(vDir);
        pInfo.SetOwner(this);
        m_hashAttackObject[iTargetIdx].Add(pAttackObj);

        return pAttackObj;
    }

    public void EndCurAttack()
    {
        //가장 마지막 공격 오브젝트 중지
        var listAttack = m_hashAttackObject[m_pCollDownModule.TargetIdx];

        //뒤에서 부터 삭제
        for(int i = listAttack.Count -1; i >=0; --i)
        {
            if(listAttack[i].TryGetComponent<MonsterAttackObject>(out var pAttack) == true)
            {
                listAttack.RemoveAt(listAttack.Count - 1);
                pAttack.PushPoolObject();
            }
        }

        listAttack.Clear();
        //복귀
        m_pBlackbard.AnimBridge.m_pAnimator.speed = 1.0f;
    }

    public void ClearAttackObject()
    {
        for(int i = 0; i < m_hashAttackObject.Count; ++i)
        {
            var listAttack = m_hashAttackObject[i];
            for (int j = 0; j < listAttack.Count; ++j)
            {
                if (listAttack[j].TryGetComponent<MonsterAttackObject>(out var pAttack) == true)
                    pAttack.PushPoolObject();
            }
            listAttack.Clear();
        }
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        
    }


 

}
