using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target; // ���� ��� (�Ӹ� ��ġ ��)
    public float smoothTime = 0.3f; // �ε巯�� �̵��� ���� �ð�
    private Vector3 velocity = Vector3.zero; // ���� �ӵ� (SmoothDamp ��� �� ���� ����)

    void Update()
    {
        if (target != null)
        {
            // ���� ��ġ�� ��ǥ ��ġ�� �ε巴�� �̵�
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        }
    }
}
