using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
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
    private NavMeshAgent _agent;
   
    private SkillRunner       _skillRunner; 
    public Rigidbody RigidBody => _rigidbody;
    public Animator  Animator  => _animator;
    public SkillRunner SkillRunner => _skillRunner;

    public NavMeshAgent Agent => _agent;

    [SerializeField] private ObjectInfo _playerInfo;
    public ObjectInfo PlayerInfo => _playerInfo;
    // 이동 관련변수
    [Header("Move")]
    [SerializeField] private LayerMask _moveMask;
    [SerializeField] private float rotateTime   = 0.1f;

    private float       rotateVel;
    private Vector3     moveDir;

    [SerializeField] private List<PlayerEquipPoint> m_listEquipPoint = new List<PlayerEquipPoint>((int)eEquipType.Weapon);

    // 상태 관리
    private bool        _hit = false;
    private bool        _run = false;
    private bool        _navRun = false;

    [Header("Hit")]
    private Coroutine _knockbackRoutine;
    [SerializeField] private float knockbackDamping = 3f;

    public Func<AttackInfo, bool> m_pHitEvent;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _playerInfo = GetComponent<ObjectInfo>();
        _skillRunner = GetComponent<SkillRunner>();

        _agent.speed = _playerInfo.Speed;
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
            RESETAGENT();

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
        if (moveDir == Vector3.zero && (_agent.velocity.sqrMagnitude < 0.0001f))
        {
            // 걷기 애니메이션 끄고
            _animator.SetBool("isWalk", false);
            return;
        }
        else
            _animator.SetBool("isWalk", true);

        //패드가 우선
        if (moveDir != Vector3.zero)
            RESETAGENT();
        else if (_navRun == true)
            return;
        

        Vector3 vStep = moveDir * _playerInfo.Speed * Time.fixedDeltaTime;
        Vector3 vPos = _rigidbody.position;

        transform.position = (vPos + vStep);
        _animator.SetBool("isWalk", true);
        
    }

    private void RESETAGENT()
    {
       _agent.ResetPath();

        _navRun = false;
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
  
    //Touch Callback
    public void NavigationMove()
    {
        Vector2 screenPoss = InputManager.m_Instance.PointerState.vScreenPos;
        Ray ray = Camera.main.ScreenPointToRay(screenPoss);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 200.0f, _moveMask))
        {
            const float MaxDistance = 10.0f;
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit,
                   MaxDistance, NavMesh.AllAreas))
            {
                _navRun = true;
                _agent.SetDestination(navHit.position);
            }
        }
    }

    //private bool IsArrived()
    //{
    //    if (_agent.pathPending == true)
    //        return false;

    //    // 경로가 없으면 도착했다고 볼 수도 있지만 일반적으로 false 처리
    //    if (_agent.hasPath == false)
    //        return false;

    //    if (_agent.remainingDistance > _agent.stoppingDistance)
    //        return false;

    //    // 남은 거리는 stoppingDistance 이하이지만,
    //    // 아직 속도가 줄지 않고 있다면 '완전 도착'은 아님
    //    if (_agent.velocity.sqrMagnitude > 0.001f)
    //        return false;

    //    //목적지 도달
    //    return true;
    //}
    public void HIT(bool _bDown = false)
    {
        _hit = true;

        RESETAGENT();
        if (_bDown == true)
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
        if (_hit == true || _skillRunner.RunSkill != null)
            return;

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

        Vector3 vPlayerPos = _rigidbody.position;
        Vector3 vDir = vPlayerPos - _attackInfo.HitPoint;
        vDir.y = 0.0f;

        if (vDir.sqrMagnitude < 0.01f)
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

        //쉴드나 여러 이벤트 관리하기 위해서 bool타입 매개변수를 넣으면 코드가 더러워질 수 있음
        if (m_pHitEvent?.Invoke(_pAttackInfo) == false)
            return;

       

        HIT();

        knockback(_pAttackInfo);

        _playerInfo.AddHP((int)_pAttackInfo.Damage * -1);
        HealthManager.m_Instance.UpdateHP();
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