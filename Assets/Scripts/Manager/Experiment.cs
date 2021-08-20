using Random = System.Random;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;

public class Experiment : MonoBehaviour
{   
    public GameManager gameManager;
    public UIManager uIManager;

    public int testCameraView;
    public int testCameraFOV;
    public int testCameraMobility;
    private int[] testCamera;
    private int[,] cameraConfigurations;

    public int[] testTask;
    public int[] testLevel;
    public int[] testTrial;
    
    private int[] cameraView;
    private int[] cameraFOV;
    private int[] cameraMobility;

    private int[] taskIndices;
    private int[] levelIndices;
    private int[] trialIndices;

    private int experimentLength;
    private int currentIndex;
    private Random random;
    
    private bool moved;

    void Start()
    {
        // Procedure marker
        currentIndex = 0;

        // Predifined full configuration
        cameraConfigurations = new int [,] 
                               {{1, 1, 1}, {0, 1, 1}, {1, 0, 1}, {1, 1, 0}};
                               
        random = new Random();
    }

    void Update()
    {
        if (!moved && (Input.GetAxis("vertical")!=0 || Input.GetAxis("horizontal")!=0) )
            moved = true;
    }

    public void StartExperiment()
    {
        currentIndex = 0;
        gameManager.LoadSceneWithRobot(taskIndices[currentIndex], 
                                       levelIndices[currentIndex],
                                       trialIndices[currentIndex]);
        moved = false;
        StartCoroutine(StartRecordInAction());
    }

    public void NextLevel()
    {
        currentIndex += 1;
        if (currentIndex != experimentLength)
        {
            gameManager.LoadSceneWithRobot(taskIndices[currentIndex], 
                                           levelIndices[currentIndex],
                                           cameraConfigurations[currentIndex, 0],
                                           cameraConfigurations[currentIndex, 1],
                                           cameraConfigurations[currentIndex, 2]);
            moved = false;
            StartCoroutine(StartRecordInAction());
        }
        else
        {
            
        }
    }
    private IEnumerator StartRecordInAction()
    {
        yield return new WaitUntil(() => moved == true);
        gameManager.Record(currentIndex.ToString() + ": " + 
                           trialIndices[currentIndex].ToString() + " ");
    }

    public void SetExperimentConditions()
    {
        testCamera = new int[1 + testCameraView + testCameraFOV + testCameraMobility];
        testCamera[0] = 0;

        int i = 1;
        if (testCameraView == 1)
        {
            testCamera[i] = 1;
            i += 1;
        }
        if (testCameraFOV == 1)
        {
            testCamera[i] = 2;
            i += 1;
        }
        if (testCameraMobility == 1)
        {
            testCamera[i] = 3;
        }
    }
        
    private void CreateIndicesArray()
    {
        // temp
        SetExperimentConditions();

        // Intialization
        int testCameraLength = (1 + testCameraView + testCameraFOV + testCameraMobility);
        experimentLength = testCameraLength * testTask.Length * 
                           testLevel.Length * testTrial.Length;

        cameraView = new int[experimentLength];
        cameraFOV = new int[experimentLength];
        cameraMobility = new int[experimentLength];

        taskIndices = new int[experimentLength];
        levelIndices = new int[experimentLength];
        trialIndices = new int[experimentLength];

        // Extend
        int count = 0;
        // level
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int s = 0; s < testTrial.Length * testCameraLength * testTask.Length; ++s)
            {
                levelIndices[count] = testLevel[l];
                count += 1;
            }
        }
        // trial
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                for (int s = 0; s < testCameraLength * testTask.Length; ++s)
                {
                    trialIndices[count] = testTrial[tr];
                    count += 1;
                }
            }
        }
        // task
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                int[] randomTestTask = testTask.OrderBy(x => random.Next()).ToArray();
                for (int ta = 0; ta < testTask.Length; ++ta)
                {
                    for (int s = 0; s < testCameraLength; ++s)
                    {
                        taskIndices[count] = randomTestTask[ta];
                        count += 1;
                    }
                }
            }
        }
        // camera
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                for (int ta = 0; ta < testTask.Length; ++ta)
                {
                    int[] randomTestCamera = 
                          testCamera.OrderBy(x => random.Next()).ToArray();
                    for (int s = 0; s < testCameraLength; ++s)
                    {
                        taskIndices[count] = randomTestCamera[s];
                        count += 1;
                    }
                }
            }
        }
        currentIndex = 0;
    }
}