using NRKernal.Record;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
#if UNITY_ANDROID && !UNITY_EDITOR
    using GalleryDataProvider = NativeGalleryDataProvider;
#else
    using GalleryDataProvider = MockGalleryDataProvider;
#endif
    /// <summary> A video capture 2 local example. </summary>
    [HelpURL("https://developer.xreal.com/develop/unity/video-capture")]
    public class VideoCapture2LocalExample : MonoBehaviour
    {
        [SerializeField] private Button m_PlayButton;
        [SerializeField] private NRPreviewer m_Previewer;
        [SerializeField] private Slider m_SliderMic;
        [SerializeField] private Text m_TextMic;
        [SerializeField] private Slider m_SliderApp;
        [SerializeField] private Text m_TextApp;

        public BlendMode blendMode = BlendMode.Blend;
        public ResolutionLevel resolutionLevel;
        public LayerMask cullingMask = -1;
        public NRVideoCapture.AudioState audioState = NRVideoCapture.AudioState.None; // 오디오 상태를 None으로 고정
        public bool useGreenBackGround = false;
        public long MaxFileSizeInBytes = 100 * 1024 * 1024; // 100MB

        public enum ResolutionLevel
        {
            High_1920p,
            Default_1280p,
            Low_640p,
        }

        void Update()
        {
            if (m_VideoCapture != null && m_VideoCapture.IsRecording)
            {
                string videoFilePath = VideoSavePath; // 현재 비디오 파일 경로
                FileInfo fileInfo = new FileInfo(videoFilePath);

                if (fileInfo.Exists && fileInfo.Length > MaxFileSizeInBytes)
                {
                    StopVideoCapture(); // 파일 크기가 초과되면 녹화 중지
                    Debug.LogWarning("Recording stopped because the file size exceeded the limit.");
                }
            }
        }

        public string VideoSavePath
        {
            get
            {
                string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
                string filename = string.Format("Xreal_Record_{0}.mp4", timeStamp);
                return Path.Combine(Application.persistentDataPath, filename);
            }
        }

        GalleryDataProvider galleryDataTool;

        void Awake()
        {
            if (m_SliderMic != null)
            {
                m_SliderMic.maxValue = 5.0f;
                m_SliderMic.minValue = 0.1f;
                m_SliderMic.value = 1;
                m_SliderMic.onValueChanged.AddListener(OnSlideMicValueChange);
            }

            if (m_SliderApp != null)
            {
                m_SliderApp.maxValue = 5.0f;
                m_SliderApp.minValue = 0.1f;
                m_SliderApp.value = 1;
                m_SliderApp.onValueChanged.AddListener(OnSlideAppValueChange);
            }

            RefreshUIState();
        }

        void OnSlideMicValueChange(float val)
        {
            // 오디오 상태가 None이므로 조정할 필요 없음
            RefreshUIState();
        }

        void OnSlideAppValueChange(float val)
        {
            // 오디오 상태가 None이므로 조정할 필요 없음
            RefreshUIState();
        }

        NRVideoCapture m_VideoCapture = null;
        void CreateVideoCapture(Action callback)
        {
            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                NRDebugger.Info("Created VideoCapture Instance!");
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;
                    callback?.Invoke();
                }
                else
                {
                    NRDebugger.Error("Failed to create VideoCapture Instance!");
                }
            });
        }

        public void OnClickPlayButton()
        {
            if (m_VideoCapture == null)
            {
                CreateVideoCapture(() =>
                {
                    StartVideoCapture();
                });
            }
            else if (m_VideoCapture.IsRecording)
            {
                this.StopVideoCapture();
            }
            else
            {
                this.StartVideoCapture();
            }
        }

        void RefreshUIState()
        {
            bool flag = m_VideoCapture == null || !m_VideoCapture.IsRecording;
            m_PlayButton.GetComponent<Image>().color = flag ? Color.red : Color.green;

            if (m_TextMic != null && m_SliderMic != null)
                m_TextMic.text = m_SliderMic.value.ToString();
            if (m_TextApp != null && m_SliderApp != null)
                m_TextApp.text = m_SliderApp.value.ToString();
        }

        public void StartVideoCapture()
        {
            if (m_VideoCapture == null || m_VideoCapture.IsRecording)
            {
                NRDebugger.Warning("Can not start video capture!");
                return;
            }

            CameraParameters cameraParameters = new CameraParameters();
            Resolution cameraResolution = GetResolutionByLevel(resolutionLevel);
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.frameRate = cameraResolution.refreshRate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.PNG;
            cameraParameters.blendMode = blendMode;
            cameraParameters.audioState = NRVideoCapture.AudioState.None; // 오디오 기록 없음

            m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);
        }

        private Resolution GetResolutionByLevel(ResolutionLevel level)
        {
            Resolution resolution = new Resolution();
            switch (level)
            {
                case ResolutionLevel.High_1920p:
                    resolution.width = 1920;
                    resolution.height = 1080;
                    resolution.refreshRate = 30;
                    break;
                case ResolutionLevel.Default_1280p:
                    resolution.width = 1280;
                    resolution.height = 720;
                    resolution.refreshRate = 30;
                    break;
                case ResolutionLevel.Low_640p:
                    resolution.width = 640;
                    resolution.height = 360;
                    resolution.refreshRate = 30;
                    break;
                default:
                    break;
            }
            return resolution;
        }

        public void StopVideoCapture()
        {
            if (m_VideoCapture == null || !m_VideoCapture.IsRecording)
            {
                NRDebugger.Warning("Can not stop video capture!");
                return;
            }

            NRDebugger.Info("Stop Video Capture!");
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            m_Previewer.SetData(null, false);
        }

        void OnStartedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Started Video Capture Mode failed!");
                return;
            }

            NRDebugger.Info("Started Video Capture Mode!");
            m_VideoCapture.StartRecordingAsync(VideoSavePath, OnStartedRecordingVideo);
            m_Previewer.SetData(m_VideoCapture.PreviewTexture, true);
        }

        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Started Recording Video Failed!");
                return;
            }

            NRDebugger.Info("Started Recording Video!");
            if (useGreenBackGround)
            {
                m_VideoCapture.GetContext().GetBehaviour().SetBackGroundColor(Color.green);
            }
            m_VideoCapture.GetContext().GetBehaviour().SetCameraMask(cullingMask.value);

            RefreshUIState();
        }

        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Stopped Recording Video Failed!");
                return;
            }

            NRDebugger.Info("Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        }

        void OnStoppedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Video Capture Mode!");
            RefreshUIState();

            var encoder = m_VideoCapture.GetContext().GetEncoder() as VideoEncoder;
            string path = encoder.EncodeConfig.outPutPath;
            string filename = string.Format("Xreal_Shot_Video_{0}.mp4", NRTools.GetTimeStamp().ToString());

            StartCoroutine(DelayInsertVideoToGallery(path, filename, "Record"));

            m_VideoCapture.Dispose();
            m_VideoCapture = null;
        }

        void OnDestroy()
        {
            m_VideoCapture?.Dispose();
            m_VideoCapture = null;
        }

        IEnumerator DelayInsertVideoToGallery(string originFilePath, string displayName, string folderName)
        {
            yield return new WaitForSeconds(0.1f);
            InsertVideoToGallery(originFilePath, displayName, folderName);
        }

        public void InsertVideoToGallery(string originFilePath, string displayName, string folderName)
        {
            NRDebugger.Info("InsertVideoToGallery: {0}, {1} => {2}", displayName, originFilePath, folderName);
            if (galleryDataTool == null)
            {
                galleryDataTool = new GalleryDataProvider();
            }

            galleryDataTool.InsertVideo(originFilePath, displayName, folderName);
        }
    }
}
