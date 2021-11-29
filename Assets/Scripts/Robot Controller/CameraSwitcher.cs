using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public enum CameraLocation {Head, Left, Right};

    [SerializeField]
    private Camera mainCamera;
    
    [SerializeField]
    private Transform headCameraLocation;

    [SerializeField]
    private Transform leftCameraLocation;

    [SerializeField]
    private Transform rightCameraLocation;

    private void Start()
    {
        SetCameraLocation(headCameraLocation);
    }

    private void SetCameraLocation(Transform targetTransform) {
        mainCamera.transform.parent = targetTransform;
        mainCamera.transform.position = targetTransform.position;
        mainCamera.transform.rotation = targetTransform.rotation;
    }

    public void SetCamera(CameraLocation cameraLocation) {
        if (cameraLocation == CameraLocation.Head) {
            SetCameraLocation(headCameraLocation);
        } else if (cameraLocation == CameraLocation.Left) {
            SetCameraLocation(leftCameraLocation);
        } else if (cameraLocation == CameraLocation.Right) {
            SetCameraLocation(rightCameraLocation);
        }
    }
}
