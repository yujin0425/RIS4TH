using System;
using UnityEngine;
using NRKernal;

// 손동작 정보를 담는 구조체
public struct HandGestureData
{
    public bool isFist;
    public bool isThumbsUp;
    public bool isThumbsDown;

    // 생성자
    public HandGestureData(bool fist, bool thumbsUp, bool thumbsDown)
    {
        this.isFist = fist;
        this.isThumbsUp = thumbsUp;
        this.isThumbsDown = thumbsDown;
    }

    // 손동작 상태를 업데이트
    public void UpdateGesture(bool fist, bool thumbsUp, bool thumbsDown)
    {
        this.isFist = fist;
        this.isThumbsUp = thumbsUp;
        this.isThumbsDown = thumbsDown;
    }

    // 손동작 상태를 출력 (디버그용)
    public void PrintGestureStatus()
    {
        Debug.Log("Fist: " + isFist + ", Thumbs Up: " + isThumbsUp + ", Thumbs Down: " + isThumbsDown);
    }
}

// 손동작을 인식하고 처리하는 클래스
public class HandGesture : MonoBehaviour
{
    // 손동작 데이터를 담는 구조체 인스턴스
    private HandGestureData handGestureData;

    void Start()
    {
        // 초기값 설정 (모든 손동작을 false로 설정)
        handGestureData = new HandGestureData(false, false, false);
    }

    void Update()
    {
        if (NRInput.Hands.IsRunning)
        {
            // 손동작 데이터를 업데이트
            UpdateHandGesture();

            // 디버그용 출력
            handGestureData.PrintGestureStatus();
        }
    }

    // 손동작 데이터를 업데이트하는 함수
    private void UpdateHandGesture()
    {
        // 왼손과 오른손의 손동작을 가져옴
        var leftHandState = NRInput.Hands.GetHandState(HandEnum.LeftHand);
        var rightHandState = NRInput.Hands.GetHandState(HandEnum.RightHand);

        // Fist, Thumbs Up, Thumbs Down 인식 (이 예시에서는 간단히 모든 손가락이 접혔는지로 판단)
        bool isFist = IsFistGesture(leftHandState) || IsFistGesture(rightHandState);
        bool isThumbsUp = IsThumbsUpGesture(leftHandState) || IsThumbsUpGesture(rightHandState);
        bool isThumbsDown = IsThumbsDownGesture(leftHandState) || IsThumbsDownGesture(rightHandState);

        // 구조체에 손동작 상태 업데이트
        handGestureData.UpdateGesture(isFist, isThumbsUp, isThumbsDown);
    }

    // Fist 손동작을 인식하는 함수
    private bool IsFistGesture(HandState handState)
    {
        // 기본적으로 Fist는 모든 손가락이 접힌 경우로 간주
        return handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }

    // Thumbs Up 손동작을 인식하는 함수
    private bool IsThumbsUpGesture(HandState handState)
    {
        // Thumbs Up은 엄지가 위로 향하고 나머지 손가락이 접힌 경우로 간주
        return handState.GetJointPose(HandJointID.ThumbTip).position.y > handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }

    // Thumbs Down 손동작을 인식하는 함수
    private bool IsThumbsDownGesture(HandState handState)
    {
        // Thumbs Down은 엄지가 아래로 향하고 나머지 손가락이 접힌 경우로 간주
        return handState.GetJointPose(HandJointID.ThumbTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }
}
