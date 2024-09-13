using System;
using UnityEngine;
using NRKernal;

// �յ��� ������ ��� ����ü
public struct HandGestureData
{
    public bool isFist;
    public bool isThumbsUp;
    public bool isThumbsDown;

    // ������
    public HandGestureData(bool fist, bool thumbsUp, bool thumbsDown)
    {
        this.isFist = fist;
        this.isThumbsUp = thumbsUp;
        this.isThumbsDown = thumbsDown;
    }

    // �յ��� ���¸� ������Ʈ
    public void UpdateGesture(bool fist, bool thumbsUp, bool thumbsDown)
    {
        this.isFist = fist;
        this.isThumbsUp = thumbsUp;
        this.isThumbsDown = thumbsDown;
    }

    // �յ��� ���¸� ��� (����׿�)
    public void PrintGestureStatus()
    {
        Debug.Log("Fist: " + isFist + ", Thumbs Up: " + isThumbsUp + ", Thumbs Down: " + isThumbsDown);
    }
}

// �յ����� �ν��ϰ� ó���ϴ� Ŭ����
public class HandGesture : MonoBehaviour
{
    // �յ��� �����͸� ��� ����ü �ν��Ͻ�
    private HandGestureData handGestureData;

    void Start()
    {
        // �ʱⰪ ���� (��� �յ����� false�� ����)
        handGestureData = new HandGestureData(false, false, false);
    }

    void Update()
    {
        if (NRInput.Hands.IsRunning)
        {
            // �յ��� �����͸� ������Ʈ
            UpdateHandGesture();

            // ����׿� ���
            handGestureData.PrintGestureStatus();
        }
    }

    // �յ��� �����͸� ������Ʈ�ϴ� �Լ�
    private void UpdateHandGesture()
    {
        // �޼հ� �������� �յ����� ������
        var leftHandState = NRInput.Hands.GetHandState(HandEnum.LeftHand);
        var rightHandState = NRInput.Hands.GetHandState(HandEnum.RightHand);

        // Fist, Thumbs Up, Thumbs Down �ν� (�� ���ÿ����� ������ ��� �հ����� ���������� �Ǵ�)
        bool isFist = IsFistGesture(leftHandState) || IsFistGesture(rightHandState);
        bool isThumbsUp = IsThumbsUpGesture(leftHandState) || IsThumbsUpGesture(rightHandState);
        bool isThumbsDown = IsThumbsDownGesture(leftHandState) || IsThumbsDownGesture(rightHandState);

        // ����ü�� �յ��� ���� ������Ʈ
        handGestureData.UpdateGesture(isFist, isThumbsUp, isThumbsDown);
    }

    // Fist �յ����� �ν��ϴ� �Լ�
    private bool IsFistGesture(HandState handState)
    {
        // �⺻������ Fist�� ��� �հ����� ���� ���� ����
        return handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }

    // Thumbs Up �յ����� �ν��ϴ� �Լ�
    private bool IsThumbsUpGesture(HandState handState)
    {
        // Thumbs Up�� ������ ���� ���ϰ� ������ �հ����� ���� ���� ����
        return handState.GetJointPose(HandJointID.ThumbTip).position.y > handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }

    // Thumbs Down �յ����� �ν��ϴ� �Լ�
    private bool IsThumbsDownGesture(HandState handState)
    {
        // Thumbs Down�� ������ �Ʒ��� ���ϰ� ������ �հ����� ���� ���� ����
        return handState.GetJointPose(HandJointID.ThumbTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.IndexTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.MiddleTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.RingTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y &&
               handState.GetJointPose(HandJointID.PinkyTip).position.y < handState.GetJointPose(HandJointID.Wrist).position.y;
    }
}
