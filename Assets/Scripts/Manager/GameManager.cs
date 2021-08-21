using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    // UI
    public UIManager uIManager;
    public bool isExperimenting;

    // Robot
    public GameObject robotPrefab; 
    private GameObject robot;
    // wheel
    public KeyboardWheelControl wheelController;
    // camera
    private Camera[] cameras;
    private MouseCameraControl[] cameraControllers;
    public int cameraIndex;
    private float[] cameraFOV;
    public int cameraFOVIndex;
    public RenderTexture[] cameraRenderTextures;
    public RenderTexture regularCameraRendertexture;
    public RenderTexture wideCameraRendertexture;
    public int cameraMobility;
    private int desiredFrameRate = 30;
    
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
    public bool isRecording = false;
    
    void Start()
    {
        // Robot
        // camera
        cameraIndex = 0;
        cameraFOVIndex = 0;
        cameraMobility = 0;
        cameraFOV = new float[] {69.4f, 91.1f};
        cameraRenderTextures = new RenderTexture[] {regularCameraRendertexture, 
                                                    wideCameraRendertexture};

        // Scene
        levelIndex = 0;
        taskIndex = 0;
        tasks = new string[] {"Human Following", "Passage", "Corner", 
                              "Passing Doors", "Exploration"};
    }

    void Update()
    {
        // Hotkeys
        if (robot != null)
        {
            // camera
            if (!isExperimenting)
            {
                if (Input.GetKeyDown(KeyCode.Tab)) 
                    ChangeCameraView();
                if (Input.GetKeyDown(KeyCode.V))
                    ChangeCameraFOV();
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    ChangeCameraControl();
            }

            // wheel
            if (Input.GetKeyDown(KeyCode.LeftShift))
                ChangeRobotSpeed();

            // record
            if (Input.GetKeyDown(KeyCode.R) && !isExperimenting)
                Record();
        }
    }

    public void ReloadScene()
    {
        LoadSceneWithRobot(taskIndex, levelIndex, cameraIndex, cameraFOVIndex, cameraMobility);
    }

    public void LoadSceneWithRobot(int taskIndex, int levelIndex,
                                   int cameraIndex=0, int cameraFOVIndex=0, int cameraMobility=0)
    {   
        // Load task
        this.taskIndex = taskIndex;
        this.levelIndex = levelIndex;

        // Load camera configuration
        this.cameraIndex = cameraIndex;
        this.cameraFOVIndex = cameraFOVIndex;
        this.cameraMobility = cameraMobility;

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
        SpawnRobot();
    }

    private void SpawnRobot()
    {
        // Spawn
        robot = Instantiate(robotPrefab, spawnPositions[taskIndex], 
                                         Quaternion.Euler(spawnRotations[taskIndex]));

        // Get components
        wheelController = robot.GetComponentInChildren<KeyboardWheelControl>();
        cameras = robot.GetComponentsInChildren<Camera>();
        cameraControllers = robot.GetComponentsInChildren<MouseCameraControl>();

        // Initialization
        InitializeCameras();
        
        // Set up data recorder
        StartCoroutine(InitializeDataRecorder());
    }

    private IEnumerator InitializeDataRecorder()
    {
        dataRecorder.setRobot(robot);
        yield return new WaitForSeconds(0.5f);
        dataRecorder.updateData = true;
    }

    private void InitializeCameras()
    {
        // Viewpoint + FOV
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
            camera.targetTexture = cameraRenderTextures[cameraFOVIndex];     
        }

        InvokeRepeating("CameraRender", 0f, 1f/desiredFrameRate);

        // Camera control
        foreach (MouseCameraControl controller in cameraControllers)
            controller.enabled = false;

        cameraControllers[cameraIndex].enabled = (cameraMobility == 1);
        if ((cameraMobility == 1))
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }
    private void CameraRender()
    {
        cameras[cameraIndex].Render();
    }

    // UI - camera
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
            camera.enabled = false;
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
            camera.targetTexture = cameraRenderTextures[cameraFOVIndex];
        }
        cameras[cameraIndex].enabled = true;

        // reload UI
        uIManager.LoadRobotUI();
    }

    public void ChangeCameraControl()
    {
        bool mobility = (cameraMobility == 1);

        foreach (MouseCameraControl controller in cameraControllers)
            controller.enabled = false;

        cameraControllers[cameraIndex].enabled = mobility;
        if (mobility)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }

    // UI - wheel
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
    public void Record(string filePrefix = "")
    {
        if (!isRecording)
        {   
            string indexNumber = filePrefix + "" +
                                 cameraIndex + "." + 
                                 cameraFOVIndex + "." +
                                 (cameraControllers[cameraIndex].enabled? 1:0) + "." +
                                 "; " +
                                 taskIndex + "." +
                                 levelIndex + ".";
            dataRecorder.StartRecording(indexNumber);
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