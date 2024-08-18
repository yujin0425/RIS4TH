using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

namespace NRKernal
{
    public class NRLaserReticle : MonoBehaviour
    {
        public enum ReticleState
        {
            Hide,
            Normal,
            Hover,
        }

        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        [SerializeField]
        private GameObject m_DefaultVisual;
        [SerializeField]
        private GameObject m_HoverVisual;

        private GameObject m_HitTarget;
        private bool m_IsVisible = true;

        public float defaultDistance = 2.5f;
        public float reticleSizeRatio = 0.02f;

        private Transform m_CameraRoot
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        public GameObject HitTarget
        {
            get
            {
                return m_HitTarget;
            }
        }

        [SerializeField]
        private Text coordinateText;

        private string csvFilePath;
        private Vector3 initialPosition = Vector3.zero;
        private bool isTrackingInitialPosition = false; // 초기 위치 추적 여부
        private float positionThreshold = 0.03f; // 3cm 오차 범위
        private float hoverTimeThreshold = 2.0f; // 2초 동안 고정될 때 저장
        private float hoverTime = 0.0f;

        private void Awake()
        {
            SwitchReticleState(ReticleState.Hide);
            defaultDistance = Mathf.Clamp(defaultDistance, m_Raycaster.NearDistance, m_Raycaster.FarDistance);

#if UNITY_ANDROID
            string downloadPath = GetAndroidDownloadPath("HoverPositions.csv");
#elif UNITY_IOS
            string downloadPath = Path.Combine(Application.persistentDataPath, "HoverPositions.csv");  // iOS에서의 파일 경로
#else
            string downloadPath = Path.Combine(Application.persistentDataPath, "HoverPositions.csv");  // 다른 플랫폼에서는 Persistent Data Path 사용
#endif
            csvFilePath = downloadPath;

            // CSV 파일이 없으면 생성
            if (!File.Exists(csvFilePath))
            {
                using (StreamWriter sw = File.CreateText(csvFilePath))
                {
                    sw.WriteLine("Time,Position X,Position Y,Position Z");
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (!m_IsVisible || !NRInput.ReticleVisualActive)
            {
                SwitchReticleState(ReticleState.Hide);
                return;
            }

            var result = m_Raycaster.FirstRaycastResult();
            var points = m_Raycaster.BreakPoints;
            var pointCount = points.Count;

            if (result.isValid)
            {
                SwitchReticleState(ReticleState.Hover);
                transform.position = result.worldPosition;
                transform.rotation = Quaternion.LookRotation(result.worldNormal, m_Raycaster.transform.forward);

                m_HitTarget = result.gameObject;

                Vector3 currentPosition = result.worldPosition;

                // Hover 상태에서 좌표를 Unity UI Text에 표시
                if (coordinateText != null)
                {
                    coordinateText.text = $"POS : ({currentPosition.x:F2}, {currentPosition.y:F2}, {currentPosition.z:F2})";
                }

                // 초기 위치 추적 시작
                if (!isTrackingInitialPosition)
                {
                    initialPosition = currentPosition;
                    isTrackingInitialPosition = true;
                    hoverTime = 0.0f;
                }

                // 오차 범위 내에서 머물렀을 때
                if (Vector3.Distance(currentPosition, initialPosition) <= positionThreshold)
                {
                    hoverTime += Time.deltaTime;

                    if (hoverTime >= hoverTimeThreshold)
                    {
                        SavePositionToCSV(currentPosition);
                        isTrackingInitialPosition = false; // 초기 위치 추적 리셋
                    }
                }
                else
                {
                    // 오차 범위를 벗어나면 초기 위치 재설정
                    initialPosition = currentPosition;
                    hoverTime = 0.0f;
                }
            }
            else
            {
                SwitchReticleState(ReticleState.Normal);
                if (pointCount != 0)
                {
                    transform.localPosition = Vector3.forward * defaultDistance;
                    transform.localRotation = Quaternion.identity;
                }

                m_HitTarget = null;
                hoverTime = 0.0f;  // 타이머 리셋
                isTrackingInitialPosition = false; // 초기 위치 추적 리셋
            }

            if (m_CameraRoot)
            {
                transform.localScale = Vector3.one * reticleSizeRatio * (transform.position - m_CameraRoot.transform.position).magnitude;
            }
        }

        private void OnDisable()
        {
            SwitchReticleState(ReticleState.Hide);
        }

        private void SwitchReticleState(ReticleState state)
        {
            switch (state)
            {
                case ReticleState.Hide:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Normal:
                    m_DefaultVisual.SetActive(true);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Hover:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void SetVisible(bool isVisible)
        {
            this.m_IsVisible = isVisible;
        }

        private void SavePositionToCSV(Vector3 position)
        {
            using (StreamWriter sw = File.AppendText(csvFilePath))
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sw.WriteLine($"{time},{position.x:F2},{position.y:F2},{position.z:F2}");
            }
        }

        private string GetAndroidDownloadPath(string fileName)
        {
            using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                using (AndroidJavaObject downloadsDir = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", environment.GetStatic<string>("DIRECTORY_DOWNLOADS")))
                {
                    return Path.Combine(downloadsDir.Call<string>("getAbsolutePath"), fileName);
                }
            }
        }
    }
}
