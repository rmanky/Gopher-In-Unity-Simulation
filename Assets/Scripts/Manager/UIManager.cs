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
    private KeyboardWheelControl wheelController;

    private Camera[] cameras;
    private MouseCameraControl[] cameraControllers;
    private int cameraIndex;
    public float[] cameraFOV;
    private int cameraFOVIndex;
   
    public string mainScene;
    public GameObject[] level;
    public Vector3[] spawnPositions;
    public Vector3[] spawnRotations;
    public int levelIndex;
    public int taskIndex;

    private bool isRecording = false;

    public GameObject minimap;

    void Start()
    {
        // Initialize indices
        cameraIndex = 0;
        cameraFOVIndex = 0;
        levelIndex = 0;
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

        // Get components
        wheelController = robot.GetComponentInChildren<KeyboardWheelControl>();
        cameras = robot.GetComponentsInChildren<Camera>();
        cameraControllers = robot.GetComponentsInChildren<MouseCameraControl>();
        Debug.Log(cameras.Length + " " + cameraControllers.Length);
        // Initialization
        InitializeCameras();
    }

    private void InitializeCameras()
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
            camera.targetDisplay = 0;
            camera.fieldOfView = cameraFOV[cameraIndex];
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }
        cameras[cameraIndex].enabled = true;

        foreach (MouseCameraControl controller in cameraControllers)
            controller.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Public methods 
    // camera
    public void ChangeCameraView()
    {
        cameras[cameraIndex].enabled = false;

        cameraIndex = (cameraIndex+1) % cameras.Length;
        cameras[cameraIndex].enabled = true;
    }

    public void ChangeCameraFOV()
    {
        cameraFOVIndex = (cameraFOVIndex+1) % cameraFOV.Length;
        foreach (Camera camera in cameras)
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
    }

    public void ChangeCameraControl()
    {
        MouseCameraControl cr = cameraControllers[cameraIndex];
        cr.enabled = !cr.enabled;
        if (cr.enabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }

    // state
    public void ChangeRobotSpeed()
    {
        if (wheelController.speed == 1.5f)
        {
            wheelController.speed = 1.0f;
            wheelController.angularSpeed = 1.0f;
        }
        else
        {
            wheelController.speed = 1.5f;
            wheelController.angularSpeed = 1.5f;
        }
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
        StartCoroutine(LoadSceneWithRobotCoroutine());
        // Loading sign
    }
    private IEnumerator LoadSceneWithRobotCoroutine()
    {
        // mainScene
        SceneManager.LoadScene(mainScene);
        yield return new WaitForSeconds(0.5f);
        // level
        Instantiate(level[levelIndex], new Vector3(), new Quaternion());
        yield return new WaitForSeconds(1.0f); 
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
