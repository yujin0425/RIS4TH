using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI 버튼을 사용하기 위해 추가

namespace NRKernal.NRExamples
{
    /// <summary> 평면 감지를 제어하고 초기화할 수 있는 클래스입니다. </summary>
    public class PlaneDetectionController : MonoBehaviour
    {
        /// <summary> 감지된 평면에 대한 프리팹입니다. </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary> 평면 초기화 버튼입니다. </summary>
        public Button ResetButton;

        /// <summary>
        /// NRSDK가 현재 프레임에서 추적하기 시작한 새로운 평면을 저장하기 위한 리스트입니다.
        /// 이 객체는 매 프레임마다 할당을 피하기 위해 애플리케이션 전체에서 사용됩니다.
        /// </summary>
        private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

        /// <summary> 감지된 모든 평면 오브젝트를 저장하는 리스트입니다. </summary>
        private List<GameObject> m_DetectedPlaneObjects = new List<GameObject>();

        void Start()
        {
            // 버튼에 리스너를 추가하여 클릭 시 ResetPlanes 메서드를 호출하도록 합니다.
            ResetButton.onClick.AddListener(ResetPlanes);
        }

        /// <summary> 매 프레임마다 이 객체를 업데이트합니다. </summary>
        void Update()
        {
            // NRFrame을 통해 현재 프레임에서 새로 감지된 평면을 가져옵니다.
            NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);

            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // 평면 시각화를 위한 프리팹을 인스턴스화하고, 새로운 평면을 추적하도록 설정합니다.
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);

                // 감지된 평면 정보를 프리팹의 NRTrackableBehaviour 컴포넌트에 초기화합니다.
                planeObject.GetComponent<NRTrackableBehaviour>().Initialize(m_NewPlanes[i]);

                // 생성된 평면 오브젝트를 리스트에 추가합니다.
                m_DetectedPlaneObjects.Add(planeObject);
            }
        }

        /// <summary> 감지된 모든 평면을 초기화하고 삭제합니다. </summary>
        void ResetPlanes()
        {
            // 모든 감지된 평면 오브젝트를 삭제합니다.
            for (int i = 0; i < m_DetectedPlaneObjects.Count; i++)
            {
                Destroy(m_DetectedPlaneObjects[i]);
            }

            // 리스트를 초기화하여 다음 평면 감지에 영향을 미치지 않도록 합니다.
            m_DetectedPlaneObjects.Clear();
            m_NewPlanes.Clear();
        }
    }
}
