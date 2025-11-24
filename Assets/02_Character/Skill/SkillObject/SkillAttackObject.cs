using Game.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackObject : MonoBehaviour, IPoolAble
{
    protected Player m_pOwner = null;

    private Rigidbody m_pRigidbody = null;
    private Collider m_pCollider = null;

    private SOSKill m_pSkill = null;

    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vDir = Vector3.zero;

    [SerializeField] private float m_fLifeTime = 10.0f;

    private float m_fCurLifeTime = 0.0f;

    [SerializeField] private int m_iAttackCount = 1;
    private int m_iCurAttackCount = 0;

    private float m_fDamage = 0.0f;
    public float Damage => m_fDamage;

    [SerializeField] bool m_bAccVel = false;

    private uint m_iCreateCount = 0;
    private string m_strSpawnKey = string.Empty;

    public void Awake()
    {
        m_pRigidbody = GetComponent<Rigidbody>();
        m_pCollider = GetComponent<Collider>();
    }

    public void OnSpawn()
    {
        m_iCreateCount = 1;
    }
    public void OnDespawn()
    {
        m_iCreateCount = 0;
    }

    public void OnEnable()
    {
        m_fCurLifeTime = 0.0f;
        m_vDir = Vector3.zero;
        m_pCollider.enabled = false;
    }

    public void Update()
    {
        m_fCurLifeTime += Time.deltaTime;
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

        if (m_bAccVel == true)
            m_pRigidbody.AddForce(m_vDir.normalized * m_fMoveSpeed, ForceMode.Acceleration);
        else
        {
            Vector3 vStep = m_vDir.normalized * m_fMoveSpeed * Time.fixedDeltaTime;
            m_pRigidbody.MovePosition(m_pRigidbody.position + vStep);
        }
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
        m_fDamage = _pSkillInfo.Damage.baseDamage;
        m_iAttackCount = _pSkillInfo.Option.targetingProfile.MaxTargets;   
        m_vDir = transform.forward;

        m_strSpawnKey = _strKey;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }

    public void SetOwner(Player _pPlayer) { m_pOwner = _pPlayer; }

    public void SetSpawnKey(string _strKey) { m_strSpawnKey = _strKey; }

    public void OnTriggerEnter(Collider other)
    {
        if ((m_pSkill.Option.targetingProfile.TargetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("타겟 레이어 충돌");
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

    private void effect(Rigidbody _pOther)
    {

    }
}
