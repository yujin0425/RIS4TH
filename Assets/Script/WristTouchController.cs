using NRKernal;
using UnityEngine;

public class WristTouchController : MonoBehaviour
{
    public GameObject menuObject; // ǥ���� �޴� ������Ʈ
    private bool isMenuVisible = false; // �޴��� ���� ǥ�õǰ� �ִ��� ����
    private bool isTouching = false; // ��ġ ������ Ȯ��
    private float touchDuration = 0f; // ��ġ�� ���ӵ� �ð�
    private float requiredTouchTime = 3f; // �ʿ��� ��ġ �ð� (3��)

    void Start()
    {
        // �� ���� �� �޴��� ��Ȱ��ȭ�մϴ�.
        menuObject.SetActive(false);
        isMenuVisible = false;
    }

    void Update()
    {
        if (NRInput.Hands.IsRunning)
        {
            // �޼��� �ո� ��ġ�� ������
            Pose leftWristPose = NRInput.Hands.GetHandState(HandEnum.LeftHand).GetJointPose(HandJointID.Wrist);

            // ������ �հ����� �޼� �ո� ��Ҵ��� Ȯ��
            isTouching = IsTouchingWrist(leftWristPose.position);

            if (isTouching)
            {
                // ��ġ�� �����Ǹ� �ð��� ������Ŵ
                touchDuration += Time.deltaTime;

                // ��ġ �ð��� �ʿ��� �ð��� �ʰ��ϸ� �޴� ���� ��ȯ
                if (touchDuration >= requiredTouchTime)
                {
                    ToggleMenu();
                    touchDuration = 0f; // ��ġ �ð��� ����
                }
            }
            else
            {
                // ��ġ�� �ߴܵǸ� �ð��� ����
                touchDuration = 0f;
            }
        }

        // �����: �ֿܼ� ��ġ ���ο� ���� �ð��� ���
        Debug.Log($"Touching: {isTouching}, Touch Duration: {touchDuration}");
    }

    private bool IsTouchingWrist(Vector3 wristPosition)
    {
        // ������ �հ����� �޼� �ո� ��ó�� �ִ��� Ȯ���ϴ� ����
        float touchRadius = 0.05f; // 5cm �ݰ�
        foreach (HandJointID jointID in new[] { HandJointID.IndexTip, HandJointID.MiddleTip, HandJointID.RingTip, HandJointID.PinkyTip })
        {
            Pose rightFingerPose = NRInput.Hands.GetHandState(HandEnum.RightHand).GetJointPose(jointID);
            if (Vector3.Distance(rightFingerPose.position, wristPosition) < touchRadius)
            {
                return true;
            }
        }
        return false;
    }

    private void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        menuObject.SetActive(isMenuVisible);
        Debug.Log($"Menu is now {(isMenuVisible ? "ON" : "OFF")}");
    }
}
