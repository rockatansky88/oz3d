using UnityEngine;

/// <summary>
/// 미니맵 카메라가 플레이어를 따라다니도록 하는 스크립트
/// </summary>
public class MinimapCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;          // 추적할 대상 (플레이어)
    [SerializeField] private Vector3 offset = new Vector3(0, 50, 0); // 카메라 오프셋

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateWithTarget = true;  // 플레이어 회전에 따라 회전 여부

    void Start()
    {
        // target이 지정되지 않았으면 Player 태그로 찾기
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("MinimapCamera: Player를 찾을 수 없습니다. Player에 'Player' 태그를 설정하거나 Target을 수동으로 지정하세요.");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 플레이어 위치를 따라다님
        transform.position = target.position + offset;

        // 플레이어 회전에 따라 미니맵도 회전 (북쪽 고정 원하면 false)
        if (rotateWithTarget)
        {
            transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}