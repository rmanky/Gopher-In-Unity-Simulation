using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Camera[] cameras;
    private int currentCameraIndex;

    public InputField cameraFOVInputField;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Camera camera in cameras)
        {
            camera.targetDisplay = 0;
            camera.fieldOfView = 69.4f;
        }

        currentCameraIndex = 0;
        DisableAllCamera();
        cameras[currentCameraIndex].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            DisableAllCamera();
            currentCameraIndex = (currentCameraIndex+1) % cameras.Length;
            cameras[currentCameraIndex].enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
    }

    void DisableAllCamera()
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }
    }

    public void ChangeCameraFOV()
    {
        float.TryParse(cameraFOVInputField.text, out float fov);
        foreach (Camera camera in cameras)
        {
            camera.fieldOfView = fov;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
