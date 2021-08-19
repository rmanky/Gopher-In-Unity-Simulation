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

    // UI
    private GameObject[] UIs;
    private int UIIndex;
    // menus
    public GameObject mainMenus;
    public GameObject loadingUI;
    public GameObject quitMenus;
    // camera
    public GameObject cameraDisplay;
    private RectTransform cameraDisplayRect;
    public GameObject regularViewUI;
    public GameObject wideViewUI;
    // task & state
    public GameObject allStateDisplay;
    private TextMeshProUGUI allStatePanelText;
    public GameObject taskStatePanel;
    public GameObject robotStatePanel;
    private TextMeshProUGUI taskStatePanelText;
    private TextMeshProUGUI robotStatePanelText;

    // Scene
    public GameObject taskDropDown;
    public GameObject levelDropDown;

    // Data
    private DataRecorder dataRecorder;
    public GameObject recordIconImage;
    
    // Others
    public GameObject miniMap;

    void Start()
    {
        // UI
        UIs = new GameObject[] {mainMenus, loadingUI, quitMenus, 
                                regularViewUI, wideViewUI,
                                cameraDisplay, allStateDisplay};
        cameraDisplayRect = cameraDisplay.GetComponent<RectTransform>();

        // Data
        dataRecorder = gameManager.dataRecorder;

        // Text to update
        allStatePanelText = allStateDisplay.GetComponentInChildren<TextMeshProUGUI>();
        taskStatePanelText = taskStatePanel.GetComponentInChildren<TextMeshProUGUI>();
        robotStatePanelText = robotStatePanel.GetComponentInChildren<TextMeshProUGUI>();

        // Load menus
        loadMainMenus();
    }

    void Update()
    {
        // Hotkeys
        // info
        if (Input.GetKeyDown(KeyCode.I))
            if (UIIndex != 0 && UIIndex != 1)
                ChangeAllStateDisplay();
        // miniMap
        if (Input.GetKeyDown(KeyCode.M))
            ChangeMinimapStatus();

        // system
        if (Input.GetKeyDown(KeyCode.Escape)) 
            if (UIIndex != 0 && UIIndex != 1)
                LoadQuitScene();

        // Update panel
        if (allStateDisplay.activeSelf)
            updateAllStatePenal();
        if (wideViewUI.activeSelf)
            updateRobotUIStatePenal();
    }
    
    // UIs
    public void loadMainMenus()
    {
        UIIndex = 0;
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        UIs[UIIndex].SetActive(true);
    }

    private IEnumerator loadLoading()
    {
        UIIndex = 1;
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        UIs[UIIndex].SetActive(true);

        yield return new WaitForSeconds(3.0f);

        loadRobotUI();
    }

    public void LoadQuitScene()
    {
        if (UIs[2].activeSelf)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;

        UIs[UIIndex].SetActive(!UIs[2].activeSelf);
    }
    public void Quit()
    {
        gameManager.Quit();
    }

    public void loadRobotUI()
    {
        foreach (GameObject UI in UIs)
            UI.SetActive(false);

        cameraDisplay.GetComponent<RawImage>().texture = 
            gameManager.cameraRenderTextures[gameManager.cameraFOVIndex];
        cameraDisplay.SetActive(true);
        if (gameManager.cameraFOVIndex == 0)
        {
            UIIndex = 3;
            UIs[UIIndex].SetActive(true);
            UIs[UIIndex+1].SetActive(true);
            cameraDisplayRect.sizeDelta = new Vector2 (1462, 823);
        }
        else if (gameManager.cameraFOVIndex == 1)
        {
            UIIndex = 4;
            UIs[UIIndex].SetActive(true);
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

    // Scene
    public void ChangeScene()
    {
        int taskIndex = taskDropDown.GetComponent<TMP_Dropdown>().value;
        int levelIndex = levelDropDown.GetComponent<TMP_Dropdown>().value;
        gameManager.LoadSceneWithRobot(taskIndex, levelIndex);
        
        StartCoroutine(loadLoading());
    }
    
    // Update panels
    private void updateRobotUIStatePenal()
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
}
