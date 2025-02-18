#if UNITY_ANDROID && !UNITY_EDITOR
#define OVR_ANDROID_MRC
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRMixedRealityCaptureTest : MonoBehaviour {

	bool inited = false;

	enum CameraMode
	{
		Normal = 0,
		OverrideFov,
		ThirdPerson,
	}

	CameraMode currentMode = CameraMode.Normal;

	public Camera defaultExternalCamera;
	OVRPlugin.Fovf defaultFov;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || OVR_ANDROID_MRC
		if (!defaultExternalCamera)
		{
			Debug.LogWarning("defaultExternalCamera undefined");
		}

#if !OVR_ANDROID_MRC
		// On Quest, we enable MRC automatically through the configuration
		if (!OVRManager.instance.enableMixedReality)
		{
			OVRManager.instance.enableMixedReality = true;
		}
#endif
#endif
	}

	void Initialize()
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || OVR_ANDROID_MRC
		if (inited)
			return;

#if OVR_ANDROID_MRC
		if (!OVRPlugin.Media.GetInitialized())
			return;
#else
		if (!OVRPlugin.IsMixedRealityInitialized())
			return;
#endif

		OVRPlugin.ResetDefaultExternalCamera();
		Debug.LogFormat("GetExternalCameraCount before adding manual external camera {0}", OVRPlugin.GetExternalCameraCount());
		UpdateDefaultExternalCamera();
		Debug.LogFormat("GetExternalCameraCount after adding manual external camera {0}", OVRPlugin.GetExternalCameraCount());

		// obtain default FOV
		{
			OVRPlugin.CameraIntrinsics cameraIntrinsics;
			OVRPlugin.CameraExtrinsics cameraExtrinsics;
			OVRPlugin.Posef calibrationRawPose;
			OVRPlugin.GetMixedRealityCameraInfo(0, out cameraExtrinsics, out cameraIntrinsics, out calibrationRawPose);
			defaultFov = cameraIntrinsics.FOVPort;
		}

		inited = true;
#endif
	}

	void UpdateDefaultExternalCamera()
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || OVR_ANDROID_MRC
		// always build a 1080p external camera
		const int cameraPixelWidth = 1920;
		const int cameraPixelHeight = 1080;
		const float cameraAspect = (float)cameraPixelWidth / cameraPixelHeight;


		string cameraName = "UnityExternalCamera";
		OVRPlugin.CameraIntrinsics cameraIntrinsics = new OVRPlugin.CameraIntrinsics();
		OVRPlugin.CameraExtrinsics cameraExtrinsics = new OVRPlugin.CameraExtrinsics();

		// intrinsics

		cameraIntrinsics.IsValid = OVRPlugin.Bool.True;
		cameraIntrinsics.LastChangedTimeSeconds = Time.time;

		float vFov = defaultExternalCamera.fieldOfView * Mathf.Deg2Rad;
		float hFov = Mathf.Atan(Mathf.Tan(vFov * 0.5f) * cameraAspect) * 2.0f;
		OVRPlugin.Fovf fov = new OVRPlugin.Fovf();
		fov.UpTan = fov.DownTan = Mathf.Tan(vFov * 0.5f);
		fov.LeftTan = fov.RightTan = Mathf.Tan(hFov * 0.5f);

		cameraIntrinsics.FOVPort = fov;
		cameraIntrinsics.VirtualNearPlaneDistanceMeters = defaultExternalCamera.nearClipPlane;
		cameraIntrinsics.VirtualFarPlaneDistanceMeters = defaultExternalCamera.farClipPlane;
		cameraIntrinsics.ImageSensorPixelResolution.w = cameraPixelWidth;
		cameraIntrinsics.ImageSensorPixelResolution.h = cameraPixelHeight;

		// extrinsics

		cameraExtrinsics.IsValid = OVRPlugin.Bool.True;
		cameraExtrinsics.LastChangedTimeSeconds = Time.time;
		cameraExtrinsics.CameraStatusData = OVRPlugin.CameraStatus.CameraStatus_Calibrated;
		cameraExtrinsics.AttachedToNode = OVRPlugin.Node.None;

		Camera mainCamera = Camera.main;
		OVRCameraRig cameraRig = mainCamera.GetComponentInParent<OVRCameraRig>();
		if (cameraRig)
		{
			Transform trackingSpace = cameraRig.trackingSpace;
			OVRPose trackingSpacePose = trackingSpace.ToOVRPose(false);
			OVRPose cameraPose = defaultExternalCamera.transform.ToOVRPose(false);
			OVRPose relativePose = trackingSpacePose.Inverse() * cameraPose;
			cameraExtrinsics.RelativePose = relativePose.ToPosef();
		}
		else
		{
			cameraExtrinsics.RelativePose = OVRPlugin.Posef.identity;
		}

		if (!OVRPlugin.SetDefaultExternalCamera(cameraName, ref cameraIntrinsics, ref cameraExtrinsics))
		{
			Debug.LogError("SetDefaultExternalCamera() failed");
		}
