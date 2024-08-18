using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI ��ư�� ����ϱ� ���� �߰�

namespace NRKernal.NRExamples
{
    /// <summary> ��� ������ �����ϰ� �ʱ�ȭ�� �� �ִ� Ŭ�����Դϴ�. </summary>
    public class PlaneDetectionController : MonoBehaviour
    {
        /// <summary> ������ ��鿡 ���� �������Դϴ�. </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary> ��� �ʱ�ȭ ��ư�Դϴ�. </summary>
        public Button ResetButton;

        /// <summary>
        /// NRSDK�� ���� �����ӿ��� �����ϱ� ������ ���ο� ����� �����ϱ� ���� ����Ʈ�Դϴ�.
        /// �� ��ü�� �� �����Ӹ��� �Ҵ��� ���ϱ� ���� ���ø����̼� ��ü���� ���˴ϴ�.
        /// </summary>
        private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

        /// <summary> ������ ��� ��� ������Ʈ�� �����ϴ� ����Ʈ�Դϴ�. </summary>
        private List<GameObject> m_DetectedPlaneObjects = new List<GameObject>();

        void Start()
        {
            // ��ư�� �����ʸ� �߰��Ͽ� Ŭ�� �� ResetPlanes �޼��带 ȣ���ϵ��� �մϴ�.
            ResetButton.onClick.AddListener(ResetPlanes);
        }

        /// <summary> �� �����Ӹ��� �� ��ü�� ������Ʈ�մϴ�. </summary>
        void Update()
        {
            // NRFrame�� ���� ���� �����ӿ��� ���� ������ ����� �����ɴϴ�.
            NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);

            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // ��� �ð�ȭ�� ���� �������� �ν��Ͻ�ȭ�ϰ�, ���ο� ����� �����ϵ��� �����մϴ�.
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);

                // ������ ��� ������ �������� NRTrackableBehaviour ������Ʈ�� �ʱ�ȭ�մϴ�.
                planeObject.GetComponent<NRTrackableBehaviour>().Initialize(m_NewPlanes[i]);

                // ������ ��� ������Ʈ�� ����Ʈ�� �߰��մϴ�.
                m_DetectedPlaneObjects.Add(planeObject);
            }
        }

        /// <summary> ������ ��� ����� �ʱ�ȭ�ϰ� �����մϴ�. </summary>
        void ResetPlanes()
        {
            // ��� ������ ��� ������Ʈ�� �����մϴ�.
            for (int i = 0; i < m_DetectedPlaneObjects.Count; i++)
            {
                Destroy(m_DetectedPlaneObjects[i]);
            }

            // ����Ʈ�� �ʱ�ȭ�Ͽ� ���� ��� ������ ������ ��ġ�� �ʵ��� �մϴ�.
            m_DetectedPlaneObjects.Clear();
            m_NewPlanes.Clear();
        }
    }
}
