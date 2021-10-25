using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private CameraView[] cameraViews;

    [System.Serializable]
    public class CameraView
    {
        public Transform cameraLocation;
        public KeyCode cameraKey;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (CameraView view in cameraViews)
        {
            if (Input.GetKeyDown(view.cameraKey))
            {
                camera.transform.parent = view.cameraLocation;
                camera.transform.position = view.cameraLocation.position;
                camera.transform.rotation = view.cameraLocation.rotation;
            }
        }
    }
}
