using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private Vector3 spawnPosition;

    private Dictionary<EnemyStateType, EnemyState> states;
    private EnemyState currentState;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // 상태 초기화
        states = new Dictionary<EnemyStateType, EnemyState>
        {
            { EnemyStateType.Idle, new IdleState(this) },
            { EnemyStateType.Patrol, new PatrolState(this) }
        };
    }

    void Start()
    {
        spawnPosition = transform.position;
        ChangeState(EnemyStateType.Idle);
    }

    void Update()
    {
        currentState?.Update();
    }

    void FixedUpdate()
    {
        UpdateAnimation();
    }

    // 상태 변경
    public void ChangeState(EnemyStateType newStateType)
    {
        currentState?.Exit();
        currentState = states[newStateType];
        currentState.Enter();
    }

    // 이동 처리
    public void Move(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            // 이동
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            currentSpeed = moveSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    // 애니메이션 속도 설정
    public void SetAnimationSpeed(float speed)
    {
        currentSpeed = speed;
    }

    // 애니메이션 업데이트
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);

            // Walk 애니메이션: 속도가 0.1 이상일 때
            bool isWalking = currentSpeed >= 0.1f;
            animator.SetBool("isWalking", isWalking);
        }
    }

    // Gizmo로 정찰 범위 표시
    void OnDrawGizmos()
    {
        Vector3 center = Application.isPlaying ? spawnPosition : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, patrolRadius);

        // 현재 이동 방향 표시
        if (Application.isPlaying && currentSpeed > 0.1f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }
    }
}