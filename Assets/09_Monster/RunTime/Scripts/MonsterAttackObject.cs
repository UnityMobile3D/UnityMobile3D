using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackObject : MonoBehaviour, IPoolAble
{
    private Rigidbody m_pRigidbody = null;
    private MonsterSkillInfo m_pMonsterSkillInfo = null;

    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vDir = Vector3.zero;

    private float m_fLifeTime = 10.0f;
    private float m_fCurLifeTime = 0.0f;

    [SerializeField] bool m_bAccVel = false;

    public void Awake()
    {
        m_pRigidbody = GetComponent<Rigidbody>();
    }

    public void OnSpawn()
    {

    }
    public void OnDespawn()
    {

    }

    public void OnEnable()
    {
        m_fCurLifeTime = 0.0f;
        m_vDir = Vector3.zero;
    }

    public void Update()
    {
        m_fCurLifeTime += Time.deltaTime;
        if(m_fCurLifeTime >=m_fLifeTime )
        {
            //풀에 반납
            m_fCurLifeTime = 0.0f;

            ObjectPoolManager.m_Instance.PushObject
                (m_pMonsterSkillInfo.SOPoolEntry.prefabRef.AssetGUID, gameObject);
        }
    }

    public void FixedUpdate()
    {
        if (m_pRigidbody == null)
            return;

        if (m_bAccVel == true)
        {
            m_pRigidbody.AddForce(m_vDir.normalized * m_fMoveSpeed, ForceMode.Acceleration);
        }
        else
        {
            Vector3 vStep = m_vDir.normalized * m_fMoveSpeed * Time.fixedDeltaTime;
            m_pRigidbody.MovePosition(m_pRigidbody.position + vStep);
        }
    }


    public void SetInfo(MonsterSkillInfo _pSkillInfo)
    {
        m_pMonsterSkillInfo = _pSkillInfo;
        m_fLifeTime = _pSkillInfo.LifeTime;
        m_fMoveSpeed = _pSkillInfo.MoveSpeed;
        m_vDir = _pSkillInfo.AttackDir;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("플레이어 타격");
        }
    }
}

