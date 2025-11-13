using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Animator animator;
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5.0f;          // 걷기 기본 속도
    [SerializeField] private float runMultiplier = 2.0f;  // 달리기 속도 배율
    [SerializeField] private float rotationSpeed = 500f;  // 회전 속도 (초당 각도)


    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;        // 점프 힘

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;     // 바닥 체크를 위한 위치 트랜스폼
    [SerializeField] private float groundCheckRadius = 0.2f; // 바닥 체크 구체의 반지름
    [SerializeField] private LayerMask groundLayerMask; // 바닥으로 간주할 레이어
    private bool isGrounded;                           // 바닥에 닿았는지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 🔍 디버그 4: GroundCheck 설정 확인
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform is NOT assigned!");
        }
        else
        {
            Debug.Log($"GroundCheck assigned: {groundCheck.name}, Position: {groundCheck.position}");
        }
    }

    void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

    }

    void FixedUpdate()
    {

        // 1. 바닥에 닿아있는지 확인
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayerMask);
        animator.SetBool("isGrounded", isGrounded); // 애니메이터 isGrounded 파라미터 업데이트

        // 2. 이동 및 회전 입력 처리
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // 3. 이동 속도 및 방향 계산
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        float currentSpeed = speed;
        if (isRunning && verticalInput != 0)
        {
            currentSpeed *= runMultiplier;
        }

        // 4. 회전 처리 (먼저 회전을 적용)
        if (horizontalInput != 0)
        {
            float rotation = horizontalInput * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, rotation, 0);
        }

        // 5. 물리 기반 이동 적용
        Vector3 moveDirection = transform.forward * verticalInput * currentSpeed;

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // 6. 애니메이션 파라미터 설정
        float animatorSpeed = Mathf.Abs(verticalInput) * currentSpeed;
        animator.SetFloat("Speed", animatorSpeed);
    }

    // Scene View에서 Ground Check 범위를 시각적으로 표시
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}