using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;
using UnityEngine.Events;

public class MonsterAttackObject : MonoBehaviour, IPoolAble
{
    protected Monster m_pOwner = null;

    private Rigidbody m_pRigidbody = null;
    private Collider m_pCollider = null;

    private MonsterSkillInfo m_pMonsterSkillInfo = null;
    public MonsterSkillInfo MonsterSkillInfo => m_pMonsterSkillInfo;

    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vDir = Vector3.zero;

    [SerializeField] private float m_fLifeTime = 10.0f;
    [SerializeField] private float m_fAttackTime = 0.0f;
    [SerializeField] private float m_fEndAttackTime = float.MaxValue;

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
        if(m_pMonsterSkillInfo != null && m_pMonsterSkillInfo.EndFrameSpawn.SpawnObject != null)
        {
            SOPoolEntry pSpawnEntry = m_pMonsterSkillInfo.EndFrameSpawn.SpawnObject;

            GameObject pSpawnObj = ObjectPoolManager.m_Instance.GetObject(pSpawnEntry.prefabRef.AssetGUID,
                transform.position, Vector3.zero);

            if(pSpawnObj.TryGetComponent<MonsterAttackObject>(out var pAttackObj) == true)
                pAttackObj.SetSpawnKey(pSpawnEntry.prefabRef.AssetGUID);
        }

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
        if(m_fCurLifeTime >=m_fLifeTime )
        {
            //풀에 반납
            m_fCurLifeTime = 0.0f;

            PushPoolObject();
        }


        if (m_fCurLifeTime >= m_fAttackTime)
            m_pCollider.enabled = true;
        else if(m_fCurLifeTime >= m_fEndAttackTime)
            m_pCollider.enabled = false;
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

    public void SetInfo(MonsterSkillInfo _pSkillInfo)
    {
        if (m_pMonsterSkillInfo != null)
            return;

        m_pMonsterSkillInfo = _pSkillInfo;
        m_fLifeTime = _pSkillInfo.LifeTime;
        m_fAttackTime = _pSkillInfo.AttackTime;
        m_fMoveSpeed = _pSkillInfo.MoveSpeed;
        m_vDir = _pSkillInfo.AttackDir;
        m_fDamage = _pSkillInfo.AttackDamage;

        m_strSpawnKey = _pSkillInfo.SOPoolEntry.prefabRef.AssetGUID;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }

    public void SetOwner(Monster _pMonster){m_pOwner = _pMonster;}

    public void SetSpawnKey(string _strKey){m_strSpawnKey = _strKey;}

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (check_attack_count() == true)
                PushPoolObject();
        }
      
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            effect(other.GetComponent<Rigidbody>());

            if (check_attack_count() == true)
                PushPoolObject();
        }
    }

    private bool check_attack_count()
    {
        ++m_iCurAttackCount;
        if(m_iCurAttackCount >= m_iAttackCount)
            return true;

        return false;
    }

    private void effect(Rigidbody _pOther)
    {
        
    }
}

