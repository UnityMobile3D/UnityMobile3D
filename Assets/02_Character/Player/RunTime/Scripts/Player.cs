using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Context
    InputManager action;
    InputAction moveAction;

    private Rigidbody _rigidbody;
    private Animator _animator;

    // 이동 관련변수
    [Header("Move")]
    [SerializeField] private float Speed        = 2f;
    [SerializeField] private float rotateTime   = 0.1f;

    private Vector3     lastDir = Vector3.forward;
    private float       rotateVel;
    private Vector3     desiredPlanarVel;   // 뭐하는 역할?

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // Rigidbody 세팅
        _rigidbody.useGravity = true;
        _rigidbody.drag = 0f;
        _rigidbody.angularDrag = 0f;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Animator 세팅
        _animator = GetComponentInChildren<Animator>();

        // Input
        action = new InputManager();
    }

    private void OnEnable()
    {
        //moveAction.Enable();
    }

    private void OnDisable()
    {
        //moveAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //MOVE();
    }

    private void MOVE()
    {
        // 1) 입력
        Vector2 input = moveAction.ReadValue<Vector2>();

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
        _animator.SetBool("isRun", desiredPlanarVel != Vector3.zero);

    }
    private void FixedUpdate()
    {
        // 6) 이동 적용: y는 물리 중력 유지, xz만 갱신
        Vector3 v = _rigidbody.velocity;
        v.x = desiredPlanarVel.x;
        v.z = desiredPlanarVel.z;
        _rigidbody.velocity = v;
    }

}