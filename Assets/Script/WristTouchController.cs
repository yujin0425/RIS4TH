using NRKernal;
using UnityEngine;

public class WristTouchController : MonoBehaviour
{
    public GameObject menuObject; // 표시할 메뉴 오브젝트
    private bool isMenuVisible = false; // 메뉴가 현재 표시되고 있는지 여부
    private bool isTouching = false; // 터치 중인지 확인
    private float touchDuration = 0f; // 터치가 지속된 시간
    private float requiredTouchTime = 3f; // 필요한 터치 시간 (3초)

    void Start()
    {
        // 앱 시작 시 메뉴를 비활성화합니다.
        menuObject.SetActive(false);
        isMenuVisible = false;
    }

    void Update()
    {
        if (NRInput.Hands.IsRunning)
        {
            // 왼손의 손목 위치를 가져옴
            Pose leftWristPose = NRInput.Hands.GetHandState(HandEnum.LeftHand).GetJointPose(HandJointID.Wrist);

            // 오른손 손가락이 왼손 손목에 닿았는지 확인
            isTouching = IsTouchingWrist(leftWristPose.position);

            if (isTouching)
            {
                // 터치가 감지되면 시간을 증가시킴
                touchDuration += Time.deltaTime;

                // 터치 시간이 필요한 시간을 초과하면 메뉴 상태 전환
                if (touchDuration >= requiredTouchTime)
                {
                    ToggleMenu();
                    touchDuration = 0f; // 터치 시간을 리셋
                }
            }
            else
            {
                // 터치가 중단되면 시간을 리셋
                touchDuration = 0f;
            }
        }

        // 디버깅: 콘솔에 터치 여부와 지속 시간을 출력
        Debug.Log($"Touching: {isTouching}, Touch Duration: {touchDuration}");
    }

    private bool IsTouchingWrist(Vector3 wristPosition)
    {
        // 오른손 손가락이 왼손 손목 근처에 있는지 확인하는 로직
        float touchRadius = 0.05f; // 5cm 반경
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
