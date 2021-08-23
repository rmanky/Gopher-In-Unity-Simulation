using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Game manager
    public GameManager gameManager;
    public Experiment experiment;

    // UI
    private GameObject[] UIs;
    private int UIIndex;
    // menus
    public GameObject mainMenus;
    public GameObject loadingUI;
    public GameObject quitMenus;
    private CursorLockMode previousCursorState;
    // camera
    public GameObject cameraDisplay;
    private RectTransform cameraDisplayRect;
    public GameObject regularViewUI;
    public GameObject wideViewUI;
    public GameObject experimentUI;
    // task & state
    public GameObject allStateDisplay;
    private TextMeshProUGUI allStatePanelText;
    public GameObject taskStatePanel;
    public GameObject robotStatePanel;
    public GameObject experimentTaskPanel;
    public GameObject experimentStatePanel;
    public GameObject messagePanel;
    private TextMeshProUGUI taskStatePanelText;
    private TextMeshProUGUI robotStatePanelText;
    private TextMeshProUGUI experimentTaskPanelText;
    private TextMeshProUGUI experimentStatePanelText;
    private TextMeshProUGUI messagePanelText;
    
    private string[] taskMessage;


    // Scene
    public GameObject taskDropDown;
    public GameObject levelDropDown;

    // Data
    private DataRecorder dataRecorder;
    public GameObject recordIconImage;
    
    // Others
    public GameObject helpDisplay;
    public GameObject miniMap;

    // Experiment
    public GameObject levelCompleteUI;
    public GameObject questionFillUI;
    public GameObject questionTextUI;
    public GameObject experimentConditionUI;
    public GameObject NumberBoardAnswerUI;
    public GameObject NumberBoardAnswerField;

    void Start()
    {
        // UI
        UIs = new GameObject[] {mainMenus, loadingUI, experimentConditionUI, 
                                experimentUI, wideViewUI, regularViewUI,
                                quitMenus,
                                cameraDisplay, allStateDisplay,
                                levelCompleteUI, questionFillUI, questionTextUI};
        cameraDisplayRect = cameraDisplay.GetComponent<RectTransform>();

        // Data
        dataRecorder = gameManager.dataRecorder;

        // Text to update
        allStatePanelText = allStateDisplay.GetComponentInChildren<TextMeshProUGUI>();
        taskStatePanelText = taskStatePanel.GetComponentInChildren<TextMeshProUGUI>();
        robotStatePanelText = robotStatePanel.GetComponentInChildren<TextMeshProUGUI>();
        experimentTaskPanelText = experimentTaskPanel.GetComponentInChildren<TextMeshProUGUI>();
        experimentStatePanelText = experimentStatePanel.GetComponentInChildren<TextMeshProUGUI>();
        messagePanelText = messagePanel.GetComponentInChildren<TextMeshProUGUI>();

        // Experiment help
        taskMessage = new string[] {
                        "Please follow the nurse until you reach the shining circle.", 
                        "Please go straight and reach the shining circle.",
                        "Please go straight and take a left turn and reach the shining circle.",
                        "Please pass the door, enter the room and reach the shining circle.",
                        "Please find the number boards inside the room and sum the numbers."};
        // Load menus
        LoadMainMenus();

        // Update panel
        InvokeRepeating("UpdateAllStatePenal", 1.0f, 0.1f);
        InvokeRepeating("UpdateRobotUIStatePenal", 1.0f, 0.1f);
        InvokeRepeating("UpdateExperimentPanel", 1.0f, 0.1f);
    }

    void Update()
    {
        // Hotkeys
        // info
        if (Input.GetKeyDown(KeyCode.I))
            if (UIIndex != 0 && UIIndex != 1 && UIIndex != 2)
                ChangeAllStateDisplay();
        // miniMap
        if (Input.GetKeyDown(KeyCode.M))
            ChangeMinimapStatus();

        // system
        if (Input.GetKeyDown(KeyCode.Escape)) 
            if (UIIndex != 0 && UIIndex != 1 && UIIndex != 2)
                LoadQuitScene();
            
        if (Input.GetKeyDown(KeyCode.H)) 
            if (UIIndex != 0 && UIIndex != 1 && UIIndex != 2)
                ChangeHelpDisplay();
    }
    
    // UIs
    public void LoadMainMenus()
    {
        Time.timeScale = 1f;

        UIIndex = 0;
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        UIs[UIIndex].SetActive(true);
    }

    private IEnumerator LoadLoading()
    {
        UIIndex = 1;
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        UIs[UIIndex].SetActive(true);

        yield return new WaitForSeconds(3.0f);

        LoadRobotUI();
    }

    public void LoadExperiment()
    {
        UIIndex = 2;
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        UIs[UIIndex].SetActive(true);
    }

    public void StartExperiment()
    {
        Toggle[] toggles = experimentConditionUI.GetComponentsInChildren<Toggle>();
        bool[] conditions = new bool[toggles.Length];

        for (int i = 0; i < toggles.Length; ++i)
            conditions[i] = toggles[i].isOn;
        // Experiment condition
        experiment.SetExperimentConditions(conditions);

        // Start
        experiment.StartExperiment();
        StartCoroutine(LoadLoading());
    }

    public void LoadNextLevelUI()
    {
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.Confined;

        foreach (GameObject UI in UIs)
            UI.SetActive(false);
        UIs[9].SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1.0f;

        experiment.NextLevel();
        StartCoroutine(LoadLoading());
    }

    public void LoadQuitScene()
    {
        if (UIs[6].activeSelf)
        {
            Cursor.lockState = previousCursorState;
            Time.timeScale = 1f;
        }
        else
        {
            previousCursorState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0f;

            NumberBoardAnswerUI.SetActive(gameManager.taskIndex == 4);
        }

        UIs[6].SetActive(!UIs[6].activeSelf);
    }
    public void Quit()
    {
        gameManager.Quit();
    }

    public void LoadRobotUI()
    {
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        cameraDisplay.GetComponent<RawImage>().texture = 
            gameManager.cameraRenderTextures[gameManager.cameraFOVIndex];
        cameraDisplay.SetActive(true);

        if(gameManager.isExperimenting)
            UIs[3].SetActive(true);
        else
            UIs[4].SetActive(true);
        if (gameManager.cameraFOVIndex == 0)
        {
            UIIndex = 5;
            UIs[UIIndex].SetActive(true);
            cameraDisplayRect.sizeDelta = new Vector2 (1462, 823);
        }
        else if (gameManager.cameraFOVIndex == 1)
        {
            UIIndex = 4;
            cameraDisplayRect.sizeDelta = new Vector2 (1920, 823);
        }
    }

    // additional
    public void ChangeAllStateDisplay()
    {
        allStateDisplay.SetActive(!allStateDisplay.activeSelf);
    }

    public void ChangeMinimapStatus()
    {
        if (miniMap != null)
            miniMap.SetActive(!miniMap.activeSelf);
    }

    public void ChangeHelpDisplay()
    {
        helpDisplay.SetActive(!helpDisplay.activeSelf);
    }

    public void PopMessage(string message)
    {
        StartCoroutine(PopMessageCoroutine(message));
    }
    private IEnumerator PopMessageCoroutine(string message)
    {
        messagePanel.SetActive(true);
        messagePanelText.text = message;
        yield return new WaitForSeconds(1.5f);
        messagePanel.SetActive(false);
    }

    // Scene
    public void ChangeScene()
    {
        StartCoroutine(LoadLoading());
        
        int taskIndex = taskDropDown.GetComponent<TMP_Dropdown>().value;
        int levelIndex = levelDropDown.GetComponent<TMP_Dropdown>().value;
        gameManager.LoadSceneWithRobot(taskIndex, levelIndex);
    }

    public void Reload()
    {
        // In quit scene
        Time.timeScale = 1f;

        StartCoroutine(LoadLoading());

        if (gameManager.isExperimenting)
        {
            experiment.ReloadLevel();
        }
        else
        {
            gameManager.ReloadScene();
        }
    }

    // task
    public void CheckNumberBoardAnswer()
    {
        int answer;
        if(int.TryParse(NumberBoardAnswerField.GetComponent<TMP_InputField>().text, out answer))
            gameManager.CheckNumberBoardAnswer(answer);
    }
    
    // Update panels
    private void UpdateExperimentPanel()
    {   
        if (experimentUI.activeSelf)
        {
            experimentTaskPanelText.text =
                "Task: \n" + 
                "\t" + gameManager.tasks[gameManager.taskIndex] + "\n" + 
                "Level: \n" + 
                "\tlevel " + string.Format("{0:0}", gameManager.levelIndex+1) + "\n" +
                "FPS: " + string.Format("{0:0}", (1.0f/Time.deltaTime));
            experimentStatePanelText.text =
                taskMessage[gameManager.taskIndex] +  "\n" + 
                "Try not to hit any obstacles." + "\n\n" + 
                "Speed: \t" + string.Format("{0:0.0}", gameManager.GetRobotSpeed());
        }
    }

    private void UpdateRobotUIStatePenal()
    {
        if (wideViewUI.activeSelf)
        {
            taskStatePanelText.text =
                "Task: \n\t" + gameManager.tasks[gameManager.taskIndex] + "\n" + 
                "\n" +
                "Level: \t" + "level " + string.Format("{0:0}", gameManager.levelIndex+1);
            robotStatePanelText.text = 
                "x: \t\t" + string.Format("{0:0.00}", dataRecorder.states[1]) + "\n" +
                "y: \t\t" + string.Format("{0:0.00}", dataRecorder.states[2]) + "\n" + 
                "yaw: \t" + string.Format("{0:0.00}", dataRecorder.states[3]) + "\n" + 
                "Vx: \t\t" + string.Format("{0:0.00}", dataRecorder.states[4]) +  "\n" +  
                "Wz: \t\t" + string.Format("{0:0.00}", dataRecorder.states[5]) + "\n";
        }
    }

    private void UpdateAllStatePenal()
    {
        if (allStateDisplay.activeSelf)
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
    }
}