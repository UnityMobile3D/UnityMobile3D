using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public enum eEquipType
{
    None,
    Hat,
    Top,
    Shoes,
    Weapon,
}
[Serializable]
public struct PlayerEquipPoint
{
    public eEquipType eEquipType;
    public Transform   EquipTransform;
}


public class Player : MonoBehaviour , IHealth
{

    [Header("Component")]
    private Rigidbody   _rigidbody;
    private Animator    _animator;
   
    private SkillRunner       _skillRunner; //이거 스킬 러너로 교체하기
    public Rigidbody RigidBody => _rigidbody;
    public Animator  Animator  => _animator;
    public SkillRunner SkillRunner => _skillRunner;

    [SerializeField] private ObjectInfo _playerInfo;
    public ObjectInfo PlayerInfo => _playerInfo;
    // 이동 관련변수
    [Header("Move")]
    //[SerializeField] private float Speed        = 2f;
    [SerializeField] private float rotateTime   = 0.1f;

    private float       rotateVel;
    Vector3 moveDir;

    [SerializeField] private List<PlayerEquipPoint> m_listEquipPoint = new List<PlayerEquipPoint>((int)eEquipType.Weapon);

    // 상태 관리
    private bool        _hit = false;
    private bool        _run = false;

    [Header("Hit")]
    private Coroutine _knockbackRoutine;
    [SerializeField] private float knockbackDamping = 3f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerInfo = GetComponent<ObjectInfo>();
        _skillRunner = GetComponent<SkillRunner>();

        // Rigidbody 세팅
        //_rigidbody.useGravity = true;
        //_rigidbody.drag = 0f;
        //_rigidbody.angularDrag = 0f;
        //_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        //_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Animator 세팅
        _animator = GetComponentInChildren<Animator>();
    }



    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_hit==true)
            return;

        //우선권은 스킬한테
        if(_skillRunner.RunSkill != null)
        {
            _skillRunner.UpdateSkill();
        }
        else
        {
            ROTATE();
        }

    }


    public Transform GetEquipPoint(eEquipType _eType)
    {
        for (int i = 0; i < m_listEquipPoint.Count; ++i)
        {
            if (m_listEquipPoint[i].eEquipType == _eType)
                return m_listEquipPoint[i].EquipTransform;
        }

        return null;
    }

    private void MOVE()
    {
        if (_hit == true || _skillRunner.RunSkill != null)
            return;

        if (moveDir == Vector3.zero)
        {
            // 걷기 애니메이션 끄고
            _animator.SetBool("isWalk", false);
            return;
        }

        Vector3 vStep = moveDir * _playerInfo.Speed * Time.fixedDeltaTime;
        Vector3 vPos = _rigidbody.position;

        transform.position = (vPos + vStep);
        //_rigidbody.MovePosition(vPos + vStep);
        _animator.SetBool("isWalk", true);
    }

    private void ROTATE()
    {
        Vector2 input = InputManager.m_Instance.ActionState.vDirection;
        moveDir = new Vector3(input.x, 0f, input.y);

        if (input == Vector2.zero)
            return;

        float targetY = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float currentY = transform.eulerAngles.y;
        float smoothY = Mathf.SmoothDampAngle(currentY, targetY, ref rotateVel, rotateTime);
        transform.rotation = Quaternion.Euler(0f, smoothY, 0f);
    }
  
    public void HIT(bool _bDown = false)
    {
        _hit = true;
        if(_bDown == true)
            _animator.SetTrigger("down");
        else
            _animator.SetTrigger("hit");

        if(_skillRunner.RunSkill != null)
            _skillRunner.CancelSkill();
        
    }
    public void ENDHIT()
    {
        _hit = false;
    }
    

    private void FixedUpdate()
    {
        MOVE();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void knockback(AttackInfo _attackInfo)
    {
        // 기존 넉백 코루틴 돌고 있으면 정지
        if (_knockbackRoutine != null)
        {
            StopCoroutine(_knockbackRoutine);
            _knockbackRoutine = null;
        }

        Vector3 vMonsterPos = _rigidbody.position;
        Vector3 vDir = vMonsterPos - _attackInfo.AttackerPosition;
        vDir.y = 0.0f;

        if (vDir.sqrMagnitude < 0.001f)
            vDir = -transform.forward;

        vDir.Normalize();

        transform.rotation = Quaternion.LookRotation(-vDir, Vector3.up);

        //시간이 지나면서 점점 멈추게 (속도 감쇠)
        _knockbackRoutine = StartCoroutine(knockback_coroutine(vDir, (int)_attackInfo.Power));
    }

    private IEnumerator knockback_coroutine(Vector3 _vDir, int _iPower)
    {
        float fElapsed = 0f;

        while (_hit == true && fElapsed <= 1.0f)
        {
            float fRevElaps = 1.0f - fElapsed;
            Vector3 vVelocity = _vDir * _iPower * fRevElaps;
            Vector3 vNextPos = _rigidbody.position + vVelocity * Time.fixedDeltaTime;

            _rigidbody.MovePosition(vNextPos);

            fElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        _knockbackRoutine = null;
    }



    //IHealth 구현 (healmanager에 플레이어 넣기)
    public int CurrentHP => _playerInfo.HP; 
    public int MaxHP => _playerInfo.MaxHp;

    public void TakeDamage(AttackInfo _pAttackInfo)
    {
        if (_hit == true)
            return;

        HIT();

        knockback(_pAttackInfo);

        _playerInfo.AddHP((int)_pAttackInfo.Damage * -1);
        HealthManager.m_Instance.PlayerTakeDamage();
    }
    public void Heal(int _iAmount)
    {
        _playerInfo.AddHP(_iAmount);
    }
    public bool IsDead()
    {
        return CurrentHP <= 0;
    }
}