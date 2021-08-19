using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    // UI
    public UIManager uIManager;

    // Robot
    public GameObject robotPrefab; 
    private GameObject robot;
    // wheel
    private KeyboardWheelControl wheelController;
    // camera
    private Camera[] cameras;
    private MouseCameraControl[] cameraControllers;
    public int cameraIndex;
    private float[] cameraFOV;
    public int cameraFOVIndex;
    public RenderTexture cameraRendertexture;

    // Scene
    public string mainScene;
    public GameObject[] level;
    public Vector3[] spawnPositions;
    public Vector3[] spawnRotations;
    public string[] tasks;
    public int levelIndex;
    public int taskIndex;

    // Data
    public DataRecorder dataRecorder;
    private bool isRecording = false;
    
    void Start()
    {
        // Robot
        // camera
        cameraIndex = 0;
        cameraFOVIndex = 0;
        cameraFOV = new float[] {69.4f, 91.1f};

        // Scene
        levelIndex = 0;
        taskIndex = 0;
        tasks = new string[] {"Human Following", "Passage", "Corner", 
                              "Passing Doors", "Exploration"};

        // Data
        dataRecorder.updateData = true;
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
        if (Input.GetKeyDown(KeyCode.R))
            Record();
    }

    public void LoadSceneWithRobot(int taskIndex, int levelIndex)
    {   
        this.taskIndex = taskIndex;
        this.levelIndex = levelIndex;

        // Keep this manager
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(uIManager);
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
        yield return new WaitForSeconds(0.5f); 
        // Robot
        SpawnRobot(spawnPositions[taskIndex], Quaternion.Euler(spawnRotations[taskIndex]));
    }

    private void SpawnRobot(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Spawn
        robot = Instantiate(robotPrefab, spawnPosition, spawnRotation);

        // Get components
        wheelController = robot.GetComponentInChildren<KeyboardWheelControl>();
        cameras = robot.GetComponentsInChildren<Camera>();
        cameraControllers = robot.GetComponentsInChildren<MouseCameraControl>();

        // Initialization
        InitializeCameras();
        dataRecorder.setRobot(robot);
        dataRecorder.updateData = true;
    }

    private void InitializeCameras()
    {
        cameraRendertexture.width = 1920;
        cameraRendertexture.height = 1080;

        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
            camera.targetTexture = cameraRendertexture;
            camera.fieldOfView = cameraFOV[cameraIndex];
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }
        cameras[cameraIndex].enabled = true;

        foreach (MouseCameraControl controller in cameraControllers)
            controller.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // camera
    public void ChangeCameraView()
    {
        cameras[cameraIndex].enabled = false;
        // TOFIX
        // cameraControllers[cameraIndex].HomeCameraJoints();
        bool mobility = cameraControllers[cameraIndex].enabled;
        cameraControllers[cameraIndex].enabled = false;

        cameraIndex = (cameraIndex+1) % cameras.Length;

        cameras[cameraIndex].enabled = true;
        cameraControllers[cameraIndex].enabled = mobility;
    }

    public void ChangeCameraFOV()
    {
        cameraFOVIndex = (cameraFOVIndex+1) % cameraFOV.Length;
        foreach (Camera camera in cameras)
        {
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
            camera.enabled = false;
        }

        cameraRendertexture.Release();
        if (cameraFOVIndex == 0)
        {
            cameraRendertexture.width = 1920;
            cameraRendertexture.height = 1080;
        }
        else
        {
            cameraRendertexture.width = 2560;
            cameraRendertexture.height = 1080;
        }
        cameras[cameraIndex].enabled = true;

        // Reload UI
        uIManager.loadRobotUI();
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

    // wheel
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

    // Data
    public void Record()
    {
        if (!isRecording)
        {
            dataRecorder.StartRecording();
            uIManager.recordIconImage.SetActive(true);
        }
        else
        {
            dataRecorder.StopRecording();
            uIManager.recordIconImage.SetActive(false);
        }

        isRecording = !isRecording;
    }

    // Other
    public void Quit()
    {
        Application.Quit();
    }
}