#endif
	}

	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || OVR_ANDROID_MRC
		if (!inited)
		{
			Initialize();
			return;
		}

		if (!defaultExternalCamera)
		{
			return;
		}

#if OVR_ANDROID_MRC
		if (!OVRPlugin.Media.GetInitialized())
		{
			return;
		}
#else
		if (!OVRPlugin.IsMixedRealityInitialized())
		{
			return;
		}
#endif

		if (OVRInput.GetDown(OVRInput.Button.One))
		{
			if (currentMode == CameraMode.ThirdPerson)
			{
				currentMode = CameraMode.Normal;
			}
			else
			{
				currentMode = currentMode + 1;
			}

			Debug.LogFormat("Camera mode change to {0}", currentMode);
		}
		
		if (currentMode == CameraMode.Normal)
		{
			UpdateDefaultExternalCamera();
			OVRPlugin.OverrideExternalCameraFov(0, false, new OVRPlugin.Fovf());
			OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);
		}
		else if (currentMode == CameraMode.OverrideFov)
		{
			OVRPlugin.Fovf fov = defaultFov;
			OVRPlugin.Fovf newFov = new OVRPlugin.Fovf();
			newFov.LeftTan = fov.LeftTan * 2.0f;
			newFov.RightTan = fov.RightTan * 2.0f;
			newFov.UpTan = fov.UpTan * 2.0f;
			newFov.DownTan = fov.DownTan * 2.0f;

			OVRPlugin.OverrideExternalCameraFov(0, true, newFov);
			OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);

			if (!OVRPlugin.GetUseOverriddenExternalCameraFov(0))
			{
				Debug.LogWarning("FOV not overridden");
			}
		}
		else if (currentMode == CameraMode.ThirdPerson)
		{
			Camera camera = GetComponent<Camera>();
			if (camera == null)
			{
				return;
			}

			float vFov = camera.fieldOfView * Mathf.Deg2Rad;
			float hFov = Mathf.Atan(Mathf.Tan(vFov * 0.5f) * camera.aspect) * 2.0f;
			OVRPlugin.Fovf fov = new OVRPlugin.Fovf();
			fov.UpTan = fov.DownTan = Mathf.Tan(vFov * 0.5f);
			fov.LeftTan = fov.RightTan = Mathf.Tan(hFov * 0.5f);
			OVRPlugin.OverrideExternalCameraFov(0, true, fov);

			Camera mainCamera = Camera.main;
			OVRCameraRig cameraRig = mainCamera.GetComponentInParent<OVRCameraRig>();
			if (cameraRig)
			{
				Transform trackingSpace = cameraRig.trackingSpace;
				OVRPose trackingSpacePose = trackingSpace.ToOVRPose(false);
				OVRPose cameraPose = transform.ToOVRPose(false);
				OVRPose relativePose = trackingSpacePose.Inverse() * cameraPose;
				OVRPlugin.Posef relativePosef = relativePose.ToPosef();
				OVRPlugin.OverrideExternalCameraStaticPose(0, true, relativePosef);
			}
			else
			{
				OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);
			}

			if (!OVRPlugin.GetUseOverriddenExternalCameraFov(0))
			{
				Debug.LogWarning("FOV not overridden");
			}

			if (!OVRPlugin.GetUseOverriddenExternalCameraStaticPose(0))
			{
				Debug.LogWarning("StaticPose not overridden");
			}
		}
#endif
	}
}
