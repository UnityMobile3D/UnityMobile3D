using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
public class Monster : MonoBehaviour , IHealth , IPoolAble
{
    [SerializeField] protected Blackboard m_pBlackbard = new Blackboard();
    private BehaviorTree m_pBHTree = null;
    [SerializeField] protected SOMonsterInfo m_SOMonsterInfo = null;

    protected Rigidbody m_pRigidbody = null;
    protected Animator m_pAnimator = null;
    protected NavMeshAgent m_pNavMeshAgent = null;
    public SOMonsterInfo SOMonsterInfo => m_SOMonsterInfo;

    protected CooldownModule m_pCollDownModule = null;

    private ObjectInfo m_pMonsterInfo = null;
    private Coroutine m_pKnockbackRoutine = null;

    private string m_strPoolKey = "";

    protected bool m_bHit = false;
    //IHealth
    public int CurrentHP => m_pMonsterInfo.HP;
    public int MaxHP => m_pMonsterInfo.MaxHp;

    //PoolAble
    public void OnSpawn()
    {
        m_bHit = false;
    }
    public void OnDespawn()
    {

    }

    virtual protected void Awake()
    {
        m_pBlackbard.Self = transform;
        m_pBlackbard.Agent = GetComponent<NavMeshAgent>();
        m_pBlackbard.AnimBridge = GetComponent<AnimationBridge>();
        m_pBHTree= GetComponent<BehaviorTree>();

        m_pMonsterInfo = GetComponent<ObjectInfo>();
        m_pRigidbody = GetComponent<Rigidbody>();
        m_pAnimator = GetComponent<Animator>();
        m_pNavMeshAgent = GetComponent<NavMeshAgent>();

        m_pBHTree.Init(m_pBlackbard, this);

        m_pCollDownModule = new CooldownModule();
        m_pCollDownModule.Init(this);
        m_pBlackbard.CooldownModule = m_pCollDownModule;


        //objectinfo
        m_pNavMeshAgent.speed = m_pMonsterInfo.Speed;
    }

    virtual protected void Start()
    {
        //나중에 네트워크로 가면 문제
        m_pBlackbard.Target = GameManager.m_Instance.Player.gameObject.transform;
    }
    virtual protected void Update()
    {
        m_pCollDownModule.UpdateCooldown();

        if(m_bHit == false)
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

        if (pAttackObj.TryGetComponent<MonsterAttackObject>(out var pAttack) == false)
        {
            ObjectPoolManager.m_Instance.PushObject(strKey, pAttackObj);
            return null;
        }

        pAttack.SetInfo(pSkillInfo);
        pAttack.SetDir(vDir);
        pAttack.SetOwner(this);

        //근접공격은 피격시 사라지고, 소환공격은 계속 유지되게
        if (pSkillInfo.DestroyOnHit == true)
            m_pBlackbard.SpawnObjects.Add(pAttack);
        

        return pAttackObj;
    }
    public void ClearAttackObject()
    {
        List<MonsterAttackObject> listAttack = m_pBlackbard.SpawnObjects;
        for(int i = 0; i<listAttack.Count; ++i)
        {
            if (listAttack[i].MonsterSkillInfo.DestroyOnHit == true)
                listAttack[i].PushPoolObject();
        }
        
        m_pBlackbard.SpawnObjects.Clear();
        m_pBlackbard.CurrentAttackIdx = -1;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    public void EndHit()
    {
        m_bHit = false;
        m_pNavMeshAgent.isStopped = true;
    }
    //IHealth 구현
    public void Hit()
    {
        m_pAnimator.SetTrigger("Hit");
        m_bHit = true;

        m_pNavMeshAgent.isStopped = true;
    }
    public void TakeDamage(AttackInfo _pAttackInfo)
    {
        m_pMonsterInfo.AddHP((int)_pAttackInfo.Damage * -1);

        ClearAttackObject();
        if (m_pMonsterInfo.HP <= 0.0f)
        {
            Dead();
        }
        else
        {
            Hit();

            knockback(_pAttackInfo);
        }

    }
    public void Heal(int _iAmount)
    {
        m_pMonsterInfo.AddHP(_iAmount);
    }

    public bool IsDead()
    {
        return CurrentHP <= 0;
    }

    private void knockback(AttackInfo _attackInfo)
    {
        // 기존 넉백 코루틴 돌고 있으면 정지
        if (m_pKnockbackRoutine != null)
        {
            StopCoroutine(m_pKnockbackRoutine);
            m_pKnockbackRoutine = null;
        }

        Vector3 vMonsterPos = m_pRigidbody.position;
        Vector3 vDir = vMonsterPos - _attackInfo.AttackerPosition;
        vDir.y = 0.0f;

        if (vDir.sqrMagnitude < 0.0001f)
            vDir = -transform.forward;

        vDir.Normalize();

        transform.rotation = Quaternion.LookRotation(-vDir, Vector3.up);


        //시간이 지나면서 점점 멈추게 (속도 감쇠)
        m_pKnockbackRoutine = StartCoroutine(knockback_coroutine(vDir,(int)_attackInfo.Power));
    }

    private IEnumerator knockback_coroutine(Vector3 _vDir, int _iPower)
    {
        float fElapsed = 0f;

        while (m_bHit == true && fElapsed <=1.0f)
        {
            float fRevElaps = 1.0f - fElapsed;
            Vector3 vVelocity = _vDir * _iPower * fRevElaps;
            Vector3 vNextPos = m_pRigidbody.position + vVelocity * Time.fixedDeltaTime;

            m_pRigidbody.MovePosition(vNextPos);

            fElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        m_pKnockbackRoutine = null;
    }

    protected virtual void Dead()
    {
        m_bHit = true;
        m_pAnimator.SetTrigger("Dead");
    }

    public void PushObjectPool()
    {
        if(string.IsNullOrEmpty(m_strPoolKey) == true)
            Destroy(gameObject);
        else
            ObjectPoolManager.m_Instance.PushObject(m_strPoolKey, gameObject);
    }
}
