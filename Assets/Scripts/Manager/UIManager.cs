using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Robot
    public GameObject robotPrefab; 
    private GameObject robot;
    // wheel
    private KeyboardWheelControl wheelController;
    // camera
    private Camera[] cameras;
    private MouseCameraControl[] cameraControllers;
    private int cameraIndex;
    public float[] cameraFOV;
    private int cameraFOVIndex;
    public RenderTexture cameraRendertexture;
    
    // Scene
    public string mainScene;
    public GameObject[] level;
    public Vector3[] spawnPositions;
    public Vector3[] spawnRotations;
    private string[] tasks;
    public int levelIndex;
    public int taskIndex;
    // record
    private bool isRecording = false;
    public DataRecorder dataRecorder;

    // UI
    public GameObject[] UIs;
    private int UIIndex;
    public GameObject mainMenus;
    public GameObject loadingUI;
    public GameObject quitMenus;
    public GameObject allStateDisplay;
    private TextMeshProUGUI allStatePanelText;
    public GameObject cameraDisplay;
    public GameObject regularViewUI;
    public GameObject wideViewUI;
    public GameObject taskStatePanel;
    public GameObject robotStatePanel;
    private TextMeshProUGUI taskStatePanelText;
    private TextMeshProUGUI robotStatePanelText;
    public GameObject taskDropDown;
    public GameObject levelDropDown;
    public GameObject recordIconImage;

    // Other
    public GameObject minimap;

    void Start()
    {
        // Initialize indices
        cameraIndex = 0;
        cameraFOVIndex = 0;
        levelIndex = 0;
        taskIndex = 0;

        // 
        tasks = new string[] {"Human Following", "Passage", "Corner", 
                              "Passing Doors", "Exploration"};
        dataRecorder.updateData = true;
        allStateDisplay.SetActive(false);

        // UI
        UIs = new GameObject[] {mainMenus, loadingUI, quitMenus, 
                                allStateDisplay, cameraDisplay,
                                regularViewUI, wideViewUI};

        // Load menus
        loadMainMenus();
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
            if (robot != null && UIIndex != 0 && UIIndex != 1)
                ChangeRobotStateDisplay();
        if (Input.GetKeyDown(KeyCode.R))
            Record();

        // others
        if (Input.GetKeyDown(KeyCode.M))
            ChangeMinimapStatus();

        // system
        if (Input.GetKeyDown(KeyCode.Escape)) 
            if (UIIndex != 0 && UIIndex != 1)
                LoadQuitScene();


        // Info
        if (allStateDisplay.activeSelf)
        {
            updateAllStatePenal();
        }
        if (wideViewUI.activeSelf)
        {
            updateRobotUIStatePenal();
        }
    }
    
    // UI
    public void loadMainMenus()
    {
        UIIndex = 0;
        foreach (GameObject UI in UIs)
        {
            UI.SetActive(false);
        }
        UIs[UIIndex].SetActive(true);
    }
    private void loadLoading()
    {
        UIIndex = 1;
        foreach (GameObject UI in UIs)
        {
            UI.SetActive(false);
        }
        UIs[UIIndex].SetActive(true);
    }
    public void LoadQuitScene()
    {
        if (UIs[2].activeSelf)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
        
        UIs[UIIndex].SetActive(!UIs[2].activeSelf);
    }
    private void loadRobotUI()
    {
        foreach (GameObject UI in UIs)
        {
            UI.SetActive(false);
        }

        cameraDisplay.SetActive(true);

        if (cameraFOVIndex == 0)
        {
            UIIndex = 5;
            UIs[UIIndex].SetActive(true);
            UIs[UIIndex+1].SetActive(true);
        }
        else if (cameraFOVIndex == 1)
        {
            UIIndex = 6;
            UIs[UIIndex].SetActive(true);
        }

        taskStatePanelText = taskStatePanel.GetComponentInChildren<TextMeshProUGUI>();
        robotStatePanelText = robotStatePanel.GetComponentInChildren<TextMeshProUGUI>();
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

    // Public methods 
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

        RectTransform cameraDisplayRect = cameraDisplay.GetComponent<RectTransform>();
        cameraRendertexture.Release();
        if (cameraFOVIndex == 0)
        {
            cameraDisplayRect.sizeDelta = new Vector2 (1462, 823);
            cameraRendertexture.width = 1920;
            cameraRendertexture.height = 1080;
        }
        else
        {
            cameraDisplayRect.sizeDelta = new Vector2 (1920, 823);
            cameraRendertexture.width = 2560;
            cameraRendertexture.height = 1080;
        }
        cameras[cameraIndex].enabled = true;

        // Reload UI
        loadRobotUI();
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

    public void Record()
    {
        if (!isRecording)
        {
            dataRecorder.StartRecording();
            recordIconImage.SetActive(true);
        }
        else
        {
            dataRecorder.StopRecording();
            recordIconImage.SetActive(false);
        }

        isRecording = !isRecording;
    }

    // scene
    public void ChangeScene()
    {
        taskIndex = taskDropDown.GetComponent<TMP_Dropdown>().value;
        levelIndex = levelDropDown.GetComponent<TMP_Dropdown>().value;
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
        // Loading UI
        loadLoading();

        // mainScene
        SceneManager.LoadScene(mainScene);
        yield return new WaitForSeconds(0.5f);
        // level
        Instantiate(level[levelIndex], new Vector3(), new Quaternion());
        yield return new WaitForSeconds(1.0f); 
        // Robot
        SpawnRobot(spawnPositions[taskIndex], Quaternion.Euler(spawnRotations[taskIndex]));
        yield return new WaitForSeconds(2.0f); 
        
        // Back to camera
        loadRobotUI();
    }
    
    // others
    private void updateRobotUIStatePenal()
    {
        taskStatePanelText.text =
            "Task: \n\t" + tasks[taskIndex] + "\n" + 
            "\n" +
            "Level: \t" + "level " + string.Format("{0:0}", levelIndex+1);
        robotStatePanelText.text = 
            "x: \t\t" + string.Format("{0:0.00}", dataRecorder.states[1]) + "\n" +
            "y: \t\t" + string.Format("{0:0.00}", dataRecorder.states[2]) + "\n" + 
            "yaw: \t" + string.Format("{0:0.00}", dataRecorder.states[3]) + "\n" + 
            "Vx: \t\t" + string.Format("{0:0.00}", dataRecorder.states[4]) +  "\n" +  
            "Wz: \t\t" + string.Format("{0:0.00}", dataRecorder.states[5]) + "\n";
    }

    public void ChangeRobotStateDisplay()
    {
        allStatePanelText = allStateDisplay.GetComponentInChildren<TextMeshProUGUI>();
        allStateDisplay.SetActive(!allStateDisplay.activeSelf);
    }
    
    private void updateAllStatePenal()
    {
        allStatePanelText.text = 
            "time: \t" + string.Format("{0:0.00}", dataRecorder.states[0]) + "\n" + 
            "\n" +
            "x: \t\t" + string.Format("{0:0.00}", dataRecorder.states[1]) + "\n" +
            "y: \t\t" + string.Format("{0:0.00}", dataRecorder.states[2]) + "\n" + 
            "yaw: \t" + string.Format("{0:0.00}", dataRecorder.states[3]) + "\n" + 
            "Vx: \t\t" + string.Format("{0:0.00}", dataRecorder.states[4]) +  "\n" +  
            "Wz: \t\t" + string.Format("{0:0.00}", dataRecorder.states[5]) + "\n" + 
            "\n" +
            "min_dis_obs: \t\t" + string.Format("{0:0.00}", dataRecorder.states[6]) + "\n" + 
            "min_dis_obs_dir: \t" + string.Format("{0:0.00}", dataRecorder.states[7]) + "\n" + 
            "min_dis_h: \t\t" + string.Format("{0:0.00}", dataRecorder.states[8]) + "\n" + 
            "min_dis_h_dir: \t" + string.Format("{0:0.00}", dataRecorder.states[9]) + "\n" + 
            "\n" +
            "main_cam_yaw: \t" + string.Format("{0:0.00}", dataRecorder.states[10]) + "\n" + 
            "main_cam_yaw_vel: \t" + string.Format("{0:0.00}", dataRecorder.states[11]) + "\n" + 
            "main_cam_pitch: \t" + string.Format("{0:0.00}", dataRecorder.states[12]) + "\n" + 
            "main_cam_yaw_vel: \t" + string.Format("{0:0.00}", dataRecorder.states[13]) + "\n" +
            "arm_cam_yaw: \t\t" + string.Format("{0:0.00}", dataRecorder.states[14]) + "\n" + 
            "arm_cam_yaw_vel: \t" + string.Format("{0:0.00}", dataRecorder.states[15]) + "\n" + 
            "arm_cam_pitch: \t" + string.Format("{0:0.00}", dataRecorder.states[16]) + "\n" + 
            "arm_cam_yaw_vel: \t" + string.Format("{0:0.00}", dataRecorder.states[17]) + "\n" +
            "\n" +
            "collision_self_name: \n" + dataRecorder.collisions[0] + "\n" + 
            "collision_other_name: \n" + dataRecorder.collisions[1];
    }

    public void ChangeMinimapStatus()
    {
        if (minimap != null)
            minimap.SetActive(!minimap.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
