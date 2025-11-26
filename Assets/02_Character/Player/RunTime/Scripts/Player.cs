using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
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


public class Player : MonoBehaviour
{
   
    private Rigidbody   _rigidbody;
    private Animator    _animator;
    private Health      _health;


    private SkillRunner       _skillRunner; //이거 스킬 러너로 교체하기
    public Rigidbody RigidBody => _rigidbody;
    public Animator  Animator  => _animator;
    public SkillRunner SkillRunner => _skillRunner;

    // 이동 관련변수
    [Header("Move")]
    [SerializeField] private float Speed        = 2f;
    [SerializeField] private float rotateTime   = 0.1f;

    private Vector3     lastDir = Vector3.forward;
    private float       rotateVel;
    private Vector3     desiredPlanarVel;  

    [SerializeField] private List<PlayerEquipPoint> m_listEquipPoint = new List<PlayerEquipPoint>((int)eEquipType.Weapon);

    // 상태 관리
    const int           MAX_ATTACK = 2;
    private int         _attack = 0;
    private bool        _hit = false;
    private bool        _run = false;

    [Header("Hit")]
    private Coroutine _knockbackRoutine;
    [SerializeField] private float knockbackDamping = 3f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _skillRunner = GetComponent<SkillRunner>();
        // Rigidbody 세팅
        _rigidbody.useGravity = true;
        _rigidbody.drag = 0f;
        _rigidbody.angularDrag = 0f;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
        //우선권은 스킬한테
        if(_skillRunner.RunSkill != null)
        {
            _skillRunner.UpdateSkill();
        }

        else
        {
            if (_attack >= 1 || _hit == true)
                return;
            else
                MOVE();
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
        // 1) 입력
        Vector2 input = InputManager.m_Instance.ActionState.vDirection;

        //Vector2 input = moveAction.ReadValue<Vector2>();

        // 2) 평면 방향
        Vector3 moveDir = new Vector3(input.x, 0f, input.y);
        if (moveDir.sqrMagnitude > 0.0001f)
            lastDir = moveDir.normalized;
    
        // 3) 회전
        float targetY = Mathf.Atan2(lastDir.x, lastDir.z) * Mathf.Rad2Deg;
        float currentY = transform.eulerAngles.y;
        float smoothY = Mathf.SmoothDampAngle(currentY, targetY, ref rotateVel, rotateTime);
        transform.rotation = Quaternion.Euler(0f, smoothY, 0f);

        // 4) 목표 평면 속도(중력 분리)
        Vector3 planar = (moveDir.sqrMagnitude > 1f ? moveDir.normalized : moveDir) * Speed;
        desiredPlanarVel = planar;

        // 5) 애니메이션 파라미터 설정
        _animator.SetBool("isWalk", desiredPlanarVel != Vector3.zero);
    }

    public void ENDATTACK(int _iAttackCombo) 
    {
        if(_iAttackCombo < MAX_ATTACK)
            _attack -= _iAttackCombo;
        else if(_iAttackCombo>= MAX_ATTACK)
            _attack = 0;

        if(_attack == 0)
            _animator.SetBool("isAttack", false);
    }

   public void ATTACK()
    {
        ++_attack;
        _animator.SetBool("isAttack", true);
    }
   
    public void HIT(bool _bDown = false)
    {
        _hit = true;
        if(_bDown == true)
            _animator.SetTrigger("down");
        else
            _animator.SetTrigger("hit");

        if(_skillRunner.RunSkill != null)
            _skillRunner.OffSkill();
        
    }
    public void ENDHIT()
    {
        _hit = false;
    }
    

    private void FixedUpdate()
    {
        // 6) 이동 적용: y는 물리 중력 유지, xz만 갱신
        if(_hit == false)
        {
            Vector3 v = _rigidbody.velocity;
            v.x = desiredPlanarVel.x;
            v.z = desiredPlanarVel.z;
            _rigidbody.velocity = v;
        }
     
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MonsterAttack")
        {
            if (_hit == true)
                return;

            HIT();
            //플레이어 기준 가장 가까운 점
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            MonsterAttackObject pAttackObject = other.GetComponent<MonsterAttackObject>();
            effect(pAttackObject.MonsterSkillInfo, hitPoint);

            _health.TakeDamage((int)pAttackObject.Damage);
        }
    }
    private void effect(MonsterSkillInfo _skillInfo, in Vector3 _hitPosition)
    {
        // 기존 넉백 코루틴 돌고 있으면 정지
        if (_knockbackRoutine != null)
        {
            StopCoroutine(_knockbackRoutine);
            _knockbackRoutine = null;
        }

        Vector3 playerPos = _rigidbody.position;
        Vector3 dir = playerPos - _hitPosition;
        dir.y = 0.0f; 

        //거의 차이가 없다면 플레이어 반대 방향으로
        if (dir.sqrMagnitude < 0.0001f)
            dir = -transform.forward;
        
        dir.Normalize();

        float power = _skillInfo.AttackPower;

        // 위로 살짝 튕기게
        Vector3 force = dir * power;
     
        // 순간 넉백을 위해 기존 속도 잠깐 끊기
        _rigidbody.velocity = Vector3.zero;

        _rigidbody.AddForce(force, ForceMode.Impulse);
    
        transform.LookAt(_hitPosition);

        //시간이 지나면서 점점 멈추게 (속도 감쇠)
        _knockbackRoutine = StartCoroutine(knockbac_coroutine());
    }

    private IEnumerator knockbac_coroutine()
    {
        while (_rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            // velocity를 0쪽으로 점점 줄임
            _rigidbody.velocity = Vector3.Lerp(
                _rigidbody.velocity,
                Vector3.zero,
                Time.fixedDeltaTime
            );

            yield return null;
        }

        _rigidbody.velocity = Vector3.zero;
        _knockbackRoutine = null;
    }
}