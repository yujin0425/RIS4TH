using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI를 위해 추가

namespace NRKernal.NRExamples
{
    public class PlaneDetector : MonoBehaviour
    {
        public GameObject DetectedPlanePrefab;
        private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

        // 추가: 리셋 버튼 참조
        public Button resetButton;

        private void Start()
        {
            // 리셋 버튼에 클릭 이벤트 추가
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(ResetPlanes);
            }
        }

        public void Update()
        {
            NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<NRTrackableBehaviour>().Initialize(m_NewPlanes[i]);
            }
        }

        // 추가: 감지된 평면을 리셋하는 메서드
        public void ResetPlanes()
        {
            // 감지된 모든 평면 오브젝트 삭제
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            // 리스트 초기화
            m_NewPlanes.Clear();
            Debug.Log("Planes reset.");
        }
    }
}
