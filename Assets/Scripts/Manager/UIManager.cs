using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject robotPrefab; 
    private GameObject robot;

    private Camera[] cameras;
    private int currentCameraIndex;
    public float[] cameraFOV;
    private int cameraFOVIndex;
    private CameraJointController[] cameraControllers;

    public string[] scenes;
    public Vector3[] spawnPositions;
    public Vector3[] spawnRotations;
    private int sceneIndex;
    private int taskIndex;

    private bool isRecording = false;

    public GameObject minimap;

    void Start()
    {
        // Initialize indices
        currentCameraIndex = 0;
        cameraFOVIndex = 0;
        sceneIndex = 0;
        taskIndex = 0;

        // Load scene
        LoadSceneWithRobot();
    }

    void Update()
    {
        // Hotkeys
        // camera
        if (Input.GetKeyDown(KeyCode.Tab)) 
            ChangeCameraView();
        if (Input.GetKeyDown(KeyCode.V))
            ChangeCameraFOV();
        if (Input.GetKeyDown(KeyCode.LeftControl))
            ChangeCameraControl();
        
        // state
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ChangeRobotSpeed();
        if (Input.GetKeyDown(KeyCode.I)) 
            ChangeRobotStateDisplay();
        if (Input.GetKeyDown(KeyCode.R))
            Record();

        // others
        if (Input.GetKeyDown(KeyCode.M))
            ChangeMinimapStatus();

        // system
        if (Input.GetKeyDown(KeyCode.Escape)) 
            Application.Quit();
    }

    private void SpawnRobot(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Spawn
        robot = Instantiate(robotPrefab, spawnPosition, spawnRotation);

        /*/ Initialization
        cameras = robot.GetComponentsInChildren<Camera>();
        cameraControllers = robot.GetComponentsInChildren<CameraJointController>();
        InitializeCameras();*/
    }

    private void InitializeCameras()
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
            camera.targetDisplay = 0;
            camera.fieldOfView = cameraFOV[currentCameraIndex];
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }
        cameras[currentCameraIndex].enabled = true;

        foreach (CameraJointController controller in cameraControllers)
            controller.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Public methods 
    // camera
    public void ChangeCameraView()
    {
        cameras[currentCameraIndex].enabled = false;

        currentCameraIndex = (currentCameraIndex+1) % cameras.Length;
        cameras[currentCameraIndex].enabled = true;
    }

    public void ChangeCameraFOV()
    {
        cameraFOVIndex = (cameraFOVIndex+1) % cameraFOV.Length;
        foreach (Camera camera in cameras)
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
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

    // state
    public void ChangeRobotSpeed()
    {
        
    }

    public void ChangeRobotStateDisplay()
    {
        
    }

    public void Record()
    {
        if (!isRecording)
        {

        }
        else
        {

        }
    }

    // scene
    public void ChangeScene()
    {
        LoadSceneWithRobot();
    }
    
    public void LoadSceneWithRobot()
    {
        // Keep this manager
        DontDestroyOnLoad(gameObject);
        // Load scene
        StartCoroutine(LoadSceneWithRobotCoroutine(0.5f));
        // Loading sign
    }
    private IEnumerator LoadSceneWithRobotCoroutine(float seconds)
    {
        // Scene
        SceneManager.LoadScene(scenes[sceneIndex]);
        // Sleep
        yield return new WaitForSeconds(seconds); 
        // Robot
        SpawnRobot(spawnPositions[taskIndex], Quaternion.Euler(spawnRotations[taskIndex]));
        // Sleep
    }

    // others
    public void ChangeMinimapStatus()
    {
        if (minimap != null)
            minimap.SetActive(!minimap.activeSelf);
    }
}
