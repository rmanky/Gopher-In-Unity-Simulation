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
    public RenderTexture[] cameraRenderTextures;
    public RenderTexture regularCameraRendertexture;
    public RenderTexture wideCameraRendertexture;
    
    // Scene
    public string mainScene;
    public GameObject[] level;
    public Vector3[] spawnPositions;
    public Vector3[] spawnRotations;
    public string[] tasks;
    public int levelIndex;
    public int taskIndex;
    private int trialIndex;

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
        cameraRenderTextures = new RenderTexture[] {regularCameraRendertexture, 
                                                    wideCameraRendertexture};

        // Scene
        levelIndex = 0;
        taskIndex = 0;
        trialIndex = 0;
        tasks = new string[] {"Human Following", "Passage", "Corner", 
                              "Passing Doors", "Exploration"};
    }

    void Update()
    {
        // Hotkeys
        if (robot != null)
        {
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
        
        dataRecorder.setRobot(robot);
        dataRecorder.updateData = true;
    }

    private void InitializeCameras()
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            camera.fieldOfView = cameraFOV[cameraIndex];
            camera.targetTexture = cameraRenderTextures[cameraIndex];     
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
            camera.enabled = false;
            camera.fieldOfView = cameraFOV[cameraFOVIndex];
            camera.targetTexture = cameraRenderTextures[cameraFOVIndex];
        }
        cameras[cameraIndex].enabled = true;

        // Reload UI
        uIManager.loadRobotUI();
    }

    public void ChangeCameraControl()
    {
        MouseCameraControl cameraControl = cameraControllers[cameraIndex];
        cameraControl.enabled = !cameraControl.enabled;
        if (cameraControl.enabled)
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
            string indexNumber = cameraIndex + "." + 
                                 cameraFOVIndex + "." +
                                 (cameraControllers[cameraIndex].enabled? 1:0) + "." +
                                 "; " +
                                 taskIndex + "." +
                                 levelIndex + "." +
                                 trialIndex;
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