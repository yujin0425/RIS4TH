using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (머리 위치 등)
    public float smoothTime = 0.3f; // 부드러운 이동을 위한 시간
    private Vector3 velocity = Vector3.zero; // 현재 속도 (SmoothDamp 사용 시 내부 상태)

    void Update()
    {
        if (target != null)
        {
            // 현재 위치를 목표 위치로 부드럽게 이동
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        }
    }
}
