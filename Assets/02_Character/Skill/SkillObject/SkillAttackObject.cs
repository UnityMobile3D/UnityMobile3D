using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillAttackObject : MonoBehaviour, IPoolAble
{
    protected SkillRunner m_pOwner = null;
    public SkillRunner Owner => m_pOwner;

    private Rigidbody m_pRigidbody = null;
    private Collider m_pCollider = null;

    private SOSKill m_pSkill = null;

    private List<IChargeEvent> m_listChargeEvent = new List<IChargeEvent>();
    public List<IChargeEvent> ChargeEvents => m_listChargeEvent;

    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vDir = Vector3.zero;

    [SerializeField] private float m_fLifeTime = 10.0f;
    private float m_fCurLifeTime = 0.0f;

    private bool m_bIsAttackActive = false;
    public bool IsAttackActive => m_bIsAttackActive;

    [SerializeField] private float m_fStartAttackTime = 0.0f;
    [SerializeField] private float m_fEndAttackTime = float.MaxValue;

    [SerializeField] private int m_iAttackCount = 1;
    private int m_iCurAttackCount = 0;

    private AttackInfo m_pAttackInfo = null;

    private uint m_iCreateCount = 0;
    private string m_strSpawnKey = string.Empty;

    public void Awake()
    {
        m_pRigidbody = GetComponentInChildren<Rigidbody>(true);
        m_pCollider = GetComponentInChildren<Collider>(true);
        m_pAttackInfo = new AttackInfo();

        var listEvent = GetComponentsInChildren<IChargeEvent>(true);
        for(int i = 0; i<listEvent.Length; ++i)
            m_listChargeEvent.Add(listEvent[i]);
    }

    public void OnSpawn()
    {
        m_iCreateCount = 1;
        if(m_fStartAttackTime <= 0.0f)
            StartAttack();
        else
            EndAttack();

    }
    public void OnDespawn()
    {
        m_iCreateCount = 0;

        m_bIsAttackActive = false;
    }
    public void OnDisable()
    {
        m_fCurLifeTime = 0.0f;
        m_iCurAttackCount = 0;
    }

    public void Update()
    {
        m_fCurLifeTime += Time.deltaTime;

        if(m_bIsAttackActive == false && m_fCurLifeTime >= m_fStartAttackTime)
            StartAttack();
        

        if (m_fCurLifeTime >= m_fLifeTime)
        {
            //풀에 반납
            m_fCurLifeTime = 0.0f;

            PushPoolObject();
        }
    }


    public void FixedUpdate()
    {
        if (m_pRigidbody == null)
            return;

         Vector3 vStep = m_vDir.normalized * m_fMoveSpeed * Time.fixedDeltaTime;
         m_pRigidbody.MovePosition(m_pRigidbody.position + vStep);
    }

    public void PushPoolObject()
    {
        if (m_iCreateCount == 0)
            return;

        m_iCreateCount = 0;
        ObjectPoolManager.m_Instance.PushObject
             (m_strSpawnKey, gameObject);
    }

    public void SetInfo(SOSKill _pSkillInfo, string _strKey)
    {
        m_pSkill = _pSkillInfo;


        m_fLifeTime = _pSkillInfo.Option.lifeTime;

        m_fMoveSpeed = _pSkillInfo.Option.moveSpeed;
        m_pAttackInfo.Damage = (int)_pSkillInfo.Damage.baseDamage;
        m_pAttackInfo.Power = (int)_pSkillInfo.Option.power;
        m_iAttackCount = _pSkillInfo.Option.targetingProfile.MaxTargets;   
        m_fStartAttackTime = _pSkillInfo.Option.startAttackTime;

        m_strSpawnKey = _strKey;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }

    public void SetOwner(SkillRunner _pPlayerSkillRunner) { m_pOwner = _pPlayerSkillRunner; }

    public void SetSpawnKey(string _strKey) { m_strSpawnKey = _strKey; }


    void OnParticleSystemStopped()
    {
        PushPoolObject();
    }


    public void OnTriggerEnter(Collider other)
    {
        if ((m_pSkill.Option.targetingProfile.TargetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            Vector3 vHitPoint = other.ClosestPoint(transform.position);
            m_pAttackInfo.HitPoint = vHitPoint;
            m_pAttackInfo.HitPoint.y = 0.0f;

            other.GetComponent<IHealth>()?.TakeDamage(m_pAttackInfo);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        
    }

    private bool check_attack_count()
    {
        ++m_iCurAttackCount;
        if (m_iCurAttackCount >= m_iAttackCount)
            return true;

        return false;
    }

    public void StartAttack()
    {
        if(m_pCollider == true)
            m_pCollider.enabled = true;
        m_bIsAttackActive = true;
    }
    public void EndAttack()
    {
        if (m_pCollider == true)
            m_pCollider.enabled = false;
        m_bIsAttackActive = false;
    }
}
