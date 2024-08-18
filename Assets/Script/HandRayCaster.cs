using NRKernal;
using UnityEngine;
using TMPro;  // TextMeshPro 네임스페이스를 포함시킵니다.
using System;

public class HandRayCaster : MonoBehaviour
{
    public HandEnum hand = HandEnum.RightHand; // 사용할 손
    public float rayLength = 10.0f;            // Ray의 길이
    public LayerMask rayLayerMask;             // Ray가 충돌할 레이어

    // 충돌 지점이 업데이트될 때 발생하는 이벤트
    public event Action<Vector3> OnHitPointUpdated;

    public TextMeshProUGUI hitPointText; // TextMeshProUGUI 필드를 추가합니다.

    void Update()
    {
        // 손의 인덱스 손가락 끝의 관절 위치 가져오기
        Vector3? indexTipPosition = GetIndexJointPosition(hand);

        // 손이 추적되지 않으면 리턴
        if (indexTipPosition == null) return;

        // 인덱스 손가락 끝의 위치로부터 전방으로 Ray 발사
        Ray ray = new Ray(indexTipPosition.Value, GetIndexJointForward(hand));

        // Ray를 시각화 (디버깅용)
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        // Raycast 수행
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength, rayLayerMask))
        {
            // Ray가 충돌한 오브젝트가 있을 때 처리
            Debug.Log($"Ray hit: {hit.collider.gameObject.name}");

            // 충돌 지점의 좌표를 구합니다.
            Vector3 hitPoint = hit.point;
            Debug.Log($"Hit point: {hitPoint}");

            // 충돌 지점에서 추가 작업을 수행할 수 있습니다.
            HandleHitPoint(hitPoint);

            // 충돌 지점이 업데이트되었음을 알림
            OnHitPointUpdated?.Invoke(hitPoint);

            // UI 텍스트 업데이트
            UpdateHitPointText(hitPoint);
        }
    }

    private Vector3? GetIndexJointPosition(HandEnum hand)
    {
        // 지정된 손의 상태를 가져옴
        HandState handState = NRInput.Hands.GetHandState(hand);

        // 손이 추적되지 않으면 null 반환
        if (!handState.isTracked) return null;

        // 인덱스 손가락 끝의 관절 위치 반환
        return handState.GetJointPose(HandJointID.IndexTip).position;
    }

    private Vector3 GetIndexJointForward(HandEnum hand)
    {
        // 지정된 손의 상태를 가져옴
        HandState handState = NRInput.Hands.GetHandState(hand);

        // 손이 추적되지 않으면 Vector3.zero 반환 (이 경우 ray 발사 안됨)
        if (!handState.isTracked) return Vector3.zero;

        // 인덱스 손가락 끝의 관절 방향 반환
        return handState.GetJointPose(HandJointID.IndexTip).rotation * Vector3.forward;
    }

    private void HandleHitPoint(Vector3 hitPoint)
    {
        // 충돌 지점에서 추가 작업을 수행합니다.
        // 예를 들어, 충돌 지점에 오브젝트를 생성하거나, 시각적 효과를 추가할 수 있습니다.

        // 아래는 예시로, 충돌 지점에 작은 구를 생성하는 코드입니다.
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hitPoint;
        sphere.transform.localScale = Vector3.one * 0.1f; // 구의 크기 조절
    }

    private void UpdateHitPointText(Vector3 hitPoint)
    {
        if (hitPointText != null)
        {
            // 좌표를 (x, y, z) 형식으로 포맷팅하여 UI 텍스트를 업데이트합니다.
            hitPointText.text = $"Hit Point: ({hitPoint.x:F2}, {hitPoint.y:F2}, {hitPoint.z:F2})";
            Debug.Log($"Updated UI with hit point: ({hitPoint.x:F2}, {hitPoint.y:F2}, {hitPoint.z:F2})");
        }
        else
        {
            Debug.LogError("hitPointText is not assigned in the inspector!");
        }
    }
}
