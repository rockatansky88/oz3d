using UnityEngine;

/// <summary>
/// 1인칭/3인칭 카메라 전환 + 우클릭 시 마우스로 카메라 회전
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Camera Mode Settings")]
    [SerializeField] private Transform player;  // 플레이어 Transform

    // 3인칭 설정
    [SerializeField] private Vector3 thirdPersonPosition;
    [SerializeField] private Vector3 thirdPersonRotation;

    // 1인칭 설정
    [SerializeField] private Vector3 firstPersonPosition;
    [SerializeField] private Vector3 firstPersonRotation;

    [Header("Right Click Mouse Look Settings")]
    [SerializeField] private bool enableRightClickView = true;
    [SerializeField] private float mouseSensitivity = 2f;  // 마우스 감도
    [SerializeField] private float minVerticalAngle = -40f;  // 최소 수직 각도 (아래)
    [SerializeField] private float maxVerticalAngle = 60f;   // 최대 수직 각도 (위)

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 5f;  // 1/3인칭 전환 속도

    private bool isFirstPerson = false;  // 현재 1인칭 모드인지
    private bool isRightClickActive = false;  // 우클릭 중인지

    private Vector3 targetPosition;
    private Vector3 targetRotation;
    private Vector3 baseTargetPosition;  // 우클릭 전 기본 위치

    // 마우스 회전 관련
    private float mouseX = 0f;
    private float mouseY = 0f;
    private Vector3 currentMouseRotation;  // 현재 마우스로 조작한 회전

    void Start()
    {
        if (player == null)
        {
            player = transform.parent;
        }

        // 초기 상태는 3인칭
        targetPosition = thirdPersonPosition;
        targetRotation = thirdPersonRotation;
        baseTargetPosition = targetPosition;
        currentMouseRotation = targetRotation;
    }

    void Update()
    {
        HandleCameraToggle();   // Q키로 1인칭/3인칭 전환
        HandleRightClickView(); // 우클릭 시 마우스로 카메라 회전
    }

    void LateUpdate()
    {
        SmoothCameraTransition(); // 부드러운 카메라 전환
    }

    // Q키로 1인칭/3인칭 전환
    private void HandleCameraToggle()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                baseTargetPosition = firstPersonPosition;
                targetRotation = firstPersonRotation;
                Debug.Log("1인칭 모드");
            }
            else
            {
                baseTargetPosition = thirdPersonPosition;
                targetRotation = thirdPersonRotation;
                Debug.Log("3인칭 모드");
            }

            // 우클릭 중이 아니면 바로 적용
            if (!isRightClickActive)
            {
                targetPosition = baseTargetPosition;
                currentMouseRotation = targetRotation;
                // 마우스 회전 누적값 초기화
                mouseX = 0f;
                mouseY = 0f;
            }
        }
    }

    // 우클릭 시 마우스로 카메라 회전
    private void HandleRightClickView()
    {
        if (!enableRightClickView) return;

        // 우클릭 시작
        if (Input.GetMouseButtonDown(1))
        {
            isRightClickActive = true;
            // 현재 회전을 기준으로 초기화
            currentMouseRotation = targetRotation;
            Cursor.lockState = CursorLockMode.Locked;  // 마우스 커서 잠금
            Cursor.visible = false;  // 커서 숨김
        }

        // 우클릭 중 - 마우스 움직임으로 카메라 회전
        if (Input.GetMouseButton(1))
        {
            // 마우스 입력 받기
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            // 수직 각도 제한
            mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

            // 기본 회전에 마우스 회전 추가
            currentMouseRotation = new Vector3(
                targetRotation.x + mouseY,
                targetRotation.y + mouseX,
                0
            );
        }

        // 우클릭 해제 - 현재 회전 상태를 저장
        if (Input.GetMouseButtonUp(1))
        {
            isRightClickActive = false;

            // ⭐ 현재 회전을 새로운 기본 회전으로 저장 (각도 유지)
            targetRotation = currentMouseRotation;

            // 위치만 복구
            targetPosition = baseTargetPosition;

            // 마우스 누적값 초기화 (다음 우클릭을 위해)
            mouseX = 0f;
            mouseY = 0f;

            Cursor.lockState = CursorLockMode.None;  // 마우스 커서 잠금 해제
            Cursor.visible = true;  // 커서 표시
        }
    }

    // 부드러운 카메라 전환
    private void SmoothCameraTransition()
    {
        // 위치 보간
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * transitionSpeed);

        // 회전 보간 (우클릭 중이면 마우스 회전 사용)
        Vector3 finalRotation = isRightClickActive ? currentMouseRotation : targetRotation;
        Quaternion targetQuat = Quaternion.Euler(finalRotation);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetQuat, Time.deltaTime * transitionSpeed);
    }

    // Inspector에서 설정값을 바로 적용 (테스트용)
    void OnValidate()
    {
        if (Application.isPlaying && !isRightClickActive)
        {
            if (isFirstPerson)
            {
                baseTargetPosition = firstPersonPosition;
                targetRotation = firstPersonRotation;
            }
            else
            {
                baseTargetPosition = thirdPersonPosition;
                targetRotation = thirdPersonRotation;
            }
            targetPosition = baseTargetPosition;
        }
    }
}