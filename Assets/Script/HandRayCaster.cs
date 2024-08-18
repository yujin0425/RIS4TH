using NRKernal;
using UnityEngine;
using TMPro;  // TextMeshPro ���ӽ����̽��� ���Խ�ŵ�ϴ�.
using System;

public class HandRayCaster : MonoBehaviour
{
    public HandEnum hand = HandEnum.RightHand; // ����� ��
    public float rayLength = 10.0f;            // Ray�� ����
    public LayerMask rayLayerMask;             // Ray�� �浹�� ���̾�

    // �浹 ������ ������Ʈ�� �� �߻��ϴ� �̺�Ʈ
    public event Action<Vector3> OnHitPointUpdated;

    public TextMeshProUGUI hitPointText; // TextMeshProUGUI �ʵ带 �߰��մϴ�.

    void Update()
    {
        // ���� �ε��� �հ��� ���� ���� ��ġ ��������
        Vector3? indexTipPosition = GetIndexJointPosition(hand);

        // ���� �������� ������ ����
        if (indexTipPosition == null) return;

        // �ε��� �հ��� ���� ��ġ�κ��� �������� Ray �߻�
        Ray ray = new Ray(indexTipPosition.Value, GetIndexJointForward(hand));

        // Ray�� �ð�ȭ (������)
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        // Raycast ����
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength, rayLayerMask))
        {
            // Ray�� �浹�� ������Ʈ�� ���� �� ó��
            Debug.Log($"Ray hit: {hit.collider.gameObject.name}");

            // �浹 ������ ��ǥ�� ���մϴ�.
            Vector3 hitPoint = hit.point;
            Debug.Log($"Hit point: {hitPoint}");

            // �浹 �������� �߰� �۾��� ������ �� �ֽ��ϴ�.
            HandleHitPoint(hitPoint);

            // �浹 ������ ������Ʈ�Ǿ����� �˸�
            OnHitPointUpdated?.Invoke(hitPoint);

            // UI �ؽ�Ʈ ������Ʈ
            UpdateHitPointText(hitPoint);
        }
    }

    private Vector3? GetIndexJointPosition(HandEnum hand)
    {
        // ������ ���� ���¸� ������
        HandState handState = NRInput.Hands.GetHandState(hand);

        // ���� �������� ������ null ��ȯ
        if (!handState.isTracked) return null;

        // �ε��� �հ��� ���� ���� ��ġ ��ȯ
        return handState.GetJointPose(HandJointID.IndexTip).position;
    }

    private Vector3 GetIndexJointForward(HandEnum hand)
    {
        // ������ ���� ���¸� ������
        HandState handState = NRInput.Hands.GetHandState(hand);

        // ���� �������� ������ Vector3.zero ��ȯ (�� ��� ray �߻� �ȵ�)
        if (!handState.isTracked) return Vector3.zero;

        // �ε��� �հ��� ���� ���� ���� ��ȯ
        return handState.GetJointPose(HandJointID.IndexTip).rotation * Vector3.forward;
    }

    private void HandleHitPoint(Vector3 hitPoint)
    {
        // �浹 �������� �߰� �۾��� �����մϴ�.
        // ���� ���, �浹 ������ ������Ʈ�� �����ϰų�, �ð��� ȿ���� �߰��� �� �ֽ��ϴ�.

        // �Ʒ��� ���÷�, �浹 ������ ���� ���� �����ϴ� �ڵ��Դϴ�.
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hitPoint;
        sphere.transform.localScale = Vector3.one * 0.1f; // ���� ũ�� ����
    }

    private void UpdateHitPointText(Vector3 hitPoint)
    {
        if (hitPointText != null)
        {
            // ��ǥ�� (x, y, z) �������� �������Ͽ� UI �ؽ�Ʈ�� ������Ʈ�մϴ�.
            hitPointText.text = $"Hit Point: ({hitPoint.x:F2}, {hitPoint.y:F2}, {hitPoint.z:F2})";
            Debug.Log($"Updated UI with hit point: ({hitPoint.x:F2}, {hitPoint.y:F2}, {hitPoint.z:F2})");
        }
        else
        {
            Debug.LogError("hitPointText is not assigned in the inspector!");
        }
    }
}
