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

    public GameObject minimap;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Camera camera in cameras)
        {
            camera.targetDisplay = 0;
            camera.fieldOfView = 69.4f;
            camera.rect = new Rect(0f, 0.0f, 1.0f, 1.0f);
        }

        currentCameraIndex = 0;
        DisableAllCamera();
        cameras[currentCameraIndex].enabled = true;

        GetComponent<CameraJointController>().enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
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

        if (Input.GetKeyDown(KeyCode.Backspace))
            ChangeCameraControl();

        if (Input.GetKeyDown(KeyCode.M))
            ChangeMinimapStatus();

        if (Input.GetKeyDown(KeyCode.Escape)) 
            Application.Quit();
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

    public void ChangeCameraControl()
    {
        CameraJointController cr = GetComponent<CameraJointController>();
        cr.enabled = !cr.enabled;
        if (cr.enabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChangeMinimapStatus()
    {
        minimap.SetActive(!minimap.activeSelf);
    }
}
