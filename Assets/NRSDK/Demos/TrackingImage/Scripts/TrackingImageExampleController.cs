using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TrackingImageExampleController : MonoBehaviour
    {
        public TrackingImageVisualizer TrackingImageVisualizerPrefab;
        public GameObject FitToScanOverlay;

        private Dictionary<int, TrackingImageVisualizer> m_Visualizers = new Dictionary<int, TrackingImageVisualizer>();
        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        // 사용자가 착용한 Nreal 기기(카메라)의 Transform
        private Transform cameraTransform;

        // CSV 파일 저장 경로
        private string csvFilePath;

        // 1초마다 갱신을 위한 타이머
        private float timer = 0.0f;
        private float updateInterval = 1.0f;  // 1초 간격으로 갱신

        void Start()
        {
            // 카메라 Transform 초기화 (Nreal HMD의 카메라)
            cameraTransform = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform;

            // CSV 파일 저장 경로 설정 (다운로드 폴더)
#if UNITY_ANDROID
            csvFilePath = GetAndroidDownloadPath("relative_positions.csv");
#elif UNITY_IOS
            csvFilePath = Path.Combine(Application.persistentDataPath, "relative_positions.csv");
#else
            csvFilePath = Path.Combine(Application.persistentDataPath, "relative_positions.csv");
#endif

            Debug.Log($"CSV File Path: {csvFilePath}");

            // CSV 파일에 헤더 추가
            if (!File.Exists(csvFilePath))
            {
                using (StreamWriter sw = File.CreateText(csvFilePath))
                {
                    sw.WriteLine("Time,CameraPosX,CameraPosY,CameraPosZ");
                }
            }
        }

        void Update()
        {
            // 이미지 추적 처리
            TrackImages();

            // 1초마다 좌표를 갱신
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0.0f;
                SaveCameraPosition();
            }
        }

        // 이미지 추적을 담당하는 메서드
        private void TrackImages()
        {
#if !UNITY_EDITOR
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return;
            }
#endif
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            foreach (var image in m_TempTrackingImages)
            {
                TrackingImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);

                if (image.GetTrackingState() != TrackingState.Stopped && visualizer == null)
                {
                    NRDebugger.Info("새로운 TrackingImageVisualizer를 생성합니다!");
                    visualizer = (TrackingImageVisualizer)Instantiate(TrackingImageVisualizerPrefab, image.GetCenterPose().position, image.GetCenterPose().rotation);
                    visualizer.Image = image;
                    visualizer.transform.parent = transform;
                    m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);
                }
                else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.GetDataBaseIndex());
                    Destroy(visualizer.gameObject);
                }

                FitToScanOverlay.SetActive(false);
            }
        }

        // 카메라(Nreal 기기)의 좌표를 CSV 파일에 저장
        private void SaveCameraPosition()
        {
            // Nreal 기기(카메라)의 좌표 가져오기
            Vector3 cameraPosition = cameraTransform.position;

            // 현재 시간 가져오기
            string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");  // 시간에 밀리초까지 포함

            // CSV 파일에 카메라 좌표와 시간 저장
            using (StreamWriter sw = File.AppendText(csvFilePath))
            {
                string newLine = $"{currentTime},{cameraPosition.x:F2},{cameraPosition.y:F2},{cameraPosition.z:F2}";
                sw.WriteLine(newLine);
            }

            Debug.Log($"Camera Position Saved: Time={currentTime}, Position={cameraPosition}");
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

        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        public void DisableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }
    }
}
